using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Rewards
{
    public class GetMemberRewardById : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetMemberRewardById";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetMemberRewardById() : base("GetMemberRewardById") { }

        #region Private Helpers        
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 3)
            {
                string errMsg = "Invalid parameters provided for GetMemberRewardByCert.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string idStr = (string)parms[0];
            if (!string.IsNullOrEmpty(idStr))
            {
                string errMsg = "No reward id provided for GetMemberRewardById.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }
            long rewardId = !string.IsNullOrEmpty(idStr) ? (long)int.Parse(idStr) : -1;
            MemberReward reward = LoyaltyService.GetMemberReward(rewardId);
            if (reward == null)
            {
                throw new LWOperationInvocationException("No reward found with id " + rewardId + ".") { ErrorCode = 3347 };
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
            if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
            {
                throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
            }
            if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
            {
                throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
            }
            
            Member member = token.CachedMember;
            long[] vcKeys = member.GetLoyaltyCardIds();

            MGMemberReward mgReward = MGMemberReward.Hydrate(vcKeys, reward, language, channel);
            return mgReward;
        }
        #endregion
    }
}