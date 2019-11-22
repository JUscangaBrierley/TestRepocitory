using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript.MathOperators
{
    /// <summary>
    /// The Sum class implements the addition operator. "+"
    /// </summary>
    [Serializable]
	public class Sum : BinaryOperation
    {
        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="lhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal Sum(Expression lhs, Expression rhs)
            : base("+", lhs, rhs)
        {
        }

        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            object left = GetLeft().evaluate(contextObject);
            object right = GetRight().evaluate(contextObject);
            if (left.GetType() == typeof(String) & right.GetType() == typeof(String))
            {
                return left.ToString() + right.ToString();
            }
            else if (left.GetType() == typeof(DateTime) & (right.GetType() == typeof(Double) || right.GetType() == typeof(Decimal)))
            {
                DateTime leftVal = (DateTime)left;
                Double rightVal = Double.Parse(right.ToString());
                return leftVal.AddDays(rightVal);
            }
            else
            {
                decimal val1 = decimal.Parse(GetLeft().evaluate(contextObject).ToString());
                decimal val2 = decimal.Parse(GetRight().evaluate(contextObject).ToString());
                decimal val3 = val1 + val2;
                if (left.GetType() != right.GetType() || left.GetType() == typeof(decimal))
                {
                    // return higher precision
                    return val3;
                }
                else
                {
                    return (Int64)val3;
                }
            }    
        }        
    }
}
