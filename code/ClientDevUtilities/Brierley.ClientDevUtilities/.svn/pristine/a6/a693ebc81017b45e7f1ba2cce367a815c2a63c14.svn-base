//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// This class defines a Message.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MessageDef")]
    [AuditLog(true)]
	public class MessageDef : ContentDefBase
    {
		/// <summary>
		/// Gets or sets the Name for the current MessageDef
		/// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Subject for the current MessageDef
		/// </summary>
		public string Subject
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Subject");
			}
			set
			{
                SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Subject", value);
			}
		}

		/// <summary>
		/// Gets or sets the folder id for the current MessageDef
		/// </summary>
        [PetaPoco.Column]
        [ColumnIndex]
		public long? FolderId { get; set; }

		/// <summary>
		/// Gets or sets the StartDate for the current MessageDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Gets or sets the ExpiryDate for the current MessageDef
		/// </summary>
        [PetaPoco.Column]
		public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the Summary for the current MessageDef
        /// </summary>
        public string Summary
        {
            get
            {
                return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Summary");
            }
            set
            {
                SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Summary", value);
            }
        }

        /// <summary>
        /// Gets or sets the Body for the current MessageDef
        /// </summary>
        public string Body
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Body");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Body", value);
			}
		}

		/// <summary>
		/// Gets or sets the DisplayOrder for the current MessageDef
		/// </summary>
        [PetaPoco.Column]
		public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the PushNotificationId associated with the current MessageDef
        /// </summary>
        [PetaPoco.Column]
        public long? PushNotificationId { get; set; }

        /// <summary>
		/// Initializes a new instance of the MessageDef class
		/// </summary>
		public MessageDef()
			: base(ContentObjType.Message)
		{
			StartDate = DateTime.Now;
		}

        public string GetSubject(string language, string channel)
		{
			return GetContent(language, channel, "Subject");
		}

        public string GetSummary(string language, string channel)
        {
            return GetContent(language, channel, "Summary");
        }

        public string GetBody(string language, string channel)
		{
			return GetContent(language, channel, "Body");
		}

		public void SetSubject(string language, string channel, string description)
		{
			SetContent(language, channel, "Subject", description);
		}

        public void SetSummary(string language, string channel, string description)
        {
            SetContent(language, channel, "Summary", description);
        }

        public void SetBody(string language, string channel, string content)
		{
			SetContent(language, channel, "Body", content);
		}

		public MessageDef Clone()
		{
			return Clone(new MessageDef());
		}

		public MessageDef Clone(MessageDef dest)
		{
			dest.Name = Name;
            dest.PushNotificationId = PushNotificationId;
			dest.FolderId = FolderId;
			dest.StartDate = StartDate;
			dest.ExpiryDate = ExpiryDate;
			dest.DisplayOrder = DisplayOrder;
			return (MessageDef)base.Clone(dest);
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			MessageDef_AL ar = new MessageDef_AL()
			{
				ObjectId = this.Id,
				Name = this.Name,
                PushNotificationId = this.PushNotificationId,
				FolderId = this.FolderId, 
				DisplayOrder = this.DisplayOrder,
				StartDate = this.StartDate,
				ExpiryDate = this.ExpiryDate,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}
