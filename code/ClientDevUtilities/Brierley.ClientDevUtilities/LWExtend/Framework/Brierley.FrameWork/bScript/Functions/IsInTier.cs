using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{

    /// <summary>
    /// The IsInTier function will return true if the member belongs to the named group.
    /// </summary>
    /// <example>
    ///     Usage : IsInTier('TierName',['Date'])
    /// </example>
    /// <remarks>
    /// Tier Name must be a valid group name. This function does not support custom tier implementations.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns true if the member belongs to the named group.",
		DisplayName = "IsInTier",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Booleans | ExpressionApplications.Strings | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Booleans,

		WizardDescription = "Is member in tier?",
		WizardCategory = WizardCategories.Tier,
		AdvancedWizard = false,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "TierName", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which Tier?", Helpers = ParameterHelpers.Tier)]
	[ExpressionParameter(Order = 1, Name = "Date", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "As of which date?")]
    public class IsInTier : UnaryOperation
    {
        //string TierName = string.Empty;
        private Expression tierNameExpression = null;
        private Expression dateExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public IsInTier()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal IsInTier(Expression rhs)
            : base("IsInTier", rhs)
        {
            ParameterList plist = rhs as ParameterList;
            if (plist != null)
            {
                if (plist.Expressions.Length == 2)
                {
                    tierNameExpression = ((ParameterList)rhs).Expressions[0];
                    dateExpression = ((ParameterList)rhs).Expressions[1];
                }
                else
                {
                    throw new CRMException("Invalid Function Call: Wrong number of arguments passed to IsInTier.");
                }
            }
            else
            {
                tierNameExpression = GetRight();
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "IsInTier('TierName',['Date'])";
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            Member member = ResolveMember(contextObject.Owner);			
            if (member != null)
            {
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					string tierName = tierNameExpression.evaluate(contextObject).ToString();
					if (dateExpression != null)
					{
						DateTime date = (DateTime)(dateExpression.evaluate(contextObject));
						return service.IsMemberInTier(member, tierName, date);
					}
					else
					{
						return service.IsMemberInTier(member, tierName);
					}
				}
            }
            else
            {
                throw new CRMException("IsInTier must be evaluated in the context of a loyalty member.");
            }
        }
    }
}
