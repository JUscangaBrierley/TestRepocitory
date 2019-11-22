using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Returns the date corresponding to the first day of the year for the specified date.
    /// </summary>
    /// <example>
    ///     Usage : GetFirstDateOfYear('date')
    ///     Usage : GetFirstDateOfYear(Member.AccountOpenDate)
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the date corresponding to the first day of the year for the specified date.",
		DisplayName = "GetFirstDateOfYear",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Dates,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "First day of year", 
		AdvancedWizard = true
	)]
	[ExpressionParameter(Order = 0, Name = "Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "Which date?")]
    public class GetFirstDateOfYear : UnaryOperation
    {
        /// <summary>
        /// Syntax definition for the function.
        /// </summary>
        public new string Syntax
        {
            get { return "GetFirstDateOfYear(['date']|[Date()]|date expression)"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetFirstDateOfYear()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetFirstDateOfYear(Expression rhs)
            : base("GetFirstDateOfYear", rhs)
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
                if (theDate.Month != 1)
                {
                    theDate = theDate.AddMonths(1 - theDate.Month);
                }
                if (theDate.Day != 1)
                {
                    theDate = theDate.AddDays((double)(1 - theDate.Day));
                }
                return theDate;
            }
            catch (Exception ex)
            {
                throw new CRMException("The GetFirstDateOfYear function requires a date parameter. " + ex.Message);
            }
        }        
    }
}
