using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{	
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_Tiers")]
	public class TierDef_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }

		[PetaPoco.Column("TierName", Length = 50, IsNullable = false)]
		public string Name { get; set; }

        [PetaPoco.Column(Length = 100)]
		public string DisplayText { get; set; }

        [PetaPoco.Column(Length = 500)]
		public string Description { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public decimal EntryPoints { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public decimal ExitPoints { get; set; }

        [PetaPoco.Column(Length = 2000)]
		public string PointTypeNames { get; set; }

        [PetaPoco.Column(Length = 2000)]
		public string PointEventNames { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public bool AddToEnrollmentDate { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string ExpirationDateExpression { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string ActivityPeriodStartExpression { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string ActivityPeriodEndExpression { get; set; }

		[PetaPoco.Column]
		public long? DefaultRewardId { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
	}
}