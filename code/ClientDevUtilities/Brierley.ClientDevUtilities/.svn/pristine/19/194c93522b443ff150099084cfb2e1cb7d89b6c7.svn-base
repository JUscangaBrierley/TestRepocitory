//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_AL_CSNote")]
    public class CSNote_AL : LWObjectAuditLogBase
    {
       
		#region Properties
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long MemberId { get; set; }
        [PetaPoco.Column(Length = 512, IsNullable = false)]
        public string Note { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long CreatedBy { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public Boolean Deleted { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public DateTime CreateDate { get; set; }
        [PetaPoco.Column]
        public DateTime? UpdateDate { get; set; }
		#endregion
    }
}
