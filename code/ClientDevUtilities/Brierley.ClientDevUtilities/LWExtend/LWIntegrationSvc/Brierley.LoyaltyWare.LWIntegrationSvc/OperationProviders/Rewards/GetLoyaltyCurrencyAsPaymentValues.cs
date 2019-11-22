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
    public class GetLoyaltyCurrencyAsPaymentValues : OperationProviderBase
    {
        private const string _className = "GetLoyaltyCurrencyAsPaymentValues";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        private RewardRuleUtil _util = new RewardRuleUtil();

        public GetLoyaltyCurrencyAsPaymentValues() : base("GetLoyaltyCurrencyAsPaymentValues") { }

        public override string Invoke(string source, string parms)
        {
            string methodName = "Invoke";
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for getting the loyalty currency as payment values") { ErrorCode = 2999 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string currency = args.ContainsKey("ToCurrency") ? args["ToCurrency"].ToString() : null;
                string language = args.ContainsKey("Language") ? args["Language"].ToString() : LanguageChannelUtil.GetDefaultCulture();
                string channel = args.ContainsKey("Channel") ? args["Channel"].ToString() : LanguageChannelUtil.GetDefaultChannel();

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

                decimal pointsBalance = _util.GetPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), member, null);
                decimal pointsOnHold = _util.GetOnHoldPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), member, null);

                decimal exchangeRate = 1;
                if (!string.IsNullOrEmpty(currency) && !string.Equals(defaultCurrency, currency, StringComparison.OrdinalIgnoreCase))
                {
                    ExchangeRate exchange = ContentService.GetExchangeRate(defaultCurrency.ToUpper(), currency.ToUpper());
                    if (exchange == null)
                        throw new LWOperationInvocationException(string.Format("Currency mapping not found: {0} -> {1}", defaultCurrency.ToUpper(), currency.ToUpper()))
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

                APIArguments resultParams = new APIArguments();
                resultParams.Add("CurrencyBalance", pointsBalance - pointsOnHold);
                resultParams.Add("BalanceCurrencyValue", (pointsBalance - pointsOnHold) * reward.ConversionRate.Value * exchangeRate);
                resultParams.Add("ConversionRate", reward.ConversionRate.Value);
                resultParams.Add("ExchangeRate", exchangeRate);
                resultParams.Add("MinimumPoints", ValidateRestrictionValue(config.GetFWConfigProperty(FWConfig.LCAPMinPoints)));
                resultParams.Add("MaximumPoints", ValidateRestrictionValue(config.GetFWConfigProperty(FWConfig.LCAPMaxPoints)));
                resultParams.Add("MinimumMonetaryValue", RestrictionWithExchangeRate(config.GetFWConfigProperty(FWConfig.LCAPMinMoney), exchangeRate));
                resultParams.Add("MaximumMonetaryValue", RestrictionWithExchangeRate(config.GetFWConfigProperty(FWConfig.LCAPMaxMoney), exchangeRate));
                resultParams.Add("MinimumPercentage", ValidateRestrictionValue(config.GetFWConfigProperty(FWConfig.LCAPMinPercent), true));
                resultParams.Add("MaximumPercentage", ValidateRestrictionValue(config.GetFWConfigProperty(FWConfig.LCAPMaxPercent), true));

                var rewardParm = RewardDefSerializationUtility.SerializeRewardDefinition(reward, language, channel, true, ContentService);
                APIStruct rewardItem = new APIStruct() { Name = "PaymentRewardDef", IsRequired = true, Parms = rewardParm };
                resultParams.Add("PaymentRewardDef", rewardItem);

                response = SerializationUtils.SerializeResult(Name, Config, resultParams);

                #region Post Processing
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("member", member);
                context.Add("reward", reward);
                PostProcessSuccessfullInvocation(context);
                #endregion

                return response;
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

        private decimal? RestrictionWithExchangeRate(string restriction, decimal exchangeRate)
        {
            decimal? value = ValidateRestrictionValue(restriction);

            if (!value.HasValue)
                return null;

            return value.Value * exchangeRate;
        }

        private decimal? ValidateRestrictionValue(string restriction, bool isPercent = false)
        {
            if (string.IsNullOrEmpty(restriction))
                return null;

            decimal restrictionValue;
            if (!decimal.TryParse(restriction, out restrictionValue))
                throw new LWOperationInvocationException(string.Format("Invalid restriction value, not a number: {0}", restriction));

            if (isPercent && (restrictionValue > 1 || restrictionValue < 0))
                throw new LWOperationInvocationException(string.Format("Invalid percent restriction, not a fractional value: {0}", restriction));

            return restrictionValue;
        }
    }
}
