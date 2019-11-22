using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Add the specified number of hours to the specified date.
    /// </summary>
    /// <example>
    ///     Usage : AddHour(date, offset)
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Add the specified number of hours to the specified date.",
        DisplayName = "AddHour",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Dates,
        ExpressionReturns = ExpressionApplications.Dates,
        WizardCategory = WizardCategories.Dates,
        WizardDescription = "Add hours to a date",
        AdvancedWizard = true
    )]
    [ExpressionParameter(Order = 1, Name = "Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "Which Date?")]
    [ExpressionParameter(Order = 2, Name = "Offset", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "How many hours do you want to add?")]
    public class AddHour : UnaryOperation
    {
        public new string Syntax
        {
            get { return "AddHour(date, offset)"; }
        }

        public AddHour()
        {
        }

        internal AddHour(Expression rhs)
            : base("AddHour", rhs)
        {
            if (!(rhs is ParameterList) || ((ParameterList)rhs).Expressions.Length != 2)
            {
                throw new CRMException("Invalid Function Call: Wrong number of arguments passed to AddHour.");
            }
        }

        public override object evaluate(ContextObject contextObject)
        {
            var parameters = GetRight() as ParameterList;

            object result = parameters.Expressions[0].evaluate(contextObject);
            if (result == null)
            {
                throw new CRMException("The AddHour function requires a date parameter.");
            }

            DateTime date = DateTime.MinValue;

            if (result != null)
            {
                if (result is DateTime)
                {
                    date = (DateTime)result;
                }
                else if (result is IConvertible)
                {
                    date = Convert.ToDateTime(result);
                }
                else
                {
                    date = DateTime.Parse(result.ToString());
                }
            }
            
            double offsetParam = Convert.ToDouble(parameters.Expressions[1].evaluate(contextObject));
            return date.AddHours(offsetParam);
        }
    }
}
