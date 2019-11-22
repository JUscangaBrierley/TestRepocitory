using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The IsNull function returns true if the result of the expression evalues to a null object.
    /// </summary>
    /// <example>
    ///     Usage : IsNull('test') : Return bool
    /// </example>
    /// <remarks>Function names are not case sensative.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Returns true if the result of the expression evaluates to true.",
        DisplayName = "IsNull", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Strings | ExpressionApplications.Numbers, 
		ExpressionReturns = ExpressionApplications.Numbers,
		WizardDescription = "Is null?",
		AdvancedWizard = true)]
	[ExpressionParameter(Order = 1, Name = "Expression", Type = ExpressionApplications.Objects, Optional = false, WizardDescription = "Is null?")]
    public class IsNull : UnaryOperation
    {
        /// <summary>
        /// Public constructor used by UI components
        /// </summary>
        public IsNull()
        {
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/></param>
        internal IsNull(Expression rhs)
            : base("IsNull", rhs)
        {
        }

        /// <summary>
        /// This method returns a string of the functions syntax definition.
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "IsNull('expression')";
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
                object result = this.GetRight().evaluate(contextObject);
                return result == null;                
            }
            catch (System.Exception ex)
            {
                throw new CRMException("The IsNull function failed. " + ex.Message);
            }

        }        
    }
}
