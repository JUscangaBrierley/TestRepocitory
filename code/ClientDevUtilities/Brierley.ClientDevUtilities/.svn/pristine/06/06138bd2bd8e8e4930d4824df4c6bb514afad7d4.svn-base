using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for Promotion.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_Promotion")]
    [AuditLog(true)]
    public class Promotion : ContentDefBase
	{
		/// <summary>
        /// Gets or sets the Code for the current Promotion
		/// </summary>
        [PetaPoco.Column(Length = 150, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public virtual string Code { get; set; }
		
		/// <summary>
        /// Gets or sets the Name for the current Promotion
		/// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the Internal Description for the current Promotion
        /// </summary>
        [PetaPoco.Column(Length = 500)]
        public virtual string PromotionDescription { get; set; }

        /// <summary>
        /// Gets or sets the Name for the current Promotion
        /// </summary>
        public virtual string PromotionName
        {
            get
            {
                return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "PromotionName");
            }
            set
            {
                SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "PromotionName", value);
            }
        }

        /// <summary>
        /// Gets or sets the Description for the current Promotion
        /// </summary>
        public virtual string Description
        {
            get
            {
                return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Description");
            }
            set
            {
                SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Description", value);
            }
        }

		/// <summary>
		/// Gets or sets the type of enrollment supported by the promotion.
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public PromotionEnrollmentSupportType EnrollmentSupportType { get; set; }

		/// <summary>
		/// Gets or sets the Folder Id for the current Promotion
		/// </summary>
        [PetaPoco.Column]
        [ColumnIndex]
		public virtual long? FolderId { get; set; }
						
		/// <summary>
        /// Gets or sets the StartDate for the current Promotion
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public virtual DateTime StartDate { get; set; }
		
		/// <summary>
        /// Gets or sets the EndDate for the current Promotion
		/// </summary>
        [PetaPoco.Column]
		public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the Content for the current MessageDef
        /// </summary>
        public virtual string Content
        {
            get
            {
                return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Content");
            }
            set
            {
                SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Content", value);
            }
        }

        /// <summary>
        /// Gets or sets the DisplayOrder for the current MessageDef
        /// </summary>
        [PetaPoco.Column]
		public virtual int? DisplayOrder { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public bool Targeted { get; set; }

		/// <summary>
        /// Initializes a new instance of the Promotion class
		/// </summary>
		public Promotion() : base(ContentObjType.Promotion)
		{
            Targeted = true;
		}

        public string GetDescription(string language, string channel)
        {
            return GetContent(language, channel, "Description");
        }

        public string GetContent(string language, string channel)
        {
            return GetContent(language, channel, "Content");
        }

        public string GetDisplayName(string language, string channel)
        {
            return GetContent(language, channel, "PromotionName");
        }

        public void SetDescription(string language, string channel, string description)
        {
            SetContent(language, channel, "Description", description);
        }

        public void SetDisplayName(string language, string channel, string description)
        {
            SetContent(language, channel, "PromotionName", description);
        }

        public void SetContent(string language, string channel, string description)
        {
            SetContent(language, channel, "Content", description);
        }

		public bool IsValid()
        {
            bool valid = false;
            if (EndDate != null)
            {
                valid = DateTimeUtil.IsDateInBetween(DateTime.Now, (DateTime)StartDate, (DateTime)EndDate, true);
            }
            else
            {
                valid = StartDate <= DateTime.Now;
            }                      
            return valid;
        }

        public Promotion Clone()
        {
            return Clone(new Promotion());
        }

        public Promotion Clone(Promotion dest)
        {
            dest.Code = Code;
            dest.Name = Name;
            dest.PromotionDescription = PromotionDescription;
            dest.StartDate = StartDate;
            dest.EndDate = EndDate;
            dest.DisplayOrder = DisplayOrder;
            dest.Targeted = Targeted;
            dest.FolderId = FolderId;
            dest.EnrollmentSupportType = EnrollmentSupportType;
            return (Promotion)base.Clone(dest);
        }

        public override LWObjectAuditLogBase GetArchiveObject()
        {
            Promotion_AL ar = new Promotion_AL()
            {
                ObjectId = this.Id,
                Code = this.Code,
                Name = this.Name,
                PromotionDescription = this.PromotionDescription,
                EnrollmentSupportType = this.EnrollmentSupportType,
                FolderId = this.FolderId,
                DisplayOrder = this.DisplayOrder,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Targeted = this.Targeted,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate
            };
            return ar;
        }
    }
}