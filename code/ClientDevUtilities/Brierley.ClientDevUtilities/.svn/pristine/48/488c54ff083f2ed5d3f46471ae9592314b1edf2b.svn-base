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

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
    public class TriggerUserEvent : OperationProviderBase
    {
        #region Fields
        private const string _className = "TriggerUserEvent";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        //private object _campaignUtil = null;        
        #endregion

        #region Construction
        public TriggerUserEvent() : base("TriggerUserEvent") { }
        #endregion
        
        #region Overriden Methods
        public override object Invoke(string source,WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 4)
            {
                string errMsg = "Invalid parameters provided for TriggerUserEvent.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string eventName = (string)parms[0];
            if (string.IsNullOrEmpty(eventName))
            {
                string errMsg = "No event name provided for TriggerUserEvent.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }
            string language = (string)parms[1];
            if (string.IsNullOrEmpty(language))
            {
                language = LanguageChannelUtil.GetDefaultCulture();
            }
            string channel = (string)parms[2];
            if (string.IsNullOrEmpty(channel))
            {
                channel = LanguageChannelUtil.GetDefaultChannel();
            }


            Member member = token.CachedMember;

            Dictionary<string, object> contextMap = new Dictionary<string, object>();
            contextMap.Add("Channel", channel);
            contextMap.Add("Language", language);
            
			if (parms[3] != null)
			{
				var context = (Dictionary<string, string>)parms[3];
				foreach (var key in context.Keys)
				{
					contextMap.Add(key, context[key]);
				}
			}

                        
            LWEvent lwEvent = LoyaltyService.GetLWEventByName(eventName);
            if (lwEvent == null)
            {
                string errMsg = string.Format("Unable to find user defined event name = {0}.", eventName);
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 3382 };
            }

            ContextObject eContext = new ContextObject()
            {
                Owner = member,
                Environment = contextMap
            };
            LoyaltyService.ExecuteEventRules(eContext, eventName);
            
            #region Prepare Response

            bool returnCoupons = GetBooleanFunctionalParameter("ReturnCoupons", true);
            bool returnAllCoupons = GetBooleanFunctionalParameter("ReturnAllCoupons", true);
            bool returnBonuses = GetBooleanFunctionalParameter("ReturnBonuses", true);
            bool returnAllBonuses = GetBooleanFunctionalParameter("ReturnAllBonuses", true);
            bool returnPromotions = GetBooleanFunctionalParameter("ReturnPromotions", true);
            bool returnAllPromotions = GetBooleanFunctionalParameter("ReturnAllPromotions", true);
            bool returnMessages = GetBooleanFunctionalParameter("ReturnMessages", true);
            bool returnAllMessages = GetBooleanFunctionalParameter("ReturnAllMessages", true);

            MGTriggerUserEvent response = new MGTriggerUserEvent()
            {
                AccountSummary = new MGAccountSummary(),
                Coupons = new List<MGMemberCoupon>(),
                Messages = new List<MGMemberMessage>()
            };

            #region General
            response.AccountSummary.CurrencyBalance = member.GetPoints(null, null, null, null);                        
            MemberTier tier = member.GetTier(DateTime.Now);
            if (tier != null)
            {
                TierDef def = LoyaltyService.GetTierDef(tier.TierDefId);
                response.AccountSummary.CurrentTierName = def.Name;
                response.AccountSummary.CurrentTierExpirationDate = tier.ToDate;                
            }
            response.AccountSummary.CurrencyToNextTier = member.GetPointsToNextTier();
            if (member.LastActivityDate != null)
            {
                response.AccountSummary.LastActivityDate = member.LastActivityDate;
            }
            #endregion

            #region Extract Content

            List<MGMemberMessage> mgMessages = null;
            List<MGMemberCoupon> mgCoupons = null;
            List<MGMemberCoupon> mgBonuses = null;
            List<MGMemberCoupon> mgPromotions = null;
            string xmlResult = null;

            MGRealtimeContentUtil.ExtractTriggerUserEventOutput(
                returnMessages, returnAllMessages, out mgMessages,
                returnCoupons, returnAllCoupons, out mgCoupons,
                returnBonuses, returnAllBonuses, out mgBonuses,
                returnPromotions, returnAllPromotions, out mgPromotions,
                member, language, channel, eContext.Results, out xmlResult);

            response.Messages = mgMessages;
            response.Coupons = mgCoupons;

            #endregion

            #region Log Event
            TriggerUserEventLog log = new TriggerUserEventLog()
            {
                MemberId = member.IpCode,
                EventName = eventName,
                Channel = channel,
            };

            log.Context = log.SerializeContext(contextMap);
            log.RulesExecuted = log.SerailizeRuleExecutionLogIds(eContext.Results);
            log.Result = xmlResult;

            EventLogger.RequestQueue.Add(log);
            #endregion

            return response;

            #endregion
        }
        #endregion
    }
}
