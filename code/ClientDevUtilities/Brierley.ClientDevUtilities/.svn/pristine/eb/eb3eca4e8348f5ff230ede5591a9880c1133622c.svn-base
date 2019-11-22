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
    /// Used to determine if the user's responses contain specific values for a particular survey 
    /// question.  Method return boolean based on the rules specified by the Criteria argument.
    /// 
    /// The method arguments are as follows:
    /// QuestionName - a string that matches the question name in the current survey's state diagram.
    /// 
    /// Criteria - a string value from the following that determines the set criteria to use:
    ///    'Any' - true result if any of the responseValues are contained in the set of responses
    ///    'All' - true result if all of the responseValues are contained in the set of responses
    ///    'Exact' - true result if the entire set of responseValues exactly equals the set of responses
    /// 
    /// ResponseValue1[,...[,ResponseValueN]]] - (2..N optional) a list of values that indicates the 
    /// set of candidate responses to be matched to the actual user responses.
    /// 
    /// Since the responses are stored in a textual format in the database, this method attempts to 
    /// convert each response value as a DateTime, then as a real/integer value, then simply as the 
    /// string itself, before using it for comparison.
    /// </summary>
    /// <example>
    ///     Usage: SurveyResponseContains('QuestionName', 'Criteria', ResponseValue1[, ...[, ResponseValueN]]])
    /// </example>
    [Serializable]
	[ExpressionContext(Description = "Is the value of the user's response to a particular survey question within a defined set of values?",
		DisplayName = "SurveyResponseContains",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Booleans,
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Survey,
		WizardDescription = "SurveyResponseContains")]

	[ExpressionParameter(Order = 0, Name = "QuestionName", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What question?")]
	[ExpressionParameter(Order = 1, Name = "Match", Optional = false, Type = ExpressionApplications.Numbers, WizardDescription = "Match")]
	[ExpressionParameter(Order = 1, Name = "Responses", Optional = false, Type = ExpressionApplications.Numbers, WizardDescription = "Response set")]

    public class SurveyResponseContains : UnaryOperation
    {
        public enum CriteriaEnum { All = 0, Any, Exact };
        private const string _className = "SurveyResponseContains";
        private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
        private Expression _questionName = null;
        private Expression _quesitonCriteria = null;
        private CriteriaEnum _criteria = CriteriaEnum.All;
        private List<Expression> _responseExp = new List<Expression>();
        private object[] _responseValues = null;
        private string questionName = null;
        int numResponseValues = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public SurveyResponseContains()
        {
        }

        /// <summary>
        /// Constructor used internally.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public SurveyResponseContains(Expression rhs)
            : base("SurveyResponseContains", rhs)
        {
            string methodName = "SurveyResponseContains";

            if (rhs == null)
            {
                string msg = "Invalid Function Call: No arguments passed to SurveyResponseContains.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            if (!(rhs is ParameterList))
            {
                string msg = "Invalid Function Call: Invalid argument passed to SurveyResponseContains constructor.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs < 3)
            {
                string msg = "Invalid Function Call: Wrong number of arguments passed to SurveyResponseContains.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            _questionName = ((ParameterList)rhs).Expressions[0];
            _quesitonCriteria = ((ParameterList)rhs).Expressions[1];

            // ResponseValue1..ResponseValueN
            numResponseValues = ((ParameterList)rhs).Expressions.Length - 2;
            for (int exprIndex = 2; exprIndex < (numResponseValues + 2); exprIndex++)
            {
                _responseExp.Add(((ParameterList)rhs).Expressions[exprIndex]);
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "SurveyResponseContains('QuestionName', {'Any' | 'All' | 'Exact'}, 'ResponseValue1'[, ...[, 'ResponseValueN']]])";
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
                    throw new Exception("Argument QuestionName evaluates to null or empty");
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg QuestionName", ex);
                throw new CRMException("SurveyResponseContains: Problem with arg QuestionName");
            }

            // Criteria
            object obj = null;
            try
            {
                obj = _quesitonCriteria.evaluate(contextObject);
                if (obj == null)
                    throw new Exception("Argument Criteria evaluates to null");
                if (!(obj is StringConstant || obj is string))
                    throw new Exception("Argument Criteria does not evaluate to a StringConstant");

                _criteria = (CriteriaEnum)Enum.Parse(typeof(CriteriaEnum), obj.ToString());
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg Criteria", ex);
                throw new CRMException("SurveyResponseContains: Problem with arg Criteria");
            }

            // ResponseValue1..ResponseValueN
            _responseValues = new object[_responseExp.Count];
            int respIdx = 0;
            foreach (Expression responseExp in _responseExp)
            {
                try
                {
                    _responseValues[respIdx] = responseExp.evaluate(contextObject);
                    respIdx++;
                }
                catch (Exception ex)
                {
                    _logger.Error(_className, methodName, "Exception evaluating arg ResponseValue[" + respIdx + "]", ex);
                }
            }
           
            try
            {
                string msg = "Evaluating: SurveyResponseContains(" + questionName + ","
                    + Enum.GetName(typeof(CriteriaEnum), _criteria);
                for (int i = 0; i < _responseValues.Length; i++)
                {
                    msg += "," + _responseValues[i].ToString();
                }
                msg += ")";
                _logger.Debug(_className, methodName, msg);
            }
            catch { }

            // Get the environment properties passed in the context object
			if (contextObject.Environment == null || !(contextObject.Environment is Dictionary<string, object>))
            {
                string msg = "SurveyResponseContains: No environment was passed.";
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

				bool result = DoContains(surveyManager, question, languageID, questionContent, respondentID);

				// Log the result
				try
				{
					string msg = "SurveyResponseContains(" + questionName + ","
						+ Enum.GetName(typeof(CriteriaEnum), _criteria);
					for (int i = 0; i < _responseValues.Length; i++)
					{
						msg += "," + _responseValues[i].ToString();
					}
					msg += ") -> " + result;
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

		private SMQuestion GetQuestion(SurveyManager surveyManager, long surveyID)
		{
			const string methodName = "GetQuestion";
			using (var svc = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMQuestion question = svc.RetrieveQuestionByStateName(surveyID, questionName);
				if (question == null)
				{
					string msg = string.Format("SurveyResponseContains: question named '{0}' not found for surveyID {1}.", questionName, surveyID);
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}
				return question;
			}
		}

        private SMQuestionContent GetQuestionContent(SurveyManager surveyManger, SMQuestion question, long languageID)
        {
            const string methodName = "GetQuestionContents";

            List<SMQuestionContent> questionContents = surveyManger.RetrieveQuestionContents(question.ID, languageID, QuestionContentType.BODY_TEXT);
            if (questionContents == null)
            {
                string msg = string.Format("SurveyResponseContains: question named '{0}' has null content.", questionName);
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            else if (questionContents.Count < 1)
            {
                string msg = string.Format("SurveyResponseContains: question named '{0}' has no content.", questionName);
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            SMQuestionContent questionContent = questionContents[0];
            return questionContent;
        }

        private bool DoContains(SurveyManager surveyManger,SMQuestion question, long languageID, SMQuestionContent questionContent, long respondentID)
        {
            List<SMResponse> responses = surveyManger.RetrieveResponses(respondentID, questionContent.ID);

            // No responses, so can't be any matches
            if (responses == null || responses.Count < 1) return false;

            // Set size is different, so can't be an exact match
            if (_criteria == CriteriaEnum.Exact && _responseValues.Length != responses.Count) return false;

            // Preprocess
            object[] userResponses = new object[responses.Count];
            for (int i = 0; i < responses.Count; i++)
            {
                object userResponseValue = ResolveUserResponseValue(surveyManger, question, languageID, responses[i]);
                userResponses[i] = userResponseValue;
            }

            // Find matches
            int numMatches = 0;
            foreach (object responseValue in _responseValues)
            {
                object argResponseValue = ResolveType(responseValue);
                foreach (object userResponseValue in userResponses)
                {
                    if (IsMatch(argResponseValue, userResponseValue))
                    {
                        numMatches++;

                        // 'Any' only needs one match
                        if (_criteria == CriteriaEnum.Any) return true;
                    }
                }
            }
            // 'Any' must have at least one match
            if (_criteria == CriteriaEnum.Any && numMatches < 1) return false;

            // 'All' must have >= the number of matches as the _responseValues
            if (_criteria == CriteriaEnum.All && numMatches < _responseValues.Length) return false;

            // 'Exact' must have the same number of matches as the _responseValues
            if (_criteria == CriteriaEnum.Exact && numMatches != _responseValues.Length) return false;

            return true;
        }

        private object ResolveUserResponseValue(SurveyManager surveyManger, SMQuestion question, long languageID, SMResponse response)
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

            object result = ResolveType(responseValue);
            return result;
        }

        private object ResolveType(object value)
        {
            object result = value;
            string stringValue = value.ToString();
            DateTime dateTimeValue;
            double doubleValue;
            decimal decimalValue;
            long longValue;
            if (DateTime.TryParse(stringValue, out dateTimeValue))
            {
                result = (object)dateTimeValue;
            }
            else if (double.TryParse(stringValue, out doubleValue))
            {
                result = (object)doubleValue;
            }
            else if (decimal.TryParse(stringValue, out decimalValue))
            {
                result = (object)decimalValue;
            }
            if (long.TryParse(stringValue, out longValue))
            {
                result = (object)longValue;
            }
            else
            {
                result = (object)value;
            }
            return result;
        }

        private bool IsMatch(object lhs, object rhs)
        {
            string methodName = "test";

            Type lhsType = lhs.GetType();
            Type rhsType = rhs.GetType();

            if (lhsType == typeof(DateTime) && rhsType == typeof(DateTime))
            {
                if ((DateTime)lhs == ((DateTime)rhs))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    if ( (lhsType == typeof(Double) || lhsType == typeof(Decimal) || lhsType == typeof(Int16) || lhsType == typeof(Int32) || lhsType == typeof(Int64)) &&
                        (rhsType == typeof(Double) || rhsType == typeof(Decimal) || rhsType == typeof(Int16) || rhsType == typeof(Int32) || rhsType == typeof(Int64)))
                    {
                        double left = System.Double.Parse(lhs.ToString());
                        double right = System.Double.Parse(rhs.ToString());
                        if (left == right)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string msg = string.Format(@"Encountered an unsupported data type while evaluating SurveyResponseContains operator.  
                        Left side data type = {0} has value {1} - 
                        Right side datatype = {2} has value {3}.",
                     lhs.GetType().ToString(),
                     lhs.ToString(),
                     rhs.GetType().ToString(),
                     rhs.ToString());
                    FormatException ex1 = new FormatException(msg, ex);
                    _logger.Error(_className, methodName, msg, ex1);
                    throw ex1;
                }
            }
            if (lhs.ToString().Equals(rhs.ToString())) return true;
            return false;
        }
    }
}
