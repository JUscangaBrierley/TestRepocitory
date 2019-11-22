using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.LogicalOperators
{
    /// <summary>
    /// Performs a logical "OR" <see cref="Brierley.Framework.bScript.Symbols.OR"/> operation on its left and right subtrees.
    /// </summary>
    [Serializable]
	public class LogicalOR : BinaryOperation
    {
		private const string _className = "LogicalOR";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="lhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal LogicalOR(Expression lhs, Expression rhs)
            : base(Symbols.OR, lhs, rhs)
        {
        }

        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
			const string methodName = "evaluate";
			Expression leftTree = GetLeft();
			try
			{
				bool leftValue = false;
				if (leftTree != null)
				{
					leftValue = (bool)leftTree.evaluate(contextObject);
				}
				if (leftValue) return true;
			}
			catch (Exception ex)
			{
				string msg = string.Format("Unexpected error evaluating '{0}': {1}",
					leftTree != null ? leftTree.ToString() : string.Empty, ex.Message);
				_logger.Error(_className, methodName, msg, ex);
				throw new CRMException(msg, ex);
			}

			// at this point leftValue=false
			Expression rightTree = GetRight();
			try
			{
				bool rightValue = false;
				if (rightTree != null)
				{
					rightValue = (bool)rightTree.evaluate(contextObject);
				}
				return rightValue;
			}
			catch (Exception ex)
			{
				string msg = string.Format("Unexpected error evaluating '{0}': {1}",
					rightTree != null ? rightTree.ToString() : string.Empty, ex.Message);
				_logger.Error(_className, methodName, msg, ex);
				throw new CRMException(msg, ex);
			}
        }
    }
}
