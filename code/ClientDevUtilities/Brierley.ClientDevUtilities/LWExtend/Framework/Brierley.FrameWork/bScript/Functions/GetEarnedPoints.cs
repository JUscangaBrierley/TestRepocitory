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
	/// The GetEarnedPoints function the members points value for the given date range.
	/// </summary>
	/// <example>
	///     Usage : GetEarnedPoints([Start Date],[End Date],[Include Expired Points])
	/// </example>
	/// <remarks>
	///     PointType is optional.
	///     Start and End dates must be valid date formated strings.
	/// </remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns the member's earned points value for the given date range.",
		DisplayName = "GetEarnedPoints",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Earned Points",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Points,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Start Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "From which date?")]
	[ExpressionParameter(Order = 1, Name = "End Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "To which date?")]
    [ExpressionParameter(Order = 2, Name = "Include Expired Points", Type = ExpressionApplications.Booleans, Optional = true, WizardDescription = "Include expired points?")]
	public class GetEarnedPoints : UnaryOperation
	{
		private Expression startDateExpression = null;
		private Expression endDateExpression = null;
        private Expression includeExpiredExpression = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public GetEarnedPoints()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetEarnedPoints(Expression rhs)
			: base("GetEarnedPoints", rhs)
		{
			ParameterList plist = rhs as ParameterList;
			if (plist != null)
			{
				if (plist.Expressions.Length == 2)
				{
					startDateExpression = ((ParameterList)rhs).Expressions[0];
					endDateExpression = ((ParameterList)rhs).Expressions[1];
					return;
				}
                else if (plist.Expressions.Length == 3)
                {
                    startDateExpression = ((ParameterList)rhs).Expressions[0];
                    endDateExpression = ((ParameterList)rhs).Expressions[1];
                    includeExpiredExpression = ((ParameterList)rhs).Expressions[2];
                    return;
                }
				else
				{
					throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetEarnedPoints.");
				}
			}
			else
			{
				startDateExpression = this.GetRight();
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
                return "GetEarnedPoints([Start Date],[End Date],[Include Expired Points])";
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
				DateTime? startDate = startDateExpression != null ? (DateTime?)(startDateExpression.evaluate(contextObject)) : null;
				DateTime? endDate = endDateExpression != null ? (DateTime?)(endDateExpression.evaluate(contextObject)) : null;
				if (startDate.HasValue && endDate.HasValue && DateTimeUtil.LessThan(endDate.Value, startDate.Value))
				{
					throw new CRMException("GetEarnedPoints: end date is less than the start date.");
				}
                bool includeExpiredPoints = includeExpiredExpression != null ? Convert.ToBoolean(includeExpiredExpression.evaluate(contextObject)) : false;

				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					return service.GetEarnedPointBalance(member.LoyaltyCards, (long[])null, (long[])null, startDate, endDate, includeExpiredPoints);
				}
			}
			else
			{
				throw new CRMException("GetEarnedPoints must be evaluated in the context of a loyalty member.");
			}
		}
	}
}
