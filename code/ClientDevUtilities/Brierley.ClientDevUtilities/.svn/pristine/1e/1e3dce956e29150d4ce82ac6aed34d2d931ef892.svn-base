using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The DATE function returns the current date as a string
    /// </summary>
    /// <example>
    ///     Usage : Date()
    /// </example>
    [Serializable]
	[ExpressionContext(Description = "Returns the current date as a string.", 
		DisplayName = "Date", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Dates, 
		ExpressionReturns = ExpressionApplications.Dates,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "The current date", 
		AdvancedWizard = false
		//WizardSuffixDescription = null,
		//VisibleInWizard = true
		)]
    public class Date : UnaryOperation
	{

        /// <summary>
        /// Public Constructor
        /// </summary>
        public Date() : base("date", null)
        {
        }


        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "Date()";
            }
        }


        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            return System.DateTime.Now;
        }        
    }
}
