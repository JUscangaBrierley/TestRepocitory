using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.CampaignManagement.DomainModel;

namespace Brierley.FrameWork.bScript.Functions
{
	/// <summary>
	/// Returns the last record count for the specified campaign step.
	/// </summary>
	/// <example>
	///     Usage : CampaignStepResultCount(['Campaign Name'],'Step Name')
	/// </example>
	/// <remarks>
	/// Campaign Name must be the name of an existing campaign.
	/// Step Name must be the name of an existing step in the campaign.
	///</remarks>
	[Serializable]
	[ExpressionContext(Description = "Returns the last record count for the specified campaign step.",
		DisplayName = "CampaignStepResultCount",
		ExcludeContext = ExpressionContexts.Survey,
		ExpressionType = ExpressionTypes.Function,
		ExpressionApplication = ExpressionApplications.Numbers | ExpressionApplications.Campaign,
		ExpressionReturns = ExpressionApplications.Numbers,

		WizardDescription = "Campaign step result count",
		AdvancedWizard = true,
		WizardCategory = WizardCategories.Function,
		EvalRequiresMember = false
		)]

	//we won't ask for the campaign name in the expression wizard. We'll only be showing this function for campaign context, so the user will be editing a campaign. For the step name list, we'll
	//pop a suggestion list of all steps for the current campaign.
	//[ExpressionParameter(Order = 0, Name = "Campaign Name", Type = ExpressionApplications.Strings, Optional = true, WizardDescription = "Which campaign?", Helpers = ParameterHelpers.CampaignName)]
	[ExpressionParameter(Order = 1, Name = "Step Name", Type = ExpressionApplications.Strings, Optional = false, WizardDescription = "Which step?", Helpers = ParameterHelpers.CampaignStepName)]
	public class CampaignStepResultCount : UnaryOperation
	{
		private const string _className = "CampaignStepResultCount";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private Expression _campaignName = null;
		private Expression _stepName = null;

		/// <summary>
		/// Public Constructor
		/// </summary>
		public CampaignStepResultCount()
		{
		}

		/// <summary>
		/// Internal Constructor
		/// </summary>
		/// <param name="rhs">An object of type <see cref="Brierley.Framework.bScript.Expression"/> containing the functions parameters.</param>
		internal CampaignStepResultCount(Expression rhs)
			: base("CampaignStepResultCount", rhs)
		{
			if (rhs == null)
			{
				throw new Exception("Invalid function call. Wrong number of arguments passed to CampaignStepResultCount.");
			}

			if (rhs is StringConstant)
			{
				_stepName = rhs;
				return;
			}

			ParameterList plist = rhs as ParameterList;

			if (plist.Expressions.Length == 0)
			{
				throw new Exception("Invalid function call. Wrong number of arguments passed to CampaignStepResultCount.");
			}
			else if (plist.Expressions.Length == 1)
			{
				_stepName = plist.Expressions[0];
			}
			else // > 1
			{
				_campaignName = plist.Expressions[0];
				_stepName = plist.Expressions[1];
			}
		}

		/// <summary>
		/// Returns the syntax definition for this function
		/// </summary>
		public new string Syntax
		{
			get
			{
				return "CampaignStepResultCount(['Campaign Name'],'Step Name')";
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="contextObject"></param>
		/// <returns></returns>
		public override object evaluate(ContextObject contextObject)
		{
			try
			{
				string campaignName = null;
				string stepName = null;

				if (_campaignName == null)
				{
					//no campaign name passed in expression, we need it from context object
					if (contextObject == null || contextObject.Environment == null || !contextObject.Environment.ContainsKey("CurrentCampaign"))
					{
						throw new Exception("Invalid function call. CampaignStepResultCount requires CampaignName either as a parameter or in the context.");
					}
					else
					{
						campaignName = contextObject.Environment["CurrentCampaign"].ToString();
					}
				}
				else
				{
					campaignName = (_campaignName.evaluate(contextObject) ?? string.Empty).ToString();
				}

				if (string.IsNullOrEmpty(campaignName))
				{
					throw new Exception("Campaign name evaluates to null or empty");
				}

				stepName = (_stepName.evaluate(contextObject) ?? string.Empty).ToString();

				if (string.IsNullOrEmpty(stepName))
				{
					throw new Exception("Step name evaluates to null or empty");
				}

                using (CampaignManager mgr = LWDataServiceUtil.CampaignManagerInstance())
                {
                    Campaign campaign = mgr.GetCampaign(campaignName);
                    if (campaign == null)
                    {
                        throw new Exception(string.Format("Campaign {0} does not exist.", campaignName));
                    }

                    Step step = campaign.Steps.Where(o => o.UIName == stepName).FirstOrDefault();
                    if (step == null)
                    {
                        throw new Exception(string.Format("Step {0} does not exist in campaign {1}", stepName, campaignName));
                    }

                    return step.UILastRecordCount.GetValueOrDefault();
                }
			}
			catch (Exception ex)
			{
				_logger.Error(_className, "Evaluate", string.Empty, ex);
				throw;
			}
		}

	}
}
