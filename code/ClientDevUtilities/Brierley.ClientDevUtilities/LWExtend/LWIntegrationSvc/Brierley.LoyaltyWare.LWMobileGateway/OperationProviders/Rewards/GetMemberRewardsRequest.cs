using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data;

namespace Brierley.ClientDevUtilities.LWExtend.LoyaltyWare.LWMobileGateway.OperationProviders.Rewards
{
    public class GetMemberRewardsRequest
    {
        public long? CategoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool UnRedeemedOnly { get; set; }
        public bool UnexpiredOnly { get; set; }
        public string Language { get; set; }
        public string Channel { get; set; }
        public int? StartIndex { get; set; }
        public int? BatchSize { get; set; }
    }
}
