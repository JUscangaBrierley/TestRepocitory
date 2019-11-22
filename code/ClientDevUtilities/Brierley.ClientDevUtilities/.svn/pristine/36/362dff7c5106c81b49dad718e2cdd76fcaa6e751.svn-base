using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// The StrCompare function compares two strings and returns an integer indicating their relative sort order
	/// the second string parameter.
	/// </summary>
	/// <example>
	///     Case sensitive comparison:
	///				StrCompare('a', 'b', true) : Returns -1, a is sorted before b
	///				StrCompare('b', 'b', true) : Returns 0, both strings are equal
	///				StrCompare('c', 'b', true) : Returns 1, c is sorted after b
	///		Case insensitive comparison:
	///				StrCompare('a', 'A', false) : Returns -1
	///				StrCompare('a', 'a', false) : Returns 0
	///				StrCompare('A', 'a', false) : Returns 1
	/// </example>
	[Serializable]
	[ExpressionContext(
		Description = "Compares two strings and returns an integer indicating their relative sort order.",
		DisplayName = "StrCompare",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings,
		ExpressionReturns = ExpressionApplications.Numbers,
		WizardDescription = "Compare two string values",
		WizardCategory = WizardCategories.Function)]

	[ExpressionParameter(Order = 0, Name = "First", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "First string?")]
	[ExpressionParameter(Order = 1, Name = "Second", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Second string?")]
	[ExpressionParameter(Order = 2, Name = "IgnoreCase", Type = ExpressionApplications.Booleans, Optional = true, WizardDescription = "Ignore case?")]
	public class StrCompare : UnaryOperation
	{
		private Expression _stringOne = null;
		private Expression _stringTwo = null;
		private Expression _ignoreCase = null;

		public StrCompare()
		{
		}

		internal StrCompare(Expression rhs)
			: base("StrCompare", rhs)
		{
			ParameterList plist = rhs as ParameterList;

			if (plist == null || plist.Expressions == null || plist.Expressions.Length < 2)
			{
				throw new CRMException("Invalid Function Call: Wrong number of arguments passed to StrCompare.");
			}

			_stringOne = plist.Expressions[0];
			_stringTwo = plist.Expressions[1];

			if (plist.Expressions.Length == 3)
			{
				_ignoreCase = plist.Expressions[2];
			}
		}

		public new string Syntax
		{
			get
			{
				return "StrCompare('string1','string2', ignoreCase)";
			}
		}

		public override object evaluate(ContextObject contextObject)
		{
			try
			{
				string one = Convert.ToString(_stringOne.evaluate(contextObject));
				string two = Convert.ToString(_stringTwo.evaluate(contextObject));
				bool ignoreCase = false;
				if (_ignoreCase != null)
				{
					ignoreCase = Convert.ToBoolean(_ignoreCase.evaluate(contextObject));
				}
				return string.Compare(one, two, ignoreCase);
			}
			catch (System.Exception ex)
			{
				throw new CRMException("The string compare bScript function failed. " + ex.Message);
			}
		}
	}
}
