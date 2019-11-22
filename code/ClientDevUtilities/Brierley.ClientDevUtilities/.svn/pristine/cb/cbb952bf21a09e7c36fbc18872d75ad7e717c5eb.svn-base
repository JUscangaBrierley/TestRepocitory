//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// This class defines a coupon.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_CouponDef")]
    public class CouponDef_AL : LWObjectAuditLogBase
    {
		[PetaPoco.Column]
        public long? ObjectId { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public long PassDefId { get; set; }
		[PetaPoco.Column(Length = 100, IsNullable = false)]
		public string Name { get; set; }
        [PetaPoco.Column]
        public long? PushNotificationId { get; set; }
		[PetaPoco.Column]
		public long? FolderId { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public long CategoryId { get; set; }
		[PetaPoco.Column(Length = 50)]
		public string CouponTypeCode { get; set; }
		[PetaPoco.Column(Length = 255)]
		public string LogoFileName { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }
		[PetaPoco.Column]
		public DateTime? ExpiryDate { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public long UsesAllowed { get; set; }
		[PetaPoco.Column]
		public int? UsesPerYear { get; set; }
		[PetaPoco.Column]
		public int? UsesPerMonth { get; set; }
		[PetaPoco.Column]
		public int? UsesPerWeek { get; set; }
		[PetaPoco.Column]
		public int? UsesPerDay { get; set; }
		[PetaPoco.Column]
		public int? DisplayOrder { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public bool IsGlobal { get; set; }
    }
}
