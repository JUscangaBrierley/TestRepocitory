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
    /// QuestionIndex - (optional) an integer value that indicates the desired zero-based index of the 
    /// question for matrix questions, and that is ignored for non-matrix questions.  If this argument 
    /// is omitted, then index value of zero will be assumed.
    /// 
    /// Since the responses are stored in a textual format in the database, this method attempts to 
    /// parse the response value as a DateTime, then as a real/integer value, then simply as the 
    /// string itself.  The parsed value is then returned by the method as the appropriate type.
    /// </summary>
    /// <example>
    ///     Usage: SurveyResponse('QuestionName'[,QuestionIndex])
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Returns the value of the user's response to a particular survey question.",
        DisplayName = "SurveyResponse",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Survey,
        ExpressionReturns = ExpressionApplications.Objects,
        AdvancedWizard = true,
        WizardCategory = WizardCategories.Survey,
        WizardDescription = "SurveyResponse"
        )]

    [ExpressionParameter(Order = 0, Name = "QuestionName", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What question?")]
    [ExpressionParameter(Order = 1, Name = "QuestionIndex", Optional = false, Type = ExpressionApplications.Numbers, WizardDescription = "What index?")]
    public class SurveyResponse : UnaryOperation
    {
        private const string _className = "SurveyResponse";
        private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
        private Expression _questionName = null;
        private Expression _questionIndex = null;
        private long questionIndex = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public SurveyResponse()
        {
        }

        /// <summary>
        /// Constructor used internally.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public SurveyResponse(Expression rhs)
            : base("SurveyResponse", rhs)
        {
            string methodName = "SurveyResponse";

            if (rhs == null)
            {
                string msg = "Invalid Function Call: No arguments passed to SurveyResponse.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            if (rhs is StringConstant)
            {
                _questionName = (StringConstant)rhs;
            }
            else if (rhs is ParameterList)
            {
                int numArgs = ((ParameterList)rhs).Expressions.Length;
                if (numArgs >= 1 && numArgs <= 2)
                {
                    ContextObject contextObject = new ContextObject();
                    try
                    {
                        _questionName = ((ParameterList)rhs).Expressions[0];
                        if (_questionName == null)
                        {
                            string msg = "Argument QuestionName evaluates to null or empty.";
                            _logger.Error(_className, methodName, msg);
                            throw new Exception(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(_className, methodName, "Exception evaluating arg QuestionName", ex);
                        throw new CRMException("SurveyResponse: Problem with arg QuestionName");
                    }
                    if (numArgs >= 2)
                    {
                        try
                        {
                            _questionIndex = ((ParameterList)rhs).Expressions[1];
                            if (_questionIndex == null)
                            {
                                string msg = "Argument QuestionIndex evaluates to null.";
                                _logger.Error(_className, methodName, msg);
                                throw new Exception(msg);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(_className, methodName, "Exception evaluating arg QuestionIndex", ex);
                        }
                    }
                }
                else
                {
                    string msg = "Invalid Function Call: Wrong number of arguments passed to SurveyResponse.";
                    _logger.Error(_className, methodName, msg);
                    throw new CRMException(msg);
                }
            }
            else
            {
                string msg = "Invalid Function Call: Unknown argument type passed to SurveyResponse.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "SurveyResponse('QuestionName', QuestionIndex)";
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

            string questionName = _questionName.evaluate(contextObject).ToString();
            object obj = _questionIndex.evaluate(contextObject);

            if (obj is long || obj is int)
            {
                questionIndex = (long)obj;
            }
            else if (obj is double)
            {
                questionIndex = (long)(double)obj;
            }
            else if (obj is decimal)
            {
                questionIndex = Decimal.ToInt64((decimal)obj);
            }
            else
            {
                _logger.Error(_className, methodName, "Problem with arg QuestionIndex, obj = " + obj);
                throw new CRMException("SurveyResponse: Problem with arg QuestionIndex");
            }

            try
            {
                string msg = "Evaluating: SurveyResponse(" + questionName + "," + questionIndex + ")";
                _logger.Debug(_className, methodName, msg);
            }
            catch { }

            // Get the environment properties passed in the context object
            if (contextObject.Environment == null || !(contextObject.Environment is Dictionary<string, object>))
            {
                string msg = "SurveyResponse: No environment was passed.";
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
				SMQuestion question = GetQuestion(surveyManager, surveyID, questionName);
				SMQuestionContent questionContent = GetQuestionContent(surveyManager, question, languageID, QuestionContentType.BODY_TEXT, questionName);
				SMQuestionContent otherSpecifyQuestionContent = null;
				if (question.HasOtherSpecify)
				{
					otherSpecifyQuestionContent = GetQuestionContent(surveyManager, question, languageID, QuestionContentType.OTHER_SPECIFY_TEXT, questionName);
				}


				object result = (object)string.Empty;
				List<SMResponse> responses = new List<SMResponse>();
				responses.AddRange(surveyManager.RetrieveResponses(respondentID, questionContent.ID));
				if (otherSpecifyQuestionContent != null)
				{
					responses.AddRange(surveyManager.RetrieveResponses(respondentID, otherSpecifyQuestionContent.ID));
				}

				if (responses.Count > 0)
				{
					result = ResolveResponseType(surveyManager, question, languageID, responses);
				}

				// Log the result
				try
				{
					string msg = "SurveyResponse(" + questionName + "," + questionIndex + ") -> " + result;
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

        private SMQuestion GetQuestion(SurveyManager surveyManger, long surveyID, string questionName)
        {
            const string methodName = "GetQuestion";

            SMQuestion question = surveyManger.RetrieveQuestionByStateName(surveyID, questionName);
            if (question == null)
            {
                string msg = string.Format("SurveyResponse: question named '{0}' not found for surveyID {1}.", questionName, surveyID);
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            return question;
        }

        private SMQuestionContent GetQuestionContent(SurveyManager surveyManger, SMQuestion question, long languageID, QuestionContentType questionContentType, string questionName)
        {
            const string methodName = "GetQuestionContent";

            List<SMQuestionContent> questionContents = surveyManger.RetrieveQuestionContents(question.ID, languageID, questionContentType);
            if (questionContents == null)
            {
                string msg = string.Format("SurveyResponse: question named '{0}' has null content.", questionName);
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            else if (questionContents.Count < 1)
            {
                string msg = string.Format("SurveyResponse: question named '{0}' has no content.", questionName);
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            else if (questionContents.Count <= questionIndex)
            {
                string msg = string.Format("SurveyResponse: question named '{0}' has no content for index {1}.", questionName, questionIndex);
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            SMQuestionContent questionContent = questionContents[(int)questionIndex];
            return questionContent;
        }

        private object ResolveResponseType(SurveyManager surveyManger, SMQuestion question, long languageID, List<SMResponse> responses)
        {
            List<SMAnswerContent> answerContents = surveyManger.RetrieveAnswerContents(question.ID, languageID);
            List<SMQuestionContent> anchors = null;
            if (question.IsMatrix)
            {
                anchors = surveyManger.RetrieveQuestionContents(question.ID, languageID, QuestionContentType.ANCHOR_TEXT);
            }

            string cumulativeResponseValue = string.Empty;
            foreach (var response in responses)
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

                if (question.IsMatrix)
                {
                    // responseValue may be the point scale value (1..n), so translate to anchor text
                    try
                    {
                        long pointValue = StringUtils.FriendlyInt64(responseValue);
                        if (anchors != null && anchors.Count > 0 && pointValue > 0 && pointValue <= (int)question.MatrixPointScale)
                        {
                            // see if there is an anchor text for this
                            foreach (SMQuestionContent anchor in anchors)
                            {
                                if ((anchor.MatrixIndex + 1) == pointValue)
                                {
                                    if (!string.IsNullOrEmpty(anchor.Content) && anchor.Content != responseValue)
                                    {
                                        responseValue = anchor.Content;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch { }
                }
                if (!string.IsNullOrWhiteSpace(cumulativeResponseValue))
                {
                    cumulativeResponseValue += ", ";
                }
                cumulativeResponseValue += responseValue;
            }

            // a multi-valued response will always be a string, otherwise see if it is another type
            object result = (object)string.Empty;
            DateTime dateTimeValue;
            double doubleValue;
            decimal decimalValue;
            long longValue;
            if (DateTime.TryParse(cumulativeResponseValue, out dateTimeValue))
            {
                result = (object)dateTimeValue;
            }
            else if (double.TryParse(cumulativeResponseValue, out doubleValue))
            {
                result = (object)doubleValue;
            }
            else if (decimal.TryParse(cumulativeResponseValue, out decimalValue))
            {
                result = (object)decimalValue;
            }
            if (long.TryParse(cumulativeResponseValue, out longValue))
            {
                result = (object)longValue;
            }
            else
            {
                result = (object)cumulativeResponseValue;
            }
            return result;
        }
    }
}
