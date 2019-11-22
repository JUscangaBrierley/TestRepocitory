using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Performs a floor function on its operand.
    /// </summary>
    /// <example>
    ///     Usage : Floor(number)
    /// </example>
    /// <remarks>The parameter value supplied to the Floor function must be numeric</remarks>
    [Serializable]
    [ExpressionContext(Description = "Performs a floor function on its operand.",
        DisplayName = "Floor",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Numbers,
        ExpressionReturns = ExpressionApplications.Numbers,

        WizardDescription = "Greatest preceeding integer value",
        AdvancedWizard = true, WizardCategory = WizardCategories.Function

        )]

    [ExpressionParameter(Name = "Number", WizardDescription = "What Number?", Type = ExpressionApplications.Numbers, Optional = false)]
    public class Floor : UnaryOperation
    {
        public Floor()
        {
        }

        internal Floor(Expression rhs) : base("Floor", rhs)
        {

        }

        public new string Syntax
        {
            get
            {
                return "Floor(number)";
            }
        }

        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                return Math.Floor(Convert.ToDecimal(this.GetRight().evaluate(contextObject)));
            }
            catch
            {
                throw new CRMException("Illegal Expression: The operand of a Floor function must be numeric");
            }
        }
    }
}
