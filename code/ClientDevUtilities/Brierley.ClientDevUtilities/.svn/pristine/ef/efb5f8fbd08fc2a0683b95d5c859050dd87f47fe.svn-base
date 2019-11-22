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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Promotions
{
    public class GetPromotionDefinitions : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetPromotionDefinitions";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetPromotionDefinitions() : base("GetPromotionDefinitions") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {
            //string methodName = "Invoke";

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
                            if (att.Parms["AttributeName"].ToString() != "Name" && att.Parms["AttributeName"].ToString() != "Code")
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

                LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

                IList<Promotion> promotions = ContentService.GetPromotions(parmsList, returnAttributes, batchInfo);

                //IList<long> idList = Service.GetPromotionIds(parmsList, string.Empty, true);
                //if (idList == null || idList.Count == 0)
                //{
                //    throw new LWOperationInvocationException("No content available that matches the specified criteria.") { ErrorCode = 3362 };
                //}

                //long[] ids = LWQueryBatchInfo.GetIds(idList.ToArray<long>(), batchInfo.StartIndex, batchInfo.BatchSize, Config.EnforceValidBatch);

                //IList<Promotion> promotions = Service.GetPromotionIds(ids);
                if (promotions.Count == 0)
                {
                    throw new LWOperationInvocationException("No content available that matches the specified criteria.") { ErrorCode = 3362 };
                }

                if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
                {
                    throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
                }
                if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
                {
                    throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
                }

                APIArguments responseArgs = new APIArguments();
                APIStruct[] promoList = new APIStruct[promotions.Count];
                int i = 0;
                foreach (Promotion promotion in promotions)
                {
                    promoList[i++] = PromotionUtil.SerializePromotionDefinition(language, channel, promotion, returnAttributes);                    
                }

                responseArgs.Add("PromotionDefinition", promoList);                
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

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
