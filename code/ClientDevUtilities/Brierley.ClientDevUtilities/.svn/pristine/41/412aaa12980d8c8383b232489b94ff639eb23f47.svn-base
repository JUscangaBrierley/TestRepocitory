using System;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_AWDeviceRegistration")]
    public class AppleWalletDeviceRegistration : LWCoreObjectBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long ID { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public long AppleWalletDeviceID { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public long MemberAppleWalletLoyaltyCardID { get; set; }

        public AppleWalletDeviceRegistration() { }
    }
}
