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
	/// Used to return the value of a concept within the content of a particular survey question.
	/// 
	/// The method arguments are as follows:
	/// ConceptName - (optional) a string that matches a concept name in the list of concepts for the current survey.
	///    If multiple concept names are provided as arguments, then a random one is selected.  If no arguments are 
	///    provided, then a random concept is selected from the entire list of concepts for the survey.
	/// 
	/// The result of this method is a string, which represents HTML of the concept to be injected.
	/// </summary>
	/// <example>
	///     Usage: SurveyConcept(['ConceptName1'[,ConceptName2[,...,ConceptNameN]]])
	/// </example>
	[Serializable]
	[ExpressionContext(Description = "Returns the value of a concept within the content of a particular survey question.",
		DisplayName = "SurveyConcept",
		ExcludeContext = 0,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Survey,
		ExpressionReturns = ExpressionApplications.Strings,
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Survey,
		WizardDescription = "Get the content for a survey concept")]
	public class SurveyConcept : UnaryOperation
	{
		private const string _className = "SurveyConcept";
		private static LWLogger _logger = LWLoggerManager.GetLogger(SurveyConstants.LOG_APP_NAME);
		private static Random random = new Random((int)DateTime.Now.Ticks);
        private List<Expression> _conceptNameArgs = new List<Expression>();

		/// <summary>
		/// Constructor
		/// </summary>
		public SurveyConcept()
		{
		}

		/// <summary>
		/// Constructor used internally.
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.FrameWork.bScript.Expression"/> containing the functions parameters.</param>
		public SurveyConcept(Expression rhs)
			: base("SurveyConcept", rhs)
		{
			string methodName = "SurveyConcept";

			if (rhs == null)
			{
				// just an empty list of _conceptNameArgs
				return;
			}

			if (rhs is StringConstant)
			{
				// Single ConceptName
				Expression conceptNameArg = (StringConstant)rhs;
				_conceptNameArgs.Add(conceptNameArg);
			}
			else if (rhs is ParameterList)
			{
				// ConceptName1 .. ConceptNameN
				for (int exprIndex = 0; exprIndex < ((ParameterList)rhs).Expressions.Length; exprIndex++)
				{
					try
					{
						object obj = ((ParameterList)rhs).Expressions[exprIndex];
						if (obj == null || string.IsNullOrEmpty(obj.ToString()))
						{
							string msg = "Argument ConceptName[" + (exprIndex + 1) + "] evaluates to null or empty.";
							_logger.Error(_className, methodName, msg);
							throw new Exception(msg);
						}
						Expression conceptNameArg = (StringConstant)obj;
						_conceptNameArgs.Add(conceptNameArg);
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Exception evaluating argument ConceptName[" + (exprIndex + 1) + "]", ex);
					}
				}
			}
			else
			{
				string msg = "Invalid Function Call: Unknown argument type passed to SurveyConcept.";
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
				return "SurveyConcept(['ConceptName1'[,ConceptName2[,...,ConceptNameN]]])";
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
				string msg = "Evaluating: SurveyConcept(" + FlattenList(_conceptNameArgs, contextObject) + ")";
				_logger.Debug(_className, methodName, msg);
			}
			catch { /* ignore */ }

			// Get the environment properties passed in the context object
			if (contextObject.Environment == null || !(contextObject.Environment is Dictionary<string, object>))
			{
				string msg = "SurveyConcept: No environment was passed.";
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
			long stateID = -1;
			if (args.ContainsKey("stateID")) stateID = StringUtils.FriendlyInt64(args["stateID"], -1);

			using (var svc = LWDataServiceUtil.SurveyManagerInstance())
			{
				SMConcept selectedConcept = null;

				// is there an existing seen concept to show?
				List<SMConceptView> existingConceptViews = svc.RetrieveConceptViewsByRespondentAndState(respondentID, stateID);
				if (existingConceptViews != null && existingConceptViews.Count > 0)
				{
					_logger.Debug(_className, methodName, "Show existing concept view: " + existingConceptViews[0].ID);
					selectedConcept = svc.RetrieveConcept(existingConceptViews[0].ConceptID);
				}

				// if not existing concept view, then select a concept to view
				if (selectedConcept == null)
				{
					// Make sure there are concepts
					selectedConcept = svc.SelectConcept(surveyID, languageID, respondentID, GetConceptList(_conceptNameArgs, contextObject));
					if (selectedConcept == null)
					{
						string msg = "SurveyConcept(" + FlattenList(_conceptNameArgs, contextObject) + "): No concept to show.";
						_logger.Error(_className, methodName, msg);
						return string.Empty;
					}

					// create a concept view for this concept/respondent
					if (respondentID != -1)
					{
						SMConceptView conceptView = new SMConceptView()
						{
							ConceptID = selectedConcept.ID,
							RespondentID = respondentID,
							StateID = stateID
						};
						svc.CreateConceptView(conceptView);
					}
				}

				// Log the result
				try
				{
					string msg = "SurveyConcept(" + FlattenList(_conceptNameArgs, contextObject) + ") -> " + selectedConcept.ID + " " + selectedConcept.Name;
					_logger.Debug(_className, methodName, msg);
				}
				catch { /* ignore */ }

				//return selectedConcept.Content;
				string innerClassName = !selectedConcept.Content.Contains("img") && !selectedConcept.Content.Contains("iframe") ? "surveyConcept" : "surveyConceptInner";
				return string.Format(@"<div id=""concept_{0}"" class=""surveyConceptOuter""><div class=""{2}"">{1}</div></div>", stateID, selectedConcept.Content, innerClassName);
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

		private string FlattenList(List<Expression> list, ContextObject contextObject)
		{
			string result = null;
			bool firstTime = true;
			foreach (Expression item in list)
			{
				if (firstTime)
					firstTime = false;
				else
					result += ",";
				result += item.evaluate(contextObject).ToString();
			}
			return result;
		}

        private List<string> GetConceptList(List<Expression> list, ContextObject contextObject)
        {
            List<string> result = list.Count > 0 ? new List<string>() : null;
            bool firstTime = true;
            foreach (Expression item in list)
            {
                result.Add(item.evaluate(contextObject).ToString());
            }
            return result;
        }
	}
}
