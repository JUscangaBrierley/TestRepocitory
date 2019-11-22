using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// GetFirstDateOfWeek is a function that will compute the date for the starting day of the week in which
    /// the supplied date falls based on a Monday - Sunday week span.
    /// </summary>
    /// <example>
    ///     Usage : GetFirstDateOfWeek('date')
    ///     Usage : GetFirstDateOfWeek(Member.AccountOpenDate)
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "GetFirstDateOfWeek is a function that will compute the date for the starting day of the week in which the supplied date falls based on a Monday - Sunday week span.",
		DisplayName = "GetFirstDateOfWeek",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Dates,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "First day of week", 
		AdvancedWizard = true
	)]
	[ExpressionParameter(Order = 0, Name = "Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "Which date?")]
    public class GetFirstDateOfWeek : UnaryOperation
    {
        /// <summary>
        /// Syntax definition for the function.
        /// </summary>
        public new string Syntax
        {
            get { return "GetFirstDateOfWeek(['date']|[Date()]|date expression)"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetFirstDateOfWeek()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetFirstDateOfWeek(Expression rhs)
            : base("GetFirstDateOfWeek", rhs)
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
                switch(theDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return theDate;
                    case DayOfWeek.Tuesday:
                        return theDate.Subtract(new TimeSpan(1, 0, 0, 0));
                    case DayOfWeek.Wednesday:
                        return theDate.Subtract(new TimeSpan(2, 0, 0, 0));
                    case DayOfWeek.Thursday:
                        return theDate.Subtract(new TimeSpan(3, 0, 0, 0));
                    case DayOfWeek.Friday:
                        return theDate.Subtract(new TimeSpan(4, 0, 0, 0));
                    case DayOfWeek.Saturday:
                        return theDate.Subtract(new TimeSpan(5, 0, 0, 0));
                    case DayOfWeek.Sunday:
                        return theDate.Subtract(new TimeSpan(6, 0, 0, 0));
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw new CRMException("The GetFirstDateOfWeek function requires a date parameter. " + ex.Message);
            }
        }        
    }
}
