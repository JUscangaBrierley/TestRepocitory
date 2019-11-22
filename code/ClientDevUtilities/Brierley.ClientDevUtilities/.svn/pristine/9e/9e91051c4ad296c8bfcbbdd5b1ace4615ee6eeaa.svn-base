using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript.Functions
{
    [Serializable]
	[ExpressionContext(Description = "Returns the current month or the month of the provided date.", 
		DisplayName = "Month", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Numbers,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "Month", 
		AdvancedWizard = true
		)]
	[ExpressionParameter(Order = 0, Name = "Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "Which date?")]
	class Month : UnaryOperation
	{
        private Expression _arg = null;

        public Month()
        {

        }

        internal Month(Expression arg)
            : base("Month", arg)
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
            return System.DateTime.Parse(dateString).Month;
                
        }
        
        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "Month() or Month('date')";
            }
        }

    }
}
