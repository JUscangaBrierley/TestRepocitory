using System;
using System.Collections.Generic;
using System.Text;


using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Retrieves an expression's result from cache, if previously evaluated using CachedResult. Otherwise, caches the result for later retrieval.
	/// </summary>
	/// <example>
	///     Usage : CachedResult(GetPoints('type', from, to))
	/// </example>
	[Serializable]

	[ExpressionContext(Description = "Retrieves an expression's result from cache, if previously evaluated using CachedResult. Otherwise, caches the result for later retrieval.",
		DisplayName = "CachedResult",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.All,
		ExpressionReturns = ExpressionApplications.All,
		WizardDescription = "Cached result",
		AdvancedWizard = true, WizardCategory = WizardCategories.Function
		)]

	[ExpressionParameter(Name = "Expression", WizardDescription = "What expression?", Type = ExpressionApplications.All, Optional = false)]

	public class CachedResult : UnaryOperation
	{
		private string _expression = null;

		public CachedResult()
		{
		}

		internal CachedResult(Expression rhs)
			: base("CachedResult", rhs)
		{
			
			_expression = parseMetaData();
		}

		public new string Syntax
		{
			get
			{
				return "CachedResult(expression)";
			}
		}

		public override object evaluate(ContextObject contextObject)
		{
			if (contextObject.ResultCache == null)
			{
				contextObject.ResultCache = new Dictionary<string, object>();
			}

			if (!string.IsNullOrEmpty(_expression) && contextObject.ResultCache.ContainsKey(_expression))
			{
				return contextObject.ResultCache[_expression];
			}

			object ret = this.GetRight().evaluate(contextObject);
			if (!string.IsNullOrEmpty(_expression))
			{
				contextObject.ResultCache.Add(_expression, ret);
			}
			return ret;
		}
	}
}
