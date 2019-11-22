using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.Data.DomainModel
{
	#region SMSurveyResponseRespondentData class
	public class SMSurveyResponseRespondentData
	{
		private long _id;
		private string _mtouch;
		private DateTime _startDate;
		private DateTime _completeDate;
		private bool _skipped;
		private Dictionary<string, string> _responses = new Dictionary<string, string>();
		private XElement _properties;
		private List<string> _conceptViews = new List<string>();

		internal SMSurveyResponseRespondentData(long ID, string mtouch, DateTime startDate, DateTime completeDate, bool skipped)
		{
			_id = ID;
			_mtouch = mtouch;
			_startDate = startDate;
			_completeDate = completeDate;
			_skipped = skipped;
		}

		public bool HasQuestionStartingWith(string questionName)
		{
			bool result = false;
			if (_responses != null && _responses.Count > 0)
			{
				foreach (var key in _responses.Keys)
				{
					if (key.StartsWith(questionName))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public void AddResponse(string questionName, string response, string propertiesXML, bool overwrite)
		{
			if (_responses.ContainsKey(questionName))
			{
				// questionname/response are unique so this should never happen except for SPSS case
				if (overwrite)
				{
					_responses[questionName] = response;
				}
				else
				{
					_responses[questionName] = _responses[questionName] + "," + response;
				}
			}
			else
			{
				_responses.Add(questionName, response);
			}

			if (_properties == null && !string.IsNullOrEmpty(propertiesXML))
			{
				_properties = XElement.Parse(propertiesXML);
			}
		}

		public void AddConceptView(string conceptName)
		{
			if (!_conceptViews.Contains(conceptName))
			{
				_conceptViews.Add(conceptName);
			}
		}

		public long ID
		{
			get { return _id; }
		}
		public string MTouch
		{
			get { return _mtouch; }
		}
		public DateTime StartDate
		{
			get { return _startDate; }
		}
		public DateTime CompleteDate
		{
			get { return _completeDate; }
		}
		public bool Skipped
		{
			get { return _skipped; }
		}
		public Dictionary<string, string> Responses
		{
			get { return _responses; }
		}
		public string FlattenedProperties
		{
			get
			{
				string result = string.Empty;
				if (_properties != null)
				{
					try
					{
						foreach (XElement prop in _properties.Elements("property"))
						{
							string attName = StringUtils.FriendlyXAttribute(prop.Attribute("name"));
							string attValue = StringUtils.FriendlyXAttribute(prop.Attribute("value"));
							if (!string.IsNullOrEmpty(attName))
							{
								result += attName + "=" + attValue + "::";
							}
						}
					}
					catch { }
				}
				return result;
			}
		}
		public string FlattenedConceptViews
		{
			get
			{
				string result = string.Empty;
				if (_conceptViews != null && _conceptViews.Count > 0)
				{
					foreach (string conceptName in _conceptViews)
					{
						result += conceptName + "::";
					}
				}
				return result;
			}
		}
	}
	#endregion

	public class SMSurveyResponseRawData
	{
		#region fields
		private static string QUOTE = Convert.ToChar(34).ToString();
		private SortedDictionary<long, SMSurveyResponseRespondentData> _respondentData
			= new SortedDictionary<long, SMSurveyResponseRespondentData>(); // [respondentID][]

		private SortedDictionary<string, string> _allQuestionNames = new SortedDictionary<string, string>();
		private ServiceConfig _config = null;
		private long _surveyID = -1;
		private bool _forSPSS = false;
		private SMLanguage _english = null;
		private Dictionary<long, SMQuestion> _questionMap = new Dictionary<long, SMQuestion>(); // [questionID] -> Question
		private Dictionary<string, string> _matrixQuestionContentMap = new Dictionary<string, string>(); // [questionID_matrixIndex] -> Content
		private Dictionary<long, SMAnswerContent> _answerContentMap = new Dictionary<long, SMAnswerContent>(); // [answerContentID] -> AnswerContent
		private Dictionary<long, SMMatrixAnswer> _matrixAnswerMap = new Dictionary<long, SMMatrixAnswer>(); // [matrixAnswerID] -> MatrixAnswer

		private const int IDX_RESPONDENTID = 0;
		private const int IDX_MTOUCH = 1;
		private const int IDX_STARTDATE = 2;
		private const int IDX_COMPLETEDATE = 3;
		private const int IDX_SKIPPED = 4;
		private const int IDX_QUESTIONNAME = 5;
		private const int IDX_QUESTIONID = 6;
		private const int IDX_MATRIXINDEX = 7;
		private const int IDX_COLUMNINDEX = 8;
		private const int IDX_ANSWERCONTENTID = 9;
		private const int IDX_ANSWERCONTROLTYPE = 10;
		private const int IDX_RESPONSECONTENT = 11;
		private const int IDX_PROPERTIESXML = 12;
		private const int IDX_MATRIXANSWERID = 13;
        private const int IDX_ANSWERINDEX = 14;
		#endregion

		#region constructors
		internal SMSurveyResponseRawData(ServiceConfig config, long surveyID, bool forSPSS)
		{
			_config = config;
			_surveyID = surveyID;
			_forSPSS = forSPSS;
			using (var svc = new SurveyManager(config))
			{
				_english = svc.RetrieveLanguage("English");
				List<SMQuestion> questions = svc.RetrieveQuestions(_surveyID);
				if (questions != null && questions.Count > 0)
				{
					foreach (SMQuestion question in questions)
					{
						_questionMap.Add(question.ID, question);
						if (question.IsMatrix)
						{
							if (question.IsPiped)
							{
								long pipedStateID = question.PipedStateID;
								SMQuestion parentQuestion = svc.RetrieveQuestionByStateID(pipedStateID);
								List<SMAnswerContent> parentAnswerContents = svc.RetrieveAnswerContents(parentQuestion.ID, _english.ID);
								if (parentAnswerContents != null && parentAnswerContents.Count > 0)
								{
									foreach (SMAnswerContent parentAnswerContent in parentAnswerContents)
									{
										string key = question.ID + "_" + parentAnswerContent.DisplayIndex;
										string value = parentAnswerContent.Content;
										_matrixQuestionContentMap.Add(key, value);
									}
								}
							}
							else
							{
								List<SMQuestionContent> matrixQuestionContents = svc.RetrieveQuestionContents(question.ID, _english.ID, QuestionContentType.BODY_TEXT);
								if (matrixQuestionContents != null && matrixQuestionContents.Count > 0)
								{
									foreach (SMQuestionContent matrixQuestionContent in matrixQuestionContents)
									{
										string key = question.ID + "_" + matrixQuestionContent.MatrixIndex;
										string value = StringUtils.DeHTML(StringUtils.FriendlyString(matrixQuestionContent.Content))/*.Replace(" ", "_")*/;
										_matrixQuestionContentMap.Add(key, value);
									}
								}
							}
						}
						List<SMAnswerContent> answerContents = svc.RetrieveAnswerContents(question.ID, _english.ID);
						if (answerContents != null && answerContents.Count > 0)
						{
							foreach (SMAnswerContent answerContent in answerContents)
							{
								_answerContentMap.Add(answerContent.ID, answerContent);
							}
						}
					}
				}
			}
		}
		#endregion

		#region internal methods
		internal void AddFromRawRows(List<dynamic> rawRows)
		{
			foreach (dynamic rawRow in rawRows)
			{
				// Marshal the query results
				// rawRow:
				//   0 - Respondent.respondent_id
				//   1 - Respondent.mtouch
				//   2 - Respondent.start_date
				//   3 - Respondent.complete_date
				//   4 - Respondent.skipped
				//   5 - State.uiname
				//   6 - Question.question_id
				//   7 - QuestionContent.matrix_index
				//   8 - Response.column_index
				//   9 - Response.answercontent_id
				//  10 - Question.answer_control_type
				//  11 - Response.Content
				//  12 - Respondent.Propertiesxml
				//  13 - Response.MatrixAnswerID
                long respondentID = StringUtils.FriendlyInt64(rawRow.RESPONDENT_ID);
                string mtouch = StringUtils.FriendlyString(rawRow.MTOUCH);
                DateTime startDate = StringUtils.FriendlyDateTime(rawRow.START_DATE);
                DateTime completeDate = StringUtils.FriendlyDateTime(rawRow.COMPLETE_DATE);
                bool skipped = StringUtils.FriendlyString(rawRow.SKIPPED, "F").ToUpper().Equals("T");
                string questionName = StringUtils.FriendlyString(rawRow.UINAME).Replace(" ", string.Empty);
                long questionID = StringUtils.FriendlyInt64(rawRow.QUESTION_ID, -1);
                long matrixIndex = StringUtils.FriendlyInt64(rawRow.MATRIX_INDEX);
                long columnIndex = StringUtils.FriendlyInt64(rawRow.COLUMN_INDEX);
                long answerContentID = StringUtils.FriendlyInt64(rawRow.ANSWERCONTENT_ID, -1);
                QA_AnswerControlType answerControlType = (QA_AnswerControlType)Enum.Parse(typeof(QA_AnswerControlType), rawRow.ANSWER_CONTROL_TYPE.ToString());
                string responseContent = StringUtils.FriendlyString(rawRow.ANSWER);
                string propertiesXML = StringUtils.FriendlyString(rawRow.PROPERTIESXML);
                long matrixAnswerID = StringUtils.FriendlyInt64(rawRow.MATRIXANSWER_ID);
                string answerIndex = StringUtils.FriendlyString(rawRow.ANSWER_INDEX);

				if (questionID > -1 && _questionMap.ContainsKey(questionID))
				{
					SMQuestion question = _questionMap[questionID];
					if (question != null && question.IsMatrix)
					{
						if (matrixAnswerID > -1)
						{
							if (_matrixAnswerMap.ContainsKey(matrixAnswerID))
							{
								answerControlType = _matrixAnswerMap[matrixAnswerID].AnswerControlType;
							}
							else
							{
								using (var svc = new SurveyManager(_config))
								{
									SMMatrixAnswer matrixAnswer = svc.RetrieveMatrixAnswerID(matrixAnswerID);
									if (matrixAnswer != null)
									{
										_matrixAnswerMap.Add(matrixAnswerID, matrixAnswer);
										answerControlType = matrixAnswer.AnswerControlType;
									}
								}
							}
						}
						if (answerControlType == QA_AnswerControlType.RADIO && responseContent == "-1")
						{
							responseContent = string.Empty;
						}
					}
				}
                long updatedMatrixIndex = !string.IsNullOrEmpty(answerIndex) ? long.Parse(answerIndex) : -1;

                string longQuestionName = ConstructQuestionName(questionName, questionID, answerContentID, updatedMatrixIndex, columnIndex, answerControlType, matrixAnswerID, responseContent);
				string response = (_forSPSS ? GetSPSSResponse(rawRow, answerControlType, responseContent) : responseContent);

				// new respondent?
				if (!_respondentData.ContainsKey(respondentID))
				{
					_respondentData.Add(respondentID, new SMSurveyResponseRespondentData(respondentID, mtouch, startDate, completeDate, skipped));
				}

				// initialize " " for multiselect simple questions for SPSS output
				if (_forSPSS && !_respondentData[respondentID].HasQuestionStartingWith(questionName + "_") && IsMultiselectSimpleQuestion(rawRow))
				{
					if (questionID > -1 && _questionMap.ContainsKey(questionID))
					{
						SMQuestion question = _questionMap[questionID];
						if (!question.IsPiped)
						{
							foreach (SMAnswerContent answerContent in _answerContentMap.Values)
							{
								if (answerContent.QuestionID == questionID)
								{
									string thisLongQuestionName = ConstructQuestionName(questionName, questionID, answerContent.ID, -1, -1, answerControlType, -1, responseContent);
									_respondentData[respondentID].AddResponse(thisLongQuestionName, " ", string.Empty, true);
									if (!_allQuestionNames.ContainsKey(thisLongQuestionName))
									{
										_allQuestionNames.Add(thisLongQuestionName, thisLongQuestionName);
									}
								}
							}
						}
					}
				}

                // LW-763 -  Start
                string responseOther = string.Empty;
                string longQuestionNameOther = (longQuestionName + "_Other");
                string multiQuestionName = _answerContentMap.ContainsKey(answerContentID) ? questionName += "_" + GetDisplayIndexForAnswer(answerContentID) + " - OtherSpec" : questionName += "_Other";
                if (answerContentID == -1)
                {
                    Dictionary<long, SMAnswerContent> specificQuestionAnswers = new Dictionary<long, SMAnswerContent>();
                    foreach (SMAnswerContent answerContent in _answerContentMap.Values)
                    {
                        if (answerContent.QuestionID == questionID)
                        {
                            specificQuestionAnswers.Add(answerContent.ID, answerContent);
                        }
                    }

                    if (IsMultiselectSimpleQuestion(rawRow))
                    {
                        //string qName = "Question_" + (specificQuestionAnswers.Count + 1) + " - OtherSpec";
                        _respondentData[respondentID].AddResponse(multiQuestionName, "1", propertiesXML, true);
                        if (!_allQuestionNames.ContainsKey(multiQuestionName))
                            _allQuestionNames.Add(multiQuestionName, multiQuestionName);
                    }
                    else
                    {
                        responseOther = response;
                        response = (specificQuestionAnswers.Count + 1).ToString();
                    }
                }
                // LW-763 - End

                _respondentData[respondentID].AddResponse(longQuestionName, response, propertiesXML, true);

                // LW-763 - Start
                if (!string.IsNullOrEmpty(responseOther))
                    _respondentData[respondentID].AddResponse(longQuestionNameOther, responseOther, propertiesXML, false);
                //else
                //    _respondentData[respondentID].AddResponse(longQuestionNameOther, "0", propertiesXML, false);
                // LW-763 - End

                if (!_allQuestionNames.ContainsKey(longQuestionName))
                {
                    _allQuestionNames.Add(longQuestionName, longQuestionName);
                    // LW-763 - Start
                    if (!longQuestionName.ToLower().Contains("other"))
                        _allQuestionNames.Add(longQuestionNameOther, longQuestionNameOther);
                    // LW-763 - End
                }
			}
		}

		internal void AddConceptViews(List<dynamic> rawRows)
		{
			foreach (dynamic rawRow in rawRows)
			{
				// Marshal the query results
                long respondentID = StringUtils.FriendlyInt64(rawRow.RESPONDENT_ID);
                string conceptName = StringUtils.FriendlyString(rawRow.NAME);

				if (_respondentData.ContainsKey(respondentID))
				{
					_respondentData[respondentID].AddConceptView(conceptName);
				}
			}
		}

		internal string AsCSV()
		{
			StringBuilder result = new StringBuilder();

			// Header
			result.Append("\"RESPONDENT_ID\",\"MTOUCH\",\"START_DATE\",\"COMPLETE_DATE\",\"TERMINATED\",");
			bool firstTime = true;
			foreach (string questionName in _allQuestionNames.Keys)
			{
				if (!firstTime)
					result.Append(",");
				else
					firstTime = false;

				string questionHeaderName = questionName;
				if (questionHeaderName.Contains("seen:"))
				{
					questionHeaderName = questionHeaderName.Replace("seen:", string.Empty);
				}
				result.Append(quote(questionHeaderName));
			}
			result.Append(",\"PROPERTIES\",\"CONCEPTS\"\n");

			// Rows
			foreach (long respondentID in _respondentData.Keys)
			{
				SMSurveyResponseRespondentData respondent = _respondentData[respondentID];
				result.Append(quote(respondentID.ToString())).Append(",");
				result.Append(quote(respondent.MTouch)).Append(",");
				result.Append(quote(respondent.StartDate.ToString())).Append(",");
				result.Append(quote(respondent.CompleteDate.ToString())).Append(",");
				result.Append(quote((respondent.Skipped ? "T" : "F"))).Append(",");

				Dictionary<string, string> tmpResponses = respondent.Responses;

				firstTime = true;
				foreach (string questionName in _allQuestionNames.Keys)
				{
					if (!firstTime)
						result.Append(",");
					else
						firstTime = false;

					if (tmpResponses.ContainsKey(questionName))
					{
						string foo = tmpResponses[questionName];
						if (!foo.StartsWith("seen:"))
						{
							string tmp = StringUtils.FriendlyString(foo);
							tmp = tmp.Replace(Environment.NewLine, string.Empty);
							tmp = tmp.Replace("\r", string.Empty);
							tmp = tmp.Replace("\n", string.Empty);
							result.Append(quote(tmp));
						}
						else
						{
							result.Append(quote(string.Empty));
						}
					}
					else
						result.Append(quote(string.Empty));
				}
				result.Append(",").Append(quote(respondent.FlattenedProperties));
				result.Append(",").Append(quote(respondent.FlattenedConceptViews));
				result.Append("\n");
			}

			return result.ToString();
		}

		internal byte[] AsXLS(string worksheetName)
		{
			byte[] result = null;
			string xlsPath = Path.GetTempPath() + Path.DirectorySeparatorChar + worksheetName + ".xls";
			OleDbConnection conn = null;
			try
			{
				if (File.Exists(xlsPath))
					File.Delete(xlsPath);

				string connectionString =
					"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + QUOTE + xlsPath + QUOTE
					+ ";Extended Properties=" + QUOTE + "Excel 8.0;Imex=2;HDR=true;" + QUOTE;

				conn = new OleDbConnection(connectionString);
				conn.Open();

				// Create the worksheet
				StringBuilder sql = new StringBuilder();
				sql.AppendFormat("create table [{0}] (", worksheetName);
				sql.Append("[RESPONDENT_ID] varchar with compression,");
				sql.Append("[MTOUCH] varchar with compression,");
				sql.Append("[START_DATE] datetime,");
				sql.Append("[COMPLETE_DATE] datetime,");
				sql.Append("[TERMINATED] char(1)");
				foreach (string questionName in _allQuestionNames.Keys)
				{
					sql.AppendFormat(",[{0}] longchar with compression", questionName);
				}
				sql.Append(",[PROPERTIES] varchar with compression");
				sql.Append(",[CONCEPTS] varchar with compression");
				sql.Append(")");
				OleDbCommand cmd = new OleDbCommand(sql.ToString(), conn);
				cmd.ExecuteNonQuery();

				// populate the worksheet
				foreach (long respondentID in _respondentData.Keys)
				{
					SMSurveyResponseRespondentData respondent = _respondentData[respondentID];

					sql = new StringBuilder();
					sql.AppendFormat("insert into [{0}] values (", worksheetName);
					sql.Append(quote(respondentID.ToString())).Append(",");
					sql.Append(quote(respondent.MTouch)).Append(",");
					sql.Append(quote(respondent.StartDate.ToString())).Append(",");
					sql.Append(quote(respondent.CompleteDate.ToString())).Append(",");
					sql.Append(quote((respondent.Skipped ? "T" : "F")));

					Dictionary<string, string> tmpResponses = respondent.Responses;
					foreach (string questionName in _allQuestionNames.Keys)
					{
						sql.Append(",");
						if (tmpResponses.ContainsKey(questionName))
							sql.Append(quote(tmpResponses[questionName]));
						else
							sql.Append(quote(string.Empty));
					}
					sql.Append(",").Append(quote(respondent.FlattenedProperties));
					sql.Append(",").Append(quote(respondent.FlattenedConceptViews));
					sql.Append(")");

					cmd.CommandText = sql.ToString();
					cmd.ExecuteNonQuery();
				}
				conn.Close();

				result = File.ReadAllBytes(xlsPath);
			}
			catch (Exception ex)
			{
				string msg = string.Format("Error encountered while writing temp XLS file '{0}'", xlsPath);
				throw new LWException(msg, ex);
			}
			finally
			{
				if (conn != null && conn.State != ConnectionState.Closed) conn.Close();
				if (File.Exists(xlsPath)) File.Delete(xlsPath);
			}
			return result;
		}
		#endregion

		#region private methods
		private string GetSPSSResponse(dynamic rawRow, QA_AnswerControlType answerControlType, string content)
		{
			string result = string.Empty;
            long questionID = StringUtils.FriendlyInt64(rawRow.QUESTION_ID, -1);
            long answerContentID = StringUtils.FriendlyInt64(rawRow.ANSWERCONTENT_ID, -1);
			SMQuestion question = null;
			if (questionID > -1) question = _questionMap[questionID];
			result = GetSPSSResponse(question, answerContentID, content, answerControlType);
			return result;
		}

		private string GetSPSSResponse(SMQuestion question, long answerContentID, string content, QA_AnswerControlType answerControlType)
		{
			string result = string.Empty;
			if (question != null)
			{
				if (question.IsMatrix)
				{
					switch (answerControlType)
					{
						case QA_AnswerControlType.CHECK:
							result = (StringUtils.FriendlyBool(content) ? "1" : "0");
							break;

						default:
							result = content;
							break;
					}
				}
				else
				{
					if (answerContentID > -1)
					{
						if (question.IsMultiselectSimpleQuestion())
						{
							if (content.StartsWith("seen:") && (_answerContentMap[answerContentID].DisplayIndex + 1).ToString() != content)
							{
								result = "0";
							}
							else
							{
								result = "1";
							}
						}
						else
						{
							result = (_answerContentMap[answerContentID].DisplayIndex + 1).ToString();
						}
					}
					else if (question.IsPiped)
					{
						if (content.StartsWith("seen:"))
						{
							result = "0";
						}
						else
						{
							result = "1";
						}
					}
					else
					{
						result = content;
					}
				}
			}
			else
			{
				result = content;
			}
			return result;
		}

		private bool IsMultiselectSimpleQuestion(dynamic rawRow)
		{
			bool result = false;
            long questionID = StringUtils.FriendlyInt64(rawRow.QUESTION_ID, -1);
			SMQuestion question = null;
			if (questionID > -1) question = _questionMap[questionID];
			if (question != null && question.IsMultiselectSimpleQuestion())
			{
				result = true;
			}
			return result;
		}

		private string ConstructQuestionName(string questionName, long questionID, long answerContentID, long matrixIndex, long columnIndex, QA_AnswerControlType answerControlType, long matrixAnswerID, string responseContent)
		{
			bool isMultianswer = true;
			SMQuestion question = null;
			if (questionID > -1) question = _questionMap[questionID];
			if (question != null)
			{
				isMultianswer = question.IsMultiAnswer();
				if (question.IsMatrix)
				{
					questionName += " - " + GetMatrixQuestionText(question, matrixIndex);
					if (answerControlType != QA_AnswerControlType.RADIO && columnIndex > -1)
					{
						questionName += "_" + columnIndex;
					}
				}
				else if (isMultianswer)
				{
					if (answerContentID > -1)
					{
						if (_answerContentMap.ContainsKey(answerContentID))
						{
							questionName += "_" + GetDisplayIndexForAnswer(answerContentID);
						}
						else
						{
							questionName += "_" + answerContentID.ToString();
						}
					}
					else if (question.IsPiped)
					{
						if (responseContent.StartsWith("seen:"))
						{
							responseContent = responseContent.Substring("seen:".Length);
						}
						questionName += "_Piped - " + responseContent;
					}
					else
					{
						questionName += "_OtherSpec";
					}
				}
				else if (question.IsPiped)
				{
					if (responseContent.StartsWith("seen:"))
					{
						responseContent = responseContent.Substring("seen:".Length);
					}
					questionName += "_Piped - " + responseContent;
				}
			}
			else
			{
				questionName += "_" + questionID.ToString() + "_" + matrixIndex.ToString();

				isMultianswer = (answerContentID > -1 && answerControlType != QA_AnswerControlType.RADIO);
			}

			if (isMultianswer && answerContentID > -1 && _answerContentMap.ContainsKey(answerContentID))
			{
				questionName += " - " + GetContentForAnswer(answerContentID);
			}

			return questionName;
		}

		private string GetContentForAnswer(long answerContentID)
		{
			SMAnswerContent answerContent = _answerContentMap[answerContentID];
			string result = StringUtils.DeHTML(StringUtils.FriendlyString(answerContent.Content));
			return result;
		}

		private string GetDisplayIndexForAnswer(long answerContentID)
		{
			SMAnswerContent answerContent = _answerContentMap[answerContentID];
			string result = (answerContent.DisplayIndex + 1).ToString();
			return result;
		}

		private string GetMatrixQuestionText(SMQuestion question, long matrixIndex)
		{
			string result = string.Empty;
			string key = question.ID + "_" + matrixIndex;
			if (_matrixQuestionContentMap.ContainsKey(key))
			{
				result = _matrixQuestionContentMap[key];
			}
			return result;
		}

		private string quote(string value)
		{
			return "\"" + value.Replace("\"", "\"\"") + "\"";
		}
		#endregion
	}
}
