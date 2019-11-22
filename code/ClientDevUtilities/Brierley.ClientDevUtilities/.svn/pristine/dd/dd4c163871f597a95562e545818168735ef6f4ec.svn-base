using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork;
using System.Xml.Linq;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class RealTimeInputQuery : Query
	{
		private Dictionary<string, object> _environment = null;
		private IEnumerable<Attribute> _attributes = null;
		private IEnumerable<CampaignAttribute> _campaignAttributes = null;

		public override List<SqlStatement> GetSqlStatement(bool IsValidationTest, Dictionary<string, string> overrideParameters = null)
		{
			return null;
		}

		/// <summary>
		/// Assembles the SQL statement for the verification (commit) process of the step
		/// </summary>
		/// <returns></returns>
		public override List<SqlStatement> GetVerifySqlStatement()
		{
			return null;
		}

		public override bool Validate(List<ValidationMessage> Warnings, bool ValidateSql)
		{
			return true;
		}


		internal override List<CampaignResult> Execute(ContextObject co = null, Dictionary<string, string> overrideParameters = null, bool resume = false)
		{
			if (co == null)
			{
				throw new ArgumentNullException("co");
			}
			if (co.Owner == null)
			{
				throw new ArgumentNullException("co.Owner");
			}

			int rowCount = 0;

			_environment = co.Environment ?? new Dictionary<string, object>();

			//environment may be null. we may be looking only at parameters.
			//if (_environment != null)
			//{
			//	var element = XElement.Parse(environment["input"]);
			List<int> conditionRows = GetConditionRows();

			foreach (int row in conditionRows)
			{
				if (EvaluateRow(row))
				{
					rowCount = 1;
					break;
				}
			}
			//}
			return new List<CampaignResult>() { new CampaignResult(rowCount) };
		}

		private bool EvaluateRow(int row, Dictionary<string, string> overrideParameters = null)
		{
			foreach (var qc in Columns)
			{
				string columnExpression = ConvertColumnExpression(qc.FieldName, overrideParameters);
				foreach (var cc in qc.Conditions.Where(o => o.RowOrder == row))
				{
					if (!EvaluateCondition(columnExpression, cc))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool EvaluateCondition(string key, ColumnCondition condition, Dictionary<string, string> overrideParameters = null)
		{
			bool ret = false;
			string val = key;
			//string val = string.Empty;
			//if (_environment.ContainsKey(key))
			//{
			//	val = _environment[key].ToString();
			//}
			//else
			//{
			//	//the field doesn't exist in environment, perhaps it's a parameter?
			//	foreach (var p in parameters)
			//	{
			//		string param = string.Format("@\"{0}\"", p.Name);
			//		if (text.IndexOf(param, StringComparison.OrdinalIgnoreCase) > -1)
			//		{
			//			fields.Parameters.Add(p);
			//		}
			//	}
			//}

			if ((condition.FieldFunction & StringFunctions.Length) == StringFunctions.Length)
			{
				int length = val.Length;
				int expression = int.Parse(condition.ConditionExpression);
				switch (condition.Operator)
				{
					case "<":
						ret = length < expression;
						break;
					case "<=":
						ret = length <= expression;
						break;
					case "=":
						ret = length == expression;
						break;
					case ">=":
						ret = length >= expression;
						break;
					case ">":
						ret = length > expression;
						break;
					case "<>":
						ret = length != expression;
						break;
					case "like":
					case "in":
						throw new Exception("Invalid operator " + condition.Operator + " for length function.");
				}
				return ret;
			}

			if ((condition.FieldFunction & StringFunctions.Trim) == StringFunctions.Trim)
			{
				val = val.Trim();
			}

			if ((condition.FieldFunction & StringFunctions.Lower) == StringFunctions.Lower)
			{
				val = val.ToLower();
			}

			if ((condition.FieldFunction & StringFunctions.Upper) == StringFunctions.Upper)
			{
				val = val.ToUpper();
			}


			switch (condition.Operator)
			{
				case "<":
					ret = string.Compare(val, condition.ConditionExpression) == -1;
					break;
				case "<=":
					ret = string.Compare(val, condition.ConditionExpression) <= 0;
					break;
				case ">=":
					ret = string.Compare(val, condition.ConditionExpression) >= 0;
					break;
				case ">":
					ret = string.Compare(val, condition.ConditionExpression) == 1;
					break;
				case "<>":
					ret = val != condition.ConditionExpression;
					break;
				case "like":
				case "in":
					throw new Exception("Invalid operator " + condition.Operator);
				case "=":
				default:
					ret = val == condition.ConditionExpression;
					break;
			}

			return ret;
		}

		private List<int> GetConditionRows()
		{
			var rows = new List<int>();

			foreach (var qc in Columns)
			{
				foreach (var cc in qc.Conditions)
				{
					if (!rows.Contains(cc.RowOrder))
					{
						rows.Add(cc.RowOrder);
					}
				}
			}

			return rows;
		}

		
		private string ConvertColumnExpression(string expression, Dictionary<string, string> overrideParameters)
		{
			if (_environment != null && _environment.ContainsKey(expression))
			{
				return _environment[expression].ToString();
			}

			string ret = expression;
			if (string.IsNullOrEmpty(expression) || !expression.Contains("@\"") || !Step.CampaignId.HasValue)
			{
				return ret;
			}

			if(_attributes == null)
			{
                using (var mgr = LWDataServiceUtil.CampaignManagerInstance())
                {
                    _attributes = mgr.GetAllAttributes() ?? new List<Attribute>();
                    if (_attributes.Count() > 0)
                    {
                        _campaignAttributes = mgr.GetAllCampaignAttributes(Step.CampaignId.Value);
                    }
                }
			}
			
			if (_attributes != null && _attributes.Count() > 0)
			{
				foreach (var attribute in _attributes)
				{
					if (expression.IndexOf(string.Format("@\"{0}\"", attribute.Name), StringComparison.OrdinalIgnoreCase) > -1)
					{
						if (overrideParameters != null && overrideParameters.ContainsKey(attribute.Name))
						{
							ret = overrideParameters[attribute.Name];
						}
						else
						{
							ret = _campaignAttributes.Where(o => o.AttributeId == attribute.Id).Select(o => o.AttributeValue).FirstOrDefault();
						}
						break;
					}
				}
			}
			return ret;
		}

	}
}