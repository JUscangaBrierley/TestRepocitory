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
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class IsMemberRewardRedeemed : OperationProviderBase
    {
        #region Fields
        //private const string _className = "IsMemberRewardRedeemed";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public IsMemberRewardRedeemed() : base("IsMemberRewardRedeemed") { }
        #endregion

        #region Private Helpers        
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            /*
             * A lot of thsi logic needs to be synchronized with IssueReward rule.
             * */
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for checking reward redemption status.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                long rewardId = (long)args["MemberRewardID"];

                MemberReward reward = LoyaltyDataService.GetMemberReward(rewardId);
                if (reward == null)
                {
                    throw new LWOperationInvocationException(string.Format("Unable to find member reward with id = {0}.", rewardId)) { ErrorCode = 3347 };
                }

                bool redeemed = reward.RedemptionDate != null ? true : false;

                APIArguments responseArgs = new APIArguments();
                responseArgs.Add("Redeemed", redeemed);
                response = SerializationUtils.SerializeResult(Name, Config, responseArgs);

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
