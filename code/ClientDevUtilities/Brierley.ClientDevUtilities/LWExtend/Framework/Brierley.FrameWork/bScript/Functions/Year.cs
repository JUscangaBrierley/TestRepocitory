using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript.Functions
{
    [Serializable]
	[ExpressionContext(Description = "Returns the current year or the year of the date passed in.",
		DisplayName = "Year",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Numbers,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "Year", 
		AdvancedWizard = true
	)]
	[ExpressionParameter(Order = 0, Name = "Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "Which date?")]
	class Year : UnaryOperation
	{
        private Expression _arg = null;


        /// <summary>
        /// Constructor
        /// </summary>
        public Year()
            : base("Year", null)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="arg"></param>
        public Year(Expression arg):base("Year", arg)
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
            return System.DateTime.Parse(dateString).Year;
        }
        
        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "Year() or Year('date')";
            }
        }

    }
}
