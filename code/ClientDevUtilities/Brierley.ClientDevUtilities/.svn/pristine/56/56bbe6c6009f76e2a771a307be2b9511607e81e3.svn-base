using System;
using System.Collections.Generic;
using System.Text;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Retrieves a query string value, when executed in a web page.
	/// </summary>
	/// <example>
	///     Usage : QueryStringValue(parameter)
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Retrieves a query string value, when executed in a web page.",
		DisplayName = "QueryString",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings,
		ExpressionReturns = ExpressionApplications.Strings,
		WizardDescription = "Query string value",
		AdvancedWizard = true, WizardCategory = WizardCategories.Function
		)]

	[ExpressionParameter(Name = "Parameter", WizardDescription = "What query string parameter?", Type = ExpressionApplications.Strings, Optional = false)]
	public class QueryString : UnaryOperation
	{
		public QueryString()
		{
		}

		internal QueryString(Expression rhs)
			: base("QueryString", rhs)
		{
		}

		public new string Syntax
		{
			get
			{
				return "QueryString(parameter)";
			}
		}

		public override object evaluate(ContextObject contextObject)
		{
			if (System.Web.HttpContext.Current == null)
			{
				throw new CRMException("Illegal function call. The QueryString function must be evaluated in the context of a website.");
			}
			var parameter = Convert.ToString(this.GetRight().evaluate(contextObject));
			return System.Web.HttpContext.Current.Request.QueryString[parameter];
		}
	}
}
