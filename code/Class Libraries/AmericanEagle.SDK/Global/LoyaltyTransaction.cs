using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Global
{
    public class LoyaltyTransaction
    {
        private static readonly LWLogger Logger = LWLoggerManager.GetLogger("LoyaltyTransaction");
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// Adds the loyalty transaction.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="pStrTxnHeaderID">The p STR TXN header ID.</param>
        /// 

        public struct txnResultStruct
        {
            public string StoreNumber;
            public string TxnRegisterNumber;
            //public double TenderAmount; // AEO-74 Upgrade 4.5 changes  here -----------SCJ
            public decimal TenderAmount; // AEO-74 Upgrade 4.5 changes  here -----------SCJ
            public string TxnNumOrderNum;
            public DateTime TxnDate;
            public string TxnLoyaltyID;
        }

        public static void AddLoyaltyTransaction(Member member, String pStrTxnHeaderID, ProcessId processId, string source)
        {
            bool isSuccess = false;
            AddLoyaltyTransaction(member, pStrTxnHeaderID, processId, source, out isSuccess);
        }

        //PI15227 - added isSuccess flag
        public static void AddLoyaltyTransaction(Member member, String pStrTxnHeaderID, ProcessId processId, string source, out bool isSuccess)
        {
            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: " + pStrTxnHeaderID);
            HistoryTxnDetail historyTxnDetail;

            VirtualCard primaryCard;
            isSuccess = false;
            bool IsBraPurchase = false;
            bool IsJeanPurchase = false;

            try
            {
                //Get primary card of member
                primaryCard = GetPrimaryVirtualCard(member);

                //get HistoryTxnDetail for a header
                var lstHeader = new List<TxnHeader>();
                var lstTxnLineItemDiscount = new List<TxnLineItemDiscount>();
                var lstTxnTender = new List<TxnTender>();
                var lstTxnRewardRedeem = new List<TxnRewardRedeem>();
                var lstTxnDetailItem = new List<TxnDetailItem>();
                var txnHeader = new TxnHeader();

                var lwCriteria = new LWCriterion("HistoryTxnDetail");
                lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderID", pStrTxnHeaderID, LWCriterion.Predicate.Eq);
                //var lstHistoryRecords = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "HistoryTxnDetail", lwCriteria, new LWQueryBatchInfo(), false); // AEO-74 Upgrade 4.5 here -----------SCJ
                using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    var lstHistoryRecords = ldService.GetAttributeSetObjects(null, "HistoryTxnDetail", lwCriteria, null, false);

                    var lstHistoryTxnDetail = lstHistoryRecords.Cast<HistoryTxnDetail>().ToList();
                    var tempTxnHeaderID = String.Empty;
                    bool bMemberHasBrand = false;

                    foreach (var historyDetail in lstHistoryTxnDetail.OrderBy(p => p.TxnHeaderID))
                    {
                        historyTxnDetail = historyDetail;
                        //inserting only once in TxnHeader
                        if (tempTxnHeaderID != historyDetail.TxnHeaderID)
                        {
                            //Check to see if this member has more than 3 txns (including this one)
                            //if so then zero out the Qualified Purchase Amount so the txn will get
                            //zero points.
                            //PI20403 - Late Enrollment - first 3 txns within 24 hurs should receive points, and the rest won't.
                            int numTxnGotPoints = 0;
                            numTxnGotPoints = CheckForTxnsIn24Hours(primaryCard.LoyaltyIdNumber, historyDetail.TxnDate, member, source);
                            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "numberOfTxns: " + numTxnGotPoints);

                            if (numTxnGotPoints >= 3)
                            {
                                txnHeader.TxnQualPurchaseAmt = 0;
                            }
                            else
                            {
                                txnHeader.TxnQualPurchaseAmt = historyTxnDetail.TxnQualPurchaseAmt;
                            }

                            txnHeader.OrderNumber = historyTxnDetail.OrderNumber;
                            txnHeader.ShipDate = historyTxnDetail.ShipDate;
                            txnHeader.TxnHeaderId = historyTxnDetail.TxnHeaderID;
                            txnHeader.BrandId = historyTxnDetail.BrandID;
                            txnHeader.CreditCardId = Convert.ToInt32(historyTxnDetail.CreditCardID);
                            txnHeader.TxnMaskId = historyTxnDetail.TxnMaskID;
                            txnHeader.TxnNumber = historyTxnDetail.TxnNumber;
                            txnHeader.TxnDate = historyTxnDetail.TxnDate;
                            txnHeader.TxnRegisterNumber = historyTxnDetail.TxnRegisterNumber;
                            txnHeader.TxnStoreId = Convert.ToInt32(historyTxnDetail.TxnStoreID);
                            txnHeader.TxnTypeId = Convert.ToInt32(historyTxnDetail.TxnType);
                            txnHeader.TxnAmount = historyTxnDetail.TxnAmount;
                            txnHeader.TxnDiscountAmount = historyTxnDetail.TxnDiscountAmount;
                            txnHeader.TxnEmployeeId = historyTxnDetail.TxnEmployeeID;
                            txnHeader.TxnChannel = historyTxnDetail.TxnChannelID;
                            txnHeader.TxnOriginalTxnRowKey = Convert.ToInt32(historyTxnDetail.TxnOriginalTxnRowKey);
                            txnHeader.TxnCreditsUsed = Convert.ToInt32(historyTxnDetail.TxnCreditsUsed);
                            txnHeader.StoreNumber = Int64.Parse(historyDetail.StoreNumber);
                            lstHeader.Add(txnHeader);

                            //PI22020 - populate last purchase date    
                            DateTime? dtLastActiviyDate = member.LastActivityDate;
                            if (dtLastActiviyDate == null)
                                dtLastActiviyDate = txnHeader.TxnDate;

                            if (dtLastActiviyDate < txnHeader.TxnDate)
                                dtLastActiviyDate = txnHeader.TxnDate;

                            if (dtLastActiviyDate != null)
                            {
                                member.LastActivityDate = dtLastActiviyDate;
                                member.ChangedBy = "AddLoyaltyTransaction";
                            }
                        }

                        tempTxnHeaderID = historyDetail.TxnHeaderID;

                        //Adding transaction to TxnDetailItem
                        var txnDetailItem = new TxnDetailItem
                        {
                            TxnHeaderId = historyTxnDetail.TxnHeaderID,
                            TxnDate = historyTxnDetail.TxnDate,
                            TxnStoreId = Convert.ToInt32(historyTxnDetail.TxnStoreID),
                            TxnDetailId = historyTxnDetail.TxnDetailID,
                            DtlClassCode = historyDetail.DtlClassCode,
                            DtlItemLineNbr = Convert.ToInt32(historyTxnDetail.DtlItemLineNbr),
                            DtlProductId = Convert.ToInt32(historyTxnDetail.DtlProductID),
                            DtlTypeId = Convert.ToInt32(historyTxnDetail.DtlTypeID),
                            DtlActionId = Convert.ToInt32(historyTxnDetail.DtlActionID),
                            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                            //DtlRetailAmount = Convert.ToDouble(historyTxnDetail.DtlRetailAmount),
                            //DtlSaleAmount = Convert.ToDouble(historyTxnDetail.DtlSaleAmount),
                            DtlRetailAmount = historyTxnDetail.DtlRetailAmount.HasValue ? historyTxnDetail.DtlRetailAmount.Value : 0,
                            DtlSaleAmount = historyTxnDetail.DtlSaleAmount.HasValue ? historyTxnDetail.DtlSaleAmount.Value : 0,
                            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                            DtlQuantity = Convert.ToInt32(historyTxnDetail.DtlQuantity),
                            DtlDiscountAmount = Convert.ToInt32(historyTxnDetail.DtlDiscountAmount),
                            DtlClearanceItem = Convert.ToInt32(historyTxnDetail.DtlClearanceItem)
                        };

                        lstTxnDetailItem.Add(txnDetailItem);

                        //PI21346 - Adding record to MemberBrand table 
                        bMemberHasBrand = false;
                        if (historyTxnDetail.DtlTypeID == 1 && historyTxnDetail.DtlProductID > 0 && historyTxnDetail.DtlClassCode != null)
                        {
                            IList<IClientDataObject> memberBrands = member.GetChildAttributeSets("MemberBrand");
                            bMemberHasBrand = false;
                            foreach (IClientDataObject obj in memberBrands)
                            {
                                MemberBrand memberBrand = (MemberBrand)obj;
                                if (memberBrand.BrandID == int.Parse(historyTxnDetail.BrandID))
                                {
                                    bMemberHasBrand = true;
                                    break;
                                }
                            }

                            // if the member does not have the brand, then add it.
                            if (!bMemberHasBrand && historyTxnDetail.BrandID != string.Empty)
                            {
                                MemberBrand memberBrand = new MemberBrand();
                                memberBrand.BrandID = int.Parse(historyTxnDetail.BrandID);
                                memberBrand.ChangedBy = "AddLoyaltyTransaction";
                                memberBrand.CreateDate = DateTime.Now;
                                member.AddChildAttributeSet(memberBrand);
                            }
                        }

                        lwCriteria = new LWCriterion("HistoryTxnDetailDiscount");
                        lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", historyTxnDetail.TxnHeaderID, LWCriterion.Predicate.Eq);
                        //lstHistoryRecords = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "HistoryTxnDetailDiscount", lwCriteria, new LWQueryBatchInfo(), false);  // AEO-74 Upgrade 4.5 here -----------SCJ
                        lstHistoryRecords = ldService.GetAttributeSetObjects(null, "HistoryTxnDetailDiscount", lwCriteria, null, false);

                        lstTxnLineItemDiscount.AddRange(from HistoryTxnDetailDiscount historyTxnDetailDiscount in lstHistoryRecords
                                                        select new TxnLineItemDiscount
                                                        {
                                                            TxnHeaderId = historyTxnDetailDiscount.TxnHeaderId,
                                                            TxnDate = historyTxnDetailDiscount.TxnDate,
                                                            TxnDetailId = historyTxnDetailDiscount.TxnDetailId,
                                                            TxnDiscountId = historyTxnDetailDiscount.TxnDiscountId,
                                                            DiscountType = historyTxnDetailDiscount.DiscountType,
                                                            DiscountAmount = historyTxnDetailDiscount.DiscountAmount,
                                                            TxnChannel = historyTxnDetailDiscount.TxnChannel,
                                                            OfferCode = historyTxnDetailDiscount.OfferCode
                                                        });
                    }


                    foreach (var thead in lstHeader)
                    {
                        //Get HistoryTxnTender for TxnTender
                        lwCriteria = new LWCriterion("HistoryTxnTender");
                        lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderID", thead.TxnHeaderId, LWCriterion.Predicate.Eq);
                        //lstHistoryRecords = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "HistoryTxnTender", lwCriteria, new LWQueryBatchInfo(), false);  // AEO-74 Upgrade 4.5 here -----------SCJ

                        lstHistoryRecords = ldService.GetAttributeSetObjects(null, "HistoryTxnTender", lwCriteria, null, false);

                        lstTxnTender.AddRange(from HistoryTxnTender historyTxnTender in lstHistoryRecords
                                              select new TxnTender
                                              {
                                                  TxnTenderId = historyTxnTender.TxnTenderID,
                                                  TxnHeaderId = historyTxnTender.TxnHeaderID,
                                                  TxnDate = historyTxnTender.TxnDate,
                                                  TxnStoreId = Convert.ToInt32(historyTxnTender.StoreID),
                                                  TenderType = Convert.ToInt32(historyTxnTender.TenderType),
                                                  TenderAmount = historyTxnTender.TenderAmount,
                                                  TenderCurrency = historyTxnTender.TenderCurrency,
                                                  // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                                                  //TenderTaxAmount = Convert.ToDouble(historyTxnTender.TenderTax),
                                                  //TenderTaxRate = Convert.ToDouble(historyTxnTender.TenderTaxRate)
                                                  TenderTaxAmount = historyTxnTender.TenderTax.HasValue ? historyTxnTender.TenderTax.Value : 0,
                                                  TenderTaxRate = historyTxnTender.TenderTaxRate.HasValue ? historyTxnTender.TenderTaxRate.Value : 0
                                                  // AEO-74 Upgrade 4.5 changes END here -----------SCJ

                                              });

                        //Get HistoryTxnRewardRedeem data for TxnRewardReedem 
                        lwCriteria = new LWCriterion("HistoryTxnRewardRedeem");
                        lwCriteria.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", thead.TxnHeaderId, LWCriterion.Predicate.Eq);
                        //lstHistoryRecords = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "HistoryTxnRewardRedeem", lwCriteria, new LWQueryBatchInfo(), false);  // AEO-74 Upgrade 4.5 here -----------SCJ
                        lstHistoryRecords = ldService.GetAttributeSetObjects(null, "HistoryTxnRewardRedeem", lwCriteria, null, false);

                        lstTxnRewardRedeem.AddRange(from HistoryTxnRewardRedeem historyTxnRewardRedeem in lstHistoryRecords
                                                    select new TxnRewardRedeem
                                                    {
                                                        TxnHeaderId = historyTxnRewardRedeem.TxnHeaderId,
                                                        TxnDate = historyTxnRewardRedeem.TxnDate,
                                                        TxnDetailId = historyTxnRewardRedeem.TxnDetailId,
                                                        ProgramId = historyTxnRewardRedeem.ProgramId,
                                                        CertificateRedeemType = historyTxnRewardRedeem.CertificateRedeemType,
                                                        CertificateCode = historyTxnRewardRedeem.CertificateCode,
                                                        CertificateDiscountAmount = historyTxnRewardRedeem.CertificateDiscountAmount
                                                    });
                    }

                    foreach (var objHead in lstHeader)
                    {
                        //Adding TxnRewardRedeem Attribute in TxnHeader Object
                        foreach (var txr in lstTxnRewardRedeem)
                        {
                            objHead.AddChildAttributeSet(txr);
                        }
                        //Adding TxnTender Attribute in TxnHeader Object
                        foreach (var txr in lstTxnTender)
                        {
                            objHead.AddChildAttributeSet(txr);
                        }
                        //Adding TxnDetailItem Attribute in TxnHeader Object
                        foreach (var txr in lstTxnDetailItem)
                        {
                            objHead.AddChildAttributeSet(txr);
                        }
                        //Adding TxnLineItemDiscount Attribute in TxnHeader Object
                        foreach (var txr in lstTxnLineItemDiscount)
                        {
                            objHead.AddChildAttributeSet(txr);
                        }
                        //Adding TxnHeader attribute in Primary Card
                        primaryCard.AddChildAttributeSet(objHead);
                    }
                    IList<IClientDataObject> clientObjects = member.GetChildAttributeSets("MemberDetails");
                    Console.WriteLine("Processing IPCode: " + member.IpCode.ToString());

                    IList<MemberDetails> memberDetails = new List<MemberDetails>();
                    MemberDetails memberDetail = (MemberDetails)clientObjects[0];

                    //Check to see if this txn qualifies for a promo.  If it does then add it to the queue
                    //AEPromo.CheckForPromo(lstTxnDetailItem, txnHeader, member, source);
                    AEPromo.CheckForPromo(lstTxnDetailItem, txnHeader, member, "tlog", out IsBraPurchase, out IsJeanPurchase);
                    if (IsBraPurchase)
                    {
                        if (memberDetail.BraFirstPurchaseDate == null)
                        {
                            memberDetail.BraFirstPurchaseDate = txnHeader.TxnDate;
                        }
                    }
                    if (IsJeanPurchase)
                    {
                        if (memberDetail.JeansFirstPurchaseDate == null)
                        {
                            memberDetail.JeansFirstPurchaseDate = txnHeader.TxnDate;
                        }
                    }

                    using (var ldServiceU = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        //Saving the member
                        ldServiceU.SaveMember(member);
                    }
                    //Saving HistoryTxnDetail with ProcessId = RequestCreditApplied
                    foreach (var htd in lstHistoryTxnDetail)
                    {
                        Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Set ProcessID and Loyatlty#: {0}, {1}", htd.TxnHeaderID, primaryCard.LoyaltyIdNumber));
                        htd.ProcessID = (Int32)processId;
                        htd.TxnLoyaltyID = primaryCard.LoyaltyIdNumber;
                        ldService.SaveAttributeSetObject(htd);
                    }

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
            }
            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }


        /// <summary>
        /// PI20403 - Check TxnHeader table to see if a member has more than 3 txns in a 24 hour period.
        /// This method will have to have an end date since the Request Credit process may be processing 
        /// a txn some time in the future and other txns could have come in after the fact.
        /// </summary>
        /// <param name="ipCode"></param>
        /// <param name="txnDate"></param>
        /// <returns></returns>
        private static int CheckForTxnsIn24Hours(string LoyaltyIdNumber, DateTime txnDate, Member member, string source)
        {
            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            int numberOfTxns = 0;

            DateTime previousDate = DateTime.MinValue;
            DateTime nextDate = DateTime.MinValue;
            // double zeroAmt = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            decimal zeroAmt = 0;
            try
            {
                using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    previousDate = txnDate.AddHours(-24);
                    nextDate = txnDate.AddHours(24);

                    LWCriterion crit = new LWCriterion("TxnHeader");
                    crit.Add(LWCriterion.OperatorType.AND, "TxnDate", previousDate, LWCriterion.Predicate.Ge);
                    crit.Add(LWCriterion.OperatorType.AND, "TxnDate", nextDate, LWCriterion.Predicate.Le);
                    crit.Add(LWCriterion.OperatorType.AND, "TxnQualPurchaseAmt", zeroAmt, LWCriterion.Predicate.Gt);

                    foreach (VirtualCard vc in member.LoyaltyCards)
                    {
                        IList<IClientDataObject> txnHeaderRecords = ldService.GetAttributeSetObjects(vc, "TxnHeader", crit, null, false);
                        Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "txnHeaderRecords: " + txnHeaderRecords.Count.ToString());
                        numberOfTxns += txnHeaderRecords.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
            }

            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return numberOfTxns;
        }

        /// <summary>
        /// Gets the primary virtual card.
        /// </summary>
        /// <param name="pMember">The p member.</param>
        /// <returns></returns>
        private static VirtualCard GetPrimaryVirtualCard(Member pMember)
        {
            IList<VirtualCard> vCard = pMember.LoyaltyCards;
            return vCard.FirstOrDefault(vc => vc.IsPrimary);
        }
        /// <summary>
        /// Create member receipt
        /// </summary>
        /// <param name="member">Member</param>
        /// <param name="p_dicSearchParams">Search Params</param>
        /// <param name="pTransactionType">Transaction types:Instore/online</param>
        public static void CreateMemberReceipt(Member member, Dictionary<String, String> p_dicSearchParams, TransactionType pTransactionType, string changedBy, long ReceiptStatusCode)
        {
            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            try
            {
                using (var _dataService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    //save receipt type to database

                    ClientConfiguration clientConfiguration = _dataUtil.DataServiceInstance().GetClientConfiguration("MemberReceiptExpirationDays");
                    Int32 i32MemberReceiptExpirationDays = 0;
                    Int32.TryParse(clientConfiguration.Value, out i32MemberReceiptExpirationDays);

                    MemberReceipts memberReceipts = new MemberReceipts();
                    memberReceipts.CHANGEDBY = changedBy;
                    memberReceipts.CreateDate = DateTime.Today;
                    memberReceipts.StatusCode = (Int32)ReceiptStatusCode;   //PI14342
                    memberReceipts.ExpirationDate = DateTime.Today.AddDays(i32MemberReceiptExpirationDays);//TBD: get member receipt expiration days from client configuration
                    switch (pTransactionType)
                    {
                        case TransactionType.Store:
                        case TransactionType.StoreLookup:
                            memberReceipts.ReceiptType = (long)ReceiptTypes.InStore;

                            if (p_dicSearchParams.ContainsKey("txt_TxnDate") && !String.IsNullOrEmpty(p_dicSearchParams["txt_TxnDate"]))
                            {
                                DateTime dtTxnDate = DateTime.Today;
                                DateTime.TryParse(p_dicSearchParams["txt_TxnDate"], out dtTxnDate);
                                memberReceipts.TxnDate = dtTxnDate;
                            }

                            if (p_dicSearchParams.ContainsKey("txt_TxnNumber") && !String.IsNullOrEmpty(p_dicSearchParams["txt_TxnNumber"]))
                            {
                                string sTxnNumber = p_dicSearchParams["txt_TxnNumber"].Trim();

                                if (sTxnNumber.Length > 1 && sTxnNumber.Length < 6)
                                {
                                    sTxnNumber = sTxnNumber.PadLeft(6, '0');
                                }

                                memberReceipts.TxnNumber = sTxnNumber;
                                Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "memberReceipts.TxnNumber: " + memberReceipts.TxnNumber);
                            }
                            if (p_dicSearchParams.ContainsKey("txt_TxnRegisterNumber") && !String.IsNullOrEmpty(p_dicSearchParams["txt_TxnRegisterNumber"]))
                            {
                                memberReceipts.TxnRegisterNumber = p_dicSearchParams["txt_TxnRegisterNumber"];
                            }
                            if (p_dicSearchParams.ContainsKey("txt_TxnStoreID") && !String.IsNullOrEmpty(p_dicSearchParams["txt_TxnStoreID"]))
                            {
                                //PI21354 - Remove leading zerors from StoreNumber before adding it to DB
                                memberReceipts.TxnStoreID = p_dicSearchParams["txt_TxnStoreID"].TrimStart('0');
                            }
                            if (p_dicSearchParams.ContainsKey("txt_TenderAmount") && !String.IsNullOrEmpty(p_dicSearchParams["txt_TenderAmount"]))
                            {
                                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                                //Double dbl = 0;
                                Decimal dbl = 0;
                                //Double.TryParse(p_dicSearchParams["txt_TenderAmount"], out dbl);
                                Decimal.TryParse(p_dicSearchParams["txt_TenderAmount"], out dbl);
                                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                                memberReceipts.TenderAmount = dbl;
                            }
                            break;
                        case TransactionType.Online:
                        case TransactionType.OnlineLookup:
                            memberReceipts.ReceiptType = (long)ReceiptTypes.Web;


                            if (p_dicSearchParams.ContainsKey("txt_TxnDate") && !String.IsNullOrEmpty(p_dicSearchParams["txt_TxnDate"]))
                            {
                                DateTime dtTxnDate = DateTime.Today;
                                DateTime.TryParse(p_dicSearchParams["txt_TxnDate"], out dtTxnDate);
                                memberReceipts.TxnDate = dtTxnDate;

                            }

                            if (p_dicSearchParams.ContainsKey("txt_OrderNumber") && !String.IsNullOrEmpty(p_dicSearchParams["txt_OrderNumber"]))
                            {
                                memberReceipts.OrderNumber = p_dicSearchParams["txt_OrderNumber"].ToUpper();
                            }
                            if (p_dicSearchParams.ContainsKey("txt_TenderAmount") && !String.IsNullOrEmpty(p_dicSearchParams["txt_TenderAmount"]))
                            {
                                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                                //Double dbl = 0;  
                                Decimal dbl = 0;
                                //Double.TryParse(p_dicSearchParams["txt_TenderAmount"], out dbl);
                                Decimal.TryParse(p_dicSearchParams["txt_TenderAmount"], out dbl);
                                memberReceipts.TenderAmount = dbl;
                                // AEO-74 Upgrade 4.5 changes END here -----------SCJ

                            }
                            break;
                        case TransactionType.Mobile:
                            break;
                        case TransactionType.Social:
                            break;
                        default:
                            break;
                    }
                    member.AddChildAttributeSet(memberReceipts);
                    _dataService.SaveMember(member);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw ex;
            }
        }
        /// <summary>
        /// Load member receipts
        /// </summary>
        /// <param name="_pMember">Member</param>
        /// <returns>List of Member receipts</returns>
        public static List<MemberReceipts> LoadMemberReceipts(Member _pMember)
        {
            List<MemberReceipts> _ListMemberReceipts = new List<MemberReceipts>();
            try
            {
                IList<IClientDataObject> _memberReceipts = _pMember.GetChildAttributeSets("MemberReceipts");

                //PI12219 - GetPoint history show the most current request first
                foreach (MemberReceipts receipt in _memberReceipts.OrderByDescending(x => x.CreateDate))
                {
                    _ListMemberReceipts.Add(receipt);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw ex;
            }
            return _ListMemberReceipts;
        }


        /// <summary>
        /// Method SearchForHistoryTxnDetails
        /// // PI 30364 - Dollar reward program - an extra parameter is added in LoyaltyTransaction.SearchForHistoryTxnDetails() method.
        /// </summary>
        /// <param name="txnNumber">transaction number</param>
        /// <param name="txnStoreID">transaction store ID</param>
        /// <param name="registerNumber">registration number</param>
        /// <param name="txnDate">transaction date</param>
        /// <param name="tenderAmount">tender amount</param>
        /// <param name="orderNumber">order number</param>
        /// <returns>List of History Transaction Details</returns>
        public static IList<IClientDataObject> SearchForHistoryTxnDetails(string txnNumber, string storeNumber, string registerNumber, DateTime? txnDate, string tenderAmount, string orderNumber, Member member)
        {
            string selectedValue = string.Empty;
            // PI 30364 - Dollar reward program - an extra parameter is added in LoyaltyTransaction.SearchForHistoryTxnDetails() method.
            IList<IClientDataObject> historyRecords = SearchForHistoryTxnDetails(txnNumber, storeNumber, registerNumber, txnDate, tenderAmount, orderNumber, selectedValue, member);
            return historyRecords;
        }
        // PI 30364 - Dollar reward program - an extra parameter is added in LoyaltyTransaction.SearchForHistoryTxnDetails() method.
        public static IList<IClientDataObject> SearchForHistoryTxnDetails(string txnNumber, string storeNumber, string registerNumber, DateTime? txnDate, string tenderAmount, string orderNumber, string selectedValue, Member member)
        {
            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            LWCriterion criteria = new LWCriterion("HistoryTxnDetail");

            // If order number is null or empty then search for store transactions else search for web transactions
            if (string.IsNullOrEmpty(orderNumber))
            {
                if (!string.IsNullOrEmpty(registerNumber))
                {
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "TxnRegisterNumber: " + registerNumber);

                    if (!string.IsNullOrEmpty(selectedValue) && (selectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) ||
                        selectedValue.Equals(((int)TransactionType.StoreLookup).ToString())))
                    {
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnRegisterNumber", registerNumber, LWCriterion.Predicate.Eq);
                    }
                    else
                    {
                        criteria.AddDistinct("TxnRegisterNumber");
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnRegisterNumber", registerNumber, LWCriterion.Predicate.Eq);
                    }
                }

                if (!string.IsNullOrEmpty(storeNumber))
                {
                    int istoreNumber = Int32.Parse(storeNumber);
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "StoreNumber: " + istoreNumber);

                    if (!string.IsNullOrEmpty(selectedValue) && (selectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) ||
                        selectedValue.Equals(((int)TransactionType.StoreLookup).ToString())))
                    {
                        criteria.Add(LWCriterion.OperatorType.AND, "StoreNumber", istoreNumber, LWCriterion.Predicate.Eq);
                    }
                    else
                    {
                        criteria.AddDistinct("StoreNumber");
                        criteria.Add(LWCriterion.OperatorType.AND, "StoreNumber", istoreNumber, LWCriterion.Predicate.Eq);
                    }
                }

                // Date comparison, since TxnDate has time and date both so only equal operator will not give any result
                // hence comparing date with in range of one day
                if (txnDate != null)
                {
                    DateTime startDate = (DateTime)txnDate;
                    DateTime endDate = startDate.AddDays(1);

                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "startDate: " + startDate);
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "endDate: " + endDate);

                    if (!string.IsNullOrEmpty(selectedValue) && (selectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) ||
                        selectedValue.Equals(((int)TransactionType.StoreLookup).ToString())))
                    {
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", startDate, LWCriterion.Predicate.Ge);
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", endDate, LWCriterion.Predicate.Le);
                    }
                    else
                    {
                        criteria.AddDistinct("TxnDate");
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", startDate, LWCriterion.Predicate.Ge);
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", endDate, LWCriterion.Predicate.Le);
                    }
                }

                if (!string.IsNullOrEmpty(txnNumber))
                {
                    //PI15162 - LeftPad zerors, if GetPoint txn is not entered with leading zerors
                    string sTxnNumber = txnNumber;

                    if (sTxnNumber.Length > 1 && sTxnNumber.Length < 6)
                    {
                        sTxnNumber = sTxnNumber.PadLeft(6, '0');
                    }

                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "TxnNumber: " + txnNumber);
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "sTxnNumber: " + sTxnNumber);

                    if (!string.IsNullOrEmpty(selectedValue) && (selectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) ||
                        selectedValue.Equals(((int)TransactionType.StoreLookup).ToString())))
                    {
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnNumber", sTxnNumber, LWCriterion.Predicate.Eq);
                    }
                    else
                    {
                        criteria.AddDistinct("TxnNumber");
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnNumber", sTxnNumber, LWCriterion.Predicate.Eq);
                    }
                }
            }
            else
            {
                Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "OrderNumber: " + orderNumber);

                if (!string.IsNullOrEmpty(selectedValue) && (selectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) ||
                     selectedValue.Equals(((int)TransactionType.StoreLookup).ToString())))
                {
                    criteria.Add(LWCriterion.OperatorType.AND, "OrderNumber", orderNumber.ToUpper(), LWCriterion.Predicate.Eq);
                }
                else
                {
                    criteria.AddDistinct("OrderNumber");
                    criteria.Add(LWCriterion.OperatorType.AND, "OrderNumber", orderNumber.ToUpper(), LWCriterion.Predicate.Eq);
                }

                //Need to add the date ranges to speed up the query so the search won't search across date partitions.
                if (member != null)
                {
                    DateTime startDate = DateTime.Now;
                    DateTime endDate = DateTime.Now;

                    Utilities.GetProgramDates(member, out startDate, out endDate);
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "startDate: " + startDate);
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "endDate: " + endDate);

                    if (!string.IsNullOrEmpty(selectedValue) && (selectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) ||
                         selectedValue.Equals(((int)TransactionType.StoreLookup).ToString())))
                    {
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", startDate, LWCriterion.Predicate.Ge);
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", endDate, LWCriterion.Predicate.Le);
                    }
                    else
                    {
                        criteria.AddDistinct("TxnDate");
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", startDate, LWCriterion.Predicate.Ge);
                        criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", endDate, LWCriterion.Predicate.Le);
                    }
                }
            }

            // This criteria will be used for both kind of orders
            if (!string.IsNullOrEmpty(tenderAmount))
            {
                Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "TxnQualPurchaseAmt: " + tenderAmount);

                if (!string.IsNullOrEmpty(selectedValue) && (selectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) ||
                     selectedValue.Equals(((int)TransactionType.StoreLookup).ToString())))
                {
                    criteria.Add(LWCriterion.OperatorType.AND, "TxnQualPurchaseAmt", tenderAmount, LWCriterion.Predicate.Eq);
                }
                else
                {
                    criteria.AddDistinct("TxnQualPurchaseAmt");
                    criteria.Add(LWCriterion.OperatorType.AND, "TxnQualPurchaseAmt", tenderAmount, LWCriterion.Predicate.Eq);
                }
            }
            var lservice = _dataUtil.LoyaltyDataServiceInstance();
            //IList<IClientDataObject> historyRecords = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "HistoryTxnDetail", criteria, new LWQueryBatchInfo(), false);  // AEO-74 Upgrade 4.5 here -----------SCJ
            IList<IClientDataObject> historyRecords = lservice.GetAttributeSetObjects(null, "HistoryTxnDetail", criteria, null, false);
            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "historyRecords.count: " + historyRecords.Count);

            //PI20449 - Ensure Store and Online lookup display distinct rows
            if (!string.IsNullOrEmpty(selectedValue) && (selectedValue.Equals(((int)TransactionType.OnlineLookup).ToString()) ||
                  selectedValue.Equals(((int)TransactionType.StoreLookup).ToString())))
            {
                IList<HistoryTxnDetail> _historyRecords = new List<HistoryTxnDetail>();

                if (historyRecords != null && historyRecords.Count > 0)
                {
                    IList<HistoryTxnDetail> lstHistoryTxnDetailRec = new List<HistoryTxnDetail>();
                    foreach (IClientDataObject obj in historyRecords)
                    {
                        HistoryTxnDetail historyTxnRec = (HistoryTxnDetail)obj;
                        lstHistoryTxnDetailRec.Add(historyTxnRec);
                    }

                    IList<txnResultStruct> lstHistoryRec = new List<txnResultStruct>();
                    IList<txnResultStruct> lstHistoryRecDistinct = new List<txnResultStruct>();

                    foreach (HistoryTxnDetail hisRec in lstHistoryTxnDetailRec)
                    {
                        txnResultStruct txnItemRec = new txnResultStruct();
                        txnItemRec.StoreNumber = hisRec.StoreNumber;
                        txnItemRec.TxnRegisterNumber = hisRec.TxnRegisterNumber;
                        //txnItemRec.TenderAmount = (double)(hisRec.TenderAmount); // AEO-74 Upgrade 4.5 changes  here -----------SCJ
                        // AEO-4222 Store Lookup not working ----- GG
                        //txnItemRec.TenderAmount = hisRec.TenderAmount.HasValue ? hisRec.TenderAmount.Value : 0; 
                        txnItemRec.TenderAmount = hisRec.TxnQualPurchaseAmt.HasValue ? hisRec.TxnQualPurchaseAmt.Value : 0;
                        if (hisRec.OrderNumber != null)
                        {
                            txnItemRec.TxnNumOrderNum = hisRec.OrderNumber;
                        }
                        else
                        {
                            txnItemRec.TxnNumOrderNum = hisRec.TxnNumber;
                        }
                        txnItemRec.TxnDate = hisRec.TxnDate;
                        txnItemRec.TxnLoyaltyID = hisRec.TxnLoyaltyID;
                        lstHistoryRec.Add(txnItemRec);
                    }

                    lstHistoryRecDistinct = lstHistoryRec.Distinct().ToList();

                    foreach (txnResultStruct hisRecDistinct in lstHistoryRecDistinct)
                    {
                        HistoryTxnDetail historyRecDistinct = new HistoryTxnDetail();
                        historyRecDistinct = GetDistinctItemRec(lstHistoryTxnDetailRec, hisRecDistinct);
                        if (historyRecDistinct != null)
                        {
                            _historyRecords.Add(historyRecDistinct);
                        }
                    }
                }
                Logger.Trace("LoyaltyTransaction", "SearchForHistoryTxnDetails", "_historyRecords Distinct count: " + _historyRecords.Count.ToString());
                Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");

                return _historyRecords.Cast<IClientDataObject>().ToList();
            }
            else
            {
                Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                return historyRecords;
            }
        }


        public static HistoryTxnDetail GetDistinctItemRec(IList<HistoryTxnDetail> lstHistoryTxnDetailRec, txnResultStruct hisRecDistinct)
        {
            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            HistoryTxnDetail historyRec = new HistoryTxnDetail();

            foreach (HistoryTxnDetail hisRec in lstHistoryTxnDetailRec)
            {
                if (!hisRec.TxnQualPurchaseAmt.HasValue)
                    hisRec.TxnQualPurchaseAmt = 0;

                if (hisRec.StoreNumber == hisRecDistinct.StoreNumber && hisRec.TxnRegisterNumber == hisRecDistinct.TxnRegisterNumber
                    && hisRec.TxnQualPurchaseAmt == hisRecDistinct.TenderAmount
                    && hisRec.TxnDate == hisRecDistinct.TxnDate && hisRec.TxnLoyaltyID == hisRecDistinct.TxnLoyaltyID)
                {
                    if (hisRec.OrderNumber != null && hisRec.OrderNumber == hisRecDistinct.TxnNumOrderNum)
                    {
                        Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                        return hisRec;
                    }
                    else if (hisRec.OrderNumber == null && hisRec.TxnNumber == hisRecDistinct.TxnNumOrderNum)
                    {
                        Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                        return hisRec;
                    }
                }
            }
            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return null;
        }

        /// <summary>
        /// Method SearchReceiptDetails
        /// </summary>
        /// <param name="txnNumber">transaction number</param>
        /// <param name="txnStoreID">transaction store ID</param>
        /// <param name="registerNumber">registration number</param>
        /// <param name="txnDate">transaction date</param>
        /// <param name="tenderAmount">tender amount</param>
        /// <param name="orderNumber">order number</param>
        /// <returns>int</returns>
        public static int SearchReceiptDetails(string txnNumber, string storeNumber, string registerNumber, DateTime? txnDate, string tenderAmount, string orderNumber)
        {
            //PI14540 - disallow Get Points request for txn that has already been requested
            Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            LWCriterion criteria = new LWCriterion("MemberReceipts");

            // If order number is null or empty then search for store transactions else search for web transactions
            if (string.IsNullOrEmpty(orderNumber))
            {
                if (!string.IsNullOrEmpty(registerNumber))
                {
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "TxnRegisterNumber: " + registerNumber);
                    criteria.Add(LWCriterion.OperatorType.AND, "TxnRegisterNumber", registerNumber, LWCriterion.Predicate.Eq);
                }

                if (!string.IsNullOrEmpty(storeNumber))
                {
                    //PI21354 - Remove leading zeros from StoreNumber for criteria 
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "storeNumber: " + Int32.Parse(storeNumber));
                    criteria.Add(LWCriterion.OperatorType.AND, "TxnStoreID", Int32.Parse(storeNumber), LWCriterion.Predicate.Eq);
                }

                // Date comparison, since TxnDate has time and date both so only equal operator will not give any result
                // hence comparing date with in range of one day
                if (txnDate != null)
                {
                    DateTime startDate = (DateTime)txnDate;
                    DateTime endDate = startDate.AddDays(1);

                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "startDate: " + startDate);
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "endDate: " + endDate);
                    criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", startDate, LWCriterion.Predicate.Ge);
                    criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", endDate, LWCriterion.Predicate.Le);
                }

                if (!string.IsNullOrEmpty(txnNumber))
                {
                    //LeftPad zerors, if GetPoint txn is not entered with leading zerors
                    string sTxnNumber = txnNumber;
                    if (sTxnNumber.Length > 1 && sTxnNumber.Length < 6)
                    {
                        sTxnNumber = sTxnNumber.PadLeft(6, '0');
                    }
                    Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "sTxnNumber: " + sTxnNumber);
                    criteria.Add(LWCriterion.OperatorType.AND, "TxnNumber", sTxnNumber, LWCriterion.Predicate.Eq);
                }

            }
            else
            {
                Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "OrderNumber: " + orderNumber);
                criteria.Add(LWCriterion.OperatorType.AND, "OrderNumber", orderNumber.ToUpper(), LWCriterion.Predicate.Eq);
            }

            // This criteria will be used for both kind of orders
            if (!string.IsNullOrEmpty(tenderAmount))
            {
                Logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "TenderAmount: " + tenderAmount);
                criteria.Add(LWCriterion.OperatorType.AND, "TenderAmount", tenderAmount, LWCriterion.Predicate.Eq);
            }

            //IList<IClientDataObject> lstReceipts = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "MemberReceipts", criteria, new LWQueryBatchInfo(), false); // AEO-74 Upgrade 4.5 here -----------SCJ
            IList<IClientDataObject> lstReceipts;
            using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
            {
                lstReceipts = ldService.GetAttributeSetObjects(null, "MemberReceipts", criteria, null, false);
            }
            MemberReceipts memReceipt = new MemberReceipts();

            if (null != lstReceipts && lstReceipts.Count > 0)
            {
                for (int i = 0; i < lstReceipts.Count; i++)
                {
                    memReceipt = (MemberReceipts)lstReceipts[i];
                    if (memReceipt.StatusCode == (int)ReceiptStatus.Posted)
                    {
                        //This txn has been Posted
                        return (int)(ReceiptStatus.AlreadyPosted);
                    }
                }

                for (int i = 0; i < lstReceipts.Count; i++)
                {
                    memReceipt = (MemberReceipts)lstReceipts[i];
                    if (memReceipt.StatusCode == (int)ReceiptStatus.Processing && memReceipt.ExpirationDate > DateTime.Now)
                    {
                        //This txn has been Requested
                        return (int)(ReceiptStatus.AlreadyRequested);
                    }
                }
            }

            //This txn has never been requested or has been but no match      
            return 0;
        }

    }
}
