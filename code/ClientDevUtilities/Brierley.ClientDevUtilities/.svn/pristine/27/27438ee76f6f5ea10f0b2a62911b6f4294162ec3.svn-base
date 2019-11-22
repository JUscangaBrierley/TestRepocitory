using System;

namespace Brierley.FrameWork.Data.DomainModel
{	
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_MemberPromotion")]
    public class MemberPromotion_AL : LWObjectAuditLogBase
	{
		[PetaPoco.Column]
		public long? ObjectId { get; set; }

        [PetaPoco.Column(Length = 150, IsNullable = false)]
		public string Code { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public long MemberId { get; set; }

		[PetaPoco.Column]
		public long? MTouchId { get; set; }

        [PetaPoco.Column(Length = 50)]
		public string CertificateNmbr { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public bool Enrolled { get; set; }	
	}
}