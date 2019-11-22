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
    /// The GetPointsToNextTier function gets the points needed to get to the next tier.
    /// </summary>
    /// <example>
    ///     Usage : GetPointsToNextTier([Include Expired Points])
    /// </example>
    /// <remarks>
    ///     PointType is optional.
    ///     Start and End dates must be valid date formated strings.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the number of points needed for the member to get to the next tier.",
        DisplayName = "GetPointsToNextTier", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Points to next tier",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Points,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Include Expired Points", Type = ExpressionApplications.Booleans, Optional = true, WizardDescription = "Include expired points?")]
	
    public class GetPointsToNextTier : UnaryOperation
    {        
        private Expression includeExpiredExpression = null;
        		
        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetPointsToNextTier()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetPointsToNextTier(Expression rhs)
            : base("GetPointsToNextTier", rhs)
        {
            includeExpiredExpression = GetRight();
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetPointsToNextTier([Include Expired Points])";
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

                bool includeExpiredPoints = includeExpiredExpression != null ? Convert.ToBoolean(includeExpiredExpression.evaluate(contextObject)) : false;
                return member.GetPointsToNextTier(includeExpiredPoints);
            }
            else
            {
                throw new CRMException("GetPointsToNextTier must be evaluated in the context of a loyalty member.");
            }
        }        
    }
}
