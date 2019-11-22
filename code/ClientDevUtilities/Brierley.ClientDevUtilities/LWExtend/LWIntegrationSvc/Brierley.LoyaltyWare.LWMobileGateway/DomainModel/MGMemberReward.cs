using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.ClientDevUtilities.LWGateway;

namespace Brierley.ClientDevUtilities.LWExtend.LoyaltyWare.LWMobileGateway.DomainModel
{
    public static class MGMemberRewardUtility
    {
        #region Data Transfer Methods
		public static T Hydrate<T>(ILWDataServiceUtil lwDataServiceUtil, long[] vcKeys, MemberReward reward, string lang, string channel) where T : MGMemberReward, new()
        {
			using (var content = lwDataServiceUtil.ContentServiceInstance())
			using (var loyalty = lwDataServiceUtil.LoyaltyDataServiceInstance())
			{
				RewardDef rdef = content.GetRewardDef(reward.RewardDefId);
				long[] rowKeys = { reward.Id };
				decimal pointsConsumed = loyalty.GetPointsConsumed(vcKeys, null, null, null, null, null, null, PointTransactionOwnerType.Reward, reward.RewardDefId, rowKeys);
				var mc = new T()
				{
					Id = reward.Id,
					RewardDefId = reward.RewardDefId,
					OfferCode = reward.OfferCode,
					CertNmbr = reward.CertificateNmbr,
					CurrencyConsumed = pointsConsumed,
					AvailableBalance = reward.AvailableBalance,
					DateIssued = reward.DateIssued,
					DateRedeemed = reward.RedemptionDate,
					ExpiryDate = reward.Expiration,
					DateFulfilled = reward.FulfillmentDate,
					LWOrderNumber = reward.LWOrderNumber,
					TrackingNumber = reward.TrackingNumber,
					TrackingUrl = reward.TrackingUrl,
					Name = rdef.Name,
					ShortDescription = rdef.GetShortDescription(lang, channel),
					LongDescription = rdef.GetLongDescription(lang, channel),
					LegalText = rdef.GetLegalText(lang, channel),
					CurrencyName = rdef.PointType
				};
				mc.OrderStatus = reward.OrderStatus != null ? reward.OrderStatus.Value.ToString() : RewardOrderStatus.Created.ToString();
				return mc;
			}
		}
        #endregion
    }
}