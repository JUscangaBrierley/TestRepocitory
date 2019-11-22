//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_CSAgent")]
    [AuditLog(true)]
	public class CSAgent : LWCoreObjectBase
	{
		private ServiceConfig _config = null;

        private string _emailAddress { get; set; }

		/// <summary>
		/// Initializes a new instance of the CSAgent class
		/// </summary>
		public CSAgent()
		{
			Status = AgentAccountStatus.Active;
		}

		/// <summary>
		/// Gets or sets the ID for the current CSAgent
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

		/// <summary>
		/// Gets or sets the RoleId for the current CSAgent
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long RoleId { get; set; }

		/// <summary>
		/// Gets or sets the GroupId for the current CSAgent
		/// </summary>
        [PetaPoco.Column]
        public long? GroupId { get; set; }

		/// <summary>
		/// Gets or sets the AgentNumber for the current CSAgent
		/// </summary>
        [PetaPoco.Column]
        public long? AgentNumber { get; set; }

		/// <summary>
		/// Gets or sets the FirstName for the current CSAgent
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string FirstName { get; set; }

		/// <summary>
		/// Gets or sets the LastName for the current CSAgent
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string LastName { get; set; }

		/// <summary>
		/// Gets or sets the EmailAddress for the current CSAgent
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string EmailAddress
		{
			get { return this._emailAddress; }
			set
			{
				this._emailAddress = !string.IsNullOrEmpty(value) ? value.ToUpper() : value;
			}
		}

		/// <summary>
		/// Gets or sets the PhoneNumber for the current CSAgent
		/// </summary>
        [PetaPoco.Column(Length = 20)]
        public string PhoneNumber { get; set; }

		/// <summary>
		/// Gets or sets the Extension for the current CSAgent
		/// </summary>
        [PetaPoco.Column(Length = 10)]
        public string Extension { get; set; }

		/// <summary>
		/// Gets or sets the Username for the current CSAgent
		/// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Username { get; set; }

		/// <summary>
		/// Gets or sets the FailedPasswordAttemptCount for the current CSAgent
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public int FailedPasswordAttemptCount { get; set; }

		/// <summary>
		/// Gets or sets the Password for the current CSAgent
		/// </summary>
        [PetaPoco.Column(Length = 255)]
        public string Password { get; set; }

		/// <summary>
		/// Gets or sets the Salt for the current CSAgent
		/// </summary>
        [PetaPoco.Column(Length = 50)]
        public string Salt { get; set; }

		/// <summary>
		/// Gets or sets the PasswordChangeRequired for the current CSAgent
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public Boolean PasswordChangeRequired { get; set; }

		/// <summary>
		/// Gets or sets the PasswordExpiredDate for the current CSAgent
		/// </summary>
        [PetaPoco.Column]
        public DateTime? PasswordExpireDate { get; set; }

		/// <summary>
		/// Gets or sets the Status for the current CSAgent
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public AgentAccountStatus Status { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public long CreatedBy { get; set; }

		/// <summary>
		/// Gets or sets the ResetCode for the current CSAgent
		/// </summary>
        [PetaPoco.Column(Length = 32)]
        public string ResetCode { get; set; }

		/// <summary>
		/// Gets or sets the ResetCodeDate for the current CSAgent
		/// </summary>
        [PetaPoco.Column]
        public DateTime? ResetCodeDate { get; set; }

		public bool IsInRole(string roleName)
		{
			bool result = false;
			if (_config == null)
			{
				_config = LWDataServiceUtil.GetServiceConfiguration();
			}
			using (var svc = new CSService(_config))
			{
				CSRole role = svc.GetRole(RoleId, true);
				if (role != null)
				{
					result = role.Name == roleName;
				}
			}
			return result;
		}

		public bool HasPermission(string functionName)
		{
			bool result = false;
			if (_config == null)
			{
				_config = LWDataServiceUtil.GetServiceConfiguration();
			}
			using (var svc = new CSService(_config))
			{
				CSRole role = svc.GetRole(RoleId, true);
				if (role != null)
				{
					result = role.HasFunction(functionName);
				}
				if (result)
				{
					ICSPermissionCallback cb = svc.GetPermissionCallback();
					if (cb != null)
					{
						result = cb.HasPermission(this, role, functionName);
					}
				}
			}
			return result;
		}

		public bool IsPasswordChangeRequired()
		{
			bool result = false;
			if (PasswordChangeRequired)
			{
				// someone has forced a password change
				result = true;
			}
			else if (PasswordExpireDate != null && DateTime.Today > PasswordExpireDate)
			{
				// password has expired
				result = true;
			}
			return result;
		}

		public bool IsAccountLocked()
		{
			return Status == AgentAccountStatus.Locked;
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			CSAgent_AL ar = new CSAgent_AL()
			{
				ObjectId = this.Id,
				RoleId = this.RoleId,
				GroupId = this.GroupId,
				AgentNumber = this.AgentNumber,
				FirstName = this.FirstName,
				LastName = this.LastName,
				EmailAddress = this.EmailAddress,
				PhoneNumber = this.PhoneNumber,
				Extension = this.Extension,
				Username = this.Username,
				Password = this.Password,
				Salt = this.Salt,
				FailedPasswordAttemptCount = this.FailedPasswordAttemptCount,
				PasswordChangeRequired = this.PasswordChangeRequired,
				PasswordExpireDate = this.PasswordExpireDate,
				Status = this.Status,
				CreatedBy = this.CreatedBy,
				ResetCode = this.ResetCode,
				ResetCodeDate = this.ResetCodeDate,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}
