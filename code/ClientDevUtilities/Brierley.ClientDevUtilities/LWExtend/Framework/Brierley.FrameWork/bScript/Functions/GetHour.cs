using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Get the hour component of the specified date.
    /// </summary>
    /// <example>
    ///     Usage : GetHour(date)
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Get the hour component of the specified date.",
        DisplayName = "GetHour",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Dates,
        ExpressionReturns = ExpressionApplications.Numbers)]
    public class GetHour : UnaryOperation
    {
        public new string Syntax
        {
            get { return "GetHour(date)"; }
        }

        public GetHour()
        {
        }

        internal GetHour(Expression rhs)
            : base("GetHour", rhs)
        {
        }

        public override object evaluate(ContextObject contextObject)
        {
            try
            {
                return DateTime.Parse(GetRight().evaluate(contextObject).ToString()).Hour;
            }
            catch (Exception)
            {
                throw new CRMException("Illegal Expression: The operand of the GetHour function must be a DateTime");
            }
        }
    }
}
