using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The ToString function returns the result of ToStrign on the object.
    /// </summary>
    /// <example>
    ///     Usage : ToString('test') : Return string
    /// </example>
    /// <remarks>Function names are not case sensative.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the result of the supplied expression's result.", 
		DisplayName = "ToString", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Strings | ExpressionApplications.Numbers, 
		ExpressionReturns = ExpressionApplications.Numbers)]
    public class ToString : UnaryOperation
    {
        /// <summary>
        /// Public constructor used by UI components
        /// </summary>
        public ToString()
        {
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal ToString(Expression rhs)
            : base("ToString", rhs)
        {
        }

        /// <summary>
        /// This method returns a string of the functions syntax definition.
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "ToString('string')";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                return (string)this.GetRight().evaluate(contextObject).ToString();
            }
            catch (System.Exception ex)
            {
                throw new CRMException("The string length function failed. " + ex.Message);
            }

        }        
    }
}
