//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.JobScheduler;
using Brierley.FrameWork.Data;


namespace Brierley.FrameWork.CampaignManagement.Jobs
{
	public class CampaignJob : IJob
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(Constants.LW_CM);
		private const string _className = "CampaignJob";
		private StateManager _stateManager = null;

		private StringBuilder _report = null;

		public void SetRunID(long runID)
		{
		}

		public FrameWork.Common.ScheduleJobStatus Run(string parms)
		{
			return ExecuteCampaign(parms, false);
		}

		public FrameWork.Common.ScheduleJobStatus Resume(string parms)
		{
			return ExecuteCampaign(parms, true);
		}

		public FrameWork.Common.ScheduleJobStatus ExecuteCampaign(string parms, bool resume)
		{
			const string methodName = "ExecuteCampaign";
			_report = new StringBuilder();
			try
			{
                using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                {

                    if (manager.BatchProvider == null)
                    {
                        _logger.Error(_className, methodName, "Module configuration does not exist. Module must be configured before the server may be used");
                        return FrameWork.Common.ScheduleJobStatus.Failure;
                    }


                    List<long> selectedSteps = new List<long>();

                    XElement element = XElement.Parse(parms);

                    long campaignId = -1;
                    long.TryParse(element.AttributeValue("id"), out campaignId);

                    bool autoVerify = bool.Parse(element.AttributeValue("autoverify", "false"));
                    bool verifyOnly = bool.Parse(element.AttributeValue("verifyonly", "false"));
                    bool purgeOnly = bool.Parse(element.AttributeValue("purgeonly", "false"));

                    if (campaignId < 1)
                    {
                        _logger.Error(_className, methodName, "Failed to determine campaign id. Params: " + parms);
                        _report.AppendLine("Failed to determine campaign id. Params: " + parms);
                        return FrameWork.Common.ScheduleJobStatus.Failure;
                    }

                    Campaign campaign = manager.GetCampaign(campaignId);
                    if (campaign == null)
                    {
                        _logger.Error(_className, methodName, "Failed to load campaign id " + campaignId.ToString());
                        _report.AppendLine("Failed to load campaign id " + campaignId.ToString());
                        return FrameWork.Common.ScheduleJobStatus.Failure;
                    }

                    _stateManager = new StateManager(campaign, true);
                    _stateManager.AutoVerify = autoVerify;
                    _stateManager.VerifyOnly = verifyOnly;

                    foreach (XElement step in element.Descendants("step"))
                    {
                        selectedSteps.Add(long.Parse(step.Value));
                    }


                    ExecutionTypes executionType = ExecutionTypes.Background;
                    try
                    {
                        executionType = (ExecutionTypes)Enum.Parse(typeof(ExecutionTypes), element.AttributeValue("executiontype"));
                    }
                    catch { }

                    long userId = -1;
                    long.TryParse(element.AttributeValue("userid"), out userId);

                    if (selectedSteps.Count == 0)
                    {
                        if (purgeOnly)
                        {
                            _logger.Trace(_className, methodName, string.Format("Purging campaign {0}, \"{1}\"", campaign.Id, campaign.Name));
                            _report.AppendLine(string.Format("Purging campaign {0}, \"{1}\"", campaign.Id, campaign.Name));
                            _stateManager.PurgeCampaign();
                        }
                        else
                        {
                            if (resume)
                            {
                                _logger.Trace(_className, methodName, string.Format("Resuming campaign {0}, \"{1}\"", campaign.Id, campaign.Name));
                                _report.AppendLine(string.Format("Resuming campaign {0}, \"{1}\"", campaign.Id, campaign.Name));
                                _stateManager.ResumeCampaign();
                            }
                            else
                            {
                                _logger.Trace(_className, methodName, string.Format("Executing campaign {0}, \"{1}\"", campaign.Id, campaign.Name));
                                _report.AppendLine(string.Format("Executing campaign {0}, \"{1}\"", campaign.Id, campaign.Name));
                                _stateManager.ExecuteCampaign();
                            }
                        }
                        _report.AppendLine(_stateManager.Report);
                    }
                    else
                    {
                        //selected steps can only happen from the campaign builder. This is not scheduled and does not need reporting.
                        List<long> steps = _stateManager.GetExecutionPlan();
                        foreach (long step in steps.ToList())
                        {
                            if (!selectedSteps.Contains(step))
                            {
                                steps.Remove(step);
                            }
                        }

                        _logger.Trace(_className, methodName, string.Format("{0} {1} selected steps of campaign {2}, \"{3}\"", purgeOnly ? "Purging" : "Executing", selectedSteps.Count, campaign.Id, campaign.Name));

                        if (purgeOnly)
                        {
                            _stateManager.PurgeCampaign(steps);
                        }
                        else
                        {
                            _stateManager.ExecuteCampaign(steps);
                        }
                    }

                    _logger.Trace(_className, methodName, string.Format("Finished executing campaign {0}, \"{1}\"", campaign.Id, campaign.Name));
                    _report.AppendLine(string.Format("Finished executing campaign {0}, \"{1}\"", campaign.Id, campaign.Name));
                    _logger.Trace(_className, methodName, "end");
                }
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Failed to execute campaign.", ex);
				throw;
			}
			return FrameWork.Common.ScheduleJobStatus.Success;
		}




		public string GetReport()
		{
			return _report == null ? string.Empty : _report.ToString();
		}

		public void RequestAbort()
		{
			if (_stateManager != null)
			{
				_stateManager.Abort();
			}
		}

        public void FinalizeJob(FrameWork.Common.ScheduleJobStatus jobStatus) { }

	}
}
