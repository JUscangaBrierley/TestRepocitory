using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The MAX function evaluates two numbers and returns the larger.
    /// </summary>
    /// <example>
    ///     Usage : Max(number, number)
    /// </example>
    /// <exception cref="CRMException">Illegal Expression exception if both operands are not numeric</exception>
    [Serializable]
	[ExpressionContext(Description = "The Max function evaluates two numbers and returns the larger.",
		DisplayName = "Max", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Numbers,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Max",
		AdvancedWizard = true)]

	[ExpressionParameter(Order = 0, Name = "number", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "First number?")]
	[ExpressionParameter(Order = 1, Name = "number1", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "Second number?")]
    public class MAX : BinaryOperation
    {
        /// <summary>
        /// Public Constructor
        /// </summary>
        public MAX()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="lhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal MAX(Expression lhs, Expression rhs)
            : base("MAX", lhs, rhs)
        {
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "Max(number1, number2)";
            }
        }
                
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                decimal val1 = decimal.Parse(GetLeft().evaluate(contextObject).ToString());
                decimal val2 = decimal.Parse(GetRight().evaluate(contextObject).ToString());
                return (decimal)System.Math.Max(val1, val2);
            }
            catch
            {
                throw new CRMException("Illegal Expression:The operands of a MAX function must be numeric");
            }
        }
    }
}
