using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.Interfaces;

namespace Brierley.FrameWork.LWIntegration
{
    public class RequestCreditHelper : IRequestCreditInterceptor
    {
        #region Fields
        private const string _className = "RequestCreditHelper";
        private static LWLogger _logger = LWLoggerManager.GetLogger("RequestCredit");        
        #endregion

        #region Properties        
        #endregion

        #region Construction
        public RequestCreditHelper()
        {            
        }

        public static IRequestCreditInterceptor CreateRequestCreditInterceptor(string className, string assemblyName)
        {
            string methodName = "CreateRequestCreditInterceptor";

            IRequestCreditInterceptor helper = null;

            if (string.IsNullOrEmpty(assemblyName))
            {
                _logger.Debug(_className, methodName, "No interceptor assembly provided.  Creating default interceptor.");
                helper = new RequestCreditHelper();
            }
            else
            {
                helper = ClassLoaderUtil.CreateInstance(assemblyName, className) as IRequestCreditInterceptor;                
            }

            if (helper == null)
            {
                _logger.Error(_className, methodName, "Unable to instantiate Request credit helper.");
                throw new Brierley.FrameWork.Common.Exceptions.LWException("Unable to instantiate Request credit helper.") { ErrorCode = 1 };
            }
            return helper;
        }
        #endregion

        #region Public methods

        #region Search Related
        public virtual IList<IClientDataObject> SearchTransaction(
            TransactionType transactionType, 
            Dictionary<String, String> searchParms,
            string processIdSuppressionList,
            LWQueryBatchInfo batchInfo)
        {
            string methodName = "SearchTransaction";

			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				AttributeSetMetaData attSetMeta = loyalty.GetAttributeSetMetaData("HistoryTxnDetail");
				IList<IClientDataObject> historyRecords = new List<IClientDataObject>();
				LWCriterion lwCriteria = new LWCriterion(attSetMeta.Name);
				try
				{
					if (searchParms.Count > 0)
					{
						foreach (KeyValuePair<String, String> p in searchParms)
						{
							//Check whether attribute has property "Show As Range" or not
							if (p.Key.EndsWith("_Start"))
							{
								lwCriteria.AddDistinct(p.Key.Substring(0, p.Key.Length - 6));
								lwCriteria.Add(LWCriterion.OperatorType.AND, p.Key.Substring(0, p.Key.Length - 6), p.Value, LWCriterion.Predicate.Ge);
							}
							else if (p.Key.EndsWith("_End"))
							{
								lwCriteria.AddDistinct(p.Key.Substring(0, p.Key.Length - 4));
								lwCriteria.Add(LWCriterion.OperatorType.AND, p.Key.Substring(0, p.Key.Length - 4), p.Value, LWCriterion.Predicate.Le);
							}
							else
							{
								lwCriteria.AddDistinct(p.Key);
								AttributeMetaData attMeta = attSetMeta.GetAttribute(p.Key);
								if (attMeta.DataType == "Date")
								{
									DateTime date = DateTimeUtil.ConvertStringToDate(string.Empty, p.Value);
									DateTime start = DateTimeUtil.GetBeginningOfDay(date);
									DateTime end = DateTimeUtil.GetEndOfDay(date);
									lwCriteria.Add(LWCriterion.OperatorType.AND, p.Key, start.ToString("F"), LWCriterion.Predicate.Ge);
									lwCriteria.Add(LWCriterion.OperatorType.AND, p.Key, end.ToString("F"), LWCriterion.Predicate.Le);
								}
								else
								{
									lwCriteria.Add(LWCriterion.OperatorType.AND, p.Key, p.Value, LWCriterion.Predicate.Eq);
								}
							}
						}
						AddClientSpecificSearchCriteria(lwCriteria, processIdSuppressionList);
						lwCriteria.AddDistinct("TxnHeaderId");
						historyRecords = loyalty.GetAttributeSetObjects(null, "HistoryTxnDetail", lwCriteria, batchInfo, false);
						if (historyRecords != null)
						{
							historyRecords = ApplyFilterOnTransactionHeader(historyRecords, transactionType);
						}
					}
					return historyRecords;
				}

				catch (Exception ex)
				{
					_logger.Error(_className, methodName, ex.Message, ex);
					throw;
				}
			}
        }

        /// <summary>
        /// Provides an opportunity for the client provider implementation to append additional criteria before searching. 
        /// </summary>
        /// <remarks>
        /// Default implementation is to exclude ProcessId 1 and 7. This method may be overridden to eliminate the default 
        /// filtering, or to add additional filtering, or both.
        /// </remarks>
        /// <example>
        /// //exclude ProcessId 1 and 7:
        /// criteria.Add(LWCriterion.OperatorType.AND, "ProcessId", 1, LWCriterion.Predicate.Ne);
        /// criteria.Add(LWCriterion.OperatorType.AND, "ProcessId", 7, LWCriterion.Predicate.Ne);
        /// </example>
        /// <param name="criteria"></param>
        public virtual void AddClientSpecificSearchCriteria(LWCriterion criteria, string processIdSuppressionList)
        {
            long[] processList = new long[] { 1, 7 };
            if (!string.IsNullOrEmpty(processIdSuppressionList))
            {
                string[] tokens = processIdSuppressionList.Split(',');
                processList = new long[tokens.Length];
                for (int i = 0; i < tokens.Length; i++)
                {
                    processList[i] = long.Parse(tokens[i]);
                }
            }
            foreach (long pid in processList)
            {
                criteria.Add(LWCriterion.OperatorType.AND, "ProcessId", pid, LWCriterion.Predicate.Ne);
            }
            //criteria.Add(LWCriterion.OperatorType.AND, "ProcessId", 1, LWCriterion.Predicate.Ne);
            //criteria.Add(LWCriterion.OperatorType.AND, "ProcessId", 7, LWCriterion.Predicate.Ne);
        }

        /// <summary>
        /// Provides an opportunity for the client provider to filter out unwanted or invalid transaction headers before they are displayed.
        /// </summary>
        /// <remarks>
        /// Victoria's Secret requires that valid transaction headers have a TxnHeaderID that ends with "VSS" for in store and 
        /// "VSD" for online. That filtering - or any other client specific filtering - may be implemented in a custom class by 
        /// overriding this method.
        /// </remarks>
        /// <example>
        ///	IList<IClientDataObject> outputList = new List<IClientDataObject>();
        ///	foreach (IClientDataObject obj in transactionList)
        ///	{
        ///		string txnHeader = (string)obj.GetAttributeValue("TxnHeaderID");
        ///		if (transactionType == TransactionType.Store && txnHeader.EndsWith("VSS"))
        ///		{
        ///			outputList.Add(obj);
        ///		}
        ///		else if (transactionType == TransactionType.Online && txnHeader.EndsWith("VSD"))
        ///		{
        ///			outputList.Add(obj);
        ///		}
        ///	}
        ///	return outputList;
        /// </example>
        /// <param name="transactionList"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public virtual IList<IClientDataObject> ApplyFilterOnTransactionHeader(IList<IClientDataObject> transactionList, TransactionType transactionType)
        {
            return transactionList;
        }
        #endregion

        #region Applying Credit        
        public virtual decimal AddLoyaltyTransaction(Member member, string cardId, string txnHeaderId)
        {
            string methodName = "AddLoyaltyTransaction";
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				IClientDataObject historyTxnDetail = null;
				VirtualCard card = member.LoyaltyCards.FirstOrDefault(vc => vc.IsPrimary);
				if (!string.IsNullOrEmpty(cardId) && card.LoyaltyIdNumber != cardId)
				{
					card = member.GetLoyaltyCard(cardId);
				}

				_logger.Debug(_className, methodName, string.Format("Adding loyalty transaction with header id {0} to member with ipcode {1}", txnHeaderId, member.IpCode));

				IClientDataObject txnHeader = null;

				try
				{
					//Get primary card of member
					var lstTxnLineItemDiscount = new List<IClientDataObject>();
					var lstTxnTender = new List<IClientDataObject>();
					var lstTxnRewardRedeem = new List<IClientDataObject>();
					var lstTxnDetailItem = new List<IClientDataObject>();

                    //check if the TxnHeader already exists
                    // Check if the header record already exists
                    LWCriterion txnHeaderCriteria = new LWCriterion("TxnHeader");
                    txnHeaderCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txnHeaderId, LWCriterion.Predicate.Eq);
                    List<IClientDataObject> txnHeaderRecords = loyalty.GetAttributeSetObjects(null, "TxnHeader", txnHeaderCriteria, null, false);
                    if (txnHeaderRecords != null && txnHeaderRecords.Count > 0)
                    {
                        throw new LWValidationException("The matching transaction has already been credited.");
                    }

					//get HistoryTxnDetail for a header
					var lwCriteria = new LWCriterion("HistoryTxnDetail");
					lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txnHeaderId, LWCriterion.Predicate.Eq);
					lwCriteria.AddOrderBy("TxnHeaderId", LWCriterion.OrderType.Descending);
					var lstHistoryTxnDetail = loyalty.GetAttributeSetObjects(null, "HistoryTxnDetail", lwCriteria, null, false);
					//var tempTxnHeaderId = String.Empty;

					foreach (var historyDetail in lstHistoryTxnDetail)
					{
						historyTxnDetail = historyDetail;
						if (txnHeader == null)
						{
							// Initialize the transaction header
							txnHeader = DataServiceUtil.GetNewClientDataObject("TxnHeader");
							txnHeader.SetAttributeValue("TxnHeaderId", historyTxnDetail.GetAttributeValue("TxnHeaderId"));
							txnHeader.SetAttributeValue("BrandId", historyTxnDetail.GetAttributeValue("BrandId"));
							txnHeader.SetAttributeValue("CreditCardId", historyTxnDetail.GetAttributeValue("CreditCardId"));
							txnHeader.SetAttributeValue("TxnMaskId", historyTxnDetail.GetAttributeValue("TxnMaskId"));
							txnHeader.SetAttributeValue("TxnNumber", historyTxnDetail.GetAttributeValue("TxnNumber"));
							txnHeader.SetAttributeValue("TxnDate", historyTxnDetail.GetAttributeValue("TxnDate"));
							txnHeader.SetAttributeValue("TxnRegisterNumber", historyTxnDetail.GetAttributeValue("TxnRegisterNumber"));
							txnHeader.SetAttributeValue("TxnStoreId", historyTxnDetail.GetAttributeValue("TxnStoreId"));
							txnHeader.SetAttributeValue("TxnTypeId", historyTxnDetail.GetAttributeValue("TxnTypeId"));
							txnHeader.SetAttributeValue("TxnAmount", historyTxnDetail.GetAttributeValue("TxnAmount"));
							txnHeader.SetAttributeValue("TxnQualPurchaseAmt", historyTxnDetail.GetAttributeValue("TxnQualPurchaseAmt"));
							txnHeader.SetAttributeValue("TxnDiscountAmount", historyTxnDetail.GetAttributeValue("TxnDiscountAmount"));
							txnHeader.SetAttributeValue("TxnEmployeeId", historyTxnDetail.GetAttributeValue("TxnEmployeeId"));
							txnHeader.SetAttributeValue("TxnChannel", historyTxnDetail.GetAttributeValue("TxnChannel"));
							txnHeader.SetAttributeValue("TxnOriginalTxnRowKey", historyTxnDetail.GetAttributeValue("TxnOriginalTxnRowKey"));
							txnHeader.SetAttributeValue("TxnCreditsUsed", historyTxnDetail.GetAttributeValue("TxnCreditsUsed"));
							//lstHeader.Add(txnHeader);
						}

						//tempTxnHeaderId = (string)historyTxnDetail.GetAttributeValue("TxnHeaderId");
						// Initialize transaction details
						var txnDetailItem = DataServiceUtil.GetNewClientDataObject("TxnDetailItem");
						txnDetailItem.SetAttributeValue("TxnHeaderId", txnHeaderId);
						txnDetailItem.SetAttributeValue("TxnDate", historyTxnDetail.GetAttributeValue("TxnDate"));
						txnDetailItem.SetAttributeValue("TxnStoreId", historyTxnDetail.GetAttributeValue("TxnStoreId"));
						txnDetailItem.SetAttributeValue("TxnDetailId", historyTxnDetail.GetAttributeValue("TxnDetailId"));
						txnDetailItem.SetAttributeValue("DtlItemLineNbr", historyTxnDetail.GetAttributeValue("DtlItemLineNbr"));
						txnDetailItem.SetAttributeValue("DtlProductId", historyTxnDetail.GetAttributeValue("DtlProductId"));
						txnDetailItem.SetAttributeValue("DtlTypeId", historyTxnDetail.GetAttributeValue("DtlTypeId"));
						txnDetailItem.SetAttributeValue("DtlActionId", historyTxnDetail.GetAttributeValue("DtlActionId"));
						txnDetailItem.SetAttributeValue("DtlRetailAmount", historyTxnDetail.GetAttributeValue("DtlRetailAmount"));
						txnDetailItem.SetAttributeValue("DtlSaleAmount", historyTxnDetail.GetAttributeValue("DtlSaleAmount"));

						txnDetailItem.SetAttributeValue("DtlQuantity", historyTxnDetail.GetAttributeValue("DtlQuantity"));
						txnDetailItem.SetAttributeValue("DtlDiscountAmount", historyTxnDetail.GetAttributeValue("DtlDiscountAmount"));
						txnDetailItem.SetAttributeValue("DtlClearanceItem", historyTxnDetail.GetAttributeValue("DtlClearanceItem"));

						txnDetailItem.UpdateTransientProperty("DtlTypeId", historyTxnDetail.GetAttributeValue("DtlTypeId"));

						//TODO: This needs to be discussed later (WS 6/5/2011)
						txnDetailItem.UpdateTransientProperty("IsReturnFound", false);

						lstTxnDetailItem.Add(txnDetailItem);

						// Get discounts
						lwCriteria = new LWCriterion("HistoryTxnDetailDiscount");
						lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txnHeaderId, LWCriterion.Predicate.Eq);
						IList<IClientDataObject> lstHistoryTxnDetailDiscount = loyalty.GetAttributeSetObjects(null, "HistoryTxnDetailDiscount", lwCriteria, null, false);

						foreach (IClientDataObject historyLineItemDiscount in lstHistoryTxnDetailDiscount)
						{
							IClientDataObject lineItemDiscount = DataServiceUtil.GetNewClientDataObject("TxnLineItemDiscount");
							lineItemDiscount.SetAttributeValue("TxnHeaderId", txnHeaderId);
							lineItemDiscount.SetAttributeValue("TxnDate", historyLineItemDiscount.GetAttributeValue("TxnDate"));
							lineItemDiscount.SetAttributeValue("TxnDetailId", historyLineItemDiscount.GetAttributeValue("TxnDetailId"));
							lineItemDiscount.SetAttributeValue("TxnDiscountId", historyLineItemDiscount.GetAttributeValue("TxnDiscountId"));
							lineItemDiscount.SetAttributeValue("DiscountType", historyLineItemDiscount.GetAttributeValue("DiscountType"));
							lineItemDiscount.SetAttributeValue("DiscountAmount", historyLineItemDiscount.GetAttributeValue("DiscountAmount"));
							lineItemDiscount.SetAttributeValue("TxnChannel", historyLineItemDiscount.GetAttributeValue("TxnChannel"));
							lineItemDiscount.SetAttributeValue("OfferCode", historyLineItemDiscount.GetAttributeValue("OfferCode"));
							lstTxnLineItemDiscount.Add(lineItemDiscount);
						}
					}

					//Get HistoryTxnTender for TxnTender
					lwCriteria = new LWCriterion("HistoryTxnTender");
					lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txnHeaderId, LWCriterion.Predicate.Eq);
					IList<IClientDataObject> lstHistoryTxnTenders = loyalty.GetAttributeSetObjects(null, "HistoryTxnTender", lwCriteria, null, false);

					foreach (IClientDataObject historyTxntender in lstHistoryTxnTenders)
					{
						IClientDataObject txnTender = DataServiceUtil.GetNewClientDataObject("TxnTender");
						txnTender.SetAttributeValue("TxnTenderId", historyTxntender.GetAttributeValue("TxnTenderId"));
						txnTender.SetAttributeValue("TxnHeaderId", historyTxntender.GetAttributeValue("TxnHeaderId"));
						txnTender.SetAttributeValue("TxnDate", historyTxntender.GetAttributeValue("TxnDate"));
						txnTender.SetAttributeValue("StoreId", historyTxntender.GetAttributeValue("StoreId"));
						txnTender.SetAttributeValue("TenderType", historyTxntender.GetAttributeValue("TenderType"));
						txnTender.SetAttributeValue("TenderAmount", historyTxntender.GetAttributeValue("TenderAmount"));
						txnTender.SetAttributeValue("TenderCurrency", historyTxntender.GetAttributeValue("TenderCurrency"));
						txnTender.SetAttributeValue("TenderTaxAmount", historyTxntender.GetAttributeValue("TenderTax"));
						txnTender.SetAttributeValue("TenderTaxRate", historyTxntender.GetAttributeValue("TenderTaxRate"));
						lstTxnTender.Add(txnTender);
					}

					//Get HistoryTxnRewardRedeem data for TxnRewardReedem 
					lwCriteria = new LWCriterion("HistoryTxnRewardRedeem");
					lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txnHeaderId, LWCriterion.Predicate.Eq);
					IList<IClientDataObject> lstHistoryTxnrewardRedeems = loyalty.GetAttributeSetObjects(null, "HistoryTxnRewardRedeem", lwCriteria, null, false);
					foreach (IClientDataObject historyRewardRedeem in lstHistoryTxnrewardRedeems)
					{
						IClientDataObject txnRewardRedeem = DataServiceUtil.GetNewClientDataObject("TxnRewardRedeem");
						txnRewardRedeem.SetAttributeValue("TxnHeaderId", historyRewardRedeem.GetAttributeValue("TxnHeaderId"));
						txnRewardRedeem.SetAttributeValue("TxnDate", historyRewardRedeem.GetAttributeValue("TxnDate"));
						txnRewardRedeem.SetAttributeValue("TxnDetailId", historyRewardRedeem.GetAttributeValue("TxnDetailId"));
						txnRewardRedeem.SetAttributeValue("ProgramId", historyRewardRedeem.GetAttributeValue("ProgramId"));
						txnRewardRedeem.SetAttributeValue("CertificateRedeemType", historyRewardRedeem.GetAttributeValue("CertificateRedeemType"));
						txnRewardRedeem.SetAttributeValue("CertificateCode", historyRewardRedeem.GetAttributeValue("CertificateCode"));
						txnRewardRedeem.SetAttributeValue("CertificateDiscountAmount", historyRewardRedeem.GetAttributeValue("CertificateDiscountAmount"));
						lstTxnRewardRedeem.Add(txnRewardRedeem);
					}

					//Adding TxnRewardRedeem Attribute in TxnHeader Object
					foreach (var txr in lstTxnRewardRedeem)
					{
						txnHeader.AddChildAttributeSet(txr);
					}
					//Adding TxnTender Attribute in TxnHeader Object
					foreach (var txr in lstTxnTender)
					{
						txnHeader.AddChildAttributeSet(txr);
					}
					//Adding TxnDetailItem Attribute in TxnHeader Object
					foreach (var txr in lstTxnDetailItem)
					{
						txnHeader.AddChildAttributeSet(txr);
					}
					//Adding TxnLineItemDiscount Attribute in TxnHeader Object
					foreach (var txr in lstTxnLineItemDiscount)
					{
						txnHeader.AddChildAttributeSet(txr);
					}
					//Adding TxnHeader attribute in Primary Card
					card.AddChildAttributeSet(txnHeader);

					//Saving the member
					loyalty.SaveMember(member);
					//Saving HistoryTxnDetail with ProcessId = RequestCreditApplied
					foreach (var htd in lstHistoryTxnDetail)
					{
						htd.SetAttributeValue("ProcessId", (Int32)ProcessCode.RequestCreditApplied);
						htd.SetAttributeValue("IpCode", member.IpCode);
						htd.SetAttributeValue("TxnLoyaltyId", card.LoyaltyIdNumber);
						loyalty.SaveAttributeSetObject(htd);
					}

					// get points earned                
					long[] vckeys = new long[] { card.VcKey };
					AttributeSetMetaData meta = loyalty.GetAttributeSetMetaData("TxnHeader");
					long[] rowkeys = new long[] { txnHeader.MyKey };
					decimal hbalance = loyalty.GetPointBalance(vckeys, null, null, null, null, null, null, null, null, null, PointTransactionOwnerType.AttributeSet, meta.ID, rowkeys);
					// now get the point balance for the details.
					IList<IClientDataObject> details = txnHeader.GetChildAttributeSets("TxnDetailItem");
					rowkeys = new long[details.Count];
					var keys = (from x in details select x.MyKey);
					meta = loyalty.GetAttributeSetMetaData("TxnDetailItem");
					decimal dbalance = loyalty.GetPointBalance(vckeys, null, null, null, null, null, null, null, null, null, PointTransactionOwnerType.AttributeSet, meta.ID, keys.ToArray<long>());
					return hbalance + dbalance;
				}
				catch (Exception ex)
				{
					_logger.Error(_className, methodName, ex.Message, ex);
					throw;
				}
			}
        }
        #endregion

        #endregion
    }
}