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
    /// Used to determine if the current respondent saw a concept in a survey.
    /// 
    /// The method arguments are as follows:
    /// ConceptName - a string that matches a concept name for the current survey.
	/// 
    /// </summary>
    /// <example>
	///     Usage: DidRespondentViewConcept('ConceptName')
    /// </example>
    [Serializable]
	[ExpressionContext(Description = "Returns bool indicating whether a respondent saw a concept in a survey.",
		DisplayName = "DidRespondentViewConcept",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Booleans, 
		AdvancedWizard = true, 
		WizardCategory = WizardCategories.Survey, 
		WizardDescription = "Did respondent view concept?"
		)]

	[ExpressionParameter(Order = 0, Name = "Concept", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What concept?")]
    public class DidRespondentViewConcept : UnaryOperation
    {
        private const string _className = "DidRespondentViewConcept";
        private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
        private Expression _conceptName = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public DidRespondentViewConcept()
        {
        }

        /// <summary>
        /// Constructor used internally.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public DidRespondentViewConcept(Expression rhs)
			: base("DidRespondentViewConcept", rhs)
        {
			string methodName = "DidRespondentViewConcept";

            if (rhs == null)
            {
				string msg = "Invalid Function Call: No arguments passed to DidRespondentViewConcept.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            if (rhs is StringConstant)
            {
				_conceptName = (StringConstant)rhs;
            }
            else if (rhs is ParameterList)
            {
                int numArgs = ((ParameterList)rhs).Expressions.Length;
                if (numArgs == 1)
                {
                    _conceptName = ((ParameterList)rhs).Expressions[0];
                }
                else
                {
					string msg = "Invalid Function Call: Wrong number of arguments passed to DidRespondentViewConcept.";
                    _logger.Error(_className, methodName, msg);
                    throw new CRMException(msg);
                }
            }
            else
            {
				string msg = "Invalid Function Call: Unknown argument type passed to DidRespondentViewConcept.";
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
				return "DidRespondentViewConcept('ConceptName')";
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
				string msg = "Evaluating: DidRespondentViewConcept(" + _conceptName.evaluate(contextObject).ToString() + ")";
                _logger.Debug(_className, methodName, msg);
            }
            catch { }

            // get the environment properties passed in the context object
            if (contextObject.Environment == null)
            {
				string msg = "DidRespondentViewConcept: No environment was passed.";
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
				string msg = "DidRespondentViewConcept: No respondentID specified in environment.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			// get respondent
			using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMRespondent respondent = surveyManager.RetrieveRespondent(respondentID);
				if (respondent == null)
				{
					string msg = string.Format("DidRespondentViewConcept: respondentID '{0}' not found.", respondentID);
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}

				SMConcept concept = surveyManager.RetrieveConceptByName(surveyID, languageID, _conceptName.evaluate(contextObject).ToString());
				if (concept == null)
				{
					string msg = string.Format("DidRespondentViewConcept: concept named '{0}' not found.", _conceptName.evaluate(contextObject).ToString());
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}

				// check concept views
				bool result = false;
				List<SMConceptView> conceptViews = surveyManager.RetrieveConceptViewsByRespondent(respondentID);
				if (conceptViews != null && conceptViews.Count > 0)
				{
					foreach (SMConceptView conceptView in conceptViews)
					{
						if (conceptView.ConceptID == concept.ID)
						{
							result = true;
							break;
						}
					}
				}

				// log the result
				try
				{
					string msg = "DidRespondentViewConcept(" + _conceptName.evaluate(contextObject).ToString() + ") -> " + result;
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
