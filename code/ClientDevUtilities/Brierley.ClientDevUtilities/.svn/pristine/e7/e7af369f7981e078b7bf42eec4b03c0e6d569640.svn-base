using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// An email.
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_NotificationCategory")]
    [AuditLog(true)]
	public class NotificationCategory : LWCoreObjectBase
    {
        /// <summary>
		/// Gets or sets the Id for the current Push Category
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the name for the current Push Category
        /// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the supported version for the current Push Category
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long SupportedVersion { get; set; }
        /// <summary>
        /// Gets or sets the is active flag for the current Push Category
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public bool IsActive { get; set; }      

        /// <summary>
        /// Initializes a new instance of the Sms class
        /// </summary>
        public NotificationCategory() : base()
		{
			Id = -1;
		}

        public NotificationCategory(NotificationCategory other)
            : base()
		{
			Id = -1;
            Name = other.Name;
            SupportedVersion = other.SupportedVersion;
            IsActive = other.IsActive;
            CreateDate = other.CreateDate;
		}

        public virtual NotificationCategory Clone()
        {
            return Clone(new NotificationCategory());
        }

        public virtual NotificationCategory Clone(NotificationCategory other)
        {
            other.Name = Name;
            other.SupportedVersion = SupportedVersion;
            other.IsActive = IsActive;
            return (NotificationCategory)base.Clone(other);
        }

        public override LWObjectAuditLogBase GetArchiveObject()
        {
            NotificationCategory_AL ar = new NotificationCategory_AL()
            {
                ObjectId = this.Id,
                Name = this.Name,
                SupportedVersion = this.SupportedVersion,
                IsActive = this.IsActive,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate
            };
            return ar;
        }
    }
}
