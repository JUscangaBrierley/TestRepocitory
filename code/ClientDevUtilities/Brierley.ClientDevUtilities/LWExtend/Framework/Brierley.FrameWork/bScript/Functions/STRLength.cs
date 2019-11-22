using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The StrLength function returns lenght of the supplied string.
    /// </summary>
    /// <example>
    ///     Usage : StrLength('test') : Return 4
    /// </example>
    /// <remarks>Function names are not case sensative.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns the length of the supplied string.", 
		DisplayName = "StrLength", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Strings | ExpressionApplications.Numbers, 
		ExpressionReturns = ExpressionApplications.Numbers)]
    public class STRLength : UnaryOperation
    {
        /// <summary>
        /// Public constructor used by UI components
        /// </summary>
        public STRLength()
        {
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal STRLength(Expression rhs)
            : base("STRLength", rhs)
        {
        }

        /// <summary>
        /// This method returns a string of the functions syntax definition.
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "STRLength('string')";
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
                return (Decimal)this.GetRight().evaluate(contextObject).ToString().Length;
            }
            catch (System.Exception ex)
            {
                throw new CRMException("The string length function failed. " + ex.Message);
            }

        }        
    }
}
