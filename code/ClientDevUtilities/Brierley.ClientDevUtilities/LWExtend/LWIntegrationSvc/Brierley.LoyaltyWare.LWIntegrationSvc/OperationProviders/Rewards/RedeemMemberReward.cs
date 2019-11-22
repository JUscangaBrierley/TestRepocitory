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
    public class RedeemMemberReward : OperationProviderBase
    {
        #region Fields
        //private const string _className = "RedeemMemberReward";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public RedeemMemberReward() : base("RedeemMemberReward") { }
        #endregion

        #region Private Helpers
        private bool NullOutRedeemDate()
        {
            string value = FunctionProviderParms["NullRedeemDateIfNotProvided"];
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

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            /*
             * A lot of thsi logic needs to be synchronized with IssueReward rule.
             * */
            try
            {
                //string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for update member reward.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                long rewardId = (long)args["MemberRewardID"];

                MemberReward reward = LoyaltyDataService.GetMemberReward(rewardId);
                if (reward == null)
                {
                    throw new LWOperationInvocationException(string.Format("Unable to find member reward with id = {0}.", rewardId)) { ErrorCode = 3347 };
                }

                if (args.ContainsKey("AvailableBalance"))
                {
                    reward.AvailableBalance = (decimal)args["AvailableBalance"];
                }

                if (args.ContainsKey("ExpirationDate"))
                {
                    reward.Expiration = (DateTime)args["ExpirationDate"];
                }

                if (args.ContainsKey("RedemptionDate"))
                {
                    reward.RedemptionDate = (DateTime)args["RedemptionDate"];
                }
                else if (NullOutRedeemDate())
                {
                    reward.RedemptionDate = null;
                }

                LoyaltyDataService.UpdateMemberReward(reward);

                #region Post Processing
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("memberId", reward.MemberId); 
                context.Add("reward", reward);                
                PostProcessSuccessfullInvocation(context);
                #endregion

                return string.Empty;
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
