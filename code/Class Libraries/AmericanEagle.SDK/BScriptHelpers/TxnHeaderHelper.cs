using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.ClientDevUtilities.LWGateway;
using LWDataServiceUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil;
using System.Reflection;
using Brierley.FrameWork;

namespace AmericanEagle.SDK.BScriptHelpers
{
    public class TxnHeaderHelper : ITxnHeaderHelper
    {
        #region [Properties]
        private ILWDataServiceUtil _dataUtil;
        private LWLogger _logger = null;
        private const string _appName = "BScriptHelpers.TxnHeaderHelper";
        private const string _attributSetNameTxnDetailItem = "TxnDetailItem";
        #endregion

        public TxnHeaderHelper()
        {
            this._dataUtil = LWDataServiceUtil.Instance;
            _logger = LWLoggerManager.GetLogger(_appName);
        }

        public TxnHeaderHelper(ILWDataServiceUtil dataUtil)
        {
            this._dataUtil = dataUtil;
            _logger = LWLoggerManager.GetLogger(_appName);
        }

        public IList<TxnFourPartKey> GetOriginalTxnHeaderInfoForReturnedItems(IList<IClientDataObject> atsList)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            
            IList<TxnFourPartKey> allInvolvedOriginalTxn = new List<TxnFourPartKey>();
            IList<TxnFourPartKey> originalTxn = new List<TxnFourPartKey>();
            if (atsList != null)
            {
                for (int i = 0; i < atsList.Count; i++)
                {
                    if (atsList[i].GetType() == typeof(TxnDetailItem))
                    {
                        TxnDetailItem txnDetail = (TxnDetailItem)atsList[i];
                        //If is a returned Item
                        if (txnDetail.DtlTypeId == 2 && txnDetail.TxnOriginalTxnDate != null && txnDetail.TxnOriginalStoreId != null
                            && txnDetail.TxnOriginalOrderNumber != null && txnDetail.TxnOriginalTxnNumber != null)
                        {
                            allInvolvedOriginalTxn.Add(
                                new TxnFourPartKey()
                                {
                                    TxnNumber = txnDetail.TxnOriginalTxnNumber,
                                    OrderNumber = txnDetail.TxnOriginalOrderNumber,
                                    TxnStoreId = txnDetail.TxnOriginalStoreId,
                                    TxnDate = (DateTime)txnDetail.TxnOriginalTxnDate
                                }
                            );
                        }
                    }
                }
            }

            //Get only different OriginalTxn
            var difInvolvedOriginalTxn = allInvolvedOriginalTxn.GroupBy(
                        x => new { x.OrderNumber, x.TxnNumber, x.TxnDate, x.TxnStoreId }
                    ).Select(group => group.First());

            foreach (var item in difInvolvedOriginalTxn)
                originalTxn.Add(item);
            
            return originalTxn;
        }

        public TxnHeader GetTxnHeader(TxnFourPartKey txnKey)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            
            TxnHeader txnHeader = null;
            
            if (!String.IsNullOrWhiteSpace(txnKey.OrderNumber) && !String.IsNullOrWhiteSpace(txnKey.TxnNumber) && txnKey.TxnStoreId != null && txnKey.TxnDate != null)
            {
                //Getting RowKey of Txn based on TxnFourPartKey
                var lwCriteria = new LWCriterion("TxnHeader");
                lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnDate", DateTimeUtil.GetBeginningOfDay((DateTime)txnKey.TxnDate), LWCriterion.Predicate.Ge);
                lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnDate", DateTimeUtil.GetEndOfDay((DateTime)txnKey.TxnDate), LWCriterion.Predicate.Le);
                lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnStoreId", txnKey.TxnStoreId, LWCriterion.Predicate.Eq);
                lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnNumber", txnKey.TxnNumber, LWCriterion.Predicate.Eq);
                lwCriteria.Add(LWCriterion.OperatorType.AND, "OrderNumber", txnKey.OrderNumber, LWCriterion.Predicate.Eq);

                using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance())
                {
                    var clientDataObjects = dataService.GetAttributeSetObjects(null, "TxnHeader", lwCriteria, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = int.MaxValue }, false);
                    if(clientDataObjects != null && clientDataObjects.Count>0)
                        txnHeader = clientDataObjects?.Cast<TxnHeader>().FirstOrDefault();
                }
            }
            return txnHeader;
        }

        public decimal GetTxnPointsOnOriginal(Member member,TxnHeader txnHeader, PointType pointType, PointEvent pointEvent)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            decimal points = 0;
            
            using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance())
            {
                if (txnHeader != null && member != null && pointType != null && pointEvent!= null)
                {
                    AttributeSetMetaData atsMetadata = dataService.GetAttributeSetMetaData("TxnHeader");
                    _logger.Trace(_appName, methodName, String.Format("Getting Points Balance for RowKey: {0}", txnHeader.RowKey));
                    points = dataService.GetPointBalance(
                        vcKeys: member.GetLoyaltyCardIds(),
                        pointTypeIds: new long[] { pointType.ID },
                        pointEventIds: new long[] { pointEvent.ID },
                        txnTypes: new PointBankTransactionType[] { PointBankTransactionType.Credit },
                        from: null,
                        to: null,
                        awardDateFrom: null,
                        awardDateTo: null,
                        changedBy: null,
                        locationId: null,
                        ownerType: PointTransactionOwnerType.AttributeSet,
                        ownerId: atsMetadata.ID,
                        rowkeys: new long[] { txnHeader.RowKey });
                    
                }
            }            
            return points;
        }

        public IList<IClientDataObject> GetDetailItemsByTxnHeader(TxnHeader txnHeader)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            IList<IClientDataObject> clDataObjectsTxnDetailItem = new List<IClientDataObject>();

            if (txnHeader != null)
            {
                using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance())
                {
                    AttributeSetMetaData attSetMdTxnDetailItem = dataService.GetAttributeSetMetaData(_attributSetNameTxnDetailItem);
                    StringBuilder whereFractionForTxn = new StringBuilder();
                    whereFractionForTxn.Append(" A_TxnHeaderId = '" + txnHeader.TxnHeaderId + "'");
                    clDataObjectsTxnDetailItem = dataService.GetAttributeSetObjects((IAttributeSetContainer)null, attSetMdTxnDetailItem, String.Empty, whereFractionForTxn.ToString(), String.Empty, (LWQueryBatchInfo)null, false, false);
                }
            }
            return clDataObjectsTxnDetailItem;
        }

        public IList<IClientDataObject> GetAllReturnedDetailItemsLinkedToTxnHeader(Member member, TxnHeader txnHeader, string currentTxnHeaderId = null)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            IList<IClientDataObject> clDataObjectsTxnDetailItem = null;

            if (txnHeader != null && member != null)
            {
                using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance())
                {
                    var lwCriteria = new LWCriterion("TxnDetailitem");
                    lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnOriginalTxnDate", DateTimeUtil.GetBeginningOfDay(txnHeader.TxnDate), LWCriterion.Predicate.Ge);
                    lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnOriginalTxnDate", DateTimeUtil.GetEndOfDay(txnHeader.TxnDate), LWCriterion.Predicate.Le);
                    lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnOriginalStoreId", txnHeader.TxnStoreId, LWCriterion.Predicate.Eq);
                    lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnOriginalTxnNumber", txnHeader.TxnNumber, LWCriterion.Predicate.Eq);
                    lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnOriginalOrderNumber", txnHeader.OrderNumber, LWCriterion.Predicate.Eq);
                    lwCriteria.Add(LWCriterion.OperatorType.AND, "DtlTypeId", 2, LWCriterion.Predicate.Eq);
                    if (!String.IsNullOrWhiteSpace(currentTxnHeaderId))
                        lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", currentTxnHeaderId, LWCriterion.Predicate.Ne);

                    AttributeSetMetaData attributeSetMetaData = dataService.GetAttributeSetMetaData("TxnDetailItem");

                    clDataObjectsTxnDetailItem = dataService.GetAttributeSetObjects(member.LoyaltyCards.ToArray(), attributeSetMetaData, lwCriteria, new LWQueryBatchInfo() { StartIndex = 0, BatchSize = int.MaxValue }, false);
                }
            }
            return clDataObjectsTxnDetailItem;
        }

        public IList<IClientDataObject> GetChildAttributesFromContextObject(ContextObject contextObject, string attributeSetName)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;

            IList<IClientDataObject> atsList;

            using (ILoyaltyDataService dataService = this._dataUtil.LoyaltyDataServiceInstance())
            {
                AttributeSetMetaData attributeSetMetaData = dataService.GetAttributeSetMetaData(attributeSetName);
                //IAttributeSetContainer root = contextObject.InvokingRow != null ? contextObject.InvokingRow : contextObject.Owner;

                if (attributeSetMetaData == null)
                    throw new LWBScriptException("Unable to retrieve metadata for " + attributeSetName);

                IAttributeSetContainer parentContainer = null;
                if (contextObject.InvokingRow.GetType() == typeof(Member))
                    parentContainer = (Member)contextObject.Owner;
                else if (contextObject.InvokingRow.GetType() == typeof(VirtualCard))
                    parentContainer = (VirtualCard)contextObject.Owner;
                else
                {
                    if (contextObject.InvokingRow != null)
                        parentContainer = contextObject.InvokingRow;
                    else
                        parentContainer = contextObject.Owner;
                }

                if (!parentContainer.IsLoaded(attributeSetName))
                {
                    _logger.Trace(_appName, methodName, String.Format("Attribute Childs not loaded"));
                    dataService.LoadAttributeSetList(parentContainer, attributeSetName, false);
                }
                atsList = parentContainer.GetChildAttributeSets(attributeSetName);
                _logger.Trace(_appName, methodName, String.Format("Attribute Childs on current:{0}", atsList.Count));
            }
            return atsList;
        }

    }

}
