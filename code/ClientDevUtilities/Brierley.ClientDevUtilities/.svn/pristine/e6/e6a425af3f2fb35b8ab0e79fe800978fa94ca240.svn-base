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
    [PetaPoco.TableName("LW_AL_MobileDevice")]
    public class MobileDevice_AL : LWObjectAuditLogBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long ObjectId { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long MemberId { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public DeviceType DeviceType { get; set; }
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string DeviceId { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string PushToken { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string DeviceOS { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string DeviceOSVersion { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public bool AcceptsPush { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public DateTime CreateDate { get; set; }
        [PetaPoco.Column]
        public DateTime? UpdateDate { get; set; }
    }
}
