using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules;
using System;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
	/// Returns the current member's point balance as expressed in the given (or default) currency's value
	/// </summary>
	/// <example>
	///     Usage : GetCurrencyMonetaryValue([Currency])
	/// </example>
	/// <remarks>Function names are not case sensitive.</remarks>
	[Serializable]
    [ExpressionContext(Description = "Returns the current member's point balance as expressed in the given (or default) currency's value",
        DisplayName = "GetCurrencyMonetaryValue",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Member,
        ExpressionReturns = ExpressionApplications.Member,
        WizardCategory = WizardCategories.Points,
        WizardDescription = "Get member's point balance value",
        AdvancedWizard = true
        )]

    [ExpressionParameter(Order = 0, Name = "Currency", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which Currency?")]
    public class GetCurrencyMonetaryValue : UnaryOperation
    {
        private Expression _currencyExp;
        private RewardRuleUtil _util = new RewardRuleUtil();

        /// <summary>
        /// Syntax definition for this function.
        /// </summary>
        public new string Syntax
        {
            get { return "GetCurrencyMonetaryValue([Currency])"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetCurrencyMonetaryValue()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetCurrencyMonetaryValue(Expression rhs)
            : base("GetCurrencyMonetaryValue", rhs)
        {
            if (rhs != null && rhs is ParameterList && ((ParameterList)rhs).Expressions.Length > 1)
            {
                throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetCurrencyMonetaryValue.");
            }
            else if (rhs != null && rhs is Expression)
            {
                if (rhs is ParameterList)
                    _currencyExp = ((ParameterList)rhs).Expressions[0];
                else
                    _currencyExp = (Expression)rhs;
            }
        }

        /// <summary>
        /// Performs the operation defined by this function. 
        /// </summary>
        /// <param name="contextObject">A container object used to pass context at runtime.</param>
        /// <returns>An object representing the result of the evaluation</returns>
        /// <exception cref="Brierley.Framework.Common.Exceptions.CRMException">thrown for illegal arguments</exception>
        public override object evaluate(ContextObject contextObject)
        {
            Member member = ResolveMember(contextObject.Owner);

            if(member == null)
                throw new CRMException("GetCurrencyMonetaryValue must be evaluated in the context of a loyalty member.");

            string currency = string.Empty;
            if (_currencyExp != null)
                currency = _currencyExp.evaluate(contextObject).ToString();

            var config = LWConfigurationUtil.GetCurrentConfiguration().FWConfig;
            string defaultCurrency = config.GetFWConfigProperty(FWConfig.LCAPDefaultCurrency);
            if (string.IsNullOrEmpty(defaultCurrency))
                throw new CRMException("A default currency must be configured.");

            decimal exchangeRate = 1;

            using (var contentService = LWDataServiceUtil.ContentServiceInstance())
            {
                if (!string.IsNullOrEmpty(currency) && !string.Equals(defaultCurrency, currency, StringComparison.OrdinalIgnoreCase))
                {
                    ExchangeRate exchange = contentService.GetExchangeRate(defaultCurrency.ToUpper(), currency.ToUpper());
                    if (exchange == null)
                        throw new CRMException(string.Format("Currency mapping not found: {0} -> {1}", defaultCurrency.ToUpper(), currency.ToUpper()));
                    string sMaxAge = config.GetFWConfigProperty(FWConfig.LCAPMaxExchangeRateAge);
                    if (!string.IsNullOrEmpty(sMaxAge))
                    {
                        double maxAge;
                        if (!double.TryParse(sMaxAge, out maxAge))
                            throw new CRMException(string.Format("Maximum exchange rate age value is set but is not a valid number: {0}", sMaxAge));
                        if (maxAge > 0 && (exchange.UpdateDate ?? exchange.CreateDate).AddDays(maxAge) < DateTime.Now)
                            throw new CRMException(string.Format("Exchange rate data is too old. Maximum days age: {0}. Last updated: {1}", sMaxAge, (exchange.UpdateDate ?? exchange.CreateDate)));
                    }
                    exchangeRate = exchange.Rate;
                }

                RewardDef reward = contentService.GetRewardDefForExchange(member);

                if(reward == null)
                    throw new CRMException("No Payment reward found for the member.");

                decimal pointsBalance = _util.GetPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), member, null);
                decimal pointsOnHold = _util.GetOnHoldPoints(reward, reward.GetPointTypes(), reward.GetPointEvents(), member, null);

                return (pointsBalance - pointsOnHold) * reward.ConversionRate.Value * exchangeRate;
            }
        }
    }
}
