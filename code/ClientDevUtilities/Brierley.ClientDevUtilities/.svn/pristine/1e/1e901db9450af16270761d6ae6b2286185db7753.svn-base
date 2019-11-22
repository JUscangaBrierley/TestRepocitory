using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.bScript
{
    /// <summary>
    /// BinaryOperation is an abstract class that provides all the basic functionality required by an
    /// expression of the form x operator y.
    /// </summary>
    [Serializable]
    public abstract class BinaryOperation : Expression
    {
        private Expression leftTree = null;
        private Expression rightTree = null;
        private string theOp = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="op">The string representation of the operation</param>
        /// <param name="lhs">An expression object representing the left sub-tree</param>
        /// <param name="rhs">An expression object representing the right sub-tree</param>
        internal BinaryOperation(String op, Expression lhs, Expression rhs)
        {
            leftTree = lhs;
            rightTree = rhs;
            theOp = op;
        }

        /// <summary>
        /// Public constructor for UI purposes.
        /// </summary>
        public BinaryOperation()
        {
        }

        /// <summary>
        /// A string representation of the operation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + leftTree.ToString() + " " + theOp + " " + rightTree.ToString() + ")";
        }

        /// <summary>
        /// Returns the left subtree of the operation
        /// </summary>
        /// <returns>An expression</returns>
        public Expression GetLeft()
        {
            return leftTree;
        }

        /// <summary>
        /// Returns the right subtree of the operation
        /// </summary>
        /// <returns>An expression</returns>
        public Expression GetRight()
        {
            return rightTree;
        }

        /// <summary>
        /// Returns the string representation of the operator
        /// </summary>
        public string Operator
        {
            get
            {
                return theOp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string parseMetaData()
        {
            string left = leftTree.parseMetaData();
            string right = rightTree.parseMetaData();
			return string.Format("{0} {1} {2}", left, theOp, right);
        }
    }
}
