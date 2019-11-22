using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.LogicalOperators
{
    /// <summary>
    /// The Equality operator <see cref="Brierley.Framework.bScript.Symbols.EQUALS"/> functions a little different then most others. In order to handle string equality correctly we cannot simply
    /// compare the objects returned by evaluate. Two objects even if they contain the same string are not equal to each other. The equality
    /// operator will examine the type of arguments contained in it's left and right sub-trees. If the type is string then it will assume your asking for a 
    /// string comparison of equality and it will convert the left tree and right tree to strings and perform the equality test. It will follow the same
    /// pattern for numbers. Thus 1=1 and '1' = '1' but 1 is not equal to '1'
    /// </summary>
    [Serializable]
	public class Equals : BinaryOperation
    {
		private const string _className = "Equals";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="lhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal Equals(Expression lhs, Expression rhs)
            : base(Symbols.EQUALS, lhs, rhs)
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
				return test(GetLeft().evaluate(contextObject), GetRight().evaluate(contextObject));
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
        }        

        private static bool test(object lhs, object rhs)
        {
			// If both sides are null then the comparison should be true because they are equal.  Hoever, if either
			// one of them is not null then it should be false.
			if (lhs == null && rhs == null)
			{
				return true;
			}
			else if (lhs == null || rhs == null)
			{
				return false;
			}
			if (lhs is string || lhs.GetType().IsEnum || rhs.GetType().IsEnum)
            {
				return lhs.ToString() == rhs.ToString();
            }

			if (lhs is DateTime)
			{
				return (DateTime)lhs == (DateTime)rhs;
			}

            if (lhs is bool || rhs is bool)
            {
                return bool.Parse(lhs.ToString()) == bool.Parse(rhs.ToString());
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
				return left == right;
            }

			return false;
        }
    }
}
