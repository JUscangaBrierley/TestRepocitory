using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;

using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
// AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
//using Brierley.LoyaltyWare.LWIntegration.Common;
using Brierley.FrameWork.LWIntegration;
// AEO-74 Upgrade 4.5 changes END here -----------SCJ
using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;
using Brierley.FrameWork.Common;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Interceptors
{
    public class AmericanEagleTLogInterceptor : AmericanEagleInboundInterceptorBase
    {
        private LWLogger _logger = LWLoggerManager.GetLogger("AmericanEagleInterceptors");
        private const string _className = "AmericanEagleTLogInterceptor";
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private bool IsBraPurchase = false;
        private bool IsJeanPurchase = false;

        public override Member ProcessMemberBeforePopulation(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            return member;
        }

        public override Member ProcessMemberBeforeSave(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            bool bCheckItemBrand = false;

            // get the pointType for bonus points.
            // This will be used to determine if a return should also remove a bonus
            PointType pointType = null;
            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                pointType = lwService.GetPointType("Bonus Points");
            }
            _logger.Trace(_className, methodName, "Begin");
            DateTime? dtLastActiviyDate = member.LastActivityDate;

            IList<IClientDataObject> clientObjects = member.GetChildAttributeSets("MemberDetails");
            Console.WriteLine("Processing IPCode: " + member.IpCode.ToString());

            IList<MemberDetails> memberDetails = new List<MemberDetails>();
            MemberDetails memberDetail = (MemberDetails)clientObjects[0];

            IList<IClientDataObject> returnDetails = new List<IClientDataObject>();

            foreach (VirtualCard vc in member.LoyaltyCards)
            {
                if (vc.IsDirty)
                {
                    IList<IClientDataObject> hdrs = vc.GetChildAttributeSets("TxnHeader");
                    if (hdrs != null && hdrs.Count > 0)
                    {
                        Console.WriteLine("Member header count: " + hdrs.Count.ToString());
                        _logger.Trace("Found " + hdrs.Count + " headers in loyalty card " + vc.LoyaltyIdNumber);
                        _logger.Debug(_className, methodName, "Found " + hdrs.Count + " headers in loyalty card " + vc.LoyaltyIdNumber);
                    }
                    else
                    {
                        _logger.Trace("No headers found for loyalty card " + vc.LoyaltyIdNumber);
                        _logger.Debug(_className, methodName, "No headers found for loyalty card " + vc.LoyaltyIdNumber);
                    }
                    foreach (IClientDataObject hdr in hdrs)
                    {
                        bool bMemberHasBrand = false;

                        TxnHeader txnHeader = (TxnHeader)hdr;

                        _logger.Trace("Get TxnDetailItem.....");

                        _logger.Trace(_className, methodName, "Processing TxnHeaderId " + txnHeader.TxnHeaderId.ToString());

                        _logger.Trace("Processing TxnHeaderId " + txnHeader.TxnHeaderId.ToString());
                        _logger.Debug(_className, methodName, "Processing TxnHeaderId " + txnHeader.TxnHeaderId.ToString());

                        IList<IClientDataObject> dtls = txnHeader.GetChildAttributeSets("TxnDetailItem");

                        List<TxnDetailItem> lstTxnDetailItem = new List<TxnDetailItem>();

                        if (dtls != null && dtls.Count > 0)
                        {
                            if (dtLastActiviyDate == null)
                                dtLastActiviyDate = txnHeader.TxnDate;

                            if (dtLastActiviyDate < txnHeader.TxnDate)
                                dtLastActiviyDate = txnHeader.TxnDate;

                            long lStoreBrandID = 0;

                           
                           //   if (txnHeader.TxnTypeId == 1)
                            if (txnHeader.TxnTypeId == 1 || txnHeader.TxnTypeId == 4) //PI 25410 include Found Orders
                            {
                                

                                _logger.Trace("Txn is a Purchase: iHdrTxnTypeId == 1 or 4 ");
                                #region SetBrandFlags by Store
                                // get the store information so we can see if the member has that brand. 
                                // If the member does not have that brand, then add it.
                                _logger.Trace("Get stores");

                                StoreDef store;
                                using (var cService = _dataUtil.ContentServiceInstance())
                                {
                                    store = cService.GetStoreDef(txnHeader.TxnStoreId);
                                }
                                if (null != store)
                                {
                                    try
                                    {
                                        /*FYI Store.BrandName = BrandID from the ats_RefBrand
                                         * When the store file is loaded from the external table, 
                                         * the StoreDef.BrandName = A_BrandID from ATS_RefBrand 
                                         * based on BrandName
                                         * */
                                        lStoreBrandID = Convert.ToInt64(store.BrandName);

                                        LWCriterion critRefBrand = new LWCriterion("RefBrand");
                                        critRefBrand.Add(LWCriterion.OperatorType.AND, "BrandID", lStoreBrandID.ToString(), LWCriterion.Predicate.Eq);
                                        //IList<IClientDataObject> refBrands = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "RefBrand", critRefBrand, new LWQueryBatchInfo(), false);
                                        // AEO-74 Upgrade 4.5 here -----------SCJ
                                        IList<IClientDataObject> refBrands;
                                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                                        {
                                            refBrands = lwService.GetAttributeSetObjects(null, "RefBrand", critRefBrand, null, false);
                                        }

                                        RefBrand refBrand = null;

                                        _logger.Trace("refBrands.Count = " + refBrands.Count.ToString());

                                        if ((refBrands != null) && (refBrands.Count > 0))
                                        {
                                            refBrand = (RefBrand)refBrands[0];

                                            int iBrandNumber = -1;
                                            int.TryParse(refBrand.BrandNumber.ToString(), out iBrandNumber);

                                            // we need to set the member brand flags at the item level
                                            // for Side By Side, ae.com, 77 Kids online brands.
                                            // These brands will have a negative (-) BrandNumber

                                            // AEREWARD$,aerie a-list, and 77kids kidcard brands will be 
                                            // set at the store level if the store has a valid brand
                                            // These brans will have a (+) positive BrandNumber

                                            if (iBrandNumber >= 0)
                                            {
                                                //bCheckItemBrand = false;            /* redesign changes     SCJ  */
                                                bCheckItemBrand = true;
                                                _logger.Trace("Store brand found. We can skip over the Item Check");
                                            }
                                            else
                                            {
                                                _logger.Trace("Store brand found but we need to check the item leve due to the store's BrandNumber is negative");
                                                bCheckItemBrand = true;
                                            }
                                        }
                                        else
                                        {
                                            _logger.Trace("Store brand NOT found. Check at the item level ");
                                            // cant find the brand based on the brandid so check at the item level
                                            bCheckItemBrand = true;
                                        }
                                    }
                                    catch
                                    {
                                        _logger.Trace("No BrandName found for Store = " + store.StoreNumber + ". We will need to check the item level...");
                                        _logger.Debug(_className, methodName, "No BrandName found for Store = " + store.StoreNumber);
                                        bCheckItemBrand = true;
                                    }

                                    // if we need to check the item brands, skip over store branding
                                    #region Check for Item brands
                                    if (!bCheckItemBrand)
                                    {
                                        if (lStoreBrandID > 0)
                                        {
                                            _logger.Trace("Get memberBrands.....");
                                            // check if the member already has the brand, if not attempt to add it
                                            IList<IClientDataObject> memberBrands = member.GetChildAttributeSets("MemberBrand");
                                            _logger.Trace("memberBrands retrieved.....");
                                            foreach (IClientDataObject obj in memberBrands)
                                            {
                                                MemberBrand memberBrand = (MemberBrand)obj;

                                                if (memberBrand.BrandID == lStoreBrandID)
                                                {
                                                    _logger.Trace("memberBrand.BrandID == lStoreBrandID");
                                                    bMemberHasBrand = true;

                                                    break;
                                                }
                                            }

                                            // if the member does not have the brand, then add it.
                                            if (!bMemberHasBrand)
                                            {
                                                _logger.Trace("Add BrandName found for Store = " + store.StoreNumber);
                                                _logger.Debug(_className, methodName, "Add BrandName found for Store = " + store.StoreNumber);

                                                MemberBrand memberBrand = new MemberBrand();
                                                memberBrand.BrandID = lStoreBrandID;
                                                memberBrand.ChangedBy = "Tlog Processor";
                                                memberBrand.CreateDate = DateTime.Now;
                                                _logger.Trace("member.AddChildAttributeSet(memberBrand)");

                                                // save the member brand. 
                                                // On the next interation, the memberBrands object will contain the newly added brand
                                                member.AddChildAttributeSet(memberBrand);
                                            }
                                        }
                                        else
                                            bCheckItemBrand = true;
                                    }
                                    #endregion
                                }
                                else
                                    bCheckItemBrand = true;

                                #endregion
                                
                            }
                            // PI31366-PI29796- removing bonus event return from tlog processing  begins here --------- SCJ
                            //else if (txnHeader.TxnQualPurchaseAmt < 0)
                            //{
                               #region Handle Bonus Event Returns
                            //    _logger.Trace("Txn has negative points");
                            //    _logger.Trace("Checking for bonus returns.....");

                            //    PointType bonusPointType = null;
                            //    PointEvent bonusPointEvent = null;
                            //    bonusPointType = LWDataServiceUtil.DataServiceInstance(true).GetPointType("Bonus Points");
                            //    bonusPointEvent = LWDataServiceUtil.DataServiceInstance(true).GetPointEvent("Bonus Points");

                            //    if (bonusPointType != null)
                            //    {
                            //        if (txnHeader.TxnOriginalTxnRowKey != null && txnHeader.TxnOriginalTxnRowKey > 0)
                            //        {
                            //            LWCriterion critTxnHeader = new LWCriterion("TxnHeader");
                            //            critTxnHeader.Add(LWCriterion.OperatorType.AND, "RowKey", txnHeader.TxnOriginalTxnRowKey, LWCriterion.Predicate.Eq);
                            //            IList<IClientDataObject> origTxnHeaders = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "TxnHeader", critTxnHeader, new LWQueryBatchInfo(), false);
                            //            _logger.Trace("origTxnHeaders.Count = " + origTxnHeaders.Count.ToString() + " txnHeader.TxnOriginalTxnRowKey = " + txnHeader.TxnOriginalTxnRowKey.ToString());
                            //            IList<PointTransaction> pointTxns;

                            //            foreach (IClientDataObject obj in origTxnHeaders)
                            //            {
                            //                TxnHeader origTxnHeader = (TxnHeader)obj;
                            //                long[] vckeys = { vc.VcKey };
                            //                long[] rowkeys = { origTxnHeader.RowKey };
                            //                pointTxns = _dataService.GetPointTransactionsByRowkeys(vckeys, PointBankTransactionType.Credit, PointTransactionOwnerType.AttributeSet, 101, rowkeys, true);
                            //                _logger.Trace("pointTxns.Count = " + pointTxns.Count.ToString());

                            //                bool isBonusTxns = false;
                            //                foreach (PointTransaction point in pointTxns)
                            //                {
                            //                    // Check to see if the points are bonus points
                            //                    string pointTypeName = LWDataServiceUtil.DataServiceInstance(true).GetPointType(point.PointTypeId).Name;
                            //                    //if (point.Points > 0 && pointTypeName == "Bonus Points")
                            //                    //PI30364  Changes begin here ----------------------------------------------------------------SCJ 
                            //                    string pointEventName = LWDataServiceUtil.DataServiceInstance(true).GetPointEvent(point.PointEventId).Name;
                            //                    if (point.Points > 0 && pointTypeName == "Bonus Points" && pointEventName != "Dollar Reward Tier Purchase Bonus")
                            //                    //PI30364  Changes end here ----------------------------------------------------------------SCJ
                            //                    {
                            //                        isBonusTxns = true;
                            //                    }
                            //                }

                            //                // If the original purchase header got any bonus points then deduct bonus points on returns
                            //                if (isBonusTxns)
                            //                {
                            //                    _logger.Trace("dtls.Count = " + dtls.Count.ToString());
                            //                    double dPointsToRemove = 0.0;

                            //                    // Iterate through all the returned items
                            //                    foreach (TxnDetailItem dtl in dtls)
                            //                    {
                            //                        // Accumulate all the bonus deductions per return txn header
                            //                        dPointsToRemove = dPointsToRemove + (Math.Ceiling(dtl.DtlSaleAmount));
                            //                    }

                            //                    if (dPointsToRemove != 0.0)
                            //                    {
                            //                        bool bIsSuccessful = false;
                            //                        // txn is a bonus so remove the bonus
                            //                        _logger.Trace("Bonus points being deducted for return header = " + dPointsToRemove.ToString());
                            //                        bIsSuccessful = Global.Utilities.AddBonusPoints(vc, bonusPointType.Name, bonusPointEvent.Name, dPointsToRemove);

                            //                        if (!bIsSuccessful)
                            //                        {
                            //                            string strError = "\nError Returning a bonus.";
                            //                            strError += "\nLoyaltyIdNumber = " + vc.LoyaltyIdNumber;
                            //                            strError += "\nReturn TxnHeaderId = " + txnHeader.TxnHeaderId.ToString();
                            //                            strError += "\nReturn Txn Points = " + txnHeader.TxnQualPurchaseAmt.ToString();
                            //                            strError += "\nOrig TxnHeaderId = " + origTxnHeader.TxnHeaderId.ToString();
                            //                            strError += "\nOrig Txn Points = " + origTxnHeader.TxnQualPurchaseAmt.ToString();
                            //                            strError += "\nAttempted to remove " + dPointsToRemove.ToString() + " Bonus points from member ";
                            //                            _logger.Trace(strError);
                            //                            _logger.Debug(_className, methodName, strError);
                            //                            _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, strError);
                            //                        }
                            //                    }
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            string strError = "\nError Returning a bonus.";
                            //            strError += "\ntxnHeader.TxnOriginalTxnRowKey is NULL or 0";
                            //            strError += "\ntxnHeader.TxnOriginalTxnRowKey = " + txnHeader.TxnOriginalTxnRowKey.ToString();
                            //            _logger.Trace(strError);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        string strError = "\nError Returning a bonus.";
                            //        strError += "\nSkipping the code to process a bonus return...";
                            //        strError += "\nLoyaltyIdNumber = " + vc.LoyaltyIdNumber;
                            //        strError += "\nPoint Type was not found for : ";
                            //        strError += "\npointType = LWDataServiceUtil.DataServiceInstance(true).GetPointType(\"Bonus Points\");";
                            //        _logger.Trace(strError);
                            //        _logger.Debug(_className, methodName, strError);
                            //        _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, strError);
                            //    }
                                #endregion
                            //}
                            // PI31366-PI29796- removing bonus event return from tlog processing  end here --------- SCJ
                            #region Process Txn Details
                            foreach (IClientDataObject dtl in dtls)
                            {
                                TxnDetailItem txnDetailItem = (TxnDetailItem)dtl;

                                lstTxnDetailItem.Add(txnDetailItem);

                                _logger.Trace("Looking at the details.....");

                                _logger.Trace("txnDetailItem.DtlTypeId = " + txnDetailItem.DtlTypeId.ToString());

                                if (txnDetailItem.DtlTypeId > 0)
                                {
                                    _logger.Trace("Gift card items....");

                                    if (txnDetailItem.DtlSaleAmount > 0 && txnDetailItem.DtlClassCode == "9911" && txnHeader.TxnDate != null)
                                    {
                                        if (this.CheckForGiftCardItemsIn24Hours(member, txnHeader.TxnDate) >= 3)
                                        {
                                            txnDetailItem.DtlSaleAmount = 0;
                                        }
                                    }

                                    _logger.Trace("Get products.....");
                                    if (bCheckItemBrand)
                                    {
                                        Product product = Utilities.GetProduct((long)txnDetailItem.DtlProductId);

                                        #region SetBrandFlags by item

                                        if (product != null)
                                        {
                                            // CHECK THE ITEMTYPE = 1
                                            // IF = 1 THEN CHECK THE bCheckItemBrand IN ORDER TO ADD A BRAND

                                            _logger.Trace("bCheckItemBrand = " + bCheckItemBrand.ToString());
                                            if (txnDetailItem.DtlTypeId == 1)
                                            {
                                                _logger.Trace("txnDetailItem.DtlTypeId == 1");
                                                long lDtlBrandID = 0;

                                                long.TryParse(product.BrandName, out lDtlBrandID);

                                                _logger.Trace("lDtlBrandID = " + lDtlBrandID.ToString());
                                                // check if the member already has the brand, if not attempt to add it
                                                _logger.Trace("Get memberBrands.....");
                                                IList<IClientDataObject> memberBrands = member.GetChildAttributeSets("MemberBrand");
                                                _logger.Trace("memberBrands retrieved");
                                                _logger.Trace("memberBrands.Count = " + memberBrands.Count.ToString());
                                                foreach (IClientDataObject obj in memberBrands)
                                                {
                                                    MemberBrand memberBrand = (MemberBrand)obj;
                                                    bMemberHasBrand = false; //PI25417-bug fix changes for brand flag not reset 
                                                    if (memberBrand.BrandID == lDtlBrandID)
                                                    {
                                                        _logger.Trace("memberBrand.BrandID == lDtlBrandID");
                                                        bMemberHasBrand = true;
                                                        break;
                                                    }

                                                }
                                                
                                                // if the member does not have the brand, then add it.
                                                if (!bMemberHasBrand)
                                                {
                                                    _logger.Debug(_className, methodName, "Add BrandID found for Product = " + txnDetailItem.DtlProductId.ToString());

                                                    MemberBrand memberBrand = new MemberBrand();
                                                    memberBrand.BrandID = lDtlBrandID;
                                                    memberBrand.ChangedBy = "Tlog Processor";
                                                    memberBrand.CreateDate = DateTime.Now;
                                                    _logger.Trace("member.AddChildAttributeSet(memberBrand);");
                                                    member.AddChildAttributeSet(memberBrand);
                                                }
                                            }
                                        #endregion

                                        }
                                        else
                                        {
                                            string strError = "0 records found for ProductSKU " + txnDetailItem.DtlProductId.ToString();
                                            _logger.Trace(strError);
                                            _logger.Debug(_className, methodName, strError);
                                            _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, strError);
                                        }
                                    }
                                }
                                else
                                {
                                    _logger.Trace("No DtlTypeCode found.");
                                    _logger.Debug(_className, methodName, "No DtlTypeCode found.");
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            _logger.Trace("TxnDetailItem.Pulled with 0 details");
                            _logger.Debug(_className, methodName, "No details found for header " + txnHeader.GetAttributeValue("TxnHeaderId"));
                        }

                        try
                        {
                            // the below line will check if the txn contains an item for one of the promotions 
                            // and will then add it to ATS_MemberPromoQueue if it does.
                            // The ATS_MemberPromoQueue will be used by the Promotion packages to pull its
                            // candidate list based on the ATS_MemberPromoQueue.PromotionType
                            AEPromo.CheckForPromo(lstTxnDetailItem, txnHeader, member, "tlog", out IsBraPurchase, out IsJeanPurchase);
                            if (IsBraPurchase)
                            {
                                if ((txnHeader.TxnTypeId == 1) || (txnHeader.TxnTypeId == 4)) //PI24327 changes
                                {
                                    //On Purchases, populate LastBraStore number and BraPurchseDate
                                    memberDetail.LastBraStoreNumber = txnHeader.StoreNumber; //PI24327 changes
                                    memberDetail.LastBraPurchaseDate = txnHeader.TxnDate; //PI24327 changes
                                    if (memberDetail.BraFirstPurchaseDate == null)
                                    {
                                        memberDetail.BraFirstPurchaseDate = txnHeader.TxnDate;
                                    }
                                }
                            }
                            if (IsJeanPurchase)
                            {
                                if ((txnHeader.TxnTypeId == 1) || (txnHeader.TxnTypeId == 4))
                                {
                                    memberDetail.LastJeansPurchaseDate = txnHeader.TxnDate;
                                    memberDetail.StoreLastJeansPurchased = txnHeader.StoreNumber.HasValue ? txnHeader.StoreNumber.Value.ToString() : " ";
                                    if (memberDetail.JeansFirstPurchaseDate == null)
                                    {
                                        memberDetail.JeansFirstPurchaseDate = txnHeader.TxnDate;
                                    }
                                }
                            }
                            // On All purchases and found order
                            if ((txnHeader.TxnTypeId == 1) || (txnHeader.TxnTypeId == 4))
                            {
                                // Check for positive points
                                if ((long)txnHeader.TxnQualPurchaseAmt >= 0)
                                {
                                    memberDetail.LastPurchasePoints = (long)txnHeader.TxnQualPurchaseAmt; //PI24327 changes
                                }
                                else // incase of negative points, move zero
                                {
                                    memberDetail.LastPurchasePoints = 0;
                                }
                            }

                            if (!string.IsNullOrEmpty(txnHeader.TxnEmployeeId) && (!memberDetail.EmployeeCode.HasValue || memberDetail.EmployeeCode.Value == 2))
                            {
                                memberDetail.EmployeeCode = Convert.ToInt32(txnHeader.TxnEmployeeId);
                            }

                            if (dtLastActiviyDate == null)
                            {
                                member.LastActivityDate = txnHeader.TxnDate;
                                member.ChangedBy = "Tlog DAP Processor";    
                            }
                            else if (dtLastActiviyDate <= txnHeader.TxnDate)
                            {
                                member.LastActivityDate = txnHeader.TxnDate;
                                member.ChangedBy = "Tlog DAP Processor";
                            }
                        }
                        catch (Exception ex)
                        {
                            string strError = string.Empty;
                            strError += "Failed to load txn to the ATS_MemberPromoQueue";
                            strError += "TxnHeaderId = " + txnHeader.TxnHeaderId.ToString() + " StoreNumber = " + txnHeader.StoreNumber.ToString() + "TxnDate = " + txnHeader.TxnDate.ToShortDateString() + "TxnNumber = " + txnHeader.TxnNumber + "TxnRegisterNumber = " + txnHeader.TxnRegisterNumber;
                            strError += "\nex.Message = " + ex.Message;
                            strError += "\n\nex.StackTrace = " + ex.StackTrace;
                            _logger.Trace(strError);
                            _logger.Debug(_className, methodName, strError);
                            _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, strError);
                        }
                    }


                    // Nullify the PrimaryEmailAddress of member object after assigning it to EmailAddress of member details object
                    if (!string.IsNullOrEmpty(member.PrimaryEmailAddress))
                    {
                        memberDetail.EmailAddress = member.PrimaryEmailAddress;
                        member.PrimaryEmailAddress = null;
                    }

                }
            }

            Console.WriteLine("Member Processed......");

            return member;
        }

        /// <summary>
        /// PI 27532 - Check TxnDetailItem table to see if a member has more than 3 Gift Card items in a 24 hour period.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="txnDate"></param>
        /// <param name="txnHeader"></param>
        /// <returns></returns>
        private int CheckForGiftCardItemsIn24Hours(Member member, DateTime txnDate)
        {
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            int numberOfTxns = 0;
            //double zeroAmt = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
            decimal zeroAmt = 0;

            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    // Get the total gift card items for the member
                    LWCriterion crit = new LWCriterion("TxnDetailItem");
                    foreach (VirtualCard vc in member.LoyaltyCards)
                    {
                        crit.Add(LWCriterion.OperatorType.AND, "VcKey", vc.VcKey, LWCriterion.Predicate.Eq);
                    }
                    crit.Add(LWCriterion.OperatorType.AND, "DtlClassCode", "9911", LWCriterion.Predicate.Eq);
                    crit.Add(LWCriterion.OperatorType.AND, "TxnDate", DateTime.Parse(txnDate.ToShortDateString()), LWCriterion.Predicate.Ge);
                    crit.Add(LWCriterion.OperatorType.AND, "TxnDate", DateTime.Parse(txnDate.ToShortDateString()).AddHours(24).AddSeconds(-1), LWCriterion.Predicate.Le);
                    crit.Add(LWCriterion.OperatorType.AND, "DtlSaleAmount", zeroAmt, LWCriterion.Predicate.Gt);
                    //IList<IClientDataObject> txnDetailItems = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "TxnDetailItem", crit, new LWQueryBatchInfo(), false); // AEO-74 Upgrade 4.5 here -----------SCJ
                    IList<IClientDataObject> txnDetailItems = lwService.GetAttributeSetObjects(null, "TxnDetailItem", crit, null, false);
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "TxnDetailItemRecords: " + txnDetailItems.Count.ToString());
                    numberOfTxns += txnDetailItems.Count;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return numberOfTxns;
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public override Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode)
        public override Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode, IList<Brierley.FrameWork.ContextObject.RuleResult> results = null)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            //Issue Member Rewards, not yet tested in QA
            //IssueMemberRewards(member);

            //Update/Insert data in history tables
            UpdateHistoryData(member);

            return member;
        }

        public void IssueMemberRewards(Member member)
        {

        }

        public virtual void UpdateHistoryData(Member member)
        {

        }

        private IClientDataObject _destination = null;
        private void SetAttributeIfNotNull(IClientDataObject source, string attributeName)
        {
            try
            {
                object o = source.GetAttributeValue(attributeName);
                if (o != null && o != DBNull.Value)
                {
                    _destination.SetAttributeValue(attributeName, o);
                }
            }
            catch
            {
                // Don't throw any exception. Continue on. We should save what we can.
            }
        }
    }
}

