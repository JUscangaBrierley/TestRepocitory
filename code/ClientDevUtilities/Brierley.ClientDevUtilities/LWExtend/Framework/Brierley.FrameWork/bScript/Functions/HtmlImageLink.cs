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
    /// targetURL - a string that is the target URL for the href part of the link.  
    /// imageURL - a string that is the URL of the image that will be embedded in the link.	
	/// 
	/// E.g., "<a href="targetURL"><img src="imageURL" /></a>"
    /// </summary>
    /// <example>
    ///     Usage: HtmlImageLink('targetURL', 'imageURL')
    /// </example>
    [Serializable]
    [ExpressionContext(Description = "Returns html markup consisting of an anchor tag containing an image.",
        DisplayName = "HtmlImageLink",
        ExcludeContext = 0,
        ExpressionType = ExpressionTypes.Function,
        ExpressionApplication = ExpressionApplications.Survey,
        ExpressionReturns = ExpressionApplications.Strings,
		WizardDescription = "HTML Image Link",
		AdvancedWizard = true)]
	[ExpressionParameter(Order = 1, Name = "targetURL", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Target URL")]
	[ExpressionParameter(Order = 2, Name = "imageURL", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Image URL")]
    public class HtmlImageLink : UnaryOperation
    {
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_WEBFRAMEWORK);
        private const string _className = "HtmlImageLink";

        private Expression targetUrlExpression = null;
        private Expression imageUrlExpression = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public HtmlImageLink()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public HtmlImageLink(Expression rhs)
			: base("HtmlImageLink", rhs)
        {
            ParameterList plist = rhs as ParameterList;
            if (plist.Expressions.Length == 2)
            {
                targetUrlExpression = ((ParameterList)rhs).Expressions[0];
                imageUrlExpression = ((ParameterList)rhs).Expressions[1];
                return;
            }
			throw new CRMException("Invalid Function Call: Wrong number of arguments passed to HtmlImageLink.");
        }

        /// <summary>
        /// Returns the syntax definition for this function
        /// </summary>
        public new string Syntax
        {
            get
            {
				return "HtmlImageLink('targetURL', 'imageURL')";
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

            string targetURL = string.Empty;
		    string imageURL = string.Empty;

            // baseURL
            try
            {
                targetURL = targetUrlExpression.evaluate(contextObject).ToString();
                if (string.IsNullOrEmpty(targetURL))
                    throw new Exception("Argument targetURL evaluates to null or empty");
            }
            catch (Exception ex)
            {
				_logger.Error(_className, methodName, "Exception evaluating arg targetURL", ex);
				throw new CRMException("HtmlImageLink: Problem with arg targetURL");
            }

            // linkText
			try
			{
				imageURL = imageUrlExpression.evaluate(contextObject).ToString();
				if (string.IsNullOrEmpty(imageURL))
					throw new Exception("Argument imageURL evaluates to null or empty");
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating arg imageURL", ex);
				throw new CRMException("HtmlImageLink: Problem with arg imageURL");
			}

            string result = string.Format("<a href=\"{0}\"><img src=\"{1}\" /></a>", targetURL, imageURL);
            return result;
        }
    }
}
