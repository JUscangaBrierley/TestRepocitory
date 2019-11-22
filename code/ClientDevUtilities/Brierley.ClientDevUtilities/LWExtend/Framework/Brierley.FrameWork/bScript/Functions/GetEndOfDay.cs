using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Returns the date corresponding to the end of the specified date.
    /// </summary>
    /// <example>
	///     Usage : GetEndOfDay('date')     
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the date corresponding to the end of the specified date.",
		DisplayName = "GetEndOfDay",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Dates,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "End of day",
		AdvancedWizard = true
	)]
	[ExpressionParameter(Name = "Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "Which date?")]
    public class GetEndOfDay : UnaryOperation
    {
        /// <summary>
        /// Syntax definition for the function.
        /// </summary>
        public new string Syntax
        {
			get { return "GetEndOfDay(['date']|[Date()]|date expression)"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetEndOfDay()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetEndOfDay(Expression rhs)
			: base("GetEndOfDay", rhs)
        {
        }
        
        /// <summary>
        /// Performs the operation defined by this function. This function expectes its parameter to be a date.
        /// The function will make every effort to convert the value to a date. 
        /// </summary>
        /// <param name="contextObject">A container object used to pass context at runtime.</param>
        /// <returns>An object representing the result of the evaluation</returns>
        /// <exception cref="Brierley.Framework.Common.Exceptions.CRMException">evaulate will throw an illegal expression exception if
        /// its operand is not a date or cannot be converted to a valid date. </exception>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                DateTime theDate = DateTime.Parse(this.GetRight().evaluate(contextObject).ToString());
				return DateTimeUtil.GetEndOfDay(theDate);
            }
            catch (Exception ex)
            {
				throw new CRMException("The GetEndOfDay function requires a date parameter. " + ex.Message);
            }
        }        
    }
}
