//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// This class defines an advertising message.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_BonusDef")]
    [AuditLog(true)]
	public class BonusDef : ContentDefBase
	{
		/// <summary>
		/// Initializes a new instance of the BonusDef class
		/// </summary>
		public BonusDef()
			: base(ContentObjType.Bonus)
		{
		}

		/// <summary>
		/// Gets or sets the Name for the current BonusDef
		/// </summary>
        [PetaPoco.Column(Length = 100, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the CategoryId for the current BonusDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Category), "ID")]
		public long CategoryId { get; set; }

		/// <summary>
		/// Gets or sets the folder id for the current BonusDef
		/// </summary>
        [PetaPoco.Column]
        [ColumnIndex]
		public long? FolderId { get; set; }

		/// <summary>
		/// Gets or sets the LogoImageHero for the current BonusDef
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string LogoImageHero { get; set; }

		/// <summary>
		/// Gets or sets the LogoImageWeb for the current BonusDef
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string LogoImageWeb { get; set; }

		/// <summary>
		/// Gets or sets the LogoImageMobile for the current BonusDef
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string LogoImageMobile { get; set; }

		/// <summary>
		/// Gets or sets the Description for the current BonusDef
		/// </summary>
		public string Description
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
		/// Gets or sets the Headline for the current BonusDef
		/// </summary>
		public string Headline
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Headline");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "Headline", value);
			}
		}

		/// <summary>
		/// Gets or sets the FinishedHtml for the current BonusDef
		/// </summary>
		public string FinishedHtml
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "FinishedHtml");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "FinishedHtml", value);
			}
		}

		/// <summary>
		/// Gets or sets the QuotaMetHtml for the current BonusDef
		/// </summary>
		public string QuotaMetHtml
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "QuotaMetHtml");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "QuotaMetHtml", value);
			}
		}

		/// <summary>
		/// Gets or sets the QuotaMetHtml for the current BonusDef
		/// </summary>
		public string HtmlContent
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "HtmlContent");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "HtmlContent", value);
			}
		}

		/// <summary>
		/// Gets or sets the ActionText for the current BonusDef
		/// </summary>
		public string ReferralLabel
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "ReferralLabel");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "ReferralLabel", value);
			}
		}

		/// <summary>
		/// Gets or sets the GoButtonLabel for the current BonusDef
		/// </summary>
		public string GoButtonLabel
		{
			get
			{
				return GetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "ButtonLabel");
			}
			set
			{
				SetContent(LanguageChannelUtil.GetDefaultCulture(), LanguageChannelUtil.GetDefaultChannel(), "ButtonLabel", value);
			}
		}

		/// <summary>
		/// Gets or sets the Points for the current BonusDef
		/// </summary>
        [PetaPoco.Column]
		public decimal? Points { get; set; }

		/// <summary>
		/// Gets or sets the SurveyText for the current BonusDef
		/// </summary>
        [PetaPoco.Column(Length = 25)]
		public string SurveyText { get; set; }

		/// <summary>
		/// Gets or sets the SurveyId for the current BonusDef
		/// </summary>
        [PetaPoco.Column]
		public long? SurveyId { get; set; }

		/// <summary>
		/// Gets or sets expression for survey points for the current BonusDef
		/// </summary>
        [PetaPoco.Column]
		public string SurveyPointsExpression { get; set; }
				
		/// <summary>
		/// Gets or sets the MovieUrl for the current BonusDef
		/// </summary>
        [PetaPoco.Column(Name = "ActionUrl", Length = 255)]
		public string MovieUrl { get; set; }

		/// <summary>
		/// Gets or sets the ReferralUrl for the current BonusDef
		/// </summary>
        [PetaPoco.Column(Length = 255)]
		public string ReferralUrl { get; set; }

		/// <summary>
		/// Gets or sets the display order of the BonusDef
		/// </summary>
        [PetaPoco.Column]
		public int? DisplayOrder { get; set; }

		/// <summary>
		/// Gets or sets the StartDate for the current BonusDef
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Gets or sets the expiration date of the bonus
		/// </summary>
        [PetaPoco.Column]
		public DateTime? ExpiryDate { get; set; }

		/// <summary>
		/// Gets or sets the Quota for the current BonusDef
		/// </summary>
        [PetaPoco.Column]
		public long? Quota { get; set; }

		/// <summary>
		/// Gets or sets the ApplyQuotaToReferral for the current BonusDef
		/// </summary>
        [PetaPoco.Column]
		public bool? ApplyQuotaToReferral { get; set; }

		/// <summary>
		/// Gets or sets the Completed for the current BonusDef
		/// </summary>
        [PetaPoco.Column]
		public long? Completed { get; set; }

		
		public string GetDescription(string language, string channel)
		{
			return GetContent(language, channel, "Description");
		}

		public string GetHeadline(string language, string channel)
		{
			return GetContent(language, channel, "Headline");
		}

		public string GetFinishedHtml(string language, string channel)
		{
			return GetContent(language, channel, "FinishedHtml");
		}

		public string GetQuotaMetHtml(string language, string channel)
		{
			return GetContent(language, channel, "QuotaMetHtml");
		}

		public string GetHtmlContent(string language, string channel)
		{
			return GetContent(language, channel, "HtmlContent");
		}

		public string GetReferralLabel(string language, string channel)
		{
			return GetContent(language, channel, "ReferralLabel");
		}

		public string GetGoButtonLabel(string language, string channel)
		{
			return GetContent(language, channel, "ButtonLabel");
		}

		public void SetDescription(string language, string channel, string description)
		{
			SetContent(language, channel, "Description", description);
		}

		public virtual void SetHeadline(string language, string channel, string headline)
		{
			SetContent(language, channel, "Headline", headline);
		}

		public void SetFinishedHtml(string language, string channel, string html)
		{
			SetContent(language, channel, "FinishedHtml", html);
		}

		public void SetQuotaMetHtml(string language, string channel, string html)
		{
			SetContent(language, channel, "QuotaMetHtml", html);
		}

		public void SetHtmlContent(string language, string channel, string html)
		{
			SetContent(language, channel, "HtmlContent", html);
		}

		public void SetReferralLabel(string language, string channel, string actionText)
		{
			SetContent(language, channel, "ReferralLabel", actionText);
		}

		public void SetGoButtonLabel(string language, string channel, string actionText)
		{
			SetContent(language, channel, "ButtonLabel", actionText);
		}

		public BonusDef Clone()
		{
			return Clone(new BonusDef());
		}

		public BonusDef Clone(BonusDef dest)
		{
			dest.Name = Name;
			dest.CategoryId = CategoryId;
			dest.FolderId = FolderId;
			dest.LogoImageHero = LogoImageHero;
			dest.LogoImageWeb = LogoImageWeb;
			dest.LogoImageMobile = LogoImageMobile;
			dest.Description = Description;
			dest.Headline = Headline;
			dest.FinishedHtml = FinishedHtml;
			dest.QuotaMetHtml = QuotaMetHtml;
			dest.Points = Points;
			dest.SurveyText = SurveyText;
			dest.SurveyId = SurveyId;
			dest.SurveyPointsExpression = SurveyPointsExpression;
			dest.ReferralLabel = ReferralLabel;
			dest.MovieUrl = MovieUrl;
			dest.ReferralUrl = ReferralUrl;
			dest.DisplayOrder = DisplayOrder;
			dest.StartDate = StartDate;
			dest.ExpiryDate = ExpiryDate;
			dest.Quota = Quota;
			dest.ApplyQuotaToReferral = ApplyQuotaToReferral;
			return (BonusDef)base.Clone(dest);
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			BonusDef_AL ar = new BonusDef_AL()
			{
				ObjectId = this.Id,
				Name = this.Name,
				CategoryId = this.CategoryId,
				FolderId = this.FolderId,
				LogoImageHero = this.LogoImageHero,
				LogoImageWeb = this.LogoImageWeb,
				LogoImageMobile = this.LogoImageMobile,
				Points = this.Points,
				SurveyText = this.SurveyText,
				SurveyId = this.SurveyId,
				MovieUrl = this.MovieUrl,
				ReferralUrl = this.ReferralUrl,
				DisplayOrder = this.DisplayOrder,
				StartDate = this.StartDate,
				ExpiryDate = this.ExpiryDate,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate,
				Quota = this.Quota,
				ApplyQuotaToReferral = this.ApplyQuotaToReferral,
				Completed = this.Completed
			};
			return ar;
		}
	}
}
