using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for MemberCoupon.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_MemberCoupon")]
    public class MemberCoupon_AL : LWObjectAuditLogBase
	{
		/// <summary>
		/// Gets or sets the ID for the current MemberCoupon
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }

		/// <summary>
		/// Gets or sets the CouponDefId for the current MemberCoupon
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public long CouponDefId { get; set; }

        /// <summary>
        /// Gets or sets the CertificateNmbr for the current MemberCoupon
        /// </summary>
        [PetaPoco.Column(Length = 50)]
		public string CertificateNmbr { get; set; }

		/// <summary>
		/// Gets or sets the MemberId for the current MemberCoupon
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public long MemberId { get; set; }

		/// <summary>
		/// Gets or sets the TimesUsed for the current MemberCoupon
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public long TimesUsed { get; set; }

		/// <summary>
		/// Gets or sets the DateIssued for the current MemberCoupon
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public DateTime DateIssued { get; set; }

		/// <summary>
		/// Gets or sets the DateRedeemed for the current MemberCoupon
		/// </summary>
		[PetaPoco.Column]
		public DateTime? DateRedeemed { get; set; }

		/// <summary>
		/// Gets or sets the ExpiryDate for the current MemberCoupon
		/// </summary>
		[PetaPoco.Column]
		public DateTime? ExpiryDate { get; set; }

		/// <summary>
		/// Gets or sets the display order for the current MemberCoupon
		/// </summary>
		[PetaPoco.Column]
		public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the CampaignId of the campaign that loaded this for the current MemberCoupon
        /// </summary>
		[PetaPoco.Column]
		public long? CampaignId { get; set; }

        /// <summary>
        /// Gets or sets the Status of the campaign that loaded this for the current MemberCoupon
        /// </summary>
        [PetaPoco.Column(Length = 25, PersistEnumAsString = true)]
		public CouponStatus? Status { get; set; }

		/// <summary>
		/// Gets or sets the Id of the MTouch associated with this coupon.
		/// </summary>
		[PetaPoco.Column]
		public long? MTouchId { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
	}
}