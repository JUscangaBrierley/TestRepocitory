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
using Brierley.FrameWork.LWIntegration;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Rewards
{
    public class RedeemMemberReward : OperationProviderBase
    {
        #region Fields
        private const string _className = "RedeemMemberReward";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public RedeemMemberReward() : base("RedeemMemberReward") { }

        #region Private Helpers
        private bool NullOutRedeemDate()
        {
            string value = GetFunctionParameter("NullRedeemDateIfNotProvided");
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                return bool.Parse(value);
            }
        }
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 4)
            {
                string errMsg = "Invalid parameters provided for RedeemMemberReward.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            long rewardId = -1;
            string rewardIdStr = (string)parms[0];
            if (!string.IsNullOrEmpty(rewardIdStr))
            {
                rewardId = long.Parse(rewardIdStr);
            }
            else
            {
                string errMsg = string.Format("No reward id provided.");
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 3218 };
            }

			MemberReward reward = LoyaltyService.GetMemberReward(rewardId);
            if (reward == null)
            {
                throw new LWOperationInvocationException(string.Format("Unable to find member reward with id = {0}.", rewardId)) { ErrorCode = 3347 };
            }

            if (!string.IsNullOrEmpty((string)parms[1]))
            {
                reward.AvailableBalance = decimal.Parse((string)parms[1]);
            }

            if (!string.IsNullOrEmpty((string)parms[2]))
            {
                reward.Expiration = (DateTime)DateTime.Parse((string)parms[2]);
            }
            if (!string.IsNullOrEmpty((string)parms[3]))
            {
                reward.RedemptionDate = (DateTime)DateTime.Parse((string)parms[3]);
            }
            else if ( NullOutRedeemDate())
            {
                reward.RedemptionDate = null;
            }

			LoyaltyService.UpdateMemberReward(reward);

            #region Post Processing
            Dictionary<string, object> context = new Dictionary<string, object>();
            context.Add("memberId", reward.MemberId);
            context.Add("reward", reward);
            PostProcessSuccessfullInvocation(context);
            #endregion

            return null;
        }
        #endregion
    }
}