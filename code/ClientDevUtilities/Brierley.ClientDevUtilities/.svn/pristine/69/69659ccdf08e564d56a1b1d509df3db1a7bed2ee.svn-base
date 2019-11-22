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

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.ProgramInfo
{
    public class GetCategories : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetRewardCategories";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetCategories() : base("GetCategories") { }
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
                    throw new LWOperationInvocationException("No parameters provided for categories.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                List<Category> catList = null;
                string[] searchOptionTypes = args.ContainsKey("SearchType") ? (string[])args["SearchType"] : new string[0];
                string[] searchValues = args.ContainsKey("SearchValue") ? (string[])args["SearchValue"] : new string[0];
                bool visibleInLN = args.ContainsKey("VisibleInLN") ? (bool)args["VisibleInLN"] : true;
                int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
                int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;
                
                LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

                if (searchOptionTypes.Length != searchValues.Length)
                    throw new LWOperationInvocationException("The number of search values provided do not match the number of search options.") { ErrorCode = 3348 };

                if (searchOptionTypes.Length == 0 && searchValues.Length == 0)
                {
                    catList = ContentService.GetCategories(visibleInLN);
                }

                for (int i = 0; i < searchOptionTypes.Length; i++)
                {
                    if (catList != null && catList.Count > 0)
                    {
                        break;
                    }
                    string searchType = searchOptionTypes[i];
                    Category cat = null;
                    if (string.IsNullOrEmpty(searchType))
                    {
                        throw new LWOperationInvocationException("No search type provided for categories.") { ErrorCode = 3317 };
                    }
                    switch (searchType)
                    {
                        case "CategoryId":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No category id provided for category search.") { ErrorCode = 3301 };
                            }
                            cat = ContentService.GetCategory(long.Parse(searchValues[i]));
                            if (cat != null)
                            {
                                catList = new List<Category>();
                                catList.Add(cat);
                            }
                            break;
                        case "ParentCategoryId":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No parent category id provided for category search.") { ErrorCode = 3304 };
                            }
                            catList = ContentService.GetChildCategories(long.Parse(searchValues[i]), visibleInLN, batchInfo);
                            break;
                        case "CategoryType":
                            if (string.IsNullOrEmpty(searchValues[i]))
                            {
                                throw new LWOperationInvocationException("No category type provided for category search.") { ErrorCode = 3318 };
                            }
                            CategoryType categoryType = CategoryType.ClientConfigurationFolder;
                            Enum.TryParse(searchValues[i], true, out categoryType);
                            if (categoryType != CategoryType.ClientConfigurationFolder)
                                catList = ContentService.GetCategoriesByType(categoryType, visibleInLN);
                            break;
                        
                    }
                }
                if (catList == null || catList.Count == 0)
                {
                    throw new LWOperationInvocationException(string.Format("No categories found.")) { ErrorCode = 3323 };
                }
                else
                {
                    APIArguments responseArgs = new APIArguments();
                    APIStruct[] cats = new APIStruct[catList.Count];
                    int i = 0;
                    foreach (Category cat in catList)
                    {
                        APIArguments rparms = new APIArguments();
                        rparms.Add("CategoryId", cat.ID);
                        rparms.Add("ParentCategoryId", cat.ParentCategoryID);
                        rparms.Add("CategoryType", cat.CategoryType);
                        rparms.Add("CategoryName", cat.Name);
                        APIStruct catStruct = new APIStruct() { Name = "Category", IsRequired = true, Parms = rparms };
                        cats[i++] = catStruct;
                    }

                    responseArgs.Add("Category", cats);
                    response = SerializationUtils.SerializeResult(Name, Config, responseArgs);                
                }                             
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
