using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using Brierley.FrameWork;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Used to create a clickable html link (anchor tag) to a survey.  Method returns a string which 
    /// is the html rendering of the link.
    /// 
    /// The method arguments are as follows:
    /// linkText - a string that will be embedded in the link.  E.g., "<a href="...">LinkText</a>"
    /// fieldName - a string that indicates the name of the template field which contains the encoded mtouch values
    /// pageName - a string that indicates the name of the page which contains the survey
    /// </summary>
    /// <example>
    ///     Usage: SurveyLink('linkText', 'fieldName'])
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Returns an HTML link to a survey.",
        DisplayName = "SurveyLink",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Survey,
        ExpressionReturns = ExpressionApplications.Strings,
        AdvancedWizard = true,
        WizardCategory = WizardCategories.Survey,
        WizardDescription = "Survey link")]

    [ExpressionParameter(Order = 0, Name = "LinkText", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "Link text")]
    [ExpressionParameter(Order = 1, Name = "MTouchField", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "MTouch field name")]
    [ExpressionParameter(Order = 2, Name = "PageName", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "Page name")]

    public class SurveyLink : UnaryOperation
    {
        private const string _className = "SurveyLink";
        private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
        private Expression _linkText = null;
        private Expression _fieldName = null;
        private Expression _pageName = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SurveyLink()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public SurveyLink(Expression rhs)
            : base("SurveyLink", rhs)
        {
            const string methodName = "SurveyLink";

            if (rhs == null)
            {
                string msg = "Invalid Function Call: No arguments passed to SurveyLink.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
            if (!(rhs is ParameterList))
            {
                string msg = "Invalid Function Call: Invalid argument passed to SurveyLink constructor.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs < 2 || numArgs > 3)
            {
                string msg = "Invalid Function Call: Wrong number of arguments passed to SurveyLink.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            _linkText = ((ParameterList)rhs).Expressions[0];

            if (numArgs > 1)
            {
                _fieldName = ((ParameterList)rhs).Expressions[1];
            }
            if (numArgs > 2)
            {
                _pageName = ((ParameterList)rhs).Expressions[2];
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "SurveyLink('linkText', 'mtouchFieldName', 'pagename')";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">An instance of ContextObject</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            const string methodName = "SurveyLink";

            // linkText
            string linkText = null;
            try
            {
                linkText = _linkText.evaluate(contextObject).ToString();
                if (string.IsNullOrEmpty(linkText))
                    throw new Exception("Argument linkText evaluates to null or empty");
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg linkText", ex);
                throw new CRMException("SurveyLink: Problem with arg linkText");
            }

            // fieldName
            string fieldName = null;
            try
            {
                fieldName = _fieldName.evaluate(contextObject).ToString();
                if (string.IsNullOrEmpty(fieldName))
                    throw new Exception("Argument mtouchFieldName evaluates to null or empty");
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg fieldName", ex);
                throw new CRMException("SurveyLink: Problem with arg mtouchFieldName");
            }

            // pageName
            string pageName = null;
            try
            {
                pageName = _pageName.evaluate(contextObject).ToString();
                if (string.IsNullOrEmpty(pageName))
                    _pageName = null;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg pageName", ex);
                throw new CRMException("SurveyLink: Problem with arg pageName");
            }


            // Get configured survey URL
            string surveyURL = LWConfigurationUtil.GetConfigurationValue("LWSurveyURL");
            if (string.IsNullOrEmpty(surveyURL))
                throw new CRMException("LWSurveyURL property not defined.");

            // Add pagename if provided
            if (!string.IsNullOrEmpty(pageName))
            {
                if (!surveyURL.EndsWith("/")) surveyURL += "/";
                if (pageName.StartsWith("/")) pageName = pageName.Substring(1);
                surveyURL += pageName;
            }

            // Add mtouch query param, invalid url throws exception
            Uri url = new Uri(surveyURL);
            if (string.IsNullOrEmpty(url.Query))
                surveyURL += "?MTouch=";
            else if (!url.Query.ToLower().Contains("mtouch="))
                surveyURL += "MTouch=";

            surveyURL += "#%%#" + fieldName + "#%%#";

            string result = string.Format("<a href=\"{0}\">{1}</a>", surveyURL, linkText);
            return result;
        }
    }
}
