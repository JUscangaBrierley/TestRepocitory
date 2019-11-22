using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// An email.
	/// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_AL_Email")]
    public class EmailDocument_AL : LWObjectAuditLogBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long ObjectId { get; set; }
        [PetaPoco.Column]
        public long? ExternalId { get; set; }
        [PetaPoco.Column]
        public long? FolderId { get; set; }
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Name { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string Description { get; set; }
		[PetaPoco.Column(Length = 254)]
		public string FromEmail { get; set; }
		[PetaPoco.Column(Length = 254, IsNullable = true)]
		public string BounceEmail { get; set; }
		[PetaPoco.Column(IsNullable = false)]
        public DateTime CreateDate { get; set; }
        [PetaPoco.Column]
        public DateTime? UpdateDate { get; set; }
    }
}
