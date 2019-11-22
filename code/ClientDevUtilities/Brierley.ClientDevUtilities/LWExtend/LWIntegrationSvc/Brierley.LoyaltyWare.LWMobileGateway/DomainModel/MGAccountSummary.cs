using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Brierley.FrameWork.Common;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGAccountSummary
    {
        [DataMember]
        public virtual decimal CurrencyBalance { get; set; }
        [DataMember]
        public virtual DateTime MemberAddDate { get; set; }
        [DataMember]
        public virtual string CurrentTierName { get; set; }
        [DataMember]
        public virtual DateTime CurrentTierExpirationDate { get; set; }
        [DataMember]
        public virtual decimal CurrencyToNextTier { get; set; }
        [DataMember]
        public virtual decimal CurrencyToNextReward { get; set; }
        [DataMember]
        public virtual DateTime? LastActivityDate { get; set; }        
        [DataMember]
        public virtual decimal PointsToRewardChoice { get; set; }

		public MGAccountSummary()
		{
			CurrentTierExpirationDate = DateTimeUtil.MinValue;
		}
    }
}