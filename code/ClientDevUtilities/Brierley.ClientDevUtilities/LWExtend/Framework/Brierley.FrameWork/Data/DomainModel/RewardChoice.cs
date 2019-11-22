using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_RewardChoice")]
	public class RewardChoice : LWCoreObjectBase
	{
		[PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		[ColumnIndex]
		public long MemberId { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public long RewardId { get; set; } //not null. once a member chooses a reward, it cannot be removed

		[PetaPoco.Column(Length = 100)]
		public string ChangedBy { get; set; }

		public RewardChoice()
		{
		}

		public RewardChoice(long memberId, long rewardId, string changedBy = null)
		{
			MemberId = memberId;
			RewardId = rewardId;
			ChangedBy = changedBy;
		}
	}
}
