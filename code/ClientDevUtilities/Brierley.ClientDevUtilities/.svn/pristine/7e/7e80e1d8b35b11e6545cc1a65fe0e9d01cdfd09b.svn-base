//-----------------------------------------------------------------------
//(C) 2013 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The GetPointTransactionCount function counts the number of point transactions for the memberfor the given parameters.
    /// </summary>
    /// <example>
    ///     Usage : GetPointTransactionCount('PointTypes','PointEvents', Start Date, End Date,[Include Expired Points])
    /// </example>
    /// <remarks>
    ///     PointType names delimeted by ;.
    ///     Start and End dates must be valid date formated strings.
    /// </remarks>
    [Serializable]
    [ExpressionContext(Description = "Returns the number of point transactions for the given date range and the set of point types",
    DisplayName = "GetPointTransactionCount",
    ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
    ExpressionType = ExpressionTypes.Function,
    ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
    ExpressionReturns = ExpressionApplications.Numbers,

    WizardDescription = "Number of point transactions by types and events",
    AdvancedWizard = false,
    WizardCategory = WizardCategories.Points,
    EvalRequiresMember = true)]

    [ExpressionParameter(Order = 0, Name = "Point Types", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which point types?", Helpers = ParameterHelpers.PointType, AllowMultiple = true)]
    [ExpressionParameter(Order = 1, Name = "Point Events", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which point events?", Helpers = ParameterHelpers.PointEvent, AllowMultiple = true)]
    [ExpressionParameter(Order = 2, Name = "Start Date", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "From which date?")]
    [ExpressionParameter(Order = 3, Name = "End Date", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "To which date?")]
    [ExpressionParameter(Order = 4, Name = "Include Expired Points", Type = ExpressionApplications.Booleans, Optional = true, WizardDescription = "Include expired points?")]

    public class GetPointTransactionCount : UnaryOperation
    {
        private Expression pointTypeSetExpression = null;
        private Expression pointEventSetExpression = null;
        private Expression startDateExpression = null;
        private Expression endDateExpression = null;
        private Expression includeExpiredExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public GetPointTransactionCount()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetPointTransactionCount(Expression rhs)
            : base("GetPointTransactionCount", rhs)
        {
            ParameterList plist = rhs as ParameterList;
            if (plist != null)
            {
                if (plist.Expressions.Length > 1)
                {
                    pointTypeSetExpression = ((ParameterList)rhs).Expressions[0];
                    pointEventSetExpression = ((ParameterList)rhs).Expressions[1];
                }
                if (plist.Expressions.Length > 2)
                {
                    startDateExpression = ((ParameterList)rhs).Expressions[2];
                }
                if (plist.Expressions.Length > 3)
                {
                    endDateExpression = ((ParameterList)rhs).Expressions[3];
                }
                if (plist.Expressions.Length > 4)
                {
                    includeExpiredExpression = ((ParameterList)rhs).Expressions[4];
                }
            }
            else
            {
                // only one parameter
                pointTypeSetExpression = this.GetRight();
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "GetPointTransactionCount('PointType1;PointType2;PointType3','PointEvent1;PointEvent2;PointEvent3', Start Date, End Date,[Include Expired Points])";
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
				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					long[] typeIds = null;
					if (pointTypeSetExpression != null)
					{
						string[] typeSet = pointTypeSetExpression.evaluate(contextObject).ToString().Split(';');
						if (typeSet == null || typeSet.Length == 0)
						{
							throw new LWBScriptException("Invalid pointtype set provided.");
						}
						typeIds = new long[typeSet.Length];
						for (int i = 0; i < typeSet.Length; i++)
						{
							if (string.IsNullOrEmpty(typeSet[i]))
							{
								continue;
							}
							PointType pt = service.GetPointType(typeSet[i]);
							if (pt == null)
							{
								throw new CRMException("Point type " + typeSet[i] + " does not exist.");
							}
							typeIds[i] = pt.ID;
						}
					}
					long[] eventIds = null;
					if (pointEventSetExpression != null)
					{
						string[] eventSet = pointEventSetExpression.evaluate(contextObject).ToString().Split(';');
						if (eventSet == null || eventSet.Length == 0)
						{
							throw new LWBScriptException("Invalid event types set provided.");
						}
						eventIds = new long[eventSet.Length];
						for (int i = 0; i < eventSet.Length; i++)
						{
							if (string.IsNullOrEmpty(eventSet[i]))
							{
								continue;
							}
							PointEvent pe = service.GetPointEvent(eventSet[i]);
							if (pe == null)
							{
								throw new CRMException("Point event " + eventSet[i] + " does not exist.");
							}
							eventIds[i] = pe.ID;
						}
					}

					DateTime? startDate = startDateExpression != null ? (DateTime?)(startDateExpression.evaluate(contextObject)) : null;
					DateTime? endDate = endDateExpression != null ? (DateTime?)(endDateExpression.evaluate(contextObject)) : null;
					if (startDate.HasValue && endDate.HasValue && DateTimeUtil.LessThan(endDate.Value, startDate.Value))
					{
						throw new CRMException("GetPointTransactionCount: end date is less than the start date.");
					}

					bool includeExpiredPoints = includeExpiredExpression != null ? Convert.ToBoolean(includeExpiredExpression.evaluate(contextObject)) : false;

					return service.HowManyPointTransactions(member, typeIds, eventIds, startDate, endDate, includeExpiredPoints);
				}
            }
            else
            {
                throw new CRMException("GetPointTransactionCount must be evaluated in the context of a loyalty member.");
            }
        }
    }
}
