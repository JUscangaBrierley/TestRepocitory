using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_LangChanContent")]
    public class LangChanContent_AL : LWObjectAuditLogBase
    {
		[PetaPoco.Column(IsNullable = false)]
        public long ObjectId { get; set; }
		[PetaPoco.Column(Length = 25, IsNullable = false, PersistEnumAsString = true)]
		public ContentObjType LangChanType { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public long RefId { get; set; }
		[PetaPoco.Column(Length = 25, IsNullable = false)]
		public string LanguageCulture { get; set; }
        [PetaPoco.Column(Length = 25, IsNullable = false)]
		public string Channel { get; set; }
        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string Name { get; set; }
		[PetaPoco.Column]
		public string Content { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate  { get; set; }
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
    }
}