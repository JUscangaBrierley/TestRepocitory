using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.LogicalOperators
{
    /// <summary>
    /// Performs a logical "NOT" <see cref="Brierley.Framework.bScript.Symbols.NOT"/> operation on its operand. 
    /// </summary>
    [Serializable]
	public class LogicalNOT : UnaryOperation
    {
		private const string _className = "LogicalNOT";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal LogicalNOT(Expression rhs)
            : base(Symbols.NOT, rhs)
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
                return !(bool)GetRight().evaluate(contextObject);
            }
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
                throw new ApplicationException("Illegal Expression: The operand of a NOT operation must be boolean");
            }
        }        
    }
}
