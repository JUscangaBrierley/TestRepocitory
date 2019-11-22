using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;


namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class CampaignQuery : Query
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(Constants.LW_CM);
		private const string _className = "CampaignQuery";

		public enum ExecutionModes
		{
			Purge, 
			Execute, 
			ExecuteAutoVerify, 
			VerifyOnly
		}

		public class Parameter
		{
			/// <summary>
			/// Gets or sets the name of the parameter (campaign attribute name)
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// Gets or sets the value of the parameter.
			/// </summary>
			public string Value { get; set; }

			/// <summary>
			/// Gets or sets whether or not the campaign's value should be overridden.
			/// </summary>
			public bool UseCampaignValue { get; set; }
		}


		public string CampaignName { get; set; }


		public List<Parameter> Parameters { get; set; }


		public ExecutionModes ExecutionMode { get; set; }


		public CampaignQuery()
			: base()
		{
			ExecutionMode = ExecutionModes.Execute;
		}
		

		public override void EnsureSchema()
		{
			return;
		}


		public override List<SqlStatement> GetSqlStatement(Dictionary<string, string> overrideParameters = null)
		{
			return null;
		}


		public override System.Data.DataTable GetDataSample(string[] groupBy)
		{
			return null;
		}


		public override List<SqlStatement> GetVerifySqlStatement()
		{
			return null;
		}


		public override bool Validate(List<ValidationMessage> Warnings, bool ValidateSql)
		{
			if (string.IsNullOrEmpty(CampaignName))
			{
				Warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("A campaign is required for step {0}", Step.UIName)));
				return false;
			}

            using (var mgr = LWDataServiceUtil.CampaignManagerInstance())
            {
                var campaign = mgr.GetCampaign(CampaignName);
                if (campaign == null)
                {
                    Warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("The campaign \"{0}\" does not exist. Step {1}", CampaignName, Step.UIName)));
                    return false;
                }
            }
			//The campaign itself may be invalid, but it takes too much time to validate a single campaign. Potentially validating 10, 25 or even 
			//more campaigns will take too much time. They should be validated as they are built, not here.

			return true;
		}


		internal override List<CampaignResult> Execute(FrameWork.ContextObject co = null, Dictionary<string, string> overrideParameters = null, bool resume = false)
		{
			const string methodName = "Execute";

			_logger.Debug(_className, methodName, string.Format("Executing campaign query {0}, id {1}", Step.UIName, Step.Id));

			List<ValidationMessage> warnings = new List<ValidationMessage>();
			if (!resume && !Validate(warnings, true))
			{
				string exception = "Failed to execute " + Step.UIName + " because the step is invalid.";
				foreach (ValidationMessage message in warnings)
				{
					exception += message.Message;
				}
				throw new Exception(exception);
			}

			try
			{
                using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                {
                    var plan = manager.GetCampaign(Step.CampaignId.Value);

                    List<long> selectedSteps = new List<long>();

                    Campaign campaign = manager.GetCampaign(CampaignName);

                    if (campaign.IsExecuting.GetValueOrDefault() && !resume)
                    {
                        throw new Exception(string.Format("Campaign {0} is already executing. Exiting.", campaign.Name));
                    }

                    campaign.ExecutionType = plan.ExecutionType;
                    //set executing user to the same user who started the plan
                    campaign.ExecutionUserId = plan.ExecutionUserId;
                    manager.UpdateCampaign(campaign);

                    var stateManager = new StateManager(campaign, true);
                    if (ExecutionMode == ExecutionModes.Purge)
                    {
                        _logger.Trace(_className, methodName, string.Format("Purging campaign {0}.", CampaignName));
                        stateManager.PurgeCampaign();
                        _logger.Trace(_className, methodName, string.Format("Purging of campaign {0} completed.", CampaignName));
                    }
                    else
                    {
                        stateManager.AutoVerify = ExecutionMode == ExecutionModes.ExecuteAutoVerify;
                        stateManager.VerifyOnly = ExecutionMode == ExecutionModes.VerifyOnly;
                        _logger.Trace(_className, methodName, string.Format("Executing campaign {0}.", CampaignName));

                        Dictionary<string, string> overrides = new Dictionary<string, string>();
                        if (Parameters != null && Parameters.Count > 0)
                        {
                            foreach (var p in Parameters.Where(o => !o.UseCampaignValue))
                            {
                                overrides.Add(p.Name, p.Value);
                            }
                        }
                        if (resume)
                        {
                            stateManager.ResumeCampaign();
                        }
                        else
                        {
                            stateManager.ExecuteCampaign(overrides);
                        }
                        _logger.Trace(_className, methodName, string.Format("Execution of campaign {0} completed.", CampaignName));
                    }
                }
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Failed to execute campaign.", ex);
				throw;
			}
			_logger.Debug(_className, methodName, string.Format("Finished executing campaign query {0}, id {1}", Step.UIName, Step.Id));
			return new List<CampaignResult>() { new CampaignResult(1) };
		}
	}
}
