using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// The Constant class is a form of expression. It simply defines a numeric value that is acting as a constant 
    /// in an expression.
    /// </summary>
    [Serializable]
    public class Constant : Expression
    {
        private decimal _value;

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="value">The Constant Value</param>
        internal Constant(decimal value)
        {
            _value = value;
        }

        /// <summary>
        /// Evaluates and returns the value of the constant at runtime.
        /// </summary>
        /// <param name="contextObject">The evaluation context. Not used by this class.</param>
        /// <returns>An object representing the value of the constant. The bScript expression engine
        /// deals in decimal numbers. So, if the constant is 10 you will get a type decimal object back with
        /// a value of 10.
        /// </returns>
        public override object evaluate(ContextObject contextObject)
        {
            return _value;
        }
        
        /// <summary>
        /// The string representation of the operation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _value.ToString();
        }

		public override string parseMetaData()
		{
			return _value.ToString();
		}
	}
}
