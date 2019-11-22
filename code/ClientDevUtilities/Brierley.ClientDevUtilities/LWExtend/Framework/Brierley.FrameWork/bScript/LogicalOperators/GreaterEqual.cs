using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.LogicalOperators
{
    /// <summary>
    /// The Greater Than or equal to operator <see cref="Brierley.Framework.bScript.Symbols.GREATEREQUAL"/> will attempt a comparison to determine whether or not the value of the left tree is Greater than or equal to
    /// the value of the right tree. To accomplish this task the operation will attempt to convert both sub-trees to double. An attempt to perform a Greater than
    /// or equal to operation on anything other than two numbers will return false.
    /// </summary>
    [Serializable]
	public class GreaterEqual : BinaryOperation
    {
        //private string _className = "GreaterEqual";
        LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="lhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal GreaterEqual(Expression lhs, Expression rhs)
            : base(Symbols.GREATEREQUAL, lhs, rhs)
        {
        }

        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            return test(GetLeft().evaluate(contextObject), GetRight().evaluate(contextObject));
        }

        private bool test(object lhs, object rhs)
        {
			if (lhs == null || rhs == null)
			{
				return false;
			}

			if (lhs is string)
			{
				return string.Compare(lhs.ToString(), rhs.ToString()) >= 0;
			}

			if (lhs is DateTime)
			{
				return (DateTime)lhs >= (DateTime)rhs;
			}

			if (lhs is double || lhs is decimal || lhs is short || lhs is int || lhs is long || lhs is float)
			{
				double left = Convert.ToDouble(lhs);
				double right = double.NaN;
				if (rhs is double || rhs is decimal || rhs is short || rhs is int || rhs is long || rhs is float)
				{
					right = Convert.ToDouble(rhs);
				}
				else
				{
					double.TryParse(rhs.ToString(), out right);
				}
				return left >= right;
			}

			return false;
        }
    }
}
