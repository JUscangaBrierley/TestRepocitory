using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{

    /// <summary>
    /// The IsPromotionValid function will return true if the named promotion is valid.
    /// </summary>
    /// <example>
    ///     Usage : IsPromotionValid('promotion code')
    /// </example>
    /// <remarks>
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns true if the named promotion is valid.",
		DisplayName = "IsPromotionValid",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Booleans,
		WizardDescription = "Is promotion valid?",
		AdvancedWizard = true)]

	[ExpressionParameter(Order = 0, Name = "Promotion Code", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which promotion?", Helpers = ParameterHelpers.PromotionCode)]
	public class IsPromotionValid : UnaryOperation
    {
        Expression promoCodeExpression = null;
        
        /// <summary>
        /// Public Constructor
        /// </summary>
        public IsPromotionValid()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal IsPromotionValid(Expression rhs)
            : base("IsPromotionValid", rhs)
        {
            promoCodeExpression = rhs;
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "IsPromotionValid('PromotionCode')";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
			using (var svc = LWDataServiceUtil.ContentServiceInstance())
			{
				string promoCode = (string)promoCodeExpression.evaluate(contextObject);
				Promotion promo = svc.GetPromotionByCode(promoCode);
				if (promo != null)
				{
					return promo.IsValid();
				}
				else
				{
					throw new LWBScriptException("Invalid promotion code provided.") { ErrorCode = 3212 };
				}
			}
        }
    }
}
