using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// A template in the content management system.
    /// </summary>
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_Template")]
	public class Template_AL : LWObjectAuditLogBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long ObjectId { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public string Name { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string Description { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public TemplateType TemplateType { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public int Version { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public bool IsLocked { get; set; }

		[PetaPoco.Column]
		public string Fields { get; set; }

		[PetaPoco.Column]
		public string HtmlContent { get; set; }

		[PetaPoco.Column]
		public string TextContent { get; set; }

		[PetaPoco.Column]
		public long? FolderId { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
    }
}