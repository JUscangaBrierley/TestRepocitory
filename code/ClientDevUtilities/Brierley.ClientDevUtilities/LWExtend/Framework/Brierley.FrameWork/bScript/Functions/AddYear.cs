using System;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Add the specified number of years to the specified date.
	/// </summary>
	/// <example>
	///     Usage : AddYear(date, offset)
	/// </example>
	/// <remarks>Function names are not case sensitive.</remarks>
	[Serializable]
	[ExpressionContext(Description = "Add the specified number of years to the specified date.",
		DisplayName = "AddYear",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Dates,
		ExpressionReturns = ExpressionApplications.Dates,
		WizardCategory = WizardCategories.Dates,
		WizardDescription = "Add years to a date",
		AdvancedWizard = true
		)]

	[ExpressionParameter(Order = 1, Name = "Date", Type = ExpressionApplications.Dates, Optional = false, WizardDescription = "Which Date?")]
	[ExpressionParameter(Order = 2, Name = "Offset", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "How many years do you want to add?")]

	public class AddYear : UnaryOperation
	{
		//private DateTime dateParam;
		//private int offsetParam;

		/// <summary>
		/// Syntax definition for this function.
		/// </summary>
		public new string Syntax
		{
			get { return "AddYear(date, offset)"; }
		}

		/// <summary>
		/// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
		/// </summary>
		public AddYear()
		{
		}

		/// <summary>
		/// The internal constructor for the function.
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal AddYear(Expression rhs)
			: base("AddYear", rhs)
		{
			if (rhs is ParameterList && ((ParameterList)rhs).Expressions.Length == 2)
			{
				//ContextObject contextObject = new ContextObject();
				//dateParam = DateTime.Parse(((ParameterList)rhs).Expressions[0].evaluate(contextObject).ToString());
				//offsetParam = int.Parse(((ParameterList)rhs).Expressions[1].evaluate(contextObject).ToString());
				return;
			}
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to AddYear.");
		}

		/// <summary>
		/// Performs the operation defined by this function. 
		/// </summary>
		/// <param name="contextObject">A container object used to pass context at runtime.</param>
		/// <returns>An object representing the result of the evaluation</returns>
		/// <exception cref="Brierley.Framework.Common.Exceptions.CRMException">thrown for illegal arguments</exception>
		public override object evaluate(ContextObject contextObject)
		{
			ParameterList plist = GetRight() as ParameterList;
			DateTime dateParam = DateTime.Parse(((ParameterList)plist).Expressions[0].evaluate(contextObject).ToString());
			int offsetParam = int.Parse(((ParameterList)plist).Expressions[1].evaluate(contextObject).ToString());
			return dateParam.AddYears(offsetParam);
		}
	}
}
