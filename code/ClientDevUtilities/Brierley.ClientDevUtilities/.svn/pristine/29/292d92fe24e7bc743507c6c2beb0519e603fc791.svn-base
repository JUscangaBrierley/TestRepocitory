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
	/// Used to return the value of a survey respondent property.
	/// 
	/// The method arguments are as follows:
	/// PropertyName - a string that matches a property name for the current survey respondent.
	/// 
	/// Since the properties are stored in a textual format in the database, this method attempts to 
	/// parse the property value as a DateTime, then as a real/integer value, then simply as the 
	/// string itself.  The parsed value is then returned by the method as the appropriate type.
	/// </summary>
	/// <example>
	///     Usage: GetRespondentProperty('PropertyName')
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns the value of a survey respondent property.",
		DisplayName = "GetRespondentProperty",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Objects,

		AdvancedWizard = true,
		WizardCategory = WizardCategories.Survey,
		WizardDescription = "Get respondent property")]

	[ExpressionParameter(Order = 0, Name = "Property", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What property?")]
	public class GetRespondentProperty : UnaryOperation
	{
		private const string _className = "GetRespondentProperty";
		private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
		private Expression _propertyName = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public GetRespondentProperty()
		{
		}

		/// <summary>
		/// Constructor used internally.
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public GetRespondentProperty(Expression rhs)
			: base("GetRespondentProperty", rhs)
		{
			string methodName = "GetRespondentProperty";

			if (rhs == null)
			{
				string msg = "Invalid Function Call: No arguments passed to GetRespondentProperty.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			if (rhs is StringConstant)
			{
				_propertyName = (StringConstant)rhs;
			}
			else if (rhs is ParameterList)
			{
				int numArgs = ((ParameterList)rhs).Expressions.Length;
				if (numArgs == 1)
				{
					_propertyName = ((ParameterList)rhs).Expressions[0];
				}
				else
				{
					string msg = "Invalid Function Call: Wrong number of arguments passed to GetRespondentProperty.";
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}
			}
			else
			{
				string msg = "Invalid Function Call: Unknown argument type passed to GetRespondentProperty.";
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
				return "GetRespondentProperty('PropertyName')";
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
			try
			{
				string msg = "Evaluating: GetRespondentProperty(" + _propertyName.evaluate(contextObject).ToString() + ")";
				_logger.Debug(_className, methodName, msg);
			}
			catch { }

			// get the environment properties passed in the context object
			if (contextObject.Environment == null)
			{
				string msg = "GetRespondentProperty: No environment was passed.";
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
			if (respondentID == -1)
			{
				string msg = "GetRespondentProperty: No respondentID specified in environment.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			// get respondent
			using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMRespondent respondent = surveyManager.RetrieveRespondent(respondentID);
				if (respondent == null)
				{
					string msg = string.Format("GetRespondentProperty: respondentID '{0}' not found.", respondentID);
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}


				// get property value
				object result = (object)string.Empty;
				string propertyValue = respondent.GetProperty(_propertyName.evaluate(contextObject).ToString());
				if (!string.IsNullOrEmpty(propertyValue))
				{
					DateTime dateTimeValue;
					double doubleValue;
					decimal decimalValue;
					long longValue;
					if (DateTime.TryParse(propertyValue, out dateTimeValue))
					{
						result = (object)dateTimeValue;
					}
					else if (double.TryParse(propertyValue, out doubleValue))
					{
						result = (object)doubleValue;
					}
					else if (decimal.TryParse(propertyValue, out decimalValue))
					{
						result = (object)decimalValue;
					}
					else if (long.TryParse(propertyValue, out longValue))
					{
						result = (object)longValue;
					}
					else
					{
						result = (object)propertyValue;
					}
				}

				// log the result
				try
				{
					string msg = "GetRespondentProperty(" + _propertyName.evaluate(contextObject).ToString() + ") -> " + result;
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
	}
}
