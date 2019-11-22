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
    /// Used to set the value of a survey respondent property.
    /// 
    /// The method arguments are as follows:
    /// PropertyName - a property name for the current survey respondent.
	/// PropertyValue - a property value for the current survey respondent.
    /// </summary>
    /// <example>
	///     Usage: SetRespondentProperty('PropertyName', 'PropertyValue')
    /// </example>
    [Serializable]
	[ExpressionContext(Description = "Sets the value of a survey respondent property.",
		DisplayName = "SetRespondentProperty",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Objects,

		AdvancedWizard = true,
		WizardCategory = WizardCategories.Survey,
		WizardDescription = "Set respondent property")]

	[ExpressionParameter(Order = 0, Name = "PropName", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What property?")]
	[ExpressionParameter(Order = 1, Name = "PropValue", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What value?")]
    public class SetRespondentProperty : UnaryOperation
    {
        private const string _className = "SetRespondentProperty";
        private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
        private Expression _propertyName = null;
		private Expression _propertyValue = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public SetRespondentProperty()
        {
        }

        /// <summary>
        /// Constructor used internally.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public SetRespondentProperty(Expression rhs)
			: base("SetRespondentProperty", rhs)
        {
			string methodName = "SetRespondentProperty";

            if (rhs == null)
            {
				string msg = "Invalid Function Call: No arguments passed to SetRespondentProperty.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            if (rhs is ParameterList)
            {
                int numArgs = ((ParameterList)rhs).Expressions.Length;
                if (numArgs == 2)
                {
                    _propertyName = ((ParameterList)rhs).Expressions[0];
                    _propertyValue = ((ParameterList)rhs).Expressions[1];
                }
                else
                {
					string msg = "Invalid Function Call: Wrong number of arguments passed to SetRespondentProperty.";
                    _logger.Error(_className, methodName, msg);
                    throw new CRMException(msg);
                }
            }
            else
            {
				string msg = "Invalid Function Call: Unknown argument type passed to SetRespondentProperty.";
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
				return "SetRespondentProperty('PropertyName', 'PropertyValue')";
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

            string propertyName = null;
            try
            {
                propertyName = _propertyName.evaluate(contextObject).ToString();
                if (string.IsNullOrEmpty(propertyName))
                {
                    string msg = "Argument PropertyName evaluates to null or empty.";
                    _logger.Error(_className, methodName, msg);
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg PropertyName", ex);
                throw new CRMException("SetRespondentProperty: Problem with arg PropertyName");
            }

            string propertyValue = null;
            try
            {
                propertyValue = _propertyValue.evaluate(contextObject).ToString();
                if (string.IsNullOrEmpty(propertyValue))
                {
                    string msg = "Argument PropertyValue evaluates to null or empty.";
                    _logger.Error(_className, methodName, msg);
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg PropertyValue", ex);
                throw new CRMException("SetRespondentProperty: Problem with arg PropertyValue");
            }

            try
            {
				string msg = "Evaluating: SetRespondentProperty(" + propertyName + ", " + propertyValue + ")";
                _logger.Debug(_className, methodName, msg);
            }
            catch { }

            // get the environment properties passed in the context object
            if (contextObject.Environment == null)
            {
				string msg = "SetRespondentProperty: No environment was passed.";
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
				string msg = "SetRespondentProperty: No respondentID specified in environment.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			// get respondent
			using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMRespondent respondent = surveyManager.RetrieveRespondent(respondentID);
				if (respondent == null)
				{
					string msg = string.Format("SetRespondentProperty: respondentID '{0}' not found.", respondentID);
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}

				// set property value
				object result = (object)string.Empty;
				respondent.SetProperty(propertyName, propertyValue);
				surveyManager.UpdateRespondent(respondent);

				// log the result
				try
				{
					string msg = "SetRespondentProperty(" + propertyName + ", " + propertyValue + ") -> " + result;
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
