using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// TodayName is a function that will return the name of the day of the week
    /// on which the supplied date falls.
    /// </summary>
    /// <example>
    ///     Usage : TodayName('date')
    ///     Usage : TodayName(Member.AccountOpenDate)
    /// </example>
    /// <remarks>Function names are not case sensative.</remarks>
    [Serializable]
	[ExpressionContext(Description = "TodayName is a function that will return the name of the day of the week on which the supplied date falls.", 
		DisplayName = "TodayName", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Dates | ExpressionApplications.Strings, 
		ExpressionReturns = ExpressionApplications.Strings,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "Day of Week",
		AdvancedWizard = true
	)]
	[ExpressionParameter(Name = "date", WizardDescription = "What date?", Type = ExpressionApplications.Dates, Optional = true)]
    public class TodayName : UnaryOperation
    {
        /// <summary>
        /// Public constructor used by UI components
        /// </summary>
        public TodayName(Expression rhs) : base("TodayName",rhs)
        {
        }

        /// <summary>
        /// Public constructor used primarily by UI components.
        /// </summary>
        public TodayName() : base ("TodayName", null)
        {
        }

        /// <summary>
        /// This method will return the function syntax definition
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "TodayName()";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">A container object used to pass context at runtime.</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            if (GetRight() != null)
            {
                DateTime date = (DateTime)GetRight().evaluate(contextObject);
                return date.DayOfWeek.ToString();
            }
            else
            {
                return System.DateTime.Now.DayOfWeek.ToString();
            }
        }        
    }
}
