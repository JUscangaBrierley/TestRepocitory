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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Bonuses
{
    public class GetBonusDefinitions : OperationProviderBase
    {
        public GetBonusDefinitions() : base("GetBonusDefinitions") { }

        public override string Invoke(string source, string parms)
        {
            try
            {
                string response = string.Empty;

                string language = LanguageChannelUtil.GetDefaultCulture();
                string channel = LanguageChannelUtil.GetDefaultChannel();
                List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
                int? startIndex = null;
                int? batchSize = null;
                bool returnAttributes = false;

                if (!string.IsNullOrEmpty(parms))
                {
                    APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                    if (args.ContainsKey("Language"))
                    {
                        language = (string)args["Language"];
                    }
                    if (args.ContainsKey("Channel"))
                    {
                        channel = (string)args["Channel"];
                    }
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
                    if (args.ContainsKey("ReturnAttributes"))
                    {
                        returnAttributes = (bool)args["ReturnAttributes"];
                    }
                    if (args.ContainsKey("StartIndex"))
                    {
                        startIndex = (int)args["StartIndex"];
                    }
                    if (args.ContainsKey("BatchSize"))
                    {
                        batchSize = (int)args["BatchSize"];
                    }                                        
                }

                // validate language and channel
                if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
                {
                    throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
                }
                if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
                {
                    throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
                }

                LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

                IList<BonusDef> bonuses = ContentService.GetBonusDefs(parmsList, returnAttributes, batchInfo);
                if (bonuses.Count == 0)
                {
                    throw new LWOperationInvocationException("No content available that matches the specified criteria.") { ErrorCode = 3362 };
                }

                APIArguments responseArgs = new APIArguments();
                APIStruct[] msgList = new APIStruct[bonuses.Count];
                int i = 0;
                foreach (BonusDef bonus in bonuses)
                {                    
                    msgList[i++] = BonusUtil.SerializeBonusDef(language, channel, returnAttributes, bonus);                    
                }

                responseArgs.Add("BonusDefinition", msgList);                
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
