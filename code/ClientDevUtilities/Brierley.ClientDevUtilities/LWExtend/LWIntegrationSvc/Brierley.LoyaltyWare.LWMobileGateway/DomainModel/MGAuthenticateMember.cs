using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Brierley.FrameWork.Common;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
	[DataContract]
    public class MGAuthenticateMember
    {
        [DataMember(Name = "token")]
        public virtual string Token { get; set; }
        
		[DataMember(Name = "statusText")]
        public virtual string StatusText { get; set; }

        [DataMember(Name = "loginStatus")]
        public virtual LoginStatusEnum LoginStatus { get; set; }
    }
}