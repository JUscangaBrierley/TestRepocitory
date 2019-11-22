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
    public class CancelMemberRewardOrder : MemberRewardsBase
    {
        #region Fields
        //private const string _className = "CancelMemberRewardOrder";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public CancelMemberRewardOrder() : base("CancelMemberRewardOrder") { }
        #endregion

        #region Overriden Methods        
        public override string Invoke(string source, string parms)
        {
            //string methodName = "Invoke";
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
                string orderNumber = (string)args["OrderNumber"];
                MemberOrder order = LoyaltyDataService.GetMemberOrder(orderNumber);
                if (order == null)
                {
                    throw new LWOperationInvocationException("Order number not found.") { ErrorCode = 3355 };
                }

                string msg = string.Format("Member order {0} has been cancelled.", order.OrderNumber);
                MemberRewardsUtil.CancelOrReturnMemberOrder(order, true, msg);
                Member member = LoyaltyDataService.LoadMemberFromIPCode(order.MemberId);                
                APIArguments resultParams = new APIArguments();
                resultParams.Add("CurrencyBalance", member.GetPoints(null, null, DateTimeUtil.MinValue, DateTimeUtil.MaxValue).ToString());
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);

                #region Post Processing
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("member", member);
                context.Add("order", order);
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
