using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The IsNullOrEmpty takes an expression that should evaluate to a string and returns boolean.
    /// </summary>
    /// <example>
    ///     Usage : IsNullOrEmpty(TestExpression)
    /// </example>
    [Serializable]
	[ExpressionContext(Description = "Takes an expression that should evaluate to a string and returns boolean.", 
		DisplayName = "IsNullOrEmpty", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = ExpressionApplications.Strings | ExpressionApplications.Booleans,
		ExpressionReturns = ExpressionApplications.Booleans,

		WizardDescription = "Is null or empty?",
		AdvancedWizard = true)]

	[ExpressionParameter(Order = 0, Name = "Expression", Type = ExpressionApplications.All, Optional = false, WizardDescription = "Expression")]

    public class IsNullOrEmpty : UnaryOperation
    {        
        //Expression TestExpression = null;        

        /// <summary>
        /// Public Constructor
        /// </summary>
        public IsNullOrEmpty()
        {
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal IsNullOrEmpty(Expression rhs)
            : base("IsNullOrEmpty", rhs)
        {
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "IsNullOrEmpty(TestExpression)";
            }
        }
        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">An instance of ContextObject</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            string exprResult = (string)this.GetRight().evaluate(contextObject);
            //string exprResult = (string)TestExpression.evaluate(contextObject);
            return string.IsNullOrEmpty(exprResult);
        }        
    }
}
