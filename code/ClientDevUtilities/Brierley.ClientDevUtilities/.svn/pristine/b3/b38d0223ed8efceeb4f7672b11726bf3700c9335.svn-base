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
	/// This class defines an advertising message.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_Category")]
    public class Category_AL : LWObjectAuditLogBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public Int64 ObjectId { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public Int64 ParentCategoryID { get; set; }

		[PetaPoco.Column]
		public bool? IsVisibleInLn { get; set; }

		[PetaPoco.Column(Length = 255, IsNullable = false)]
		public string Name { get; set; }

		[PetaPoco.Column(Length = 1000)]
		public string Description { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public CategoryType CategoryType { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
    }
}
