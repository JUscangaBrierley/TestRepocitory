using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
	[DataContract]
	public class MGBonusDef
	{
		[DataMember]
		public virtual long Id { get; set; }

		[DataMember]
		public virtual string Name { get; set; }

		[DataMember]
		public virtual long CategoryId { get; set; }

		[DataMember]
		public virtual string LogoImageHero { get; set; }

		[DataMember]
		public virtual string LogoImageWeb { get; set; }

		[DataMember]
		public virtual string LogoImageMobile { get; set; }

		[DataMember]
		public virtual string Description { get; set; }

		[DataMember]
		public virtual string Headline { get; set; }

		[DataMember]
		public virtual string FinishedHtml { get; set; }

		[DataMember]
		public virtual string QuotaMetHtml { get; set; }

		[DataMember]
		public virtual string HtmlContent { get; set; }

		[DataMember]
		public virtual string ReferralLabel { get; set; }

		[DataMember]
		public virtual string GoButtonLabel { get; set; }

		[DataMember]
		public virtual decimal? Points { get; set; }

		[DataMember]
		public virtual string SurveyText { get; set; }

		[DataMember]
		public virtual long? SurveyId { get; set; }

		[DataMember]
		public virtual string MovieUrl { get; set; }

		[DataMember]
		public virtual string ReferralUrl { get; set; }

		[DataMember]
		public virtual int DisplayOrder { get; set; }

		[DataMember]
		public virtual DateTime StartDate { get; set; }

		[DataMember]
		public virtual DateTime? ExpiryDate { get; set; }

		[DataMember]
		public virtual List<MGContentAttribute> ContentAttributes { get; set; }


		public static MGBonusDef Hydrate(BonusDef bonus, string language, string channel, bool hydrateAttributes)
		{
			if (bonus == null)
			{
				throw new ArgumentNullException("bonus");
			}
						
			var ret = new MGBonusDef()
			{
				Id = bonus.Id,
				Name = bonus.Name,
				CategoryId = bonus.CategoryId,
				LogoImageHero = bonus.LogoImageHero, 
				LogoImageWeb = bonus.LogoImageWeb, 
				LogoImageMobile = bonus.LogoImageMobile, 
				Description = bonus.GetDescription(language, channel),
				Headline = bonus.GetHeadline(language, channel), 
				FinishedHtml = bonus.GetFinishedHtml(language, channel), 
				QuotaMetHtml = bonus.GetQuotaMetHtml(language, channel), 
				HtmlContent = bonus.GetHtmlContent(language, channel), 
				ReferralLabel = bonus.GetReferralLabel(language, channel), 
				GoButtonLabel = bonus.GetGoButtonLabel(language, channel), 
				Points = bonus.Points, 
				SurveyText = bonus.SurveyText, 
				SurveyId = bonus.SurveyId, 
				MovieUrl = bonus.MovieUrl, 				
				ReferralUrl = bonus.ReferralUrl, 
				DisplayOrder = bonus.DisplayOrder.GetValueOrDefault(1), 
				StartDate = bonus.StartDate,
				ExpiryDate = bonus.ExpiryDate				
			};
			if (hydrateAttributes)
			{
				ret.ContentAttributes = new List<MGContentAttribute>();
				foreach (ContentAttribute att in bonus.ContentAttributes)
				{
					ret.ContentAttributes.Add(MGContentAttribute.Hydrate(att));
				}
			}
			return ret;
		}
	}
}