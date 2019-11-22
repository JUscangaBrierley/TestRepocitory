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
    public class GetLoyaltyCards : OperationProviderBase
    {
        public GetLoyaltyCards() : base("GetLoyaltyCards") { }

        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            Member m = LoadMemberAttributeSets(Config.GetOperationDirectiveByName(Name), token.CachedMember);
            List<MGLoyaltyCard> cards = new List<MGLoyaltyCard>();
            foreach (VirtualCard card in m.LoyaltyCards)
            {
                MGLoyaltyCard c = MGLoyaltyCard.Hydrate(card);
                cards.Add(c);
            }
            return cards;
        }
    }
}