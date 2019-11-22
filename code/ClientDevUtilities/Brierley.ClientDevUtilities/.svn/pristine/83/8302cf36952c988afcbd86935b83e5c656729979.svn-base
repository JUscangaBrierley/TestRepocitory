using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class GetRewardCatalog : OperationProviderBase
    {
        public GetRewardCatalog() : base("GetRewardCatalog") { }

        public override string Invoke(string source, string parms)
        {
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for reward catalog count.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string language = args.ContainsKey("Language") ? (string)args["Language"] : LanguageChannelUtil.GetDefaultCulture();
                string channel = args.ContainsKey("Channel") ? (string)args["Channel"] : LanguageChannelUtil.GetDefaultChannel();
                long? categoryId = args.ContainsKey("CategoryId") ? (long?)args["CategoryId"] : null;
                int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
                int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;
                bool returnCategory = args.ContainsKey("ReturnRewardCategory") ? (bool)args["ReturnRewardCategory"] : false;

                LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);                

                List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();

                if (args.ContainsKey("ActiveOnly") && (bool)args["ActiveOnly"])
                {
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    entry.Add("Property", "Active");
                    entry.Add("Predicate", LWCriterion.Predicate.Eq);
                    entry.Add("Value", true);
                    parmsList.Add(entry);
                }
                if (args.ContainsKey("Tier"))
                {
                    TierDef tier = LoyaltyDataService.GetTierDef((string)args["Tier"]);
                    if (tier != null)
                    {
                        Dictionary<string, object> entry = new Dictionary<string, object>();
                        entry.Add("Property", "TierId");
                        entry.Add("Predicate", LWCriterion.Predicate.Eq);
                        entry.Add("Value", tier.Id);
                        parmsList.Add(entry);
                    }
                }
                if (args.ContainsKey("CurrencyToEarnLow"))
                {   
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    entry.Add("Property", "HowManyPointsToEarn");
                    entry.Add("Predicate", LWCriterion.Predicate.Ge);
                    entry.Add("Value", args["CurrencyToEarnLow"]);
                    parmsList.Add(entry);                    
                }
                if (args.ContainsKey("CurrencyToEarnHigh"))
                {   
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    entry.Add("Property", "HowManyPointsToEarn");
                    entry.Add("Predicate", LWCriterion.Predicate.Le);
                    entry.Add("Value", args["CurrencyToEarnHigh"]);
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
                        entry.Add("IsAttribute", true);
                        parmsList.Add(entry);
                    }
                }

                Dictionary<string, object> excludePayment = new Dictionary<string, object>();
                excludePayment.Add("Property", "RewardType");
                excludePayment.Add("Predicate", LWCriterion.Predicate.Ne);
                excludePayment.Add("Value", (int)RewardType.Payment);
                parmsList.Add(excludePayment);

                IList<RewardDef> rewards = ContentService.GetRewardDefsByProperty(parmsList, null, false, batchInfo, categoryId);

                APIArguments responseArgs = new APIArguments();
                APIStruct[] summary = new APIStruct[rewards.Count];
                int i = 0;
                foreach (RewardDef reward in rewards)
                {
                    APIArguments rparms = new APIArguments();
                    rparms.Add("RewardID", reward.Id);
                    rparms.Add("RewardName", reward.Name);
                    rparms.Add("DisplayName", reward.GetDisplayName(language, channel));
                    if (!string.IsNullOrEmpty(reward.CertificateTypeCode))
                    {
                        rparms.Add("TypeCode", reward.CertificateTypeCode);
                    }
                    if (!string.IsNullOrEmpty(reward.GetShortDescription(language, channel)))
                    {
                        rparms.Add("ShortDescription", reward.GetShortDescription(language, channel));
                    }
                    if (reward.CatalogStartDate != null)
                    {
                        rparms.Add("CatalogStartDate", reward.CatalogStartDate);
                    }
                    if (reward.CatalogEndDate != null)
                    {
                        rparms.Add("CatalogEndDate", reward.CatalogEndDate);
                    }
                    rparms.Add("CurrencyToEarn", reward.HowManyPointsToEarn);
                    rparms.Add("CurrencyType", reward.PointType);
                    rparms.Add("SmallImageFile", reward.SmallImageFile);
                    if (returnCategory)
                    {
                        Product p = reward.Product != null ? reward.Product : ContentService.GetProduct(reward.ProductId);
                        rparms.Add("CategoryId", p.CategoryId);
                    }
                    APIStruct rewardSummary = new APIStruct() { Name = "RewardCatalogSummary", IsRequired = false, Parms = rparms };
                    summary[i++] = rewardSummary;
                }

                responseArgs.Add("RewardCatalogSummary", summary);                
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
