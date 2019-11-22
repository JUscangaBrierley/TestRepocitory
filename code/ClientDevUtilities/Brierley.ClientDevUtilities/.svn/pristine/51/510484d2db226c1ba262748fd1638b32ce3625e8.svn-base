using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data.DomainModel
{
    public class RewardOrderItem
    {
        public RewardOrderItem()
        {
            PointsConsumptionPolicy = PointsConsumptionOnIssueReward.Consume;            
        }
        public string RewardName { get; set; }
        public string TypeCode { get; set; }
        public RuleTrigger Rule { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string CertificateNumber { get; set; }
        public string PartNumber { get; set; }
        public RewardDef RewardDefinition { get; set; }
        public RewardFulfillmentOption FulfillmentOption { get; set; }
        public long? ProviderId { get; set; }
        public IOrderFulfillmentProvider Provider { get; set; }
        public PointsConsumptionOnIssueReward PointsConsumptionPolicy { get; set; }
        public string FPOrderNumber { get; set; }
        public long MemberRewardId { get; set; }        
    }
}
