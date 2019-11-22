using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Rules;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class CancelMemberReward : OperationProviderBase
    {
        #region Fields
        private const string _className = "CancelMemberReward";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public CancelMemberReward() : base("CancelMemberReward") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {
            string methodName = "Invoke";
            /*
             * A lot of thsi logic needs to be synchronized with IssueReward rule.
             * */
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for deleting member reward.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                long rewardId = (long)args["MemberRewardID"];
                                  
                _logger.Trace(_className,methodName,"Deleting member reward with id " + rewardId);

                MemberReward reward = LoyaltyDataService.GetMemberReward(rewardId);
                if (reward == null)
                {
                    throw new LWOperationInvocationException("Invalid member reward id.") { ErrorCode = 3356 };
                }

                string msg = string.Format("Member reward id {0} has been cancelled.", reward.Id);
                MemberRewardsUtil.CancelOrReturnMemberReward(reward, true, msg);
                Member member = LoyaltyDataService.LoadMemberFromIPCode(reward.MemberId);
                APIArguments resultParams = new APIArguments();                
                resultParams.Add("CurrencyBalance", member.GetPoints(null, null, DateTimeUtil.MinValue, DateTimeUtil.MaxValue).ToString());
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);

                #region Post Processing
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("member", member);
                context.Add("reward", reward);                
                PostProcessSuccessfullInvocation(context);
                #endregion

                return response;
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
