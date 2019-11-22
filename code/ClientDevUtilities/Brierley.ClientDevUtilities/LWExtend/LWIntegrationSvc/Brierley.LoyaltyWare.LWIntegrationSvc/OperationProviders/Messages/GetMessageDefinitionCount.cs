using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Messages
{
    public class GetMessageDefinitionCount : OperationProviderBase
    {
        public GetMessageDefinitionCount() : base("GetMessageDefinitionCount") { }

        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;

                List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
                
                if (!string.IsNullOrEmpty(parms))
                {
                    APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                    if (args.ContainsKey("ActiveOnly") && (bool)args["ActiveOnly"])
                    {
                        Dictionary<string, object> entry = new Dictionary<string, object>();
                        entry.Add("Property", "Active");
                        entry.Add("Predicate", LWCriterion.Predicate.Eq);
                        entry.Add("Value", true);
                        parmsList.Add(entry);
                    }
                    if (args.ContainsKey("ContentSearchAttributes"))
                    {
                        APIStruct[] attList = (APIStruct[])args["ContentSearchAttributes"];
                        foreach (APIStruct att in attList)
                        {
                            Dictionary<string, object> entry = new Dictionary<string, object>();
                            entry.Add("Property", att.Parms["AttributeName"]);
                            entry.Add("Predicate", LWCriterion.Predicate.Eq);
                            entry.Add("Value", att.Parms["AttributeValue"]);
                            if (att.Parms["AttributeName"].ToString() != "Name")
                            {
                                entry.Add("IsAttribute", true);
                            }
                            parmsList.Add(entry);
                        }
                    }                    
                }
                                                
                int count = ContentService.HowManyMessageDefs(parmsList);

                APIArguments responseArgs = new APIArguments();
                responseArgs.Add("Count", count);
                response = SerializationUtils.SerializeResult(Name, Config, responseArgs);

                return response;
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }
    }
}
