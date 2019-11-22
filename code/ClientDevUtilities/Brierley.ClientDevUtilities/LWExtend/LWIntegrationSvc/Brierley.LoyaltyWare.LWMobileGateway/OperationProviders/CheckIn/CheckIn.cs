using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

using Brierley.LoyaltyWare.LWMobileGateway.OperationProviders;
using Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Surveys;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.CheckIn
{
    public class CheckIn : CheckInBase
    {
        #region Fields
        private const string _className = "CheckIn";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public CheckIn() : base("CheckIn") { }
        
        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 4)
            {
                string errMsg = "Invalid parameters provided for CheckIn.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            MGLocation location = (MGLocation)parms[0];
            double mapRadiusInKM = (double)parms[1];            
            string language = (string)parms[2];
            if (string.IsNullOrEmpty(language))
            {
                language = LanguageChannelUtil.GetDefaultCulture();
            }            
            string channel = (string)parms[3];
            if (string.IsNullOrEmpty(channel))
            {
                channel = LanguageChannelUtil.GetDefaultChannel();
            }
            Member member = token.CachedMember;

            #region Initialize Configuration Parameters
            string awardPointRule = GetFunctionParameter("AwardPointRule");
            bool awardPointsForMultipleCheckIns = GetBooleanFunctionalParameter("AwardPointsForMultipleCheckIns", true);
            bool returnNearbyStores = GetBooleanFunctionalParameter("ReturnNearbyStores", true);
            string checkInRulesEvent = GetFunctionParameter("CheckInRulesEvent");
            bool returnCoupons = GetBooleanFunctionalParameter("ReturnCoupons", true);
            bool returnAllCoupons = GetBooleanFunctionalParameter("ReturnAllCoupons", true);
            bool returnBonuses = GetBooleanFunctionalParameter("ReturnBonuses", true);
            bool returnAllBonuses = GetBooleanFunctionalParameter("ReturnAllBonuses", true);
            bool returnPromotions = GetBooleanFunctionalParameter("ReturnPromotions", true);
            bool returnAllPromotions = GetBooleanFunctionalParameter("ReturnAllPromotions", true);
            bool returnMessages = GetBooleanFunctionalParameter("ReturnMessages", true);
            bool returnAllMessages = GetBooleanFunctionalParameter("ReturnAllMessages", true);
            #endregion
                        
            MGCheckInResponse response = new MGCheckInResponse();

            long nTimesChecked = 0;
            StoreDef store = null;
            response.CheckInStatus = VerifyCheckInAllowed(member, location, mapRadiusInKM, out nTimesChecked, ref store);

            if (response.CheckInStatus == MemberMobileCheckInStatus.Ok)
            {
                response.StoreLocation = MGStoreDef.Hydrate(store);
                bool awardPoints = nTimesChecked < 1 || awardPointsForMultipleCheckIns ? true : false;
                ContextObject eContext = MobileService.MemberCheckIn(member, location.Longitude, location.Latitude, mapRadiusInKM, store, awardPointRule, awardPoints, checkInRulesEvent);
                
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

                #region Log Event
                TriggerUserEventLog log = new TriggerUserEventLog()
                {
                    MemberId = member.IpCode,
                    EventName = checkInRulesEvent,
                    Channel = channel,
                };

                log.Context = log.SerializeContext(eContext.Environment);
                log.RulesExecuted = log.SerailizeRuleExecutionLogIds(eContext.Results);
                log.Result = xmlResult;

                EventLogger.RequestQueue.Add(log);
                #endregion

                #endregion
            }
            
            #region Post Processing
            Dictionary<string, object> context = new Dictionary<string, object>();
            context.Add("member", member);
            context.Add("store", response.StoreLocation);
            PostProcessSuccessfullInvocation(context);
            #endregion
            
            return response;
        }
        #endregion
    }
}