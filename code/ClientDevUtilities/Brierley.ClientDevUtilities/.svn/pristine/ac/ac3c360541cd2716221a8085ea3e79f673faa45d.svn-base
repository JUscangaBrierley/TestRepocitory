using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for RewardDef. 
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_RewardsDef")]
    [AuditLog(true)]
    public class RewardDef : ContentDefBase
	{
		/// <summary>
		/// Gets or sets the CertificateTypeCode for the current RewardDef
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string CertificateTypeCode { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current RewardDef
		/// </summary>
        [PetaPoco.Column(Length = 300, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the HowManyPointsToEarn for the current RewardDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public decimal HowManyPointsToEarn { get; set; }

		/// <summary>
		/// Gets or sets the PointType for the current RewardDef
		/// </summary>
        [PetaPoco.Column(Length = 2000)]
		public string PointType { get; set; }

		/// <summary>
		/// Gets or sets the PointEvent for the current RewardDef
		/// </summary>
        [PetaPoco.Column(Length = 2000)]
		public string PointEvent { get; set; }

		/// <summary>
		/// Gets or sets the ProductId for the current RewardDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Product), "Id")]
        [ColumnIndex]
        public long ProductId { get; set; }

		/// <summary>
		/// Gets or sets the TierId for the current RewardDef
		/// </summary>
        [PetaPoco.Column]
        [ForeignKey(typeof(TierDef), "Id")]
		public long? TierId { get; set; }

		/// <summary>
		/// Threshold for the certificate numbers.
		/// </summary>
        [PetaPoco.Column("Threshhold", IsNullable = false)]
        public long Threshold { get; set; }
		
		/// <summary>
		/// An thumbnail file associated with the reward.  This is the relative path in from the root of the image store.
		/// </summary>
        [PetaPoco.Column(Length = 250)]
		public string SmallImageFile { get; set; }

		/// <summary>
		/// An image file associated with the reward.  This is the relative path in from the root of the image store.
		/// </summary>
        [PetaPoco.Column(Length = 250)]
		public string LargeImageFile { get; set; }

		/// <summary>
		/// Gets or sets the CatalogStartDate for the current RewardDef
		/// </summary>
        [PetaPoco.Column]
		public DateTime? CatalogStartDate { get; set; }

		/// <summary>
		/// Gets or sets the CatalogEndDate for the current RewardDef
		/// </summary>
        [PetaPoco.Column]
		public DateTime? CatalogEndDate { get; set; }

		/// <summary>
		/// Active for the current RewardDef.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool Active { get; set; }

		/// <summary>
		/// Gets or sets the FulfillmentProviderId for the current RewardDef
		/// </summary>
        [PetaPoco.Column(IsNullable = true)]
		public long? FulfillmentProviderId { get; set; }

        /// <summary>
        /// Gets or sets the MediumImageFile for the current RewardDef
        /// </summary>
        [PetaPoco.Column(Length = 256)]
		public string MediumImageFile { get; set; }


        /// <summary>
        /// Gets or sets the DisplayName for the current RewardDef
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public String DisplayName
        {
            get
            {
                return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "DisplayName");
            }
            set
            {
                SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "DisplayName", value);
            }
        }

        /// <summary>
        /// Gets or sets the ShortDescription for the current RewardDef
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
		public String ShortDescription
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "ShortDescription");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "ShortDescription", value);
			}
		}

		/// <summary>
		/// Gets or sets the LongDescription for the current RewardDef
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public String LongDescription
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "LongDescription");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "LongDescription", value);
			}
		}

		/// <summary>
		/// Gets or sets the LegalText for the current RewardDef
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public String LegalText
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "LegalText");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "LegalText", value);
			}
		}

		/// <summary>
		/// Gets or sets the Product for the current RewardDef
		/// </summary>
		public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the RedeemTimeLimit for the current RewardDef
        /// </summary>
        [PetaPoco.Column]
        public long? RedeemTimeLimit { get; set; }

        /// <summary>
        /// Gets or sets the PushNotificationId associated with the current RewardDef
        /// </summary>
        [PetaPoco.Column]
        public long? PushNotificationId { get; set; }

        /// <summary>
        /// Gets or sets the RewardType for the current RewardDef
        /// </summary>
        [PetaPoco.Column(PersistEnumAsString = false, IsNullable = false)]
        public RewardType RewardType { get; set; }

        /// <summary>
        /// Gets or set the Loyalty Currency as payment conversion rate for the current RewardDef
        /// </summary>
        [PetaPoco.Column]
        public decimal? ConversionRate { get; set; }

		/// <summary>
		/// Initializes a new instance of the RewardDef class
		/// </summary>
		public RewardDef() : base(ContentObjType.Reward)
		{
			CertificateTypeCode = string.Empty;
			Name = string.Empty;
			PointType = string.Empty;
			PointEvent = string.Empty;
			SmallImageFile = string.Empty;
			LargeImageFile = string.Empty;
		    MediumImageFile = string.Empty;            
		}

		public string[] GetPointTypes()
		{
			return !string.IsNullOrEmpty(PointType) ? PointType.Split(';') : null;
		}

		public string[] GetPointEvents()
		{
			return !string.IsNullOrEmpty(PointEvent) ? PointEvent.Split(';') : null;
		}

        public RewardDef Clone()
        {
            return Clone(new RewardDef());
        }

        public RewardDef Clone(RewardDef dest)
        {
            dest.CertificateTypeCode = CertificateTypeCode;
            dest.Name = Name;
            dest.DisplayName = DisplayName;
            dest.ShortDescription = ShortDescription;
            dest.LongDescription = LongDescription;
            dest.LegalText = LegalText;
            dest.HowManyPointsToEarn = HowManyPointsToEarn;
            dest.PointType = PointType;
            dest.PointEvent = PointEvent;
            dest.ProductId = ProductId;
            dest.TierId = TierId;
            dest.Threshold = Threshold;
            dest.SmallImageFile = SmallImageFile;
            dest.LargeImageFile = LargeImageFile;
            dest.CatalogStartDate = CatalogStartDate;
            dest.CatalogEndDate = CatalogEndDate;
            dest.Active = Active;
            dest.FulfillmentProviderId = FulfillmentProviderId;
            dest.MediumImageFile = MediumImageFile;
            dest.RedeemTimeLimit = RedeemTimeLimit;
            dest.RewardType = RewardType;
            dest.ConversionRate = ConversionRate;
            return (RewardDef)base.Clone(dest);
        }

        public string GetDisplayName(string language, string channel)
        {
            return GetContent(language, channel, "DisplayName");
        }
				        
        public string GetShortDescription(string language, string channel)
		{
            return GetContent(language, channel, "ShortDescription");
		}

        public string GetLongDescription(string language, string channel)
		{
            return GetContent(language, channel, "LongDescription");
		}

        public string GetLegalText(string language, string channel)
		{
            return GetContent(language, channel, "LegalText");
		}

        public void SetDisplayName(string language, string channel, string shortDescription)
        {
            SetContent(language, channel, "DisplayName", shortDescription);
        }

        public void SetShortDescription(string language, string channel, string shortDescription)
		{
            SetContent(language, channel, "ShortDescription", shortDescription);
		}

        public void SetLongDescription(string language, string channel, string longDescription)
		{
            SetContent(language, channel, "LongDescription", longDescription);
		}

        public void SetLegalText(string language, string channel, string legalText)
		{
            SetContent(language, channel, "LegalText", legalText);
		}

        public override LWObjectAuditLogBase GetArchiveObject()
        {
            RewardDef_AL ar = new RewardDef_AL()
            {
                ObjectId = this.Id,
                CertificateTypeCode = this.CertificateTypeCode,
                Name = this.Name,
                HowManyPointsToEarn = this.HowManyPointsToEarn,
                PointType = this.PointType,
                PointEvent = this.PointEvent,
                ProductId = this.ProductId,
                TierId = this.TierId,
                Threshold = this.Threshold,
                SmallImageFile = this.SmallImageFile,
                LargeImageFile = this.LargeImageFile,
                CatalogStartDate = this.CatalogStartDate,
                CatalogEndDate = this.CatalogEndDate,
                Active = this.Active,
                FulfillmentProviderId = this.FulfillmentProviderId,
                MediumImageFile = this.MediumImageFile,
                RedeemTimeLimit = this.RedeemTimeLimit,
                RewardType = this.RewardType,
                ConversionRate = this.ConversionRate,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate
            };
            return ar;
        }

    }
}