using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The MIN function evaluates two numbers and returns the smaller of the two.
    /// </summary>
    /// <example>
    ///     Usage : Min(number, number)
    /// </example>
    [Serializable]
	[ExpressionContext(Description = "The Min function evaluates two numbers and returns the smaller of the two.", 
		DisplayName = "Min", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Min",
		AdvancedWizard = true)]

	[ExpressionParameter(Order = 0, Name = "number", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "First number?")]
	[ExpressionParameter(Order = 1, Name = "number1", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "Second number?")]
    public class MIN : BinaryOperation
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        public MIN()
        {
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="lhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal MIN(Expression lhs, Expression rhs)
            : base("MIN", lhs, rhs)
        {
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "Min(number1, number2)";
            }
        }
        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                decimal val1 = decimal.Parse(GetLeft().evaluate(contextObject).ToString());
                decimal val2 = decimal.Parse(GetRight().evaluate(contextObject).ToString());
                return System.Math.Min(val1, val2);
            }
            catch
            {
                throw new CRMException("Illegal Expression:The operands of a MIN function must be numeric");
            }
        }        
    }
}
