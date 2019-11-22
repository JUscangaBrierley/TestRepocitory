using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Rules;

namespace Brierley.FrameWork.Data.DomainModel
{	
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_LoyaltyMember")]
    public class Member_AL : LWObjectAuditLogBase
    {
        
        #region Properties

        [PetaPoco.Column(IsNullable = false)]
        public Int64 ObjectId { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public DateTime MemberCreateDate { get; set; }

        [PetaPoco.Column]
        public DateTime? MemberCloseDate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public MemberStatusEnum MemberStatus { get; set; }

        [PetaPoco.Column]
        public DateTime? StatusChangeDate { get; set; }

        [PetaPoco.Column]
        public long? MergedToMember { get; set; }

        [PetaPoco.Column]
        public DateTime? BirthDate { get; set; }

        [PetaPoco.Column(Length = 50)]
        public String FirstName { get; set; }

        [PetaPoco.Column(Length = 50)]
        public String LastName { get; set; }

        [PetaPoco.Column(Length = 50)]
        public String MiddleName { get; set; }

        [PetaPoco.Column(Length = 10)]
        public String NamePrefix { get; set; }

        [PetaPoco.Column(Length = 10)]
        public String NameSuffix { get; set; }

        [PetaPoco.Column(Length = 255)]
        public string AlternateId { get; set; }

        [PetaPoco.Column(Length = 254)]
        public String Username { get; set; }

        [PetaPoco.Column(Length = 50)]
        public String Password { get; set; }

        [PetaPoco.Column(Length = 50)]
        public String Salt { get; set; }

        [PetaPoco.Column(Length = 254)]
        public String PrimaryEmailAddress { get; set; }

        [PetaPoco.Column(Length = 25)]
        public String PrimaryPhoneNumber { get; set; }

        [PetaPoco.Column(Length = 15)]
        public String PrimaryPostalCode { get; set; }

        [PetaPoco.Column]
        public DateTime? LastActivityDate { get; set; }

        [PetaPoco.Column]
        public bool? IsEmployee { get; set; }

        [PetaPoco.Column(Length = 25)]
        public String ChangedBy { get; set; }

        [PetaPoco.Column]
        public MemberStatusEnum? NewStatus { get; set; }

        [PetaPoco.Column]
        public DateTime? NewStatusEffectiveDate { get; set; }

        [PetaPoco.Column(Length = 255)]
        public String StatusChangeReason { get; set; }

        [PetaPoco.Column(Length = 6)]
        public String ResetCode { get; set; }

        [PetaPoco.Column]
        public DateTime? ResetCodeDate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public int FailedPasswordAttemptCount { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public Boolean PasswordChangeRequired { get; set; }

        [PetaPoco.Column]
        public DateTime? PasswordExpireDate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public DateTime CreateDate { get; set; }

        [PetaPoco.Column]
        public DateTime? UpdateDate { get; set; }
		#endregion

      
    }
}
