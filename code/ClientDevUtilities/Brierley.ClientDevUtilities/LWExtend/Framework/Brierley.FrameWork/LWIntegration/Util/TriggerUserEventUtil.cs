using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules;

using Brierley.FrameWork.CampaignManagement;

namespace Brierley.FrameWork.LWIntegration.Util
{
    public static class TriggerUserEventUtil
    {
        #region Fields
        private static string _className = "TriggerUserEventUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        #endregion

        #region Private Helpers

        private static List<MemberMessage> GetMessagesFromThisRun(IEnumerable<ContextObject.RuleResult> results)
        {
            var ret = new List<MemberMessage>();
            if (results == null || results.Count() == 0)
            {
                return ret;
            }

            using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                foreach (ContextObject.RuleResult result in results)
                {
                    long messageId = 0;         //transient message will only have a message id
                    long memberMessageId = 0;   //non-transient messages will have an MemberMessage record
                    
                    if (result is CampaignResult)
                    {
                        var cr = (CampaignResult)result;
                        if (cr.OutputType == OutputType.Message)
                        {
                            messageId = cr.ReferenceId;
                            memberMessageId = cr.MemberReferenceId;
                        }
                    }
                    else if (result is IssueMessage.IssueMessageRuleResult)
                    {
                        var messageResult = (IssueMessage.IssueMessageRuleResult)result;
                        messageId = messageResult.MessageId;
                        memberMessageId = messageResult.MemberMessageId;
                    }
                    else if (
                            result.OwnerType != null &&
                            result.OwnerType.Value == PointTransactionOwnerType.Message &&
                            result.OwnerId != null &&
                            result.RowKey != null
                            )
                    {
                        memberMessageId = result.RowKey.Value;
                    }

                    if (memberMessageId > 0)
                    {
                        var message = svc.GetMemberMessage(memberMessageId);
                        if (message != null)
                        {
                            ret.Add(message);
                        }
                    }
                    else if(messageId > 0)
                    {
                        ret.Add(new MemberMessage() { MessageDefId = messageId });
                    }
                }
                return ret;
            }
        }

        private static List<MemberCoupon> GetMemberCouponsFromThisRun(LoyaltyDataService svc, IEnumerable<ContextObject.RuleResult> ruleResults)
        {
            var ret = new List<MemberCoupon>();
            if (ruleResults == null || ruleResults.Count() == 0)
            {
                return ret;
            }
            if (ruleResults != null && ruleResults.Count() > 0)
            {
                foreach (ContextObject.RuleResult result in ruleResults)
                {
                    long couponId = 0;
                    if (result is CampaignResult)
                    {
                        var cr = (CampaignResult)result;
                        if (cr.OutputType == OutputType.Coupon)
                        {
                            couponId = cr.MemberReferenceId;
                        }
                    }
                    else if (
                            result.OwnerType != null &&
                            result.OwnerType.Value == PointTransactionOwnerType.Coupon &&
                            result.OwnerId != null &&
                            result.RowKey != null
                            )
                    {
                        couponId = result.RowKey.Value;
                    }

                    if (couponId > 0)
                    {
                        MemberCoupon mc = svc.GetMemberCoupon(couponId);
                        if (mc != null)
                        {
                            ret.Add(mc);
                        }
                    }
                }
            }
            return ret;
        }

        private static List<MemberBonus> GetMemberBonusesFromThisRun(LoyaltyDataService svc, IEnumerable<ContextObject.RuleResult> ruleResults)
        {
            var ret = new List<MemberBonus>();
            if (ruleResults == null || ruleResults.Count() == 0)
            {
                return ret;
            }

            foreach (var result in ruleResults)
            {
                long bonusId = 0;
                if (result is CampaignResult)
                {
                    var cr = (CampaignResult)result;
                    if (cr.OutputType == OutputType.Offer)
                    {
                        bonusId = cr.MemberReferenceId;
                    }
                }
                else if (
                        result.OwnerType != null &&
                        result.OwnerType.Value == PointTransactionOwnerType.Bonus &&
                        result.OwnerId != null &&
                        result.RowKey != null
                        )
                {
                    bonusId = result.RowKey.Value;
                }

                if (bonusId > 0)
                {
                    var bonus = svc.GetMemberOffer(bonusId);
                    if (bonus != null)
                    {
                        ret.Add(bonus);
                    }
                }
            }
            return ret;
        }

        private static List<MemberPromotion> GetMembePromotionsFromThisRun(LoyaltyDataService svc, IEnumerable<ContextObject.RuleResult> ruleResults)
        {
            var ret = new List<MemberPromotion>();
            if (ruleResults == null || ruleResults.Count() == 0)
            {
                return ret;
            }
            foreach (var result in ruleResults)
            {
                long promoId = 0;
                if (result is CampaignResult)
                {
                    var cr = (CampaignResult)result;
                    if (cr.OutputType == OutputType.Promotion)
                    {
                        promoId = cr.MemberReferenceId;
                    }
                }
                else if (result.OwnerType != null && result.OwnerType.Value == PointTransactionOwnerType.Promotion && result.OwnerId != null && result.RowKey != null)
                {
                    promoId = result.RowKey.Value;
                }

                if (promoId > 0)
                {
                    var promotion = svc.GetMemberPromotion(promoId);
                    if (promotion != null)
                    {
                        ret.Add(promotion);
                    }
                }
            }
            return ret;
        }

        #endregion

        public static void ExtractTriggerUserEventOutput(
            bool returnMessages, bool returnAllMessages, out IList<MemberMessage> messages,
            bool returnCoupons, bool returnAllCoupons, out IList<MemberCoupon> coupons,
            bool returnBonuses, bool returnAllBonuses, out IList<MemberBonus> bonuses,
            bool returnPromotions, bool returnAllPromotions, out IList<MemberPromotion> promotions,
            Member member, string language, string channel,
            IList<ContextObject.RuleResult> ruleResults,
            out string xmlResult
            )
        {
            string methodName = "ExtractTriggerUserEventOutput";

            using (var service = LWDataServiceUtil.ContentServiceInstance())
            using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {

                XDocument doc = new XDocument();
                XElement root = new XElement("TriggerUserEventResult");
                doc.Add(root);

                xmlResult = null;

                if (returnCoupons || returnBonuses || returnMessages || returnPromotions)
                {
                    if (!LanguageChannelUtil.IsLanguageValid(service, language))
                    {
                        string errMsg = string.Format("Invalid language {0} specified.", language);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWException(errMsg) { ErrorCode = 1 };
                    }
                    if (!LanguageChannelUtil.IsChannelValid(service, channel))
                    {
                        string errMsg = string.Format("Invalid channel {0} specified.", language);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWException(errMsg) { ErrorCode = 1 };
                    }
                }


                messages = null;
                coupons = null;
                bonuses = null;
                promotions = null;

                #region Extract Content

                #region Extract Messages
                if (returnMessages)
                {
                    IList<MemberMessage> thisRunMessages = GetMessagesFromThisRun(ruleResults);
                    if (thisRunMessages != null && thisRunMessages.Count > 0)
                    {
                        XElement msgRoot = new XElement("Messages");
                        root.Add(msgRoot);
                        foreach (MemberMessage msg in thisRunMessages)
                        {
                            XElement msgNode = new XElement("MemberMessage");
                            XAttribute att = new XAttribute("Id", msg.Id);
                            msgNode.Add(att);
                            msgRoot.Add(msgNode);
                        }
                    }

                    if (returnAllMessages)
                    {
                        // return all active member messages
                        messages = loyalty.GetMemberMessagesByMember(member.IpCode, null);
                    }
                    else
                    {
                        if (thisRunMessages != null)
                        {
                            messages = thisRunMessages;
                        }
                        else
                        {
                            messages = new List<MemberMessage>();
                        }
                    }
                }
                #endregion

                #region Extract Coupons
                if (returnCoupons)
                {
                    IList<MemberCoupon> thisRunCoupons = GetMemberCouponsFromThisRun(loyalty, ruleResults);
                    if (thisRunCoupons != null && thisRunCoupons.Count > 0)
                    {
                        XElement couponRoot = new XElement("Coupons");
                        root.Add(couponRoot);
                        foreach (MemberCoupon coupon in thisRunCoupons)
                        {
                            XElement couponNode = new XElement("MemberCoupon");
                            XAttribute att = new XAttribute("Id", coupon.ID);
                            couponNode.Add(att);
                            couponRoot.Add(couponNode);
                        }
                    }
                    if (returnAllCoupons)
                    {
                        // return all active member coupons
                        coupons = loyalty.GetMemberCouponsByMember(member.IpCode);
                    }
                    else
                    {
                        if (thisRunCoupons != null)
                        {
                            coupons = thisRunCoupons;
                        }
                        else
                        {
                            coupons = new List<MemberCoupon>();
                        }
                    }
                }
                #endregion

                #region Extract Bonuses
                if (returnBonuses)
                {
                    IList<MemberBonus> thisRunBonuses = GetMemberBonusesFromThisRun(loyalty, ruleResults);
                    if (thisRunBonuses != null && thisRunBonuses.Count > 0)
                    {
                        XElement bonusRoot = new XElement("Bonuses");
                        root.Add(bonusRoot);
                        foreach (MemberBonus bonus in thisRunBonuses)
                        {
                            XElement bonusNode = new XElement("MemberBonus");
                            XAttribute att = new XAttribute("Id", bonus.ID);
                            bonusNode.Add(att);
                            bonusRoot.Add(bonusNode);
                        }
                    }
                    if (returnAllBonuses)
                    {
                        // return all active member bonuses
                        bonuses = loyalty.GetMemberBonusesByMember(member.IpCode, null, true, null, null);
                    }
                    else
                    {
                        if (thisRunBonuses != null)
                        {
                            bonuses = thisRunBonuses;
                        }
                        else
                        {
                            bonuses = new List<MemberBonus>();
                        }
                    }
                }
                #endregion

                #region Extract Promotions
                if (returnPromotions)
                {
                    IList<MemberPromotion> thisRunPromotions = GetMembePromotionsFromThisRun(loyalty, ruleResults);
                    if (thisRunPromotions != null && thisRunPromotions.Count > 0)
                    {
                        XElement promoRoot = new XElement("Promotions");
                        root.Add(promoRoot);
                        foreach (MemberPromotion promo in thisRunPromotions)
                        {
                            XElement promoNode = new XElement("MemberPromotion");
                            XAttribute att = new XAttribute("Id", promo.Id);
                            promoNode.Add(att);
                            promoRoot.Add(promoNode);
                        }
                    }
                    if (returnAllPromotions)
                    {
                        // return all active member promotions
                        promotions = loyalty.GetMemberPromotionsByMember(member.IpCode, null);
                    }
                    else
                    {
                        if (thisRunPromotions != null)
                        {
                            promotions = thisRunPromotions;
                        }
                        else
                        {
                            promotions = new List<MemberPromotion>();
                        }
                    }
                }
                #endregion

                xmlResult = doc.ToString();

                #endregion
            }
        }

        /// <summary>
        /// This method deserializes the result contents of a trigger user event.
        /// </summary>
        /// <param name="xmlMessage"></param>
        public static int DeserializeResult(string xmlMessage,
            out long[] messageIds,
            out long[] couponIds,
            out long[] bonusIds,
            out long[] promotionIds)
        {
            int totalOffers = 0;
            messageIds = null;
            couponIds = null;
            bonusIds = null;
            promotionIds = null;

            XDocument doc = XDocument.Parse(xmlMessage);
            XElement root = doc.Root;

            IEnumerable<XElement> messages = root.Elements("Messages");
            if (messages != null && messages.Count() > 0)
            {
                List<long> ids = new List<long>();
                foreach (XElement message in messages.Elements())
                {
                    XAttribute att = message.Attribute("Id");
                    if (att != null)
                    {
                        ids.Add(long.Parse(att.Value));
                    }
                }
                if (ids.Count > 0)
                {
                    messageIds = ids.ToArray<long>();
                    totalOffers += ids.Count;
                }
            }

            IEnumerable<XElement> coupons = root.Elements("Coupons");
            if (coupons != null && coupons.Count() > 0)
            {
                List<long> ids = new List<long>();
                foreach (XElement coupon in coupons.Elements())
                {
                    XAttribute att = coupon.Attribute("Id");
                    if (att != null)
                    {
                        ids.Add(long.Parse(att.Value));
                    }
                }
                if (ids.Count > 0)
                {
                    couponIds = ids.ToArray<long>();
                    totalOffers += ids.Count;
                }
            }

            IEnumerable<XElement> bonuses = root.Elements("Bonuses");
            if (bonuses != null && bonuses.Count() > 0)
            {
                List<long> ids = new List<long>();
                foreach (XElement bonus in bonuses.Elements())
                {
                    XAttribute att = bonus.Attribute("Id");
                    if (att != null)
                    {
                        ids.Add(long.Parse(att.Value));
                    }
                }
                if (ids.Count > 0)
                {
                    bonusIds = ids.ToArray<long>();
                    totalOffers += ids.Count;
                }
            }

            IEnumerable<XElement> promotions = root.Elements("Promotions");
            if (promotions != null && promotions.Count() > 0)
            {
                List<long> ids = new List<long>();
                foreach (XElement promotion in promotions.Elements())
                {
                    XAttribute att = promotion.Attribute("Id");
                    if (att != null)
                    {
                        ids.Add(long.Parse(att.Value));
                    }
                }
                if (ids.Count > 0)
                {
                    promotionIds = ids.ToArray<long>();
                    totalOffers += ids.Count;
                }
            }

            return totalOffers;
        }
    }
}
