//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "SEQ_CSNOTE")]
    [PetaPoco.TableName("LW_CSNote")]
    [AuditLog(false)]
	public class CSNote : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the ID for the current CSNote
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

		/// <summary>
		/// Gets or sets the MemberId for the current CSNote
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Member), "IpCode")]
        public long MemberId { get; set; }

		/// <summary>
		/// Gets or sets the Note for the current CSNote
		/// </summary>
        [PetaPoco.Column(Length = 512, IsNullable = false)]
        public string Note { get; set; }

		/// <summary>
		/// Gets or sets the CreatedBy for the current CSNote
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long CreatedBy { get; set; }

		/// <summary>
		/// Gets or sets the Deleted for the current CSNote
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public Boolean Deleted { get; set; }

		/// <summary>
		/// Initializes a new instance of the CSNote class
		/// </summary>
		public CSNote()
		{
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			CSNote_AL ar = new CSNote_AL()
			{
				ObjectId = this.Id,
				MemberId = this.MemberId,
				Note = this.Note,
				CreatedBy = this.CreatedBy,
				Deleted = this.Deleted,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}
