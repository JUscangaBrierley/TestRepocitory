using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("CampaignId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLCampaign")]
	public class Campaign : LWCoreObjectBase
	{
		private StepCollection _steps = null;

		[XmlIgnore]
		public StepCollection Steps
		{
			get
			{
				if (_steps == null)
				{
					_steps = new StepCollection(Id);
				}
				return _steps;
			}
		}

		/// <summary>
		/// The unique ID of the campaign
		/// </summary>
		[PetaPoco.Column("CampaignId")]
		public long Id { get; set; }

		/// <summary>
		/// The type of the campaign.
		/// </summary>
		[PetaPoco.Column]
		public CampaignType CampaignType { get; set; }

		/// <summary>
		/// The name of the campaign
		/// </summary>
        [PetaPoco.Column("CampaignName", Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = true)]
        public string Name { get; set; }

		/// <summary>
		/// The description of the campaign
		/// </summary>
		[PetaPoco.Column("CampaignDescription", Length = 500)]
		public string Description { get; set; }

		/// <summary>
		/// Id of the folder the campaign is stored in
		/// </summary>
		[PetaPoco.Column]
		public long? FolderId { get; set; }

		/// <summary>
		/// Is the campaign a template for other campaigns?
		/// </summary>
		[PetaPoco.Column]
		public bool IsTemplate { get; set; }

		/// <summary>
		/// describes how the campaign is being executed
		/// </summary>
		[PetaPoco.Column]
		public ExecutionTypes? ExecutionType { get; set; }

		/// <summary>
		/// The id of the user who is currently executing the campaign.
		/// </summary>
		[PetaPoco.Column]
		public long? ExecutionUserId { get; set; }

		/// <summary>
		/// Is the campaign - or a step in the campaign - currently being executed?
		/// </summary>
		[PetaPoco.Column]
		public bool? IsExecuting { get; set; }

		/// <summary>
		/// Has a cancellation request been made for the campaign?
		/// </summary>
		/// <remarks>
		/// This is used to determine if the state manager should halt execution of the campaign. Initially, campaigns running in the background
		/// will not use a windows service and will instead be executed on a separate thread in asp.net. This property will be set by a campaign 
		/// manager to request that the campaign executing in the background halt execution.
		/// </remarks>
		[PetaPoco.Column]
		public bool? ExecutionCancelled { get; set; }


		/// <summary>
		/// Gets or sets the configuration xml for the campagin. Today, this value houses campaign builder state (zoom, canvas position etc.), 
		/// but may eventually contain step and query data.
		/// </summary>
		[PetaPoco.Column]
		public string Config { get; set; }

		/// <summary>
		/// Initializes a new instance of the Campaign class
		/// </summary>
		public Campaign()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Campaign class
		/// </summary>
		/// <param name="name">Initial <see cref="Campaign.Name" /> value</param>
		/// <param name="description">Initial <see cref="Campaign.Description" /> value</param>
		/// <param name="createDate">Initial <see cref="Campaign.CreateDate" /> value</param>
		public Campaign(string name, string description, DateTime createDate)
		{
			Name = name;
			Description = description;
			CreateDate = createDate;
		}

		/// <summary>
		/// Validates the campaign.
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		/// <param name="warnings"></param>
		/// <param name="validateSql"></param>
		/// <returns></returns>
		public bool Validate(ref List<ValidationMessage> warnings, bool validateSql)
		{
			StateManager stateManager = new StateManager(this);
			try
			{
				stateManager.GetExecutionPlan();
			}
			catch (CircularPathException ex)
			{
				warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("The campaign cannot be validated because a circular execution path was detected at step {0}.", Steps[ex.DuplicateStepID].UIName)));
				return false;
			}

			List<long> invalidSteps = new List<long>();

			foreach (Step step in Steps)
			{
				if (step.Query == null)
				{
					warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("No query exists for step \"{0}\".", step.UIName)));
					invalidSteps.Add(step.Id);
					continue;
				}
				else
				{
					if (step.StepType == StepType.Select && step.Inputs.Count == 0 && step.KeyId == null)
					{
						warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Step \"{0}\" requires an audience level and none has been selected.", step.UIName)));
						invalidSteps.Add(step.Id);
						continue;
					}
				}

				if (step.StepType == StepType.Merge)
				{
					if (step.Inputs.Count == 0)
					{
						warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Step \"{0}\" does not have any input steps. Merge steps require at least two input steps.", step.UIName)));
						invalidSteps.Add(step.Id);
						continue;
					}
					else if (step.Inputs.Count == 1)
					{
						warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Step \"{0}\" only contains one input step. Merge steps require at least two input steps.", step.UIName)));
						invalidSteps.Add(step.Id);
						continue;
					}
					else
					{
						//make sure all input steps share a common audience level.
						List<long> audiences = new List<long>();
						foreach (long stepID in step.Inputs)
						{
							if (Steps[stepID].Query != null && step.Key != null)
							{
								if (!audiences.Contains(step.Key.AudienceId))
								{
									audiences.Add(step.Key.AudienceId);
								}
							}
						}
						if (audiences.Count > 1)
						{
							warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Step \"{0}\" is attempting to merge different audience levels. All input steps must have the same audience level in order to merge.", step.UIName)));
							invalidSteps.Add(step.Id);
						}
					}
				}
				else
				{
					//non-merge step type
					if (step.Inputs.Count > 1)
					{
						warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Step \"{0}\" contains multiple input steps. Only merge steps may accept multiple input steps.", step.UIName)));
						invalidSteps.Add(step.Id);
						continue;
					}
					else
					{
						if (step.Inputs.Count == 0 && step.RequiresInputStep())
						{
							warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Step \"{0}\" requires an input step.", step.UIName)));
							invalidSteps.Add(step.Id);
							continue;
						}
					}
				}

				if (step.Outputs.Count > 1)
				{
					//ensure that segment steps at the next level all have the same segment name (this is only a warning, not an exception)
					List<string> segmentNames = new List<string>();
					foreach (long stepID in step.Outputs)
					{
						var splitStep = Steps[stepID];
						if (splitStep.StepType == StepType.SplitProcess)
						{
							string segmentName = ((SplitProcessQuery)splitStep.Query).ProcessName.Trim().ToLower();
							if (!segmentNames.Contains(segmentName))
							{
								segmentNames.Add(segmentName);
							}
						}
					}
					if (segmentNames.Count > 1)
					{
						//multiple segment names exist at the same level of segmentation.
						string errorMessage = "Step \"{0}\" outputs to segmentation steps which do not have matching segment names. Please ensure that all segment names have been spelled correctly:\r\n";
						foreach (string name in segmentNames)
						{
							errorMessage += name + "\r\n";
						}
						warnings.Add(new ValidationMessage(ValidationLevel.Warning, string.Format(errorMessage, step.UIName)));
					}
				}
			}

			foreach (Step step in Steps)
			{
				if (invalidSteps.Contains(step.Id))
				{
					continue;
				}

				bool invalid = false;
				foreach (long input in step.Inputs)
				{
					if (invalidSteps.Contains(input))
					{
						invalid = true;
						break;
					}
				}
				if (invalid)
				{
					continue;
				}

				if (!step.Query.Validate(warnings, validateSql))
				{
					invalidSteps.Add(step.Id);
				}
			}

			if (invalidSteps.Count > 0)
			{
				return false; //invalid campaign
			}
			else
			{
				return true; //valid campaign - could possibly have warnings, though.
			}
		}

		public bool Executing()
		{
			if (IsExecuting.GetValueOrDefault(false))
			{
				return true;
			}

			foreach (Step step in Steps)
			{
				if (step.IsExecuting.GetValueOrDefault(false))
				{
					return true;
				}
			}
			return false;
		}
	}
}