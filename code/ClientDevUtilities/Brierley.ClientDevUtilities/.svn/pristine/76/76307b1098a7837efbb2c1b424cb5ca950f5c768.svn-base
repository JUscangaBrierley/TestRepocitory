using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.Sql;

namespace Brierley.FrameWork.JobScheduler.Jobs
{
	public class LodReportAggregationJob : IJob
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_JOBSCHEDULER);
		private const string _className = "LodReportAggregationJob";

		private StringBuilder _report = null;

		private Task _task;
		private CancellationTokenSource _cancelToken;

		public void SetRunID(long runID)
		{
		}

		public FrameWork.Common.ScheduleJobStatus Run(string parms)
		{
			const string methodName = "Run";

			Action<object> executeProcs = delegate(object state)
			{
				ServiceConfig config = (ServiceConfig)state;
				LWConfigurationUtil.SetCurrentEnvironmentContext(config.Organization, config.Environment);


				XElement element = XElement.Parse(parms);
				bool memberEnrollmentSummary = element.AttributeValue("MemberEnrollmentSummary").Equals("true", StringComparison.OrdinalIgnoreCase);
				bool memberSalesSummary = element.AttributeValue("MemberSalesSummary").Equals("true", StringComparison.OrdinalIgnoreCase);
				bool pointsSummary = element.AttributeValue("PointsSummary").Equals("true", StringComparison.OrdinalIgnoreCase);
				bool rewardsSummary = element.AttributeValue("RewardsSummary").Equals("true", StringComparison.OrdinalIgnoreCase);
				bool topPointEarners = element.AttributeValue("TopPointEarners").Equals("true", StringComparison.OrdinalIgnoreCase);
				bool topRewardredeemers = element.AttributeValue("TopRewardRedeemers").Equals("true", StringComparison.OrdinalIgnoreCase);
                bool topRewardRedeemerVisits = element.AttributeValue("TopRewardRedeemerVisits").Equals("true", StringComparison.OrdinalIgnoreCase);
                bool csrAdjustments = element.AttributeValue("CsrAdjustments").Equals("true", StringComparison.OrdinalIgnoreCase);
				bool mobileEvents = element.AttributeValue("MobileEvents").Equals("true", StringComparison.OrdinalIgnoreCase);
				bool topVisitors = element.AttributeValue("TopVisitors").Equals("true", StringComparison.OrdinalIgnoreCase);

				Action<string, string> exec = delegate(string description, string proc)
				{
					try
					{
						LWQueryUtil queryUtil = new LWQueryUtil();
						DateTime start = DateTime.MinValue;
						TimeSpan elapsed = TimeSpan.FromTicks(0);

						string trace = string.Format("Executing {0} ({1})", description, proc);
						_logger.Trace(_className, methodName, trace);
						_report.AppendLine(trace);

						if (_cancelToken.Token.IsCancellationRequested)
						{
							_logger.Trace(_className, methodName, "Job cancellation was requested. Exiting.");
							_report.AppendLine("Job cancellation was requested. Exiting.");
							return;
						}

						start = DateTime.Now;
						using (var uselessReader = queryUtil.ExecuteStoredProc(proc, null, false))
						{
						}
						elapsed = DateTime.Now - start;

						trace = string.Format("Finished executing member enrollment summary in {0}:{1}", elapsed.Minutes, elapsed.Seconds);

						_logger.Trace(_className, methodName, trace);
						_report.AppendLine(trace);
					}
					catch (Exception ex)
					{
						string error = string.Format("Execution of aggregation procedure {0} ({1}) failed.", description, proc);
						_logger.Error(_className, methodName, error, ex);
						_report.AppendLine(error);
						throw;
					}
				};

				if (memberEnrollmentSummary)
				{
					exec("member enrollment summary", "RPT_POPULATE_LOD.build_mbr_enrollment_summary");
				}

				if (memberSalesSummary)
				{
					exec("member sales summary", "RPT_POPULATE_LOD.build_mbr_sales_summary");
				}

				if (pointsSummary)
				{
					exec("point summary", "RPT_POPULATE_LOD.build_points_summary");
				}

				if (rewardsSummary)
				{
					exec("reward summary", "RPT_POPULATE_LOD.build_rewards_summary");
				}

				if (topPointEarners)
				{
					exec("top point earners", "RPT_POPULATE_LOD.build_LP_top_points_earners");
				}

				if (topRewardredeemers)
				{
					exec("top reward redeemers", "RPT_POPULATE_LOD.build_LP_top_reward_redeemers");
				}

                if (topRewardRedeemerVisits)
                {
                    exec("top reward redeemers", "RPT_POPULATE_LOD.build_LP_top_rwrd_redeemers_v");
                }

				if (csrAdjustments)
				{
					exec("customer service adjustments", "RPT_POPULATE_LOD.build_LP_CSR_adjustments");
				}

				if (mobileEvents)
				{
					exec("customer service adjustments", "RPT_POPULATE_LOD.build_mobile_event_summary");
				}

				if (topVisitors)
				{
					exec("top visitors", "RPT_POPULATE_LOD.build_top_visits");					
				}
			};

			try
			{
				_report = new StringBuilder();

				var config = LWDataServiceUtil.GetServiceConfiguration();
				_cancelToken = new CancellationTokenSource();
				_task = new Task(executeProcs, config, _cancelToken.Token);

				_task.Start();
				_task.Wait();

				if (_task.IsCanceled)
				{
					return ScheduleJobStatus.Cancelled;
				}

				return ScheduleJobStatus.Success;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Failed to execute job.", ex);
				throw;
			}
		}

		public FrameWork.Common.ScheduleJobStatus Resume(string parms)
		{
			throw new NotImplementedException("LodReportAggregationJob cannot resume because it does not save its state.");
		}

		public string GetReport()
		{
			return _report == null ? string.Empty : _report.ToString();
		}

		public void RequestAbort()
		{
			_cancelToken.Cancel();
		}

		public void FinalizeJob(FrameWork.Common.ScheduleJobStatus jobStatus) { }

	}
}
