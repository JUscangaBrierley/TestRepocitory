using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Brierley.LoyaltyWare.LWIntegrationSvc
{
    [ServiceContract(Namespace="urn:Brierley.LoyaltyWare.LWIntegrationSvc")]
    public interface ILWIntegrationService
    {
		[OperationContract]
		LWAPIResponse Execute(string operationName, string source, string sourceEnv, string externalId, string payload, List<NewRelicAttribute> newRelicAttributes);
	}
    
}
