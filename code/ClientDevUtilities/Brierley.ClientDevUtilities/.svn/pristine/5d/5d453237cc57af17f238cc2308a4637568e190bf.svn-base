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
    /// Used to create a clickable html link (anchor tag) to a generic URL.  Method returns a string 
    /// which is the html rendering of the link.
    /// 
    /// The method arguments are as follows:
    /// baseURL - a string that is the base URL for the href part of the link. 
    /// linkText - a string that will be embedded in the link.	
	/// extraAttributes - a string that will be added as attributes to the link.	
	/// 
	/// E.g., "<a href="baseURL" extraAttributes>LinkText</a>"
    /// </summary>
    /// <example>
    ///     Usage: MTouchLink('baseURL', 'linkText'[, 'extraAttributes'])
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Returns an Achor tag.",
        DisplayName = "HtmlLink",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Survey,
        ExpressionReturns = ExpressionApplications.Strings,
		WizardDescription = "HTML Link",
		AdvancedWizard = true)]
	[ExpressionParameter(Order = 1, Name = "baseURL", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Base URL")]
	[ExpressionParameter(Order = 2, Name = "linkText", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Link Text")]
	[ExpressionParameter(Order = 3, Name = "extraAttributes", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Extra Attributes")]
    public class HtmlLink : UnaryOperation
    {
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_WEBFRAMEWORK);
        private const string _className = "HtmlLink";

        private Expression baseUrlExpression = null;
        private Expression linkTextExpression = null;
		private Expression extraAttributesExpression = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public HtmlLink()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
        public HtmlLink(Expression rhs)
            : base("HtmlLink", rhs)
        {
            ParameterList plist = rhs as ParameterList;
            if (plist.Expressions.Length == 2)
            {
                baseUrlExpression = ((ParameterList)rhs).Expressions[0];
                linkTextExpression = ((ParameterList)rhs).Expressions[1];
                return;
            }
			else if (plist.Expressions.Length == 3)
			{
				baseUrlExpression = ((ParameterList)rhs).Expressions[0];
				linkTextExpression = ((ParameterList)rhs).Expressions[1];
				extraAttributesExpression = ((ParameterList)rhs).Expressions[2];
				return;
			}
            throw new CRMException("Invalid Function Call: Wrong number of arguments passed to HtmlLink.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
				return "HtmlLink('baseURL', 'linkText'[, 'extraAttributes'])";
            }
        }

        /// <summary>
        /// Performs the operation defined by this function
        /// </summary>
        /// <param name="contextObject">An instance of ContextObject</param>
        /// <returns>An object representing the result of the evaluation</returns>
        public override object evaluate(ContextObject contextObject)
        {
            string methodName = "evaluate";

            string baseURL = string.Empty;
		    string linkText = string.Empty;
			string extraAttributes = string.Empty;

            // baseURL
            try
            {
                baseURL = baseUrlExpression.evaluate(contextObject).ToString();
                if (string.IsNullOrEmpty(baseURL))
                    throw new Exception("Argument baseURL evaluates to null or empty");
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg baseURL", ex);
                throw new CRMException("HtmlLink: Problem with arg baseURL");
            }

            // linkText
			try
			{
				linkText = linkTextExpression.evaluate(contextObject).ToString();
				if (string.IsNullOrEmpty(linkText))
					throw new Exception("Argument linkText evaluates to null or empty");
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating arg linkText", ex);
				throw new CRMException("HtmlLink: Problem with arg linkText");
			}

			// extraAttributes
			if (extraAttributesExpression != null)
			{
				try
				{
					extraAttributes = StringUtils.FriendlyString(extraAttributesExpression.evaluate(contextObject));
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, "Exception evaluating arg extraAttributes", ex);
					throw new CRMException("HtmlLink: Problem with arg extraAttributes");
				}
			}

			if (!string.IsNullOrEmpty(extraAttributes))
			{
				return string.Format("<a href=\"{0}\" {2}>{1}</a>", baseURL, linkText, extraAttributes);
			}
			return string.Format("<a href=\"{0}\">{1}</a>", baseURL, linkText);
        }
    }
}
