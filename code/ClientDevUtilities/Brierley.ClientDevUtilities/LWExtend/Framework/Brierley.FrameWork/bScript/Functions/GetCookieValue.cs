using System;
using System.Globalization;
using System.Threading;

using Brierley.FrameWork.Common.Exceptions;
using System.Web;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.Functions
{
    /// <summary>
    /// Get the value of a cookie.
    /// </summary>
    /// <example>
    ///     Usage : GetCurrentUILanguage()
    /// </example>
    /// <remarks>Function names are not case sensitive.</remarks>
    [Serializable]
	[ExpressionContext(Description = "Get the value of a cookie.",
		DisplayName = "GetCookieValue",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Strings,
		ExpressionReturns = ExpressionApplications.Strings,
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Content,
		WizardDescription = "Get the value of a cookie")]
	[ExpressionParameter(Name = "cookieName", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Cookie name?")]
    public class GetCookieValue : UnaryOperation
    {
		private const string _className = "GetCookieValue";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private Expression _cookieNameExpression = null;

        /// <summary>
        /// Syntax definition for this function.
        /// </summary>
        public new string Syntax
        {
            get { return "GetCookieValue('cookieName')"; }
        }

        /// <summary>
        /// External constructor, primarily used for UI implementations of drag and drop and getting syntax.
        /// </summary>
        public GetCookieValue()
        {
        }

        /// <summary>
        /// The internal constructor for the function.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal GetCookieValue(Expression rhs)
			: base("GetCookieValue", rhs)
        {
			if (rhs != null)
			{
				_cookieNameExpression = rhs;
				return;
			}
			throw new LWBScriptException("Invalid Function Call: Wrong number of arguments passed to GetCookieValue.");
        }

        /// <summary>
        /// Performs the operation defined by this function. 
        /// </summary>
        /// <param name="contextObject">A container object used to pass context at runtime.</param>
        /// <returns>An object representing the result of the evaluation</returns>
        /// <exception cref="Brierley.Framework.Common.Exceptions.CRMException">thrown for illegal arguments</exception>
        public override object evaluate(ContextObject contextObject)
        {
			const string methodName = "evaluate";
            try
            {
				string result = string.Empty;
				if (_cookieNameExpression != null)
				{
					object obj = _cookieNameExpression.evaluate(contextObject);
					if (obj != null)
					{
						string cookieName = obj.ToString();

						HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
						if (cookie != null)
						{
							result = StringUtils.FriendlyString(cookie.Value);
							_logger.Debug(_className, methodName, string.Format("Found cookie '{0}': {1}", cookieName, cookie.Value));
						}
						else
						{
							_logger.Debug(_className, methodName, string.Format("No cookie '{0}' found.", cookieName));
						}
					}
					else
					{
						_logger.Error(_className, methodName, "Expression for 'cookieName' evaluates to null: " + _cookieNameExpression);
					}
				}
				else
				{
					_logger.Error(_className, methodName, "Missing 'cookieName' argument");
				}
				return result;
            }
            catch (Exception)
            {
				throw new CRMException("Error evaluating GetCookieValue function");
            }
        }        
    }
}
