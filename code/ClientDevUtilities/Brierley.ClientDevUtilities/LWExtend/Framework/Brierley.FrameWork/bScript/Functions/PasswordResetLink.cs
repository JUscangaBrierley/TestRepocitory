using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Used to create a clickable html link (anchor tag) to a generic URL with a single use code ("suid").  Method returns a string 
	/// which is the html rendering of the link.
	/// 
	/// The method arguments are as follows:
	/// baseURL - a string that is the base URL for the href part of the link.  E.g., "<a href="baseURL?id=fieldName">LinkText</a>"
	/// linkText - a string that will be embedded in the link.
	/// suidFieldName - a string that indicates the name of the template field which contains the encoded suid value(s)
	/// </summary>
	/// <example>
	///     Usage: PasswordResetLink('baseURL', 'linkText', 'suidFieldName'])
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns an HTML link for resetting password.",
		DisplayName = "PasswordResetLink",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Strings,
		WizardDescription = "Password Reset Link",
		AdvancedWizard = true)]
	[ExpressionParameter(Order = 1, Name = "baseURL", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Base URL")]
	[ExpressionParameter(Order = 2, Name = "linkText", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Link Text")]
	[ExpressionParameter(Order = 3, Name = "suidFieldName", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "SUID Field Name")]
	public class PasswordResetLink : UnaryOperation
	{
		private const string _className = "PasswordResetLink";
		private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
		private Expression _baseUrl = null;
		private Expression _linkText = null;
		private Expression _suidFieldName = null;

		/// <summary>
		/// Default Constructor
		/// </summary>
		public PasswordResetLink()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public PasswordResetLink(Expression rhs)
			: base("PasswordResetLink", rhs)
		{
			const string methodName = "PasswordResetLink";

			if (rhs == null)
			{
				string msg = "Invalid Function Call: No arguments passed to PasswordResetLink.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			if (!(rhs is ParameterList))
			{
				string msg = "Invalid Function Call: Invalid argument passed to PasswordResetLink constructor.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			int numArgs = ((ParameterList)rhs).Expressions.Length;
			if (numArgs != 3)
			{
				string msg = "Invalid Function Call: Wrong number of arguments passed to PasswordResetLink.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			// baseURL
			_baseUrl = ((ParameterList)rhs).Expressions[0];

			// linkText
			_linkText = ((ParameterList)rhs).Expressions[1];

			// fieldName
			_suidFieldName = ((ParameterList)rhs).Expressions[2];
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "PasswordResetLink('baseURL', 'linkText', 'suidFieldName')";
			}
		}

		/// <summary>
		/// Performs the operation defined by this function
		/// </summary>
		/// <param name="contextObject">An instance of ContextObject</param>
		/// <returns>An object representing the result of the evaluation</returns>
		public override object evaluate(ContextObject contextObject)
		{
			const string methodName = "evaluate";

			string baseUrl = null;
			try
			{
				baseUrl = (_baseUrl.evaluate(contextObject) ?? string.Empty).ToString();
				if (string.IsNullOrEmpty(baseUrl))
				{
					throw new Exception("Argument baseURL evaluates to null or empty");
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating arg baseURL", ex);
				throw new CRMException("PasswordResetLink: Problem with arg baseURL");
			}

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
				throw new CRMException("PasswordResetLink: Problem with arg linkText");
			}

			// fieldName
			string suidFieldName = null;
			try
			{
				suidFieldName = (_suidFieldName.evaluate(contextObject) ?? string.Empty).ToString();
				if (string.IsNullOrEmpty(suidFieldName))
				{
					throw new Exception("Argument suidFieldName evaluates to null or empty");
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating arg suidFieldName", ex);
				throw new CRMException("PasswordResetLink: Problem with arg suidFieldName");
			}

			string href = baseUrl;

			// Add mtouch query param, invalid url throws exception
			Uri url = new Uri(baseUrl);

			if (string.IsNullOrEmpty(url.Query))
			{
				href += "?id=";
			}
			else if (!url.Query.ToLower().Contains("id="))
			{
				href += "id=";
			}

			href += "#%%#" + _suidFieldName + "#%%#";

			string result = string.Format("<a href=\"{0}\">{1}</a>", href, _linkText);
			return result;
		}
	}
}
