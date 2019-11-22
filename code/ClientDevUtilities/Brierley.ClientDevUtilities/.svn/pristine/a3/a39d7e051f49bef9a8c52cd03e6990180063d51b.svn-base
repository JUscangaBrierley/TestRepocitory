using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Performs a ceiling function on its operand.
    /// </summary>
    /// <example>
    ///     Usage : Ceiling(number)
    /// </example>
    /// <remarks>The parameter value supplied to the ceiling function must be numeric</remarks>
    [Serializable]
    [ExpressionContext(Description = "Performs a ceiling function on its operand.",
        DisplayName = "Ceiling",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Numbers,
        ExpressionReturns = ExpressionApplications.Numbers,

        WizardDescription = "Least succeeding integer value",
        AdvancedWizard = true, WizardCategory = WizardCategories.Function

        )]

    [ExpressionParameter(Name = "Number", WizardDescription = "What Number?", Type = ExpressionApplications.Numbers, Optional = false)]
    public class Ceiling : UnaryOperation
    {
        public Ceiling()
        {
        }

        internal Ceiling(Expression rhs) : base("Ceiling", rhs)
        {

        }

        public new string Syntax
        {
            get
            {
                return "Ceiling(number)";
            }
        }

        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                return Math.Ceiling(Convert.ToDecimal(this.GetRight().evaluate(contextObject)));
            }
            catch
            {
                throw new CRMException("Illegal Expression: The operand of a Ceiling function must be numeric");
            }
        }
    }
}
