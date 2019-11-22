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
	/// Used to get the content for a viewed concept for a particular state in a survey.
	/// 
	/// The method arguments are as follows:
	/// StateName - a string that matches a state name for the current survey.
	/// 
	/// </summary>
	/// <example>
	///     Usage: GetViewedConceptForState('StateName')
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns string content for a viewed concept for a particular state in a survey.",
		DisplayName = "GetViewedConceptForState",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Strings,
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Survey,
		WizardDescription = "Get viewed concept for survey state"
		)]

	[ExpressionParameter(Order = 0, Name = "StateName", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "What state name?")]
	public class GetViewedConceptForState : UnaryOperation
	{
		private const string _className = "GetViewedConceptForState";
        private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
        private Expression _stateName = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public GetViewedConceptForState()
        {
        }

        /// <summary>
        /// Constructor used internally.
        /// </summary>
        /// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public GetViewedConceptForState(Expression rhs)
			: base("GetViewedConceptForState", rhs)
        {
			string methodName = "GetViewedConceptForState";

            if (rhs == null)
            {
				string msg = "Invalid Function Call: No arguments passed to GetViewedConceptForState.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }

            if (rhs is StringConstant)
            {
				_stateName = ((StringConstant)rhs);
            }
            else if (rhs is ParameterList)
            {
                int numArgs = ((ParameterList)rhs).Expressions.Length;
                if (numArgs == 1)
                {
                    _stateName = ((ParameterList)rhs).Expressions[0];
                }
                else
                {
					string msg = "Invalid Function Call: Wrong number of arguments passed to GetViewedConceptForState.";
                    _logger.Error(_className, methodName, msg);
                    throw new CRMException(msg);
                }
            }
            else
            {
				string msg = "Invalid Function Call: Unknown argument type passed to GetViewedConceptForState.";
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
				return "GetViewedConceptForState('StateName')";
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

            string stateName = null;
            try
            {
                stateName = (_stateName.evaluate(contextObject) ?? string.Empty).ToString();
                if (string.IsNullOrEmpty(stateName))
                {
                    throw new Exception("Argument StateName evaluates to null or empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating arg StateName", ex);
                throw new CRMException("GetViewedConceptForState: Problem with arg StateName");
            }

            try
            {
				string msg = "Evaluating: GetViewedConceptForState(" + stateName + ")";
                _logger.Debug(_className, methodName, msg);
            }
            catch { }

            // get the environment properties passed in the context object
            if (contextObject.Environment == null)
            {
				string msg = "GetViewedConceptForState: No environment was passed.";
                _logger.Error(_className, methodName, msg);
                throw new CRMException(msg);
            }
			var args = contextObject.Environment;
            long surveyID = -1;
			if (args.ContainsKey("surveyID")) surveyID = StringUtils.FriendlyInt64(args["surveyID"], -1);
			long respondentID = -1;
			if (args.ContainsKey("respondentID")) respondentID = StringUtils.FriendlyInt64(args["respondentID"], -1);

			if (surveyID == -1)
			{
				string msg = "GetViewedConceptForState: No survey ID specified in environment.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}
			if (respondentID == -1)
			{
				string msg = "GetViewedConceptForState: No respondent ID specified in environment.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			// get the viewed concept
			string result = string.Empty;
			using (var surveyManager = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMState state = surveyManager.RetrieveState(surveyID, stateName);
				if (state != null)
				{
					List<SMConceptView> existingConceptViews = surveyManager.RetrieveConceptViewsByRespondentAndState(respondentID, state.ID);
					if (existingConceptViews != null && existingConceptViews.Count > 0)
					{
						SMConcept concept = surveyManager.RetrieveConcept(existingConceptViews[0].ConceptID);
						result = concept.Content;
					}
				}

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