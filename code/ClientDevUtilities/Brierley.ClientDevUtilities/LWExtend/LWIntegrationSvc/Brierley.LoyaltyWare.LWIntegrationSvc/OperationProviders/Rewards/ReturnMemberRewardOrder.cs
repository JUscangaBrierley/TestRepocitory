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
    public class ReturnMemberRewardOrder : MemberRewardsBase
    {
        #region Fields
        private const string _className = "ReturnMemberRewardOrder";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public ReturnMemberRewardOrder() : base("ReturnMemberRewardOrder") { }
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
                    throw new LWOperationInvocationException("No parameters provided for returning member reward.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string orderNumber = (string)args["OrderNumber"];
                bool isFPOrder = args.ContainsKey("IsFPOrder") ? (bool)args["IsFPOrder"] : false;
                //string partNumber = args.ContainsKey("PartNumber") ? (string)args["PartNumber"] : string.Empty;

                string lwOrderNumber = orderNumber;
                if (isFPOrder)
                {
                    IList<MemberReward> mrList = LoyaltyDataService.GetMemberRewardsByFPOrderNumber(orderNumber);
                    if (mrList.Count == 0)
                    {
                        throw new LWOperationInvocationException("No rewards found for FP order number " + orderNumber + ".") { ErrorCode = 3355 };
                    }
                    lwOrderNumber = mrList[0].LWOrderNumber;
                }

                MemberOrder order = LoyaltyDataService.GetMemberOrder(lwOrderNumber);
                if (order == null)
                {
                    throw new LWOperationInvocationException("Order number not found.") { ErrorCode = 3355 };
                }

                //if (!string.IsNullOrEmpty(partNumber))
                //{
                //    _logger.Trace(_className, methodName,
                //        string.Format("Returning reward with order {0} and part number {1}.", orderNumber, partNumber));                    
                //}
                //else
                //{
                    _logger.Trace(_className, methodName,
                       string.Format("Returning order {0}.", orderNumber));
                    string msg = string.Format("Member order {0} has been returned.", order.OrderNumber);
                    MemberRewardsUtil.CancelOrReturnMemberOrder(order, false, msg);
                //}

                #region Post Processing
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("memberId", order.MemberId);
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
