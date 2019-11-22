using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The StrContains function returns a boolean value indicating whether the first string parameter contains
    /// the second string parameter.
    /// </summary>
    /// <example>
    ///     Usage : StrContains('mytest','test') : Return true
    /// </example>
    /// <remarks>Both parameters to StrContains must be string values.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns a boolean value indicating whether the first string parameter contains the second string parameter.", 
		DisplayName = "StrContains", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Strings | ExpressionApplications.Booleans, 
		ExpressionReturns = ExpressionApplications.Booleans)]
    public class STRContains : BinaryOperation
    {
        /// <summary>
        /// public constructor
        /// </summary>
        public STRContains()
        {
        }

        /// <summary>
        /// internal constructor
        /// </summary>
        /// <param name="rhs">an object of type <see cref="Brierley.Framework.bScript.Expression"/> contining the first first function parameter.</param>
        /// <param name="lhs">an object of type <see cref="Brierley.Framework.bScript.Expression"/> contining the first second function parameter.</param>
        internal STRContains(Expression rhs, Expression lhs)
            : base("STRContains", lhs, rhs)
        {
        }

        /// <summary>
        /// This method returns a string of the functions syntax definition
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "STRContains('string1','string2')";
            }
        }

        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <param name="contextObject">The context used by the function at runtime.</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                string InputString = this.GetRight().evaluate(contextObject).ToString();
                string SearchString = this.GetLeft().evaluate(contextObject).ToString();
                if (InputString.Contains(SearchString))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                throw new CRMException("The string contains function failed. " + ex.Message);
            }

        }        
    }
}
