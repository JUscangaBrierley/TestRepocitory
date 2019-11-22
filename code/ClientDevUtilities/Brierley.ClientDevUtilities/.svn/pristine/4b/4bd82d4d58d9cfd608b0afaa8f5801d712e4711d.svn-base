using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Used to return the value of the user's response to a particular survey question.
    /// 
    /// The method arguments are as follows:
    /// QuestionName - a string that matches the question name in the current survey's state diagram.
    /// 
    /// AnswerName - a string that matches the desired answer name in the specified question.
    /// 
    /// Since the responses are stored in a textual format in the database, this method attempts to 
    /// parse the response value as a DateTime, then as a real/integer value, then simply as the 
    /// string itself.  The parsed value is then returned by the method as the appropriate type.
    /// </summary>
    /// <example>
    ///     Usage: SurveyResponseByAnswer('QuestionName','AnswerName')
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Returns the value of the user's response to a particular survey question.",
        DisplayName = "SurveyResponseByAnswer",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Survey,
        ExpressionReturns = ExpressionApplications.Objects,
        AdvancedWizard = true,
        WizardCategory = WizardCategories.Survey,
        WizardDescription = "SurveyResponseByAnswer"
    )]

    [ExpressionParameter(Order = 0, Name = "QuestionName", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What question?")]
    [ExpressionParameter(Order = 1, Name = "AnswerName", Optional = false, Type = ExpressionApplications.Numbers, WizardDescription = "What answer?")]

    public class SurveyResponseByAnswer : UnaryOperation
    {
        private const string _className = "SurveyResponseByAnswer";
        private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
        private Expression _questionName = null;
        private Expression _answerName = null;
        private string questionName = null;
        private string answerName = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public SurveyResponseByAnswer()
        {
        }

        /// <summary>
        /// Constructor used internally.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public SurveyResponseByAnswer(Expression rhs)
            : base("SurveyResponseByAnswer", rhs)
        {
            string methodName = "SurveyResponseByAnswer";

            if (rhs == null)
            {
                string msg = "Invalid Function Call: No arguments passed to SurveyResponseByAnswer.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            if (!(rhs is ParameterList))
            {
                string msg = "Invalid Function Call: Unknown argument type passed to SurveyResponseByAnswer.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs != 2)
            {
                string msg = "Invalid Function Call: Wrong number of arguments passed to SurveyResponseByAnswer.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            ContextObject contextObject = new ContextObject();

            _questionName = ((ParameterList)rhs).Expressions[0];
            _answerName = ((ParameterList)rhs).Expressions[1];
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "SurveyResponseByAnswer('QuestionName','AnswerName')";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">A context object used for evaluating argument expressions</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            const string methodName = "evaluate";

            // QuestionName
            try
            {
                questionName = _questionName.evaluate(contextObject).ToString();
                if (string.IsNullOrEmpty(questionName))
                {
                    string msg = "Argument QuestionName evaluates to null or empty.";
                    _logger.Error(_className, methodName, msg);
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg QuestionName", ex);
                throw new CRMException("SurveyResponseByAnswer: Problem with arg QuestionName");
            }

            // AnswerName
            try
            {
                answerName = _answerName.evaluate(contextObject).ToString();
                if (string.IsNullOrEmpty(answerName))
                {
                    string msg = "Argument AnswerName evaluates to null or empty.";
                    _logger.Error(_className, methodName, msg);
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg AnswerName", ex);
                throw new CRMException("SurveyResponseByAnswer: Problem with arg AnswerName");
            }

            try
            {
                string msg = "Evaluating: SurveyResponseByAnswer(" + questionName + "," + answerName + ")";
                _logger.Debug(_className, methodName, msg);
            }
            catch { }

            // Get the environment properties passed in the context object
            if (contextObject.Environment == null || !(contextObject.Environment is Dictionary<string, object>))
            {
                string msg = "SurveyResponseByAnswer: No environment was passed.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            var args = contextObject.Environment;
            long surveyID = -1;
            if (args.ContainsKey("surveyID")) surveyID = StringUtils.FriendlyInt64(args["surveyID"], -1);
            long respondentID = -1;
            if (args.ContainsKey("respondentID")) respondentID = StringUtils.FriendlyInt64(args["respondentID"], -1);
            long languageID = -1;
            if (args.ContainsKey("languageID")) languageID = StringUtils.FriendlyInt64(args["languageID"], -1);

			using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMQuestion question = GetQuestion(surveyManager, surveyID);
				SMQuestionContent questionContent = GetQuestionContent(surveyManager, question, languageID);
				SMAnswerContent answerContent = GetMatchingAnswerContent(surveyManager, question.ID, languageID);

				bool responseFound = false;
				object result = (object)string.Empty;
				SMResponse response = GetResponse(surveyManager, questionContent, respondentID, answerContent.ID);
				if (response != null)
				{
					result = ResolveResponseType(surveyManager, question, languageID, response);
					responseFound = true;
				}

				// Log the result
				try
				{
					string msg = "SurveyResponseByAnswer(" + questionName + "," + answerName + ") -> "
						+ (responseFound ? result : "<'' because no response found>");
					_logger.Debug(_className, methodName, msg);
				}
				catch { }

				return result;
			}
        }

        /// <summary>
        /// Parse the expression for meta data.  Used in LoyaltyNavigator to determine which attributes need
        /// to be provided in order to render a page during preview.
        /// </summary>
        /// <returns>semicolon separated list of attributes or empty string if no metadata</returns>
        public override string parseMetaData()
        {
            return string.Empty;
        }

        private SMQuestion GetQuestion(SurveyManager surveyManger, long surveyID)
        {
            const string methodName = "GetQuestion";

            SMQuestion question = surveyManger.RetrieveQuestionByStateName(surveyID, questionName);
            if (question == null)
            {
                string msg = string.Format("SurveyResponseByAnswer: question named '{0}' not found for surveyID {1}.", questionName, surveyID);
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            return question;
        }

        private SMQuestionContent GetQuestionContent(SurveyManager surveyManger, SMQuestion question, long languageID)
        {
            const string methodName = "GetQuestionContents";

            List<SMQuestionContent> questionContents = surveyManger.RetrieveQuestionContents(question.ID, languageID, QuestionContentType.BODY_TEXT);
            if (questionContents == null)
            {
                string msg = string.Format("SurveyResponseByAnswer: question named '{0}' has null content.", questionName);
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            else if (questionContents.Count < 1)
            {
                string msg = string.Format("SurveyResponseByAnswer: question named '{0}' has no content.", questionName);
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            SMQuestionContent questionContent = questionContents[0];
            return questionContent;
        }

        private SMAnswerContent GetMatchingAnswerContent(SurveyManager surveyManger, long questionID, long languageID)
        {
            const string methodName = "GetMatchingAnswerContent";
            List<SMAnswerContent> answerContents = surveyManger.RetrieveAnswerContents(questionID, languageID);
            foreach (SMAnswerContent answerContent in answerContents)
            {
                string thisAnswerName = StringUtils.DeHTML(StringUtils.FriendlyString(answerContent.Content));
                if (thisAnswerName == answerName)
                    return answerContent;
            }
            string msg = string.Format("SurveyResponseByAnswer: question named '{0}' has no answer with name '{1}'.", questionName, answerName);
            _logger.Error(_className, methodName, msg);
            throw new CRMException(msg);
        }

        private SMResponse GetResponse(SurveyManager surveyManger, SMQuestionContent questionContent, long respondentID, long answerContentID)
        {
            //const string methodName = "GetResponse";

            List<SMResponse> responses = surveyManger.RetrieveResponses(respondentID, questionContent.ID);
            if (responses == null || responses.Count < 1) return null;

            foreach (SMResponse response in responses)
            {
                if (response.AnswerContentID == answerContentID)
                    return response;
            }
            return null;
        }

        private object ResolveResponseType(SurveyManager surveyManger, SMQuestion question, long languageID, SMResponse response)
        {
            string responseValue = string.Empty;
            if (response.AnswerContentID == -1)
            {
                // Response is stored in the Response
                responseValue = response.Content;
            }
            else
            {
                // Response is stored as an AnswerContent ID
                List<SMAnswerContent> answerContents = surveyManger.RetrieveAnswerContents(question.ID, languageID);
                if (answerContents != null)
                {
                    foreach (SMAnswerContent answerContent in answerContents)
                    {
                        if (response.AnswerContentID == answerContent.ID)
                        {
                            responseValue = StringUtils.DeHTML(StringUtils.FriendlyString(answerContent.Content));
                            break;
                        }
                    }
                }
            }

            object result = (object)string.Empty;
            DateTime dateTimeValue;
            double doubleValue;
            decimal decimalValue;
            long longValue;
            if (DateTime.TryParse(responseValue, out dateTimeValue))
            {
                result = (object)dateTimeValue;
            }
            else if (double.TryParse(responseValue, out doubleValue))
            {
                result = (object)doubleValue;
            }
            else if (decimal.TryParse(responseValue, out decimalValue))
            {
                result = (object)decimalValue;
            }
            if (long.TryParse(responseValue, out longValue))
            {
                result = (object)longValue;
            }
            else
            {
                result = (object)responseValue;
            }
            return result;
        }
    }
}
