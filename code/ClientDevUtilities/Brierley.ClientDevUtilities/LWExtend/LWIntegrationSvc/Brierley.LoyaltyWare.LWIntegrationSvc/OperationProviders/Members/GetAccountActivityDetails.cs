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
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class GetAccountActivityDetails : OperationProviderBase
    {
        #region Fields
        //private const string _className = "GetAccountActivityDetails";
        //private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        #endregion

        #region Construction
        public GetAccountActivityDetails() : base("GetAccountActivityDetails") { }
        #endregion

        #region Helpers
        private string[] GetAdditionalAttributesList()
        {
            Dictionary<string, string> preDefined = new Dictionary<string, string>()
            {
                {"DtlQuantity", "DtlQuantity"},
                {"DtlSaleAmount", "DtlSaleAmount"}                
            };
            string addAttParms = GetFunctionParameter("ExtendedHeaderFields");
            string[] addAttList = null;
            if (!string.IsNullOrEmpty(addAttParms))
            {
                List<string> validList = new List<string>();
                string[] tokens = addAttParms.Split(';');
                foreach (string token in tokens)
                {
                    if (!preDefined.ContainsKey(token))
                    {
                        validList.Add(token);
                    }
                }
                if (validList.Count > 0)
                {
                    addAttList = validList.ToArray<string>();
                }
            }
            return addAttList;
        }
        #endregion

        #region Overriden Methods
        public override string Invoke(string source, string parms)
        {            
            try
            {
                string[] additionalAttributes = GetAdditionalAttributesList();
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for reward catalog item.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string txnHeaderId = (string)args["TxnHeaderId"];
                bool getPointsHistory = args.ContainsKey("GetPointsHistory") ? (bool)args["GetPointsHistory"] : false;

                bool retrieveExpiredTransactions = false;
                if (!string.IsNullOrEmpty(GetFunctionParameter("RetrieveExpiredTransactions")))
                {
                    retrieveExpiredTransactions = bool.Parse(GetFunctionParameter("RetrieveExpiredTransactions"));
                }

                IList<IClientDataObject> txnDetails = AccountActivityUtil.GetAccountActivityDetail(txnHeaderId, getPointsHistory, retrieveExpiredTransactions);
                APIArguments responseArgs = new APIArguments();
                APIStruct[] summary = new APIStruct[txnDetails.Count];
                int i = 0;
                foreach (IClientDataObject dobj in txnDetails)
                {
                    APIArguments rparms = new APIArguments();
                    if (dobj.GetAttributeValue("DtlQuantity") != null)
                    {
                        rparms.Add("Quantity", dobj.GetAttributeValue("DtlQuantity"));
                    }
                    rparms.Add("SaleAmount", dobj.GetAttributeValue("DtlSaleAmount"));
                    if (dobj.HasTransientProperty("Product"))
                    {
                        Product product = (Product)dobj.GetTransientProperty("Product");
						rparms.Add("ItemDescription", product.ShortDescription);
                        rparms.Add("ClassDescription", product.ClassDescription);
                        rparms.Add("ClassCode", product.ClassCode);
                    }
                    if (getPointsHistory)
                    {
                        IList<PointTransaction> detailTxnList = (IList<PointTransaction>)dobj.GetTransientProperty("PointsHistory");
                        if (detailTxnList != null && detailTxnList.Count > 0)
                        {
                            APIStruct[] txnPoints = new APIStruct[detailTxnList.Count];
                            int idx1 = 0;
                            foreach (PointTransaction txn in detailTxnList)
                            {
                                txnPoints[idx1++] = PointTransactionHelper.SerializePointTransaction(txn,"PointsHistory");                                
                            }
                            rparms.Add("PointsHistory", txnPoints);
                        }                        
                    }
                    // handle additional attributes
                    if (additionalAttributes != null && additionalAttributes.Length > 0)
                    {
                        Dictionary<string, object> attValues = new Dictionary<string, object>();
                        foreach (string attName in additionalAttributes)
                        {
                            object attValue = dobj.GetAttributeValue(attName);
                            if (attValue != null && !string.IsNullOrEmpty(attValue.ToString()))
                            {
                                attValues.Add(attName, attValue);
                            }
                        }
                        if (attValues.Count > 0)
                        {
                            APIStruct[] addAtts = new APIStruct[attValues.Count];
                            int x = 0;
                            foreach (string key in attValues.Keys)
                            {
                                APIArguments aparms = new APIArguments();
                                aparms.Add("AttributeName", key);
                                aparms.Add("AttributeValue", attValues[key]);
                                aparms.Add(key, attValues[key]);
                                APIStruct addAtt = new APIStruct() { Name = "AdditionalAttributes", IsRequired = false, Parms = aparms };
                                addAtts[x++] = addAtt;
                            }
                            rparms.Add("AdditionalAttributes", addAtts);
                        }
                    }
                    APIStruct activitySummary = new APIStruct() { Name = "AccountActivityDetails", IsRequired = false, Parms = rparms };
                    summary[i++] = activitySummary;
                }

                responseArgs.Add("AccountActivityDetails", summary);
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
