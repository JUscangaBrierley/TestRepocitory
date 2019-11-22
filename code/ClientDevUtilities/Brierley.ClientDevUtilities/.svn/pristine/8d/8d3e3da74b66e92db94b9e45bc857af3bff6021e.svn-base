using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
    public class GetAccountActivityDetails : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetAccountActivityDetails";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        #region Private Helpers
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

        public GetAccountActivityDetails() : base("GetAccountActivityDetails") { }


        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            //string methodName = "Invoke";
            
            Member member = token.CachedMember;

            string txnHeaderId = parms[0] as string;            
            string getPointsHistoryStr = parms[1] as string;
            bool getPointsHistory = false;
            if (!string.IsNullOrEmpty(getPointsHistoryStr))
            {
                getPointsHistory = bool.Parse(getPointsHistoryStr);
            }

            bool retrieveExpiredTransactions = false;
            if (!string.IsNullOrEmpty(GetFunctionParameter("RetrieveExpiredTransactions")))
            {
                retrieveExpiredTransactions = bool.Parse(GetFunctionParameter("RetrieveExpiredTransactions"));
            }

            List<MGAccountActivityDetails> details = new List<MGAccountActivityDetails>();
            IList<IClientDataObject> txnDetails = AccountActivityUtil.GetAccountActivityDetail(txnHeaderId, getPointsHistory, retrieveExpiredTransactions);
            foreach (IClientDataObject dobj in txnDetails)
            {
                MGAccountActivityDetails detail = new MGAccountActivityDetails();

                object quantity = dobj.GetAttributeValue("DtlQuantity");
                if (quantity != null)
                {
                    detail.Quantity = int.Parse(quantity.ToString());                    
                }

                object saleAmount = dobj.GetAttributeValue("DtlSaleAmount");
                if (saleAmount != null)
                {
                    detail.SaleAmount = decimal.Parse(saleAmount.ToString());
                }
                if (dobj.HasTransientProperty("Product"))
                {
                    Product product = (Product)dobj.GetTransientProperty("Product");
                    detail.ItemDescription = product.ShortDescription;
                    detail.ClassDescription = product.ClassDescription;
                    detail.ClassCode = product.ClassCode;                    
                }
                if (getPointsHistory)
                {
                    IList<PointTransaction> detailTxnList = (IList<PointTransaction>)dobj.GetTransientProperty("PointsHistory");
                    if (detailTxnList != null && detailTxnList.Count > 0)
                    {
                        detail.PointsHistory = new List<MGPointsHistory>();                        
                        foreach (PointTransaction txn in detailTxnList)
                        {
                            detail.PointsHistory.Add(MGPointsHistory.Hydrate(txn));                            
                        }                        
                    }
                }

                AttributeSetMetaData attSetMeta = LoyaltyService.GetAttributeSetMetaData("TxnDetailItem");
                string[] additionalAttributes = GetAdditionalAttributesList();
                if (additionalAttributes != null && additionalAttributes.Length > 0)
                {
                    detail.AdditionalAttributes = new List<MGClientEntityAttribute>();
                    Dictionary<string, object> attValues = new Dictionary<string, object>();
                    foreach (string attName in additionalAttributes)
                    {
                        object attValue = dobj.GetAttributeValue(attName);
                        if (attValue != null && !string.IsNullOrEmpty(attValue.ToString()))
                        {
                            AttributeMetaData attMeta = attSetMeta.GetAttribute(attName);
                            MGClientEntityAttribute att = new MGClientEntityAttribute()
                            {
                                Name = attName,
                                Value = attValue,
                                DataType = (DataType)Enum.Parse(typeof(DataType), attMeta.DataType)
                            };
                            detail.AdditionalAttributes.Add(att);
                        }
                    }                    
                }
				details.Add(detail);
            }

            return details;
        }
    }
}