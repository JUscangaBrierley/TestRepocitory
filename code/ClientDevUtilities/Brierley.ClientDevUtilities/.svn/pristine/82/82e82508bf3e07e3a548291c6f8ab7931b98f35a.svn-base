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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class GetRewardCatalogCount : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetRewardCatalogCount";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetRewardCatalogCount() : base("GetRewardCatalogCount") { }
        #endregion

        #region Overriden Methods
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
                long? categoryId = args.ContainsKey("CategoryId") ? (long?)args["CategoryId"] : null;
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

                int count = ContentService.HowManyRewardDefs(parmsList, categoryId);

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

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
