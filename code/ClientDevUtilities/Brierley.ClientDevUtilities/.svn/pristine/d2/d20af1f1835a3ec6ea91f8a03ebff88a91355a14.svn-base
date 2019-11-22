using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_VirtualCard")]
	public class VirtualCard_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }

        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string LoyaltyIdNumber { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public long IpCode { get; set; }
		
		[PetaPoco.Column]
		public long? LinkKey { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime DateIssued { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime DateRegistered { get; set; }
		
		[PetaPoco.Column]
		public DateTime? ExpirationDate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public VirtualCardStatusType Status { get; set; }
		
		[PetaPoco.Column]
		public VirtualCardStatusType? NewStatus { get; set; }
		
		[PetaPoco.Column]
		public DateTime? NewStatusEffectiveDate { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string StatusChangeReason { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public bool IsPrimary { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public long CardType { get; set; }

        [PetaPoco.Column(Length = 25)]
		public string ChangedBy { get; set; }
		
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
		
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
	}
}