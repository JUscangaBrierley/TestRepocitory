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
    public class GetAccountActivitySummary : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetAccountActivitySummary";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        #region Private Helpers
        private string[] GetAdditionalAttributesList()
        {
            Dictionary<string, string> preDefined = new Dictionary<string, string>()
            {
                {"TxnHeaderId", "TxnHeaderId"},
                {"TxnDate", "TxnDate"},
                {"TxnNumber", "TxnNumber"},
                {"TxnRegisterNumber", "TxnRegisterNumber"},
                {"TxnStoreId", "TxnStoreId"},
                {"TxnAmount", "TxnAmount"},
                {"TxnChannel","TxnChannel"}
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

        public GetAccountActivitySummary() : base("GetAccountActivitySummary") { }


        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";
            
            Member member = token.CachedMember;

            string startDateStr = parms[0] as string;
            DateTime? startDate = null;
            if (!string.IsNullOrEmpty(startDateStr))
            {
                startDate = DateTime.Parse(startDateStr);
				if (startDate < DateTimeUtil.MinValue)
				{
					startDate = DateTimeUtil.MinValue;
				}
            }

            string endDateStr = parms[1] as string;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endDateStr))
            {
                endDate = DateTime.Parse(endDateStr);
				if (endDate > DateTimeUtil.MaxValue)
				{
					endDate = DateTimeUtil.MaxValue;
				}
            }

            string getPointsHistoryStr = parms[2] as string;
            bool getPointsHistory = false;
            if (!string.IsNullOrEmpty(getPointsHistoryStr))
            {
                getPointsHistory = bool.Parse(getPointsHistoryStr);
            }

            string getOtherPointsHistoryStr = parms[3] as string;
            bool getOtherPointsHistory = false;
            if (!string.IsNullOrEmpty(getOtherPointsHistoryStr))
            {
                getOtherPointsHistory = bool.Parse(getOtherPointsHistoryStr);
            }

            string summaryStartIndexStr = parms[4] as string;
            string summaryBatchSizeStr = parms[5] as string;
            int? summaryStartIndex = !string.IsNullOrEmpty(summaryStartIndexStr) ? (int?)int.Parse(summaryStartIndexStr) : null;
            int? summaryBatchSize = !string.IsNullOrEmpty(summaryBatchSizeStr) ? (int?)int.Parse(summaryBatchSizeStr) : null;

            string otherStartIndexStr = parms[6] as string;
            string otherBatchSizeStr = parms[7] as string;
            int? otherStartIndex = !string.IsNullOrEmpty(otherStartIndexStr) ? (int?)int.Parse(otherStartIndexStr) : null;
            int? otherBatchSize = !string.IsNullOrEmpty(otherBatchSizeStr) ? (int?)int.Parse(otherBatchSizeStr) : null;


            if (endDate < startDate)
            {
                _logger.Error(_className, methodName, "End date cannot be earlier than the start date");
                throw new LWOperationInvocationException("End date cannot be earlier than the start date") { ErrorCode = 3204 };
            }
            if (startDate != null)
            {
                startDate = DateTimeUtil.GetBeginningOfDay((DateTime)startDate);
            }
            if (endDate != null)
            {
                endDate = DateTimeUtil.GetEndOfDay((DateTime)endDate);
            }

            LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(summaryStartIndex, summaryBatchSize, Config.EnforceValidBatch);

            bool retrieveExpiredTransactions = false;
            if (!string.IsNullOrEmpty(GetFunctionParameter("RetrieveExpiredTransactions")))
            {
                retrieveExpiredTransactions = bool.Parse(GetFunctionParameter("RetrieveExpiredTransactions"));
            }

            IList<IClientDataObject> txnHeaders = AccountActivityUtil.GetAccountActivitySummary(member, startDate, endDate, getPointsHistory, retrieveExpiredTransactions, batchInfo);

            List<MGAccountActivitySummary> summaryList = new List<MGAccountActivitySummary>();

            MGAccountActivitySummaryResponse response = null;

            if (txnHeaders != null && txnHeaders.Count > 0)
            {
                foreach (IClientDataObject dobj in txnHeaders)
                {
                    MGAccountActivitySummary summary = new MGAccountActivitySummary()
                    {
                        TxnHeaderId = dobj.GetAttributeValue("TxnHeaderId").ToString()
                    };
                    object txnDate = dobj.GetAttributeValue("TxnDate");
                    if (txnDate != null)
                    {
                        summary.TxnDate = txnDate as DateTime?;
                    }
                    object txnNumber = dobj.GetAttributeValue("TxnNumber");
                    if (txnNumber != null)
                    {
                        summary.TxnNumber = txnNumber.ToString();
                    }
                    object txnRegNmbr = dobj.GetAttributeValue("TxnRegisterNumber");
                    if (txnRegNmbr != null)
                    {
                        summary.TxnRegisterNumber = txnRegNmbr.ToString();
                    }
                    if (dobj.HasTransientProperty("Store"))
                    {
                        StoreDef store = (StoreDef)dobj.GetTransientProperty("Store");
                        summary.TxnStoreInfo = new MGStoreInfo() 
                        {
                            StoreName = store.StoreName,
                            StoreNumber = store.StoreNumber,
                            City = store.City,
                            State = store.StateOrProvince
                        };                                                
                    }
                    summary.TxnAmount = (decimal)dobj.GetAttributeValue("TxnAmount");
                    summary.TxnChannel = dobj.GetAttributeValue("TxnChannel") as string;
                    if (getPointsHistory)
                    {
                        IList<PointTransaction> headerTxnList = (IList<PointTransaction>)dobj.GetTransientProperty("PointsHistory");
                        if (headerTxnList != null && headerTxnList.Count > 0)
                        {
                            summary.PointsHistory = new List<MGPointsHistory>();
                            foreach (PointTransaction txn in headerTxnList)
                            {
                                summary.PointsHistory.Add(MGPointsHistory.Hydrate(txn));
                            }                                                        
                        }
                    }
                    string[] predefinedAttributes = GetAdditionalAttributesList();
                    if (predefinedAttributes != null && predefinedAttributes.Length > 0)
                    {
                        AttributeSetMetaData attSetMeta = LoyaltyService.GetAttributeSetMetaData("TxnHeader");
                        summary.AdditionalAttributes = new List<MGClientEntityAttribute>();                        
                        foreach (string attName in predefinedAttributes)
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
                                summary.AdditionalAttributes.Add(att);
                            }
                        }                        
                    }
					summaryList.Add(summary);
                }
            }

            List<MGPointsHistory> otherPointsHistory = new List<MGPointsHistory>();

            if (getOtherPointsHistory)
            {
                batchInfo = LWQueryBatchInfo.GetValidBatch(otherStartIndex, otherBatchSize, Config.EnforceValidBatch);

                IList<PointTransaction> filteredList = null;
                try
                {
                    filteredList = AccountActivityUtil.GetOtherPointsHistory(
                         member,
                         startDate,
                         endDate,
                         GetFunctionParameter("OrphanTxnTypesFilter"),
                         GetFunctionParameter("OrphanPointTypesFilter"),
                         GetFunctionParameter("OrphanPointEventsFilter"),
                         retrieveExpiredTransactions,
                         false,
                         batchInfo);
                }
                catch (LWException ex)
                {
                    if (ex.ErrorCode != 3230)
                    {
                        _logger.Error(_className, methodName, "Error loading grid data.", ex);
                        throw;
                    }
                    else
                    {
                        _logger.Error(_className, methodName, ex.Message);
                    }
                }
                if (filteredList != null && filteredList.Count > 0)
                {                    
                    foreach (PointTransaction txn in filteredList)
                    {
                        otherPointsHistory.Add(MGPointsHistory.Hydrate(txn));                        
                    }                    
                }
            }

            response = new MGAccountActivitySummaryResponse()
            {
                AccountActivitySummary = summaryList,
                OtherPointsHistory = otherPointsHistory
            };
            return response;
        }
    }
}