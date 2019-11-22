using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Returns the date corresponding to the last day of the quarter for the specified date.
    /// </summary>
    /// <example>
    ///     Usage : GetLastDateOfQuarter('date')
    ///     Usage : GetLastDateOfQuarter(Member.AccountOpenDate)
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the date corresponding to the last day of the quarter for the specified date.",
		DisplayName = "GetLastDateOfQuarter",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Dates,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "Last day of quarter", 
		AdvancedWizard = true
	)]
	[ExpressionParameter(Order = 0, Name = "Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "Which date?")]
    public class GetLastDateOfQuarter : UnaryOperation
    {
        /// <summary>
        /// Syntax definition for the function.
        /// </summary>
        public new string Syntax
        {
            get { return "GetLastDateOfQuarter(['date']|[Date()]|date expression)"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetLastDateOfQuarter()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetLastDateOfQuarter(Expression rhs)
            : base("GetLastDateOfQuarter", rhs)
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
                switch (theDate.Month)
                {
                    case 1:
                    case 4:
                    case 7:
                    case 10:
                        theDate = theDate.AddMonths(2);
                        break;

                    case 2:
                    case 5:
                    case 8:
                    case 11:
                        theDate = theDate.AddMonths(1);
                        break;

                    case 3:
                    case 6:
                    case 9:
                    case 12:
                        break;
                }
                while (theDate.AddDays((double)1).Month == theDate.Month)
                {
                    theDate = theDate.AddDays((double)1);
                }
                return theDate;
            }
            catch (Exception ex)
            {
                throw new CRMException("The GetLastDateOfQuarter function requires a date parameter. " + ex.Message);
            }
        }        
    }
}
