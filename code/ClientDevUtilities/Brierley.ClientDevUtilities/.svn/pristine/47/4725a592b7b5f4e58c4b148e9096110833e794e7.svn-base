using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class RewardDefSerializationUtility
    {
        public static APIArguments SerializeRewardDefinition(RewardDef reward, string language, string channel, bool returnAttributes, ContentService ContentService)
        {
            APIArguments rewardCatalogItemParams = new APIArguments();
            rewardCatalogItemParams.Add("RewardID", reward.Id);
            rewardCatalogItemParams.Add("RewardName", reward.Name);
            rewardCatalogItemParams.Add("DisplayName", reward.GetDisplayName(language, channel));
            if (!string.IsNullOrEmpty(reward.CertificateTypeCode))
            {
                rewardCatalogItemParams.Add("TypeCode", reward.CertificateTypeCode);
            }
            if (!string.IsNullOrEmpty(reward.GetShortDescription(language, channel)))
            {
                rewardCatalogItemParams.Add("ShortDescription", reward.GetShortDescription(language, channel));
            }
            if (!string.IsNullOrEmpty(reward.GetLongDescription(language, channel)))
            {
                rewardCatalogItemParams.Add("LongDescription", reward.GetLongDescription(language, channel));
            }
            if (!string.IsNullOrEmpty(reward.GetLegalText(language, channel)))
            {
                rewardCatalogItemParams.Add("LegalText", reward.GetLegalText(language, channel));
            }
            rewardCatalogItemParams.Add("CurrencyToEarn", reward.HowManyPointsToEarn);
            rewardCatalogItemParams.Add("CurrencyType", reward.PointType);
            if (!string.IsNullOrEmpty(reward.SmallImageFile))
            {
                rewardCatalogItemParams.Add("SmallImageFile", reward.SmallImageFile);
            }
            if (!string.IsNullOrEmpty(reward.MediumImageFile))
            {
                rewardCatalogItemParams.Add("MediumImageFile", reward.MediumImageFile);
            }
            if (!string.IsNullOrEmpty(reward.LargeImageFile))
            {
                rewardCatalogItemParams.Add("LargeImageFile", reward.LargeImageFile);
            }
            if (reward.CatalogStartDate != null)
            {
                rewardCatalogItemParams.Add("CatalogStartDate", reward.CatalogStartDate);
            }
            if (reward.CatalogEndDate != null)
            {
                rewardCatalogItemParams.Add("CatalogEndDate", reward.CatalogEndDate);
            }
            rewardCatalogItemParams.Add("Active", reward.Active);
            if (reward.RedeemTimeLimit != null)
            {
                rewardCatalogItemParams.Add("RedeemTimeLimit", reward.RedeemTimeLimit);
            }
            if(reward.ConversionRate.HasValue)
            {
                rewardCatalogItemParams.Add("ConversionRate", reward.ConversionRate.Value);
            }
            rewardCatalogItemParams.Add("RewardType", reward.RewardType);

            if (returnAttributes && reward.Attributes.Count > 0)
            {
                APIStruct[] atts = new APIStruct[reward.Attributes.Count];
                int idx = 0;
                foreach (ContentAttribute ra in reward.Attributes)
                {
                    ContentAttributeDef def = ContentService.GetContentAttributeDef(ra.ContentAttributeDefId);
                    APIArguments attparms = new APIArguments();
                    attparms.Add("AttributeName", def.Name);
                    attparms.Add("AttributeValue", ra.Value);
                    APIStruct v = new APIStruct() { Name = "ContentAttributes", IsRequired = false, Parms = attparms };
                    atts[idx++] = v;
                }
                rewardCatalogItemParams.Add("ContentAttributes", atts);
            }
            return rewardCatalogItemParams;
        }
    }
}
