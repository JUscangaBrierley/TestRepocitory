using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
	/// Formats a date string according to the specified format.
    /// </summary>
    /// <example>
	///     Usage : FormatDate(date, format)
    /// </example>
    [Serializable]
	[ExpressionContext(Description = "Formats a date according to the provided format string.",
		DisplayName = "FormatDate", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Dates, 
		ExpressionReturns = ExpressionApplications.Dates,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "Format a date", 
		AdvancedWizard = true
		)]

	[ExpressionParameter(Order = 0, Name = "Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "Which date do you want to format?")]
	[ExpressionParameter(Order = 1, Name = "Format", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "What format do you wish to use?", Helpers = ParameterHelpers.DateFormat)]
    public class FormatDate : UnaryOperation
    {
		private Expression formatExpression = null;
		private Expression dateExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
		public FormatDate()
        {
        }

		internal FormatDate(Expression rhs)
			: base("FormatDate", rhs)
		{
			if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length == 2)
			{
				dateExpression = ((ParameterList)rhs).Expressions[0];
				formatExpression = ((ParameterList)rhs).Expressions[1];
				return;
			}
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to FormatDate.");
		}

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
				return "FormatDate(date,format)";
            }
        }


        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
		public override object evaluate(ContextObject contextObject)
		{
            object x = dateExpression.evaluate(contextObject);
            if (x != null)
            {
                //DateTime x = (DateTime)(dateExpression.evaluate(contextObject);
                string fmtStr = (string)formatExpression.evaluate(contextObject);

                return Brierley.FrameWork.Common.DateTimeUtil.ConvertDateToString(fmtStr, (DateTime)x);
            }
            else
            {
                return string.Empty;
            }
		}        
    }
}
