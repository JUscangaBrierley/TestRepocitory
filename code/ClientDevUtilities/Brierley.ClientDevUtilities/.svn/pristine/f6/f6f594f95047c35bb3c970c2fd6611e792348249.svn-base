using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The DateDiffInDays function returns the difference in days between 2 dates
	/// </summary>
	/// <example>
	///     Usage : DateDiffInDays()
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns the difference between two dates.",
		DisplayName = "DateDiffInDays",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Numbers,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "Date difference in days",
		AdvancedWizard = true
		)]

	[ExpressionParameter(Order = 0, Name = "First Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "What is the first date?")]
	[ExpressionParameter(Order = 1, Name = "Second Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "What is the second date?")]

	public class DateDiffInDays : UnaryOperation
	{
		private const string _className = "DateDiffInDays";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		//DateTime date1, date2;		
		/// <summary>
		/// Public Constructor
		/// </summary>
		public DateDiffInDays()
		{
		}

		internal DateDiffInDays(Expression rhs)
			: base("DateDiffInDays", rhs)
		{
			ParameterList plist = rhs as ParameterList;
			//ContextObject cObj = new ContextObject();
			if (plist.Expressions.Length == 2)
			{
				//date1 = DateTime.Parse(plist.Expressions[0].evaluate(cObj).ToString());
				//date2 = DateTime.Parse(plist.Expressions[1].evaluate(cObj).ToString());
				return;
			}
			else
			{
				throw new CRMException("Invalid Function Call: Wrong number of arguments passed to DateDiffInDays.");
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "DateDiffInDays('date1','date2')";
			}
		}


		/// <summary>
		/// Performs the operation defined by this operator
		/// </summary>
		/// <returns>An object representing the result of the evaluation</returns>
		public override object evaluate(ContextObject contextObject)
		{
			ParameterList plist = GetRight() as ParameterList;
			DateTime date1 = DateTime.Parse(plist.Expressions[0].evaluate(contextObject).ToString());
			DateTime date2 = DateTime.Parse(plist.Expressions[1].evaluate(contextObject).ToString());

			System.TimeSpan diffResult = date1.Subtract(date2);
			return (int)diffResult.TotalDays;
		}
	}
}
