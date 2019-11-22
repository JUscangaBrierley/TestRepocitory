using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// The date constant class is similar in nature to the constant class except that it is designed specifically
    /// to handle date values. Strings that appear in an expression of the form 'mm/dd/yy' will be converted into
    /// date constant objects so that date operations such as subtract and add on the date object will function
    /// properly.
    /// </summary>
    [Serializable]
    public class DateConstant : Expression
    {
        private DateTime _value;

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="value"></param>
        internal DateConstant(DateTime value)
        {
            _value = value;
        }

        /// <summary>
        /// Performs the evaluation at runtime
        /// </summary>
        /// <param name="contextObject">The runtime context</param>
        /// <returns>A date object</returns>
        public override object evaluate(ContextObject contextObject)
        {
            return _value;
        }
        
        /// <summary>
        /// Returns a string representation of the value.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return _value.ToShortDateString();
        }

		public override string parseMetaData()
		{
			//convert to UTC string
			return string.Format("'{0}'", _value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
		}

	}
}
