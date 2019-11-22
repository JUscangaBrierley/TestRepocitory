using Brierley.FrameWork.Data.ModelAttributes;
using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for MemberPromotion.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberPromotion")]
    [AuditLog(false)]
    public class MemberPromotion : LWCoreObjectBase
	{
		/// <summary>
        /// Gets or sets the ID for the current MemberPromotion
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }
		
		/// <summary>
        /// Gets or sets the Code for the current MemberPromotion
		/// </summary>
        [PetaPoco.Column(Length = 150, IsNullable = false)]
        [ForeignKey(typeof(Promotion), "Code")]
		public string Code { get; set; }
		
		/// <summary>
        /// Gets or sets the MemberId for the current MemberPromotion
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Member), "IpCode")]
        [ColumnIndex]
		public long MemberId { get; set; }

		/// <summary>
		/// Gets or sets the Id of the MTouch associated with this coupon.
		/// </summary>
        [PetaPoco.Column]
		public long? MTouchId { get; set; }

		/// <summary>
		/// Gets or sets the CertificateNmbr for the current MemberPromotion
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string CertificateNmbr { get; set; }

		/// <summary>
		/// gets or sets whether or not the member has enrolled into the promotion.
		/// </summary>
		/// <remarks>
		/// Applicable only when the promotion definition either supports or requires enrollment.
		/// </remarks>
		[PetaPoco.Column(IsNullable = false)]
		public bool Enrolled { get; set; }

		/// <summary>
		/// Initializes a new instance of the MemberPromotion class
		/// </summary>
		public MemberPromotion()
		{
		}			
	
        public override LWObjectAuditLogBase GetArchiveObject()
        {
            MemberPromotion_AL ar = new MemberPromotion_AL()
            {
                ObjectId = this.Id,
                Code = this.Code,
                MemberId = this.MemberId,
                MTouchId = this.MTouchId,
                CertificateNmbr = this.CertificateNmbr,                
				Enrolled = this.Enrolled, 
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate
            };
            return ar;
        }
	}
}