using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.LogicalOperators
{
    /// <summary>
    /// The Not Equal operator <see cref="Brierley.Framework.bScript.Symbols.NOTEQUAL"/> will return true if it's operands are not equal to each other. False otherwise.
    /// </summary>
    [Serializable]
	public class NotEqual : BinaryOperation
    {
		private const string _className = "NotEqual";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="lhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal NotEqual(Expression lhs, Expression rhs)
            : base(Symbols.NOTEQUAL, lhs, rhs)
        {
        }
        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
			const string methodName = "evaluate";
			try
			{
				object lhs = GetLeft().evaluate(contextObject);
				object rhs = GetRight().evaluate(contextObject);
				return test(lhs, rhs);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
        }

        private static bool test(object lhs, object rhs)
        {
			if (lhs == null && rhs == null)
			{
				return false;
			}
			if (lhs == null || rhs == null)
			{
				return true;
			}

			if (lhs is string || lhs.GetType().IsEnum || rhs.GetType().IsEnum)
			{
				return string.Compare(lhs.ToString(), rhs.ToString()) != 0;
			}

			if (lhs is DateTime)
			{
				return (DateTime)lhs != (DateTime)rhs;
			}

            if (lhs is bool || rhs is bool)
            {
                return bool.Parse(lhs.ToString()) != bool.Parse(rhs.ToString());
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
				return left != right;
			}

			return false;
        }

    }
}
