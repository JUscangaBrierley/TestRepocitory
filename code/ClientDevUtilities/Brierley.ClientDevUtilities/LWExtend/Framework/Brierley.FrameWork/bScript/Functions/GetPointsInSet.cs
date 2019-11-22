//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
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
	/// The GetPointsInSet function the members points value for the given date range and the set of point types.
	/// </summary>
	/// <example>
	///     Usage : GetPointsInSet('PointTypes',Start Date, End Date)
	/// </example>
	/// <remarks>
	///     PointType names delimeted by ;.
	///     Start and End dates must be valid date formated strings.
	/// </remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns the member's points value for the given date range and the set of point types",
	DisplayName = "GetPointsInSet",
	ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
	ExpressionType = ExpressionTypes.Function,
	ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
	ExpressionReturns = ExpressionApplications.Numbers,
	EvalRequiresMember = true)]
	public class GetPointsInSet : UnaryOperation
	{
		//private long[] typeIds = null;        
		//private DateTime startDate = DateTimeUtil.MinValue;
		//private DateTime endDate = DateTimeUtil.MinValue;
		private Expression setExpression = null;
		private Expression startDateExpression = null;
		private Expression endDateExpression = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public GetPointsInSet()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetPointsInSet(Expression rhs)
			: base("GetPointsInSet", rhs)
		{
			ContextObject cObj = new ContextObject();
			if (((ParameterList)rhs).Expressions.Length == 3)
			{
				setExpression = ((ParameterList)rhs).Expressions[0];
				startDateExpression = ((ParameterList)rhs).Expressions[1];
				endDateExpression = ((ParameterList)rhs).Expressions[2];
				return;
			}
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetPointsInSet.");
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "GetPointsInSet('PointType1;PointType2;PointType3',Start Date, End Date)";
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
					string[] typeSet = setExpression.evaluate(contextObject).ToString().Split(';');
					if (typeSet == null || typeSet.Length == 0)
					{
						throw new CRMException("Invalid pointtype set provided.");
					}
					long[] typeIds = new long[typeSet.Length];
					for (int i = 0; i < typeSet.Length; i++)
					{
						PointType pt = service.GetPointType(typeSet[i]);
						if (pt == null)
						{
							throw new CRMException("Point type " + typeSet[i] + " does not exist.");
						}
						typeIds[i] = pt.ID;
					}

					DateTime x = (DateTime)(startDateExpression.evaluate(contextObject));
					DateTime startDate = DateTime.Parse(x.ToShortDateString());
					DateTime y = (DateTime)(DateTime)(endDateExpression.evaluate(contextObject));
					DateTime endDate = DateTime.Parse(y.ToShortDateString());
					endDate = endDate.AddHours(23);
					endDate = endDate.AddMinutes(59);
					endDate = endDate.AddSeconds(59);
					if (typeIds != null)
					{
						return member.GetPoints(typeIds, null, startDate, endDate);
					}
					else
					{
						throw new CRMException("GetPointsInSet must have type ids to evaluate.");
					}
				}
			}
			else
			{
				throw new CRMException("GetPointsInSet must be evaluated in the context of a loyalty member.");
			}
		}
	}
}
