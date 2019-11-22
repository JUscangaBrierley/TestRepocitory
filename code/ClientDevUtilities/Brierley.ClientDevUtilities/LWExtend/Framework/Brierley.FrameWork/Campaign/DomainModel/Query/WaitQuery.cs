using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;


namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public enum WaitType
	{
		Verification,
		Time
	}


	public class WaitQuery : Query
	{
		private long? _waitAmount;
		private DateTime? _waitUntil;
		private static LWLogger _logger = LWLoggerManager.GetLogger(Constants.LW_CM);
		private const string _className = "WaitQuery";
		
		/// <summary>
		/// Gets or sets the wait type for the step
		/// </summary>
		public WaitType WaitType { get; set; }


		/// <summary>
		/// Gets or sets the interval to wait for
		/// </summary>
		public DateParts? WaitInterval { get; set; }


		/// <summary>
		/// Gets or sets the wait amount for the selected interval
		/// </summary>
		public long? WaitAmount
		{
			get
			{
				return _waitAmount;
			}
			set
			{
				_waitAmount = value;
				if (_waitAmount.HasValue)
				{
					WaitUntil = null;
				}
			}
		}
		

		/// <summary>
		/// Gets or sets the exact date and time to wait until.
		/// </summary>
		public DateTime? WaitUntil
		{
			get
			{
				return _waitUntil;
			}
			set
			{
				_waitUntil = value;
				if (_waitUntil.HasValue)
				{
					WaitAmount = null;
				}
			}
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
			if (WaitType == DomainModel.WaitType.Time)
			{
				if (!WaitAmount.HasValue && !WaitUntil.HasValue)
				{
					Warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("A wait amount is required for step {0}", Step.UIName)));
					return false;
				}
			}

            using (var mgr = LWDataServiceUtil.CampaignManagerInstance())
            {
                if (WaitType == DomainModel.WaitType.Verification)
                {
                    //step must have a single input step
                    if (Step.InputCount != 1)
                    {
                        Warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Step {0} has a wait type of verification, which requires a single campaign input step.", Step.UIName)));
                        return false;
                    }

                    //input step must be a campaign step
                    var input = mgr.GetStep(Step.Inputs[0]);
                    if (input.StepType != StepType.Campaign)
                    {
                        Warnings.Add(
                            new ValidationMessage(
                                ValidationLevel.Exception,
                                string.Format("Step {0} has a wait type of verification, which requires a single campaign input step. Current input step type is {1}", Step.UIName, input.StepType.ToString()))
                        );
                        return false;
                    }

                    //If the input step has no selected campaign, the campaign step itself will trigger a validation error. Nothing for us to do here but exit.
                    var query = input.Query as CampaignQuery;
                    if (string.IsNullOrEmpty(query.CampaignName))
                    {
                        return true;
                    }

                    //the input step's campaign must exist and must have at least one output step that can be verified by a human
                    var campaign = mgr.GetCampaign(query.CampaignName);
                    int outputCount = campaign.Steps.Where(o => o.StepType == StepType.Output).Count();
                    if (outputCount < 1)
                    {
                        Warnings.Add(
                            new ValidationMessage(
                                ValidationLevel.Exception,
                                string.Format("Step {0} has a wait type of verification, but its input step {1} executes a campaign with no output steps to be verified.", Step.UIName, input.UIName))
                            );
                        return false;
                    }
                }
                return true;
            }
		}


		internal override List<CampaignResult> Execute(FrameWork.ContextObject co = null, Dictionary<string, string> overrideParameters = null, bool resume = false)
		{
			const string methodName = "Execute";

			_logger.Debug(_className, methodName, string.Format("Executing wait query {0} \"{1}\".", Step.Id, Step.UIName));

			List<ValidationMessage> warnings = new List<ValidationMessage>();
			if (!Validate(warnings, true))
			{
				string exception = "Failed to execute " + Step.UIName + " because the step is invalid.";
				foreach (ValidationMessage message in warnings)
				{
					exception += message.Message;
				}
				throw new Exception(exception);
			}

			if (WaitType == DomainModel.WaitType.Time)
			{
				//Using the step's ExecutionStart as our wait starting point allows the step to continue execution if the scheduler service
				//fails and/or is restarted by infrastructure. If there is no execution start time available, we'll default to current date
				DateTime waitUntil = WaitUntil ?? GetWaitTime();
				TimeSpan executionOffset = DateTime.Now - Step.ExecutionStart.GetValueOrDefault(DateTime.Now);
				waitUntil = waitUntil.Subtract(executionOffset);
				
				_logger.Debug(_className, methodName, 
				string.Format
				(
					"Wait query {0} \"{1}\", execution began at {2}, will sleep until {3}", 
					Step.Id, 
					Step.UIName, 
					Step.ExecutionStart.GetValueOrDefault(DateTime.Now).ToString(), 
					waitUntil.ToString())
				);

				while (DateTime.Now < waitUntil)
				{
					Thread.Sleep(1000);
				}
			}
			else
			{
				const int sleepTime = 60000;
				DateTime waitStart = Step.ExecutionStart.GetValueOrDefault(DateTime.Now);
                using (var mgr = LWDataServiceUtil.CampaignManagerInstance())
                {
                    var input = mgr.GetStep(Step.Inputs[0]);
                    var query = input.Query as CampaignQuery;
                    var campaignName = query.CampaignName;
                    var campaign = mgr.GetCampaign(campaignName);
                    IEnumerable<long> outputSteps = (from x in campaign.Steps where x.StepType == StepType.Output select x.Id);

                    _logger.Debug(_className, methodName, string.Format("Wait query {0} \"{1}\" is waiting for verification of input step's campaign \"{2}\"", Step.Id, Step.UIName, campaign.Name));

                    long attempts = 0;
                    while (true)
                    {
                        bool allVerified = true;
                        foreach (var id in outputSteps)
                        {
                            var step = mgr.GetStep(id);
                            if (step.RequiresVerification() && step.VerificationState.GetValueOrDefault() != VerificationState.Verified)
                            {
                                allVerified = false;
                                break;
                            }
                        }

                        if (allVerified)
                        {
                            _logger.Debug(_className, methodName, string.Format("Input step of wait query {0} \"{1}\" has been verified.", Step.Id, Step.UIName));
                            break;
                        }

                        Thread.Sleep(sleepTime);

                        if (++attempts % 100 == 0)
                        {
                            _logger.Debug(_className, methodName, string.Format("Wait query {0} \"{1}\" has been awaiting verification of its input step for {2}.", Step.Id, Step.UIName, (DateTime.Now - waitStart).ToString()));
                        }
                    }
                }
			}
			_logger.Debug(_className, methodName, string.Format("Finished executing wait query {0} \"{1}\".", Step.Id, Step.UIName));
			return new List<CampaignResult>() { new CampaignResult(1) };
		}

		private DateTime GetWaitTime()
		{
			DateTime ret = DateTime.Now;
			long amount = WaitAmount.GetValueOrDefault();
			switch(WaitInterval)
			{
				case DateParts.Day:
					ret = ret.AddDays(amount);
					break;
				case DateParts.Hour:
					ret = ret.AddHours(amount);
					break;
				case DateParts.Minute:
					ret = ret.AddMinutes(amount);
					break;
				case DateParts.Second:
					ret = ret.AddSeconds(amount);
					break;
			}
			return ret;
		}

	}
}
