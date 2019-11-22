using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript.Functions
{
    [Serializable]
	[ExpressionContext(Description = "Returns the current day or the day of the provided date.", 
		DisplayName = "Day", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Dates, 
		ExpressionReturns = ExpressionApplications.Numbers,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "Day of a date",
		AdvancedWizard=true
		)]

	[ExpressionParameter(Name = "Date", Type = ExpressionApplications.Dates, Optional = true, WizardDescription = "Which date?" )]
	class Day : UnaryOperation
    {

        private Expression _arg = null;

        public Day()
        {
        }

        public Day(Expression arg)
			: base("day", null)
		{
            _arg = arg;
        }


        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            string dateString = "";
            if (_arg != null)
            {
                dateString = _arg.evaluate(contextObject).ToString();
            }
            else
            {
                dateString = System.DateTime.Now.ToShortDateString();
            }
            return System.DateTime.Parse(dateString).Day;
        }        

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "Day() or Day('date')";
            }
        }

    }
}
