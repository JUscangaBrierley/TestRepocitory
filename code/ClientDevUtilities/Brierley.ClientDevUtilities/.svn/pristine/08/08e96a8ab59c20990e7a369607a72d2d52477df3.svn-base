using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.LWIntegration.Util
{
	public class AccountActivityUtil
	{
        private const string _className = "AccountActivityUtil";        
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public static IList<IClientDataObject> GetAccountActivitySummary(
			Member member,
			DateTime? startDate,
			DateTime? endDate,
			bool getPointsHistory,
            bool includeExpired,
			LWQueryBatchInfo batchInfo)
		{
            string methodName = "GetAccountActivitySummary";

			using(LoyaltyDataService loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData meta = loyaltyService.GetAttributeSetMetaData("TxnHeader");
				if (meta == null)
				{
					throw new LWIntegrationException("Standard implementation requires TxnHeader attribute set to be defined.") { ErrorCode = 3357 };
				}

				LWCriterion crit = new LWCriterion(meta.Name);
				if (startDate != null)
				{
					crit.Add(LWCriterion.OperatorType.AND, "TxnDate", startDate, LWCriterion.Predicate.Ge);
				}
				if (endDate != null)
				{
					crit.Add(LWCriterion.OperatorType.AND, "TxnDate", endDate, LWCriterion.Predicate.Le);
				}

                //LW3856 Adding a sort order, we will be using TxnDate for now but should be configurable in the future
                crit.AddOrderBy("TxnDate", LWCriterion.OrderType.Descending);

				VirtualCard[] vcCards = member.LoyaltyCards.ToArray<VirtualCard>();
				if (vcCards == null || vcCards.Length == 0)
				{
					_logger.Error(_className, methodName, string.Format("Member with IpCode {0} has no loyalty cards.", member.IpCode));
					return null;
				}

				IList<IClientDataObject> txnHeaders = loyaltyService.GetAttributeSetObjects(vcCards, meta, crit, batchInfo, false, false);

				if (txnHeaders != null && txnHeaders.Count > 0)
				{
					// get all the stores
					Dictionary<long, long> storeMap = new Dictionary<long, long>();
					IList<long> rowKeyList = new List<long>();
					IList<long> storeIdList = new List<long>();
					StoreDef sd = new StoreDef();
					foreach (IClientDataObject dobj in txnHeaders)
					{
						rowKeyList.Add(dobj.MyKey);
						//int storeId = (int)dobj.GetAttributeValue("TxnStoreId");
						long storeId = long.Parse(dobj.GetAttributeValue("TxnStoreId").ToString());
						if (!storeMap.ContainsKey(storeId))
						{
							storeIdList.Add(storeId);
							storeMap.Add(storeId, storeId);
						}
					}

					if (storeIdList.Count > 0)
					{
                        using (ContentService contentService = LWDataServiceUtil.ContentServiceInstance())
                        {
                            IList<StoreDef> stores = contentService.GetAllStoreDefs(storeIdList.ToArray<long>());
                            foreach (StoreDef store in stores)
                            {
                                foreach (IClientDataObject dobj in txnHeaders)
                                {
                                    long storeId = long.Parse(dobj.GetAttributeValue("TxnStoreId").ToString());
                                    if (storeId == store.StoreId)
                                    {
                                        dobj.UpdateTransientProperty("Store", store);
                                    }
                                }
                            }
                        }
					}
					// get all point transactions
					long[] vckeys = new long[member.LoyaltyCards.Count];
					int idx = 0;
					foreach (VirtualCard vc in member.LoyaltyCards)
					{
						vckeys[idx++] = vc.VcKey;
					}
					Dictionary<long, IList<PointTransaction>> txnMap = new Dictionary<long, IList<PointTransaction>>();

					if (getPointsHistory)
					{
						IList<PointTransaction> pointTxns = loyaltyService.GetPointTransactionsByOwner(
							vckeys, null,
							PointTransactionOwnerType.AttributeSet, meta.ID, rowKeyList.ToArray<long>(), includeExpired);
						foreach (PointTransaction txn in pointTxns)
						{
							IList<PointTransaction> headerTxnList = null;
							if (txnMap.ContainsKey(txn.RowKey))
							{
								headerTxnList = txnMap[txn.RowKey];
							}
							else
							{
								headerTxnList = new List<PointTransaction>();
								txnMap.Add(txn.RowKey, headerTxnList);
								foreach (IClientDataObject header in txnHeaders)
								{
									if (header.MyKey == txn.RowKey)
									{
										header.UpdateTransientProperty("PointsHistory", headerTxnList);
									}
								}
							}
							headerTxnList.Add(txn);
						}
					}
				}
				return txnHeaders;
			}
		}

        public static IList<PointTransaction> GetOtherPointsHistory(
            Member member,
            DateTime? startDate,
            DateTime? endDate,
            PointBankTransactionType[] txnTypes,
            long[] pointEventIds,
            long[] pointTypeIds,
            bool includeExpired,
            LWQueryBatchInfo batchInfo)
        {
			using (LoyaltyDataService svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData txnHeaderMeta = svc.GetAttributeSetMetaData("TxnHeader");
				AttributeSetMetaData txnDtlMeta = svc.GetAttributeSetMetaData("TxnDetailItem");

				IList<PointTransaction> txnList = svc.GetPointTransactionsByPointTypePointEvent(member, startDate, endDate, txnTypes, pointTypeIds, pointEventIds, null, null, null, includeExpired, batchInfo);
				IList<PointTransaction> filteredList = new List<PointTransaction>();
				if (txnList != null && txnList.Count > 0)
				{
					foreach (PointTransaction txn in txnList)
					{
						if (txn.OwnerType == PointTransactionOwnerType.AttributeSet &&
							(txn.OwnerId == txnHeaderMeta.ID || txn.OwnerId == txnDtlMeta.ID))
						{
							continue;
						}
						filteredList.Add(txn);
					}
				}
				return filteredList;
			}
        }

		public static IList<PointTransaction> GetOtherPointsHistory(
			Member member,
			DateTime? startDate,
			DateTime? endDate,
            string txnTypeFilter,
            string PointTypeFilter,
            string PointEventFilter,
            bool includeExpired,
            bool aggregatePointTxns,
			LWQueryBatchInfo batchInfo)
		{
            string methodName = "GetOtherPointsHistory";

			using (LoyaltyDataService svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				PointBankTransactionType[] txnTypes = null;
				if (!string.IsNullOrEmpty(txnTypeFilter))
				{
					string[] tokens = txnTypeFilter.Split(';');
					txnTypes = new PointBankTransactionType[tokens.Length];
					for (int i = 0; i < tokens.Length; i++)
					{
						txnTypes[i] = (PointBankTransactionType)Enum.Parse(typeof(PointBankTransactionType), tokens[i]);
					}
				}

				long[] pointTypeIds = null;
				if (!string.IsNullOrEmpty(PointTypeFilter))
				{
					string[] tokens = PointTypeFilter.Split(';');
					pointTypeIds = new long[tokens.Length];
					for (int i = 0; i < tokens.Length; i++)
					{
						PointType pt = svc.GetPointType(tokens[i]);
						if (pt != null)
						{
							pointTypeIds[i] = pt.ID;
						}
						else
						{
							string errMsg = string.Format("Loyalty currency {0} provided in currency filter does not exist.", tokens[i]);
							_logger.Error(_className, methodName, errMsg);
							throw new LWException(errMsg) { ErrorCode = 3311 };
						}
					}
				}

				long[] pointEventIds = null;
				if (!string.IsNullOrEmpty(PointEventFilter))
				{
					string[] tokens = PointEventFilter.Split(';');
					pointEventIds = new long[tokens.Length];
					for (int i = 0; i < tokens.Length; i++)
					{
						PointEvent pe = svc.GetPointEvent(tokens[i]);
						if (pe != null)
						{
							pointEventIds[i] = pe.ID;
						}
						else
						{
							string errMsg = string.Format("Loyalty event {0} provided in event filter does not exist.", tokens[i]);
							_logger.Error(_className, methodName, errMsg);
							throw new LWException(errMsg) { ErrorCode = 3310 };
						}
					}
				}

				IList<PointTransaction> finalTxnList = new List<PointTransaction>();
				IList<PointTransaction> txnList = GetOtherPointsHistory(member, startDate, endDate, txnTypes, pointEventIds, pointTypeIds, includeExpired, batchInfo);
				if (txnList != null && txnList.Count > 0 && aggregatePointTxns)
				{
					Dictionary<string, List<PointTransaction>> txnMap = new Dictionary<string, List<PointTransaction>>();
					foreach (PointTransaction txn in txnList)
					{
						if (txn.OwnerType != PointTransactionOwnerType.Unknown &&
							txn.OwnerId > 0)
						{
							List<PointTransaction> grpTxns = null;
							string key = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", txn.TransactionType, txn.PointTypeId, txn.PointEventId, txn.OwnerType, txn.OwnerId, txn.RowKey);
							if (txnMap.ContainsKey(key))
							{
								grpTxns = txnMap[key];
							}
							else
							{
								grpTxns = new List<PointTransaction>();
								txnMap.Add(key, grpTxns);
							}
							grpTxns.Add(txn);
						}
						else
						{
							finalTxnList.Add(txn);
						}
					}
					foreach (List<PointTransaction> grpTxns in txnMap.Values)
					{
						PointTransaction aggrtTxn = null;
						// create one aggregate point transaction
						foreach (PointTransaction txn in grpTxns)
						{
							if (aggrtTxn == null)
							{
								aggrtTxn = new PointTransaction()
								{
									VcKey = txn.VcKey,
									PointTypeId = txn.PointTypeId,
									PointEventId = txn.PointEventId,
									TransactionType = txn.TransactionType,
									TransactionDate = txn.TransactionDate,
									PointAwardDate = txn.PointAwardDate,
									Points = txn.Points,
									ExpirationReason = txn.ExpirationReason,
									ExpirationDate = txn.ExpirationDate,
									Notes = txn.Notes,
									PromoCode = txn.PromoCode,
									OwnerType = txn.OwnerType,
									OwnerId = txn.OwnerId,
									RowKey = txn.RowKey,
									ParentTransactionId = txn.ParentTransactionId,
									PointsConsumed = txn.PointsConsumed,
									PointsOnHold = txn.PointsOnHold,
									LocationId = txn.LocationId,
									ChangedBy = txn.ChangedBy,
								};
							}
							else
							{
								aggrtTxn.Points += txn.Points;
							}
						}
						if (aggrtTxn != null)
						{
							finalTxnList.Add(aggrtTxn);
						}
					}
					return finalTxnList;
				}
				else
				{
					return txnList;
				}
			}
		}

        private static IList<IClientDataObject> PopulateDetails(IList<IClientDataObject> txnDetails, AttributeSetMetaData meta, bool getPointsHistory, bool includeExpired)
		{
			using (ContentService contentService = LWDataServiceUtil.ContentServiceInstance())
			using (LoyaltyDataService loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				if (txnDetails != null && txnDetails.Count > 0)
				{
					// get all products
					Dictionary<long, long> prodMap = new Dictionary<long, long>();
					IList<long> prodIdList = new List<long>();
					IList<long> rowKeyList = new List<long>();
					IList<long> vcKeyList = new List<long>();
					foreach (IClientDataObject dobj in txnDetails)
					{
						//PropertyInfo prop = dobj.GetType().GetProperty("IpCode");
						//object ipcode = prop.GetValue(dobj, null);
						object vcKey = ClassLoaderUtil.InvokeMethod(dobj, "get_VcKey", null);
						vcKeyList.Add((long)vcKey);
						rowKeyList.Add(dobj.MyKey);
						if (dobj.GetAttributeValue("DtlProductId") != null)
						{
							long prodId = long.Parse(dobj.GetAttributeValue("DtlProductId").ToString());
							if (!prodMap.ContainsKey(prodId))
							{
								prodIdList.Add(prodId);
								prodMap.Add(prodId, prodId);
							}
						}
					}

					if (prodIdList.Count > 0)
					{
						IList<Product> products = contentService.GetAllProducts(prodIdList.ToArray<long>(), true);
						foreach (Product product in products)
						{
							foreach (IClientDataObject dobj in txnDetails)
							{
								long prodId = 0;
								if (dobj.GetAttributeValue("DtlProductId") != null)
								{
									prodId = long.Parse(dobj.GetAttributeValue("DtlProductId").ToString());
								}
								if (prodId == product.Id)
								{
									dobj.UpdateTransientProperty("Product", product);
								}
							}
						}
					}
					// get all point transactions                    
					Dictionary<long, IList<PointTransaction>> txnMap = new Dictionary<long, IList<PointTransaction>>();

					if (getPointsHistory)
					{
						var vckeys = (from vckey in vcKeyList select vckey).Distinct();
						IList<PointTransaction> pointTxns = loyaltyService.GetPointTransactionsByOwner(vckeys.ToArray<long>(), null, PointTransactionOwnerType.AttributeSet, meta.ID, rowKeyList.ToArray<long>(), includeExpired);
						foreach (PointTransaction txn in pointTxns)
						{
							IList<PointTransaction> deatilTxnList = null;
							if (txnMap.ContainsKey(txn.RowKey))
							{
								deatilTxnList = txnMap[txn.RowKey];
							}
							else
							{
								deatilTxnList = new List<PointTransaction>();
								txnMap.Add(txn.RowKey, deatilTxnList);
								foreach (IClientDataObject detail in txnDetails)
								{
									if (detail.MyKey == txn.RowKey)
									{
										detail.UpdateTransientProperty("PointsHistory", deatilTxnList);
									}
								}
							}
							deatilTxnList.Add(txn);
						}
					}
				}
				return txnDetails;
			}
		}

		public static IList<IClientDataObject> GetAccountActivityDetail(
			string txnHeaderId, bool getPointsHistory, bool includeExpired)
		{
			using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData meta = svc.GetAttributeSetMetaData("TxnDetailItem");
				if (meta == null)
				{
					throw new LWIntegrationException("Standard implementation requires TxnDetailItem attribute set to be defined.") { ErrorCode = 3357 };
				}

				LWCriterion crit = new LWCriterion(meta.Name);
				crit.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txnHeaderId, LWCriterion.Predicate.Eq);

				IList<IClientDataObject> txnDetails = svc.GetAttributeSetObjects((IAttributeSetContainer)null, meta, crit, null, false);

				return PopulateDetails(txnDetails, meta, getPointsHistory, includeExpired);
			}
		}

		public static IList<IClientDataObject> GetAccountActivityDetails(
            AttributeSetContainer[] headers, bool getPointsHistory, bool includeExpired)
		{
			using (var svc = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData meta = svc.GetAttributeSetMetaData("TxnDetailItem");
				if (meta == null)
				{
					throw new LWIntegrationException("Standard implementation requires TxnDetailItem attribute set to be defined.") { ErrorCode = 3357 };
				}

				IList<IClientDataObject> txnDetails = svc.GetAttributeSetObjects(headers, meta, null, null, false);

				return PopulateDetails(txnDetails, meta, getPointsHistory, includeExpired);
			}
		}
	}
}
