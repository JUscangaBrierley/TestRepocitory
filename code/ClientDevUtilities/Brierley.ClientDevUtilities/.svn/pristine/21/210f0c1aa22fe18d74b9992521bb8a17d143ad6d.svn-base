using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// The Parameter list class is a type of expression that contains other expressions to be consumed by a 
    /// function. The expressions contained in this class will be arranged in the order they were encountered
    /// in the functions parameter list.
    /// </summary>
    [Serializable]
    public class ParameterList : Expression
    {

        /// <summary>
        /// Public constructor
        /// </summary>
        public Expression[] Expressions;

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="expressions">An array of expression objects</param>
        internal ParameterList(Expression[] expressions)
        {
            Expressions = expressions;
        }

        /// <summary>
        /// Performs the evaluation. Returns 0 for the parameter list class.
        /// </summary>
        /// <param name="contextObject"></param>
        /// <returns></returns>
        public override object evaluate(ContextObject contextObject)
        {
            return 0;
        }        

        /// <summary>
        /// Returns the string name of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Parameter List";
        }

		public override string parseMetaData()
		{
			var ret = new List<string>();
			foreach(var exp in Expressions)
			{
				ret.Add(exp.parseMetaData());
			}
			return string.Join(",", ret);
		}
	}
}
