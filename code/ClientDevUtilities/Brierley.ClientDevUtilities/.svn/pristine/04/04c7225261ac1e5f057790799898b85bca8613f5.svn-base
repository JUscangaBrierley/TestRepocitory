using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Used to create a clickable html link (anchor tag) to a generic URL with an MTouch for getting a user's 
	/// loyalty card as a passbook pass.  Method returns a string which is the html rendering of the link.
	/// 
	/// The method arguments are as follows (all are optional):
	/// linkText - a string that will be embedded in the link.  (default = 'Please click here for loyalty card.')
	/// mtouchFieldName - a string that indicates the name of the template field which contains the encoded mtouch values.
	///    (default = 'mtouch')
	/// baseURL - a string that is the base URL for the href part of the link.  
	///    (default = value of LoyaltyCardPassGeneratorURL)
	/// imageURL - a string that is the URL for the image part of the link.  
	///    (default: Apple's image from the content root at LWContentRootURL/orgname/Add_to_Passbook_US_UK.png)
	/// 
	/// E.g., "<a href="baseURL?MTouch=fieldName"><img src="imageURL" alt="LinkText" /></a>".
	/// </summary>
	/// <example>
	///     Usage: LoyaltyCardLink(['linkText' [, 'mtouchFieldName' [, 'baseURL' [, 'imageURL']]]])
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns an HTML link to a loyalty card pass.",
		DisplayName = "LoyaltyCardLink",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Member,
		ExpressionReturns = ExpressionApplications.Strings,
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function,
		WizardDescription = "Loyalty Card link"
	)]

	[ExpressionParameter(Order = 0, Name = "LinkText", Optional = true, Type = ExpressionApplications.Strings, WizardDescription = "Link Text")]
	[ExpressionParameter(Order = 1, Name = "FieldName", Optional = true, Type = ExpressionApplications.Strings, WizardDescription = "Field Name")]
	[ExpressionParameter(Order = 2, Name = "BaseUrl", Optional = true, Type = ExpressionApplications.Strings, WizardDescription = "URL")]
	[ExpressionParameter(Order = 3, Name = "ImageUrl", Optional = true, Type = ExpressionApplications.Strings, WizardDescription = "URL")]
	public class LoyaltyCardLink : UnaryOperation
	{
		private const string _className = "LoyaltyCardLink";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private Expression _linkText = new StringConstant("Please click here for loyalty card.");
		private Expression _mtouchFieldName = new StringConstant("mtouch");
		private Expression _baseUrl = null;
		private Expression _imageUrl = null;

		/// <summary>
		/// Default Constructor
		/// </summary>
		public LoyaltyCardLink()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public LoyaltyCardLink(Expression rhs)
			: base("LoyaltyCardLink", rhs)
		{
			const string methodName = "evaluate";

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
			if (numArgs < 1 || numArgs > 4)
			{
				string msg = "Invalid Function Call: Wrong number of arguments passed to LoyaltyCardLink.";
				_logger.Error(_className, methodName, msg);
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

			// imageURL
			if (numArgs > 3)
			{
				_imageUrl = ((ParameterList)rhs).Expressions[3];
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "LoyaltyCardLink(['linkText' [, 'mtouchFieldName' [, 'baseURL' [, 'imageURL']]]])";
			}
		}

		/// <summary>
		/// Performs the operation defined by this function
		/// </summary>
		/// <param name="contextObject">An instance of ContextObject</param>
		/// <returns>An object representing the result of the evaluation</returns>
		public override object evaluate(ContextObject contextObject)
		{
			const string methodName = "LoyaltyCardLink";

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
				throw new CRMException("LoyaltyCardLink: Problem with argument linkText", ex);
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
				throw new CRMException("LoyaltyCardLink: Problem with argument mtouchFieldName", ex);
			}

			// baseUrl
			string baseUrl = null;
			try
			{
				if (_baseUrl == null)
				{
					baseUrl = LWConfigurationUtil.GetConfigurationValue("LoyaltyCardPassGeneratorURL");
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
				throw new CRMException("LoyaltyCardLink: Problem with argument baseUrl", ex);
			}

			//image url
			string imageUrl = null;
			try
			{
				if (_imageUrl == null)
				{
					imageUrl = string.Format(
						"{0}/{1}/Add_to_Passbook_US_UK.png",
						StringUtils.FriendlyString(LWConfigurationUtil.GetConfigurationValue("LWContentRootURL")),
						LWConfigurationUtil.GetCurrentEnvironmentContext().Organization);
				}
				else
				{
					imageUrl = _imageUrl.evaluate(contextObject).ToString();
				}

				if (string.IsNullOrEmpty(imageUrl))
				{
					throw new Exception("Argument imageUrl evaluates to null or empty");
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Exception evaluating argument imageUrl", ex);
				throw new CRMException("LoyaltyCardLink: Problem with argument imageUrl", ex);
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

			href += "#%%#" + mtouchFieldName + "#%%#";

			string result = string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" /></a>", href, imageUrl, linkText);
			return result;
		}
	}
}
