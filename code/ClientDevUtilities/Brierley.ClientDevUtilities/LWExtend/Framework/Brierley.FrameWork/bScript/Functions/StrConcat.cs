using System;
using System.Collections.Generic;
using System.Text;


using Brierley.FrameWork.Common.Exceptions;
namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The StrConcat function takes two strings as concatenates them together end to end.
    /// </summary>
    /// <example>
    ///     Usage : STRConcat('string1','string2')
    /// </example>
    /// <remarks>The parameters supplied to STRConcat must both be string values.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Takes two strings as concatenates them together end to end.", 
		DisplayName = "StrConcat", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Strings, 
		ExpressionReturns = ExpressionApplications.Strings)]
    public class StrConcat : BinaryOperation
    {

        /// <summary>
        /// Public Constructor
        /// </summary>
        public StrConcat()
        {
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="rhs">an object of type <see cref="Brierley.Framework.bScript.Expression"/> contining the first first function parameter.</param>
        /// <param name="lhs">an object of type <see cref="Brierley.Framework.bScript.Expression"/> contining the first second function parameter.</param>
        internal StrConcat(Expression rhs, Expression lhs)
            : base("STRConcat", lhs, rhs)
        {
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "STRConcat('string1','string2')";
            }
        }
        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                string FirstString = this.GetLeft().evaluate(contextObject).ToString();
                string SecondString = this.GetRight().evaluate(contextObject).ToString();
                return String.Concat(FirstString, SecondString);
            }
            catch (System.Exception ex)
            {
                throw new CRMException("The string concatination function failed. " + ex.Message);
            }

        }        
    }
}
