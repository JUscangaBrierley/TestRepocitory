using System;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_MemberAWLoyaltyCard")]
    public class MemberAppleWalletLoyaltyCard : LWCoreObjectBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long ID { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public long MemberId { get; set; }

        [PetaPoco.Column(IsNullable = false, Length = 50)]
        public string SerialNumber { get; set; }

        [PetaPoco.Column(IsNullable = false, Length = 50)]
        public string AuthToken { get; set; }

        [PetaPoco.Column(Length = 100)]
        public string LastHash { get; set; }

        public MemberAppleWalletLoyaltyCard() { }
    }
}
