using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// WeekEnd is a function that will compute the date for the ending day of the week in which
    /// the supplied date falls based on a Monday - Sunday week span.
    /// </summary>
    /// <example>
    ///     Usage : WeekEnd('date')
    ///     Usage : WeekEnd(Member.AccountOpenDate)
    /// </example>
    /// <remarks>Function names are not case sensative.</remarks>
    [Serializable]
	[ExpressionContext(Description = "WeekEnd is a function that will compute the date for the ending day of the week in which the supplied date falls based on a Monday - Sunday week span.", 
		DisplayName = "WeekEnd", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Dates, 
		ExpressionReturns = ExpressionApplications.Dates)]
    public class WeekEnd : UnaryOperation
    {

        /// <summary>
        /// The internal constructor.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal WeekEnd(Expression rhs)
            : base("WeekEnd", rhs)
        {
        }

        /// <summary>
        /// Public constructor used primarily by UI components.
        /// </summary>
        public WeekEnd()
        {
        }

        /// <summary>
        /// This method will return the functions syntax defintion.
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "WeekEnd(['date']|[Date()]|date expression)";
            }
        }
        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">A container object used to pass context at runtime.</param>
        /// <returns>An object representing the result of the evaluation</returns>
        /// <exception cref="Brierley.Framework.Common.Exceptions.CRMException">evaulate will throw an ellegal expression exception if
        /// its operand is not a date.</exception>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                System.DateTime theDate = System.DateTime.Parse(this.GetRight().evaluate(contextObject).ToString());
                switch (theDate.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return theDate.Add(new TimeSpan(6, 0, 0, 0));
                    case DayOfWeek.Tuesday:
                        return theDate.Add(new TimeSpan(5, 0, 0, 0));
                    case DayOfWeek.Wednesday:
                        return theDate.Add(new TimeSpan(4, 0, 0, 0));
                    case DayOfWeek.Thursday:
                        return theDate.Add(new TimeSpan(3, 0, 0, 0));
                    case DayOfWeek.Friday:
                        return theDate.Add(new TimeSpan(2, 0, 0, 0));
                    case DayOfWeek.Saturday:
                        return theDate.Add(new TimeSpan(1, 0, 0, 0));
                    case DayOfWeek.Sunday:
                        return theDate;
                    default:
                        return null;
                }
            }
            catch (System.Exception ex)
            {
                throw new CRMException("The WeekEnd function requires a date parameter. " + ex.Message);
            }
        }        
    }
}
