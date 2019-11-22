using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The GetPointsToNextReward function gets the points needed to get to the next reward.
    /// </summary>
    /// <example>
    ///     Usage : GetPointsToNextReward(['PointType'])
    /// </example>
    /// <remarks>
    ///     PointType is optional.
    ///     Start and End dates must be valid date formated strings.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the number of points needed for the member to get to the next reward.",
        DisplayName = "GetPointsToNextReward", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Points to next reward",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Points,
		EvalRequiresMember = true)]

    [ExpressionParameter(Order = 0, Name = "Default Tier", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Default Tier?", Helpers = ParameterHelpers.Tier)]
    [ExpressionParameter(Order = 1, Name = "Override Tier", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Override Tier?", Helpers = ParameterHelpers.Tier)]
    public class GetPointsToNextReward : UnaryOperation
    {
        Expression defaultTier;
        Expression overrideTier;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetPointsToNextReward()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetPointsToNextReward(Expression rhs)
            : base("GetPointsToNextReward", rhs)
        {
            ParameterList plist = rhs as ParameterList;
            if (plist != null)
            {
                if (plist.Expressions.Length == 2)
                {
                    defaultTier = ((ParameterList)rhs).Expressions[0];
                    overrideTier = ((ParameterList)rhs).Expressions[1];
                    return;
                }
            }
            else
            {
                // only one parameter
                defaultTier = this.GetRight();
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetPointsToNextReward(['DefaultTier'](optional), ['OverrideTier'](optional))";
            }
        }

        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <param name="contextObject">The context provided at runtime</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            Member member = ResolveMember(contextObject.Owner);
            if (member != null)
            {
                string defaultTierString = defaultTier != null ? defaultTier.evaluate(contextObject).ToString() : string.Empty;
                string overrideTierString = overrideTier != null ? overrideTier.evaluate(contextObject).ToString() : string.Empty;
                return member.GetPointsToNextReward(defaultTierString, overrideTierString);
            }
            else
            {
                throw new CRMException("GetPointsToNextReward must be evaluated in the context of a loyalty member.");
            }
        }        
    }
}
