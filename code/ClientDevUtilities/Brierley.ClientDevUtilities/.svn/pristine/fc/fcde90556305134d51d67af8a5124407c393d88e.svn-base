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
    [PetaPoco.TableName("LW_AL_NotificationDef")]
    public class NotificationDef_AL : LWObjectAuditLogBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public Int64 ObjectId { get; set; }
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Name { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string Sound { get; set; }
        [PetaPoco.Column(Length = 2000)]
        public string Actions { get; set; }
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
