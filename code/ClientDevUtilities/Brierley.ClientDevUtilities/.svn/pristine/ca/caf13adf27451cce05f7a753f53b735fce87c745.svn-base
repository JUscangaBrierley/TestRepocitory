using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript
{
	public class ExpressionUtil
	{
		public static bool IsLibraryExpression(string expression)
		{
			if (string.IsNullOrEmpty(expression))
			{
				return false;
			}
			return expression.Contains(":") && expression.Trim().ToLower().StartsWith("library:");
		}

		public static bool IsLibraryExpression(string expression, out Bscript libraryExpression)
		{
			if (string.IsNullOrEmpty(expression))
			{
				libraryExpression = null;
				return false;
			}
			if (IsLibraryExpression(expression))
			{
				using (var svc = LWDataServiceUtil.DataServiceInstance())
				{
					string library = expression.Trim().Substring(8).Trim();
					long id = -1;
					if (long.TryParse(library, out id))
					{
						libraryExpression = svc.GetBscriptExpression(id);
					}
					else
					{
						libraryExpression = svc.GetBscriptExpression(library);
					}
					return true;
				}
			}
			libraryExpression = null;
			return false;
		}

		public static string GetLibraryName(string expression)
		{
			if (!IsLibraryExpression(expression))
			{
				throw new ArgumentException("Parameter expression must be a library expression (e.g., \"Library:<library expression name>\")");
			}
			return expression.Trim().Substring(8).Trim();
		}


		public static Expression Create(string expr)
		{
			return new ExpressionFactory().Create(expr);
		}

		public static object Evaluate(string expr, ContextObject contextObject)
		{
			return new ExpressionFactory().Create(expr).evaluate(contextObject);
		}

		public static string ParseExpressions(string text, ContextObject co)
		{
			Func<Match, string> eval = delegate(Match m)
			{
				try
				{
					string expression = m.Groups["FieldName"].ToString();
					Expression exp = null;
					exp = new ExpressionFactory().Create(expression);

					object value = exp.evaluate(co);
					if (value != null)
					{
						return value.ToString();
					}
					else
					{
						return string.Empty;
					}
				}
				catch
				{
					return m.ToString();
				}
			};
			if (text.StartsWith("{") && text.EndsWith("}"))
			{
				//may contain an expression built with the expression wizard
				try
				{
					var exp = Create(text);
					object value = exp.evaluate(co);
					if (value != null)
					{
						return value.ToString();
					}
					else
					{
						return string.Empty;
					}
				}
				catch
				{
					//maybe not
				}
			}
			return System.Text.RegularExpressions.Regex.Replace(text, @"\#\#(?<FieldName>.*?)\#\#", new MatchEvaluator(eval));
		}

	}
}
