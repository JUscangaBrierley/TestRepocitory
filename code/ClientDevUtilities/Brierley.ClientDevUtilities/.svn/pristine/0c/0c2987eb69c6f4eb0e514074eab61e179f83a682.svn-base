using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// An Sms message.
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_Sms")]
    [AuditLog(true)]
	public class SmsDocument : LWCoreObjectBase
    {
        /// <summary>
        /// Gets or sets the id for the current Sms
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public Int64 Id { get; set; }

        /// <summary>
        /// Gets or sets the external (Teradata DMC) Id for the Sm.
        /// </summary>
        /// <remarks>
        /// This property uniquely identifies the Sms in a 3rd party's system. It is used to retrieve 
        /// personalizations (fields) for the Sms as well as invoking a send of the Sms through their API.
        /// </remarks>
        [PetaPoco.Column]
        /// <summary>
        /// Gets or sets the external id for the current Sms
        /// </summary>
		public long? ExternalId { get; set; }

		/// <summary>
        /// Gets or sets the folder id for the current Sms
		/// </summary>
        [PetaPoco.Column]
        public long? FolderId { get; set; }

		/// <summary>
        /// Gets or sets the name for the current Sms
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for the current Sms
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string Description { get; set; }

		/// <summary>
		/// Initializes a new instance of the Sms class
		/// </summary>
		public SmsDocument()
		{
			Id = -1;
		}

        public SmsDocument(SmsDocument other)
		{
			Id = -1;
			ExternalId = other.ExternalId;
			Name = other.Name;
			CreateDate = other.CreateDate;
		}

        public virtual SmsDocument Clone()
		{
            return Clone(new SmsDocument());
		}

        public virtual SmsDocument Clone(SmsDocument other)
		{
			other.ExternalId = ExternalId;
			other.Name = Name;
            return (SmsDocument)base.Clone(other);
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
            SmsDocument_AL ar = new SmsDocument_AL()
			{
				ObjectId = this.Id,
				ExternalId = this.ExternalId,
				FolderId = this.FolderId,
				Name = this.Name,
                Description = this.Description,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}
