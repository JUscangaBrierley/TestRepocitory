using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// WeekStart is a function that will compute the date for the starting day of the week in which
    /// the supplied date falls based on a Monday - Sunday week span.
    /// </summary>
    /// <example>
    ///     Usage : WeekStart('date')
    ///     Usage : WeekStart(Member.AccountOpenDate)
    /// </example>
    /// <remarks>Function names are not case sensative.</remarks>
    [Serializable]
	[ExpressionContext(Description = "WeekStart is a function that will compute the date for the starting day of the week in which the supplied date falls based on a Monday - Sunday week span.", 
		DisplayName = "WeekStart", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Dates, 
		ExpressionReturns = ExpressionApplications.Dates)]
    public class WeekStart : UnaryOperation
    {

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal WeekStart(Expression rhs)
            : base("WeekStart", rhs)
        {

        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public WeekStart()
        {
        }


        /// <summary>
        /// This method will return a string containing the functions syntax definition.
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "WeekStart(['date']|[Date()]|date expression)";
            }
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
                System.DateTime theDate = System.DateTime.Parse(this.GetRight().evaluate(contextObject).ToString());
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
            catch (System.Exception ex)
            {
                throw new CRMException("The WeekStart function requires a date parameter. " + ex.Message);
            }

        }        
    }
}
