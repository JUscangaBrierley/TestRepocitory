using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Creates a date object based on the parameters.
    /// </summary>
    /// <example>
    ///     Usage : MakeDate(Year, Month, Day, [Hour], [Minutes], [Seconds])
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Creates a date object based on the parameters.",
        DisplayName = "MakeDate",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Dates,
        ExpressionReturns = ExpressionApplications.Dates,
        WizardCategory = WizardCategories.Dates,
        WizardDescription = "Create a date",
        AdvancedWizard = true
        )]

    [ExpressionParameter(Order = 0, Name = "Year", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "Which year?")]
    [ExpressionParameter(Order = 1, Name = "Month", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "Which month?")]
    [ExpressionParameter(Order = 2, Name = "Day", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "Which day?")]
    [ExpressionParameter(Order = 3, Name = "Hour", Type = ExpressionApplications.Numbers, Optional = true, WizardDescription = "Hours (24 hr format)?")]
    [ExpressionParameter(Order = 4, Name = "Minutes", Type = ExpressionApplications.Numbers, Optional = true, WizardDescription = "Minutes?")]
    [ExpressionParameter(Order = 5, Name = "Seconds", Type = ExpressionApplications.Numbers, Optional = true, WizardDescription = "Seconds?")]
    public class MakeDate : UnaryOperation
    {
        private Expression monthExpression = null;
        private Expression dayExpression = null;
        private Expression yearExpression = null;
        private Expression hourExpression = null;
        private Expression minExpression = null;
        private Expression secExpression = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public MakeDate()
        {
        }

        internal MakeDate(Expression rhs)
            : base("MakeDate", rhs)
        {
            if (rhs is ParameterList && (((ParameterList)rhs).Expressions.Length < 3) || ((ParameterList)rhs).Expressions.Length > 6)
            {
                throw new CRMException("Invalid Function Call: Wrong number of arguments passed to MakeDate.");
            }
            yearExpression = ((ParameterList)rhs).Expressions[0];
            monthExpression = ((ParameterList)rhs).Expressions[1];
            dayExpression = ((ParameterList)rhs).Expressions[2];
            if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length > 3)
            {
                hourExpression = ((ParameterList)rhs).Expressions[3];
            }
            if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length > 4)
            {
                minExpression = ((ParameterList)rhs).Expressions[4];
            }
            if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length > 5)
            {
                secExpression = ((ParameterList)rhs).Expressions[5];
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "MakeDate(Year, Month, Dayr, [Hour], [Minutes], [Seconds])";
            }
        }


        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            object x = monthExpression.evaluate(contextObject);
            int month = int.Parse(x.ToString());
            x = dayExpression.evaluate(contextObject);
            int day = int.Parse(x.ToString());
            x = yearExpression.evaluate(contextObject);
            int year = int.Parse(x.ToString());

            int hour = hourExpression != null ? int.Parse(hourExpression.evaluate(contextObject).ToString()) : 0;
            int min = minExpression != null ? int.Parse(minExpression.evaluate(contextObject).ToString()) : 0;
            int sec = secExpression != null ? int.Parse(secExpression.evaluate(contextObject).ToString()) : 0;

            DateTime date = new DateTime(year, month, day, hour, min, sec);

            return date;
        }
    }
}
