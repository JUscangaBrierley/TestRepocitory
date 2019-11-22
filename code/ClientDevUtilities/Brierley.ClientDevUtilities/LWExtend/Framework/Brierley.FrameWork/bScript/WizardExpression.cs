using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Brierley.FrameWork.bScript
{
	public class WizardExpression
	{
		public List<WizardItem> Items { get; set; }

		public string GetRawExpression()
		{
			StringBuilder ret = new StringBuilder();

			foreach (var item in Items)
			{
				item.AppendExpression(ret);
			}
			return ret.ToString();
		}

		public WizardExpression()
		{
			Items = new List<WizardItem>();
		}

		public void Validate()
		{
			if (Items == null || Items.Count == 0) return;

			if (Items.Count > 1)
			{
				Stack<string> stack = new Stack<string>();
				for (int index = 0; index < Items.Count; index++)
				{
					var item = Items[index];
					switch (item.type)
					{
						case "operator":
							if ((string)item.Data["Symbol"] == Symbols.NOT)
							{
								// unary operator: wait for operand
								stack.Push("operator:" + item.Data["Symbol"]);
							}
							else
							{
								// binary operator: ensure operand1 and wait for operand2
								if (stack.Count < 1 || !stack.Peek().StartsWith("operand:"))
								{
									throw new LWException(string.Format("Missing left operand for operator {0}", item.Data["Symbol"]));
								}
								else
								{
									stack.Push("operator:" + item.Data["Symbol"]);
								}
							}
							break;

						default:
							if (stack.Count < 1)
							{
								// first operand
								StringBuilder sb = new StringBuilder();
								item.AppendExpression(sb);
								stack.Push("operand:" + sb.ToString());
							}
							else
							{
								string peekVal = stack.Peek();
								if (!peekVal.StartsWith("operator:"))
								{
									StringBuilder sb = new StringBuilder();
									item.AppendExpression(sb);
									throw new LWException(string.Format("Missing operator between '{0}' and '{1}'", peekVal.Substring(peekVal.IndexOf(":")+1), sb.ToString()));
								}
								else
								{
									string operatorVal = stack.Pop();
									if (operatorVal == ("operator:" + Symbols.NOT))
									{
										StringBuilder sb = new StringBuilder();
										item.AppendExpression(sb);
										stack.Push("operand:" + Symbols.NOT + sb.ToString());
									}
									else 
									{
										if (stack.Count < 1 || !stack.Peek().StartsWith("operand:"))
										{
											throw new LWException(string.Format("Missing left operand for operator {0}", operatorVal));
										}
										else
										{
											string operand1Val = stack.Pop();
											operatorVal = operatorVal.Substring(operatorVal.IndexOf(":") + 1);
											StringBuilder sb = new StringBuilder();
											item.AppendExpression(sb);
											stack.Push(operand1Val + operatorVal + sb.ToString());
										}
									}
								}
							}
							break;
					}
				}

				if (stack.Count > 0 && !stack.Peek().StartsWith("operand:"))
				{
					string peekVal = stack.Peek();
					throw new LWException(
						string.Format("Missing right operand for expression '{0}'", 
						peekVal.Substring(peekVal.IndexOf(":")+1)
					));
				}
			}
			else if (Items[0].type == "operator")
			{
				throw new LWException(
					string.Format("Expression consists of single operator '{0}'", 
					Items[0].Data["Symbol"]
				));
			}
		}
	}

	public class WizardItem
	{
		public string type { get; set; }
		public string BuilderId { get; set; }
		public Dictionary<string, object> Data { get; set; }
		public string ManualString { get; set; }
		public List<WizardItem> Items { get; set; }

		public WizardItem()
		{
			Items = new List<WizardItem>();
			Data = new Dictionary<string, object>();
		}

		public void AppendExpression(StringBuilder sb)
		{
			//ensure at least one space between our expressions
			Action<StringBuilder> ensureSpace = delegate(StringBuilder s)
			{
				if (s.Length > 0)
				{
					char last = s[s.Length - 1];
					if (last != ' ' && last != '(' && last != '!')
					{
						s.Append(" ");
					}
				}
			};

			switch (type)
			{
				case "func":
					ensureSpace(sb);
					sb.Append(GetData("fname"));
					sb.Append("(");

					bool first = true;
					foreach (var item in this.Items)
					{
						var len = sb.Length;
						item.AppendExpression(sb);
						if (sb.Length > len)
						{
							if (!first)
							{
								sb.Insert(len, Symbols.PARAMETERSEPERATOR + " ");
							}
							first = false;
						}
					}
					sb.Append(")");
					break;

				case "helper":
					ensureSpace(sb);
					string op = GetData("op");
					string val = GetData("val");
					switch (BuilderId)
					{
						case "memberPropertyBuilder":
							if (op == "IsInSet")
							{
								sb.AppendFormat("IsInSet(Member.{0}, ExprWizSet({1}))", GetData("prop"), EnsureQuotes(op, val));
							}
							else if (op == "IsWithinDateRange")
							{
								sb.AppendFormat("IsDateWithinRange(Member.{0}, {1}, {2}, 'true')", GetData("prop"), EnsureQuotes(GetData("StartDate")), EnsureQuotes(GetData("EndDate")));
							}
							else
							{
								sb.AppendFormat("Member.{0} {1} {2}", GetData("prop"), op, EnsureQuotes(op, val));
							}
							break;

						case "memberAttributeBuilder":
							if (op == "IsInSet")
							{
								sb.AppendFormat("IsInSet(AttrValue({0}, {1}, 0), ExprWizSet({2}))", 
									EnsureQuotes(GetData("AttributeSet")), EnsureQuotes(GetData("Attribute")), EnsureQuotes(op, val));
							}
							else if (op == "IsWithinDateRange")
							{
								sb.AppendFormat("IsDateWithinRange(AttrValue({0}, {1}, 0), {2}, {3}, 'true')", EnsureQuotes(GetData("AttributeSet")), EnsureQuotes(GetData("Attribute")), EnsureQuotes(GetData("StartDate")), EnsureQuotes(GetData("EndDate")));
							}
							else
							{
								sb.AppendFormat("AttrValue({0}, {1}, 0) {2} {3}", 
									EnsureQuotes(GetData("AttributeSet")), EnsureQuotes(GetData("Attribute")), op, EnsureQuotes(op, val));
							}
							break;

						case "currentConditionBuilder":
							if (op == "IsInSet")
							{
								sb.AppendFormat("IsInSet(row.{0}, ExprWizSet({1}))", GetData("Attribute"), EnsureQuotes(op, val));
							}
							else if (op == "IsWithinDateRange")
							{
								sb.AppendFormat("IsDateWithinRange(row.{0}, {1}, {2}, 'true')", GetData("Attribute"), EnsureQuotes(GetData("StartDate")), EnsureQuotes(GetData("EndDate")));
							}
							else
							{
								sb.AppendFormat("row.{0} {1} {2}", GetData("Attribute"), op, EnsureQuotes(op, val));
							}
							break;

						case "historicalConditionBuilder":
							sb.AppendFormat("RowCount({0}, (", EnsureQuotes(GetData("AttributeSet")));
							first = true;
							foreach (JObject jobject in ((JArray)Data["MatchingConditions"]))
							{
								string json = jobject.ToString();
								MatchingCondition condition = JsonConvert.DeserializeObject<MatchingCondition>(json);
								if (!first)
								{
									sb.AppendFormat(" {0} ", condition.Condition == "AND" ? Symbols.AND : Symbols.OR);
								}
								first = false;
								if (!string.IsNullOrWhiteSpace(condition.LeftParen)) sb.Append(condition.LeftParen.Trim());
								if (condition.op == "IsInSet")
								{
									sb.AppendFormat("IsInSet(AttrValue({0}, {1}, 0), ExprWizSet({2}))",
										EnsureQuotes(condition.AttributeSet),
										EnsureQuotes(condition.Attribute),
										EnsureQuotes(condition.op, condition.val)
									);
								}
								else if (condition.op == "IsWithinDateRange")
								{
									sb.AppendFormat("IsDateWithinRange(AttrValue({0}, {1}, 0), {2}, {3}, 'true')",
										EnsureQuotes(condition.AttributeSet),
										EnsureQuotes(condition.Attribute),
										EnsureQuotes(condition.StartDate),
										EnsureQuotes(condition.EndDate)
									);
								}
								else
								{
									sb.AppendFormat("AttrValue({0}, {1}, 0) {2} {3}",
										EnsureQuotes(condition.AttributeSet),
										EnsureQuotes(condition.Attribute),
										condition.op,
										EnsureQuotes(condition.op, condition.val)
									);
								}
								if (!string.IsNullOrWhiteSpace(condition.RightParen)) sb.Append(condition.RightParen.Trim());
							}
							sb.AppendFormat(")) {0} {1}",
								GetData("rcop"),
								GetData("rc")
							);
							break;

						case "tierBuilder":
							string tierOperator = GetData("op");
							if (tierOperator == "IN")
							{
								string tiers = GetData("val");
								if (tiers.Contains(";"))
								{
									var tierArray = tiers.Split(';');
									sb.Append("(");
									for (int i = 0; i < tierArray.Length; i++)
									{
										if (i > 0)
										{
											sb.Append(" | ");
										}
										sb.AppendFormat("GetCurrentTierProperty('Name') == '{0}'", tierArray[i]);
									}
									sb.Append(")");
								}
								else
								{
									sb.AppendFormat("GetCurrentTierProperty('Name') == '{0}'", tiers);
								}
							}
							else
							{
								sb.AppendFormat("GetCurrentTierProperty('Name') {0} '{1}'", GetData("op"), GetData("val"));
							}
							break;

						case "pointBalanceBuilder":
							string functionName = GetData("FunctionName");
							string pointType = GetData("PointType");
							string pointTypes = GetData("PointTypes");
							string pointEvent = GetData("PointEvent");
							string pointEvents = GetData("PointEvents");
							string startDate = GetData("StartDate");
							string endDate = GetData("EndDate");
							string IncludeExpiredPoints = GetData("IncludeExpiredPoints");

							if (string.IsNullOrEmpty(startDate))
							{
								startDate = "GetFirstDateOfYear(Date())";
							}
							if (string.IsNullOrEmpty(endDate))
							{
								endDate = "Date()";
							}

							//wrap quotes around start and end dates, only if they are actual dates. Otherwise, the user had
							//entered an expression that should not get wrapped in quotes
							DateTime test = DateTime.Now;
							if (DateTime.TryParse(startDate, out test))
							{
								//date expression
								startDate = EnsureQuotes(startDate);
							}
							if (DateTime.TryParse(endDate, out test))
							{
								//non-date expression
								endDate = EnsureQuotes(endDate);
							}

							switch (functionName)
							{
								case "GetPoints":
									if (string.IsNullOrEmpty(pointType))
									{
										sb.AppendFormat("GetPoints({0}, {1})", startDate, endDate);
									}
									else
									{
										sb.AppendFormat("GetPoints('{0}', {1}, {2})", pointType, startDate, endDate);
									}
									break;

								case "GetEarnedPoints":
									if (string.IsNullOrEmpty(IncludeExpiredPoints))
									{
										sb.AppendFormat("GetEarnedPoints({0}, {1})", startDate, endDate);
									}
									else
									{
										sb.AppendFormat("GetEarnedPoints({0}, {1}, '{2}')", startDate, endDate, StringUtils.FriendlyBool(IncludeExpiredPoints, false));
									}
									break;

								case "GetEarnedPointsInSet":
									sb.AppendFormat("GetEarnedPointsInSet('{0}', '{1}', {2}, {3}", 
										StringUtils.FriendlyString(pointTypes), StringUtils.FriendlyString(pointEvents), startDate, endDate);
									if (string.IsNullOrEmpty(IncludeExpiredPoints))
									{
										sb.Append(")");
									}
									else
									{
										sb.AppendFormat(", '{0}')", StringUtils.FriendlyBool(IncludeExpiredPoints, false));
									}
									break;

								case "GetPointsByEvent":
									if (string.IsNullOrEmpty(pointEvent))
									{
										sb.AppendFormat("GetPointsByEvent({0}, {1})", startDate, endDate);
									}
									else
									{
										sb.AppendFormat("GetPointsByEvent('{0}', {1}, {2})", pointEvent, startDate, endDate);
									}
									break;

								case "GetPointsToNextReward":
									if (string.IsNullOrEmpty(pointType))
									{
										sb.Append("GetPointsToNextReward()");
									}
									else
									{
										sb.AppendFormat("GetPointsToNextReward('{0}')", pointType);
									}
									break;

								case "GetPointsToNextTier":
									if (string.IsNullOrEmpty(IncludeExpiredPoints))
									{
										sb.Append("GetPointsToNextTier()");
									}
									else
									{
										sb.AppendFormat("GetPointsToNextTier('{0}')", StringUtils.FriendlyBool(IncludeExpiredPoints, false));
									}
									break;
							}
							sb.AppendFormat(" {0} {1}", op, val);
							break;

						case "expressionBuilder":						
						case "literalNumberBuilder":
							sb.Append(GetData("val"));
							break;

						case "literalStringBuilder":
							sb.Append("'").Append(GetData("val")).Append("'");
							break;
					}
					break;

				case "group":
					ensureSpace(sb);
					sb.Append("(");
					foreach (var item in this.Items)
					{
						item.AppendExpression(sb);
					}
					sb.Append(")");
					break;

				case "parameter":
					if (Items.Count > 0)
					{
						foreach (var item in this.Items)
						{
							item.AppendExpression(sb);
						}
					}
					else if (!string.IsNullOrEmpty(this.ManualString))
					{
						//need an ensure quotes method that takes the parameter and function. the method will need to look
						//up the type of the parameter and type of string passed to determine whether or not to wrap in quotes
						sb.Append(EnsureQuotes(this.ManualString));
					}
					else
					{
						//need to know if the parameter is optional or not. If it's required, then a value must be provided
						string optional = GetData("Optional");
						if (!string.IsNullOrEmpty(optional) && !bool.Parse(optional))
						{
							ExpressionApplications parameterType = ExpressionApplications.Strings;
							string t = GetData("Type");
							if (!string.IsNullOrEmpty(t))
							{
								Enum.TryParse<ExpressionApplications>(t, out parameterType);
							}
							switch (parameterType)
							{
								case ExpressionApplications.Booleans:
									sb.Append("'False'");
									break;
								case ExpressionApplications.Dates:
									sb.Append("'" + DateTimeUtil.MinValue + "'");
									break;
								case ExpressionApplications.Numbers:
									sb.Append("0");
									break;
								default:
									sb.Append("''");
									break;
							}
						}
					}
					break;

				case "operator":
					ensureSpace(sb);
					sb.Append(GetData("Symbol"));
					break;

				default:
					throw new Exception("Failed to create expression - unknown item type " + type);
			}
		}

		private string GetData(string key)
		{
			if (Data.ContainsKey(key))
			{
				return Data[key].ToString();
			}
			return string.Empty;
		}

		private string EnsureQuotes(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return "''";
			}
			s = s.Trim();
			if (!s.StartsWith("'"))
			{
				s = "'" + s;
			}
			if (!s.EndsWith("'"))
			{
				s += "'";
			}
			return s;
		}

		/// <summary>
		/// Ensure quotes on string only if the operator is not a math operation and the value is not a numeric value
		/// </summary>
		/// <param name="op"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		private string EnsureQuotes(string op, string val)
		{
			if (op == Symbols.SUM || op == Symbols.DIFFERENCE || op == Symbols.PRODUCT || op == Symbols.QUOTIENT)
			{
				decimal v = 0;
				if(decimal.TryParse(val, out v))
				{
					return val;
				}
			}
			return EnsureQuotes(val);
		}

	}

	public class MatchingCondition
	{
		public string Condition { get; set; }
	    public string LeftParen { get; set; }
	    public string AttributeSet { get; set; }
	    public string Attribute { get; set; }
	    public string op { get; set; }
	    public string val { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
		public string RightParen { get; set; }
	}
}
