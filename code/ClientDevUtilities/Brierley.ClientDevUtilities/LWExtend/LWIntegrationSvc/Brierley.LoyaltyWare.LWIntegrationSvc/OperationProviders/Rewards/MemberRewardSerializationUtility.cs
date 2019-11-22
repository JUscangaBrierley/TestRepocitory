using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class MemberRewardSerializationUtility
    {
        public static APIStruct SerializeMemberRewardSummary(
            string language,
            string channel,
            long[] vcKeys,
            DateTime? startDate,
            DateTime? endDate,
            MemberReward reward,
            bool returnAttributes)
        {
            APIArguments rewardParams = new APIArguments();
            rewardParams.Add("MemberRewardID", reward.Id);

            long[] rowKeys = { reward.Id };
            using (LoyaltyDataService svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                decimal pointsConsumed = svc.GetPointsConsumed(vcKeys, null, null, startDate, endDate, null, null, PointTransactionOwnerType.Reward, reward.RewardDefId, rowKeys);
                rewardParams.Add("PointsConsumed", Math.Abs(pointsConsumed));
            }
            if (!string.IsNullOrEmpty(reward.CertificateNmbr))
            {
                rewardParams.Add("CertificateNumber", reward.CertificateNmbr);
            }
            rewardParams.Add("AvailableBalance", reward.AvailableBalance);
            rewardParams.Add("DateIssued", reward.DateIssued);
            if (reward.Expiration != null)
            {
                rewardParams.Add("ExpirationDate", reward.Expiration);
            }
            if (reward.FulfillmentDate != null)
            {
                rewardParams.Add("FulfillmentDate", reward.FulfillmentDate);
            }
            if (reward.RedemptionDate != null)
            {
                rewardParams.Add("RedemptionDate", reward.RedemptionDate);
            }
            Product product;
            Category category;
            using (ContentService svc = LWDataServiceUtil.ContentServiceInstance())
            {
                if (reward.RewardDef == null)
                {
                    reward.RewardDef = svc.GetRewardDef(reward.RewardDefId);
                }
                product = svc.GetProduct(reward.ProductId);
                category = svc.GetCategory(product.CategoryId);
            }
            rewardParams.Add("RewardDefID", reward.RewardDefId);
            rewardParams.Add("RewardName", reward.RewardDef.Name);
            rewardParams.Add("DisplayName", reward.RewardDef.GetDisplayName(language, channel));
            rewardParams.Add("ShortDescription", reward.RewardDef.GetShortDescription(language, channel));
            rewardParams.Add("LongDescription", reward.RewardDef.GetLongDescription(language, channel));
            rewardParams.Add("LegalText", reward.RewardDef.GetLegalText(language, channel));
            rewardParams.Add("SmallImageFile", reward.RewardDef.SmallImageFile);
            rewardParams.Add("LargeImageFile", reward.RewardDef.LargeImageFile);
            rewardParams.Add("PartNumber", product.PartNumber);
            rewardParams.Add("CategoryName", category.Name);
            APIStruct rv = new APIStruct() { Name = "MemberRewardSummary", IsRequired = false, Parms = rewardParams };
            return rv;
        }
    }
}
