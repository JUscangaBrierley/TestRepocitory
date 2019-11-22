using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.Authorization
{
    public interface IAuthorizationInterceptor : ILWInterceptor
    {
        WcfAuthenticationToken CheckAuthorization(string clientId);        
    }
}