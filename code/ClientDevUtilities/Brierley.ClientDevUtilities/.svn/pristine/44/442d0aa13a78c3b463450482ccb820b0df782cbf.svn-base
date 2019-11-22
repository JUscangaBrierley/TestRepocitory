using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// The Unary Operation class is an abstract class that underpins all the supported unary operators 
    /// such as the ! (NOT) operator.
    /// </summary>
    [Serializable]
    public abstract class UnaryOperation : Expression
    {
        private Expression _operand = null;
        private string theOp = string.Empty;


        /// <summary>
        /// Public Constructor
        /// </summary>
        public UnaryOperation()
        {
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="op">The string representation of the operation</param>
        /// <param name="operand">The expression upon which the Unary operation will act.</param>
        public UnaryOperation(String op, Expression operand)
        {
            _operand = operand;
            theOp = op;
        }


        /// <summary>
        /// returns a string representation of the operation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" +  theOp + " " + _operand.ToString() + ")";
        }


        /// <summary>
        /// returns the objects left subtree
        /// </summary>
        /// <returns>An object of type <see cref="Brierley.Framework.bScript.Expression"/></returns>
        public static Expression GetLeft()
        {
            return null;
        }


        /// <summary>
        /// Returns the objects right subtree
        /// </summary>
        /// <returns>An object of type <see cref="Brierley.Framework.bScript.Expression"/></returns>
        public Expression GetRight()
        {
            return _operand;
        }


        /// <summary>
        /// return the string representsion of the operator.
        /// </summary>
        public string Operator
        {
            get
            {
                return theOp;
            }
        }

		public override string parseMetaData()
		{
			string operand = null;
			if(_operand != null)
			{
				operand = _operand.parseMetaData();
			}
			return string.Format("{0}({1})", theOp, operand);
		}
	}
}
