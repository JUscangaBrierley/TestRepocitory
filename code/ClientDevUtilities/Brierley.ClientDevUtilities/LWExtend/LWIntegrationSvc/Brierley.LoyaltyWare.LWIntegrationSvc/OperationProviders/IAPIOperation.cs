using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders
{
    public interface IAPIOperation : IDisposable
    {
        void Initialize(string opName, LWIntegrationDirectives config, NameValueCollection functionProviderParms);
        string Invoke(string source, string parms);
    }
}
