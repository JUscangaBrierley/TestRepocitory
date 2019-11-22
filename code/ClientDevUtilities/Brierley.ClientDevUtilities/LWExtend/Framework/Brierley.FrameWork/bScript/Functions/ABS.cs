using System;
using System.Collections.Generic;
using System.Text;


using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Performs an absolute value operation on its operand.
    /// </summary>
    /// <example>
    ///     Usage : ABS(number)
    /// </example>
    /// <remarks>The parameter value supplied to the ABS function must be numeric</remarks>
    [Serializable]
	[ExpressionContext(Description = "Performs an absolute value operation on its operand.",
		DisplayName = "Abs",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Absolute numerical value",
		AdvancedWizard = true, WizardCategory = WizardCategories.Function

		)]

	[ExpressionParameter(Name = "Number", WizardDescription = "What Number?", Type = ExpressionApplications.Numbers, Optional = false)]
    public class ABS : UnaryOperation
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        public ABS()
        {
        }


        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
        internal ABS(Expression rhs)
            : base("ABS", rhs)
        {
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "ABS(number)";
            }
        }
        /// <summary>
        /// Performs the operation defined by this function.
        /// </summary>
        /// <returns>An object representing the result of the evaluation</returns>
        /// <exception cref="CRMException">evaulate will throw an ellegal expression exception if
        /// its operand is non numeric.</exception>
        public override object evaluate(ContextObject contextObject)
        {
            try
            {                
				return System.Math.Abs(Convert.ToDecimal(this.GetRight().evaluate(contextObject)));
                
            }
            catch
            {
                throw new CRMException("Illegal Expression:The operand of an ABS function must be numeric");
            }
        }        
    }
}
