using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// The If function evaluates expression expr1 and if true will then return the value of expr2. If false the
    /// if function will return the value of expr3
    /// </summary>
    /// <example>
    ///     Usage : If(expr1,expr2,expr3)
    /// </example>
    /// <remarks>
    /// All three parameters must be valid bScript expression.
    /// </remarks>
    [Serializable]
	[ExpressionContext(Description = "Evaluates expression expr1 and if true will then return the value of expr2. If false the if function will return the value of expr3", 
		DisplayName = "If", 
		ExcludeContext = 0, 
		ExpressionType = ExpressionTypes.Function, 
		ExpressionApplication = 0,
		ExpressionReturns = ExpressionApplications.Objects,

		WizardDescription = "If",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function)]

	[ExpressionParameter(Order = 0, Name = "Expression 1", Type = ExpressionApplications.All, Optional = false, WizardDescription = "Expression")]
	[ExpressionParameter(Order = 1, Name = "Expression 2", Type = ExpressionApplications.All, Optional = false, WizardDescription = "Then")]
	[ExpressionParameter(Order = 2, Name = "Expression 3", Type = ExpressionApplications.All, Optional = false, WizardDescription = "Else")]
	
	public class If : UnaryOperation
    {
        private Expression _expr1 = null;
        private Expression _expr2 = null;
        private Expression _expr3 = null;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public If()
        {
        }


        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        public If(Expression rhs)
            : base("If", rhs)
        {
			ParameterList plist = rhs as ParameterList;
			//ContextObject cObj = new ContextObject();
            if (plist.Expressions.Length == 3)
            {
                _expr1 = plist.Expressions[0];
                _expr2 = plist.Expressions[1];
                _expr3 = plist.Expressions[2];
                return;
            }
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to If Function.");
        }
        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "If(expr1,expr2,expr3)";
            }
        }
        /// <summary>
        /// Performs the operation defined by this operator
        /// </summary>
        /// <param name="contextObject">The context provided at runtime</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            if (System.Convert.ToBoolean(_expr1.evaluate(contextObject)) == true)
            {
                return _expr2.evaluate(contextObject);
            }
            else
            {
                return _expr3.evaluate(contextObject);
            }
        }        
    }
}
