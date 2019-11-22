using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace Brierley.LoyaltyWare.LWIntegrationSvc
{
    [DataContract]
    public class LWAPIResponse
    {
        [DataMember]
        public int ResponseCode { get; set; }

        [DataMember]
        public string ResponseDescription { get; set; }

        [DataMember]
        public string ResponseDetail { set; get; }

        [DataMember]
        public double ElapsedTime { set; get; }
    }
}
