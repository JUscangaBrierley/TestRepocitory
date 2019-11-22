using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Used to create a clickable html link (anchor tag) to a generic URL with an MTouch for getting a user's 
	/// coupon as a passbook pass.  Method returns a string which is the html rendering of the link.
	/// 
	/// The method arguments are as follows (all are optional):
	/// linkText - a string that will be embedded in the link.  (default = 'Please click here for coupon.')
	/// mtouchFieldName - a string that indicates the name of the template field which contains the encoded mtouch values.
	///    (default = 'mtouch')
	/// baseUrl - a string that is the base URL for the href part of the link.  
	///    E.g., "<a href="baseUrl?MTouch=fieldName">LinkText</a>".  (default = value of LWCouponPassGeneratorURL)
	/// </summary>
	/// <example>
	///     Usage: CouponLink(['linkText', ['mtouchFieldName', ['baseUrl']]])
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns an HTML link to a coupon pass.",
		DisplayName = "CouponLink",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Strings,

		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function,
		WizardDescription = "Coupon link"
	)]

	[ExpressionParameter(Order = 0, Name = "LinkText", Optional = true, Type = ExpressionApplications.Strings, WizardDescription = "Link Text")]
	[ExpressionParameter(Order = 1, Name = "FieldName", Optional = true, Type = ExpressionApplications.Strings, WizardDescription = "Field Name")]
	[ExpressionParameter(Order = 2, Name = "BaseUrl", Optional = true, Type = ExpressionApplications.Strings, WizardDescription = "URL")]
	public class CouponLink : UnaryOperation
	{
		private const string _className = "CouponLink";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private Expression _linkText = new StringConstant("Please click here for coupon.");
		private Expression _mtouchFieldName = new StringConstant("mtouch");
		private Expression _baseUrl = null;

		/// <summary>
		/// Default Constructor
		/// </summary>
		public CouponLink()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public CouponLink(Expression rhs)
			: base("CouponLink", rhs)
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
				string msg = "Invalid Function Call: Wrong number of arguments passed to CouponLink.";
				_logger.Error(_className, "CouponLink", msg);
				throw new CRMException(msg);
			}

			// linkText
			_linkText = ((ParameterList)rhs).Expressions[0];

			// fieldName
			if (numArgs > 1)
			{
				_mtouchFieldName = ((ParameterList)rhs).Expressions[1];
			}

			// baseURL
			if (numArgs > 2)
			{
				_baseUrl = ((ParameterList)rhs).Expressions[2];
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "CouponLink(['linkText', ['mtouchFieldName', ['baseUrl']]])";
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
				throw new CRMException("CouponLink: Problem with argument linkText", ex);
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
				throw new CRMException("CouponLink: Problem with argument mtouchFieldName", ex);
			}

			// baseUrl
			string baseUrl = null;
			try
			{
				if (_baseUrl == null)
				{
					baseUrl = LWConfigurationUtil.GetConfigurationValue("LWCouponPassGeneratorURL");
				}
				else
				{
					baseUrl = _baseUrl.evaluate(contextObject).ToString();
				}

				if (string.IsNullOrEmpty(baseUrl))
				{
					throw new Exception("Argument baseUrl evaluates to null or empty");
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating argument baseUrl", ex);
				throw new CRMException("CouponLink: Problem with argument baseUrl", ex);
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
