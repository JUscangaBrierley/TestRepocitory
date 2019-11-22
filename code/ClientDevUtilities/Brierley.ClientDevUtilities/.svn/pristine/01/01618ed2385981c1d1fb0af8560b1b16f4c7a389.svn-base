using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("ID", sequenceName = "SEQ_MTOUCH")]
	[PetaPoco.TableName("LW_MTouch")]
    public class MTouch : LWCoreObjectBase
    {
		/// <summary>
		/// Unique key for the mtouch record
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// MTouch code (a GUID)
		/// </summary>
        [PetaPoco.Column(Length = 32, IsNullable = false)]
		public string MTouchValue { get; set; }

		/// <summary>
		/// Type of MTouch
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public MTouchType MTouchType { get; set; }

		/// <summary>
		/// Is this MTouch available?  It may have been pre-allocated
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool Available { get; set; }

		/// <summary>
		/// The human entity this MTouch is associated with.  E.g., for Email
		/// MTouch types, it is the email address of the user.
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string EntityId { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of uses allowed for the MTouch
		/// </summary>
        [PetaPoco.Column]
		public int? UsesAllowed { get; set; }

		/// <summary>
		/// This is a secondary ID for the MTouch used to associate it with
		/// some other item in the database.  For example, for a promotion
		/// type MTouch, it represents the ID of the associated promotion.
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string SecondaryId { get; set; }

		/// <summary>
		/// Initializes a new instance of the MTouch class
		/// </summary>
		public MTouch()
		{
			ID = -1;
			MTouchType = MTouchType.Coupon;
			Available = true;
		}
    }
}
