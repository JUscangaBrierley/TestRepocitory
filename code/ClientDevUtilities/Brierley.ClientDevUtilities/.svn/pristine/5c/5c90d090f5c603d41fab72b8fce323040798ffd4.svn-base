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
    public class GetPrimaryLoyaltyCard : OperationProviderBase
    {
        public GetPrimaryLoyaltyCard() : base("GetPrimaryLoyaltyCard") { }

        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            Member m = LoadMemberAttributeSets(Config.GetOperationDirectiveByName(Name), token.CachedMember);
			VirtualCard primaryCard = m.GetLoyaltyCardByType(FrameWork.Common.VirtualCardSearchType.PrimaryCard);
			if (primaryCard != null)
			{
				return MGLoyaltyCard.Hydrate(primaryCard);
			}
			return null;
            
        }
    }
}