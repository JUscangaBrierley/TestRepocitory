using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Get the day component of the specified date.
    /// </summary>
    /// <example>
    ///     Usage : GetDay(date)
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Get the day component of the specified date.",
		DisplayName = "GetDay",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Numbers)]
    public class GetDay : UnaryOperation
    {
        /// <summary>
        /// Syntax definition for this function.
        /// </summary>
        public new string Syntax
        {
            get { return "GetDay(date)"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetDay()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal GetDay(Expression rhs)
            : base("GetDay", rhs)
        {
        }

        /// <summary>
        /// Performs the operation defined by this function. 
        /// </summary>
        /// <param name="contextObject">A container object used to pass context at runtime.</param>
        /// <returns>An object representing the result of the evaluation</returns>
        /// <exception cref="Brierley.Framework.Common.Exceptions.CRMException">thrown for illegal arguments</exception>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                return DateTime.Parse(GetRight().evaluate(contextObject).ToString()).Day;
            }
            catch (Exception)
            {
                throw new CRMException("Illegal Expression: The operand of the GetDay function must be a DateTime");
            }
        }        
    }
}
