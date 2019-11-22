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
	/// Used to allow the user to review a concept that had been seen on a particular state in the current survey.
	/// 
	/// The method arguments are as follows:
	/// StateName - The state name (for a question or message).
	/// Prompt - (optional) If the concept is to be seen in a dialog box, then this is the prompt for the link.  If not 
	///              provided, then the concept will be shown inline.
	/// 
	/// The result of this method is a string, which represents HTML to be injected to review the concept.
	/// </summary>
	/// <example>
	///     Usage: SurveyReviewConcept('StateName'[,'Prompt'])
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns the HTML to review a concept seen in a previous state in the current survey.",
		DisplayName = "SurveyReviewConcept",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Strings,
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Survey,
		WizardDescription = "Review a survey concept")]
	[ExpressionParameter(Order = 0, Name = "StateName", Optional = false, Type = ExpressionApplications.Strings, WizardDescription = "Name of the survey state where the concept was seen?")]
	[ExpressionParameter(Order = 1, Name = "Prompt", Optional = true, Type = ExpressionApplications.Strings, WizardDescription = "Optional prompt for hyperlink?")]
	public class SurveyReviewConcept : UnaryOperation
	{
		private const string _className = "SurveyReviewConcept";
		private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
		private static Random random = new Random((int)DateTime.Now.Ticks);
		private Expression _stateName = null;
		private Expression _prompt = null;
        private string stateName = null;
        private string prompt = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public SurveyReviewConcept()
		{
		}

		/// <summary>
		/// Constructor used internally.
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public SurveyReviewConcept(Expression rhs)
			: base("SurveyReviewConcept", rhs)
		{
			string methodName = "SurveyReviewConcept";

			if (rhs == null)
			{
				string msg = "Invalid Function Call: No arguments passed to SurveyReviewConcept.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			if (rhs is StringConstant)
			{
				// Single argument: state name
				_stateName = (StringConstant)rhs;
			}
			else if (rhs is ParameterList)
			{
				if (((ParameterList)rhs).Expressions.Length != 2)
				{
					string msg = "Invalid Function Call: Invalid number of arguments passed to SurveyReviewConcept.";
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}

                _stateName = ((ParameterList)rhs).Expressions[0];
                _prompt = ((ParameterList)rhs).Expressions[1];
			}
			else
			{
				string msg = "Invalid Function Call: Unknown argument type passed to SurveyReviewConcept.";
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
				return "SurveyReviewConcept('StateName'[,'Prompt'])";
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
                object obj = _stateName.evaluate(contextObject);
                if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                {
                    string msg = "Argument StateName evaluates to null or empty.";
                    _logger.Error(_className, methodName, msg);
                    throw new Exception(msg);
                }
                stateName = obj.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating argument StateName", ex);
            }

            try
            {
                object obj = _prompt.evaluate(contextObject);
                if (obj != null)
                {
                    prompt = obj.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Exception evaluating argument Prompt", ex);
            }

			try
			{
				string msg = string.Format("Evaluating: SurveyReviewConcept('{0}','{1}')", 
					StringUtils.FriendlyString(stateName), StringUtils.FriendlyString(prompt));
				_logger.Debug(_className, methodName, msg);
			}
			catch { /* ignore */ }

			// Get the environment properties passed in the context object
			if (contextObject.Environment == null || !(contextObject.Environment is Dictionary<string, object>))
			{
				string msg = "SurveyReviewConcept: No environment was passed.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}
			var args = contextObject.Environment;
			long surveyID = -1;
			if (args.ContainsKey("surveyID"))
				surveyID = StringUtils.FriendlyInt64(args["surveyID"], -1);
			long respondentID = -1;
			if (args.ContainsKey("respondentID"))
				respondentID = StringUtils.FriendlyInt64(args["respondentID"], -1);

			if (surveyID == -1)
			{
				string msg = "SurveyReviewConcept: No survey ID specified in environment.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}
			if (respondentID == -1)
			{
				string msg = "SurveyReviewConcept: No respondent ID specified in environment.";
				_logger.Error(_className, methodName, msg);
				throw new CRMException(msg);
			}

			using (var svc = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMState state = svc.RetrieveState(surveyID, stateName);
				if (state == null)
				{
					string msg = string.Format("SurveyReviewConcept: No state named '{0}' was found.", stateName);
					_logger.Error(_className, methodName, msg);
					throw new CRMException(msg);
				}

				List<SMConceptView> conceptViews = svc.RetrieveConceptViewsByRespondentAndState(respondentID, state.ID);

				string result = string.Empty;
				if (conceptViews != null && conceptViews.Count > 0)
				{
					SMConcept selectedConcept = svc.RetrieveConcept(conceptViews[0].ConceptID);
					if (selectedConcept != null)
					{
						if (string.IsNullOrEmpty(prompt))
						{
							string innerClassName = !selectedConcept.Content.Contains("img") && !selectedConcept.Content.Contains("iframe") ? "surveyInlineReviewConcept" : "surveyInlineReviewConceptInner";
							_logger.Debug(_className, methodName, string.Format("[{0}] Selected concept {1} ({2}) without prompt", respondentID, selectedConcept.Name, selectedConcept.ID));
							result = string.Format(@"<div id=""conceptShowHide_{0}"" class=""surveyInlineLinkContainer""><a id=""lnkReviewConcept_{0}"" class=""surveyInlineVisibilityLink"" href=""#"" onclick=""showConceptReview('reviewconcept_{0}','lnkReviewConcept_{0}'); return false;"">show</a></div><div id=""reviewconcept_{0}"" class=""surveyInlineReviewConceptOuter"" style=""display:none;""><div class=""{2}"">{1}</div></div>", state.ID, selectedConcept.Content, innerClassName);
						}
						else
						{
							_logger.Debug(_className, methodName, string.Format("[{0}] Selected concept {1} ({2}) with prompt {3}", respondentID, selectedConcept.Name, selectedConcept.ID, _prompt));
							result = string.Format(@"<a href=""#"" onclick=""showConceptDialog('reviewconceptdialog_{0}'); return false;"">{1}</a><div id=""reviewconceptdialog_{0}"" style=""display:none"">{2}</div>", state.ID, _prompt, selectedConcept.Content);
						}
					}
					else
					{
						_logger.Warning(_className, methodName, string.Format("[{0}] No concept was found with concept ID {1}, using concept view {2}", respondentID, conceptViews[0].ConceptID, conceptViews[0].ID));
					}
				}
				else
				{
					_logger.Warning(_className, methodName, string.Format("[{0}] No concept views were found", respondentID));
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

		private string FlattenList(List<string> list)
		{
			string result = string.Empty;
			bool firstTime = true;
			foreach (string item in list)
			{
				if (firstTime)
					firstTime = false;
				else
					result += ",";
				result += item;
			}
			return result;
		}
	}
}
