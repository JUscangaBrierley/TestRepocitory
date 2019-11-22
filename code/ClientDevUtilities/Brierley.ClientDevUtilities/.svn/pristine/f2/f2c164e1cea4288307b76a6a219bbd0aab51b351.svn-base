using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Returns a boolean indicating if the specified date is within the specified date range.
    /// </summary>
    /// <example>
    ///     Usage : IsDateWithinRange('date','startdate','enddate', 'true')
    ///     Usage : IsDateWithinRange(Member.AccountOpenDate,'startdate','enddate','true')
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "IsDateWithinRange is a function that will returns a boolean indicating if the specified date is within the specified date range.", 
		DisplayName = "IsDateWithinRange", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Dates | ExpressionApplications.Strings, 
		ExpressionReturns = ExpressionApplications.Booleans,
		WizardDescription = "Is Within Date Range?",
		AdvancedWizard = true, 
		WizardCategory = WizardCategories.Dates)]
	[ExpressionParameter(Name = "date", WizardDescription = "What date?", Type = ExpressionApplications.Dates, Optional = false, Order = 1)]
	[ExpressionParameter(Name = "startdate", WizardDescription = "What start date?", Type = ExpressionApplications.Dates, Optional = false, Order = 2)]
	[ExpressionParameter(Name = "enddate", WizardDescription = "What end date?", Type = ExpressionApplications.Dates, Optional = false, Order = 3)]
	[ExpressionParameter(Name = "isinclusive", WizardDescription = "Inclusive?", Type = ExpressionApplications.Booleans, Optional = false, Order = 4)]
    public class IsDateWithinRange : UnaryOperation
    {
		private const string _className = "IsDateWithinRange";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private Expression _dateExpression;
		private Expression _startdateExpression;
		private Expression _enddateExpression;
		private Expression _isinclusiveExpression;

        /// <summary>
        /// Public constructor used by UI components
        /// </summary>
        public IsDateWithinRange(Expression rhs) : base("IsDateWithinRange", rhs)
        {
			const string methodName = "IsDateWithinRange";
			if (rhs == null)
			{
				string msg = "Invalid Function Call: No arguments passed to IsDateWithinRange.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			if (rhs is ParameterList)
			{
				int numArgs = ((ParameterList)rhs).Expressions.Length;
				if (numArgs == 4)
				{
					_dateExpression = ((ParameterList)rhs).Expressions[0];
					_startdateExpression = ((ParameterList)rhs).Expressions[1];
					_enddateExpression = ((ParameterList)rhs).Expressions[2];
					_isinclusiveExpression = ((ParameterList)rhs).Expressions[3];
				}
				else
				{
					string msg = "Invalid Function Call: Wrong number of arguments passed to IsDateWithinRange.";
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}
			}
			else
			{
				string msg = "Invalid Function Call: Unknown argument type passed to IsDateWithinRange.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}
        }

        /// <summary>
        /// Public constructor used primarily by UI components.
        /// </summary>
		public IsDateWithinRange()
        {
        }

        /// <summary>
        /// This method will return the function syntax definition
        /// </summary>
        public new string Syntax
        {
            get
            {
				return "IsDateWithinRange('date','startdate','enddate', 'isinclusive: true/false')";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">A container object used to pass context at runtime.</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
			const string methodName = "evaluate";

			DateTime? date;
			DateTime? startdate;
			DateTime? enddate;
			bool isinclusive = true;

			try
			{
				date = _dateExpression.evaluate(contextObject) as DateTime?;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating arg date", ex);
				throw new CRMException("IsDateWithinRange: Problem with arg date");
			}
			if (!date.HasValue)
			{
				string msg = "IsDateWithinRange: Argument date evaluates to null or is not a date.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			try
			{
				startdate = _startdateExpression.evaluate(contextObject) as DateTime?;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating arg startdate", ex);
				throw new CRMException("IsDateWithinRange: Problem with arg startdate");
			}
			if (!startdate.HasValue)
			{
				string msg = "IsDateWithinRange: Argument startdate evaluates to null or is not a date.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			try
			{
				enddate = _enddateExpression.evaluate(contextObject) as DateTime?;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating arg enddate", ex);
				throw new CRMException("IsDateWithinRange: Problem with arg enddate");
			}
			if (!enddate.HasValue)
			{
				string msg = "IsDateWithinRange: Argument enddate evaluates to null or empty.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			object obj = null;
			try
			{
				obj = _isinclusiveExpression.evaluate(contextObject);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating arg isinclusive", ex);
				throw new CRMException("IsDateWithinRange: Problem with arg isinclusive");
			}
			if (obj == null || string.IsNullOrEmpty(obj.ToString()))
			{
				string msg = "IsDateWithinRange: Argument isinclusive evaluates to null.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}
			if (!bool.TryParse(obj.ToString(), out isinclusive))
			{
				string msg = "IsDateWithinRange: Argument isinclusive evaluates to a non-boolean value.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			bool result = DateTimeUtil.IsDateInBetween(date.Value, startdate.Value, enddate.Value, isinclusive);
            return result;
        }        
    }
}
