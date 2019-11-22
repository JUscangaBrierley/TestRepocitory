using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The Time function returns the current system time.
    /// </summary>
    /// <example>
    ///     Usage : Time()
    /// </example>
    /// <remarks>Function names are not case sensative.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the current system time.",
		DisplayName = "Time", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Dates | ExpressionApplications.Strings, 
		ExpressionReturns = ExpressionApplications.Strings)]
    public class Time : UnaryOperation
	{
        /// <summary>
        /// Public constructor primarily used by UI components.
        /// </summary>
        public Time() : base ("Time", null)
        {
        }

        /// <summary>
        /// This method will retun the syntax definition for the function.
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "Time()";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">An object containing context data used for evaluation</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            return System.DateTime.Now.ToShortTimeString();
        }        
    }
}
