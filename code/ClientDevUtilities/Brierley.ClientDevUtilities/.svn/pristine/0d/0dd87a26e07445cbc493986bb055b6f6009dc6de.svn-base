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
    [PetaPoco.TableName("LW_NotificationDef")]
    [AuditLog(true)]
	public class NotificationDef : ContentDefBase
	{
        /// <summary>
		/// Gets or sets the Name for the current NotificationDef
		/// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Title for the current NotificationDef
        /// </summary>
        public string Title
        {
            get
            {
                return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Title");
            }
            set
            {
                SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Title", value);
            }
        }

        /// <summary>
        /// Gets or sets the Body for the current NotificationDef
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
        /// Gets or sets the sound for the current NotificationDef
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string Sound { get; set; }

        /// <summary>
        /// Gets or sets the Addistional Properties for the current NotificationDef
        /// </summary>
        [PetaPoco.Column(Length = 2000)]
        public string Actions { get; set; }

        /// <summary>
        /// Gets or sets the folder id for the current NotificationDef
        /// </summary>
        [PetaPoco.Column]
        public long? FolderId { get; set; }

        /// <summary>
        /// Gets or sets the StartDate for the current NotificationDef
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the ExpiryDate for the current NotificationDef
        /// </summary>
        [PetaPoco.Column]
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the DisplayOrder for the current NotificationDef
        /// </summary>
        [PetaPoco.Column]
        public int? DisplayOrder { get; set; }

        /// <summary>
		/// Initializes a new instance of the NotificationDef class
		/// </summary>
		public NotificationDef()
			: base(ContentObjType.Notification)
		{
            StartDate = DateTime.Now;
        }

        public string GetTitle(string language, string channel)
        {
            return GetContent(language, channel, "Title");
        }

        public string GetBody(string language, string channel)
        {
            return GetContent(language, channel, "Body");
        }

        public void SetTitle(string language, string channel, string title)
        {
            SetContent(language, channel, "Title", title);
        }

        public void SetBody(string language, string channel, string body)
        {
            SetContent(language, channel, "Body", body);
        }

        public NotificationDef Clone()
        {
            return Clone(new NotificationDef());
        }

        public NotificationDef Clone(NotificationDef dest)
        {
            dest.Name = Name;
            dest.Sound = Sound;
            dest.Actions = Actions;
            dest.FolderId = FolderId;
            dest.StartDate = StartDate;
            dest.ExpiryDate = ExpiryDate;
            dest.DisplayOrder = DisplayOrder;
            return (NotificationDef)base.Clone(dest);
        }

        public override LWObjectAuditLogBase GetArchiveObject()
        {
            NotificationDef_AL ar = new NotificationDef_AL()
            {
                ObjectId = this.Id,
                Name = this.Name,
                Sound = this.Sound,
                Actions = this.Actions,
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
