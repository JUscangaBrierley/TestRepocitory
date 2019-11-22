using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The NextWholeNumber function returns the next whole number up from the supplied number.
	/// </summary>
	/// <example>
	///     Usage : NextWholeNumber(number)
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns the next whole number up from the supplied number.",
		DisplayName = "NextWholeNumber",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers,
		ExpressionReturns = ExpressionApplications.Numbers,
		WizardDescription = "Next whole number",
		AdvancedWizard = true)]

	[ExpressionParameter(Order = 0, Name = "number", Type = ExpressionApplications.Numbers, Optional = false, WizardDescription = "What number?")]
	public class NextWholeNumber : UnaryOperation
	{
		/// <summary>
		/// Public Constructor
		/// </summary>
		public NextWholeNumber()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal NextWholeNumber(Expression rhs)
			: base("NextWholeNumber", rhs)
		{
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "NextWholeNumber(number)";
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
			Object obj = null;
			try
			{
				obj = this.GetRight().evaluate(contextObject);
				Decimal operand = 0;
				if (obj is String)
				{
					operand = Decimal.Parse(obj.ToString());
				}
				else
				{
					operand = (Decimal)obj;
				}
				return System.Math.Ceiling(operand);
			}
			catch
			{
				throw new CRMException(string.Format(
					"Illegal Expression: The operand of NextWholeNumber function must be numeric{0}", 
					obj != null ? " but is of type " + obj.GetType().Name : string.Empty
				));
			}
		}
	}
}
