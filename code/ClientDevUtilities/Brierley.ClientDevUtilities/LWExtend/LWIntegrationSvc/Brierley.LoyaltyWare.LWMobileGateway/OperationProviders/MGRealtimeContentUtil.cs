using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders
{
    public static class MGRealtimeContentUtil
    {
        public static void ExtractTriggerUserEventOutput(
            bool returnMessages, bool returnAllMessages, out List<MGMemberMessage> mgMessages,
            bool returnCoupons, bool returnAllCoupons, out List<MGMemberCoupon> mgCoupons,
            bool returnBonuses, bool returnAllBonuses, out List<MGMemberCoupon> mgBonuses,
            bool returnPromotions, bool returnAllPromotions, out List<MGMemberCoupon> mgPromotions,
            Member member, string language, string channel,
            IList<ContextObject.RuleResult> results,
            out string xmlResult
            )
        {
            IList<MemberMessage> messages = null;
            IList<MemberCoupon> coupons = null;
            IList<MemberBonus> bonuses = null;
            IList<MemberPromotion> promotions = null;
            
            TriggerUserEventUtil.ExtractTriggerUserEventOutput(
                returnMessages, returnAllMessages, out messages, 
                returnCoupons, returnAllCoupons, out coupons,
                returnBonuses, returnAllBonuses, out bonuses, 
                returnPromotions, returnAllPromotions, out promotions, 
                member, language, channel, results, out xmlResult);

            mgMessages = null;
            mgCoupons = null;
            mgBonuses = null;
            mgPromotions = null;

            if (messages != null && messages.Count > 0)
            {
                mgMessages = new List<MGMemberMessage>();
                foreach (MemberMessage message in messages)
                {
                    mgMessages.Add(MGMemberMessage.Hydrate(member, message, language, channel));
                }
            }

            if (coupons != null && coupons.Count > 0)
            {
                mgCoupons = new List<MGMemberCoupon>();
                foreach (MemberCoupon coupon in coupons)
                {
                    mgCoupons.Add(MGMemberCoupon.Hydrate(member, coupon, language, channel, false));
                }
            }

            if (bonuses != null && bonuses.Count > 0)
            {
            }

            if (promotions != null && promotions.Count > 0)
            {
            }
        }
    }
}