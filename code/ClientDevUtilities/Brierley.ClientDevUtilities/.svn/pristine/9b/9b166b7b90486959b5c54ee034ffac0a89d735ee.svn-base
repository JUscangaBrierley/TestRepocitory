using System;
using System.Collections.Generic;
using System.Text;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_AL_CSAgent")]
    public class CSAgent_AL : LWObjectAuditLogBase
    {
        private string _emailAddress { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public long ObjectId { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long RoleId { get; set; }
        [PetaPoco.Column]
        public long? GroupId { get; set; }
        [PetaPoco.Column]
        public long? AgentNumber { get; set; }
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string FirstName { get; set; }
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string LastName { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string EmailAddress
        {
			get { return this._emailAddress; }
			set { this._emailAddress = !string.IsNullOrEmpty(value) ? value.ToUpper() : value; }
        }
        [PetaPoco.Column(Length = 20)]
        public string PhoneNumber { get; set; }
        [PetaPoco.Column(Length = 10)]
        public string Extension { get; set; }
        [PetaPoco.Column(Length = 100, IsNullable = false)]
        public string Username { get; set; }
        [PetaPoco.Column(Length = 255)]
        public string Password { get; set; }
        [PetaPoco.Column(Length = 50)]
        public string Salt { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public int FailedPasswordAttemptCount { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public Boolean PasswordChangeRequired { get; set; }
        [PetaPoco.Column]
        public DateTime? PasswordExpireDate { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public AgentAccountStatus Status { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public long CreatedBy { get; set; }
        [PetaPoco.Column(Length = 32)]
        public string ResetCode { get; set; }
        [PetaPoco.Column]
        public DateTime? ResetCodeDate { get; set; }
        [PetaPoco.Column(IsNullable = false)]
        public DateTime CreateDate { get; set; }
        [PetaPoco.Column]
        public DateTime? UpdateDate { get; set; }

		/// <summary>
		/// Initializes a new instance of the PointEvent class
		/// </summary>
		public CSAgent_AL()
		{
		}

    }
}