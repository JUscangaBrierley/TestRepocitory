#define ForSpeed

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.LWIntegration.Util;


namespace Brierley.FrameWork.CampaignManagement
{
	public class CircularPathException : Exception
	{
		public long DuplicateStepID = -1;

		public CircularPathException(string Message, long DuplicateStepID)
		{
			this.DuplicateStepID = DuplicateStepID;
		}
	}


	public class StateManager
	{
		private static List<long> _executing = new List<long>();
		private Campaign _campaign = null;
		private static LWLogger _logger = LWLoggerManager.GetLogger(Constants.LW_CM);
		private const string _className = "StateManager";
		private StringBuilder _report = null;
		private bool _includeReport = false;
		private volatile bool _cancelled = false;
        private string _org;
        private string _env;

		public bool AutoVerify { get; set; }
		public bool VerifyOnly { get; set; }

		public string Report
		{
			get
			{
				return _report != null ? _report.ToString() : string.Empty;
			}
		}


		public StateManager(Campaign campaign) : this(campaign, false) { }


		public StateManager(Campaign Campaign, bool includeReport)
		{
			_campaign = Campaign;
			_includeReport = includeReport;
			if (includeReport) _report = new StringBuilder();
            var context = LWConfigurationUtil.GetCurrentEnvironmentContext();
            _org = context.Organization;
            _env = context.Environment;
		}

		public StateManager(long campaignId, bool includeReport)
		{
            using (var manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                _campaign = (Campaign)manager.CacheManager.Get(Constants.CacheRegions.CampaignById, campaignId);

                if (_campaign == null)
                {
                    _campaign = manager.GetCampaign(campaignId);
                    manager.CacheManager.Update(Constants.CacheRegions.CampaignById, campaignId, _campaign);
                }
                _includeReport = includeReport;
                if (includeReport) _report = new StringBuilder();
                var context = LWConfigurationUtil.GetCurrentEnvironmentContext();
                _org = context.Organization;
                _env = context.Environment;
            }
		}


		public bool CanExecute(long StepID)
		{
            LWConfigurationUtil.SetCurrentEnvironmentContext(_org, _env);
            foreach (long inputStep in _campaign.Steps[StepID].Inputs)
			{
				var step = _campaign.Steps[inputStep];
				if (step.NeedsOutputTable())
				{
					if (step.OutputTableId == null)
					{
						return false;
					}
                    using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance(_org, _env))
					if (!manager.BatchProvider.TableExists(manager.GetCampaignTable((long)_campaign.Steps[inputStep].OutputTableId).Name, false))
					{
						return false;
					}
				}
				if (step.LastRunDate == null)
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>
		/// Determines whether linking two steps creates a circular reference anywhere in the campaign.
		/// </summary>
		/// <remarks>
		/// Linking a chain of steps together in a way that causes the ouput of a step to feed back into the
		/// input of the same step will cause infinite loops. This method adds the proposed step link to the 
		/// execution plan and checks to see if steps end up in the execution chain muiltiple times. If so, 
		/// the steps should not be allowed to be linked.
		/// For the simplest types of loops (step a connects to step b which then connects to step a), the UI 
		/// will handle error messaging to the user, but this is needed in more complex cases:
		///	a
		///	  -> b
		///	  -> c
		///        -> e
		///            -> i
		///            -> j
		///        -> f
		///            -> g
		///                 -> h
		///                      -> a
		///	  -> d
		///   -> e
		/// </remarks>
		/// <param name="StepID"></param>
		/// <param name="OutputStepID"></param>
		/// <returns></returns>
		public bool CreatesCircularReference(long StepID, long OutputStepID)
		{
			string methodName = "CreatesCircularReference";
			foreach (Step step in _campaign.Steps)
			{
				if (step.Inputs.Count == 0 && step.Outputs.Count == 0)
				{
					if (step.Id == StepID || step.Id == OutputStepID)
					{
						return false;
					}
				}
			}

			List<long> starterSteps = new List<long>();

			foreach (Step step in _campaign.Steps)
			{
				if (step.Inputs.Count == 0)
				{
					starterSteps.Add(step.Id);
				}
			}

			//if there is only one starting point in the campaign and that starting point is being
			//set as the output of another step, then a circular reference exists
			if (starterSteps.Count == 1 && starterSteps[0] == OutputStepID)
			{
				return true;
			}

			try
			{
				_campaign.Steps[StepID].Outputs.Add(OutputStepID, null, null, false);
				_campaign.Steps[OutputStepID].Inputs.Add(StepID, null, null, false);
				List<long> plan = GetExecutionPlan();

				if (plan.Count < _campaign.Steps.Count)
				{
					//steps in the campaign are missing from the plan, which means that steps exist that are not linked in some 
					//way to a starter step (step with no input step), which means a circular path exists somewhere.
					return true;
				}
			}
			catch (CircularPathException)
			{
				_logger.Debug(_className, methodName, string.Format("Circular execution path detected. Campaign ID: {0}, StepID: {1}, OutputStepID: {2}", _campaign.Id, StepID, OutputStepID));
				return true;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, string.Format("Error checking for circular execution path. Campaign ID: {0}, StepID: {1}, OutputStepID: {2}", _campaign.Id, StepID, OutputStepID), ex);
				throw;
			}
			return false;
		}


		public List<long> GetStepsNotRun(long StepID)
		{
			List<long> steps = new List<long>();
			foreach (int inputStep in _campaign.Steps[StepID].Inputs)
			{
				if (!HasStepBeenRun(inputStep))
				{
					steps.Add(inputStep);
				}
			}
			return steps;
		}


		private bool HasStepBeenRun(int StepID)
		{
			if (_campaign.Steps[StepID].LastRunDate != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Runs a real-time campaign
		/// </summary>
		public List<CampaignResult> ExecuteRealTimeCampaign(ContextObject co, Dictionary<string, string> overrideParameters = null)
		{
			List<long> plan = GetExecutionPlan();
			return ExecuteRealTimeCampaign(co, plan, overrideParameters);
		}

		/// <summary>
		/// Runs the entire campaign, start to finish
		/// </summary>
		/// <returns></returns>
		public void ExecuteCampaign(Dictionary<string, string> overrideParameters = null)
		{
			//SetThreadSlot();
			List<long> plan = GetExecutionPlan();
			ExecuteCampaign(plan, null, overrideParameters);
		}


		/// <summary>
		/// Runs the campaign, starting at the last point of execution
		/// </summary>
		/// <remarks>
		/// This method is primarily for campaign plans. A long running plan may be interrupted if the scheduler service is restarted, 
		/// and will need to recover and continue execution. This will restart execution at whatever step was last executing.
		/// </remarks>
		public void ResumeCampaign(Dictionary<string, string> overrideParameters = null)
		{
			/*
			We could throw the exception that's commented out below, but we're not able to resume a campaign step if we only allow plans to resume.
			The following scenario is trouble: 
				A plan executes. It begins a campaign step that has multiple output steps: one to give bonuses and another to give coupons. It's set to auto-verify.
				The campaign finishes the bonuses first, and the scheduler service is stopped. Upon resume, the plan executes the entire campaign again, which gives
				duplicate bonuses. That's not a continuation, but a restart.
			*/
			//if (_campaign.CampaignType != CampaignType.Plan)
			//{
			//	throw new NotSupportedException(string.Format("ResumeCampaign may only be invoked for Plans. Current campaign is {0}", _campaign.CampaignType.ToString()));
			//}

			List<long> plan = GetExecutionPlan();
			long? lastKnownStep = null;
			foreach (var step in _campaign.Steps)
			{
				if (step.IsExecuting.GetValueOrDefault())
				{
					lastKnownStep = step.Id;
					break;
				}
			}
			ExecuteCampaign(plan, lastKnownStep, overrideParameters);
		}

		/// <summary>
		/// Runs the entire campaign, start to finish
		/// </summary>
		/// <returns></returns>
		public void ExecuteCampaign(object steps)
		{
            LWConfigurationUtil.SetCurrentEnvironmentContext(_org, _env);
            if (steps is List<long>)
			{
				List<long> stepsToExecute = (List<long>)steps;

				//get actual execution plan, with correct execution order
				List<long> plan = GetExecutionPlan();


				//remove any steps in the execution plan that are not in the list to execute.
				//this results in a correctly ordered list of steps.
				foreach (long stepId in plan.ToList())
				{
					if (!stepsToExecute.Contains(stepId))
					{
						plan.Remove(stepId);
					}
				}

				ExecuteCampaign(plan);
			}
			else
			{
				throw new ArgumentException("Unexpected type passed for steps. Expected List<long>.", "steps");
			}
		}


		/// <summary>
		/// Runs the provided list of steps in a campaign.
		/// </summary>
		/// <param name="steps"></param>
		/// <param name="continueAtStep">If supplied, execution will continue at the step, without changing the execution date.</param>
		public void ExecuteCampaign(List<long> steps, long? continueAtStep = null, Dictionary<string, string> overrideParameters = null, long? executingUserId = null, ExecutionTypes? executionType = null)
		{
			const string methodName = "ExecuteCampaign";
			DateTime campaignStartTime = DateTime.Now;
            LWConfigurationUtil.SetCurrentEnvironmentContext(_org, _env);
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance(_org, _env))
            {
                try
                {

                    _logger.Trace(_className, methodName, "Begin, campaign id: " + _campaign.Id.ToString());
                    if (_includeReport) _report.AppendLine("Starting execution of campaign " + _campaign.Name);

                    if (overrideParameters == null)
                    {
                        //we need to make the reference here, otherwise it won't be passed through each step
                        overrideParameters = new Dictionary<string, string>();
                    }

                    lock (_executing)
                    {
                        if (_executing.Contains(_campaign.Id) || _campaign.Executing())
                        {
                            if (continueAtStep.HasValue)
                            {
                                _logger.Trace(_className, methodName, string.Format("Continuing execution of campaign {0}.", _campaign.Name));
                            }
                            else
                            {
                                _logger.Trace(_className, methodName, string.Format("Campaign {0} is already executing. Exiting.", _campaign.Name));
                                if (_includeReport) _report.AppendLine(string.Format("{0} The campaign is already being executed by another process. Exiting.", DateTime.Now.ToString()));
                                return;
                            }
                        }

                        _executing.Add(_campaign.Id);
                        _campaign.IsExecuting = true;
                        _campaign.ExecutionCancelled = false;
                        if (!_campaign.ExecutionType.HasValue)
                        {
                            _campaign.ExecutionType = executionType ?? ExecutionTypes.Schedule;
                        }
                        if (executingUserId.HasValue)
                        {
                            _campaign.ExecutionUserId = executingUserId.Value;
                        }
                        manager.UpdateCampaign(_campaign);
                    }

                    if (continueAtStep.HasValue)
                    {
                        foreach (long id in steps.ToList())
                        {
                            if (id == continueAtStep.Value)
                            {
                                break;
                            }
                            _logger.Trace(_className, methodName, string.Format("Step {0} is before the continuation point. Skipping.", _campaign.Steps[id].UIName));
                            steps.Remove(id);
                        }
                    }

                    foreach (long stepID in steps)
                    {
                        _campaign = manager.GetCampaign(_campaign.Id);
                        if (_campaign.ExecutionCancelled.GetValueOrDefault(false) || _cancelled)
                        {
                            _logger.Trace(_className, methodName, string.Format("Campaign {0} execution has been halted. Exiting.", _campaign.Id.ToString()));
                            _campaign.IsExecuting = false;
                            _campaign.ExecutionCancelled = false;
                            manager.UpdateCampaign(_campaign);
                            if (_includeReport) _report.AppendLine(string.Format("{0} Campaign execution has been halted. Exiting", DateTime.Now.ToString()));
                            break;
                        }

                        Step step = _campaign.Steps[stepID];
                        if (step.Query != null)
                        {
                            if (VerifyOnly && !step.RequiresVerification())
                            {
                                continue;
                            }
                            DateTime stepStartTime = DateTime.Now;
                            _logger.Trace(_className, methodName, "Executing step id " + step.Id.ToString());
                            if (_includeReport) _report.AppendLine(string.Format("{0} Executing Step {1}.", DateTime.Now.ToString(), step.UIName));

                            step.IsExecuting = true;
                            step.LastError = null;
                            if (step.Id != continueAtStep && step.ExecutionStart == null)
                            {
                                //we only want to set the execution start time if it hasn't been set already. There are time sensitive
                                //steps that may depend on this value (e.g., WaitQuery).
                                step.ExecutionStart = DateTime.Now;
                            }
                            manager.UpdateStep(step);

                            int rowsAffected = 0;

                            Exception threadException = null;
                            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
                            ContextObject co = new ContextObject();
                            co.Environment.Add(EnvironmentKeys.CurrentCampaign, _campaign.Name);
                            co.Environment.Add(EnvironmentKeys.OverrideParameters, overrideParameters);
                            Thread t = new Thread(
                                () =>
                                {
                                    try
                                    {
                                        LWConfigurationUtil.SetCurrentEnvironmentContext(ctx.Organization, ctx.Environment);
                                        if (!VerifyOnly)
                                        {
                                            if (!string.IsNullOrEmpty(step.EmailExecutionStartTo))
                                            {
                                                try
                                                {
                                                    SendStepStartEmail(step);
                                                }
                                                catch (Exception) { }
                                            }

                                            var results = step.Query.Execute(co, overrideParameters, step.Id == continueAtStep);
                                            foreach (var result in results)
                                            {
                                                rowsAffected += result.RowCount;
                                            }
                                        }
                                        if ((VerifyOnly || AutoVerify) && step.RequiresVerification())
                                        {
                                            rowsAffected = step.Query.Verify(co, overrideParameters);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        threadException = ex;
                                    }
                                }
                            );

                            t.Start();
                            while (t.IsAlive && threadException == null)
                            {
                                if (_cancelled)
                                {
                                    t.Interrupt();
                                    _report.AppendLine("Campaign execution cancelled.");
                                    break;
                                }
                                System.Threading.Thread.Sleep(500);
                            }

                            if (threadException != null)
                            {
                                try
                                {
                                    step.LastError = GetExceptionXml(threadException);
                                    step.UILastRecordCount = rowsAffected;
                                    step.LastRunDate = DateTime.Now;
                                    step.IsExecuting = false;
                                    step.ExecutionStart = null;
                                    step.UILastStatus = null;

                                    manager.UpdateStep(step);

                                    if (_includeReport) _report.AppendLine(string.Format("{0} Step execution failed:\r\n{1}.", DateTime.Now.ToString(), threadException.Message));
                                }
                                catch { }
                                throw threadException;
                            }

                            if (!_cancelled)
                            {
                                step.UILastRecordCount = rowsAffected;
                                step.LastRunDate = DateTime.Now;
                                if (step.RequiresVerification())
                                {
                                    if (VerifyOnly || AutoVerify)
                                    {
                                        step.VerificationState = VerificationState.Verified;
                                    }
                                    else
                                    {
                                        step.VerificationState = VerificationState.Pending;
                                    }
                                }
                            }
                            step.IsExecuting = false;
                            step.ExecutionStart = null;
                            step.LastError = null;
                            step.UILastStatus = null;
                            manager.UpdateStep(step);

                            if (!string.IsNullOrEmpty(step.EmailExecutionEndTo))
                            {
                                try
                                {
                                    SendStepEndEmail(step);
                                }
                                catch { }
                            }


                            if (_includeReport) _report.AppendLine(string.Format("{0} Finished executing Step. Row count = {1}", DateTime.Now.ToString(), rowsAffected));

                            _logger.Trace(_className, methodName, "Finished executing step id " + step.Id.ToString() + ". Execution time: " + ((TimeSpan)(DateTime.Now - campaignStartTime)).TotalSeconds.ToString() + " seconds.");
                        }
                    }

                    _campaign.IsExecuting = false;
                    _campaign.ExecutionCancelled = false;
                    manager.UpdateCampaign(_campaign);

                }
                catch (Exception ex)
                {
                    _logger.Error(_className, methodName, string.Empty, ex);

                    //attempt to reset execution properties
                    try
                    {
                        _campaign.IsExecuting = false;
                        _campaign.ExecutionCancelled = false;
                        manager.UpdateCampaign(_campaign);
                        foreach (Step step in _campaign.Steps)
                        {
                            if (step.IsExecuting.GetValueOrDefault(false))
                            {
                                step.IsExecuting = false;
                                step.ExecutionStart = null;
                                manager.UpdateStep(step);
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        _logger.Error(_className, methodName, "Failed to reset all execution properties", ex2);
                    }
                    if (_includeReport) _report.AppendLine(string.Format("{0} Campaign execution failed. \r\n{1}", DateTime.Now.ToString(), ex.Message));
                    throw;
                }
                finally
                {
                    lock (_executing)
                    {
                        if (_campaign != null && _executing.Contains(_campaign.Id))
                        {
                            _executing.Remove(_campaign.Id);
                        }
                    }
                }
            }
			_logger.Trace(_className, methodName, "End. Execution Time " + ((TimeSpan)(DateTime.Now - campaignStartTime)).TotalSeconds.ToString() + " seconds.");
		}


		/// <summary>
		/// Purges temp tables for the entire campaign, start to finish
		/// </summary>
		/// <returns></returns>
		public void PurgeCampaign()
		{
			List<long> plan = GetExecutionPlan();
			PurgeCampaign(plan);
		}

		/// <summary>
		/// Purges specified tables for the campaign
		/// </summary>
		/// <returns></returns>
		public void PurgeCampaign(object steps)
		{
			if (steps is List<long>)
			{
				List<long> stepsToPurge = (List<long>)steps;

				//get actual execution plan, with correct execution order
				List<long> plan = GetExecutionPlan();

				//remove any steps in the execution plan that are not in the list to execute.
				//this results in a correctly ordered list of steps.
				foreach (long stepId in plan.ToList())
				{
					if (!stepsToPurge.Contains(stepId))
					{
						plan.Remove(stepId);
					}
				}

				PurgeCampaign(plan);
			}
			else
			{
				throw new ArgumentException("Unexpected type passed for steps. Expected List<long>.", "steps");
			}
		}


		/// <summary>
		/// Purges the provided list of steps in a campaign.
		/// </summary>
		/// <param name="steps"></param>
		public void PurgeCampaign(List<long> steps)
		{
			const string methodName = "PurgeCampaign";
			DateTime campaignStartTime = DateTime.Now;
            LWConfigurationUtil.SetCurrentEnvironmentContext(_org, _env);
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance(_org, _env))
            {
                try
                {

                    _logger.Trace(_className, methodName, "Begin, campaign id: " + _campaign.Id.ToString());
                    if (_includeReport) _report.AppendLine("Starting purge of campaign " + _campaign.Name);


                    if (_campaign.Executing() && _campaign.ExecutionType == ExecutionTypes.Schedule)
                    {
                        _logger.Trace(_className, methodName, string.Format("Campaign {0} is already executing. Exiting.", _campaign.Id.ToString()));
                        if (_includeReport) _report.AppendLine(string.Format("{0} The campaign is already being executed by another process. Exiting.", DateTime.Now.ToString()));
                        return;
                    }

                    _campaign.IsExecuting = true;
                    _campaign.ExecutionCancelled = false;
                    if (!_campaign.ExecutionType.HasValue)
                    {
                        _campaign.ExecutionType = ExecutionTypes.Schedule;
                    }
                    manager.UpdateCampaign(_campaign);

                    foreach (long stepID in steps)
                    {
                        _campaign = manager.GetCampaign(_campaign.Id);
                        if (_campaign.ExecutionCancelled.GetValueOrDefault(false) || _cancelled)
                        {
                            _logger.Trace(_className, methodName, string.Format("Campaign {0} purge has been halted. Exiting.", _campaign.Id.ToString()));
                            _campaign.IsExecuting = false;
                            _campaign.ExecutionCancelled = false;
                            manager.UpdateCampaign(_campaign);
                            if (_includeReport) _report.AppendLine(string.Format("{0} Campaign purge has been halted. Exiting", DateTime.Now.ToString()));
                            break;
                        }

                        Step step = _campaign.Steps[stepID];
                        if (step.Query != null && step.OutputTableId.HasValue)
                        {
                            DateTime stepStartTime = DateTime.Now;
                            _logger.Trace(_className, methodName, "Purging step id " + step.Id.ToString());
                            if (_includeReport) _report.AppendLine(string.Format("{0} Purging Step {1}.", DateTime.Now.ToString(), step.UIName));

                            step.IsExecuting = true;
                            step.LastError = null;
                            step.ExecutionStart = DateTime.Now;
                            manager.UpdateStep(step);


                            CampaignTable table = manager.GetCampaignTable((long)step.OutputTableId);
                            if (table != null && manager.BatchProvider.TableExists(table.Name, false))
                            {
                                manager.BatchProvider.TruncateTable(table.Name);
                            }

                            step.UILastRecordCount = 0;
                            step.LastRunDate = DateTime.Now;
                            if (step.RequiresVerification())
                            {
                                step.VerificationState = VerificationState.None;
                            }

                            step.IsExecuting = false;
                            step.ExecutionStart = null;
                            step.LastError = null;
                            manager.UpdateStep(step);

                            if (_includeReport) _report.AppendLine(string.Format("{0} Finished purging Step.", DateTime.Now.ToString()));

                            _logger.Trace(_className, methodName, "Finished purging step id " + step.Id.ToString() + ". Execution time: " + ((TimeSpan)(DateTime.Now - campaignStartTime)).TotalSeconds.ToString() + " seconds.");
                        }
                    }

                    _campaign.IsExecuting = false;
                    _campaign.ExecutionCancelled = false;
                    manager.UpdateCampaign(_campaign);

                }
                catch (Exception ex)
                {
                    _logger.Error(_className, methodName, string.Empty, ex);

                    //attempt to reset execution properties
                    try
                    {
                        _campaign.IsExecuting = false;
                        _campaign.ExecutionCancelled = false;
                        manager.UpdateCampaign(_campaign);
                        foreach (Step step in _campaign.Steps)
                        {
                            if (step.IsExecuting.GetValueOrDefault(false))
                            {
                                step.IsExecuting = false;
                                step.ExecutionStart = null;
                                manager.UpdateStep(step);
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        _logger.Error(_className, methodName, "Failed to reset all execution properties", ex2);
                    }
                    if (_includeReport) _report.AppendLine(string.Format("{0} Campaign execution failed. \r\n{1}", DateTime.Now.ToString(), ex.Message));
                    throw;
                }
            }
			_logger.Trace(_className, methodName, "End. Execution Time " + ((TimeSpan)(DateTime.Now - campaignStartTime)).TotalSeconds.ToString() + " seconds.");
		}


		/// <summary>
		/// Runs the provided list of steps in a campaign.
		/// </summary>
		/// <param name="steps"></param>
		public List<CampaignResult> ExecuteRealTimeCampaign(ContextObject co, List<long> steps, Dictionary<string, string> overrideParameters = null)
		{
			const string methodName = "ExecuteRealTimeCampaign";
			DateTime campaignStartTime = DateTime.Now;
			var results = new List<CampaignResult>();
			try
			{
				if (overrideParameters == null)
				{
					overrideParameters = new Dictionary<string, string>();
				}
				if (co.Environment == null)
				{
					co.Environment = new Dictionary<string, object>();
				}
				if (!co.Environment.ContainsKey("CurrentCampaign"))
				{
					co.Environment.Add(EnvironmentKeys.CurrentCampaign, _campaign.Name);
				}
				if (!co.Environment.ContainsKey("OverrideParameters"))
				{
					co.Environment.Add(EnvironmentKeys.OverrideParameters, overrideParameters);
				}

				//Create a local map of row counts - row counts were originally assigned to the step, but this could have caused
				//issues with multiple member campaigns running against a single cached instance of a campaign.
				var rowCounts = new Dictionary<long, long>();

				_logger.Trace(_className, methodName, "Begin, campaign id: " + _campaign.Id.ToString());
				if (_includeReport) _report.AppendLine("Starting execution of campaign " + _campaign.Name);

				foreach (long stepID in steps)
				{
					Step step = _campaign.Steps[stepID];
					if (step.Inputs.Count > 0)
					{
						var firstInput = step.Inputs[0];
						if (!rowCounts.ContainsKey(firstInput) || rowCounts[firstInput] < 1)
						{
							//skip this step, its input failed or returned no records.
							continue;
						}
					}
					if (step.Query != null)
					{
						DateTime stepStartTime = DateTime.Now;
						_logger.Trace(_className, methodName, "Executing step id " + step.Id.ToString());
						if (_includeReport) _report.AppendLine(string.Format("{0} Executing Step {1}.", DateTime.Now.ToString(), step.UIName));

						string status = string.Empty;
						List<CampaignResult> result = step.Query.Execute(co, overrideParameters);
						long rowsAffected = 0;
						foreach (var r in result)
						{
							rowsAffected += r.RowCount;
						}
						rowCounts.Add(step.Id, rowsAffected);

						if (step.StepType == StepType.RealTimeOutput)
						{
							var outputQuery = step.Query as RealTimeOutputQuery;
							if (
								//most output types:
								(rowsAffected > 0 && outputQuery != null && outputQuery.OutputOption != null) ||
								//or triggered email that resulted in failure causing the email to fall into the retry queue:
								(rowsAffected < 0 && outputQuery.OutputOption.OutputType == OutputType.Email)
								)
							{
								results.AddRange(result);
							}
						}


						//rowsAffected may be a member reference id, if step is an output step. No need really to show any number other than one or zero. 
						if (_includeReport) _report.AppendLine(string.Format("{0} Finished executing Step. Status = {1}", DateTime.Now.ToString(), status));

						_logger.Trace(_className, methodName, "Finished executing step id " + step.Id.ToString() + ". Execution time: " + ((TimeSpan)(DateTime.Now - campaignStartTime)).TotalSeconds.ToString() + " seconds.");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, string.Empty, ex);
				if (_includeReport) _report.AppendLine(string.Format("{0} Campaign execution failed. \r\n{1}", DateTime.Now.ToString(), ex.Message));
				throw;
			}
			_logger.Trace(_className, methodName, "End. Execution Time " + ((TimeSpan)(DateTime.Now - campaignStartTime)).TotalSeconds.ToString() + " seconds.");
			return results;
		}

		/// <summary>
		/// Abort the current campaign execution
		/// </summary>
		public void Abort()
		{
			_cancelled = true;
		}

		/// <summary>
		/// Builds an execution plan for the campaign.
		/// </summary>
		/// <returns></returns>
		public List<long> GetExecutionPlan()
		{
			const string methodName = "GetExecutionPlan";
			List<long> executionSteps = new List<long>();
			List<long> starterSteps = new List<long>();

			foreach (Step step in _campaign.Steps.OrderBy(o => o.ExecutionPriority.GetValueOrDefault(int.MaxValue)).ThenBy(o => o.UIPositionX).ThenByDescending(o => o.UIPositionY))
			{
				if (step.Inputs.Count == 0)
				{
					starterSteps.Add(step.Id);
				}
			}

			try
			{
				foreach (long step in starterSteps)
				{
					if (!executionSteps.Contains(step))
					{
						executionSteps.Add(step);
						GetNextSteps(step, executionSteps, starterSteps);
					}
				}
			}
			catch (StackOverflowException ex)
			{
				// Possibly caused by a circular step reference (step A outputs to step B, which outputs to step A)
				// This should have been caught by campaign validation. Log this error along with a list of steps and their
				// input/output list so we can check it.
				string exceptionData = string.Empty;
				if (executionSteps != null && executionSteps.Count > 0)
				{
					foreach (long step in executionSteps)
					{
						if (!string.IsNullOrEmpty(exceptionData))
						{
							exceptionData += ", ";
						}
						exceptionData += step.ToString();
					}
				}
				_logger.Error(_className, methodName, string.Format("StackOverflowException caught while attempting to get the execution plan. CampaignID: {0}, execution plan: {1}", _campaign.Id, exceptionData), ex);
				throw;
			}

			if (executionSteps.Count < _campaign.Steps.Count)
			{
				//steps in the campaign are missing from the plan, which means that steps exist that are not linked in some 
				//way to a starter step (step with no input step), which means a circular path exists somewhere.
				long missingStep = -1;
				foreach (Step step in _campaign.Steps)
				{
					if (!executionSteps.Contains(step.Id))
					{
						missingStep = step.Id;
						break;
					}
				}
				_logger.Error(_className, methodName, string.Format("Error getting execution plan. Plan contains fewer steps than the campaign. CampaignID: {0}", _campaign.Id));
				throw new CircularPathException("Circular execution path detected", missingStep);
			}
			return executionSteps;
		}



		private void GetNextSteps(long StepID, List<long> ExecutionSteps, List<long> StarterSteps)
		{
			var outputs = new List<long>();

			var ordered = new Dictionary<long, System.Drawing.Point>();
			foreach (long outputID in _campaign.Steps[StepID].Outputs)
			{
				Step step = _campaign.Steps[outputID];
				ordered.Add(step.Id, new System.Drawing.Point(step.UIPositionX, step.UIPositionY));
			}

			outputs.AddRange(ordered.OrderBy(o => o.Value.X).ThenBy(o => o.Value.Y).Select(o => o.Key));

			foreach (long outputID in outputs)
			{
				Step step = _campaign.Steps[outputID];
				bool isDependant = false;
				foreach (long dependencyID in step.Inputs)
				{
					if (dependencyID != StepID && !ExecutionSteps.Contains(dependencyID))
					{
						//step has a dependency that has not been added yet. 
						//the step will be picked up after its 
						//dependency is added to the execution plan
						isDependant = true;
					}
				}
				if (isDependant)
				{
					continue;
				}
				if (!ExecutionSteps.Contains(outputID))
				{
					ExecutionSteps.Add(outputID);
					GetNextSteps(outputID, ExecutionSteps, StarterSteps);
				}
				else
				{
					if (!StarterSteps.Contains(outputID))
					{
						throw new CircularPathException("Circular execution path detected.", outputID);
					}
				}
			}
		}

		private string GetExceptionXml(Exception ex)
		{
			XElement root = new XElement("exception",
				new XAttribute("time", DateTime.Now.ToString()),
				new XAttribute("message", ex.Message),
				new XAttribute("stacktrace", ex.StackTrace)
				);

			ex = ex.InnerException;
			while (ex != null)
			{
				root.Add(new XElement("innerexception",
					new XAttribute("message", ex.Message),
					new XAttribute("stacktrace", ex.StackTrace)
					));
				ex = ex.InnerException;
			}
			return root.ToString();
		}

		private void SendStepStartEmail(Step step)
		{
			string subjectAndBody  = string.Format("Step {0} is starting execution.", step.UIName);
			SendEmail(step.EmailExecutionStartTo, subjectAndBody, subjectAndBody);
		}

		private void SendStepEndEmail(Step step)
		{
			string subject = string.Format("Step {0} has finished execution.", step.UIName);
			string body = string.Format("Step {0} has finished execution.<br/>Row Count: {1}", step.UIName, step.UILastRecordCount);
			if (!string.IsNullOrEmpty(step.UILastStatus))
			{
				body += string.Format("<br/>Status:", step.UILastStatus);
			}
			if (!string.IsNullOrEmpty(step.LastError))
			{
				body += string.Format("<br/>Error:", step.LastError);
			}
			SendEmail(step.EmailExecutionStartTo, subject, body);
		}

		private static void SendEmail(string to, string subject, string body)
		{
			const string methodName = "SendEmail";

			try
			{
				string smtpServer = LWConfigurationUtil.GetConfigurationValue("SmtpServer");
				if (!string.IsNullOrEmpty(smtpServer))
				{
					//create the mail message
					System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();

					//set the addresses
					mail.From = new System.Net.Mail.MailAddress("loyaltynavigator@brierley.com");
					string[] addresses = to.Split(';');
					foreach (string address in addresses)
					{
						mail.To.Add(address);
					}

					mail.Subject = subject;
					mail.Body = StringUtils.HtmlFriendly(body);
					mail.IsBodyHtml = true;

					System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpServer);

					//smtp.Port = 25;
					smtp.Send(mail);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error sending step execution email.", ex);
			}
		}



	}
}
