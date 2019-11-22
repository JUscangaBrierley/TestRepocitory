using Brierley.FrameWork.Data.ModelAttributes;
using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for MemberTier. 
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberTiers")]
    [AuditLog(false)]
	public class MemberTier : LWCoreObjectBase
	{
		/// <summary>
		/// Initializes a new instance of the MemberTier class
		/// </summary>
		public MemberTier()
		{
		}

		/// <summary>
		/// Gets or sets the Id for the current MemberTier
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the TierDefId for the current MemberTier
		/// </summary>
		[PetaPoco.Column("TierId", IsNullable = false)]
        [ForeignKey(typeof(TierDef), "Id")]
		public long TierDefId { get; set; }

		/// <summary>
		/// Gets or sets the MemberId for the current MemberTier
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Member), "IpCode")]
        [ColumnIndex]
		public long MemberId { get; set; }

		/// <summary>
		/// Gets or sets the FormDate for the current MemberTier
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public DateTime FromDate { get; set; }

		/// <summary>
		/// Gets or sets the ToDate for the current MemberTier
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public DateTime ToDate { get; set; }

		[PetaPoco.Column(Length = 150)]
		public string Description { get; set; }

		public TierDef TierDef { get; set; }

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			MemberTier_AL ar = new MemberTier_AL()
			{
				Description = this.Description,
				FromDate = this.FromDate,
				MemberId = this.MemberId,
				ObjectId = this.Id,
				TierDefId = this.TierDefId,
				ToDate = this.ToDate,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}