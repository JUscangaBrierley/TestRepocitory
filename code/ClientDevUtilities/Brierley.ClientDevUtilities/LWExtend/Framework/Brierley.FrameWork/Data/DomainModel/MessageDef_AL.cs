//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_MessageDef")]
    public class MessageDef_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
        public Int64 ObjectId { get; set; }
		[PetaPoco.Column(Length = 50, IsNullable = false)]
		public string Name { get; set; }
        [PetaPoco.Column]
        public long? PushNotificationId { get; set; }
		[PetaPoco.Column]
		public long? FolderId { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }
		[PetaPoco.Column]
		public DateTime? ExpiryDate { get; set; }
		[PetaPoco.Column]
		public int? DisplayOrder { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }

    }
}
