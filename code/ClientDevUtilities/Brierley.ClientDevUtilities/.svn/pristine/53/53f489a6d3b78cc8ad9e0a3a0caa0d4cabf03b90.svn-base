
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
    [PetaPoco.TableName("LW_AL_NotificationCategory")]
    public class NotificationCategory_AL : LWObjectAuditLogBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long ObjectId { get; set; }
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Name { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long SupportedVersion { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public bool IsActive { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public DateTime CreateDate { get; set; }
        [PetaPoco.Column]
        public DateTime? UpdateDate { get; set; }
    }
}
