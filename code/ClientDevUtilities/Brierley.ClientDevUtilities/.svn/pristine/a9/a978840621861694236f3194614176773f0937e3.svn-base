using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// A document in the content management system.
    /// </summary>
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_Document")]
    public class Document_AL : LWObjectAuditLogBase
    {
		[PetaPoco.Column(IsNullable = false)]
        public long ObjectId { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public long TemplateID { get; set; }

		[PetaPoco.Column(Length = 50)]
		public string Name { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DocumentType DocumentType { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public int Version { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public bool IsLocked { get; set; }

		[PetaPoco.Column]
		public string Properties { get; set; }

		[PetaPoco.Column]
		public string HtmlContentAreas { get; set; }

		[PetaPoco.Column]
		public string TextContentAreas { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
    }
}