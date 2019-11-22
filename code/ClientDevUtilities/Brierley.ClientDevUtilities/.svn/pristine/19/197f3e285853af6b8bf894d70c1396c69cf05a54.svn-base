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
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class GetMemberRewardsByOrderNumber : MemberRewardsBase
    {
        #region Fields
        private const string _className = "GetMemberRewardsByOrderNumber";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetMemberRewardsByOrderNumber() : base("GetMemberRewardsByOrderNumber") { }
        #endregion

        #region Overriden Methods
        
        public override string Invoke(string source, string parms)
        {
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {                    
                    throw new LWOperationInvocationException("No parameters provided to retrieve member rewards.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string orderNumber = (string)args["OrderNumber"];
                bool isFPOrder = args.ContainsKey("IsFPOrder") ? (bool)args["IsFPOrder"] : false;                
                MemberOrder order = null;
                Dictionary<long,MemberReward> rewardMap = null;

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

                if (!MemberRewardsUtil.GetMemberRewards(lwOrderNumber, out order, out rewardMap))                
                {
                    throw new LWOperationInvocationException("Order number not found.") { ErrorCode = 3355 };
                }
                if (order.OrderItems.Count == 0)
                {
                    throw new LWOperationInvocationException("No member rewards found.") { ErrorCode = 3362 };
                }
                
                APIArguments orderParams = new APIArguments();
                orderParams.Add("OrderNumber", order.OrderNumber);
                if (!string.IsNullOrEmpty(order.Source))
                {
                    orderParams.Add("Source", order.Source);
                }
                if (!string.IsNullOrEmpty(order.Channel))
                {
                    orderParams.Add("Channel", order.Channel);
                }
                if (!string.IsNullOrEmpty(order.ChangedBy))
                {
                    orderParams.Add("ChangedBy", order.ChangedBy);
                }
                if (!string.IsNullOrEmpty(order.FirstName))
                {
                    orderParams.Add("FirstName", order.FirstName);
                }
                if (!string.IsNullOrEmpty(order.LastName))
                {
                    orderParams.Add("LastName", order.LastName);
                }
                if (!string.IsNullOrEmpty(order.EmailAddress))
                {
                    orderParams.Add("EmailAddress", order.EmailAddress);
                }
                if (order.ShippingAddress != null)
                {
                    if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineOne))
                    {
                        orderParams.Add("AddressLineOne", order.ShippingAddress.AddressLineOne);
                    }
                    if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineTwo))
                    {
                        orderParams.Add("AddressLineTwo", order.ShippingAddress.AddressLineTwo);
                    }
                    if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineThree))
                    {
                        orderParams.Add("AddressLineThree", order.ShippingAddress.AddressLineThree);
                    }
                    if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLineFour))
                    {
                        orderParams.Add("AddressLineFour", order.ShippingAddress.AddressLineFour);
                    }
                    if (!string.IsNullOrEmpty(order.ShippingAddress.City))
                    {
                        orderParams.Add("City", order.ShippingAddress.City);
                    }
                    if (!string.IsNullOrEmpty(order.ShippingAddress.StateOrProvince))
                    {
                        orderParams.Add("StateOrProvince", order.ShippingAddress.StateOrProvince);
                    }
                    if (!string.IsNullOrEmpty(order.ShippingAddress.ZipOrPostalCode))
                    {
                        orderParams.Add("ZipOrPostalCode", order.ShippingAddress.ZipOrPostalCode);
                    }
                    if (!string.IsNullOrEmpty(order.ShippingAddress.County))
                    {
                        orderParams.Add("County", order.ShippingAddress.County);
                    }
                    if (!string.IsNullOrEmpty(order.ShippingAddress.Country))
                    {
                        orderParams.Add("Country", order.ShippingAddress.Country);
                    }
                }                
                APIStruct[] memberRewards = new APIStruct[order.OrderItems.Count];
                int rewardIdx = 0;
                foreach (RewardOrderItem item in order.OrderItems)
                {
                    MemberReward reward = rewardMap[item.MemberRewardId];
                    APIArguments rewardParams = new APIArguments();
                    rewardParams.Add("MemberRewardID", reward.Id);
                    rewardParams.Add("RewardDefID", reward.RewardDefId);
                    if (!string.IsNullOrEmpty(reward.FPOrderNumber))
                    {
                        rewardParams.Add("FPOrderNumber", reward.FPOrderNumber);
                    }
                    if (!string.IsNullOrEmpty(reward.CertificateNmbr))
                    {
                        rewardParams.Add("CertificateNumber", reward.CertificateNmbr);
                    }
                    rewardParams.Add("AvailableBalance", reward.AvailableBalance);
                    rewardParams.Add("DateIssued", reward.DateIssued);
                    if (reward.Expiration != null)
                    {
                        rewardParams.Add("ExpirationDate", reward.Expiration);
                    }
                    if (reward.FulfillmentDate != null)
                    {
                        rewardParams.Add("FulfillmentDate", reward.FulfillmentDate);
                    }
                    if (reward.RedemptionDate != null)
                    {
                        rewardParams.Add("RedemptionDate", reward.RedemptionDate);
                    }
                    if (reward.OrderStatus != null)
                    {
                        rewardParams.Add("OrderStatus", Enum.GetName(typeof(RewardOrderStatus), reward.OrderStatus));
                    }
                    if (!string.IsNullOrEmpty(reward.TrackingUrl))
                    {
                        rewardParams.Add("TrackingUrl", reward.TrackingUrl);
                    }
                    if (!string.IsNullOrEmpty(reward.TrackingNumber))
                    {
                        rewardParams.Add("TrackingNumber", reward.TrackingNumber);
                    }                    
                    APIStruct rv = new APIStruct() { Name = "MemberRewardInfo", IsRequired = false, Parms = rewardParams };
                    memberRewards[rewardIdx++] = rv;
                }
                orderParams.Add("MemberRewardInfo", memberRewards);
                APIStruct v = new APIStruct() { Name = "MemberRewardOrder", IsRequired = false, Parms = orderParams };
                APIArguments resultParams = new APIArguments();
                resultParams.Add("MemberRewardOrder", v);
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);
                return response;
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(_className, "Invoke", ex.Message, ex);
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
