using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
    public class GetMember : OperationProviderBase
    {
        public GetMember() : base("GetMember") { }

        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            Member member = LoadMemberAttributeSets(Config.GetOperationDirectiveByName(Name), token.CachedMember);
            return MGMember.Hydrate(member);
        }
    }
}