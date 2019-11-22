using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Surveys
{
    public class MGSurveyManager
    {
        #region Fields
        private const string _className = "MGSurveyManager";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        #region Private Helpers

        private static MGSurveyMessage GetMessage(SMMessage message)
        {
            MGSurveyMessage smsg = new MGSurveyMessage
            {
                Id = message.ID,
                Content = message.Content,
                TypeIdentifier = MGSurveyComponentType.Message,
                StateId = message.StateID
            };
            return smsg;
        }

        private static MGSurveyMessage GetMessage(SMStateModel stateModel)
        {
            SMMessage message = stateModel.GetCurrentMessage();
            return GetMessage(message);            
        }

        private static MGSurveySimpleQuestion GetSimpleQuestion(SMQuestion question, SMStateModel stateModel)
        {
            MGSurveySimpleQuestion sq = new MGSurveySimpleQuestion()
            {
                Id = question.ID,
                Question = stateModel.EvaluateContent(stateModel.GetSimpleQuestionContent(question), true, null),
                AnswerType = question.AnswerControlType,
                AnswerOrientation = question.AnswerOrientation,
                IsResponseOptional = question.ResponseOptional,
                MinValue = question.ResponseMinVal,
                MaxValue = question.ResponseMaxVal,
                StateId = question.StateID,
            };
            if (question.HasOtherSpecify)
            {
            }
            List<SMAnswerContent> answers = stateModel.GetAnswerContent(question);
            if (answers != null && answers.Count > 0)
            {
                sq.AnswerChoices = new List<MGSurveySimpleQuestionValidAnswerChoice>();
                foreach (SMAnswerContent answer in answers)
                {
                    MGSurveySimpleQuestionValidAnswerChoice choice = new MGSurveySimpleQuestionValidAnswerChoice()
                    {
                        Id = answer.ID,
                        Answer = answer.Content
                    };
                    sq.AnswerChoices.Add(choice);
                }
            }
            return sq;
        }

        private static MGSurveySimpleQuestion GetSimpleQuestion(SMStateModel stateModel)
        {
            SMQuestion question = stateModel.GetCurrentQuestion();
            if (question != null)
            {
                return GetSimpleQuestion(question, stateModel);                
            }
            else
            {
                return null;
            }
        }

        private static MGSurveyMatrixQuestion GetMatrixQuestion(SMQuestion matrix, SMStateModel stateModel)
        {
            MGSurveyMatrixQuestion mq = new MGSurveyMatrixQuestion()
            {
                Id = matrix.ID,
                PointScale = matrix.MatrixPointScale,
                Header = stateModel.EvaluateContent(stateModel.GetMatrixQuestionHeader(matrix), true, null),
                StateId = matrix.StateID,
            };
            List<SMQuestionContent> questionContents = stateModel.GetMatrixQuestionContent(matrix);
            List<SMResponse> responses = stateModel.GetResponses(matrix);
            List<SMQuestionContent> anchorTexts = stateModel.GetMatrixQuestionAnchors(matrix);
            if (questionContents != null && questionContents.Count > 0)
            {
                mq.Rows = new List<MGSurveyMatrixQuestionRow>();
                mq.Columns = new List<MGSurveyMatrixQuestionColumn>();
                for (int rowIndex = 0; rowIndex < questionContents.Count; rowIndex++)
                {
                    SMQuestionContent q = questionContents[rowIndex];
                    MGSurveyMatrixQuestionRow row = new MGSurveyMatrixQuestionRow()
                    {
                        Id = q.ID,
                        Question = q.Content,
                        RowCells = new List<MGSurveyMatrixQuestionCell>()
                    };
                    mq.Rows.Add(row);
                    if (responses != null && responses.Count > 0)
                    {
                        SMResponse response = responses[rowIndex];
                        if (response != null)
                        {
                            MGSurveyMatrixQuestionCell cell = new MGSurveyMatrixQuestionCell()
                            {
                                Id = response.ID,
                                Answer = response.Content,
                            };
                            row.RowCells.Add(cell);
                        }
                    }
                }
            }
            if (anchorTexts != null && anchorTexts.Count > 0)
            {
                int colIndex = 0;
                foreach (SMQuestionContent anchor in anchorTexts)
                {
                    MGSurveyMatrixQuestionColumn column = new MGSurveyMatrixQuestionColumn()
                    {
                        AnchorText = anchor.Content,
                        ColumnIndex = colIndex++,
                        HasColumnSummary = false
                    };
                    mq.Columns.Add(column);
                }
            }
            return mq;
        }

        private static MGSurveyMatrixQuestion GetMatrixQuestion(SMStateModel stateModel)
        {
            SMQuestion matrix = stateModel.GetCurrentQuestion();
            if (matrix != null)
            {
                return GetMatrixQuestion(matrix, stateModel);                
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Public Methods
        public static MGSurvey GetSurvey(string culture, string surveyName, Member member)
        {
            string methodName = "GetSurveys";

            _logger.Debug(_className, methodName, "Getting surveys  " + surveyName + ".");

			using (SurveyManager surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMCultureMap cm = surveyManager.RetrieveCultureMap(culture);
				SMLanguage smLanguage = surveyManager.RetrieveLanguage(cm.LanguageID);

				MGSurvey mgSurvey = null;
				SMSurvey survey = surveyManager.RetrieveSurvey(surveyName);
				if (survey != null &&
					survey.EffectiveDate <= DateTime.Now &&
					survey.ExpirationDate > DateTime.Now &&
					(survey.GetAbsoluteQuota() == -1 || !surveyManager.IsQuotaMet(survey.ID, survey.GetAbsoluteQuota()))
					)
				{
					mgSurvey = new MGSurvey()
					{
						Id = survey.ID,
						Name = survey.Name,
						Type = survey.SurveyType.ToString(),
						Completed = surveyManager.IsSurveyCompleted(survey, smLanguage, member)
					};
				}

				return mgSurvey;
			}
        }

        public static List<MGSurvey> GetSurveys(string culture, SurveyType type, Member member)
        {
            string methodName = "GetSurveys";

            _logger.Debug(_className, methodName, "Getting all surveys of type " + type.ToString());

			using (SurveyManager surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMCultureMap cm = surveyManager.RetrieveCultureMap(culture);
				SMLanguage smLanguage = surveyManager.RetrieveLanguage(cm.LanguageID);
				List<SMSurvey> surveys = null;
				List<MGSurvey> results = new List<MGSurvey>();
				if (type == SurveyType.General)
				{
					surveys = surveyManager.RetrieveNonProfileSurveys();
				}
				else
				{
					surveys = surveyManager.RetrieveProfileSurveys();
				}
				if (surveys != null)
				{
					_logger.Debug(_className, methodName, "Found " + surveys.Count + " surveys of type " + type.ToString() + ".");
					foreach (SMSurvey survey in surveys)
					{
						if (survey.EffectiveDate > DateTime.Now) continue;
						if (survey.ExpirationDate <= DateTime.Now) continue;
						if (survey.GetAbsoluteQuota() >= 0 && surveyManager.IsQuotaMet(survey.ID, survey.GetAbsoluteQuota())) continue;
						MGSurvey mgs = new MGSurvey()
						{
							Id = survey.ID,
							Name = survey.Name,
							Type = type.ToString(),
							Completed = surveyManager.IsSurveyCompleted(survey, smLanguage, member)
						};
						results.Add(mgs);
					}
				}
				else
				{
					_logger.Debug(_className, methodName, "No surveys of type " + type.ToString() + " found.");
				}
				return results;
			}
        }
        
        public static MGSurveyState GetNextSurveyState(Member member, string culture, long surveyID, long stateID)
        {
            string methodName = "GetNextSurveyState";

            _logger.Debug(_className, methodName, string.Format("Getting next state for survey with id {0}.  Previous state is {1}.", surveyID, stateID));

            MGSurveyState result = new MGSurveyState();
			using (SurveyManager surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMCultureMap cm = surveyManager.RetrieveCultureMap(culture);
				SMLanguage smLanguage = surveyManager.RetrieveLanguage(cm.LanguageID);
				if (surveyManager != null)
				{
                    using (SMStateModel stateModel = new SMStateModel(LWDataServiceUtil.GetServiceConfiguration()))
                    {
                        stateModel.SurveyID = surveyID;
                        stateModel.StateID = stateID;
                        stateModel.SurveySelectionMethod = SurveySelectionMethod.SurveyID;
                        stateModel.IPCode = member.IpCode;
                        stateModel.MTouch = string.Empty;
                        stateModel.BScript = string.Empty;
                        stateModel.Language = smLanguage;
                        stateModel.Cultures = new string[] { cm.Culture };
                        stateModel.Member = member;

                        if (!stateModel.Initialize())
                        {
                            // No applicable survey is available
                            result.StateModelStatus = stateModel.Status;
                            result.Id = stateModel.StateID;
                            result.TerminationType = stateModel.TerminationType;
                        }
                        else
                        {
                            if (stateID != -1) stateModel.GetNextRestingState();
                            result.TerminationType = stateModel.TerminationType;

                            // Only visible if a resting state is available
                            SMState currentRestingState = stateModel.GetCurrentRestingState();
                            if (currentRestingState != null)
                            {
                                result.Id = currentRestingState.ID;

                                // Render the resting state
                                switch (currentRestingState.StateType)
                                {
                                    case StateType.Question:
                                        MGSurveySimpleQuestion sq = GetSimpleQuestion(stateModel);
                                        if (sq != null)
                                        {
                                            result.StateModelStatus = StateModelStatus.OnRestingState;
                                            result.StateType = currentRestingState.StateType;
                                            result.Component = sq;
                                        }
                                        else
                                        {
                                            result.StateModelStatus = StateModelStatus.Completed;
                                        }
                                        break;
                                    case StateType.MatrixQuestion:
                                        MGSurveyMatrixQuestion mq = GetMatrixQuestion(stateModel);
                                        if (mq != null)
                                        {
                                            result.StateModelStatus = StateModelStatus.OnRestingState;
                                            result.StateType = currentRestingState.StateType;
                                            result.Component = mq;
                                        }
                                        else
                                        {
                                            result.StateModelStatus = StateModelStatus.Completed;
                                        }
                                        break;
                                    case StateType.Message:
                                        MGSurveyMessage smsg = GetMessage(stateModel);
                                        result.StateModelStatus = StateModelStatus.OnRestingState;
                                        result.StateType = currentRestingState.StateType;
                                        result.Component = smsg;
                                        break;
                                    case StateType.Page:
                                        long pageStateId = stateModel.CurPage;
                                        SMState pageState = stateModel.GetCurrentRestingState();
                                        result.StateModelStatus = StateModelStatus.OnRestingState;
                                        result.StateType = pageState.StateType;
                                        List<SMState> states = stateModel.GetPageStates(pageState);
                                        MGSurveyPage page = new MGSurveyPage()
                                        {
                                            Id = pageState.ID,
                                            Components = new List<MGSurveyComponent>(),
                                            StateId = pageState.ID,
                                        };
                                        if (states != null && states.Count > 0)
                                        {
                                            foreach (SMState state in states)
                                            {
                                                switch (state.StateType)
                                                {
                                                    case StateType.Message:
                                                        SMMessage message = surveyManager.RetrieveMessageByStateID(state.ID);
                                                        MGSurveyMessage msg = GetMessage(message);
                                                        page.Components.Add(msg);
                                                        break;
                                                    case StateType.Question:
                                                        SMQuestion question1 = surveyManager.RetrieveQuestionByStateID(state.ID);
                                                        MGSurveySimpleQuestion simpleQuestion = GetSimpleQuestion(question1, stateModel);
                                                        page.Components.Add(simpleQuestion);
                                                        break;
                                                    case StateType.MatrixQuestion:
                                                        SMQuestion question2 = surveyManager.RetrieveQuestionByStateID(state.ID);
                                                        MGSurveyMatrixQuestion matrixQuestion = GetMatrixQuestion(question2, stateModel);
                                                        page.Components.Add(matrixQuestion);
                                                        break;
                                                }
                                            }
                                        }
                                        result.Component = page;
                                        break;
                                }
                            }
                        }
                    }
				}
				else
				{
					string errMsg = string.Format("Unable to instantiate survey manager.");
					_logger.Error(_className, methodName, errMsg);
					throw new LWOperationInvocationException(errMsg) { ErrorCode = 3500 };
				}
				return result;
			}
        }

        public static bool PostSurveyResponse(Member member, string culture, long surveyId, long stateId, MGSurveyResponse surveyResponse)
        {
            string methodName = "PostSurveyResponse";

            _logger.Trace(_className, methodName, string.Format("Posting response for survey with id {0}.", surveyId));

            bool result = false;

			using (SurveyManager surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMCultureMap cm = surveyManager.RetrieveCultureMap(culture);
				SMLanguage smLanguage = surveyManager.RetrieveLanguage(cm.LanguageID);
				if (surveyManager != null)
				{
                    using (SMStateModel stateModel = new SMStateModel(LWDataServiceUtil.GetServiceConfiguration()))
                    {
                        stateModel.SurveyID = surveyId;
                        stateModel.StateID = stateId;
                        stateModel.SurveySelectionMethod = SurveySelectionMethod.SurveyID;
                        stateModel.IPCode = member.IpCode;
                        stateModel.MTouch = string.Empty;
                        stateModel.BScript = string.Empty;
                        stateModel.Language = smLanguage;
                        stateModel.Cultures = new string[] { cm.Culture };
                        stateModel.Member = member;

                        if (stateModel.Initialize())
                        {
                            SMState currentState = stateModel.GetCurrentRestingState();
                            SMQuestion question = stateModel.GetCurrentQuestion();
                            switch (currentState.StateType)
                            {
                                case StateType.Question:
                                    MGSurveySimpleResponse simpleResponse = (MGSurveySimpleResponse)surveyResponse;
                                    result = stateModel.PostSimpleAnswers(question, simpleResponse.Answers, simpleResponse.Others, new List<string>());
                                    break;
                                case StateType.MatrixQuestion:
                                    MGSurveyMatrixResponse matrixResponse = (MGSurveyMatrixResponse)surveyResponse;
                                    result = stateModel.PostMatrixAnswers(question, matrixResponse.Answers, matrixResponse.RadioAnswers);
                                    break;
                                default:
                                    string errMsg = string.Format("Unknown state encountered for survey statemodel for survey id {0} - state id {1}.", surveyId, stateId);
                                    _logger.Error(_className, methodName, errMsg);
                                    throw new LWOperationInvocationException(errMsg) { ErrorCode = 3502 };
                            }
                        }
                        else
                        {
                            string errMsg = string.Format("Unable to instantiate survey statemodel for survey id {0} - state id {1}.", surveyId, stateId);
                            _logger.Error(_className, methodName, errMsg);
                            throw new LWOperationInvocationException(errMsg) { ErrorCode = 3501 };
                        }
                    }
				}
				else
				{
					string errMsg = string.Format("Unable to instantiate survey manager.");
					_logger.Error(_className, methodName, errMsg);
					throw new LWOperationInvocationException(errMsg) { ErrorCode = 3500 };
				}
				return result;
			}
        }

        #endregion
    }
}