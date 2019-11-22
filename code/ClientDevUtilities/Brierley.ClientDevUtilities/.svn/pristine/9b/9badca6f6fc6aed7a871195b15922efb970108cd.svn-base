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
    public class GetRewardCategories : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetRewardCategories";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetRewardCategories() : base("GetRewardCategories") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {
            //string methodName = "Invoke";

            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for reward catalog count.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                long categoryId = args.ContainsKey("CategoryId") ? (long)args["CategoryId"] : 0;
                bool visibleInLN = args.ContainsKey("VisibleInLN") ? (bool)args["VisibleInLN"] : true;
                int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
                int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;
                
                LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

                List<Category> catList = ContentService.GetChildCategories(categoryId, visibleInLN, batchInfo);
                
                APIArguments responseArgs = new APIArguments();
                APIStruct[] cats = new APIStruct[catList.Count];
                int i = 0;
                foreach (Category cat in catList)
                {
                    APIArguments rparms = new APIArguments();
                    rparms.Add("CategoryId", cat.ID);
                    rparms.Add("CategoryName", cat.Name);
                    APIStruct catStruct = new APIStruct() { Name = "RewardCategory", IsRequired = true, Parms = rparms };
                    cats[i++] = catStruct;
                }

                responseArgs.Add("RewardCategory", cats);                
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
