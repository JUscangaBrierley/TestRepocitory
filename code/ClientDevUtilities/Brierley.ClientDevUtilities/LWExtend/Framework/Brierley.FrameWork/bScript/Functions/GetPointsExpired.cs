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
	/// The GetPointsExpired function returns points that are expired at a certain date.
    /// </summary>
    /// <example>
	///     Usage : GetPointsExpired(['PointType'],Start Date,Expiry Date)
    /// </example>
    /// <remarks>
	///     Expiry date must be valid date formated strings.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the member's points that are expired at a certain date.",
		DisplayName = "GetPointsExpired", 
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Expired points",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Points,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Point Type", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which point type?", Helpers = ParameterHelpers.PointType)]
	[ExpressionParameter(Order = 1, Name = "Start Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "From which date?")]
	[ExpressionParameter(Order = 2, Name = "End Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "To which date?")]

	public class GetPointsExpired : UnaryOperation
    {
		private Expression typeExpression = null;
		private Expression startDateExpression = null;
		private Expression endDateExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetPointsExpired()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetPointsExpired(Expression rhs)
            : base("GetPointsExpired", rhs)
        {
			if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length == 2)
			{
				startDateExpression = ((ParameterList)rhs).Expressions[0];
				endDateExpression = ((ParameterList)rhs).Expressions[1];
				return;
			}
			else if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length == 3)
			{
				typeExpression = ((ParameterList)rhs).Expressions[0];
				startDateExpression = ((ParameterList)rhs).Expressions[1];
				endDateExpression = ((ParameterList)rhs).Expressions[2];
				return;
			}
			throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to GetPointsExpired.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
				return "GetPointsExpired(['PointType'],FromDate,ToDate)";
            }
        }

        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <param name="contextObject">The context provided at runtime</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
			try
			{
				Member member = ResolveMember(contextObject.Owner);
				if (member != null)
				{
					DateTime from = (DateTime)(startDateExpression.evaluate(contextObject));
					DateTime to = (DateTime)(endDateExpression.evaluate(contextObject));
					if (DateTimeUtil.LessThan(to, from))
					{
						throw new CRMException("GetPointsExpired: end date is less than the start date.");
					}
					using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
					{
						if (typeExpression != null)
						{
							PointType type = service.GetPointType(typeExpression.evaluate(contextObject).ToString());
							IList<PointType> ptList = new List<PointType>();
							ptList.Add(type);
							return service.GetExpiredPointBalance(member.LoyaltyCards, ptList, null, null, from, to, null, null, null, null, null, null, null);
						}
						else
						{
							return service.GetExpiredPointBalance(member.LoyaltyCards, null, null, null, from, to, null, null, null, null, null, null, null);
						}
					}
				}
				else
				{
					throw new LWBScriptException("GetPointsExpired must be evaluated in the context of a loyalty member.");
				}
			}
			catch (CRMException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LWBScriptException("Error evaluating GetPointsExpired.", ex);
			}
        }        
    }
}
