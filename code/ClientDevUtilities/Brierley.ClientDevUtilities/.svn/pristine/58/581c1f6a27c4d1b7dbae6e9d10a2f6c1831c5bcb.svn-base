using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// This class defines an advertising offer in member's account.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberBonus")]
	public class MemberBonus : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the Id for the current MemberOffer
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the AdvertismentDefId for the current MemberOffer
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(BonusDef), "Id")]
		public long BonusDefId { get; set; }

		/// <summary>
		/// Gets or sets the MemberId for the current MemberOffer
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Member), "IpCode")]
        [ColumnIndex]
		public long MemberId { get; set; }

		/// <summary>
		/// Gets or sets the MTouch Id for the current MemberOffer
		/// </summary>
        [PetaPoco.Column]
		public long? MTouchId { get; set; }

		/// <summary>
		/// Gets or sets the TimesClicked for the current MemberOffer
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long TimesClicked { get; set; }

		/// <summary>
		/// Gets or sets the status for the current MemberBonus
		/// </summary>
        [PetaPoco.Column(Length = 20, IsNullable = false, PersistEnumAsString = true)]
		public MemberBonusStatus Status { get; set; }

		/// <summary>
		/// Gets or sets ReferralCompleted for the current MemberOffer
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool ReferralCompleted { get; set; }

        [PetaPoco.Column]
		public int? DisplayOrder { get; set; }
        
        [PetaPoco.Column(IsNullable = false)]
        public DateTime StartDate { get; set; }

        [PetaPoco.Column]
		public DateTime? ExpiryDate { get; set; }

		/// <summary>
		/// Initializes a new instance of the MemberOffer class
		/// </summary>
		public MemberBonus()
		{
		}
	}
}