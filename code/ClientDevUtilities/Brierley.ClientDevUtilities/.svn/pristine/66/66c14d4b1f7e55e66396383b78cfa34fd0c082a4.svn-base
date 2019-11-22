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
    /// The GetPointsByEvent function the members points value for the given date range and point event.
    /// </summary>
    /// <example>
    ///     Usage : GetPointsByEvent('PointEvent',Start Date, End Date)
    /// </example>
    /// <remarks>
    ///     PointEvent must be a valid point event.
    ///     Start and End dates must be valid date formated strings.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the member's points value for the given date range and point event.",
        DisplayName = "GetPointsByEvent", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Points by event",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Points,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Point Event", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which point event?", Helpers = ParameterHelpers.PointEvent)]
	[ExpressionParameter(Order = 1, Name = "Start Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "From which date?")]
	[ExpressionParameter(Order = 2, Name = "End Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "To which date?")]
    public class GetPointsByEvent:UnaryOperation
    {
        private PointEvent pEvent = null;
        private Expression pointEvent = null;
		private Expression startDateExpression = null;
		private Expression endDateExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetPointsByEvent()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetPointsByEvent(Expression rhs)
            : base("GetPointsByEvent", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 3)
            {
                pointEvent = ((ParameterList)rhs).Expressions[0];
                startDateExpression = ((ParameterList)rhs).Expressions[1];
                endDateExpression = ((ParameterList)rhs).Expressions[2];
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetPointsByEvent.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetPointsByEvent('PointType',Start Date, End Date)";
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
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				pEvent = service.GetPointEvent(pointEvent.evaluate(contextObject).ToString());
				if (member != null)
				{
					DateTime startDate = (DateTime)(startDateExpression.evaluate(contextObject));
					DateTime endDate = (DateTime)(endDateExpression.evaluate(contextObject));
					if (DateTimeUtil.LessThan(endDate, startDate))
					{
						throw new CRMException("GetPointsByEvent: end date is less than the start date.");
					}
					long[] peIds = new long[] { pEvent.ID };
					return member.GetPoints(null, peIds, startDate, endDate);
				}
				else
				{
					throw new CRMException("GetPointsByEvent must be evaluated in the context of a loyalty member.");
				}
			}
		}
    }
}
