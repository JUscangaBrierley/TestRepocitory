using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
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
	/// Used to create a clickable html link (anchor tag) to a generic URL with an MTouch.  Method returns a string 
	/// which is the html rendering of the link.
	/// 
	/// The method arguments are as follows:
	/// baseURL - a string that is the base URL for the href part of the link.  E.g., "<a href="baseURL?MTouch=fieldName">LinkText</a>"
	/// linkText - a string that will be embedded in the link.
	/// mtouchFieldName - a string that indicates the name of the template field which contains the encoded mtouch values
	/// </summary>
	/// <example>
	///     Usage: MTouchLink('baseURL', 'linkText', 'mtouchFieldName'])
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns an HTML link to a survey.",
		DisplayName = "MTouchLink",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Strings, 
		
		AdvancedWizard = true, 
		WizardCategory = WizardCategories.Function, 
		WizardDescription = "MTouch link"
	)]

	[ExpressionParameter(Order = 0, Name = "BaseUrl", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "URL")]
	[ExpressionParameter(Order = 1, Name = "LinkText", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "Link Text")]
	[ExpressionParameter(Order = 2, Name = "FieldName", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "Field Name")]

    public class MTouchLink : UnaryOperation
    {
		private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
		private const string _className = "MTouchLink";
		private Expression _baseURL = null;
        private Expression _linkText = null;
        private Expression _mtouchFieldName = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MTouchLink()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public MTouchLink(Expression rhs)
			: base("MTouchLink", rhs)
        {
            // no arguments, so use defaults
            if (rhs == null)
            {
                return;
            }

            // single argument is expression for linkText
            if (!(rhs is ParameterList))
            {
                _linkText = rhs;
                return;
            }

            // multiple arguments are multiple parameters, so make sure 1..3 arguments are present
            int numArgs = ((ParameterList)rhs).Expressions.Length;
            if (numArgs < 1 || numArgs > 3)
            {
                string msg = "Invalid Function Call: Wrong number of arguments passed to MTouchLink.";
                _logger.Error(_className, "MTouchLink", msg);
                throw new CRMException(msg);
            }

            // linkText
            _linkText = ((ParameterList)rhs).Expressions[1];

            // fieldName
            if (numArgs > 1)
            {
                _mtouchFieldName = ((ParameterList)rhs).Expressions[2];
            }

            // baseURL
            if (numArgs > 2)
            {
                _baseURL = ((ParameterList)rhs).Expressions[0];
            }
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
                return "MTouchLink('baseURL', 'linkText', 'mtouchFieldName')";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">An instance of ContextObject</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            const string methodName = "MTouchLink";

            // linkText
            string linkText = null;
            try
            {
                linkText = (_linkText.evaluate(contextObject) ?? string.Empty).ToString();
                if (string.IsNullOrEmpty(linkText))
                {
                    throw new Exception("Argument linkText evaluates to null or empty");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg linkText", ex);
                throw new CRMException("MTouchLink: Problem with argument linkText", ex);
            }

            string mtouchFieldName = null;
            try
            {
                mtouchFieldName = (_mtouchFieldName.evaluate(contextObject) ?? string.Empty).ToString();
                if (string.IsNullOrEmpty(mtouchFieldName))
                {
                    throw new Exception("Argument mtouchFieldName evaluates to null or empty");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg fieldName", ex);
                throw new CRMException("MTouchLink: Problem with argument mtouchFieldName", ex);
            }

            // baseUrl
            string baseUrl = null;
            try
            {
                if (_baseURL == null)
                {
                    baseUrl = LWConfigurationUtil.GetConfigurationValue("LWCouponPassGeneratorURL");
                }
                else
                {
                    baseUrl = _baseURL.evaluate(contextObject).ToString();
                }

                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new Exception("Argument baseUrl evaluates to null or empty");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating argument baseUrl", ex);
                throw new CRMException("MTouchLink: Problem with argument baseUrl", ex);
            }

           	string href = baseUrl;

			// Add mtouch query param, invalid url throws exception
			Uri url = new Uri(baseUrl);
			if (string.IsNullOrEmpty(url.Query))
			{
				href += "?MTouch=";
			}
			else if (!url.Query.ToLower().Contains("mtouch="))
			{
				href += "&MTouch=";
			}

			href += "#%%#" + _mtouchFieldName + "#%%#";

			string result = string.Format("<a href=\"{0}\">{1}</a>", href, _linkText);
			return result;
        }
    }
}
