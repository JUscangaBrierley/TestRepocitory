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
    /// The GetPoints function the members points value for the given date range and point type.
    /// </summary>
    /// <example>
    ///     Usage : GetPoints(['PointType'],Start Date, End Date)
    /// </example>
    /// <remarks>
    ///     PointType is optional.
    ///     Start and End dates must be valid date formated strings.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the member's points value for the given date range and point type.", 
		DisplayName = "GetPoints", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Point Balance",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Points,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Point Type", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which point type?", Helpers = ParameterHelpers.PointType)]
	[ExpressionParameter(Order = 1, Name = "Start Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "From which date?")]
	[ExpressionParameter(Order = 2, Name = "End Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "To which date?")]

    public class GetPoints:UnaryOperation
    {
        private PointType type = null;
		//private DateTime startDate = DateTimeUtil.MinValue;
		//private DateTime endDate = DateTimeUtil.MinValue;
        private Expression pointType = null;
		private Expression startDateExpression = null;
		private Expression endDateExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetPoints()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetPoints(Expression rhs)
            : base("GetPoints", rhs)
        {
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs == 3)
            {
                pointType = ((ParameterList)rhs).Expressions[0];
                startDateExpression = ((ParameterList)rhs).Expressions[1];
                endDateExpression = ((ParameterList)rhs).Expressions[2];
                //DateTime x = (DateTime)((ParameterList)rhs).Expressions[1].evaluate(cObj);
                //startDate = DateTime.Parse(x.ToShortDateString());
                //DateTime y = (DateTime)((ParameterList)rhs).Expressions[2].evaluate(cObj);
                //endDate = DateTime.Parse(y.ToShortDateString());
                //endDate = endDate.AddHours(23);
                //endDate = endDate.AddMinutes(59);
                //endDate = endDate.AddSeconds(59);
                return;
            }
            if (numArgs == 2)
            {
                startDateExpression = ((ParameterList)rhs).Expressions[0];
                endDateExpression = ((ParameterList)rhs).Expressions[1];
                //DateTime x = (DateTime)((ParameterList)rhs).Expressions[0].evaluate(cObj);
                //startDate = DateTime.Parse(x.ToShortDateString());
                //DateTime y = (DateTime)((ParameterList)rhs).Expressions[1].evaluate(cObj);
                //endDate = DateTime.Parse(y.ToShortDateString());
                //endDate = endDate.AddHours(23);
                //endDate = endDate.AddMinutes(59);
                //endDate = endDate.AddSeconds(59);
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetPoints.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetPoints(['PointType'],Start Date, End Date)";
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
				if (pointType != null)
					type = service.GetPointType(pointType.evaluate(contextObject).ToString());

				if (member != null)
				{
					DateTime startDate = (DateTime)(startDateExpression.evaluate(contextObject));
					//DateTime startDate = DateTime.Parse(x.ToShortDateString());
					DateTime endDate = (DateTime)(endDateExpression.evaluate(contextObject));
					//DateTime endDate = DateTime.Parse(y.ToShortDateString());
					//endDate = endDate.AddHours(23);
					//endDate = endDate.AddMinutes(59);
					//endDate = endDate.AddSeconds(59);
					if (DateTimeUtil.LessThan(endDate, startDate))
					{
						throw new CRMException("GetPoints: end date is less than the start date.");
					}

					if (this.type != null)
					{
						long[] ptIds = new long[] { this.type.ID };
						return member.GetPoints(ptIds, null, startDate, endDate);
					}
					else
					{
						return member.GetPoints(null, null, startDate, endDate);
					}
				}
				else
				{
					throw new CRMException("GetPoints must be evaluated in the context of a loyalty member.");
				}
			}
        }        
    }
}
