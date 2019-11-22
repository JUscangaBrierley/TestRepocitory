using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.MathOperators
{
    /// <summary>
    /// The difference class implements the subtraction operator. "-"
    /// </summary>
    [Serializable]
	public class Difference : BinaryOperation
    {
        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="lhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal Difference(Expression lhs, Expression rhs)
            : base("-", lhs, rhs)
        {
        }

        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                decimal left = System.Decimal.Parse(GetLeft().evaluate(contextObject).ToString());
                decimal right = System.Decimal.Parse(GetRight().evaluate(contextObject).ToString());
                return left - right;
            }
            catch
            {
                throw new CRMException("Illegal Expression:Both operands of a difference operation must be numeric");
            }
        }        
    }
}
