using Brierley.FrameWork.Data.ModelAttributes;
using System;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("InputStepId, OutputStepId", autoIncrement = false)]
	[PetaPoco.TableName("LW_CLStepIOXref")]
	public class StepIO
	{
		[PetaPoco.Column]
        [ForeignKey(typeof(Step), "Id")]
		public long InputStepId { get; set; }

		[PetaPoco.Column]
        [ForeignKey(typeof(Step), "Id")]
		public long OutputStepId { get; set; }

		[PetaPoco.Column("MergeTypeId")]
		public MergeType? MergeType { get; set; }

		[PetaPoco.Column]
		public int? MergeOrder { get; set; }

		public StepIO()
		{
		}

		public StepIO(long inputStepID, long outputStepID)
		{
			InputStepId = inputStepID;
			OutputStepId = outputStepID;
		}

		public override bool Equals(object obj)
		{
			StepIO otherInstance = obj as StepIO;
			if (otherInstance != null)
			{
				return otherInstance.InputStepId == InputStepId && otherInstance.OutputStepId == OutputStepId;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
