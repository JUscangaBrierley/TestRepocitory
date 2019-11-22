using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.DataAccess;
using PetaPoco;

namespace Brierley.FrameWork.Data
{
	public class SurveyManager : ServiceBase
	{
		private const string _className = "SurveyManager";
		private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
		private static Dictionary<string, SurveyManager> _instanceCache = new Dictionary<string, SurveyManager>();
		private static object _syncRoot = new object();
		private static Random random = new Random((int)DateTime.Now.Ticks);

		private static object _appCacheLock = new object();
		private bool _useAppCache = false;
		private List<SMLanguage> _languages = null;
		private List<SMCultureMap> _cultureMaps = null;

        private SMRespListStageDao _smRespListStageDao;
        private SMRespondentListDao _smRespondentListDao;
        private SMFieldListDao _smFieldListDao;
        private SMLanguageDao _smLanguageDao;
        private SMCultureMapDao _smCultureMapDao;
        private SMConceptViewDao _smConceptViewDao;
        private SMConceptDao _smConceptDao;
        private SMMessageDao _smMessageDao;
        private SMDecisionDao _smDecisionDao;
        private SMResponseDao _smResponseDao;
        private SMRespondentDao _smRespondentDao;
        private SMMatrixAnswerDao _smMatrixAnswerDao;
        private SMAnswerContentDao _smAnswerContentDao;
        private SMQuestionContentDao _smQuestionContentDao;
        private SMQuestionDao _smQuestionDao;
        private SMTransitionDao _smTransitionDao;
        private SMStateDao _smStateDao;
        private SMSurveyDao _smSurveyDao;

        public SMRespListStageDao SMRespListStageDao
        {
            get
            {
                if (_smRespListStageDao == null)
                {
                    _smRespListStageDao = new SMRespListStageDao(Database, Config);
                }
                return _smRespListStageDao;
            }
        }

        public SMRespondentListDao SMRespondentListDao
        {
            get
            {
                if (_smRespondentListDao == null)
                {
                    _smRespondentListDao = new SMRespondentListDao(Database, Config);
                }
                return _smRespondentListDao;
            }
        }

        public SMFieldListDao SMFieldListDao
        {
            get
            {
                if (_smFieldListDao == null)
                {
                    _smFieldListDao = new SMFieldListDao(Database, Config);
                }
                return _smFieldListDao;
            }
        }

        public SMLanguageDao SMLanguageDao
        {
            get
            {
                if (_smLanguageDao == null)
                {
                    _smLanguageDao = new SMLanguageDao(Database, Config);
                }
                return _smLanguageDao;
            }
        }

        public SMCultureMapDao SMCultureMapDao
        {
            get
            {
                if (_smCultureMapDao == null)
                {
                    _smCultureMapDao = new SMCultureMapDao(Database, Config);
                }
                return _smCultureMapDao;
            }
        }

        public SMConceptViewDao SMConceptViewDao
        {
            get
            {
                if (_smConceptViewDao == null)
                {
                    _smConceptViewDao = new SMConceptViewDao(Database, Config);
                }
                return _smConceptViewDao;
            }
        }

        public SMConceptDao SMConceptDao
        {
            get
            {
                if (_smConceptDao == null)
                {
                    _smConceptDao = new SMConceptDao(Database, Config);
                }
                return _smConceptDao;
            }
        }

        public SMMessageDao SMMessageDao
        {
            get
            {
                if (_smMessageDao == null)
                {
                    _smMessageDao = new SMMessageDao(Database, Config);
                }
                return _smMessageDao;
            }
        }

        public SMDecisionDao SMDecisionDao
        {
            get
            {
                if (_smDecisionDao == null)
                {
                    _smDecisionDao = new SMDecisionDao(Database, Config);
                }
                return _smDecisionDao;
            }
        }

        public SMResponseDao SMResponseDao
        {
            get
            {
                if (_smResponseDao == null)
                {
                    _smResponseDao = new SMResponseDao(Database, Config);
                }
                return _smResponseDao;
            }
        }

        public SMRespondentDao SMRespondentDao
        {
            get
            {
                if (_smRespondentDao == null)
                {
                    _smRespondentDao = new SMRespondentDao(Database, Config);
                }
                return _smRespondentDao;
            }
        }

        public SMMatrixAnswerDao SMMatrixAnswerDao
        {
            get
            {
                if (_smMatrixAnswerDao == null)
                {
                    _smMatrixAnswerDao = new SMMatrixAnswerDao(Database, Config);
                }
                return _smMatrixAnswerDao;
            }
        }

        public SMAnswerContentDao SMAnswerContentDao
        {
            get
            {
                if (_smAnswerContentDao == null)
                {
                    _smAnswerContentDao = new SMAnswerContentDao(Database, Config);
                }
                return _smAnswerContentDao;
            }
        }

        public SMQuestionContentDao SMQuestionContentDao
        {
            get
            {
                if (_smQuestionContentDao == null)
                {
                    _smQuestionContentDao = new SMQuestionContentDao(Database, Config);
                }
                return _smQuestionContentDao;
            }
        }

        public SMQuestionDao SMQuestionDao
        {
            get
            {
                if (_smQuestionDao == null)
                {
                    _smQuestionDao = new SMQuestionDao(Database, Config);
                }
                return _smQuestionDao;
            }
        }

        public SMTransitionDao SMTransitionDao
        {
            get
            {
                if (_smTransitionDao == null)
                {
                    _smTransitionDao = new SMTransitionDao(Database, Config);
                }
                return _smTransitionDao;
            }
        }

        public SMStateDao SMStateDao
        {
            get
            {
                if (_smStateDao == null)
                {
                    _smStateDao = new SMStateDao(Database, Config);
                }
                return _smStateDao;
            }
        }

        public SMSurveyDao SMSurveyDao
        {
            get
            {
                if (_smSurveyDao == null)
                {
                    _smSurveyDao = new SMSurveyDao(Database, Config);
                }
                return _smSurveyDao;
            }
        }


		public SurveyManager(ServiceConfig config)
			: base(config)
		{
		}

		public ITransaction StartTransaction(int timeout)
		{
			ITransaction txn = Database.GetTransaction();
			return txn;
		}

		private static string GetInstanceCacheKey(string orgName, string envName)
		{
			return DataServiceUtil.GetKey(orgName, envName);
		}

		private static void DeleteResourceInfo(DataServiceUtil.ResourceInfo rinfo)
		{
			if (rinfo != null && !string.IsNullOrEmpty(rinfo.Path))
			{
				File.Delete(rinfo.Path);
			}
		}

		public void LoadAppCache(bool force)
		{
            lock (_appCacheLock)
            {
                _useAppCache = true;
                if (force || _cultureMaps == null)
                {
                    _cultureMaps = SMCultureMapDao.RetrieveAll();
                }
            }
		}

		public void SetForceQueryCacheRefresh(bool value)
		{
            SMAnswerContentDao.SetForceQueryCacheReset(value);
            SMCultureMapDao.SetForceQueryCacheReset(value);
            SMLanguageDao.SetForceQueryCacheReset(value);
            SMQuestionContentDao.SetForceQueryCacheReset(value);
            SMRespondentDao.SetForceQueryCacheReset(value);
            SMTransitionDao.SetForceQueryCacheReset(value);
            if (value)
            {
                LoadAppCache(true);
            }
		}

		#region SMSurvey
		/// <summary>
		/// Create a survey.
		/// </summary>
		/// <param name="survey">survey to create</param>
		/// <returns>created survey</returns>
		public void CreateSurvey(SMSurvey survey)
		{
            SMSurveyDao.Create(survey);
            CacheManager.Update(CacheRegions.SurveyById, survey.ID, survey);
            CacheManager.Update(CacheRegions.SurveyByName, survey.Name, survey);
		}

		/// <summary>
		/// Create a survey.
		/// </summary>
		/// <param name="name">name of the survey</param>
		/// <param name="description">description of the survey</param>
		/// <returns>created survey</returns>
		public void CreateSurvey(string name, string description)
		{
            SMSurvey result = new SMSurvey();
            result.Name = name;
            result.Description = description;
            CreateSurvey(result);
		}

		/// <summary>
		/// Create a clone of a survey
		/// </summary>
		/// <param name="oldSurveyID">unique ID for the source survey</param>
		/// <param name="newSurveyName">unique name for the cloned survey</param>
		/// <param name="resetDocID">don't use same document as parent</param>
		/// <returns>cloned survey</returns>
		public SMSurvey CloneSurvey(long oldSurveyID, string newSurveyName, bool resetDocID)
		{
            const string methodName = "CloneSurvey";

            SMSurvey oldSurvey = RetrieveSurvey(oldSurveyID);
            if (oldSurvey == null)
            {
                _logger.Error(_className, methodName, "Invalid survey ID: " + oldSurveyID);
                throw new ArgumentException("Invalid survey ID: " + oldSurveyID);
            }
            if (RetrieveSurvey(newSurveyName) != null)
            {
                _logger.Error(_className, methodName, "Survey name already exists: " + newSurveyName);
                throw new ArgumentException("Survey name already exists: " + newSurveyName);
            }

            // survey
            var newSurvey = new SMSurvey(oldSurvey) { Name = newSurveyName };
            if (resetDocID) newSurvey.DocumentID = -1;
            CreateSurvey(newSurvey);

            // concepts
            RetrieveLanguages();
            foreach (var language in _languages)
            {
                var oldConcepts = RetrieveConcepts(oldSurveyID, language.ID);
                if (oldConcepts != null && oldConcepts.Count > 0)
                {
                    foreach (var oldConcept in oldConcepts)
                    {
                        var newConcept = new SMConcept(oldConcept) { SurveyID = newSurvey.ID };
                        CreateConcept(newConcept);
                    }
                }
            }

            // states
            List<SMState> oldStates = RetrieveStatesBySurveyID(oldSurveyID);
            if (oldStates != null && oldStates.Count > 0)
            {
                // create states and mapping of state IDs
                Dictionary<long, SMState> oldStateID2NewState = new Dictionary<long, SMState>();
                foreach (var oldState in oldStates)
                {
                    var newState = new SMState(oldState) { SurveyID = newSurvey.ID, Page = 0 };
                    CreateState(newState);
                    oldStateID2NewState.Add(oldState.ID, newState);
                }

                // update the pages for the new states based on the mapping
                foreach (var oldState in oldStates)
                {
                    if (oldState.Page != 0 && oldStateID2NewState[oldState.ID].Page == 0)
                    {
                        oldStateID2NewState[oldState.ID].Page = oldStateID2NewState[oldState.Page].ID;
                        UpdateState(oldStateID2NewState[oldState.ID]);
                    }
                }

                // now update the transitions, etc that depend on the stateID
                List<string> newTransitions = new List<string>();
                foreach (var oldState in oldStates)
                {
                    List<SMTransition> oldInputs = RetrieveInputTransitions(oldState.ID);
                    if (oldInputs != null && oldInputs.Count > 0)
                    {
                        foreach (var oldInput in oldInputs)
                        {
                            var newInput = new SMTransition()
                            {
                                SrcStateID = oldStateID2NewState[oldInput.SrcStateID].ID,
                                SrcConnectorIndex = oldInput.SrcConnectorIndex,
                                DstStateID = oldStateID2NewState[oldInput.DstStateID].ID,
                                DstConnectorIndex = oldInput.DstConnectorIndex,
                                Page = (oldInput.Page != 0 ? oldStateID2NewState[oldInput.Page].ID : 0)
                            };
                            string key = newInput.SrcStateID + "_" + newInput.SrcConnectorIndex + "_" + newInput.DstStateID + "_" + newInput.DstConnectorIndex + "_" + newInput.Page;
                            if (!newTransitions.Contains(key))
                            {
                                newTransitions.Add(key);
                                CreateTransition(newInput);
                            }
                        }
                    }

                    List<SMTransition> oldOutputs = RetrieveOutputTransitions(oldState.ID);
                    if (oldOutputs != null && oldOutputs.Count > 0)
                    {
                        foreach (var oldOutput in oldOutputs)
                        {
                            var newOutput = new SMTransition()
                            {
                                SrcStateID = oldStateID2NewState[oldOutput.SrcStateID].ID,
                                SrcConnectorIndex = oldOutput.SrcConnectorIndex,
                                DstStateID = oldStateID2NewState[oldOutput.DstStateID].ID,
                                DstConnectorIndex = oldOutput.DstConnectorIndex,
                                Page = (oldOutput.Page != 0 ? oldStateID2NewState[oldOutput.Page].ID : 0)
                            };
                            string key = newOutput.SrcStateID + "_" + newOutput.SrcConnectorIndex + "_" + newOutput.DstStateID + "_" + newOutput.DstConnectorIndex + "_" + newOutput.Page;
                            if (!newTransitions.Contains(key))
                            {
                                newTransitions.Add(key);
                                CreateTransition(newOutput);
                            }
                        }
                    }

                    // Content based on state type
                    long newStateID = oldStateID2NewState[oldState.ID].ID;
                    switch (oldState.StateType)
                    {
                        case StateType.Message:
                            {
                                SMMessage oldMessage = RetrieveMessageByStateID(oldState.ID);
                                if (oldMessage != null)
                                {
                                    SMMessage newMessage = new SMMessage() { StateID = newStateID, Content = StringUtils.FriendlyString(oldMessage.Content) };
                                    CreateMessage(newMessage);
                                }
                            }
                            break;

                        case StateType.Decision:
                            {
                                SMDecision oldDecision = RetrieveDecisionByStateID(oldState.ID);
                                if (oldDecision != null)
                                {
                                    SMDecision newDecision = new SMDecision() { StateID = newStateID, Expression = oldDecision.Expression };
                                    CreateDecision(newDecision);
                                }
                            }
                            break;

                        case StateType.Question:
                        case StateType.MatrixQuestion:
                            {
                                SMQuestion oldQuestion = RetrieveQuestionByStateID(oldState.ID);
                                if (oldQuestion != null)
                                {
                                    SMQuestion newQuestion = new SMQuestion(oldQuestion) { StateID = newStateID };
                                    CreateQuestion(newQuestion);

                                    foreach (var language in _languages)
                                    {
                                        var oldQuestionContents = RetrieveQuestionContents(oldQuestion.ID, language.ID, QuestionContentType.ANCHOR_TEXT);
                                        oldQuestionContents.AddRange(RetrieveQuestionContents(oldQuestion.ID, language.ID, QuestionContentType.BODY_TEXT));
                                        oldQuestionContents.AddRange(RetrieveQuestionContents(oldQuestion.ID, language.ID, QuestionContentType.HEADER_TEXT));
                                        oldQuestionContents.AddRange(RetrieveQuestionContents(oldQuestion.ID, language.ID, QuestionContentType.OTHER_SPECIFY_TEXT));
                                        if (oldQuestionContents != null && oldQuestionContents.Count > 0)
                                        {
                                            foreach (var oldQuestionContent in oldQuestionContents)
                                            {
                                                var newQuestionContent = new SMQuestionContent(oldQuestionContent) { QuestionID = newQuestion.ID };
                                                CreateQuestionContent(newQuestionContent);

                                                if (oldState.StateType == StateType.MatrixQuestion)
                                                {
                                                    var oldMatrixAnswers = RetrieveMatrixAnswerByQuestionContentID(oldQuestionContent.ID);
                                                    if (oldMatrixAnswers != null && oldMatrixAnswers.Count > 0)
                                                    {
                                                        foreach (var oldMatrixAnswer in oldMatrixAnswers)
                                                        {
                                                            var newMatrixAnswer = new SMMatrixAnswer(oldMatrixAnswer) { QuestionContentID = newQuestionContent.ID };
                                                            CreateMatrixAnswer(newMatrixAnswer);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (oldState.StateType == StateType.Question)
                                        {
                                            // Answer bodys
                                            var oldAnswerContents = RetrieveAnswerContents(oldQuestion.ID, language.ID);
                                            if (oldAnswerContents != null && oldAnswerContents.Count > 0)
                                            {
                                                foreach (var oldAnswerContent in oldAnswerContents)
                                                {
                                                    SMAnswerContent newAnswerContent = new SMAnswerContent(oldAnswerContent) { QuestionID = newQuestion.ID };
                                                    CreateAnswerContent(newAnswerContent);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                        case StateType.Start:
                        case StateType.PageStart:
                        case StateType.PageEnd:
                        case StateType.Page:
                        case StateType.Terminate:
                        case StateType.Skip:
                            break;
                    }
                }
            }

            return newSurvey;
		}

		/// <summary>
		/// Update a survey.
		/// </summary>
		/// <param name="survey">survey to update</param>
		/// <returns>updated survey</returns>
		public void UpdateSurvey(SMSurvey survey)
		{
            SMSurveyDao.Update(survey);
            CacheManager.Update(CacheRegions.SurveyById, survey.ID, survey);
            CacheManager.Update(CacheRegions.SurveyByName, survey.Name, survey);
		}

		/// <summary>
		/// Retrieve a survey.
		/// </summary>
		/// <param name="ID">unique identifier for the survey</param>
		/// <returns>specified survey, or null if it doesn't exist</returns>
		public SMSurvey RetrieveSurvey(long ID)
		{
            SMSurvey result = (SMSurvey)CacheManager.Get(CacheRegions.SurveyById, ID);
            if (result == null)
            {
                result = SMSurveyDao.Retrieve(ID);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.SurveyById, result.ID, result);
                    CacheManager.Update(CacheRegions.SurveyByName, result.Name, result);
                }
            }
            return result;
		}

		/// <summary>
		/// Retrieve a survey.
		/// </summary>
		/// <param name="name">unique survey name</param>
		/// <returns>specified survey, or null if it doesn't exist</returns>
		public SMSurvey RetrieveSurvey(string name)
		{
            SMSurvey result = (SMSurvey)CacheManager.Get(CacheRegions.SurveyByName, name);
            if (result == null)
            {
                result = SMSurveyDao.Retrieve(name);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.SurveyById, result.ID, result);
                    CacheManager.Update(CacheRegions.SurveyByName, result.Name, result);
                }
            }
            return result;
		}

		/// <summary>
		/// Retrieve a list of all surveys.
		/// </summary>
		/// <returns>list of surveys</returns>
		public List<SMSurvey> RetrieveSurveys()
		{
            return SMSurveyDao.RetrieveAll() ?? new List<SMSurvey>();
		}

		/// <summary>
		/// Retrieve a list of all surveys.
		/// </summary>
		/// <param name="statusFilter">filter by SurveyStatus enum</param>
		/// <param name="typeFilter">filter by SurveyType enum</param>
		/// <returns>list of surveys</returns>
		public List<SMSurvey> RetrieveSurveys(int statusFilter, int typeFilter, long? folderId = null)
		{
			return SMSurveyDao.RetrieveAll(statusFilter, typeFilter, folderId) ?? new List<SMSurvey>();
		}

		public List<long> RetrieveSurveyIds(string search, DateTime? startDate, DateTime? endDate, SurveyType? type, SurveyAudience? audience, SurveyStatus? status)
		{
			return SMSurveyDao.Retrieve(search, startDate, endDate, type, audience, status) ?? new List<long>();
		}

		/// <summary>
		/// Retrieve a list of all profile surveys.
		/// </summary>
		/// <returns>list of profile surveys</returns>
		public List<SMSurvey> RetrieveProfileSurveys()
		{
			return SMSurveyDao.RetrieveAllProfileSurveys() ?? new List<SMSurvey>();
		}

		/// <summary>
		/// Retrieve a list of all non-profile surveys.
		/// </summary>
		/// <returns>list of non-profile surveys</returns>
		public List<SMSurvey> RetrieveNonProfileSurveys()
		{
			return SMSurveyDao.RetrieveAllNonProfileSurveys() ?? new List<SMSurvey>();
		}

		/// <summary>
		/// Get the ID for the rule to use when the survey has been completed successfully.
		/// </summary>
		/// <param name="surveyId">ID of the survey</param>
		/// <returns>rule ID</returns>
		public long RetrieveSurveyCompleteRuleId(long surveyId)
		{
			SMSurvey survey = RetrieveSurvey(surveyId);
			return (survey != null ? survey.SurveyCompleteRuleId : -1);
		}

		/// <summary>
		/// Get the ID for the rule to use when the survey has been completed as a "terminate and tally".
		/// </summary>
		/// <param name="surveyId">ID of the survey</param>
		/// <returns>rule ID</returns>
		public long RetrieveSurveyTerminateAndTallyRuleId(long surveyId)
		{
			SMSurvey survey = RetrieveSurvey(surveyId);
			return (survey != null ? survey.SurveyTerminateAndTallyRuleId : -1);
		}

		/// <summary>
		/// Gets a list of surveys that have changed since the specified date
		/// </summary>
		/// <param name="changedSince"></param>
		/// <returns></returns>
		public List<SMSurvey> RetrieveChangedSurveys(DateTime changedSince)
		{
            List<SMSurvey> result = new List<SMSurvey>();
            var surveys = SMSurveyDao.RetrieveAll();
            foreach (var survey in surveys)
            {
                // SMSurvey changed
                if (survey.UpdateDate == null || DateTimeUtil.GreaterEqual((DateTime)survey.UpdateDate, changedSince))
                {
                    result.Add(survey);
                    continue;
                }

                // SMConcept changed
                bool foundConceptChange = false;
                foreach (var language in _languages)
                {
                    var concepts = RetrieveConcepts(survey.ID, language.ID);
                    if (concepts != null && concepts.Count > 0)
                    {
                        foreach (var concept in concepts)
                        {
                            if (DateTimeUtil.GreaterEqual((DateTime)concept.UpdateDate, changedSince))
                            {
                                result.Add(survey);
                                foundConceptChange = true;
                                break;
                            }
                        }
                        if (foundConceptChange) break;
                    }
                }
                if (foundConceptChange) continue;

                var states = RetrieveStatesBySurveyID(survey.ID);
                if (states != null && states.Count > 0)
                {
                    foreach (var state in states)
                    {
                        // SMState changed
                        if (DateTimeUtil.GreaterEqual((DateTime)state.UpdateDate, changedSince))
                        {
                            result.Add(survey);
                            break;
                        }

                        // SMTransition changed
                        bool foundTransitionChange = false;
                        var transitions = RetrieveInputTransitions(state.ID);
                        transitions.AddRange(RetrieveOutputTransitions(state.ID));
                        if (transitions != null && transitions.Count > 0)
                        {
                            foreach (var transition in transitions)
                            {
                                if (DateTimeUtil.GreaterEqual((DateTime)transition.UpdateDate, changedSince))
                                {
                                    result.Add(survey);
                                    foundTransitionChange = true;
                                    break;
                                }
                            }
                            if (foundTransitionChange) break;
                        }

                        bool foundStateTypeChange = false;
                        switch (state.StateType)
                        {
                            case StateType.Message:
                                {
                                    var message = RetrieveMessageByStateID(state.ID);
                                    if (message != null && DateTimeUtil.GreaterEqual((DateTime)message.UpdateDate, changedSince))
                                    {
                                        result.Add(survey);
                                        foundStateTypeChange = true;
                                    }
                                }
                                break;

                            case StateType.Decision:
                                {
                                    var decision = RetrieveDecisionByStateID(state.ID);
                                    if (decision != null && DateTimeUtil.GreaterEqual((DateTime)decision.UpdateDate, changedSince))
                                    {
                                        result.Add(survey);
                                        foundStateTypeChange = true;
                                    }
                                }
                                break;

                            case StateType.Question:
                            case StateType.MatrixQuestion:
                                {
                                    var question = RetrieveQuestionByStateID(state.ID);
                                    if (question != null)
                                    {
                                        foreach (var language in _languages)
                                        {
                                            var questionContents = RetrieveQuestionContents(question.ID, language.ID, QuestionContentType.ANCHOR_TEXT);
                                            questionContents.AddRange(RetrieveQuestionContents(question.ID, language.ID, QuestionContentType.BODY_TEXT));
                                            questionContents.AddRange(RetrieveQuestionContents(question.ID, language.ID, QuestionContentType.HEADER_TEXT));
                                            questionContents.AddRange(RetrieveQuestionContents(question.ID, language.ID, QuestionContentType.OTHER_SPECIFY_TEXT));
                                            if (questionContents != null && questionContents.Count > 0)
                                            {
                                                foreach (var questionContent in questionContents)
                                                {
                                                    if (DateTimeUtil.GreaterEqual((DateTime)questionContent.UpdateDate, changedSince))
                                                    {
                                                        result.Add(survey);
                                                        foundStateTypeChange = true;
                                                        break;
                                                    }

                                                    if (state.StateType == StateType.MatrixQuestion)
                                                    {
                                                        var matrixAnswers = RetrieveMatrixAnswerByQuestionContentID(questionContent.ID);
                                                        if (matrixAnswers != null && matrixAnswers.Count > 0)
                                                        {
                                                            foreach (var matrixAnswer in matrixAnswers)
                                                            {
                                                                if (DateTimeUtil.GreaterEqual((DateTime)matrixAnswer.UpdateDate, changedSince))
                                                                {
                                                                    result.Add(survey);
                                                                    foundStateTypeChange = true;
                                                                    break;
                                                                }
                                                            }
                                                            if (foundStateTypeChange) break;
                                                        }
                                                    }
                                                }
                                                if (foundStateTypeChange) break;
                                            }

                                            if (state.StateType == StateType.Question)
                                            {
                                                // Answer bodys
                                                var answerContents = RetrieveAnswerContents(question.ID, language.ID);
                                                if (answerContents != null && answerContents.Count > 0)
                                                {
                                                    foreach (var answerContent in answerContents)
                                                    {
                                                        if (DateTimeUtil.GreaterEqual((DateTime)answerContent.UpdateDate, changedSince))
                                                        {
                                                            result.Add(survey);
                                                            foundStateTypeChange = true;
                                                            break;
                                                        }
                                                    }
                                                    if (foundStateTypeChange) break;
                                                }
                                            }
                                        }
                                    }
                                }
                                break;

                            case StateType.Start:
                            case StateType.PageStart:
                            case StateType.PageEnd:
                            case StateType.Page:
                            case StateType.Terminate:
                            case StateType.Skip:
                                break;
                        }
                        if (foundStateTypeChange) break;
                    }
                }
            }
            return result;
		}

		/// <summary>
		/// Delete a survey
		/// </summary>
		/// <param name="ID">unique ID of the survey to delete</param>
		public void DeleteSurvey(long surveyID)
		{
            SMSurvey survey = (SMSurvey)CacheManager.Get(CacheRegions.SurveyById, surveyID);
            int rows = 0;

            long numConceptViews = SMConceptViewDao.RetrieveCountBySurvey(surveyID);
            long numResponses = SMResponseDao.RetrieveCountBySurvey(surveyID);
            long numRespondents = SMRespondentDao.NumRespondents(surveyID, -1);
            if (numConceptViews > 0 || numResponses > 0 || numRespondents > 0)
            {
                throw new LWException(string.Format("Survey contains existing data: {0} concept views, {1} responses, {2} respondents",
                    numConceptViews, numResponses, numRespondents));
            }

            // delete concepts
            rows += SMConceptDao.DeleteAllForSurvey(surveyID);

            // delete the state model
            rows += SMTransitionDao.DeleteAllForSurvey(surveyID);
            rows += SMMessageDao.DeleteAllForSurvey(surveyID);
            rows += SMDecisionDao.DeleteAllForSurvey(surveyID);
            rows += SMAnswerContentDao.DeleteAllForSurvey(surveyID);
            rows += SMMatrixAnswerDao.DeleteAllForSurvey(surveyID);
            rows += SMQuestionContentDao.DeleteAllForSurvey(surveyID);
            rows += SMQuestionDao.DeleteAllForSurvey(surveyID);
            rows += SMStateDao.DeleteAllForSurvey(surveyID);

            // now we can delete the survey
            SMSurveyDao.Delete(surveyID);

            CacheManager.Remove(CacheRegions.SurveyById, surveyID);
            if (survey != null) CacheManager.Remove(CacheRegions.SurveyByName, survey.Name);
		}

		/// <summary>
		/// Does a survey exist with the specified name?
		/// </summary>
		/// <param name="name">survey name</param>
		/// <returns>true of it already exists, false otherwise</returns>
		public bool SurveyExists(string name)
		{
            SMSurvey survey = (SMSurvey)CacheManager.Get(CacheRegions.SurveyByName, name);
            if (survey == null)
            {
                survey = SMSurveyDao.Retrieve(name);
            }
            bool result = (survey != null);
            return result;
		}

		/// <summary>
		/// Has the specified survey been completed by the specified member in the specified language?
		/// </summary>
		/// <param name="survey">survey</param>
		/// <param name="language">language</param>
		/// <param name="member">member</param>
		/// <returns>true if survey completed, false otherwise</returns>
		public bool IsSurveyCompleted(SMSurvey survey, SMLanguage language, Member member)
		{
            const string methodName = "IsSurveyCompleted";
            if (survey == null || survey.ID < 1) throw new ArgumentNullException("survey");
            if (language == null || language.ID < 0) throw new ArgumentNullException("language");
            if (member == null || member.IpCode < 1) throw new ArgumentNullException("member");
            bool result = false;

            SMRespondent respondent = SMRespondentDao.RetrieveByIPCode(survey.ID, language.ID, member.IpCode);
            if (respondent != null)
            {
                // existing respondent, see if completed
                if (respondent.Skipped || respondent.CompleteDate != null)
                {
                    result = true;
                }
            }
            else
            {
                // if survey allows auto creation of respondent, then create it since none exists
                bool createRespondent = survey.AutoCreateRespondents();
                if (createRespondent)
                {
                    SMRespondent newRespondent = new SMRespondent();
                    newRespondent.SurveyID = survey.ID;
                    newRespondent.LanguageID = language.ID;
                    newRespondent.MTouch = string.Empty;
                    newRespondent.IPCode = member.IpCode;
                    newRespondent.Skipped = false;
                    newRespondent.CreateDate = DateTime.Now;
                    newRespondent.UpdateDate = DateTime.Now;
                    CreateRespondent(newRespondent);
                }
                else
                {
                    _logger.Error(_className, methodName, string.Format(
                        "Unable to autocreate respondent for survey '{0}', language '{1}', ipcode {2} due to survey audience setting '{3}'",
                        survey.Name, language.Description, member.IpCode, survey.GetSurveyAudienceName()
                    ));
                }
            }
            return result;
		}
		#endregion

		#region SMState
		/// <summary>
		/// Create a state.
		/// </summary>
		/// <param name="state">the state to create</param>
		/// <returns>the new state</returns>
		public void CreateState(SMState state)
		{
            if (string.IsNullOrEmpty(state.UIDescription)) state.UIDescription = " ";
			SMStateDao.Create(state);
		}

		/// <summary>
		/// Clone a state within the same survey and page
		/// </summary>
		/// <param name="surveyID">unique ID for survey</param>
		/// <param name="stateIDs">list of state IDs</param>
		/// <param name="transitions">list of transitions</param>
		/// <param name="clonedStates">resulting cloned stated</param>
		/// <param name="clonedTransitions">resulting cloned transitions</param>
		public void CloneStates(long surveyID, List<long> stateIDs, List<SMTransition> transitions, ref List<SMState> clonedStates, ref List<SMTransition> clonedTransitions)
		{
            //const string methodName = "CloneStates";

            if (clonedStates == null) clonedStates = new List<SMState>();
            if (clonedTransitions == null) clonedTransitions = new List<SMTransition>();

            Dictionary<long, SMState> srcStateID2SrcState = GetStateMap(RetrieveStatesBySurveyID(surveyID));
            Dictionary<long, SMState> srcStateID2ClonedState = new Dictionary<long, SMState>();
            foreach (long stateID in stateIDs)
            {
                SMState srcState = srcStateID2SrcState[stateID];
                if (srcState != null)
                {
                    // clone the state
                    SMState clonedState = new SMState(srcState) { UIPositionX = srcState.UIPositionX + 350 };
                    if (srcState.Page != 0 && srcStateID2ClonedState.ContainsKey(srcState.Page))
                    {
                        // cloned state is on a cloned page, so map the pageID to the cloned one
                        clonedState.Page = srcStateID2ClonedState[srcState.Page].ID;
                    }
                    CreateState(clonedState);
                    srcStateID2ClonedState.Add(srcState.ID, clonedState);
                    clonedStates.Add(clonedState);
                }
            }

            // clone the specified transitions between cloned states
            List<string> clonedTransitionKeys = new List<string>();
            if (transitions != null && transitions.Count > 0)
            {
                foreach (SMTransition transition in transitions)
                {
                    if (!srcStateID2ClonedState.ContainsKey(transition.SrcStateID)) continue;
                    if (!srcStateID2ClonedState.ContainsKey(transition.DstStateID)) continue;

                    long srcStateID = srcStateID2ClonedState[transition.SrcStateID].ID;
                    long srcConnectorIndex = transition.SrcConnectorIndex;
                    long dstStateID = srcStateID2ClonedState[transition.DstStateID].ID;
                    long dstConnectorIndex = transition.DstConnectorIndex;
                    long page = (transition.Page != 0 ? srcStateID2ClonedState[transition.Page].ID : 0);
                    string key = string.Format("{0}_{1}_{2}_{3}_{4}", srcStateID, srcConnectorIndex, dstStateID, dstConnectorIndex, page);

                    if (!clonedTransitionKeys.Contains(key))
                    {
                        var clonedTransition = new SMTransition()
                        {
                            SrcStateID = srcStateID,
                            SrcConnectorIndex = srcConnectorIndex,
                            DstStateID = dstStateID,
                            DstConnectorIndex = dstConnectorIndex,
                            Page = page
                        };

                        clonedTransitionKeys.Add(key);
                        CreateTransition(clonedTransition);
                        clonedTransitions.Add(clonedTransition);
                    }
                }
            }

            // clone the objects that depend on the stateID
            foreach (long stateID in stateIDs)
            {
                SMState srcState = srcStateID2SrcState[stateID];
                if (srcState != null)
                {
                    // Content based on state type
                    long clonedStateID = srcStateID2ClonedState[srcState.ID].ID;
                    switch (srcState.StateType)
                    {
                        case StateType.Start:
                        case StateType.PageStart:
                        case StateType.PageEnd:
                        case StateType.Page:
                        case StateType.Terminate:
                        case StateType.Skip:
                            break;

                        case StateType.Message:
                            {
                                SMMessage srcMessage = RetrieveMessageByStateID(srcState.ID);
                                if (srcMessage != null)
                                {
                                    SMMessage clonedMessage = new SMMessage() { StateID = clonedStateID, Content = StringUtils.FriendlyString(srcMessage.Content) };
                                    CreateMessage(clonedMessage);
                                }
                            }
                            break;

                        case StateType.Decision:
                            {
                                SMDecision srcDecision = RetrieveDecisionByStateID(srcState.ID);
                                if (srcDecision != null)
                                {
                                    SMDecision clonedDecision = new SMDecision() { StateID = clonedStateID, Expression = srcDecision.Expression };
                                    CreateDecision(clonedDecision);
                                }
                            }
                            break;

                        case StateType.Question:
                        case StateType.MatrixQuestion:
                            {
                                SMQuestion srcQuestion = RetrieveQuestionByStateID(srcState.ID);
                                if (srcQuestion != null)
                                {
                                    SMQuestion clonedQuestion = new SMQuestion(srcQuestion) { StateID = clonedStateID };
                                    CreateQuestion(clonedQuestion);

                                    foreach (var language in _languages)
                                    {
                                        var srcQuestionContents = RetrieveQuestionContents(srcQuestion.ID, language.ID, QuestionContentType.ANCHOR_TEXT);
                                        srcQuestionContents.AddRange(RetrieveQuestionContents(srcQuestion.ID, language.ID, QuestionContentType.BODY_TEXT));
                                        srcQuestionContents.AddRange(RetrieveQuestionContents(srcQuestion.ID, language.ID, QuestionContentType.HEADER_TEXT));
                                        srcQuestionContents.AddRange(RetrieveQuestionContents(srcQuestion.ID, language.ID, QuestionContentType.OTHER_SPECIFY_TEXT));
                                        if (srcQuestionContents != null && srcQuestionContents.Count > 0)
                                        {
                                            foreach (var srcQuestionContent in srcQuestionContents)
                                            {
                                                var clonedQuestionContent = new SMQuestionContent(srcQuestionContent) { QuestionID = clonedQuestion.ID };
                                                CreateQuestionContent(clonedQuestionContent);

                                                if (srcState.StateType == StateType.MatrixQuestion)
                                                {
                                                    // Matrix Answers
                                                    var srcMatrixAnswers = RetrieveMatrixAnswerByQuestionContentID(srcQuestionContent.ID);
                                                    if (srcMatrixAnswers != null && srcMatrixAnswers.Count > 0)
                                                    {
                                                        foreach (var srcMatrixAnswer in srcMatrixAnswers)
                                                        {
                                                            SMMatrixAnswer clonedMatrixAnswer = new SMMatrixAnswer(srcMatrixAnswer) { QuestionContentID = clonedQuestionContent.ID };
                                                            CreateMatrixAnswer(clonedMatrixAnswer);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (srcState.StateType == StateType.Question)
                                        {
                                            // Answer bodys
                                            var srcAnswerContents = RetrieveAnswerContents(srcQuestion.ID, language.ID);
                                            if (srcAnswerContents != null && srcAnswerContents.Count > 0)
                                            {
                                                foreach (var srcAnswerContent in srcAnswerContents)
                                                {
                                                    SMAnswerContent clonedAnswerContent = new SMAnswerContent(srcAnswerContent) { QuestionID = clonedQuestion.ID };
                                                    CreateAnswerContent(clonedAnswerContent);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
		}

		/// <summary>
		/// Move a set of states and transitions to another page and delete orphaned transitions
		/// </summary>
		/// <param name="stateIDs">list of state IDs</param>
		/// <param name="transitions">list of transitions</param>
		/// <param name="newPageID">page to move states and transitions</param>
		public void MoveStatesToPage(List<long> stateIDs, List<SMTransition> transitions, long newPageID, List<SMTransition> orphanTransitions)
		{
            const string methodName = "MoveStatesToPage";

            if (newPageID != 0)
            {
                SMState page = RetrieveState(newPageID);
                if (page == null)
                {
                    string msg = string.Format("Page with ID {0} doesn't exist", newPageID);
                    _logger.Error(_className, methodName, msg);
                    throw new ArgumentException(msg);
                }
            }

            if (stateIDs != null && stateIDs.Count > 0)
            {
                foreach (long stateID in stateIDs)
                {
                    SMState state = RetrieveState(stateID);
                    if (state == null)
                    {
                        string msg = string.Format("State with ID {0} doesn't exist", stateID);
                        _logger.Error(_className, methodName, msg);
                        throw new ArgumentException(msg);
                    }
                    else
                    {
                        state.Page = newPageID;
                        UpdateState(state);
                    }
                }
            }

            if (transitions != null && transitions.Count > 0)
            {
                foreach (SMTransition transition in transitions)
                {
                    SMTransition trans = RetrieveTransition(transition.SrcStateID, transition.SrcConnectorIndex, transition.DstStateID, transition.DstConnectorIndex);
                    if (trans == null)
                    {
                        string msg = string.Format("Transition {0},{1}->{2},{3} doesn't exist", transition.SrcStateID, transition.SrcConnectorIndex, transition.DstStateID, transition.DstConnectorIndex);
                        _logger.Error(_className, methodName, msg);
                        throw new ArgumentException(msg);
                    }
                    else
                    {
                        trans.Page = newPageID;
                        UpdateTransition(trans);
                    }
                }
            }

            if (orphanTransitions != null && orphanTransitions.Count > 0)
            {
                foreach (SMTransition orphanTransition in orphanTransitions)
                {
                    DeleteTransition(orphanTransition.SrcStateID, orphanTransition.SrcConnectorIndex, orphanTransition.DstStateID, orphanTransition.DstConnectorIndex);
                }
            }
		}

		/// <summary>
		/// Update an existing state.
		/// </summary>
		/// <param name="state">existing state</param>
		/// <returns>the updated state</returns>
		public void UpdateState(SMState state)
		{
            if (string.IsNullOrEmpty(state.UIDescription)) state.UIDescription = " ";
			SMStateDao.Update(state);
		}

		/// <summary>
		/// Get a specific state.
		/// </summary>
		/// <param name="ID">unique identifier for the state</param>
		/// <returns>the specified state</returns>
		public SMState RetrieveState(long ID)
		{
			return SMStateDao.Retrieve(ID);
		}

		/// <summary>
		/// Get a specific state.
		/// </summary>
		/// <param name="surveyID">unique identifier for the survey</param>
		/// <param name="name">the UI name for the state</param>
		/// <returns>the specified state</returns>
		public SMState RetrieveState(long surveyID, string name)
		{
			return SMStateDao.Retrieve(surveyID, name);
		}

		/// <summary>
		/// Get a list of states for a specific survey.
		/// </summary>
		/// <param name="surveyID">unique identifier for the survey</param>
		/// <returns>list of states for the specified survey</returns>
		public List<SMState> RetrieveStatesBySurveyID(long surveyID)
		{
			return SMStateDao.RetrieveAllBySurveyID(surveyID) ?? new List<SMState>();
		}

		/// <summary>
		/// Get a list of states on a page in the order they should appear on the page.
		/// </summary>
		/// <param name="pageID">unique ID of the page</param>
		/// <returns>list of states</returns>
		public List<SMState> RetrieveStatesByPageID(long pageID)
		{
            List<SMState> result = new List<SMState>();
            List<SMState> tmp = SMStateDao.RetrieveAllByPageID(pageID);
            if (tmp != null)
            {
                SMState curState = null;
                foreach (SMState state in tmp)
                {
                    if (state.StateType == StateType.PageStart)
                    {
                        curState = state;
                        break;
                    }
                }
                while (curState != null)
                {
                    SMTransitionCollection outputs = curState.GetOutputs(this.Config);
                    if (outputs == null || outputs.Count <= 0) break;

                    SMState nextState = null;
                    foreach (SMState state in tmp)
                    {
                        if (state.ID == outputs[0].DstStateID)
                        {
                            nextState = state;
                            break;
                        }
                    }
                    if (nextState != null)
                    {
                        switch (nextState.StateType)
                        {
                            case StateType.Message:
                            case StateType.Question:
                            case StateType.MatrixQuestion:
                                result.Add(nextState);
                                curState = nextState;
                                break;
                            default:
                                nextState = null;
                                break;
                        }
                    }
                    curState = nextState;
                }
            }
            return result;
		}

		/// <summary>
		/// Delete a specific state
		/// </summary>
		/// <param name="stateID">unique identifier for the state</param>
		public void DeleteState(long stateID)
		{
            SMState state = RetrieveState(stateID);
            if (state != null)
            {
                state.ClearInputsAndOutputs();

                switch (state.StateType)
                {
                    case StateType.Message:
                        SMMessage message = SMMessageDao.RetrieveByStateID(stateID);
                        if (message != null)
                            SMMessageDao.Delete(message.ID);
                        break;
                    case StateType.Decision:
                        SMDecision decision = SMDecisionDao.RetrieveByStateID(stateID);
                        if (decision != null)
                            SMDecisionDao.Delete(decision.ID);
                        break;
                    case StateType.Question:
                    case StateType.MatrixQuestion:
                        SMQuestion question = SMQuestionDao.RetrieveByStateID(stateID);
                        if (question != null)
                        {
                            List<SMLanguage> languages = SMLanguageDao.RetrieveAll();
                            if (languages != null)
                            {
                                foreach (SMLanguage language in languages)
                                {
                                    List<SMAnswerContent> answers = SMAnswerContentDao.RetrieveAllForQuestion(question.ID, language.ID);
                                    if (answers != null)
                                    {
                                        foreach (SMAnswerContent answer in answers)
                                        {
                                            SMAnswerContentDao.Delete(answer.ID);
                                        }
                                    }

                                    List<SMMatrixAnswer> matrixAnswers = SMMatrixAnswerDao.RetrieveByQuestionID(question.ID, language.ID);
                                    if (matrixAnswers != null)
                                    {
                                        foreach (SMMatrixAnswer matrixAnswer in matrixAnswers)
                                        {
                                            SMMatrixAnswerDao.Delete(matrixAnswer.ID);
                                        }
                                    }

                                    List<SMQuestionContent> questionContents = SMQuestionContentDao.RetrieveAllByType(question.ID, language.ID, QuestionContentType.HEADER_TEXT);
                                    if (questionContents != null)
                                    {
                                        foreach (SMQuestionContent questionContent in questionContents)
                                        {
                                            SMQuestionContentDao.Delete(questionContent.ID);
                                        }
                                    }

                                    questionContents = SMQuestionContentDao.RetrieveAllByType(question.ID, language.ID, QuestionContentType.BODY_TEXT);
                                    if (questionContents != null)
                                    {
                                        foreach (SMQuestionContent questionContent in questionContents)
                                        {
                                            List<SMResponse> responses = SMResponseDao.RetrieveAllForQuestionContent(questionContent.ID);
                                            if (responses != null)
                                            {
                                                foreach (SMResponse response in responses)
                                                {
                                                    SMResponseDao.Delete(response.ID);
                                                }
                                            }
                                            SMQuestionContentDao.Delete(questionContent.ID);
                                        }
                                    }

                                    questionContents = SMQuestionContentDao.RetrieveAllByType(question.ID, language.ID, QuestionContentType.ANCHOR_TEXT);
                                    if (questionContents != null)
                                    {
                                        foreach (SMQuestionContent questionContent in questionContents)
                                        {
                                            SMQuestionContentDao.Delete(questionContent.ID);
                                        }
                                    }

                                    questionContents = SMQuestionContentDao.RetrieveAllByType(question.ID, language.ID, QuestionContentType.OTHER_SPECIFY_TEXT);
                                    if (questionContents != null)
                                    {
                                        foreach (SMQuestionContent questionContent in questionContents)
                                        {
                                            List<SMResponse> responses = SMResponseDao.RetrieveAllForQuestionContent(questionContent.ID);
                                            if (responses != null)
                                            {
                                                foreach (SMResponse response in responses)
                                                {
                                                    SMResponseDao.Delete(response.ID);
                                                }
                                            }
                                            SMQuestionContentDao.Delete(questionContent.ID);
                                        }
                                    }
                                }
                            }
                            DeleteQuestion(question);
                        }
                        break;
                }
                SMStateDao.Delete(stateID);
            }
		}

		/// <summary>
		/// Delete a set of states and transitions
		/// </summary>
		/// <param name="stateIDs">list of state IDs</param>
		/// <param name="transitions">list of transitions</param>
		public void DeleteStates(List<long> stateIDs, List<SMTransition> transitions)
		{
            const string methodName = "DeleteStates";
            foreach (SMTransition transition in transitions)
            {
                try
                {
                    DeleteTransition(transition);
                }
                catch (Exception ex)
                {
                    _logger.Error(_className, methodName, "Failed to delete state output " + transition.DstStateID.ToString() + " of state " + transition.SrcStateID.ToString(), ex);
                    throw;
                }
            }

            foreach (long stateID in stateIDs)
            {
                try
                {
                    DeleteState(stateID);
                }
                catch (Exception ex)
                {
                    _logger.Error(_className, methodName, "Failed to delete state " + stateID.ToString(), ex);
                    throw;
                }
            }
		}

		public Dictionary<long, SMState> GetStateMap(List<SMState> states)
		{
            Dictionary<long, SMState> result = new Dictionary<long, SMState>();
            if (states != null && states.Count > 0)
            {
                foreach (SMState state in states)
                {
                    result.Add(state.ID, state);
                }
            }
            return result;
		}
		#endregion

		#region SMTransition
		/// <summary>
		/// Create a transition.
		/// </summary>
		/// <param name="transition">the transition to create</param>
		/// <returns>the new transition</returns>
		public void CreateTransition(SMTransition transition)
		{
			SMTransitionDao.Create(transition);
		}

		/// <summary>
		/// Update a transition.
		/// </summary>
		/// <param name="transition">the transition to update</param>
		/// <returns>the updated transition</returns>
		public void UpdateTransition(SMTransition transition)
		{
			SMTransitionDao.Update(transition);
		}

		/// <summary>
		/// Get a specific transition.
		/// </summary>
		/// <param name="srcStateID">the ID of the source state</param>
		/// <param name="srcConnectorIndex">the index of the output connector on the source state</param>
		/// <param name="dstStateID">the ID of the destination state</param>
		/// <param name="dstConnectorIndex">the index of the input connector on the source state</param>
		/// <returns>the specified transition, or null if it doesn't exist</returns>
		public SMTransition RetrieveTransition(long srcStateID, long srcConnectorIndex, long dstStateID, long dstConnectorIndex)
		{
			return SMTransitionDao.Retrieve(srcStateID, srcConnectorIndex, dstStateID, dstConnectorIndex);
		}

		/// <summary>
		/// Get the list of transitions that terminate at the given state.
		/// </summary>
		/// <param name="stateID">a specific state</param>
		/// <returns>list of input transitions for the state</returns>
		public List<SMTransition> RetrieveInputTransitions(long stateID)
		{
            return SMTransitionDao.RetrieveInputs(stateID) ?? new List<SMTransition>();
		}

		/// <summary>
		/// Get the list of transitions that originate at the given state.
		/// </summary>
		/// <param name="stateID">a specific state</param>
		/// <returns>list of output transitions for the state</returns>
		public List<SMTransition> RetrieveOutputTransitions(long stateID)
		{
			return SMTransitionDao.RetrieveOutputs(stateID) ?? new List<SMTransition>();
		}

		/// <summary>
		/// Delete a transition.
		/// </summary>
		/// <param name="transition">transition to be deleted</param>
		public void DeleteTransition(SMTransition transition)
		{
			DeleteTransition(transition.SrcStateID, transition.SrcConnectorIndex, transition.DstStateID, transition.DstConnectorIndex);
		}

		/// <summary>
		/// Delete a transition.
		/// </summary>
		/// <param name="srcStateID">the ID of the source state</param>
		/// <param name="srcConnectorIndex">the index of the output connector on the source state</param>
		/// <param name="dstStateID">the ID of the destination state</param>
		/// <param name="dstConnectorIndex">the index of the input connector on the source state</param>
		public void DeleteTransition(long srcStateID, long srcConnectorIndex, long dstStateID, long dstConnectorIndex)
		{
			SMTransitionDao.Delete(srcStateID, srcConnectorIndex, dstStateID, dstConnectorIndex);
		}
		#endregion

		#region SMQuestion
		/// <summary>
		/// Create a question.
		/// </summary>
		/// <param name="question">the question to create</param>
		/// <returns>the new question</returns>
		public void CreateQuestion(SMQuestion question)
		{
			SMQuestionDao.Create(question);
            CacheManager.Update(CacheRegions.QuestionByStateId, question.StateID, question);
		}

		/// <summary>
		/// Update a question.
		/// </summary>
		/// <param name="question">the question to update</param>
		/// <returns>the updated question</returns>
		public void UpdateQuestion(SMQuestion question)
		{
			SMQuestionDao.Update(question);
            CacheManager.Update(CacheRegions.QuestionByStateId, question.StateID, question);
		}

		/// <summary>
		/// Get a specific question.
		/// </summary>
		/// <param name="questionID">the ID of the question</param>
		/// <returns>the specified question, or null if it doesn't exist</returns>
		public SMQuestion RetrieveQuestion(long questionID)
		{
            SMQuestion result = SMQuestionDao.Retrieve(questionID);
            CacheManager.Update(CacheRegions.QuestionByStateId, result.StateID, result);
			return result;
		}

		/// <summary>
		/// Get a specific question.
		/// </summary>
		/// <param name="stateID">the ID of the associated state</param>
		/// <returns>the specified question, or null if it doesn't exist</returns>
		public SMQuestion RetrieveQuestionByStateID(long stateID)
		{
            SMQuestion result = (SMQuestion)CacheManager.Get(CacheRegions.QuestionByStateId, stateID);
			if (result == null)
			{
				result = SMQuestionDao.RetrieveByStateID(stateID);
                CacheManager.Update(CacheRegions.QuestionByStateId, stateID, result);
			}
			return result;
		}

		/// <summary>
		/// Get a specific question.
		/// </summary>
		/// <param name="surveyID">the associated survey</param>
		/// <param name="stateName">the name of the question's state</param>
		/// <returns>the specified question, or null if it doesn't exist</returns>
		public SMQuestion RetrieveQuestionByStateName(long surveyID, string stateName)
		{
            SMQuestion result = SMQuestionDao.RetrieveByStateName(surveyID, stateName);
            if (result != null)
            {
                CacheManager.Update(CacheRegions.QuestionByStateId, result.StateID, result);
            }
            return result;
		}

		/// <summary>
		/// Get the questions for a survey.
		/// </summary>
		/// <param name="surveyID">unique identifier for the survey</param>
		/// <returns>list of questions for the associated survey</returns>
		public List<SMQuestion> RetrieveQuestions(long surveyID)
		{
            List<SMQuestion> result = SMQuestionDao.RetrieveAllBySurveyID(surveyID) ?? new List<SMQuestion>();
            foreach (SMQuestion question in result)
            {
                CacheManager.Update(CacheRegions.QuestionByStateId, question.StateID, question);
            }
            return result;
		}

		/// <summary>
		/// Delete a question.
		/// </summary>
		/// <param name="question">question to be deleted</param>
		public void DeleteQuestion(SMQuestion question)
		{
            if (question != null)
            {
                CacheManager.Remove(CacheRegions.QuestionByStateId, question.StateID);
                SMQuestionDao.Delete(question.ID);
            }
		}

		/// <summary>
		/// Delete a question.
		/// </summary>
		/// <param name="questionID">the ID of the question</param>
		public void DeleteQuestion(long questionID)
		{
            SMQuestion question = RetrieveQuestion(questionID);
            if (question != null)
            {
                DeleteQuestion(question);
            }
		}
		#endregion

		#region SMQuestionContent
		/// <summary>
		/// Create a questionContent.
		/// </summary>
		/// <param name="question">the questionContent to create</param>
		/// <returns>the new questionContent</returns>
		public void CreateQuestionContent(SMQuestionContent questionContent)
		{
			SMQuestionContentDao.Create(questionContent);
		}

		/// <summary>
		/// Update a questionContent.
		/// </summary>
		/// <param name="questionContent">the questionContent to update</param>
		/// <returns>the updated questionContent</returns>
		public void UpdateQuestionContent(SMQuestionContent questionContent)
		{
			SMQuestionContentDao.Update(questionContent);
		}

		/// <summary>
		/// Get a specific questionContent.
		/// </summary>
		/// <param name="questionContentID">the ID of the questionContent</param>
		/// <returns>the specified questionContent, or null if it doesn't exist</returns>
		public SMQuestionContent RetrieveQuestionContent(long questionContentID)
		{
			return SMQuestionContentDao.Retrieve(questionContentID);
		}

		/// <summary>
		/// Get a list of questionContent
		/// </summary>
		/// <param name="questionID">associated question</param>
		/// <param name="languageID">associated language</param>
		/// <param name="contentType">content type</param>
		/// <returns>list of matching QuestionContent</returns>
		public List<SMQuestionContent> RetrieveQuestionContents(long questionID, long languageID, QuestionContentType contentType)
		{
			return SMQuestionContentDao.RetrieveAllByType(questionID, languageID, contentType) ?? new List<SMQuestionContent>();
		}

		/// <summary>
		/// Delete a questionContent.
		/// </summary>
		/// <param name="questionContent">questionContent to be deleted</param>
		public void DeleteQuestionContent(SMQuestionContent questionContent)
		{
			DeleteResponsesForQuestionContent(questionContent.ID);
			DeleteQuestionContent(questionContent.ID);
		}

		/// <summary>
		/// Delete a questionContent.
		/// </summary>
		/// <param name="questionContentID">the ID of the questionContent</param>
		public void DeleteQuestionContent(long questionContentID)
		{
			DeleteResponsesForQuestionContent(questionContentID);
			SMQuestionContentDao.Delete(questionContentID);
		}
		#endregion

		#region SMAnswerContent
		/// <summary>
		/// Create a answerContent.
		/// </summary>
		/// <param name="question">the answerContent to create</param>
		/// <returns>the new answerContent</returns>
		public void CreateAnswerContent(SMAnswerContent answerContent)
		{
			SMAnswerContentDao.Create(answerContent);
		}

		/// <summary>
		/// Update a answerContent.
		/// </summary>
		/// <param name="answerContent">the answerContent to update</param>
		/// <returns>the updated answerContent</returns>
		public void UpdateAnswerContent(SMAnswerContent answerContent)
		{
			SMAnswerContentDao.Update(answerContent);
		}

		/// <summary>
		/// Get a specific answerContent.
		/// </summary>
		/// <param name="answerContentID">the ID of the answerContent</param>
		/// <returns>the specified answerContent, or null if it doesn't exist</returns>
		public SMAnswerContent RetrieveAnswerContent(long answerContentID)
		{
			return SMAnswerContentDao.Retrieve(answerContentID);
		}

		/// <summary>
		/// Get a list of answerContent for a specific question and language.
		/// </summary>
		/// <param name="questionID">associated question</param>
		/// <param name="languageID">associated language</param>
		/// <returns>list of matching AnswerContent</returns>
		public List<SMAnswerContent> RetrieveAnswerContents(long questionID, long languageID)
		{
			return SMAnswerContentDao.RetrieveAllForQuestion(questionID, languageID) ?? new List<SMAnswerContent>();
		}

		/// <summary>
		/// Delete a answerContent.
		/// </summary>
		/// <param name="answerContent">answerContent to be deleted</param>
		public void DeleteAnswerContent(SMAnswerContent answerContent)
		{
			DeleteAnswerContent(answerContent.ID);
		}

		/// <summary>
		/// Delete a answerContent.
		/// </summary>
		/// <param name="answerContentID">the ID of the answerContent</param>
		public void DeleteAnswerContent(long answerContentID)
		{
			SMAnswerContentDao.Delete(answerContentID);
		}
		#endregion

		#region SMMatrixAnswer
		/// <summary>
		/// Create a matrixAnswer.
		/// </summary>
		/// <param name="matrixAnswer">the matrixAnswer to create</param>
		/// <returns>the new matrixAnswer</returns>
		public void CreateMatrixAnswer(SMMatrixAnswer matrixAnswer)
		{
			SMMatrixAnswerDao.Create(matrixAnswer);
		}

		/// <summary>
		/// Update a matrixAnswer.
		/// </summary>
		/// <param name="matrixAnswer">the matrixAnswer to update</param>
		/// <returns>the updated matrixAnswer</returns>
		public void UpdateMatrixAnswer(SMMatrixAnswer matrixAnswer)
		{
			SMMatrixAnswerDao.Update(matrixAnswer);
		}

		/// <summary>
		/// Get a specific matrixAnswer.
		/// </summary>
		/// <param name="matrixAnswerID">the ID of the matrixAnswer</param>
		/// <returns>the specified matrixAnswer, or null if it doesn't exist</returns>
		public SMMatrixAnswer RetrieveMatrixAnswerID(long matrixAnswerID)
		{
			return SMMatrixAnswerDao.Retrieve(matrixAnswerID);
		}

		/// <summary>
		/// Get a list of matrixAnswer for a questionContentID.
		/// </summary>
		/// <param name="questionContentID">the ID of the associated questionContent</param>
		/// <returns>the list of matrixAnswers</returns>
		public List<SMMatrixAnswer> RetrieveMatrixAnswerByQuestionContentID(long questionContentID)
		{
            return SMMatrixAnswerDao.RetrieveByQuestionContentID(questionContentID) ?? new List<SMMatrixAnswer>();
		}

		/// <summary>
		/// Get a list of the matrixAnswers for a specific question/language.
		/// </summary>
		/// <param name="questionID">unique ID for the question</param>
		/// <param name="languageID">unique ID for the language</param>
		/// <returns>list of matrixAnswers</returns>
		public List<SMMatrixAnswer> RetrieveMatrixAnswerByQuestionID(long questionID, long languageID)
		{
            return SMMatrixAnswerDao.RetrieveByQuestionID(questionID, languageID) ?? new List<SMMatrixAnswer>();
		}

		/// <summary>
		/// Delete a matrixAnswer.
		/// </summary>
		/// <param name="matrixAnswer">matrixAnswer to be deleted</param>
		public void DeleteMatrixAnswer(SMMatrixAnswer matrixAnswer)
		{
			DeleteMatrixAnswer(matrixAnswer.ID);
		}

		/// <summary>
		/// Delete a matrixAnswer.
		/// </summary>
		/// <param name="matrixAnswerID">the ID of the matrixAnswer</param>
		public void DeleteMatrixAnswer(long matrixAnswerID)
		{
			SMMatrixAnswerDao.Delete(matrixAnswerID);
		}
		#endregion

		#region SMRespondent
		public void CreateRespondent(SMRespondent respondent)
		{
			SMRespondentDao.Create(respondent);
		}

		public void CreateRespondents(long surveyID, long languageID, long respListID, List<string> mtouchValues, List<string> propertiesXMLs)
		{
            if (mtouchValues.Count < 1)
                throw new ArgumentException("mtouchValues.Count < 1");
            if (propertiesXMLs.Count < 1)
                throw new ArgumentException("mtouchValues.Count < 1");
            if (mtouchValues.Count != propertiesXMLs.Count)
                throw new ArgumentException("mtouchValues.Count != propertiesXMLs.Count");

            if (this.DatabaseType == SupportedDataSourceType.Oracle10g)
            {
                SMRespondentDao.CreateRespondentsOracle(surveyID, languageID, respListID, mtouchValues, propertiesXMLs);
            }
            else
            {
                for (int index = 0; index < mtouchValues.Count; index++)
                {
                    SMRespondent respondent = new SMRespondent()
                    {
                        SurveyID = surveyID,
                        LanguageID = languageID,
                        RespListID = respListID,
                        MTouch = mtouchValues[index],
                        IPCode = -1,
                        PropertiesXML = propertiesXMLs[index],
                        Skipped = false
                    };
                    respondent.UpdateDate = respondent.CreateDate = DateTime.Now;
                    CreateRespondent(respondent);
                }
            }
		}

		public void UpdateRespondent(SMRespondent respondent)
		{
			SMRespondentDao.Update(respondent);
		}

		/// <summary>
		/// Reset the start and completion dates for a set of respondents.
		/// </summary>
		/// <param name="surveyID">ID of the survey, or -1 for all</param>
		/// <param name="languageID">ID of the language, or -1 for all</param>
		/// <param name="mtouch">MTouch code, or null/string.Empty for unspecified</param>
		/// <param name="ipcode">IPCode, or -1 for unspecified</param>
		/// <returns>number of rows updated</returns>
		public int ResetRespondents(long surveyID, long languageID, string mtouch, long ipcode)
		{
			return SMRespondentDao.Reset(surveyID, languageID, mtouch, ipcode);
		}

		/// <summary>
		/// Get a specific respondent.
		/// </summary>
		/// <param name="respondentID">the ID of the respondent</param>
		/// <returns>the specified respondent, or null if it doesn't exist</returns>
		public SMRespondent RetrieveRespondent(long respondentID)
		{
			return SMRespondentDao.Retrieve(respondentID);
		}

		/// <summary>
		/// Get a specific respondent.
		/// </summary>
		/// <param name="surveyID">unique identifier for the survey</param>
		/// <param name="languageID">unique identifier for the language</param>
		/// <param name="mtouch">unique mtouch code</param>
		/// <param name="ipcode">unique identifier for the member</param>
		/// <returns></returns>
		public SMRespondent RetrieveRespondent(long surveyID, long languageID, string mtouch, long ipcode)
		{
            SMRespondent result = null;

            // Try mtouch first
            if (!string.IsNullOrEmpty(mtouch))
                result = SMRespondentDao.RetrieveByMTouch(surveyID, languageID, mtouch);

            if (result == null && ipcode != -1)
                result = SMRespondentDao.RetrieveByIPCode(surveyID, languageID, ipcode);

            return result;
		}

		/// <summary>
		/// Get a list of respondent for a specific survey and language.
		/// </summary>
		/// <param name="surveyID">associated survey</param>
		/// <param name="languageID">associated language</param>
		/// <returns>list of matching Respondent</returns>
		public List<SMRespondent> RetrieveRespondents(long surveyID, long languageID)
		{
            return SMRespondentDao.RetrieveAllForSurvey(surveyID, languageID) ?? new List<SMRespondent>();
		}

		/// <summary>
		/// Get a list of provisioned MTouch codes.
		/// </summary>
		/// <returns>list of MTouch codes</returns>
		public List<string> RetrieveAllMTouches()
		{
            return SMRespondentDao.RetrieveAllMTouches() ?? new List<string>();
		}

		/// <summary>
		/// Get a list of provisioned MTouch codes for the specified survey.
		/// </summary>
		/// <param name="surveyID">unique ID for the survey</param>
		/// <returns>list of MTouch codes</returns>
		public List<string> RetrieveAllMTouchesForSurveyID(long surveyID)
		{
            if (surveyID == -1) return RetrieveAllMTouches();

            return SMRespondentDao.RetrieveAllMTouchesForSurveyID(surveyID) ?? new List<string>();
		}

		/// <summary>
		/// Get a list of provisioned MTouch codes for the specified language.
		/// </summary>
		/// <param name="languageID">unique ID for the language</param>
		/// <returns>list of MTouch codes</returns>
		public List<string> RetrieveAllMTouchesForLanguageID(long languageID)
		{
            if (languageID == -1) return RetrieveAllMTouches();

            return SMRespondentDao.RetrieveAllMTouchesForLanguageID(languageID) ?? new List<string>();
		}

		/// <summary>
		/// Get a list of provisioned MTouch codes for the specified survey and language.
		/// </summary>
		/// <param name="surveyID">unique ID for the survey</param>
		/// <param name="languageID">unique ID for the language</param>
		/// <returns>list of MTouch codes</returns>
		public List<string> RetrieveAllMTouches(long surveyID, long languageID)
		{
            if (surveyID == -1 && languageID == -1) return RetrieveAllMTouches();
            if (surveyID == -1) return RetrieveAllMTouchesForLanguageID(languageID);
            if (languageID == -1) return RetrieveAllMTouchesForSurveyID(surveyID);

            return SMRespondentDao.RetrieveAllMTouches(surveyID, languageID) ?? new List<string>();
		}

		/// <summary>
		/// Get a list of provisioned IPCodes
		/// </summary>
		/// <returns>list of IPCodes</returns>
		public List<long> RetrieveAllIPCodes()
		{
            return SMRespondentDao.RetrieveAllIPCodes() ?? new List<long>();
		}

		/// <summary>
		/// Get a list of provisioned IPCodes by survey ID
		/// </summary>
		/// <param name="surveyID">unique ID for the survey</param>
		/// <returns>list of IPCodes</returns>
		public List<long> RetrieveAllIPCodesForSurveyID(long surveyID)
		{
            if (surveyID == -1) return RetrieveAllIPCodes();

            return SMRespondentDao.RetrieveAllIPCodesForSurveyID(surveyID) ?? new List<long>();
		}

		/// <summary>
		/// Get a list of provisioned IPCodes by language ID
		/// </summary>
		/// <param name="languageID">unique ID for the language</param>
		/// <returns>list of IPCodes</returns>
		public List<long> RetrieveAllIPCodesForLanguageID(long languageID)
		{
            if (languageID == -1) return RetrieveAllIPCodes();

            return SMRespondentDao.RetrieveAllIPCodesForLanguageID(languageID) ?? new List<long>();
		}

		/// <summary>
		/// Get a list of provisioned IPCodes by survey ID and language ID
		/// </summary>
		/// <param name="surveyID">unique ID for the survey</param>
		/// <param name="languageID">unique ID for the language</param>
		/// <returns>list of IPCodes</returns>
		public List<long> RetrieveAllIPCodes(long surveyID, long languageID)
		{
            if (surveyID == -1 && languageID == -1) return RetrieveAllIPCodes();
            if (surveyID == -1) return RetrieveAllIPCodesForLanguageID(languageID);
            if (languageID == -1) return RetrieveAllIPCodesForSurveyID(surveyID);

            return SMRespondentDao.RetrieveAllIPCodes(surveyID, languageID) ?? new List<long>();
		}

		/// <summary>
		/// Get a list of eligible MTouch respondents
		/// </summary>
		/// <param name="surveyID">unique survey ID</param>
		/// <param name="languageID">unique language ID</param>
		/// <param name="maxResults">max results to return [1..100]</param>
		/// <returns>list of respondents</returns>
		public List<SMRespondent> RetrieveEligibleMTouches(long surveyID, long languageID, int maxResults)
		{
			return SMRespondentDao.RetrieveEligibleMTouches(surveyID, languageID, maxResults);
		}

		/// <summary>
		/// Get a list of eligible respondents for a specific survey/language, and for a specific mtouch and/or ipcode.
		/// </summary>
		/// <param name="surveyID">unique identifier for associated survey</param>
		/// <param name="languageID">unique identifier for associated language</param>
		/// <param name="mtouch">mtouch</param>
		/// <param name="ipcode">ipcode</param>
		/// <returns></returns>
		public List<SMRespondent> RetrieveEligibleRespondents(long surveyID, long languageID, string mtouch, long ipcode)
		{
            return SMRespondentDao.RetrieveAllEligible(surveyID, languageID, mtouch, ipcode) ?? new List<SMRespondent>();
		}

		/// <summary>
		/// Get a list of eligible respondents for a specific language, and for a specific mtouch and/or ipcode.
		/// </summary>
		/// <param name="languageID">unique identifier for associated language</param>
		/// <param name="mtouch">mtouch</param>
		/// <param name="ipcode">ipcode</param>
		/// <returns></returns>
		public List<SMRespondent> RetrieveEligibleRespondents(long languageID, string mtouch, long ipcode)
		{
            return SMRespondentDao.RetrieveAllEligible(languageID, mtouch, ipcode) ?? new List<SMRespondent>();
		}

		/// <summary>
		/// Evict cached respondent from cache
		/// </summary>
		/// <param name="respondent">respondent</param>
		public void EvictRespondentFromCache(SMRespondent respondent)
		{
			SMRespondentDao.EvictRespondentFromCache(respondent);
		}

		/// <summary>
		/// Is the provided mtouch valid?  That is, does it exist in the Respondent table?
		/// </summary>
		/// <param name="mtouch">mtouch code</param>
		/// <returns>true if valid, false otherwise</returns>
		public bool IsValidMTouch(string mtouch)
		{
			return SMRespondentDao.IsValidMTouch(mtouch);
		}

		/// <summary>
		/// Number of completions by segment
		/// </summary>
		/// <param name="surveyID">unique ID of the survey</param>
		/// <param name="respondentPropName">segment property name</param>
		/// <param name="respondentPropValue">segment property value</param>
		/// <returns>count of completed surveys for specified segment</returns>
		public long NumCompletesForSegment(long surveyID, string respondentPropName, string respondentPropValue)
		{
			return SMRespondentDao.NumCompletesForSegment(surveyID, respondentPropName, respondentPropValue);
		}

		public IDataReader GetRespondentListExport(long respListID)
		{
			return SMRespondentDao.RetrieveRespondentListExport(respListID, this.DatabaseType);
		}

		public List<long> RetrieveRespondentIDs(long surveyID, long languageID, string mtouch, long ipcode, long? respListID)
		{
            return SMRespondentDao.RetrieveIDs(surveyID, languageID, mtouch, ipcode, respListID) ?? new List<long>();
		}

		/// <summary>
		/// Delete a respondent.
		/// </summary>
		/// <param name="respondent">respondent to be deleted</param>
		public void DeleteRespondent(SMRespondent respondent)
		{
			DeleteRespondent(respondent.ID);
		}

		/// <summary>
		/// Delete a respondent.
		/// </summary>
		/// <param name="respondentID">the ID of the respondent</param>
		public void DeleteRespondent(long respondentID)
		{
			DeleteConceptViewsByRespondent(respondentID);
			DeleteResponsesForRespondent(respondentID);
			SMRespondentDao.Delete(respondentID);
		}

		public void DeleteRespondents(List<long> respondentIDs)
		{
			SMConceptViewDao.DeleteAllForRespondentIDs(respondentIDs);
			SMResponseDao.DeleteAllForRespondentIDs(respondentIDs);
			SMRespondentDao.DeleteAllForRespondentIDs(respondentIDs);
		}
		#endregion

		#region SMResponse
		/// <summary>
		/// Create a response.
		/// </summary>
		/// <param name="question">the response to create</param>
		/// <returns>the new response</returns>
		public void CreateResponse(SMResponse response)
		{
			SMResponseDao.Create(response);
		}

		/// <summary>
		/// Update a response.
		/// </summary>
		/// <param name="response">the response to update</param>
		/// <returns>the updated response</returns>
		public void UpdateResponse(SMResponse response)
		{
			SMResponseDao.Update(response);
		}

		/// <summary>
		/// Get a specific response.
		/// </summary>
		/// <param name="responseID">the ID of the response</param>
		/// <returns>the specified response, or null if it doesn't exist</returns>
		public SMResponse RetrieveResponse(long responseID)
		{
			return SMResponseDao.Retrieve(responseID);
		}

		/// <summary>
		/// Get a list of response for a specific Respondent and State.
		/// </summary>
		/// <param name="respondentID">associated Respondent</param>
		/// <param name="stateID">associated State</param>
		/// <returns>list of matching Response</returns>
		public List<SMResponse> RetrieveResponsesForState(long respondentID, long stateID)
		{
            List<SMResponse> result = SMResponseDao.RetrieveAllForState(respondentID, stateID);
            if (result == null)
            {
                result = new List<SMResponse>();
            }
            for (int index = result.Count - 1; index >= 0; index--)
            {
                if (!string.IsNullOrWhiteSpace(result[index].Content) && result[index].Content.StartsWith("seen:"))
                {
                    result.RemoveAt(index);
                }
            }
            return result;
		}

		/// <summary>
		/// Get a list of response for a specific Respondent and QuestionContent.
		/// </summary>
		/// <param name="respondentID">associated Respondent</param>
		/// <param name="questionContentID">associated QuestionContent</param>
		/// <returns>list of matching Response</returns>
		public List<SMResponse> RetrieveResponses(long respondentID, long questionContentID)
		{
            List<SMResponse> result = SMResponseDao.RetrieveAllForQuestionContent(respondentID, questionContentID);
            if (result == null)
            {
                result = new List<SMResponse>();
            }
            for (int index = result.Count - 1; index >= 0; index--)
            {
                if (result[index].Content.StartsWith("seen:"))
                {
                    result.RemoveAt(index);
                }
            }
            return result;
		}

		/// <summary>
		/// Get responses as CSV
		/// </summary>
		/// <param name="surveyID">unique identifier for the desired survey</param>
		/// <returns>string containing CSV data</returns>
		public string RetrieveResponsesAsCSV(long surveyID, bool forSPSS)
		{
			SMSurveyResponseRawData result = GetRawResponses(surveyID, forSPSS);
			return result.AsCSV();
		}

		/// <summary>
		/// Get responses as XLS
		/// </summary>
		/// <param name="surveyID">unique identifier for the desired survey</param>
		/// <param name="worksheetName">the value to use for the worksheet name</param>
		/// <returns>byte[] containing XLS data</returns>
		public byte[] RetrieveResponsesAsXLS(long surveyID, string worksheetName)
		{
			SMSurveyResponseRawData result = GetRawResponses(surveyID, false);
			return result.AsXLS(worksheetName);
		}

		private SMSurveyResponseRawData GetRawResponses(long surveyID, bool forSPSS)
		{
            SMSurveyResponseRawData result = new SMSurveyResponseRawData(this.Config, surveyID, forSPSS);

            List<dynamic> rawRows = SMResponseDao.RetrieveRawResponses(this.DatabaseType, surveyID);
            result.AddFromRawRows(rawRows);

            List<dynamic> conceptViews = SMResponseDao.RetrieveConceptViews(surveyID);
            result.AddConceptViews(conceptViews);

            return result;
		}

		/// <summary>
		/// Get responses SPSS metadata.  The metadata provides a mapping between the answer values and
		/// the numeric values used in RetrieveResponsesAsCSV with forSPSS=true.
		/// </summary>
		/// <param name="surveyID">unique identifier for the desired survey</param>
		/// <returns>string with the SPSS metadata definitions</returns>
		public string RetrieveResponsesSPSSMetadata(long surveyID)
		{
            StringBuilder result = new StringBuilder()
                .AppendFormat("COMMENT 'Generated by LoyaltyWare version {0} on {1} {2}'.",
                    Version,
                    DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                    DateTimeUtil.GetLocalTimezone()
                ).AppendLine()
                .AppendLine("EXECUTE.");
            SMLanguage english = RetrieveLanguage("English");
            List<SMQuestion> questions = RetrieveQuestions(surveyID);
            List<SMState> states = RetrieveStatesBySurveyID(surveyID);
            if (questions != null && questions.Count > 0)
            {
                var questionsSortedByStateName = from q in questions
                                                 join s in states on q.StateID equals s.ID
                                                 orderby s.UIName ascending
                                                 select q;
                foreach (SMQuestion question in questionsSortedByStateName)
                {
                    if (!question.IsMultiAnswer())
                    {
                        SMState state = (from s in states where s.ID == question.StateID select s).FirstOrDefault<SMState>();
                        List<SMQuestionContent> questionContents = RetrieveQuestionContents(question.ID, english.ID, QuestionContentType.BODY_TEXT);
                        List<SMAnswerContent> answerContents = RetrieveAnswerContents(question.ID, english.ID);
                        if (state != null && questionContents != null && questionContents.Count > 0 && answerContents != null && answerContents.Count > 0)
                        {
                            // for simple question there is only one question content
                            string questionName = state.UIName.Replace(" ", string.Empty);
                            string questionValue =
                                StringUtils.DeHTML(StringUtils.FriendlyString(questionContents[0].Content))
                                    .Replace("'", string.Empty)
                                    .Replace("[[[answerblock]]]", string.Empty)
                                    .Trim();

                            result.AppendLine();
                            result.AppendFormat("VARIABLE LABELS {0} '{1}'.", questionName, questionValue);
                            result.AppendLine();
                            result.AppendFormat("VALUE LABELS {0}", questionName);
                            result.AppendLine();
                            for (int index = 0; index < answerContents.Count; index++)
                            {
                                SMAnswerContent answerContent = answerContents[index];
                                result.AppendFormat("{0}          '{1}'",
                                    (answerContent.DisplayIndex + 1),
                                    StringUtils.DeHTML(StringUtils.FriendlyString(answerContent.Content))
                                        .Replace("'", string.Empty).Trim()
                                );
                                if ((index + 1) == answerContents.Count) result.Append(".");
                                result.AppendLine();
                            }
                        }
                    }
                }
            }
            return result.ToString();
		}

		/// <summary>
		/// Get responses SPSS data map.  This is used to compare the numerical values in the SPSS data
		/// download with the textual values in the regular CSV data download.
		/// </summary>
		/// <param name="surveyID">unique identifier for the desired survey</param>
		/// <returns>string with SPSS data map</returns>
		public string RetrieveResponsesSPSSDatamap(long surveyID)
		{
            StringBuilder result = new StringBuilder()
                .AppendFormat("Generated by LoyaltyWare version {0} on {1} {2}.",
                    Version,
                    DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                    DateTimeUtil.GetLocalTimezone()
                ).AppendLine();
            SMLanguage english = RetrieveLanguage("English");
            List<SMQuestion> questions = RetrieveQuestions(surveyID);
            List<SMState> states = RetrieveStatesBySurveyID(surveyID);
            if (questions != null && questions.Count > 0 && states != null && states.Count > 0)
            {
                var questionsSortedByStateName = from q in questions
                                                 join s in states on q.StateID equals s.ID
                                                 orderby s.UIName ascending
                                                 select q;
                foreach (SMQuestion question in questionsSortedByStateName)
                {
                    SMState state = (from s in states where s.ID == question.StateID select s).FirstOrDefault<SMState>();
                    List<SMQuestionContent> questionContents = RetrieveQuestionContents(question.ID, english.ID, QuestionContentType.BODY_TEXT);
                    List<SMAnswerContent> answerContents = RetrieveAnswerContents(question.ID, english.ID);
                    if (state != null && questionContents != null && questionContents.Count > 0)
                    {
                        result.AppendLine().AppendLine().AppendLine();

                        string questionName = state.UIName.Replace(" ", string.Empty);
                        result.AppendFormat("Question Name: {0}", questionName).AppendLine();
                        result.AppendFormat("Question Type: {0}", question.IsMatrix ? "Matrix" : "Simple").AppendLine();
                        if (!question.IsMatrix)
                        {
                            // simple question
                            string questionValue =
                                StringUtils.DeHTML(StringUtils.FriendlyString(questionContents[0].Content))
                                    .Replace("'", string.Empty)
                                    .Replace("[[[answerblock]]]", string.Empty)
                                    .Trim();
                            result.AppendFormat("Question Text: '{0}'", questionValue).AppendLine();
                            if (answerContents != null && answerContents.Count > 0)
                            {
                                for (int index = 0; index < answerContents.Count; index++)
                                {
                                    SMAnswerContent answerContent = answerContents[index];
                                    result.AppendFormat("Answer[{0}] = '{1}'",
                                        (answerContent.DisplayIndex + 1),
                                        StringUtils.DeHTML(StringUtils.FriendlyString(answerContent.Content))
                                            .Replace("'", string.Empty).Trim()
                                    ).AppendLine();
                                }
                            }
                            else if (question.IsPiped)
                            {
                                SMState pipedState = (from s in states where s.ID == question.PipedStateID select s).FirstOrDefault<SMState>();
                                SMQuestion pipedQuestion = RetrieveQuestionByStateID(question.PipedStateID);
                                result.AppendFormat("Piped From Question: '{0}'", pipedState.UIName).AppendLine();
                                if (pipedQuestion != null)
                                {
                                    List<SMAnswerContent> pipedAnswerContents = RetrieveAnswerContents(pipedQuestion.ID, english.ID);
                                    if (pipedAnswerContents != null && pipedAnswerContents.Count > 0)
                                    {
                                        for (int index = 0; index < pipedAnswerContents.Count; index++)
                                        {
                                            SMAnswerContent pipedAnswerContent = pipedAnswerContents[index];
                                            result.AppendFormat("Answer[{0}] = '{1}'",
                                                (pipedAnswerContent.DisplayIndex + 1),
                                                StringUtils.DeHTML(StringUtils.FriendlyString(pipedAnswerContent.Content))
                                                    .Replace("'", string.Empty).Trim()
                                            ).AppendLine();
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // matrix question
                            List<SMQuestionContent> headers = RetrieveQuestionContents(question.ID, english.ID, QuestionContentType.HEADER_TEXT);
                            if (headers != null && headers.Count > 0)
                            {
                                result.AppendFormat("Header: '{0}'", StringUtils.DeHTML(StringUtils.FriendlyString(headers[0].Content)).Replace("[[[matrixquestion]]]", string.Empty).Replace("'", string.Empty).Trim()).AppendLine();
                            }
                            result.AppendFormat("Point Scale: {0}", question.MatrixPointScale.ToString()).AppendLine();
                            List<SMQuestionContent> anchors = RetrieveQuestionContents(question.ID, english.ID, QuestionContentType.ANCHOR_TEXT);
                            for (int index = 0; index < (int)question.MatrixPointScale; index++)
                            {
                                SMQuestionContent anchor = (index < anchors.Count ? anchors[index] : null);
                                if (anchor != null)
                                {
                                    result.AppendFormat("Anchor[{0}] = '{1}'",
                                        (anchor.MatrixIndex + 1),
                                        StringUtils.DeHTML(StringUtils.FriendlyString(anchor.Content))
                                            .Replace("'", string.Empty).Trim()
                                    ).AppendLine();
                                }
                                else
                                {
                                    result.AppendFormat("Anchor[{0}] = '{0}'", (index + 1)).AppendLine();
                                }
                            }
                            for (int index = 0; index < questionContents.Count; index++)
                            {
                                SMQuestionContent questionContent = questionContents[index];
                                result.AppendFormat("Question[{0}] = '{1}'",
                                    (questionContent.MatrixIndex + 1),
                                    StringUtils.DeHTML(StringUtils.FriendlyString(questionContent.Content))
                                        .Replace("'", string.Empty).Trim()
                                ).AppendLine();
                            }
                        }
                    }
                }
            }
            return result.ToString();
		}

		/// <summary>
		/// Get a simple report.
		/// </summary>
		/// <param name="surveyID">unique identifier for the desired survey</param>
		/// <returns>SurveyReportData</returns>
		public SMSurveyReportData SimpleReport(long surveyID, string filter)
		{
            SMSurveyReportData result = new SMSurveyReportData();

            // get the response data
            List<dynamic> rawRows = SMResponseDao.RetrieveResponseReport(this.DatabaseType, surveyID, filter);
            result.AddFromRawRows(rawRows);

            //// get the simple raw data
            //IList rawRows = SMResponseDAO.RetrieveSimpleReport(this.DBType, surveyID, filter);
            //result.AddFromRawRows(rawRows);

            //// get the matrix raw data
            //rawRows = SMResponseDAO.RetrieveMatrixReport(this.DBType, surveyID, filter);
            //result.AddFromRawRows(rawRows);

            //// get the text box raw data
            //rawRows = SMResponseDAO.RetrieveTextboxReport(this.DBType, surveyID, filter);
            //result.AddFromRawRows(rawRows);

            // Get summary of concept views
            result.ConceptViewSummary = SMConceptViewDao.RetrieveSummaryReport(surveyID, filter);

            result.NumCompletes = SMResponseDao.NumCompleted(surveyID);
            result.NumSkips = SMResponseDao.NumSkipped(surveyID);
            result.NumStarts = SMResponseDao.NumStarts(surveyID);

            return result;
		}

		/// <summary>
		/// Has the specified quota been met for the specified survey?  Note, a negative quota value can
		/// never be met, and a zero value is always met.
		/// </summary>
		/// <param name="surveyID">unique identifier for the survey</param>
		/// <param name="quota">quota</param>
		/// <returns>true if quota exceeded, false otherwise</returns>
		public bool IsQuotaMet(long surveyID, long quota)
		{
			return SMResponseDao.IsQuotaMet(surveyID, quota);
		}

		/// <summary>
		/// The number of respondents who answered the specified question in the specified survey.
		/// </summary>
		/// <param name="surveyID">unique identifier for the survey</param>
		/// <param name="questionID">unique identifier for the question</param>
		/// <param name="matrixIndex">optional index of the question in the matrix, use -1 to omit</param>
		/// <returns>number of respondents</returns>
		public long NumRespondents(long surveyID, long questionID, long matrixIndex)
		{
			return SMResponseDao.NumRespondents(surveyID, questionID, matrixIndex);
		}

		public long NumRespondents(long surveyID, long languageID)
		{
			return SMRespondentDao.NumRespondents(surveyID, languageID);
		}

		/// <summary>
		/// Do we need a response from the specified respondent to the question associated with the 
		/// specified state in the specified language?  That is, is there no existing response?
		/// </summary>
		/// <param name="stateID">unique identifier for the state in the state model</param>
		/// <param name="languageID">unique identifier for the language</param>
		/// <param name="respondentID">unique identifier for the respondent</param>
		/// <returns>true if we need a response, false otherwise</returns>
		public bool NeedResponse(long stateID, long languageID, long respondentID)
		{
			return SMResponseDao.NeedResponse(stateID, languageID, respondentID);
		}

		/// <summary>
		/// Delete a response.
		/// </summary>
		/// <param name="response">response to be deleted</param>
		public void DeleteResponse(SMResponse response)
		{
			DeleteResponse(response.ID);
		}

		/// <summary>
		/// Delete a response.
		/// </summary>
		/// <param name="responseID">the ID of the response</param>
		public void DeleteResponse(long responseID)
		{
			SMResponseDao.Delete(responseID);
		}

		/// <summary>
		/// Delete responses based on specified criteria.
		/// </summary>
		/// <param name="surveyID">ID of the survey, or -1 for all</param>
		/// <param name="languageID">ID of the language, or -1 for all</param>
		/// <param name="mtouch">MTouch code, or null/string.Empty for unspecified</param>
		/// <param name="ipcode">IPCode, or -1 for unspecified</param>
		/// <returns>number of rows updated</returns>
		public int DeleteResponses(long surveyID, long languageID, string mtouch, long ipcode)
		{
			return SMResponseDao.DeleteResponses(surveyID, languageID, mtouch, ipcode);
		}

		/// <summary>
		/// Delete all of the responses for a survey/language
		/// </summary>
		/// <param name="surveyID">ID for the survey</param>
		/// <param name="languageID">ID for the language (optional)</param>
		/// <returns>number of rows deleted</returns>
		public int DeleteResponsesForSurvey(long surveyID, long languageID = -1)
		{
            List<long> responseIDs = SMResponseDao.RetrieveIDs(surveyID, languageID);

            int rowsDeleted = 0;
            int curIndex = 0;
            while (curIndex < responseIDs.Count)
            {
                List<long> batch = new List<long>();
                for (int index = curIndex; index < Math.Min(1000, responseIDs.Count - curIndex); index++)
                {
                    batch.Add(responseIDs[index]);
                }
                rowsDeleted += SMResponseDao.Delete(batch);
                curIndex += batch.Count;
            }
            return rowsDeleted;
		}

		/// <summary>
		/// Delete responses for question content
		/// </summary>
		/// <param name="questionContentID">ID of the question content</param>
		/// <returns>number of rows deleted</returns>
		public int DeleteResponsesForQuestionContent(long questionContentID)
		{
			return SMResponseDao.DeleteAllForQuestionContent(questionContentID);
		}

		public int DeleteResponsesForRespondent(long respondentID)
		{
			return SMResponseDao.DeleteAllForRespondent(respondentID);
		}
		#endregion

		#region SMDecision
		/// <summary>
		/// Create a decision.
		/// </summary>
		/// <param name="decision">the decision to create</param>
		/// <returns>the new decision</returns>
		public void CreateDecision(SMDecision decision)
		{
			SMDecisionDao.Create(decision);
		}

		/// <summary>
		/// Update a decision.
		/// </summary>
		/// <param name="decision">the decision to update</param>
		/// <returns>the updated decision</returns>
		public void UpdateDecision(SMDecision decision)
		{
			SMDecisionDao.Update(decision);
		}

		/// <summary>
		/// Get a specific decision.
		/// </summary>
		/// <param name="decisionID">the ID of the decision</param>
		/// <returns>the specified decision, or null if it doesn't exist</returns>
		public SMDecision RetrieveDecision(long decisionID)
		{
			return SMDecisionDao.Retrieve(decisionID);
		}

		/// <summary>
		/// Get a specific decision.
		/// </summary>
		/// <param name="stateID">the ID of the associated state</param>
		/// <returns>the specified decision, or null if it doesn't exist</returns>
		public SMDecision RetrieveDecisionByStateID(long stateID)
		{
			return SMDecisionDao.RetrieveByStateID(stateID);
		}

		/// <summary>
		/// Delete a decision.
		/// </summary>
		/// <param name="decision">decision to be deleted</param>
		public void DeleteDecision(SMDecision decision)
		{
			DeleteDecision(decision.ID);
		}

		/// <summary>
		/// Delete a decision.
		/// </summary>
		/// <param name="decisionID">the ID of the decision</param>
		public void DeleteDecision(long decisionID)
		{
			SMDecisionDao.Delete(decisionID);
		}
		#endregion

		#region SMMessage
		/// <summary>
		/// Create a message.
		/// </summary>
		/// <param name="message">the message to create</param>
		/// <returns>the new message</returns>
		public void CreateMessage(SMMessage message)
		{
			SMMessageDao.Create(message);
		}

		/// <summary>
		/// Update a message.
		/// </summary>
		/// <param name="message">the message to update</param>
		/// <returns>the updated message</returns>
		public void UpdateMessage(SMMessage message)
		{
            SMMessageDao.Update(message);
		}

		/// <summary>
		/// Get a specific message.
		/// </summary>
		/// <param name="messageID">the ID of the message</param>
		/// <returns>the specified message, or null if it doesn't exist</returns>
		public SMMessage RetrieveMessage(long messageID)
		{
			return SMMessageDao.Retrieve(messageID);
		}

		/// <summary>
		/// Get a specific message.
		/// </summary>
		/// <param name="stateID">the ID of the associated state</param>
		/// <returns>the specified message, or null if it doesn't exist</returns>
		public SMMessage RetrieveMessageByStateID(long stateID)
		{
			return SMMessageDao.RetrieveByStateID(stateID);
		}

		/// <summary>
		/// Delete a message.
		/// </summary>
		/// <param name="message">message to be deleted</param>
		public void DeleteMessage(SMMessage message)
		{
            DeleteMessage(message.ID);
		}

		/// <summary>
		/// Delete a message.
		/// </summary>
		/// <param name="messageID">the ID of the message</param>
		public void DeleteMessage(long messageID)
		{
			SMMessageDao.Delete(messageID);
		}
		#endregion

		#region SMConcept
		/// <summary>
		/// Create a concept.
		/// </summary>
		/// <param name="concept">the concept to create</param>
		/// <returns>the new concept</returns>
		public void CreateConcept(SMConcept concept)
		{
			SMConceptDao.Create(concept);
		}

		/// <summary>
		/// Update a concept.
		/// </summary>
		/// <param name="concept">the concept to update</param>
		/// <returns>the updated concept</returns>
		public void UpdateConcept(SMConcept concept)
		{
			SMConceptDao.Update(concept);
		}

		/// <summary>
		/// Get a specific concept.
		/// </summary>
		/// <param name="conceptID">the ID of the concept</param>
		/// <returns>the specified concept, or null if it doesn't exist</returns>
		public SMConcept RetrieveConcept(long conceptID)
		{
			return SMConceptDao.Retrieve(conceptID);
		}

		/// <summary>
		/// Get a specific concept by name.
		/// </summary>
		/// <param name="surveyID">the ID of the associated survey</param>
		/// <param name="languageID">the ID of the associated language</param>
		/// <param name="name">the name of the concept</param>
		/// <returns>the specified concept, or null if it doesn't exist</returns>
		public SMConcept RetrieveConceptByName(long surveyID, long languageID, string name)
		{
			return SMConceptDao.Retrieve(surveyID, languageID, name);
		}

		/// <summary>
		/// Get list of concepts for a specific survey.
		/// </summary>
		/// <param name="surveyID">the ID of the associated survey</param>
		/// <param name="languageID">the ID of the associated language</param>
		/// <returns>the list of concepts for the specified survey</returns>
		public List<SMConcept> RetrieveConcepts(long surveyID, long languageID)
		{
            return SMConceptDao.RetrieveAll(surveyID, languageID) ?? new List<SMConcept>();
		}

		public SMConcept SelectConcept(long surveyID, long languageID, long respondentID, List<string> desiredConceptNames)
		{
            const string methodName = "SelectConcept";

            List<SMConcept> desiredConcepts = SMConceptDao.RetrieveAll(surveyID, languageID, desiredConceptNames);
            if (desiredConcepts == null || desiredConcepts.Count < 1)
            {
                string msg = "No desired concepts were found.";
                _logger.Error(_className, methodName, msg);
                return null;
            }

            // check constraints to filter desired concepts to candidate concepts
            List<SMConcept> candidateConcepts = new List<SMConcept>();
            foreach (SMConcept desiredConcept in desiredConcepts)
            {
                if (desiredConcept.CanViewConcept(this.Config, respondentID))
                {
                    candidateConcepts.Add(desiredConcept);
                }
            }

            // now we can pick one of the candidate concepts at random
            SMConcept selectedConcept = null;
            if (candidateConcepts.Count > 0)
            {
                int index = random.Next(0, candidateConcepts.Count);
                selectedConcept = candidateConcepts[index];
            }

            return selectedConcept;
		}

		/// <summary>
		/// Delete a concept.
		/// </summary>
		/// <param name="concept">concept to be deleted</param>
		public void DeleteConcept(SMConcept concept)
		{
			DeleteConcept(concept.ID);
		}

		/// <summary>
		/// Delete a concept.
		/// </summary>
		/// <param name="conceptID">the ID of the concept</param>
		public void DeleteConcept(long conceptID)
		{
			DeleteConceptViewsByConcept(conceptID);
			SMConceptDao.Delete(conceptID);
		}
		#endregion

		#region SMConceptView
		/// <summary>
		/// Create a conceptView.
		/// </summary>
		/// <param name="conceptView">the conceptView to create</param>
		/// <returns>the new conceptView</returns>
		public void CreateConceptView(SMConceptView conceptView)
		{
			SMConceptViewDao.Create(conceptView);
		}

		/// <summary>
		/// Update a conceptView.
		/// </summary>
		/// <param name="conceptView">the conceptView to update</param>
		/// <returns>the updated conceptView</returns>
		public void UpdateConceptView(SMConceptView conceptView)
		{
			SMConceptViewDao.Update(conceptView);
		}

		/// <summary>
		/// Get a specific conceptView.
		/// </summary>
		/// <param name="conceptViewID">the ID of the conceptView</param>
		/// <returns>the specified conceptView, or null if it doesn't exist</returns>
		public SMConceptView RetrieveConceptView(long conceptViewID)
		{
			return SMConceptViewDao.Retrieve(conceptViewID);
		}

		/// <summary>
		/// Get list of all conceptViews.
		/// </summary>
		/// <returns>the list of conceptViews</returns>
		public List<SMConceptView> RetrieveConceptViews()
		{
            return SMConceptViewDao.RetrieveAll() ?? new List<SMConceptView>();
		}

		/// <summary>
		/// Get list of all ConceptViews for a particular Concept.
		/// </summary>
		/// <param name="conceptID">the ID of the associated concept</param>
		/// <returns>the list of conceptViews for the specified concept</returns>
		public List<SMConceptView> RetrieveConceptViewsByConcept(long conceptID)
		{
            return SMConceptViewDao.RetrieveAllByConcept(conceptID) ?? new List<SMConceptView>();
		}

		/// <summary>
		/// Get list of all ConceptViews for a particular Respondent.
		/// </summary>
		/// <param name="respondentID">the ID of the associated respondent</param>
		/// <returns>the list of conceptViews for the specified respondent</returns>
		public List<SMConceptView> RetrieveConceptViewsByRespondent(long respondentID)
		{
            return SMConceptViewDao.RetrieveAllByRespondent(respondentID) ?? new List<SMConceptView>();
		}

		/// <summary>
		/// Get list of all ConceptViews for a particular Respondent and State.
		/// </summary>
		/// <param name="respondentID">the ID of the associated respondent</param>
		/// <param name="stateID">the ID of the associated state</param>
		/// <returns>the list of conceptViews for the specified respondent and state</returns>
		public List<SMConceptView> RetrieveConceptViewsByRespondentAndState(long respondentID, long stateID)
		{
            return SMConceptViewDao.RetrieveAllByRespondentAndState(respondentID, stateID) ?? new List<SMConceptView>();
		}

		public long RetrieveConceptViewsForSegment(long conceptID, string respondentPropName, string respondentPropValue)
		{
			return SMConceptViewDao.RetrieveCountBySegment(conceptID, respondentPropName, respondentPropValue);
		}

		public long RetrieveConceptViewsForGroup(long respondentID, string groupName)
		{
			return SMConceptViewDao.RetrieveCountByGroup(respondentID, groupName);
		}

		/// <summary>
		/// Delete a conceptView.
		/// </summary>
		/// <param name="conceptView">conceptView to be deleted</param>
		public void DeleteConceptView(SMConceptView conceptView)
		{
			DeleteConceptView(conceptView.ID);
		}

		/// <summary>
		/// Delete a conceptView.
		/// </summary>
		/// <param name="conceptID">the ID of the conceptView</param>
		public void DeleteConceptView(long conceptViewID)
		{
			SMConceptViewDao.Delete(conceptViewID);
		}

		/// <summary>
		/// Delete the conceptViews for a particular concept.
		/// </summary>
		/// <param name="conceptID">the ID of the concept</param>
		public void DeleteConceptViewsByConcept(long conceptID)
		{
            List<SMConceptView> conceptViews = RetrieveConceptViewsByConcept(conceptID);
            foreach (SMConceptView conceptView in conceptViews)
            {
                SMConceptViewDao.Delete(conceptView.ID);
            }
		}

		/// <summary>
		/// Delete the conceptViews for a particular respondent.
		/// </summary>
		/// <param name="conceptID">the ID of the concept</param>
		public void DeleteConceptViewsByRespondent(long respondentID)
		{
            List<SMConceptView> conceptViews = RetrieveConceptViewsByRespondent(respondentID);
            foreach (SMConceptView conceptView in conceptViews)
            {
                SMConceptViewDao.Delete(conceptView.ID);
            }
		}

		/// <summary>
		/// Delete the conceptViews for a survey/language
		/// </summary>
		/// <param name="surveyID">ID of the survey</param>
		/// <param name="languageID">ID of the language (optional)</param>
		/// <returns>number of rows deleted</returns>
		public int DeleteConceptViewsForSurvey(long surveyID, long languageID = -1)
		{
            List<long> conceptViewIDs = SMConceptViewDao.RetrieveIDs(surveyID, languageID);

            int rowsDeleted = 0;
            int curIndex = 0;
            while (curIndex < conceptViewIDs.Count)
            {
                List<long> batch = new List<long>();
                for (int index = curIndex; index < Math.Min(1000, conceptViewIDs.Count - curIndex); index++)
                {
                    batch.Add(conceptViewIDs[index]);
                }
                rowsDeleted += SMConceptViewDao.Delete(batch);
                curIndex += batch.Count;
            }
            return rowsDeleted;
		}
		#endregion

		#region SMCultureMap
		/// <summary>
		/// Create a cultureMap.
		/// </summary>
		/// <param name="cultureMap">the cultureMap to create</param>
		/// <returns>the new cultureMap</returns>
		public void CreateCultureMap(SMCultureMap cultureMap)
		{
			SMCultureMapDao.Create(cultureMap);
		}

		/// <summary>
		/// Update a cultureMap.
		/// </summary>
		/// <param name="cultureMap">the cultureMap to update</param>
		/// <returns>the updated cultureMap</returns>
		public void UpdateCultureMap(SMCultureMap cultureMap)
		{
			SMCultureMapDao.Update(cultureMap);
		}

		/// <summary>
		/// Get a specific cultureMap.
		/// </summary>
		/// <param name="cultureMapID">the ID of the cultureMap</param>
		/// <returns>the specified cultureMap, or null if it doesn't exist</returns>
		public SMCultureMap RetrieveCultureMap(long cultureMapID)
		{
            SMCultureMap result = null;
            if (_useAppCache)
            {
                lock (_appCacheLock)
                {
                    foreach (SMCultureMap cultureMap in _cultureMaps)
                    {
                        if (cultureMap.ID == cultureMapID)
                        {
                            result = cultureMap;
                            break;
                        }
                    }
                }
            }
            else
            {
                result = SMCultureMapDao.Retrieve(cultureMapID);
            }
            return result;
		}

		/// <summary>
		/// Get a specific cultureMap.
		/// </summary>
		/// <param name="culture">culture (e.g., "en-US")</param>
		/// <returns>specified cultureMap, or null if it doesn't exist</returns>
		public SMCultureMap RetrieveCultureMap(string culture)
		{
            SMCultureMap result = null;
            if (_useAppCache)
            {
                lock (_appCacheLock)
                {
                    foreach (SMCultureMap cultureMap in _cultureMaps)
                    {
                        if (cultureMap.Culture == culture)
                        {
                            result = cultureMap;
                            break;
                        }
                    }
                }
            }
            else
            {
                result = SMCultureMapDao.RetrieveByCulture(culture);
            }
            return result;
		}

		/// <summary>
		/// Get all cultureMaps.
		/// </summary>
		/// <returns>list of CultureMap</returns>
		public List<SMCultureMap> RetrieveCultureMaps()
		{
            List<SMCultureMap> result = null;
            if (_useAppCache)
            {
                lock (_appCacheLock)
                {
                    result = new List<SMCultureMap>(_cultureMaps);
                }
            }
            else
            {
                result = SMCultureMapDao.RetrieveAll() ?? new List<SMCultureMap>();
            }
            return result;
		}

		/// <summary>
		/// Delete a cultureMap.
		/// </summary>
		/// <param name="cultureMap">cultureMap to be deleted</param>
		public void DeleteCultureMap(SMCultureMap cultureMap)
		{
			DeleteCultureMap(cultureMap.ID);
		}

		/// <summary>
		/// Delete a cultureMap.
		/// </summary>
		/// <param name="cultureMapID">the ID of the cultureMap</param>
		public void DeleteCultureMap(long cultureMapID)
		{
			SMCultureMapDao.Delete(cultureMapID);
		}
		#endregion

		#region SMLanguage
		/// <summary>
		/// Create a language.
		/// </summary>
		/// <param name="language">the language to create</param>
		/// <returns>the new language</returns>
		public void CreateLanguage(SMLanguage language)
		{
            SMLanguageDao.Create(language);
            CacheManager.Update(CacheRegions.SurveyLanguageById, language.ID, language);
            CacheManager.Update(CacheRegions.SurveyLanguageByDescription, language.Description, language);
		}

		/// <summary>
		/// Update a language.
		/// </summary>
		/// <param name="language">the language to update</param>
		/// <returns>the updated language</returns>
		public void UpdateLanguage(SMLanguage language)
		{
            SMLanguageDao.Update(language);
            CacheManager.Update(CacheRegions.SurveyLanguageById, language.ID, language);
            CacheManager.Update(CacheRegions.SurveyLanguageByDescription, language.Description, language);
		}

		/// <summary>
		/// Get a specific language.
		/// </summary>
		/// <param name="languageID">the ID of the language</param>
		/// <returns>the specified language, or null if it doesn't exist</returns>
		public SMLanguage RetrieveLanguage(long languageID)
		{
            SMLanguage result = (SMLanguage)CacheManager.Get(CacheRegions.SurveyLanguageById, languageID);
            if (result == null)
            {
                result = SMLanguageDao.Retrieve(languageID);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.SurveyLanguageById, result.ID, result);
                    CacheManager.Update(CacheRegions.SurveyLanguageByDescription, result.Description, result);
                }
            }
            return result;
		}

		/// <summary>
		/// Get a specific language.
		/// </summary>
		/// <param name="description">description (e.g., "English")</param>
		/// <returns>the specified language, or null if it doesn't exist</returns>
		public SMLanguage RetrieveLanguage(string description)
		{
            SMLanguage result = (SMLanguage)CacheManager.Get(CacheRegions.SurveyLanguageByDescription, description);
            if (result == null)
            {
                result = SMLanguageDao.RetrieveByDescription(description);
                if (result != null)
                {
                    CacheManager.Update(CacheRegions.SurveyLanguageById, result.ID, result);
                    CacheManager.Update(CacheRegions.SurveyLanguageByDescription, result.Description, result);
                }
            }
            return result;
		}

		/// <summary>
		/// Get all languages.
		/// </summary>
		/// <returns>list of SMLanguage</returns>
		public List<SMLanguage> RetrieveLanguages()
		{
            if (_languages != null && _languages.Count > 0) return _languages;

            _languages = (List<SMLanguage>)SMLanguageDao.RetrieveAll();

            List<SMLanguage> result = _languages;
            if (result == null)
            {
                result = new List<SMLanguage>();
            }
            else if (result.Count > 0)
            {
                foreach (SMLanguage language in result)
                {
                    CacheManager.Update(CacheRegions.SurveyLanguageById, language.ID, language);
                    CacheManager.Update(CacheRegions.SurveyLanguageByDescription, language.Description, language);
                }
            }
            return result;
		}

		/// <summary>
		/// Delete a language.
		/// </summary>
		/// <param name="language">language to be deleted</param>
		public void DeleteLanguage(SMLanguage language)
		{
            CacheManager.Remove(CacheRegions.SurveyLanguageById, language.ID);
            CacheManager.Remove(CacheRegions.SurveyLanguageByDescription, language.Description);
            SMLanguageDao.Delete(language.ID);
		}

		/// <summary>
		/// Delete a language.
		/// </summary>
		/// <param name="languageID">the ID of the language</param>
		public void DeleteLanguage(long languageID)
		{
            SMLanguage language = RetrieveLanguage(languageID);
            if (language != null)
            {
                DeleteLanguage(language);
            }
		}
		#endregion

		#region SMFieldList
		public void CreateFieldList(SMFieldList fieldList)
		{
			SMFieldListDao.Create(fieldList);
		}

		public void UpdateFieldList(SMFieldList fieldList)
		{
            SMFieldListDao.Update(fieldList);
		}

		public SMFieldList RetrieveFieldList(long fieldListID)
		{
			return SMFieldListDao.Retrieve(fieldListID);
		}

		public SMFieldList RetrieveFieldListByName(string fieldListName)
		{
			return SMFieldListDao.Retrieve(fieldListName);
		}

		public List<SMFieldList> RetrieveAllFieldLists()
		{
            return SMFieldListDao.RetrieveAll() ?? new List<SMFieldList>();
		}

		public void DeleteFieldList(SMFieldList fieldList)
		{
			DeleteFieldList(fieldList.ID);
		}

		public void DeleteFieldList(long fieldListID)
		{
			SMFieldListDao.Delete(fieldListID);
		}
		#endregion

		#region SMRespondentList
		public void CreateRespondentList(SMRespondentList respondentList)
		{
			SMRespondentListDao.Create(respondentList);
		}

		public void UpdateRespondentList(SMRespondentList respondentList)
		{
			SMRespondentListDao.Update(respondentList);
		}

		public SMRespondentList RetrieveRespondentList(long respondentListID)
		{
			return SMRespondentListDao.Retrieve(respondentListID);
		}

		public SMRespondentList RetrieveRespondentListByBatchID(string batchID)
		{
			return SMRespondentListDao.RetrieveByBatchID(batchID);
		}

		public List<SMRespondentList> RetrieveAllRespondentLists()
		{
            return SMRespondentListDao.RetrieveAll() ?? new List<SMRespondentList>();
		}

		public void DeleteRespondentList(SMRespondentList respondentList)
		{
			DeleteRespondentList(respondentList.ID);
		}

		public void DeleteRespondentList(long respondentListID)
		{
			SMRespondentListDao.Delete(respondentListID);
		}
		#endregion

		#region SMRespListStage
		public bool IsRespondentListStaged(long respListID)
		{
			return SMRespListStageDao.IsRespondentListStaged(respListID);
		}

		public int DeleteStagedRespondentList(long respListID)
		{
			return SMRespListStageDao.DeleteStagedRespondentList(respListID);
		}

		public void StagedRespondentList2File(long respListID, string outputFileName, string connectionString)
		{
			SMRespListStageDao.StagedRespondentList2File(respListID, this.DatabaseType, outputFileName, connectionString);
		}

		public void File2StagedRespondentList(long respListID, string inputFileName, string connectionString)
		{
			SMRespListStageDao.File2StagedRespondentList(respListID, this.DatabaseType, inputFileName, connectionString);
		}
		#endregion
	}
}