using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Brierley.FrameWork;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders
{
    public interface IAPIOperation : IDisposable
    {
        void Initialize(string opName, MobileGatewayDirectives config, NameValueCollection functionProviderParms);
        object Invoke(string source, WcfAuthenticationToken token, object[] parms);
    }
}
