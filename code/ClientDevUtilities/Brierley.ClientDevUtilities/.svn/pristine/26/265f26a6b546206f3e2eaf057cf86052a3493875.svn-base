using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class ConsumeLoyaltyCurrencyAsPayment : OperationProviderBase
    {
        private const string _className = "ConsumeLoyaltyCurrencyAsPayment";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        // Input parm keys
        private const string _inToCurrency = "ToCurrency";
        private const string _inPointsToConsume = "PointsToConsume";
        private const string _inCurrencyValue = "CurrencyValue";
        private const string _inCartTotal = "CartTotal";

        public ConsumeLoyaltyCurrencyAsPayment() : base("ConsumeLoyaltyCurrencyAsPayment") { }

        public override string Invoke(string source, string parms)
        {
            string methodName = "Invoke";
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                    throw new LWOperationInvocationException("No parameters provided for consuming loyalty currency as payment") { ErrorCode = 2999 };

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string toCurrency = args.ContainsKey(_inToCurrency) ? args[_inToCurrency].ToString() : null;
                decimal? points = args.ContainsKey(_inPointsToConsume) ? decimal.Parse(args[_inPointsToConsume].ToString()) : (decimal?)null;
                decimal? currencyValue = args.ContainsKey(_inCurrencyValue) ? decimal.Parse(args[_inCurrencyValue].ToString()) : (decimal?)null;
                decimal? cartTotal = args.ContainsKey(_inCartTotal) ? decimal.Parse(args[_inCartTotal].ToString()) : (decimal?)null;

                string sAssignCert = FunctionProviderParms["AssignLWCertificate"];
                bool assignCert = false;
                bool.TryParse(sAssignCert, out assignCert);

                string emailName = FunctionProviderParms["LowCertEmailName"];
                string emailRecipient = FunctionProviderParms["LowCertEmailRecipient"];


                if (!points.HasValue && !currencyValue.HasValue)
                    throw new LWOperationInvocationException(string.Format("Either {0} or {1} is required but neither was provided", _inPointsToConsume, _inCurrencyValue))
                    { ErrorCode = 3001 };
                if (points.HasValue && currencyValue.HasValue)
                    throw new LWOperationInvocationException(string.Format("Either {0} or {1} is required but both were provided", _inPointsToConsume, _inCurrencyValue))
                    { ErrorCode = 3011 };

                Member member = LoadMember(args);

                RewardDef reward = ContentService.GetRewardDefForExchange(member);
                if (reward == null)
                    throw new LWOperationInvocationException("No reward definition found for the member's tier") { ErrorCode = 3119 };
                if (!reward.ConversionRate.HasValue)
                    throw new LWOperationInvocationException("No conversion rate defined on the reward definition for the member's tier") { ErrorCode = 3121 };

                var config = LWConfigurationUtil.GetCurrentConfiguration().FWConfig;
                string defaultCurrency = config.GetFWConfigProperty(FWConfig.LCAPDefaultCurrency);
                if (string.IsNullOrEmpty(defaultCurrency))
                    throw new LWOperationInvocationException("A default currency must be configured") { ErrorCode = 3109 };

                // Get the exchange rate
                decimal exchangeRate = 1;
                if (!string.IsNullOrEmpty(toCurrency) && !string.Equals(defaultCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
                {
                    ExchangeRate exchange = ContentService.GetExchangeRate(defaultCurrency.ToUpper(), toCurrency.ToUpper());
                    if (exchange == null)
                        throw new LWOperationInvocationException(string.Format("Currency mapping not found: {0} -> {1}", defaultCurrency.ToUpper(), toCurrency.ToUpper()))
                        { ErrorCode = 3019 };
                    string sMaxAge = config.GetFWConfigProperty(FWConfig.LCAPMaxExchangeRateAge);
                    if (!string.IsNullOrEmpty(sMaxAge))
                    {
                        double maxAge;
                        if (!double.TryParse(sMaxAge, out maxAge))
                            throw new LWOperationInvocationException(string.Format("Maximum exchange rate age value is set but is not a valid number: {0}", sMaxAge))
                            { ErrorCode = 3023 };
                        if (maxAge > 0 && (exchange.UpdateDate ?? exchange.CreateDate).AddDays(maxAge) < DateTime.Now)
                            throw new LWOperationInvocationException(string.Format("Exchange rate data is too old. Maximum days age: {0}. Last updated: {1}", sMaxAge, (exchange.UpdateDate ?? exchange.CreateDate)))
                            { ErrorCode = 3037 };
                    }
                    exchangeRate = exchange.Rate;
                }
                else
                    toCurrency = defaultCurrency;

                // points * exchangeRate * conversionRate = currencyValue
                if (points.HasValue)
                    currencyValue = points.Value * exchangeRate * reward.ConversionRate.Value;
                else
                    points = currencyValue.Value / (exchangeRate * reward.ConversionRate.Value);

                // Validate restrictions
                ValidateConfiguredRestrictions(config, points.Value, currencyValue.Value, cartTotal, exchangeRate);

                RewardRuleUtil _util = new RewardRuleUtil() { MarkAsRedeemed = true, AssignLWCertificate = assignCert, LowThresholdEmailName = emailName, LowThresholdEmailRecepient = emailRecipient };
                decimal pointsBalance = _util.GetPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), member, null);
                decimal pointsOnHold = _util.GetOnHoldPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), member, null);

                if (points.Value > pointsBalance - pointsOnHold)
                    throw new LWOperationInvocationException(string.Format("Insufficient member points balance: {1}. Points requested: {0}",
                        points.Value, pointsBalance - pointsOnHold)) { ErrorCode = 3041 };

                // Create order/reward and consume points
                using (var txn = LoyaltyDataService.StartTransaction())
                {
                    ContextObject contextObject = new ContextObject();
                    contextObject.Owner = member;
                    string certNumber = string.Empty;
                    long memberRewardId = _util.IssueRewardCertificate(contextObject, member, reward, null, RewardFulfillmentOption.Electronic, null, ref certNumber, 0, string.Empty, string.Empty, string.Empty,
                        points, defaultCurrency.ToUpper(), toCurrency.ToUpper(), reward.ConversionRate, exchangeRate, currencyValue, cartTotal);
                    _util.ConsumePoints(member, null, reward, memberRewardId, null, new IssueRewardRuleResult(), points);
                    txn.Complete();
                    APIArguments resultParams = new APIArguments();
                    //Member reward id
                    resultParams.Add("MemberRewardId", memberRewardId);
                    //Member remaining balance
                    resultParams.Add("RemainingBalance", pointsBalance - pointsOnHold - points.Value);
                    //Certificate number
                    resultParams.Add("CertificateNumber", certNumber);
                    resultParams.Add("PointsConsumed", points.Value);
                    resultParams.Add("CurrencyValue", currencyValue.Value);
                    response = SerializationUtils.SerializeResult(Name, Config, resultParams);

                    #region Post Processing
                    Dictionary<string, object> context = new Dictionary<string, object>();
                    context.Add("member", member);
                    context.Add("reward", reward);
                    PostProcessSuccessfullInvocation(context);
                    #endregion

                    _util.CheckLowCertificateThreshold(member, reward);
                    return response;
                }
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, ex.Message, ex);
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }

        protected override void Cleanup()
        {
        }

        private void ValidateConfiguredRestrictions(FWConfig config, decimal pointsSpent, decimal currencyValue, decimal? cartTotal, decimal exchangeRate)
        {
            string minPoints = config.GetFWConfigProperty(FWConfig.LCAPMinPoints);
            string maxPoints = config.GetFWConfigProperty(FWConfig.LCAPMaxPoints);
            string minMoney = config.GetFWConfigProperty(FWConfig.LCAPMinMoney);
            string maxMoney = config.GetFWConfigProperty(FWConfig.LCAPMaxMoney);
            string minPercent = config.GetFWConfigProperty(FWConfig.LCAPMinPercent);
            string maxPercent = config.GetFWConfigProperty(FWConfig.LCAPMaxPercent);

            if (!IsValueInRestriction(minPoints, pointsSpent, true))
                throw new LWOperationInvocationException(string.Format("Insufficient points spend: {0}. Minimum points: {1}", pointsSpent, minPoints))
                { ErrorCode = 3049 };
            if (!IsValueInRestriction(maxPoints, pointsSpent, false))
                throw new LWOperationInvocationException(string.Format("Points spend maximum exceeded: {0}. Maximum points: {1}", pointsSpent, maxPoints))
                { ErrorCode = 3061 };
            if (!IsValueInRestriction(minMoney, currencyValue, true, exchangeRate))
                throw new LWOperationInvocationException(string.Format("Insufficient currency spend: {0}. Minimum currency: {1}", currencyValue, decimal.Parse(minMoney) * exchangeRate))
                { ErrorCode = 3067 };
            if (!IsValueInRestriction(maxMoney, currencyValue, false, exchangeRate))
                throw new LWOperationInvocationException(string.Format("Currency spend maximum exceeded: {0}. Maximum currency: {1}", currencyValue, decimal.Parse(maxMoney) * exchangeRate))
                { ErrorCode = 3079 };
            if (cartTotal.HasValue)
            {
                if (!IsValueInRestriction(minPercent, currencyValue / cartTotal.Value, true, isPercent: true))
                    throw new LWOperationInvocationException(string.Format("Insufficient cart total requested: {0}. Minimum percent: {1}", currencyValue / cartTotal.Value, minPercent))
                    { ErrorCode = 3083 };
                if (!IsValueInRestriction(maxPercent, currencyValue / cartTotal.Value, false, isPercent: true))
                    throw new LWOperationInvocationException(string.Format("Cart total maximum exceeded: {0}. Maximum percent: {1}", currencyValue / cartTotal.Value, maxPercent))
                    { ErrorCode = 3089 };
            }
        }

        private bool IsValueInRestriction(string restriction, decimal value, bool greater, decimal multiplier = 1, bool isPercent = false)
        {
            if (string.IsNullOrEmpty(restriction))
                return true;

            decimal restrictionValue;
            if (!decimal.TryParse(restriction, out restrictionValue))
                throw new LWOperationInvocationException(string.Format("Invalid restriction value, not a number: {0}", restriction));

            if(isPercent && (restrictionValue > 1 || restrictionValue < 0))
                throw new LWOperationInvocationException(string.Format("Invalid percent restriction, not a fractional value: {0}", restriction));

            if (greater)
                return value >= (restrictionValue * multiplier);
            else
                return value <= (restrictionValue * multiplier);
        }
    }
}
