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
    [PetaPoco.TableName("LW_Email")]
    [AuditLog("Email", true)]
	public class EmailDocument : LWCoreObjectBase
    {
        /// <summary>
        /// Gets or sets the id for the current Email
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public Int64 Id { get; set; }

        /// <summary>
        /// Gets or sets the external (Teradata DMC) Id for the email.
        /// </summary>
        /// <remarks>
        /// This property uniquely identifies the email in a 3rd party's system. It is used to retrieve 
        /// personalizations (fields) for the email as well as invoking a send of the email through their API.
        /// </remarks>
        [PetaPoco.Column]
        /// <summary>
        /// Gets or sets the external id for the current Email
        /// </summary>
		public long? ExternalId { get; set; }

		/// <summary>
		/// Gets or sets the folder id for the current Email
		/// </summary>
        [PetaPoco.Column]
        [ColumnIndex]
        public long? FolderId { get; set; }

		/// <summary>
		/// Gets or sets the name for the current Email
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for the current Email
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string Description { get; set; }

		/// <summary>
		/// Gets or sets the DocumentID for the current Email
		/// </summary>
		[PetaPoco.Column]
		public long? DocumentId { get; set; }

		/// <summary>
		/// Gets or sets the ReplyTo for the current Email
		/// </summary>
		[PetaPoco.Column(Length = 254)]
		public string FromEmail { get; set; }

		/// <summary>
		/// Gets or sets the bounce address the current Email
		/// </summary>
		/// <remarks>
		/// For Amazon SES, this is used for the ReturnPath, which is used to send 
		/// bounce/complaint feedback via email to the address specified.
		/// </remarks>
		[PetaPoco.Column(Length = 254, IsNullable = true)]
		public string BounceEmail { get; set; }

		/// <summary>
		/// Gets or sets the Subject for the current Email
		/// </summary>
		[PetaPoco.Column(Length = 255)]
		public string Subject { get; set; }


		/// <summary>
		/// Initializes a new instance of the Email class
		/// </summary>
		public EmailDocument()
		{
			Id = -1;
		}

		public EmailDocument(EmailDocument other)
		{
			Id = -1;
			ExternalId = other.ExternalId;
			Name = other.Name;
			CreateDate = other.CreateDate;
		}
		
		public virtual EmailDocument Clone()
		{
			return Clone(new EmailDocument());
		}

		public virtual EmailDocument Clone(EmailDocument other)
		{
			other.ExternalId = ExternalId;
			other.Name = Name;
            other.Description = Description;
            other.FromEmail = FromEmail;
            other.BounceEmail = BounceEmail;
            other.Subject = Subject;
			return (EmailDocument)base.Clone(other);
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			EmailDocument_AL ar = new EmailDocument_AL()
			{
				ObjectId = Id,
				ExternalId = ExternalId,
				FolderId = FolderId,
				Name = Name,
                Description = Description,
				FromEmail = FromEmail, 
				BounceEmail = BounceEmail, 
				CreateDate = CreateDate,
				UpdateDate = UpdateDate
			};
			return ar;
		}
	}
}
