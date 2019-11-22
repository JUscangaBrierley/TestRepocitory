using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Reflection;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.XML;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using PetaPoco;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class SMStateModel : IDisposable
	{
		#region fields
		private bool _disposed = false;
		private const string DEFAULT_CULTURE = "en-US";
		private const string DEFAULT_LANGUAGE = "English";
		private const string _className = "SMStateModel";
		private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
		private ServiceConfig _config = null;
		private SurveyManager _surveyManager = null;
		private StateModelStatus _status = StateModelStatus.NotInitialized;
		private StateModelTerminationType _terminationType = StateModelTerminationType.NotInitialized;
		private long _surveyID = -1;
		private long _stateID = -1;
		private long _ipcode = -1;
		private string _mtouch = string.Empty;
        private long _respondentID = -1;
		private string _bscript = string.Empty;
		private string[] _cultures = null;
		private SMLanguage _language = null;
		private SMSurvey _survey = null;
		private IList<SMState> _states = null;
		private SMState _currentState = null;
		private SMRespondent _respondent = null;
		private SurveySelectionMethod _surveySelectionMethod = SurveySelectionMethod.AnyAvailable;
		private long _numPages = -1;
		private long _curPage = -1;
		private bool _allowPreview = false;
		private decimal? _surveyCompletedAccrualPoints = null;   
		#endregion

		#region properties
		public Member Member { get; set; }

		public bool AllowPreview
		{
			get { return _allowPreview; }
			set { _allowPreview = value; }
		}

		public bool ForceAllowSurveyReview { get; set; }

		public SMSurvey Survey
		{
			get { return _survey; }
			set
			{
				if (_status >= StateModelStatus.Initialized)
					throw new InvalidOperationException("Can't set survey ID when state model is already initialized.");
				_survey = value;
				if (_survey != null)
				{
					_surveyID = _survey.ID;
					if (_states != null && _states.Count > 0 && _states[0].SurveyID != _survey.ID)
					{
						_states = null;
					}
					if (_states == null || _states.Count == 0)
					{
						_states = _survey.GetStates(_config);
					}
				}
				else
				{
					_states = null;
				}
			}
		}

		public long SurveyID
		{
			get { return _surveyID; }
			set
			{
				if (_status >= StateModelStatus.Initialized)
					throw new InvalidOperationException("Can't set survey ID when state model is already initialized.");
				_surveyID = value;
			}
		}

		public long StateID
		{
			get
			{
				if (_currentState != null) _stateID = _currentState.ID;
				return _stateID;
			}
			set
			{
				if (_status >= StateModelStatus.Initialized)
					throw new InvalidOperationException("Can't set state ID when state model is already initialized.");
				_stateID = value;
			}
		}

		public long IPCode
		{
			get { return _ipcode; }
			set
			{
				if (_status >= StateModelStatus.Initialized)
					throw new InvalidOperationException("Can't set IPCode when state model is already initialized.");
				_ipcode = value;
			}
		}

		public string MTouch
		{
			get { return _mtouch; }
			set
			{
				if (_status >= StateModelStatus.Initialized)
					throw new InvalidOperationException("Can't set MTouch code when state model is already initialized.");
				_mtouch = value;
			}
		}

        public long RespondentID
        {
            get { return _respondentID; }
            set
            {
                if (_status >= StateModelStatus.Initialized)
                    throw new InvalidOperationException("Can't set respondent ID when state model is already initialized.");
                _respondentID = value;
            }
        }

		public string BScript
		{
			get { return _bscript; }
			set
			{
				if (_status >= StateModelStatus.Initialized)
					throw new InvalidOperationException("Can't set BScript when state model is already initialized.");
				_bscript = value;
			}
		}

		public SMLanguage Language
		{
			get { return _language; }
			set { _language = value; }
		}

		public string[] Cultures
		{
			get { return _cultures; }
			set
			{
				if (_status >= StateModelStatus.Initialized)
					throw new InvalidOperationException("Can't set Cultures when state model is already initialized.");
				_cultures = value;
			}
		}

		public SurveySelectionMethod SurveySelectionMethod
		{
			get { return _surveySelectionMethod; }
			set
			{
				if (_status >= StateModelStatus.Initialized)
					throw new InvalidOperationException("Can't set SurveySelectionMethod when state model is already initialized.");
				_surveySelectionMethod = value;
			}
		}

		public StateModelStatus Status
		{
			get { return _status; }
		}

		public StateModelTerminationType TerminationType
		{
			get { return _terminationType; }
		}

		public long CurPage
		{
			get { return _curPage; }
		}

		public long NumPages
		{
			get { return _numPages; }
		}

        public decimal? SurveyCompletedAccrualPoints
		{
			get { return _surveyCompletedAccrualPoints; }
			set { _surveyCompletedAccrualPoints = value; }
		}

        public PointTransactionOwnerType? OwnerType { get; set; }
        public long? OwnerId { get; set; }
        public long? RowKey { get; set; }
		#endregion

		#region constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		public SMStateModel(ServiceConfig config)
		{
			_config = config;
			_surveyManager = new SurveyManager(config);
			Member = null;
		}
		#endregion

		#region public methods


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		/// <summary>
		/// Initialize the state model to a resting state.  If false is returned, Status property indicates the cause.
		/// </summary>
		/// <returns>true if the survey is on a resting state, otherwise false</returns>
		public bool Initialize()
		{
			const string methodName = "Initialize";

			_terminationType = StateModelTerminationType.NotInitialized;
			if (_status != StateModelStatus.NotInitialized)
			{
				// already initialized
				_logger.Error(_className, methodName, "Already initialized.");
				return true;
			}

			if (string.IsNullOrEmpty(_mtouch) && _ipcode == -1)
			{
				_status = StateModelStatus.NoRespondent;
				_logger.Debug(_className, methodName, "Unable to select respondent since both mtouch and ipcode are not specified.");
				return false;
			}

			if (!SelectLanguage())
			{
				_status = StateModelStatus.NoLanguage;
				_logger.Error(_className, methodName, "Unable to select language.");
				return false;
			}

			switch (_surveySelectionMethod)
			{
				case SurveySelectionMethod.SurveyID:
					if (!SelectSurveyViaSurveyID())
					{
						// reason already logged
						return false;
					}
					_logger.Trace(_className, methodName, string.Format("Selected survey '{0}' using ID '{1}'.", _survey.Name, _surveyID));
					if (!SelectRespondentForSelectedSurvey())
					{
						// reason already logged
						return false;
					}
					break;

				case SurveySelectionMethod.BScript:
					if (!SelectSurveyViaBscript())
					{
						// reason already logged
						return false;
					}
					_logger.Trace(_className, methodName, string.Format("Selected survey '{0}' using bscript '{1}'.", _survey.Name, _bscript));
					if (!SelectRespondentForSelectedSurvey())
					{
						// reason already logged
						return false;
					}
					break;

				case SurveySelectionMethod.AnyAvailable:
					if (!SelectAnyAvailableSurveyAndRespondent())
					{
						// reason already logged
						return false;
					}
					_logger.Trace(_className, methodName, string.Format("Selected survey '{0}' ({1}) and respondent ID '{2}'.", _survey.Name, _survey.ID, _respondent.ID));
					break;
			}

			if (_respondent.StartDate == null)
			{
				_respondent.StartDate = DateTime.Now;
				_surveyManager.UpdateRespondent(_respondent);
			}

			_logger.Trace(_className, methodName, string.Format("Selecting current state for selected survey '{0}' ({1}) and respondent ID '{2}' for stateID {3}.", _survey.Name, _survey.ID, _respondent.ID, _stateID));
			SelectCurrentState();
			if (_currentState == null)
			{
				_status = StateModelStatus.NoState;
				return false;
			}
			if (_terminationType == StateModelTerminationType.NotInitialized)
			{
				_terminationType = StateModelTerminationType.NotYetTerminated;
			}
			return true;
		}

		/// <summary>
		/// Reset the state model
		/// </summary>
		public void Reset()
		{
			_status = StateModelStatus.NotInitialized;
			_terminationType = StateModelTerminationType.NotInitialized;
			_surveyID = -1;
			_stateID = -1;
			_ipcode = -1;
			_mtouch = string.Empty;
			_bscript = string.Empty;
			_cultures = null;
			_language = null;
			_survey = null;
			_states = null;
			_currentState = null;
			_respondent = null;
			_surveySelectionMethod = SurveySelectionMethod.AnyAvailable;
			_curPage = -1;
			_numPages = -1;
			Member = null;
		}

		public ITransaction StartTransaction()
		{
			return _surveyManager.StartTransaction();
		}

		#region content/data related
		/// <summary>
		/// Gets the current question.
		/// </summary>
		/// <returns>current question, or null if not exist</returns>
		public SMQuestion GetCurrentQuestion()
		{
			if (_status == StateModelStatus.OnRestingState && _currentState != null
				&& (_currentState.StateType == StateType.Question || _currentState.StateType == StateType.MatrixQuestion))
			{
				SMQuestion question = _surveyManager.RetrieveQuestionByStateID(_currentState.ID);
				return question;
			}
			return null;
		}

		public SMQuestion GetPipedQuestion(SMQuestion question)
		{
			SMQuestion result = null;
			if (question.PipedStateID > 0)
			{
				result = _surveyManager.RetrieveQuestionByStateID(question.PipedStateID);
			}
			return result;
		}

		/// <summary>
		/// Gets the current message
		/// </summary>
		/// <returns>current message, or null if not exist</returns>
		public SMMessage GetCurrentMessage()
		{
			if (_status == StateModelStatus.OnRestingState && _currentState != null && _currentState.StateType == StateType.Message)
			{
				SMMessage message = _surveyManager.RetrieveMessageByStateID(_currentState.ID);
				return message;
			}
			return null;
		}

		public SMMessage GetMessageForState(SMState state)
		{
			SMMessage result = null;
			if (state.StateType == StateType.Message)
			{
				result = _surveyManager.RetrieveMessageByStateID(state.ID);
			}
			return result;
		}

		public void GoToState(long stateID)
		{
			_stateID = stateID;
			_currentState = _surveyManager.RetrieveState(stateID);
			if (!IsRestingState(_currentState))
			{
				throw new ArgumentException(string.Format("State with ID {0} is not a resting state", stateID));		
			}
			_status = StateModelStatus.OnRestingState;
			_terminationType = StateModelTerminationType.NotYetTerminated;
			_curPage = GetPageNum();
		}

		/// <summary>
		/// Gets the current resting state.
		/// </summary>
		/// <returns>next resting state, or null if not exist</returns>
		public SMState GetCurrentRestingState()
		{
			if (_currentState == null) return null;
			if (IsRestingState(_currentState)) return _currentState;
			return null;
		}

		/// <summary>
		/// Get the current resting state as XML.
		/// </summary>
		/// <returns>XML string</returns>
		public string GetCurrentRestingStateAsXML()
		{
			string result = null;
			SMState currentRestingState = GetCurrentRestingState();
			if (currentRestingState != null)
			{
				switch (currentRestingState.StateType)
				{
					case StateType.Question:
					case StateType.MatrixQuestion:
						SMQuestion question = GetCurrentQuestion();
						if (question != null)
						{
							result = SurveyXML.QuestionToXML(this, question);
						}
						break;
                    case StateType.Page:
                        result = "<page/>";
                        break;
					case StateType.Message:
						SMMessage message = GetCurrentMessage();
						result = SurveyXML.MessageToXML(this, message);
						break;
				}
			}
			return result;
		}

		/// <summary>
		/// Gets the next resting state.
		/// </summary>
		/// <returns>next resting state, or null if not exist</returns>
		public SMState GetNextRestingState()
		{
			IterateModel();
			return GetCurrentRestingState();
		}

        public List<SMState> GetPageStates(SMState pageState)
        {
            if (pageState == null || pageState.StateType != StateType.Page)
                throw new Exception("Invalid page state");

            List<SMState> result = _surveyManager.RetrieveStatesByPageID(pageState.ID);
            if (result == null) result = new List<SMState>();
            return result;
        }

		private bool NeedResponseForPage(SMState pageState, long languageID, long respondentID) 
		{
			if (pageState.StateType != StateType.Page)
			{
				throw new InvalidOperationException("Called NeedResponseForPage() on non-page state.");
			}

			List<SMState> states = GetPageStates(pageState);
			bool result = false;
			if (states != null && states.Count > 0)
			{
				foreach (SMState state in states)
				{
					if (_surveyManager.NeedResponse(state.ID, languageID, respondentID))
					{
						result = true;
						break;
					}
				}
			}
			return result;			
		}

		/// <summary>
		/// Get the content of a non-matrix question.
		/// </summary>
		/// <param name="question">a non-matrix question</param>
		/// <returns>question content, or null if not exist</returns>
		public string GetSimpleQuestionContent(SMQuestion question)
		{
            if (_language == null)
                throw new Exception("Language has not been set in the statemodel");

			string result = null;
			List<SMQuestionContent> questionBodies = _surveyManager.RetrieveQuestionContents(question.ID, _language.ID, QuestionContentType.BODY_TEXT);
			if (questionBodies != null && questionBodies.Count > 0)
				result = questionBodies[0].Content;
			return result;
		}

		/// <summary>
		/// Get the header of a matrix question.
		/// </summary>
		/// <param name="question">a matrix question</param>
		/// <returns>question header, or null if not exist</returns>
		public string GetMatrixQuestionHeader(SMQuestion question)
		{
			string result = null;
			List<SMQuestionContent> questionHeaders = _surveyManager.RetrieveQuestionContents(question.ID, _language.ID, QuestionContentType.HEADER_TEXT);
			if (questionHeaders != null && questionHeaders.Count > 0)
				result = questionHeaders[0].Content;
			return result;
		}

		/// <summary>
		/// Get the list of content of a matrix question.
		/// </summary>
		/// <param name="question">a matrix question</param>
		/// <returns>list of question content</returns>
		public List<SMQuestionContent> GetMatrixQuestionContent(SMQuestion question)
		{
			List<SMQuestionContent> result = _surveyManager.RetrieveQuestionContents(question.ID, _language.ID, QuestionContentType.BODY_TEXT);
			return result;
		}

		/// <summary>
		/// Get the list of anchor text of a matrix question.
		/// </summary>
		/// <param name="question">a matrix question</param>
		/// <returns>list of question content</returns>
		public List<SMQuestionContent> GetMatrixQuestionAnchors(SMQuestion question)
		{
			List<SMQuestionContent> result = _surveyManager.RetrieveQuestionContents(question.ID, _language.ID, QuestionContentType.ANCHOR_TEXT);
			return result;
		}

        /// <summary>
        /// Get the answers for a matrix question.
        /// </summary>
        /// <param name="question">a matrix question</param>
        /// <returns>list of matrix answers</returns>
        public List<SMMatrixAnswer> GetMatrixAnswers(SMQuestion question)
        {
            List<SMMatrixAnswer> result = _surveyManager.RetrieveMatrixAnswerByQuestionID(question.ID, _language.ID);
            return result;
        }

		/// <summary>
		/// Get the list of answer content of a question.
		/// </summary>
		/// <param name="question">a question</param>
		/// <returns>list of answer content</returns>
		public List<SMAnswerContent> GetAnswerContent(SMQuestion question)
		{
			List<SMAnswerContent> result = _surveyManager.RetrieveAnswerContents(question.ID, _language.ID);
			return result;
		}

		public List<SMResponse> GetResponses(SMQuestion question)
		{
			List<SMResponse> result = new List<SMResponse>();
            if (_respondent == null) return result;

			List<SMQuestionContent> questionContents = _surveyManager.RetrieveQuestionContents(question.ID, _language.ID, QuestionContentType.BODY_TEXT);
            if (questionContents != null && questionContents.Count > 0)
			{
				foreach (SMQuestionContent questionContent in questionContents)
				{
					List<SMResponse> responses = _surveyManager.RetrieveResponses(_respondent.ID, questionContent.ID);
					if (responses != null && responses.Count > 0)
					{
						result.AddRange(responses);
					}
				}
			}

			if (question.HasOtherSpecify)
			{
				List<SMQuestionContent> otherSpecifyQuestionContents = _surveyManager.RetrieveQuestionContents(question.ID, _language.ID, QuestionContentType.OTHER_SPECIFY_TEXT);
				if (otherSpecifyQuestionContents != null && otherSpecifyQuestionContents.Count > 0)
				{
					foreach (SMQuestionContent otherSpecifyQuestionContent in otherSpecifyQuestionContents)
					{
						List<SMResponse> otherSpecifyResponses = _surveyManager.RetrieveResponses(_respondent.ID, otherSpecifyQuestionContent.ID);
						if (otherSpecifyResponses != null && otherSpecifyResponses.Count > 0)
						{
							result.AddRange(otherSpecifyResponses);
						}
					}
				}
			}
			return result;
		}

		public SMQuestionContent GetOtherSpecifiedQuestionContent(SMQuestion question)
		{
			SMQuestionContent result = null;
			List<SMQuestionContent> otherSpecify = _surveyManager.RetrieveQuestionContents(question.ID, _language.ID, QuestionContentType.OTHER_SPECIFY_TEXT);
			if (otherSpecify != null && otherSpecify.Count > 0)
			{
				result = otherSpecify[0];
			}
			return result;
		}

		public void PostSawMessage(SMMessage message)
		{
			DateTime now = DateTime.Now;
			SMResponse response = new SMResponse()
			{
				RespondentID = (_respondent != null ? _respondent.ID : -1),
				StateID = message.StateID,
				QuestionContentID = -1,
				AnswerContentID = -1,
				PipedResponseID = -1,
				Content = string.Empty,
				PipedContent = null,
				StartDate = now,
				CompleteDate = now
			};
			_surveyManager.CreateResponse(response);
		}

		public bool PostSimpleAnswers(SMQuestion question, List<string> answers, string otherSpecifiedText, List<string> seen)
		{
			const string methodName = "PostSimpleAnswers";

			// There must be a question
			if (question == null) return false;

			// There should be one and only one question content
			List<SMQuestionContent> questionContents = _surveyManager.RetrieveQuestionContents(question.ID, _language.ID, QuestionContentType.BODY_TEXT);
			if (questionContents == null || questionContents.Count < 1) return false;
			SMQuestionContent questionContent = questionContents[0];

			// Remove previous response
			if (_respondent != null && _respondent.ID != -1)
			{
				List<SMResponse> oldResponses = _surveyManager.RetrieveResponses(_respondent.ID, questionContent.ID);
				if (oldResponses != null && oldResponses.Count > 0)
				{
					if (ForceAllowSurveyReview || _survey.CanReviewQuestions())
					{
						foreach (SMResponse oldResponse in oldResponses)
						{
							_surveyManager.DeleteResponse(oldResponse);
						}
					}
					else
					{
						_logger.Error(_className, methodName, string.Format("Attempt to re-post answers to a survey (ID={0}, Name={1}) that doesn't support review.  Respondent ID = {2}", _survey.ID, _survey.Name, _respondent.ID));
						throw new SurveyQuestionException("You've already answered this question.");
					}
				}
			}

			// "Non-List" types have a single free-form answer that can be optional
			if (!question.IsListAnswerType())
			{
				if (answers != null && answers.Count > 0 && !string.IsNullOrEmpty(answers[0]))
				{
					DateTime now = DateTime.Now;
					SMResponse response = new SMResponse()
					{
						RespondentID = (_respondent != null ? _respondent.ID : -1),
						StateID = question.StateID,
						QuestionContentID = questionContent.ID,
						AnswerContentID = -1,
						PipedResponseID = -1,
						Content = answers[0],
						PipedContent = null,
						StartDate = now,
						CompleteDate = now
					};
					_surveyManager.CreateResponse(response);
					return true;
				}
				if (question.ResponseOptional) return true;
				return false;
			}

			// piped question
			bool isPiped = question.CanBePiped();
			List<SMResponse> pipedResponses = null;
			if (isPiped)
			{
				SMQuestion pipedQuestion = GetPipedQuestion(question);
				pipedResponses = GetResponses(pipedQuestion);
			}

			// Check for "List" type answers
			bool postedResponse = false;
			if (answers != null && answers.Count > 0)
			{
				List<SMAnswerContent> answerContents = _surveyManager.RetrieveAnswerContents(question.ID, _language.ID);				
				if (question.AnswerControlType == QA_AnswerControlType.RADIO)
				{
					if (isPiped)
					{
						DateTime now = DateTime.Now;
						SMResponse response = new SMResponse()
						{
							RespondentID = (_respondent != null ? _respondent.ID : -1),
							StateID = question.StateID,
							QuestionContentID = questionContent.ID,
							AnswerContentID = -1,
							PipedResponseID = -1,
							Content = answers[0],
							PipedContent = null,
							StartDate = now,
							CompleteDate = now
						};
						_surveyManager.CreateResponse(response);
						return true;
					}
					else
					{
						SMAnswerContent answerContent = GetMatchingAnswerContent(answerContents, answers[0]);
						if (answerContent != null)
						{
							DateTime now = DateTime.Now;
							SMResponse response = new SMResponse()
							{
								RespondentID = (_respondent != null ? _respondent.ID : -1),
								StateID = question.StateID,
								QuestionContentID = questionContent.ID,
								AnswerContentID = answerContent.ID,
								PipedResponseID = -1,
								Content = answerContent.Content,
								PipedContent = null,
								StartDate = now,
								CompleteDate = now
							};
							_surveyManager.CreateResponse(response);
							return true;
						}
					}
				}
				else
				{
					foreach (string answer in answers)
					{
						if (isPiped)
						{
							long pipedResponseID = -1;
							foreach (SMResponse pipedResponse in pipedResponses)
							{
								if (pipedResponse.Content.Equals(answer, StringComparison.CurrentCultureIgnoreCase))
								{
									pipedResponseID = pipedResponse.ID;
									break;
								}
							}

							DateTime now = DateTime.Now;
							SMResponse response = new SMResponse()
							{
								RespondentID = (_respondent != null ? _respondent.ID : -1),
								StateID = question.StateID,
								QuestionContentID = questionContent.ID,
								AnswerContentID = -1,
								PipedResponseID = pipedResponseID,
								Content = answer,
								PipedContent = null,
								StartDate = now,
								CompleteDate = now
							};
							_surveyManager.CreateResponse(response);
							postedResponse = true;
						}
						else
						{
							SMAnswerContent answerContent = GetMatchingAnswerContent(answerContents, answer);
							if (answerContent != null)
							{
								DateTime now = DateTime.Now;
								SMResponse response = new SMResponse()
								{
									RespondentID = (_respondent != null ? _respondent.ID : -1),
									StateID = question.StateID,
									QuestionContentID = questionContent.ID,
									AnswerContentID = answerContent.ID,
									PipedResponseID = -1,
									Content = answerContent.Content,
									PipedContent = null,
									StartDate = now,
									CompleteDate = now
								};
								_surveyManager.CreateResponse(response);
								postedResponse = true;
							}
						}
					}

					if (question.IsMultiselectSimpleQuestion())
					{
						foreach (string answer in seen)
						{
							if (isPiped)
							{
								long pipedResponseID = -1;
								foreach (SMResponse pipedResponse in pipedResponses)
								{
									if (pipedResponse.Content.Equals(answer, StringComparison.CurrentCultureIgnoreCase))
									{
										pipedResponseID = pipedResponse.ID;
										break;
									}
								}

								DateTime now = DateTime.Now;
								SMResponse response = new SMResponse()
								{
									RespondentID = (_respondent != null ? _respondent.ID : -1),
									StateID = question.StateID,
									QuestionContentID = questionContent.ID,
									AnswerContentID = -1,
									PipedResponseID = pipedResponseID,
									Content = "seen:" + answer,
									PipedContent = null,
									StartDate = now,
									CompleteDate = now
								};
								_surveyManager.CreateResponse(response);
								postedResponse = true;
							}
							else
							{
								SMAnswerContent answerContent = GetMatchingAnswerContent(answerContents, answer);
								if (answerContent != null)
								{
									DateTime now = DateTime.Now;
									SMResponse response = new SMResponse()
									{
										RespondentID = (_respondent != null ? _respondent.ID : -1),
										StateID = question.StateID,
										QuestionContentID = questionContent.ID,
										AnswerContentID = answerContent.ID,
										PipedResponseID = -1,
										Content = "seen:" + answerContent.Content,
										PipedContent = null,
										StartDate = now,
										CompleteDate = now
									};
									_surveyManager.CreateResponse(response);
									postedResponse = true;
								}
							}
						}
					}
				}
			}

			// Check for "Other - Specified"
			SMQuestionContent otherSpecifiedQuestionContent = GetOtherSpecifiedQuestionContent(question);
			if (question.HasOtherSpecify && otherSpecifiedQuestionContent != null)
			{
				if (!string.IsNullOrEmpty(otherSpecifiedText))
				{
					DateTime now = DateTime.Now;
					SMResponse response = new SMResponse()
					{
						RespondentID = (_respondent != null ? _respondent.ID : -1),
						StateID = question.StateID,
						QuestionContentID = otherSpecifiedQuestionContent.ID,
						AnswerContentID = -1,
						PipedResponseID = -1,
						Content = otherSpecifiedText,
						PipedContent = null,
						StartDate = now,
						CompleteDate = now
					};
					_surveyManager.CreateResponse(response);
					return true;
				}
			}

			if (question.ResponseOptional) return true;
			return postedResponse;
		}

        [Obsolete("Obsolete due to new Free-Form Matrix functionality", false)]
        public bool PostMatrixAnswers(SMQuestion question, List<int> answers)
        {
            _logger.Error(_className, "PostMatrixAnswers", "Use of old PostMatrixAnswers");
            return false;
        }

        [Obsolete("Obsolete due to new Free-Form Matrix functionality", false)]
        public bool PostMatrixAnswer(SMQuestion question, int questionIndex, int answerIndex)
        {
            _logger.Error(_className, "PostMatrixAnswer", "Use of old PostMatrixAnswer");
            return false;
        }

        public SMMatrixAnswer FindMatrixAnswerForMatrixCell(List<SMMatrixAnswer> matrixAnswers, long questionContentID, int columnIndex)
        {
            SMMatrixAnswer result = null;
            foreach (SMMatrixAnswer matrixAnswer in matrixAnswers)
            {
                if (matrixAnswer.QuestionContentID == questionContentID
                    && matrixAnswer.ColumnIndex == columnIndex)
                {
                    result = matrixAnswer;
                    break;
                }
            }
            return result;
        }

		/// <summary>
		/// Post the set of all responses for a matrix question
		/// </summary>
		/// <param name="question">matrix question</param>
		/// <param name="answers">list of list containing an answer for each cell of the matrix</param>
        /// /// <param name="radioAnswers">list containing an answer index (0..n-1) for each row of the matrix</param>
		/// <returns>true if response was posted, false otherwise</returns>
		public bool PostMatrixAnswers(SMQuestion question, List<List<string>> answers, List<int> radioAnswers)
		{
			const string methodName = "PostMatrixAnswers";

			if (question == null || answers == null || answers.Count < 1) return false;

			List<SMQuestionContent> questionContents = _surveyManager.RetrieveQuestionContents(question.ID, _language.ID, QuestionContentType.BODY_TEXT);
			List<SMQuestionContent> questionContentsToRemove = new List<SMQuestionContent>();
			foreach (SMQuestionContent questionContent in questionContents)
			{
				if (!string.IsNullOrWhiteSpace(questionContent.VisibilityExpression))
				{
					string expr = questionContent.VisibilityExpression;
					if (!IsItemVisible(questionContent.VisibilityExpression))
					{
						questionContentsToRemove.Add(questionContent);
					}
				}
			}
			foreach (SMQuestionContent questionContentToRemove in questionContentsToRemove)
			{
				questionContents.Remove(questionContentToRemove);
			}
			List<SMMatrixAnswer> matrixAnswers = _surveyManager.RetrieveMatrixAnswerByQuestionID(question.ID, _language.ID);
			if (questionContents == null || matrixAnswers == null)
				return false;

			List<SMResponse> pipedResponses = null;
			bool isPiped = question.CanBePiped();
			int numRows = questionContents.Count;
			if (isPiped)
			{
				SMQuestion pipedQuestion = GetPipedQuestion(question);
				pipedResponses = GetResponses(pipedQuestion);
				numRows = pipedResponses.Count;
			}
			if (numRows != answers.Count)
				return false;

            int numPoints = (int)question.MatrixPointScale;
			for (int rowIndex = 0; rowIndex < numRows; rowIndex++)
			{
				long qcID = isPiped ? questionContents[0].ID : questionContents[rowIndex].ID;

				// Delete previous answers if any
				if (!isPiped && _respondent != null && _respondent.ID != -1)
				{
					List<SMResponse> oldResponses = _surveyManager.RetrieveResponses(_respondent.ID, questionContents[rowIndex].ID);
					if (oldResponses != null && oldResponses.Count > 0)
					{
						if (ForceAllowSurveyReview || _survey.CanReviewQuestions())
						{
							foreach (SMResponse oldResponse in oldResponses)
							{
								_surveyManager.DeleteResponse(oldResponse);
							}
						}
						else
						{
							_logger.Error(_className, methodName, string.Format("Attempt to re-post answers to a survey (ID={0}, Name={1}) that doesn't support review.  Respondent ID = {2}", _survey.ID, _survey.Name, _respondent.ID));
							throw new SurveyQuestionException("You've already answered this question.");
						}
					}
				}

                bool hasRadio = false;
                for (int pointIndex = 0; pointIndex < numPoints; pointIndex++)
                {
					SMMatrixAnswer matrixAnswer = FindMatrixAnswerForMatrixCell(matrixAnswers, qcID, pointIndex);
                    if (matrixAnswer == null) return false;
                    if (matrixAnswer.AnswerControlType == QA_AnswerControlType.RADIO)
                    {
                        hasRadio = true;
                    }
                    else
                    {
                        string content = string.Empty;
                        if (answers != null && rowIndex < answers.Count && pointIndex < answers[rowIndex].Count)
                        {
                            content = StringUtils.FriendlyString(answers[rowIndex][pointIndex]).Trim();
                        }
						DateTime now = DateTime.Now;
						SMResponse response = new SMResponse()
						{
							RespondentID = (_respondent != null ? _respondent.ID : -1),
							StateID = question.StateID,
							QuestionContentID = qcID,
							AnswerContentID = -1,
							PipedResponseID = isPiped ? pipedResponses[rowIndex].ID : -1,
							MatrixAnswerID = matrixAnswer.ID,
							ColumnIndex = pointIndex,
							Content = content,
							PipedContent = isPiped ? pipedResponses[rowIndex].Content : null,
							StartDate = now,
							CompleteDate = now
						};
                        _surveyManager.CreateResponse(response);
                    }
                }
                if (hasRadio)
                {
                    string content = string.Empty;
                    if (radioAnswers != null && rowIndex < radioAnswers.Count)
                    {
                        content = StringUtils.FriendlyString(radioAnswers[rowIndex]).Trim();
                    }
					if (!string.IsNullOrEmpty(content))
					{
						DateTime now = DateTime.Now;
						SMResponse response = new SMResponse()
						{
							RespondentID = (_respondent != null ? _respondent.ID : -1),
							StateID = question.StateID,
							QuestionContentID = qcID,
							AnswerContentID = -1,
							PipedResponseID = isPiped ? pipedResponses[rowIndex].ID : -1,
							MatrixAnswerID = -1,
							ColumnIndex = -2,  // indicates radio group selection
							Content = content,
							PipedContent = isPiped ? pipedResponses[rowIndex].Content : null,
							StartDate = now,
							CompleteDate = now
						};
						_surveyManager.CreateResponse(response);
					}
                }
			}
			return true;
		}

		/// <summary>
		/// Validate the state model.
		/// </summary>
		/// <param name="warnings">list of warnings received while validating the state model</param>
		/// <returns>true if valid, false otherwise</returns>
		public bool Validate(ref List<SMValidationMessage> warnings)
		{
			if (_survey == null)
			{
				if (_surveyID != -1)
				{
					_survey = _surveyManager.RetrieveSurvey(_surveyID);
					_states = _survey.GetStates(_config);
				}
				else
					throw new ArgumentNullException("SurveyID");
			}

			// Check for cycles in the graph
			try
			{
				// Throws CircularPathException if a cycle is detected
				CheckForCycles();
			}
			catch (CircularPathException ex)
			{
				SMState duplicateState = FindStateByID(ex.DuplicateStateID);
				string duplicateStateName = duplicateState != null ? duplicateState.UIName : "ID " + ex.DuplicateStateID;
				warnings.Add(new SMValidationMessage(SMValidationLevel.Exception,
					string.Format("The Survey cannot be validated because a circular execution path was detected at state {0}.",
					duplicateStateName)));
				return false;
			}

			List<long> invalidStates = new List<long>();
			bool hasStart = false;
			int numStart = 0;
			bool hasTerminate = false;
			foreach (SMState state in _states)
			{
				if (state.StateType == StateType.Start)
				{
					hasStart = true;
					numStart++;
				}
				if (state.StateType == StateType.Terminate || state.StateType == StateType.Skip)
				{
					hasTerminate = true;
				}

				switch (state.StateType)
				{
					case StateType.Start:
					case StateType.PageStart:
						if (state.GetInputs(_config).Count != 0)
						{
							warnings.Add(new SMValidationMessage(SMValidationLevel.Exception, string.Format("State \"{0}\" requires no input states.", state.UIName)));
							invalidStates.Add(state.ID);
						}
						break;

					default:
						if (state.GetInputs(_config).Count < 1)
						{
							warnings.Add(new SMValidationMessage(SMValidationLevel.Exception, string.Format("State \"{0}\" requires at least one input state.", state.UIName)));
							invalidStates.Add(state.ID);
						}
						break;
				}

				// State outputs: terminate = 0, decision = 2+, else 1
				switch (state.StateType)
				{
					case StateType.Terminate:
					case StateType.Skip:
					case StateType.PageEnd:
						if (state.GetOutputs(_config).Count != 0)
						{
							warnings.Add(new SMValidationMessage(SMValidationLevel.Exception, string.Format("State \"{0}\" requires no output states.", state.UIName)));
							invalidStates.Add(state.ID);
						}
						break;

					case StateType.Decision:
						SMDecision decision = _surveyManager.RetrieveDecisionByStateID(state.ID);
						if (decision == null || decision.NumConditions() <= 0)
						{
							warnings.Add(new SMValidationMessage(SMValidationLevel.Exception, string.Format("State \"{0}\" requires at least one condition.", state.UIName)));
							invalidStates.Add(state.ID);
						}
						else if (state.GetOutputs(_config).Count < 2)
						{
							warnings.Add(new SMValidationMessage(SMValidationLevel.Exception, string.Format("State \"{0}\" requires at least two output states.", state.UIName)));
							invalidStates.Add(state.ID);
						}
						break;

					default:
						if (state.GetOutputs(_config).Count != 1)
						{
							warnings.Add(new SMValidationMessage(SMValidationLevel.Exception, string.Format("State \"{0}\" requires exactly one output state.", state.UIName)));
							invalidStates.Add(state.ID);
						}
						break;
				}
			}

			if (!hasStart)
			{
				warnings.Add(new SMValidationMessage(SMValidationLevel.Exception, string.Format("No start state.")));
			}
			if (numStart > 1)
			{
				warnings.Add(new SMValidationMessage(SMValidationLevel.Exception, string.Format("More than one start state.")));
			}
			if (!hasTerminate)
			{
				warnings.Add(new SMValidationMessage(SMValidationLevel.Exception, string.Format("No terminate state.")));
			}

			foreach (SMState state in _states)
			{
				if (invalidStates.Contains(state.ID))
				{
					continue;
				}

				bool invalid = false;
				foreach (SMTransition inputTransition in state.GetInputs(_config))
				{
					if (invalidStates.Contains(inputTransition.SrcStateID))
					{
						invalid = true;
						break;
					}
				}
				if (invalid)
				{
					continue;
				}
			}

			if (invalidStates.Count > 0)
			{
				return false; //invalid Survey
			}
			else
			{
				return true; //valid Survey - could possibly have warnings, though.
			}
		}

		/// <summary>
		/// Check for cycles in the state model.  Throws CircularPathException if a cycle is detected.
		/// </summary>
		public void CheckForCycles()
		{
			// Get the list of states with no inputs
			List<long> starterStates = new List<long>();
			foreach (SMState state in _states)
			{
				if (state.GetInputs(_config).Count == 0)
				{
					starterStates.Add(state.ID);
				}
			}

			// Check for cycles in each starter state's graph
			foreach (long currentState in starterStates)
			{
				// Throws CircularPathException if cycle detected
				FindCycles(currentState, new Stack<long>());
			}
		}

		public bool IsItemVisible(string expression)
		{
			const string methodName = "IsItemVisible";

			ContextObject contextObject = new ContextObject();
			var args = new Dictionary<string, object>();
			args.Add("surveyID", _survey.ID.ToString());
			args.Add("respondentID", _respondent.ID.ToString());
			args.Add("languageID", _language.ID.ToString());
			args.Add("stateID", _stateID.ToString());
			contextObject.Environment = args;

			if (Member != null)
			{
				contextObject.Owner = Member;
			}
			else if (IPCode != -1)
			{
				using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					Member = loyalty.LoadMemberFromIPCode(IPCode);
					contextObject.Owner = Member;
				}
			}

			bool result = true;
			try
			{
				ExpressionFactory expressionFactory = new ExpressionFactory();
				Expression expr = expressionFactory.Create(expression);
				object obj = expr.evaluate(contextObject);
				result = StringUtils.FriendlyBool(obj, true);
				_logger.Debug(_className, methodName, string.Format("Expression '{0}' => {1}", expression, result));
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, string.Format("Failed to evaluate expression '{0}': {1}", expression, ex.Message), ex);
			}
			return result;
		}

		public string EvaluateContent(string content, bool evaluateContent, Queue<SMStateModel.TokenReplacement> replacements)
		{
			ContextObject contextObject = new ContextObject();
			var args = new Dictionary<string, object>();
			args.Add("surveyID", _survey.ID.ToString());
			args.Add("respondentID", _respondent.ID.ToString());
			args.Add("languageID", _language.ID.ToString());
			args.Add("stateID", _stateID.ToString());
			contextObject.Environment = args;

			if (Member != null)
			{
				contextObject.Owner = Member;
			}
			else if (IPCode != -1)
			{
				using (var loyalty = new LoyaltyDataService(_config))
				{
					Member = loyalty.LoadMemberFromIPCode(IPCode);
					contextObject.Owner = Member;
				}
			}

			ExpressionFactory expressionFactory = new ExpressionFactory();
			string delimeter = "##";
			int nPasses = 5;

			string result = content;
			for (int i = 0; i < nPasses; i++)
			{
				if (result.IndexOf("##") != -1)
				{
					if (replacements == null)
					{
						replacements = new Queue<SMStateModel.TokenReplacement>();
					}
					result = MatchAndReplace(delimeter, result, contextObject, expressionFactory, evaluateContent, replacements);
				}
				else
				{
					break;
				}
			}
			return result;
		}
		#endregion
		#endregion

		#region private methods
		private class ExpressionToken
		{
			public int begin;
			public int end;
			public string expression;
			public string result;
		}

		[Serializable]
		public class TokenReplacement
		{
			public string token;
			public string replacement;
		}

		private string MatchAndReplace(string delimeter, string lcontent, ContextObject contextObject, ExpressionFactory expressionFactory, bool evaluateContent, Queue<SMStateModel.TokenReplacement> replacements)
		{
			List<ExpressionToken> tokens = new List<ExpressionToken>();

			// first phase - find and evaluate tokens.
			int beginIndex = -1;
			while (beginIndex < lcontent.Length - 4) //need at least 4 characters for the 2 ##'s (plus whatever expression falls in between)
			{
				beginIndex = lcontent.IndexOf(delimeter, beginIndex == -1 ? 0 : beginIndex + 2);
				if (beginIndex != -1)
				{
					int endIndex = lcontent.IndexOf(delimeter, beginIndex + 1);
					if (endIndex == -1)
					{
						throw new FormatException("Invalid content format.");
					}
					ExpressionToken token = new ExpressionToken();
					tokens.Add(token);
					token.begin = beginIndex;
					token.end = endIndex;
					string expr = lcontent.Substring(beginIndex + 2, endIndex - beginIndex - 2);
					string evalautedStr = string.Empty;
					if (evaluateContent) {
						evalautedStr = EvaluateExpression(expr, contextObject, expressionFactory);
					}
					else if (replacements != null && replacements.Count > 0)
					{
						evalautedStr = replacements.Dequeue().replacement;
					}
					token.expression = expr;
					token.result = evalautedStr;
					beginIndex = endIndex + 2;
					if (evaluateContent)
					{
						if (replacements == null) replacements = new Queue<TokenReplacement>();
						replacements.Enqueue(new TokenReplacement() { token = expr, replacement = evalautedStr });
					}
				}
				else
				{
					break;
				}
			}

			// phase 2 - construct return string.
			StringBuilder strBuilder = new StringBuilder();
			if (tokens.Count > 0)
			{
				int idx = 0;
				foreach (ExpressionToken token in tokens)
				{
					string piece = lcontent.Substring(idx, token.begin - idx);
					strBuilder.Append(piece);
					strBuilder.Append(token.result);
					idx = token.end + 2;
				}
				int left = lcontent.Length - idx;
				if (left > 0)
				{
					strBuilder.Append(lcontent.Substring(idx, left));
				}
			}
			else
			{
				// no tokens found.
				strBuilder.Append(lcontent);
			}

			return strBuilder.ToString();
		}

		private string EvaluateExpression(string expression, ContextObject contextObject, ExpressionFactory expressionFactory)
		{
			Expression expr = expressionFactory.Create(expression);
			string result = string.Empty;

			result = StringUtils.FriendlyString(expr.evaluate(contextObject), string.Empty);
			return result;
		}

		private SMAnswerContent GetMatchingAnswerContent(List<SMAnswerContent> answerContents, string answer)
		{
			SMAnswerContent result = null;
			foreach (SMAnswerContent answerContent in answerContents)
			{
                String thisAnswerContent = StringUtils.DeHTML(StringUtils.FriendlyString(answerContent.Content));
				if (thisAnswerContent.Contains("##"))
				{
					thisAnswerContent = EvaluateContent(thisAnswerContent, true, new Queue<TokenReplacement>());
				}
                if (thisAnswerContent == answer)
				{
					result = answerContent;
				}
			}
			return result;
		}

		private bool SelectLanguage()
		{
			if (_language == null)
			{
				// user's culture
				string culture = DEFAULT_CULTURE;
				if (_cultures != null && _cultures.Length > 0)
				{
					culture = _cultures[0];
				}

				// find cultureMap for culture
				SMCultureMap cultureMap = _surveyManager.RetrieveCultureMap(culture);
				if (cultureMap == null && culture != DEFAULT_CULTURE)
				{
					culture = DEFAULT_CULTURE;
					cultureMap = _surveyManager.RetrieveCultureMap(culture);
				}

				// use indicated language
				if (cultureMap != null)
				{
					_language = _surveyManager.RetrieveLanguage(cultureMap.LanguageID);
				}

				// use default language
				if (_language == null)
				{
					_language = _surveyManager.RetrieveLanguage(DEFAULT_LANGUAGE);
				}
			}
			return (_language != null);
		}

		private bool SelectSurveyViaSurveyID()
		{
			const string methodName = "SelectSurveyViaSurveyID";

			if (_surveyID == -1)
			{
				_status = StateModelStatus.NoSurvey;
				string message = string.Format("SurveyID is -1 while survey selection method is by SurveyID.");
				_logger.Error(_className, methodName, message);
				return false;
			}

			if (_survey == null || _survey.ID != _surveyID)
			{
				_survey = _surveyManager.RetrieveSurvey(_surveyID);
				if (_survey == null)
				{
					_status = StateModelStatus.NoSurvey;
					string message = string.Format("Survey is null for SurveyID '{0}'.", _surveyID);
					_logger.Error(_className, methodName, message);
					return false;
				}
			}

			bool canSurveyBeViewed = VerifySurveyCanBeViewed(_survey, ref _status, true);
			if (canSurveyBeViewed)
			{
				_states = _survey.GetStates(_config);
			}
			return canSurveyBeViewed;
		}

		private bool SelectSurveyViaBscript()
		{
			const string methodName = "SelectSurveyViaBscript";

			if (string.IsNullOrWhiteSpace(_bscript))
			{
				_status = StateModelStatus.NoSurvey;
				string message = "Bscript is null/empty while survey selection method is by Bscript.";
				_logger.Error(_className, methodName, message);
				return false;
			}

			Expression expr = new ExpressionFactory().Create(_bscript);
			ContextObject contextObject = new ContextObject();
			object obj = expr.evaluate(contextObject);
			string surveyName = (obj != null ? obj.ToString() : string.Empty);
			if (string.IsNullOrWhiteSpace(surveyName))
			{
				_status = StateModelStatus.NoSurvey;
				string message = string.Format("Bscript '{0}' evaluates to null/empty.", _bscript);
				_logger.Error(_className, methodName, message);
				return false;
			}
			
			SMSurvey namedSurvey = _surveyManager.RetrieveSurvey(surveyName);
			if (namedSurvey == null)
			{
				_status = StateModelStatus.NoSurvey;
				string message = string.Format("No survey found for surveyName {0}, method bscript", surveyName);
				_logger.Error(_className, methodName, message);
				return false;
			}

			_surveyID = namedSurvey.ID;
			_survey = namedSurvey;

			bool canSurveyBeViewed = VerifySurveyCanBeViewed(_survey, ref _status, true);
			if (canSurveyBeViewed)
			{
				_states = _survey.GetStates(_config);
			}
			return canSurveyBeViewed;
		}

		private bool VerifySurveyCanBeViewed(SMSurvey survey, ref StateModelStatus status, bool logErrors)
		{
			const string methodName = "VerifySurveyCanBeViewed";

			if (survey.SurveyStatus == SurveyStatus.Design && !_allowPreview)
			{
				status = StateModelStatus.SurveyUnpublished;
				if (logErrors)
				{
					string message = string.Format("Survey '{0}' is not active, it needs to be published.", survey.Name);
					_logger.Error(_className, methodName, message);
				}
				return false;
			}

			if (survey.SurveyStatus == SurveyStatus.Closed)
			{
				status = StateModelStatus.SurveyClosed;
				if (logErrors)
				{
					string message = string.Format("Survey '{0}' is closed.", survey.Name);
					_logger.Error(_className, methodName, message);
				}
				return false;
			}

			if (!survey.IsSurveyEffective())
			{
				status = StateModelStatus.SurveyNotEffective;
				if (logErrors)
				{
					string message = string.Format("Survey '{0}' is not effective.", survey.Name);
					_logger.Error(_className, methodName, message);
				}
				return false;
			}

			if (!survey.IsAbsoluteQuotaNotExceeded(_config))
			{
				status = StateModelStatus.QuotaExceeded;
				if (logErrors)
				{
					string message = string.Format("Survey '{0}' absolute quota exceeded.", survey.Name);
					_logger.Error(_className, methodName, message);
				}
				return false;
			}

			return true;
		}

		private bool VerifyRespondentCanTakeSurvey(SMRespondent respondent, ref StateModelStatus status, bool isSingleRespondent)
		{
			const string methodName = "VerifyRespondentCanTakeSurvey";

			if (!string.IsNullOrEmpty(_mtouch) && respondent.MTouch != _mtouch)
			{
				status = StateModelStatus.NoRespondent;
				if (isSingleRespondent)
				{
					_status = status;
					string message = string.Format("Respondent '{0}' mtouch mismatch: '{1}' != '{2}'.", respondent.ID, respondent.MTouch, _mtouch);
					_logger.Error(_className, methodName, message);
				}
				return false;
			}

			if (_ipcode != -1 && respondent.IPCode != _ipcode)
			{
				status = StateModelStatus.NoRespondent;
				if (isSingleRespondent)
				{
					_status = status;
					string message = string.Format("Respondent '{0}' ipcode mismatch: '{1}' != '{2}'.", respondent.ID, respondent.IPCode, _ipcode);
					_logger.Error(_className, methodName, message);
				}
				return false;
			}

			SMSurvey survey = ((_survey != null && _survey.ID == respondent.SurveyID) 
				? _survey 
				: _surveyManager.RetrieveSurvey(respondent.SurveyID)
			);

			if (respondent.CompleteDate != null && !(survey.CanReviewQuestions() || ForceAllowSurveyReview))
			{
				status = StateModelStatus.AlreadyTookSurvey;
				if (isSingleRespondent)
				{
					_status = status;
					string message = string.Format("Respondent '{0}' has already taken survey '{1}'.", respondent.ID, respondent.SurveyID);
					_logger.Error(_className, methodName, message);
				}
				return false;
			}

			if (!VerifySurveyCanBeViewed(survey, ref status, isSingleRespondent))
			{
				status = StateModelStatus.NotInitialized;
				if (isSingleRespondent)
				{
					_status = status;
				}
				return false;
			}

			string propName = null;
			string propValue = null;
			if (!survey.IsSegmentQuotaNotExceeded(_config, respondent, ref propName, ref propValue))
			{
				status = StateModelStatus.QuotaExceeded;
				if (isSingleRespondent)
				{
					_status = status;
					string message = string.Format("Survey '{0}' segment quota '{1}={2}' exceeded.", survey.Name, propName, propValue);
					_logger.Error(_className, methodName, message);
				}
				return false;
			}

			return true;
		}

		private bool SelectRespondentForSelectedSurvey()
		{
			const string methodName = "SelectRespondentForSelectedSurvey";

			EvictRespondentCache();

			List<SMRespondent> eligibleRespondents = _surveyManager.RetrieveEligibleRespondents(_surveyID, _language.ID, _mtouch, _ipcode);
			if (eligibleRespondents.Count <= 0 && !AutoCreateRespondent(ref eligibleRespondents))
			{
				_status = StateModelStatus.NoEligibleRespondent;
				string message = string.Format("No eligible respondents for survey {0} language {1}, mtouch {2}, ipcode {3}, method {4}",
					_surveyID, _language.ID, _mtouch, _ipcode, _surveySelectionMethod);
				_logger.Error(_className, methodName, message);
				return false;
			}

			_logger.Trace(_className, methodName,
				string.Format("Found {0} eligible respondents for survey {1} language {2}, mtouch {3}, ipcode {4}, method {5}",
				eligibleRespondents.Count, _surveyID, _language.ID, _mtouch, _ipcode, _surveySelectionMethod));

			SMRespondent eligibleRespondent = null;
			bool isSingleRespondent = eligibleRespondents.Count == 1;
			foreach (SMRespondent respondent in eligibleRespondents)
			{
				if (respondent.SurveyID != _survey.ID)
				{
					// this should never happen since (1) we have a survey selected, (2) the eligible respondents were selected 
					// based on the selected survey, and (3) respondent for this survey would have already been autocreated if possible
					_logger.Trace(_className, methodName,
						string.Format("{0}: No match because different survey {1} for survey {2} language {3} mtouch {4} ipcode {5}, method {6}, status {7}",
						respondent.ID, respondent.SurveyID, _survey.ID, Language.ID, MTouch, IPCode, _surveySelectionMethod, _status));
					continue;
				}

				if (!VerifySurveyCanBeViewed(_survey, ref _status, isSingleRespondent))
				{
					_logger.Trace(_className, methodName,
						string.Format("{0}: No match because survey can't be viewed: survey {1} language {2} mtouch {3} ipcode {4}, method {5}, status {6}",
						respondent.ID, _survey.ID, Language.ID, MTouch, IPCode, _surveySelectionMethod, _status));
					continue;
				}

				if (!ForceAllowSurveyReview && !VerifyRespondentCanTakeSurvey(respondent, ref _status, isSingleRespondent))
				{
					_logger.Trace(_className, methodName,
						string.Format("{0}: No match because respondent can't take survey: survey {1} language {2} mtouch {3} ipcode {4}, method {5}, status {6}",
						respondent.ID, _survey.ID, Language.ID, MTouch, IPCode, _surveySelectionMethod, _status));
					continue;
				}
				
				// found eligible respondent!
				eligibleRespondent = respondent;
				break;
			}
			if (eligibleRespondent == null)
			{
				if (!isSingleRespondent)
				{
					_status = StateModelStatus.NoMatchingRespondent;
					string message = string.Format("No matching respondents for survey {0}, language {1}, mtouch {2}, ipcode {3}, method {4}",
						_surveyID, _language.ID, _mtouch, _ipcode, _surveySelectionMethod);
					_logger.Error(_className, methodName, message);
				}
				return false;
			}

			_respondent = eligibleRespondent;
			string msg = string.Format("Match respondent {0} for survey {1}, language {2}, mtouch {3}, ipcode {4}, method {5}",
				eligibleRespondent.ID, _surveyID, _language.ID, _mtouch, _ipcode, _surveySelectionMethod);
			_logger.Trace(_className, methodName, msg);
			return true;
		}

		private bool AutoCreateRespondent(ref List<SMRespondent> eligibleRespondents)
		{
			const string methodName = "AutoCreateRespondent";

			if (_survey.AutoCreateRespondents())
			{
				DateTime now = DateTime.Now;
				SMRespondent respondent = new SMRespondent()
				{
					SurveyID = _surveyID,
					LanguageID = _language.ID,
					MTouch = _mtouch,
					IPCode = _ipcode,
					CreateDate = now,
					UpdateDate = now,
					Skipped = false
				};
				_surveyManager.CreateRespondent(respondent);
				eligibleRespondents.Add(respondent);

				string message = string.Format("Autocreated respondent {0} for survey {1} language {2}, mtouch {3}, ipcode {4}, method {5}",
					respondent.ID, _surveyID, _language.ID, _mtouch, _ipcode, _surveySelectionMethod);
				_logger.Trace(_className, methodName, message);
				return true;
			}
			return false;
		}

		private bool SelectAnyAvailableSurveyAndRespondent()
		{
			const string methodName = "SelectAnyAvailableSurveyAndRespondent";

			EvictRespondentCache();

			List<SMRespondent> eligibleRespondents = _surveyManager.RetrieveEligibleRespondents(_language.ID, _mtouch, _ipcode);
			if (eligibleRespondents.Count <= 0)
			{
				// look at all effective surveys to see if we can autocreate a respondent for one
				List<SMSurvey> surveys = _surveyManager.RetrieveSurveys((int)SurveyStatus.Active, -1);
				StateModelStatus status = StateModelStatus.NotInitialized;
				if (surveys != null && surveys.Count > 0)
				{
					foreach (SMSurvey survey in surveys)
					{
						if (VerifySurveyCanBeViewed(survey, ref status, false))
						{
							if (AutoCreateRespondent(ref eligibleRespondents))
							{
								break;
							}
						}
					}
				}

				if (eligibleRespondents.Count <= 0)
				{
					_status = StateModelStatus.NoEligibleRespondent;
					string message = string.Format("No eligible survey respondents for language {0}, mtouch {1}, ipcode {2}",
						_language.ID, _mtouch, _ipcode);
					_logger.Error(_className, methodName, message);
					return false;
				}
			}

			_logger.Debug(_className, methodName,
				string.Format("Found {0} eligible respondents for language {1}, mtouch {2}, ipcode {3}",
				eligibleRespondents.Count, _language.ID, _mtouch, _ipcode));

			bool isSingleRespondent = eligibleRespondents.Count == 1;
			foreach (SMRespondent eligibleRespondent in eligibleRespondents)
			{
				StateModelStatus dummy = StateModelStatus.NotInitialized;
				if (VerifyRespondentCanTakeSurvey(eligibleRespondent, ref dummy, isSingleRespondent))
				{
					_respondent = eligibleRespondent;
					break;
				}
			}

			if (_respondent == null)
			{
				_status = StateModelStatus.NoMatchingRespondent;
				string message = string.Format("No match found for mtouch {0}, ipcode {1}, method AnyAvailable", _mtouch, _ipcode);
				_logger.Error(_className, methodName, message);
				return false;
			}

			_surveyID = _respondent.SurveyID;
			if (_survey == null || _survey.ID != _surveyID)
			{
				_survey = _surveyManager.RetrieveSurvey(_surveyID);
				if (_survey != null)
				{
					_states = _survey.GetStates(_config);
				}
			}

			return true;
		}

		private void EvictRespondentCache()
		{
			if (StateID == -1)
			{
				// try to find out if we're in a "get" (not a postback), otherwise just use _allowPreview value
				bool isNotPostBack = _allowPreview;
				try
				{
					isNotPostBack = !((System.Web.UI.Page)System.Web.HttpContext.Current.CurrentHandler).IsPostBack;
				}
				catch { }

				if (_allowPreview || isNotPostBack)
				{
					List<SMRespondent> cachedEligibleRespondents = _surveyManager.RetrieveEligibleRespondents(_language.ID, _mtouch, _ipcode);
					if (cachedEligibleRespondents != null && cachedEligibleRespondents.Count > 0)
					{
						foreach (SMRespondent eligibleRespondent in cachedEligibleRespondents)
						{
							_surveyManager.EvictRespondentFromCache(eligibleRespondent);
						}
					}
				}
			}

		}

		private void SelectCurrentState()
		{
			const string methodName = "SelectCurrentState";
			_currentState = null;
			_numPages = NumRestingStates();
			if (_stateID != -1)
			{
				// existing state
				_logger.Debug(_className, methodName, "Looking for existing state by ID: " + _stateID);
				_currentState = FindStateByID(_stateID);
				if (_currentState != null)
				{
					_stateID = _currentState.ID;
					_status = StateModelStatus.Initialized;
					if (IsRestingState(_currentState))
					{
						_status = StateModelStatus.OnRestingState;
					}
					_logger.Debug(_className, methodName, string.Format("Found existing state by ID {0}: '{1}'", _stateID, _currentState.UIName));
				}
				else
				{
					_logger.Error(_className, methodName, "Can't find existing state by ID: " + _stateID);
					_stateID = -1;
				}
			}

			if (_stateID == -1)
			{
				_logger.Debug(_className, methodName, "StateID = -1 so looking for start state.");
				_currentState = FindStartState();
				if (_currentState != null)
				{
					_stateID = _currentState.ID;
					_status = StateModelStatus.Initialized;

					_logger.Debug(_className, methodName, "Found start state, so iterating model.");

					// Sets _currentState as a side effect
					IterateModel();

					if (_currentState != null)
					{
						_logger.Debug(_className, methodName, string.Format("State model rested on state '{0}' ({1}).", _currentState.UIName, _currentState.ID));
					}
					else
					{
						_logger.Warning(_className, methodName, "State model terminated.");
					}
				}
				else
				{
					_logger.Warning(_className, methodName, "Couldn't find start state.");
					_stateID = -1;
				}
			}

			_curPage = GetPageNum();
		}

		private long GetPageNum()
		{
			if (_currentState == null) return -1;
			if (!IsRestingState(_currentState)) return -1;

			long result = 0;
			SMState tmpState = _currentState;
			SMStateCollection states = new SMStateCollection(_config, _surveyID, _states);
			while (true)
			{
				SMTransitionCollection inputs = tmpState.GetInputs(_config);
				if (inputs == null || inputs.Count < 1) break;

				if (inputs[0].SrcStateID == tmpState.ID)
				{
					tmpState = states[inputs[0].DstStateID];
				}
				else
				{
					tmpState = states[inputs[0].SrcStateID];
				}

				switch (tmpState.StateType)
				{
					case StateType.Message:
					case StateType.Question:
					case StateType.MatrixQuestion:
                    case StateType.Page:
						result++;
						break;
				}
			}
			return result;
		}

		private long NumRestingStates()
		{
			long result = 0;
			if (_states != null && _states.Count > 0)
			{
				foreach (SMState state in _states)
				{
					switch (state.StateType)
					{
						case StateType.Message:
						case StateType.Question:
						case StateType.MatrixQuestion:
                        case StateType.Page:
							result++;
							break;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Find a state by it's unique identifier
		/// </summary>
		/// <param name="stateID">unique identifier for the state</param>
		/// <returns>state, or null if not exists</returns>
		private SMState FindStateByID(long stateID)
		{
			SMState result = null;
			foreach (SMState state in _states)
			{
				if (state.ID == stateID)
				{
					result = state;
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// Find the start state.
		/// </summary>
		/// <returns>start state, or null if not exists</returns>
		private SMState FindStartState()
		{
			SMState result = null;
			foreach (SMState state in _states)
			{
				if (state.StateType == StateType.Start)
				{
					result = state;
				}
			}
			return result;
		}

		/// <summary>
		/// Find cycles using a depth first search.  Throws CircularPathException if a cycle is detected.
		/// </summary>
		/// <param name="currentState">the current state being examined</param>
		/// <param name="visitedStates">chain of states leading from currentState to the starter state</param>
		private void FindCycles(long currentStateID, Stack<long> visitedStates)
		{
			SMState currentState = FindStateByID(currentStateID);
			if (currentState == null)
				return;

			// We got to a terminal node without finding a cycle
			SMTransitionCollection currentStateOutputs = currentState.GetOutputs(_config);
			if (currentStateOutputs.Count < 1)
				return;

			// I've been here before
			if (visitedStates.Contains(currentStateID))
				throw new CircularPathException("Cycle detected.", currentStateID);

			// Recursively check children nodes
			visitedStates.Push(currentStateID);
			List<long> childStates = new List<long>();
			foreach (SMTransition outputTransition in currentStateOutputs)
			{
				SMState childState = FindStateByID(outputTransition.DstStateID);
				FindCycles(childState.ID, visitedStates);
			}
			visitedStates.Pop();
		}

		private bool IsRestingState(SMState state)
		{
			bool result = false;
			if (state != null)
			{
				switch (state.StateType)
				{
					case StateType.Message:
					case StateType.Question:
					case StateType.MatrixQuestion:
                    case StateType.Page:
						result = true;
						break;
				}
			}
			return result;
		}

		private void IterateModel()
		{
			if (_currentState == null) return;

			// We're currently on a resting state, so go to the next state
			if (IsRestingState(_currentState))
			{
				if (_currentState.GetOutputs(_config) != null && _currentState.GetOutputs(_config).Count > 0)
				{
					// follow single output transition to next state
					_currentState = FindStateByID(_currentState.GetOutputs(_config)[0].DstStateID);
					_stateID = (_currentState == null ? -1 : _currentState.ID);
				}
				else
				{
					// resting state with no output transition
					_terminationType = StateModelTerminationType.MissingTransition;
					Terminate();
					return;
				}
			}

			// Keep going until we see a stopping state
			while (_currentState != null)
			{
				switch (_currentState.StateType)
				{
					case StateType.Start:
						if (_currentState.GetOutputs(_config) != null && _currentState.GetOutputs(_config).Count > 0)
						{
							// follow single output transition to next state
							_currentState = FindStateByID(_currentState.GetOutputs(_config)[0].DstStateID);
							_stateID = (_currentState == null ? -1 : _currentState.ID);
						}
						else
						{
							// start with no output transition
							_terminationType = StateModelTerminationType.MissingTransition;
							Terminate();
							return;
						}
						break;

					case StateType.Message:
						List<SMResponse> messageResponses = _surveyManager.RetrieveResponsesForState(_respondent.ID, _currentState.ID);
						if (ForceAllowSurveyReview || messageResponses == null || messageResponses.Count < 1)
						{
							// wait for response to this resting state
							_status = StateModelStatus.OnRestingState;
							_curPage = GetPageNum();
							return;
						}
						else if (_currentState.GetOutputs(_config) != null && _currentState.GetOutputs(_config).Count > 0)
						{
							// follow single output transition to next state
							_currentState = FindStateByID(_currentState.GetOutputs(_config)[0].DstStateID);
							_stateID = (_currentState == null ? -1 : _currentState.ID);
						}
						else
						{
							// resting state with no output transition
							_terminationType = StateModelTerminationType.MissingTransition;
							Terminate();
							return;
						}
						break;

					case StateType.Page:
						if (ForceAllowSurveyReview || _survey.CanReviewQuestions() || NeedResponseForPage(_currentState, _language.ID, _respondent.ID))
						{
							// wait for response to this resting state
							_status = StateModelStatus.OnRestingState;
							_curPage = GetPageNum();
							return;
						}
						else if (_currentState.GetOutputs(_config) != null && _currentState.GetOutputs(_config).Count > 0)
						{
							// follow single output transition to next state
							_currentState = FindStateByID(_currentState.GetOutputs(_config)[0].DstStateID);
							_stateID = (_currentState == null ? -1 : _currentState.ID);
						}
						else
						{
							// resting state with no output transition
							_terminationType = StateModelTerminationType.MissingTransition;
							Terminate();
							return;
						}
						break;

					case StateType.Question:
					case StateType.MatrixQuestion:
						if (ForceAllowSurveyReview || _survey.CanReviewQuestions() 
                            || _surveyManager.NeedResponse(_currentState.ID, _language.ID, _respondent.ID))
						{
							// wait for response to this resting state
							_status = StateModelStatus.OnRestingState;
							_curPage = GetPageNum();
							return;
						}
						else if (_currentState.GetOutputs(_config) != null && _currentState.GetOutputs(_config).Count > 0)
						{
							// follow single output transition to next state
							_currentState = FindStateByID(_currentState.GetOutputs(_config)[0].DstStateID);
							_stateID = (_currentState == null ? -1 : _currentState.ID);
						}
						else
						{
							// resting state with no output transition
							_terminationType = StateModelTerminationType.MissingTransition;
							Terminate();
							return;
						}
						break;

					case StateType.Decision:
						SMTransitionCollection outputs = _currentState.GetOutputs(_config);
						if (outputs != null && outputs.Count > 0)
						{
							bool exprResult = false;
							int index = -1;
							EvaluateDecisionState(ref exprResult, ref index);
							if (exprResult)
							{
								SMTransition output = GetDecisionOutput(outputs, index);
								if (output != null)
								{
									// output transition for a true condition
									_currentState = FindStateByID(output.DstStateID);
									_stateID = (_currentState == null ? -1 : _currentState.ID);
								}
								else
								{
									// no output transition exists at that index, so we terminate
									_terminationType = StateModelTerminationType.MissingTransition;
									Terminate();
								}
							}
							else
							{
								SMTransition defaultDecisionOutput = GetDefaultDecisionOutput(outputs);
								if (defaultDecisionOutput != null)
								{
									// false with false transition
									_currentState = FindStateByID(defaultDecisionOutput.DstStateID);
									_stateID = (_currentState == null ? -1 : _currentState.ID);
								}
								else
								{
									// false, but no false transition exists, so we terminate
									_terminationType = StateModelTerminationType.MissingTransition;
									Terminate();
								}
							}
						}
						else
						{
							// No outputs for the decision state, so we terminate
							_terminationType = StateModelTerminationType.MissingTransition;
							Terminate();
						}
						break;

					case StateType.Terminate:
						_terminationType = StateModelTerminationType.Success;
						Terminate();
						break;

					case StateType.Skip:
						_respondent.Skipped = true;
						_terminationType = StateModelTerminationType.Skip;
						Terminate();
						break;
				}
			}
		}

		private SMTransition GetDefaultDecisionOutput(SMTransitionCollection outputs)
		{
			SMTransition result = GetDecisionOutput(outputs, -1);
			return result;
		}

		private SMTransition GetDecisionOutput(SMTransitionCollection outputs, int index)
		{
			SMTransition result = null;
			if (outputs != null && outputs.Count > 0)
			{
				foreach (SMTransition output in outputs)
				{
					if (output.SrcConnectorIndex == index)
					{
						result = output;
						break;
					}
				}
			}
			return result;
		}

		private void Terminate()
		{
			InvokeBusinessRules();

			_currentState = null;
			_stateID = -1;
			_status = StateModelStatus.Completed;
			_respondent.CompleteDate = DateTime.Now;
			_surveyManager.UpdateRespondent(_respondent);
			_curPage = -1;

			SendThankYouEmail();
		}

		private void InvokeBusinessRules()
		{
			const string methodName = "InvokeBusinessRules";
			try
			{
				using (var loyalty = new LoyaltyDataService(_config))
				{
					// Is there a survey?
					if (_survey == null)
					{
						_logger.Debug(_className, methodName, "Survey is null");
						return;
					}

					// Is there a rule to invoke?
					RuleTrigger rule = null;
					long ruleId = -1;
					bool isSurveyCompleted = false;
					switch (_terminationType)
					{
						case StateModelTerminationType.Success:
						case StateModelTerminationType.MissingTransition:
							ruleId = _survey.SurveyCompleteRuleId;
							isSurveyCompleted = true;
							break;
						case StateModelTerminationType.Skip:
							ruleId = _survey.SurveyTerminateAndTallyRuleId;
							break;
					}
					if (ruleId < 0)
					{
						if (isSurveyCompleted && _surveyCompletedAccrualPoints != null)
						{
							// Is there a default rule we can use?
							string ruleName = LWConfigurationUtil.GetConfigurationValue("SurveyDefaultAwardPointRule");
							if (!string.IsNullOrWhiteSpace(ruleName))
							{

								rule = loyalty.GetRuleByName(ruleName);

							}
							if (rule == null)
							{
								_logger.Error(_className, methodName,
									string.Format("No rule configured and no default rule to invoke for survey '{0}' ({1}) and termination type '{2}'",
									_survey.Name, _survey.ID, Enum.GetName(typeof(StateModelTerminationType), _terminationType)));
								return;
							}
						}
						else
						{
							_logger.Debug(_className, methodName,
								string.Format("No rule to invoke for survey '{0}' ({1}) and termination type '{2}'",
								_survey.Name, _survey.ID, Enum.GetName(typeof(StateModelTerminationType), _terminationType)));
							return;
						}
					}
					else
					{
						// Is there a rule with the given ID?
						rule = loyalty.GetRuleById(ruleId);
						if (rule == null)
						{
							_logger.Error(_className, methodName,
								string.Format("Invalid rule ID {0} for survey '{1}' ({2}) and termination type '{3}'",
								ruleId, _survey.Name, _survey.ID, Enum.GetName(typeof(StateModelTerminationType), _terminationType)));
							return;
						}
					}

					// Add notation to rule
					if (rule.Rule != null && rule.Rule.GetType().GetProperty("DescriptionExpression") != null)
					{
						string tmp = string.Format("'Points awarded for taking survey {0}.  Termination type is {1}'",
							_survey.Name.Replace("'", string.Empty), Enum.GetName(typeof(StateModelTerminationType), _terminationType));

						_logger.Debug(_className, methodName, string.Format("Setting rule.Rule.DescriptionExpression = {0}", tmp));

						rule.Rule.GetType().GetProperty("DescriptionExpression").SetValue(rule.Rule, tmp, null);
					}
					else if (rule.Rule == null)
					{
						_logger.Debug(_className, methodName, "Rule not defined for survey completion/skip");
					}
					else
					{
						_logger.Error(_className, methodName, string.Format("Rule '{0}' DescriptionExpression property not defined for survey completion/skip", rule.RuleName));
					}

					// override the accrual points if appropriate
					if (isSurveyCompleted && _surveyCompletedAccrualPoints != null)
					{
						PropertyInfo info = null;
						try
						{
							info = rule.Rule.GetType().GetProperty("AccrualExpression", typeof(String));
						}
						catch
						{
							// whatever error we get here just means we can't override the rule AccrualExpression
						}
						if (info != null)
						{
							info.SetValue(rule.Rule, _surveyCompletedAccrualPoints.Value.ToString(), null);
							_logger.Trace(_className, methodName, string.Format("Setting AccrualExpression={0} for survey completed rule {1}", _surveyCompletedAccrualPoints.Value, rule.Rule.RuleName));
						}
						else
						{
							_logger.Warning(_className, methodName, string.Format("Survey completed rule {0} has no AccrualExpression property to override", rule.Rule.RuleName));
						}
					}

					// Execute the rule
					try
					{
						if (Member == null && _ipcode != -1 && loyalty != null)
						{
							Member = loyalty.LoadMemberFromIPCode(_ipcode);
						}
						ContextObject context = new ContextObject();
						context.Owner = Member;
						context.InvokingRow = null;

						if (OwnerType == null)
						{
							context.Environment["OwnerType"] = PointTransactionOwnerType.Survey;
							context.Environment["OwnerId"] = SurveyID;
							context.Environment["RowKey"] = RespondentID;
						}
						else
						{
							context.Environment["OwnerType"] = OwnerType.Value;
							context.Environment["OwnerId"] = OwnerId.Value;
							if (RowKey != null)
							{
								context.Environment["RowKey"] = RowKey.Value;
							}
						}
						loyalty.Execute(rule, context);

						long tmp = 0;

						_logger.Debug(_className, methodName,
							string.Format("Rule '{0}' ({1}) returns {2} for survey '{3}' ({4}) and termination type '{5}'",
							rule.RuleName, ruleId, tmp, _survey.Name, _survey.ID,
							Enum.GetName(typeof(StateModelTerminationType), _terminationType)));
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName,
							string.Format("Exception executing rule '{0}' ({1}) for survey '{2}' ({3}) and termination type '{4}': {5}",
							rule.RuleName, ruleId, _survey.Name, _survey.ID,
							Enum.GetName(typeof(StateModelTerminationType), _terminationType), ex.Message), ex);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName,
						string.Format("Unexpected exception executing rule for survey '{0}' ({1}) and termination type '{2}': {3}",
						_survey.Name, _survey.ID, Enum.GetName(typeof(StateModelTerminationType), _terminationType), ex.Message), ex);
			}
		}

		private void SendThankYouEmail()
		{
			const string methodName = "SendThankYouEmail";

			if (_ipcode > -1 && _survey.EmailID != -1)
			{
				try
				{
					using (var loyalty = new LoyaltyDataService(_config))
					{
						Member member = loyalty.LoadMemberFromIPCode(_ipcode);
						if (!string.IsNullOrEmpty(member.PrimaryEmailAddress))
						{
							//LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
							using (ITriggeredEmail email = TriggeredEmailFactory.Create(_survey.EmailID))
							{
								email.SendAsync(member).Wait();

								string msg = string.Format("Sent email '{0}' to member '{1}' at '{2}'",
									_survey.EmailID, _ipcode, member.PrimaryEmailAddress);
								_logger.Debug(_className, methodName, msg);
							}
						}
					}
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				}
			}
		}

		private void EvaluateDecisionState(ref bool exprResult, ref int index)
		{
			const string methodName = "EvaluateDecisionState";

			ContextObject contextObject = new ContextObject();
			var environment = new Dictionary<string, object>();
			environment.Add("surveyID", _currentState.SurveyID.ToString());
			environment.Add("respondentID", _respondent.ID.ToString());
			environment.Add("languageID", _language.ID.ToString());
			contextObject.Environment = environment;

			exprResult = false;
			index = -1;
			SMDecision decision = _surveyManager.RetrieveDecisionByStateID(_currentState.ID);
			if (decision != null)
			{
				long numConditions = decision.NumConditions();
				for (int i = 0; i < numConditions; i++)
				{
					string condition = decision.GetCondition(i);
					_logger.Debug(_className, methodName, "Evaluating condition: " + condition);

					try
					{
						Expression expression = new ExpressionFactory().Create(condition);
						object obj = expression.evaluate(contextObject);
						if (obj != null)
						{
							if (obj is bool)
							{
								if ((bool)obj)
								{
									_logger.Debug(_className, methodName, "Condition is true");
									exprResult = true;
									index = i;
									break;
								}
							}
							else if (obj.ToString().ToLower().Equals("true"))
							{
								_logger.Debug(_className, methodName, "Condition is true");
								exprResult = true;
								index = i;
								break;
							}
							_logger.Debug(_className, methodName, "Condition is false");
						}
						else
						{
							_logger.Error(_className, methodName, "Condition evaluates to null: " + condition);
						}
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, string.Format("Error evaluating condition '{0}': {1}", condition, ex.Message), ex);
					}
				}
			}
			else
			{
				_logger.Error(_className, methodName, string.Format("There are no conditions defined for decision state '{0}' ({1}), returning false", _currentState.UIName, _currentState.ID));
			}
		}

		#endregion

		
		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_surveyManager.Dispose();
			}
			_disposed = true;
		}

		~SMStateModel()
		{
			Dispose(false);
		}
	}
}
