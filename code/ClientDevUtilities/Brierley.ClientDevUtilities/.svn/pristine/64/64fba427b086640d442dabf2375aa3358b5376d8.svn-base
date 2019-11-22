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
    /// Used to determine if the quota has been met for a concept in a survey for a particular segment.
    /// 
    /// The method arguments are as follows:
    /// ConceptName - a string that matches a concept name for the current survey.
	/// RespondentPropName - a string that matches a respondent property name (e.g., 'gender').
	/// RespondentPropValue - a string that matches a respondent property value (e.g., 'male').
	/// 
    /// </summary>
    /// <example>
	///     Usage: IsQuotaMetForConcept('ConceptName', 'RespondentPropName', 'RespondentPropValue')
    /// </example>
    [Serializable]
	[ExpressionContext(Description = "Returns bool indicating whether segment quota met for a concept in a survey.",
		DisplayName = "IsQuotaMetForConcept",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Booleans,
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Survey,
		WizardDescription = "Is quota met for concept?")]

	[ExpressionParameter(Order = 0, Name = "Concept", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What concept?")]
	[ExpressionParameter(Order = 1, Name = "PropName", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What property?")]
	[ExpressionParameter(Order = 2, Name = "PropValue", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What property value?")]
    public class IsQuotaMetForConcept : UnaryOperation
    {
        private const string _className = "IsQuotaMetForConcept";
        private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
        private Expression _conceptName = null;
		private Expression _respondentPropName = null;
		private Expression _respondentPropValue = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public IsQuotaMetForConcept()
        {
        }

        /// <summary>
        /// Constructor used internally.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public IsQuotaMetForConcept(Expression rhs)
			: base("IsQuotaMetForConcept", rhs)
        {
			string methodName = "IsQuotaMetForConcept";

            if (rhs == null)
            {
				string msg = "Invalid Function Call: No arguments passed to IsQuotaMetForConcept.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            if (rhs is ParameterList)
            {
                int numArgs = ((ParameterList)rhs).Expressions.Length;
                if (numArgs == 3)
                {
                    _conceptName = ((ParameterList)rhs).Expressions[0];
                    _respondentPropName = ((ParameterList)rhs).Expressions[1];
                    _respondentPropValue = ((ParameterList)rhs).Expressions[2];
                }
                else
                {
					string msg = "Invalid Function Call: Wrong number of arguments passed to IsQuotaMetForConcept.";
                    _logger.Error(_className, methodName, msg);
                    throw new CRMException(msg);
                }
            }
            else
            {
				string msg = "Invalid Function Call: Unknown argument type passed to IsQuotaMetForConcept.";
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
				return "IsQuotaMetForConcept('ConceptName', 'RespondentPropName', 'RespondentPropValue')";
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

            string conceptName = null;
            try
			{
                conceptName = (_conceptName.evaluate(contextObject) ?? string.Empty).ToString();
                if (string.IsNullOrEmpty(conceptName))
				{
                    throw new Exception("Argument ConceptName evaluates to null or empty.");
				}
			}
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg ConceptName", ex);
                throw new CRMException("IsQuotaMetForConcept: Problem with arg ConceptName");
            }

            string respondentPropName = null;
            try
            {
                respondentPropName = (_respondentPropName.evaluate(contextObject) ?? string.Empty).ToString();
                if (string.IsNullOrEmpty(respondentPropName))
                {
                    throw new Exception("Argument RespondentPropName evaluates to null or empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg RespondentPropName", ex);
                throw new CRMException("IsQuotaMetForConcept: Problem with arg RespondentPropName");
            }

            string respondentPropValue = null;
            try
            {
                respondentPropValue = (_respondentPropValue.evaluate(contextObject) ?? string.Empty).ToString();
                if (string.IsNullOrEmpty(respondentPropValue))
                {
                    throw new Exception("Argument RespondentPropValue evaluates to null or empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg RespondentPropValue", ex);
                throw new CRMException("IsQuotaMetForConcept: Problem with arg RespondentPropValue");
            }
            
            try
            {
				string msg = "Evaluating: IsQuotaMetForConcept(" + conceptName + ")";
                _logger.Debug(_className, methodName, msg);
            }
            catch { }

            // get the environment properties passed in the context object
            if (contextObject.Environment == null)
            {
				string msg = "IsQuotaMetForConcept: No environment was passed.";
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
				string msg = "IsQuotaMetForConcept: No respondentID specified in environment.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			// get concept
			using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMConcept concept = surveyManager.RetrieveConceptByName(surveyID, languageID, conceptName);
				if (concept == null)
				{
					string msg = string.Format("IsQuotaMetForConcept: concept named '{0}' not found.", conceptName);
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}

				bool result = false;

				// get concept quota
				long quota = concept.GetSegmentQuota(respondentPropName, respondentPropValue);
				if (quota < 0)
				{
					// no quota
					result = false;
				}
				else if (quota == 0)
				{
					// quota is 0, so always met
					result = true;
				}
				else
				{
					// compare quota with concept view count
					long count = surveyManager.RetrieveConceptViewsForSegment(concept.ID, respondentPropName, respondentPropValue);
					if (count >= quota) result = true;
				}

				// log the result
				try
				{
					string msg = "IsQuotaMetForConcept(" + conceptName + ") -> " + result;
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
