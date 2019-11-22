using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;


namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class SurveyQuery : Query
	{
		public override void EnsureSchema()
		{
			return;
		}

		public override List<SqlStatement> GetSqlStatement(Dictionary<string, string> overrideParameters = null)
		{
			return null;
		}

		public override System.Data.DataTable GetDataSample(string[] groupBy)
		{
			return null;
		}

		public override List<SqlStatement> GetVerifySqlStatement()
		{
			return null;
		}

		public override bool Validate(List<ValidationMessage> Warnings, bool ValidateSql)
		{
			/*
			if (<required condition not met>)
			{
				Warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Expression is required for step {0}", Step.UIName)));
				return false;
			}
			*/
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

			long ipCode = -1;
			if (co != null && co.Owner is Member)
			{
				ipCode = ((Member)co.Owner).IpCode;
			}

			int rowCount = 0;

			List<int> conditionRows = GetConditionRows();

			foreach (int row in conditionRows)
			{
				if (EvaluateRow(ipCode, row))
				{
					rowCount = 1;
					break;
				}
			}

			return new List<CampaignResult>() { new CampaignResult(rowCount) };
		}

		private bool EvaluateRow(long ipCode, int row)
		{
			foreach (var qc in Columns)
			{
				foreach (var cc in qc.Conditions.Where(o => o.RowOrder == row))
				{
					if (!EvaluateCondition(ipCode, qc.TableId, cc))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool EvaluateCondition(long ipCode, long questionContentId, ColumnCondition condition)
		{
			using (var sm = LWDataServiceUtil.SurveyManagerInstance())
			{

				SMQuestionContent qc = sm.RetrieveQuestionContent(questionContentId);

				//todo: this is a performance issue - we're forced to retrieve the respondent based on language, but we don't know which 
				//language the survey was presented in to the member, so we're having to brute force it. Surveys are typically presented
				//in English, but there's no guarantee that they'll always be that way.
				List<SMLanguage> languages = sm.RetrieveLanguages();

				//todo: Ok, we really need a way to get the survey id from the questioncontentid. This will be too slow. We could also
				//look into extending the ColumnCondition in some way so that it stores the survey id
				var q = sm.RetrieveQuestion(qc.QuestionID);
				var s = sm.RetrieveState(q.StateID);
				var survey = sm.RetrieveSurvey(s.SurveyID);

				SMRespondent respondent = null;
				foreach (var language in languages)
				{
					respondent = sm.RetrieveRespondent(survey.ID, language.ID, null, ipCode);
					if (respondent != null)
					{
						break;
					}
				}

				if (respondent == null)
				{
					return false;
				}

				List<SMResponse> responses = sm.RetrieveResponses(respondent.ID, questionContentId);

				foreach (var response in responses)
				{
					bool ret = false;

					if (condition.DatePart.HasValue)
					{
						DateTime completeDate = response.CompleteDate;
						ret = EvaluateDateCondition(condition, completeDate);
					}
					else
					{
						var answer = sm.RetrieveAnswerContent(response.AnswerContentID);
						if (answer == null)
						{
							continue;
						}

						string val = answer.Content;

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
					}

					if (ret == true)
					{
						return ret;
					}
				}
				return false;
			}
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

		private static bool EvaluateDateCondition(ColumnCondition condition, DateTime completeDate)
		{
			
			bool ret = false;
			int completeDatePart = -1;

			if (condition.Operator.ToLower() == "bet")
			{
				//range
				string[] parts = condition.ConditionExpression.Split('|');
				if (parts.Length != 2)
				{
					return false;
				}

				if (condition.DatePart == DateParts.Complete)
				{
					DateTime start = DateTime.MinValue;
					DateTime end = DateTime.MinValue;
					if (DateTime.TryParse(parts[0], out start) && DateTime.TryParse(parts[1], out end))
					{
						ret = completeDate >= start && completeDate < end;
					}
				}
				else
				{
					switch (condition.DatePart)
					{
						case DateParts.Day:
							completeDatePart = completeDate.Day;
							break;
						case DateParts.Month:
							completeDatePart = completeDate.Month;
							break;
						case DateParts.Year:
							completeDatePart = completeDate.Year;
							break;
					}
					int start = int.MinValue;
					int end = int.MinValue;
					if (int.TryParse(parts[0], out start) && int.TryParse(parts[1], out end))
					{
						ret = completeDatePart >= start && completeDatePart < end;
					}
				}
				return ret;
			}

			if (condition.DatePart == DateParts.Complete)
			{
				DateTime conditionDate = DateTime.MinValue;
				if (!DateTime.TryParse(condition.ConditionExpression, out conditionDate))
				{
					return false;
				}
				//comparing DateTime
				switch (condition.Operator)
				{
					case ">=":
						ret = completeDate >= conditionDate;
						break;
					case ">":
						ret = completeDate > conditionDate;
						break;
					case "<=":
						ret = completeDate <= conditionDate;
						break;
					case "<":
						ret = completeDate < conditionDate;
						break;
					case "<>":
						ret = completeDate != conditionDate;
						break;
					default:
						ret = completeDate == conditionDate;
						break;
				}
				return ret;
			}


			//comparing int (month, day or year - all int)
			switch (condition.DatePart)
			{
				case DateParts.Day:
					completeDatePart = completeDate.Day;
					break;
				case DateParts.Month:
					completeDatePart = completeDate.Month;
					break;
				case DateParts.Year:
					completeDatePart = completeDate.Year;
					break;
			}

			int conditionExpression = -1;
			if (!int.TryParse(condition.ConditionExpression, out conditionExpression))
			{
				return false;
			}

			switch (condition.Operator)
			{
				case ">=":
					ret = completeDatePart >= conditionExpression;
					break;
				case ">":
					ret = completeDatePart > conditionExpression;
					break;
				case "<=":
					ret = completeDatePart <= conditionExpression;
					break;
				case "<":
					ret = completeDatePart < conditionExpression;
					break;
				case "<>":
					ret = completeDatePart != conditionExpression;
					break;
				default:
					ret = completeDatePart == conditionExpression;
					break;
			}

			return ret;
		}


	}
}
