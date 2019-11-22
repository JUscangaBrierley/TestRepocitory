using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders
{
    public class GetReferenceData : OperationProviderBase
    {
        #region Fields
        private ServiceConfig _config = null;
        #endregion

        #region Construction
        public GetReferenceData() : base("GetReferenceData") { }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string response = string.Empty;
                LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
                _config = LWDataServiceUtil.GetServiceConfiguration(ctx.Organization, ctx.Environment);

                APIArguments apiArguments = SerializationUtils.DeserializeRequest(Name, Config, parms);
                APIArguments resultParams = new APIArguments();

                string refSetName = apiArguments["ReferenceSetName"].ToString();
                int? startIndex = apiArguments.ContainsKey("StartIndex") ? (int?)apiArguments["StartIndex"] : 0;
                int? batchSize = apiArguments.ContainsKey("BatchSize") ? (int?)apiArguments["BatchSize"] : int.MaxValue;

                string clientAssembly = DataServiceUtil.ClientsDataBaseName;

                Assembly assembly = Assembly.Load(clientAssembly);
                Type refType = assembly.GetType(clientAssembly + "." + refSetName);
                PropertyInfo[] refProperties = refType.GetProperties();

                using (var svc = new LoyaltyDataService(_config))
                {
                    LWCriterion lwCriteria = new LWCriterion(refSetName);
                    if (apiArguments.ContainsKey("ReferenceSetSearchParams"))
                    {
                        APIStruct[] searchParams = (APIStruct[])apiArguments["ReferenceSetSearchParams"];
                        if (searchParams != null)
                        {
                            foreach (APIStruct searchParam in searchParams)
                            {
                                if (searchParam.Parms.ContainsKey("SearchName") && searchParam.Parms.ContainsKey("SearchValue"))
                                {
                                    string searchName = searchParam.Parms["SearchName"].ToString();
                                    string searchValue = searchParam.Parms["SearchValue"].ToString();
                                    if (!string.IsNullOrEmpty(searchName))
                                        lwCriteria.Add(LWCriterion.OperatorType.AND, searchName, searchValue, LWCriterion.Predicate.Eq);
                                }
                            }
                        }
                    }
                    List<IClientDataObject> clientDataObjects = svc.GetAttributeSetObjects(null, refSetName, lwCriteria, new LWQueryBatchInfo() { StartIndex = startIndex.Value, BatchSize = batchSize.Value }, false);

                    if (clientDataObjects.Count <= 0)
                        throw new LWException(string.Format("Attribute set {0} returned no data.", refSetName));

                    Array dataArray = Array.CreateInstance(refType, clientDataObjects.Count);
                    //APIStruct[] dataArray = new APIStruct[clientDataObjects.Count];
                    int recordCount = 0;
                    foreach (IClientDataObject cObj in clientDataObjects)
                    {
                        bool processSet = true;
                        APIArguments rParms = new APIArguments();

                        if (cObj.GetMetaData().Type != AttributeSetType.Global && cObj.GetMetaData().ParentID == -1)
                            throw new LWException(string.Format("Attribute set {0} is not a Global attribute set", refSetName));

                        if (cObj.GetMetaData().ParentID != -1)
                        {
                            processSet = IsParentGlobal(refSetName);
                        }

                        if (!processSet)
                            throw new LWException(string.Format("Parent of attribute set {0} is not a Global attribute set", refSetName));

                        var obj = Activator.CreateInstance(refType);

                        foreach (PropertyInfo pInfo in refProperties)
                        {
                            Type type = obj.GetType();
                            PropertyInfo propertyInfo = type.GetProperty(pInfo.Name);
                            if (propertyInfo.GetSetMethod() != null)
                                propertyInfo.SetValue(obj, cObj.GetAttributeValue(pInfo.Name), null);

                            //rParms.Add(pInfo.Name, cObj.GetAttributeValue(pInfo.Name));
                        }

                        //APIStruct refDataStruct = new APIStruct() { Name = "ReferenceDataRecord", IsRequired = false, Parms = rParms };
                        //dataArray[recordCount++] = refDataStruct;
                        dataArray.SetValue(obj, recordCount++);
                    }
                    resultParams.Add("ReferenceDataRecords", dataArray);
                }
                
                response = SerializationUtils.SerializeResult(Name, Config, resultParams);
 
                return response;
            }
            catch (LWOperationInvocationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message, ex);
            }
        }

        private bool IsParentGlobal(string attSetName)
        {
            bool returnValue = false;
            long parentId = -1;
            long lookupId = -1;

            using (var svc = new LoyaltyDataService(_config))
            {
                parentId = svc.GetAttributeSetMetaData(attSetName).ParentID;
                while (parentId != -1)
                {
                    lookupId = parentId;
                    parentId = svc.GetAttributeSetMetaData(parentId).ParentID;
                }

                returnValue = svc.GetAttributeSetMetaData(lookupId).Type != AttributeSetType.Global ? false : true;
            }

            return returnValue;
        }

        protected override void Cleanup()
        {
        }
        #endregion
    }
}
