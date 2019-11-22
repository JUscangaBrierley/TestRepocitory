using System;

using Brierley.FrameWork.Common.Exceptions;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Gets the value of the provided key from ContextObject.Environment.
	/// </summary>
	/// <example>
	///     Usage : GetEnvironmentString(key)
	/// </example>
	/// <remarks>Function names are not case sensitive.</remarks>
	[Serializable]
	[ExpressionContext(Description = "Gets the value of the provided key from ContextObject.Environment.",
		DisplayName = "GetEnvironmentString",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings,
		ExpressionReturns = ExpressionApplications.Strings,

		WizardDescription = "Get a value from the current context",
		AdvancedWizard = true,
		EvalRequiresMember = false)]

	[ExpressionParameter(Order = 0, Name = "Key", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "From which key?", Helpers = ParameterHelpers.EnvironmentKey)]
	public class GetEnvironmentString : UnaryOperation
	{
		/// <summary>
		/// Syntax definition for this function.
		/// </summary>
		public new string Syntax
		{
			get { return "GetEnvironmentString(key)"; }
		}

		/// <summary>
		/// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
		/// </summary>
		public GetEnvironmentString()
		{
		}

		/// <summary>
		/// The internal constructor for the function.
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetEnvironmentString(Expression rhs)
			: base("GetEnvironmentString", rhs)
		{
		}

		/// <summary>
		/// Performs the operation defined by this function. 
		/// </summary>
		/// <param name="contextObject">A container object used to pass context at runtime.</param>
		/// <returns>An object representing the result of the evaluation</returns>
		/// <exception cref="Brierley.Framework.Common.Exceptions.CRMException">thrown for illegal arguments</exception>
		public override object evaluate(ContextObject contextObject)
		{
			string ret = null;
			if (contextObject != null && contextObject.Environment != null)
			{
				string key = GetRight().evaluate(contextObject).ToString();
				ret = contextObject.Environment.ContainsKey(key) ? contextObject.Environment[key].ToString() : string.Empty;
			}
			if (ret == null)
			{
				ret = string.Empty;
			}
			return ret;
		}
	}
}
