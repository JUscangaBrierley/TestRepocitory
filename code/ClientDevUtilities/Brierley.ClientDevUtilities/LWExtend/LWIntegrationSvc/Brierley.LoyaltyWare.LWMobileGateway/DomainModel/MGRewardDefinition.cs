using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGRewardDefinition
    {
        public virtual long Id { get; set; }
        public virtual long CategoryId { get; set; }
        public virtual String CertificateTypeCode { get; set; }
        public virtual String Name { get; set; }
        public virtual String DisplayName { get; set; }
        public virtual String ShortDescription { get; set; }
        public virtual String LongDescription { get; set; }
        public virtual String LegalText { get; set; }
        public virtual Decimal CurrencyToEarn { get; set; }
        public virtual String LoyaltyCurrency { get; set; }
        public virtual String LoyaltyEvent { get; set; }
        public virtual String TierName { get; set; }
        public virtual string SmallImageFile { get; set; }
        public virtual string LargeImageFile { get; set; }
        public virtual DateTime? CatalogStartDate { get; set; }
        public virtual DateTime? CatalogEndDate { get; set; }
        public virtual Boolean Active { get; set; }
        public virtual string MediumImageFile { get; set; }
        public virtual List<MGContentAttribute> ContentAttributes { get; set; }

		public static MGRewardDefinition Hydrate(RewardDef reward, string lang, string channel, bool returnAttributes)
		{
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			using (var content = LWDataServiceUtil.ContentServiceInstance())
			{
				Product p = content.GetProduct(reward.ProductId);
				string tierName = reward.TierId != null ? loyalty.GetTierDef(reward.TierId.Value).Name : string.Empty;

				MGRewardDefinition mc = new MGRewardDefinition()
				{
					Id = reward.Id,
					Name = reward.Name,
                    DisplayName = reward.GetDisplayName(lang, channel), 
					CategoryId = p.CategoryId,
					CertificateTypeCode = reward.CertificateTypeCode,
					ShortDescription = reward.GetShortDescription(lang, channel),
					LongDescription = reward.GetLongDescription(lang, channel),
					LegalText = reward.GetLegalText(lang, channel),
					CurrencyToEarn = reward.HowManyPointsToEarn,
					LoyaltyCurrency = reward.PointType,
					LoyaltyEvent = reward.PointEvent,
					TierName = tierName,
					SmallImageFile = reward.SmallImageFile,
					LargeImageFile = reward.LargeImageFile,
					CatalogStartDate = reward.CatalogStartDate,
					CatalogEndDate = reward.CatalogEndDate,
					Active = reward.Active,
                    MediumImageFile = reward.MediumImageFile,
				};

				if (returnAttributes)
				{
					mc.ContentAttributes = new List<MGContentAttribute>();
					foreach (ContentAttribute att in reward.ContentAttributes)
					{
						mc.ContentAttributes.Add(MGContentAttribute.Hydrate(att));
					}
				}
				return mc;
			}
		}
    }
}