using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The GetPointsSummary function gets the members points sumamry for the given point event and point type.
	/// </summary>
	/// <example>
	///     Usage : GetPointsSunmmary('Type',['PointType'],['PointEvent'])
	/// </example>
	/// <remarks>
	///     Type - Balance | Earned
	///     PointType is a semi colon delimeted list of point types
	///     PointEvent is a semi colon delimeted list of point events
	///    
	/// </remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns either the member's point balance or the earned balance for the provided point type and point event",
		DisplayName = "GetPointsSummary",
		ExcludeContext = ExpressionContexts.Email | ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Dates | ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Point Summary",
		AdvancedWizard = false,
		WizardCategory = WizardCategories.Points,
		EvalRequiresMember = true)]

	[ExpressionParameter(Order = 0, Name = "Type", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which type of summary?", Helpers = ParameterHelpers.PointsSummaryType)]
	[ExpressionParameter(Order = 1, Name = "Point Type", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which point type?", Helpers = ParameterHelpers.PointType)]
	[ExpressionParameter(Order = 2, Name = "Point Event", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which point event?", Helpers = ParameterHelpers.PointEvent)]

	public class GetPointsSummary : UnaryOperation
	{
		private Expression _type = null;
		private Expression _pointTypeExpression = null;
		private Expression _pointEventExpression = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public GetPointsSummary()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetPointsSummary(Expression rhs)
			: base("GetPointsSummary", rhs)
		{
			ContextObject cObj = new ContextObject();

			if (rhs is ParameterList)
			{
				var plist = (ParameterList)rhs;

				if (plist.Expressions.Length > 0)
				{
					_type = plist.Expressions[0];
				}

				if (plist.Expressions.Length > 1)
				{
					_pointTypeExpression = plist.Expressions[1];
				}

				if (plist.Expressions.Length > 2)
				{
					_pointEventExpression = plist.Expressions[2];
				}
			}
			else if (rhs is Expression)
			{
				_type = rhs;
			}

			if (_type == null)
			{
				throw new CRMException("Invalid Function Call: Wrong number of arguments passed to GetPointsSummary.");
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "GetPointsSummary('Type',['PointType'],['PointEvent'])";
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
				string[] pointTypes = null;
				string[] pointEvents = null;

				string type = _type.evaluate(contextObject).ToString();

				if (_pointTypeExpression != null)
				{
					string pointTypesStr = _pointTypeExpression.evaluate(contextObject).ToString();
					pointTypes = pointTypesStr.Split(';');
				}

				if (_pointEventExpression != null)
				{
					string pointEventsStr = _pointEventExpression.evaluate(contextObject).ToString();
					pointEvents = pointEventsStr.Split(';');
				}

				using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{

					IList<PointsSummary> list = service.RetrievePointsSummariesByMember(member.IpCode, pointEvents, pointTypes);

					decimal balance = 0;

					foreach (PointsSummary summary in list)
					{
						if (type.Equals("Balance", StringComparison.OrdinalIgnoreCase))
						{
							balance += summary.Balance;
						}
						else if (type.Equals("Earned", StringComparison.OrdinalIgnoreCase))
						{
							balance += summary.Earned;
						}
					}
					return balance;
				}
			}
			else
			{
				throw new CRMException("GetPointsSummary must be evaluated in the context of a loyalty member.");
			}
		}
	}
}
