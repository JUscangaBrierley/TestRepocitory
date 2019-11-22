using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Email;
using Brierley.Clients.AmericanEagle.DataModel;

using Brierley.FrameWork.Rules.UIDesign;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.CustomRules
{
    class AmericanEagleIssueRewardUtil
    {
        #region Private Fields
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private string className = "RewardRuleUtil";

        private PointsConsumptionOnIssueReward pointsConsumption = PointsConsumptionOnIssueReward.Consume;
        private string emailName = string.Empty;
        private string offerCode = string.Empty;
        private string rewardCertificateBucket = string.Empty;
        private string certificateTypeCodeAttribute = string.Empty;
        private string certificateNmbrAttribute = string.Empty;
        private string certificateStatusAttribute = string.Empty;
        private string lowThresholdEmail = string.Empty;
        private const int ReturnTxnType = 2; // AEO-565
        private const int HoldTxnType = 3; //AEO-1542
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        #endregion

        #region Public Fields
        public enum OfferType
        {
            B5G1 = 1,
            FiveDollar = 2,
            FiveDollarDM = 3,
            B5G1DM  = 4,
            // AEO-592 Begin
            BirthdayEM = 5,
            BirthdayDM = 6,
            BirthdaySM = 7,
            // AEO-592 End
            // AEO-817 begin
            TenDollar = 8,
            TenDollarDM = 9,
            // AEO-817 end
            Stacked5 = 10,
            Stacked10 = 11,
            Stacked15 = 12,
            Stacked20 = 13,
            Stacked25 = 14,
            Stacked30 = 15,
            Stacked35 = 16,
            Stacked40 = 17,
            Stacked45 = 18,
            Stacked50 = 19,
            Stacked55 = 20,
            Stacked60 = 21,
            Stacked65 = 22,
            Stacked70 = 23,
            Stacked75 = 24,
            Stacked80 = 25,
            Stacked85 = 26,
            Stacked90 = 27,
            Stacked95 = 28,
            Stacked100 = 29


        }
        public PointsConsumptionOnIssueReward PointsConsumption
        {
            get
            {
                return pointsConsumption;
            }
            set
            {
                pointsConsumption = value;
            }
        }

        public string RewardCertificateBucket
        {
            get
            {
                return rewardCertificateBucket;
            }
            set
            {
                rewardCertificateBucket = value;
            }
        }

        public string CertificateTypeCodeAttribute
        {
            get
            {
                return certificateTypeCodeAttribute;
            }
            set
            {
                certificateTypeCodeAttribute = value;
            }
        }

        public string CertificateNmbrAttribute
        {
            get
            {
                return certificateNmbrAttribute;
            }
            set
            {
                certificateNmbrAttribute = value;
            }
        }

        public string CertificateStatusAttribute
        {
            get
            {
                return certificateStatusAttribute;
            }
            set
            {
                certificateStatusAttribute = value;
            }
        }


        #endregion

        internal decimal GetPoints(RewardDef rdef, PointType pt, PointEvent pe, Member lwmember, VirtualCard lwvirtualcard)
        {

            // AEO-1441 begin
            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {
                Boolean isJean = rdef.Name.Equals("B5G1 Jean Reward");
                Boolean isBra = rdef.Name.Equals("B5G1 Bra Reward");
                PointEvent specialEvent = isBra ? service.GetPointEvent("Bra Credit Appeasement") :
                                                      isJean ? service.GetPointEvent("Jean Credit Appeasement") : null;

                // AEO-1441 end

                string methodName = "GetPoints";


                List<string> pointTypes = new List<string>();
                List<PointType> pointTypesDef = new List<PointType>();


                DateTime from;
                DateTime to = DateTime.Today;
                //new star date ,that is dap configuration use to query AEO-2114
                from = new DateTime(2012, 1, 1);
                to = new DateTime(to.Year, to.Month, to.Day, 12, 00, 00);

                DateTime returnToDate = DateTime.Today;

                logger.Trace(className, methodName, string.Format("Getting Points for member{0}, from: {1}, to: {2}", lwmember.IpCode, from, to));

                decimal points = 0;
                long[] PtType = new long[1];
                long[] PtEvent = new long[1];
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ

                if (rdef.PointType != "All")
                {
                    pointTypes = new List<string>(rdef.PointType.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));


                    foreach (string pType in pointTypes)
                    {
                        PointType pointType = service.GetPointType(pType);
                        pointTypesDef.Add(pointType);
                        //points += lwmember.GetPoints(pointType, from, to);  //AEO-557 BEgin & end
                    }


                    IList<PointTransaction> pointsList;
                    pointsList = service.GetPointTransactions(lwmember,
                        null, null, null, null, null, null, false);

                    foreach (PointTransaction ptt in pointsList)
                    {

                        // AEO-1925 AH    
                        if (ptt.TransactionType == PointBankTransactionType.Hold ||
                            ptt.PointsOnHold == ptt.Points ||
                            ptt.TransactionType == PointBankTransactionType.Consumed)
                        {
                            // We want to skip transactions that are currently on hold.
                            // It can be assumed that if a transaction has
                            // PointsOnHold > 0 is currently still on hold.
                            continue;
                        }

                        // AEO-1441 begin
                        foreach (PointType pointDef in pointTypesDef)
                        {
                            if (ptt.PointTypeId == pointDef.ID)
                            {
                                points += (ptt.Points - ptt.PointsConsumed);
                            }
                            else
                            {

                                if (isBra || isJean)
                                {
                                    if (specialEvent.ID == ptt.PointEventId)
                                    {
                                        points += (ptt.Points - ptt.PointsConsumed);
                                    }
                                }

                            }
                        }
                        // AEO-1441 end
                    }


                    // AEO-557 END


                }
                else
                {
                    IList<PointType> coll = service.GetAllPointTypes();
                    foreach (PointType ptype in coll)
                    {
                        if (lwvirtualcard == null)
                        {
                            // use all virtual cards
                            //points += lwmember.GetEarnedPoints(ptype, from, to);
                            if (pe == null)
                            {
                                points += lwmember.GetPoints(ptype, from, to);

                            }
                            else
                            {
                                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                                // points += lwmember.GetPoints(ptype, pe, from, to);
                                PtType[0] = pt.ID;
                                PtEvent[0] = pe.ID;

                                points += lwmember.GetPoints(PtType, PtEvent, from, to);
                                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                            }

                            //AEO-258 Begin
                            IList<PointTransaction> pointsList = service.GetPointTransactions(lwvirtualcard, from, to, null, null, false);

                            foreach (PointTransaction ptt in pointsList)
                            {
                                if (ptt.PointTypeId == pt.ID && ptt.TransactionType == (PointBankTransactionType)1 && ptt.PointsConsumed > 0)
                                {
                                    points -= ptt.PointsConsumed;
                                }
                            }
                            //AEO-258 End

                        }
                        else
                        {
                            //TODO: Check with Bill.
                            // use the virtual card being used.
                            //points += lwvirtualCard.GetEarnedPoints(ptype, from, to);
                            if (pe == null)
                            {
                                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                                //points += service.GetPointBalanceByType(lwvirtualcard, ptype, from, to);
                                long[] vcKey = { lwvirtualcard.VcKey };
                                long[] ptypeL = { ptype.ID };
                                points += service.GetPointBalance(vcKey, ptypeL, null, null, from, to, null, null, null, null, null, null, null);
                                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                            }
                            else
                            {
                                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                                //points += service.GetPointBalanceByType(lwvirtualcard, ptype, pe, from, to);
                                long[] vcKey = { lwvirtualcard.VcKey };
                                long[] ptypeL = { ptype.ID };
                                points += service.GetPointBalance(vcKey, ptypeL, null, null, from, to, null, null, null, null, null, null, null);
                                // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                            }
                        }
                    }
                }
                logger.Trace(className, methodName, string.Format("Points for member{0}: {1}", lwmember.IpCode, points));
                return points;
            }
        }

        // AEO-74 Upgrade 4.5 changes here -----------SCJ
        //internal long IssueRewardCertificate(Member lwmember, RewardDef rdef, DateTime expiryDate, RewardFulfillmentOption rewardFulfillmentOption, string certNmbr, int variantId, string orderNumber, double rewardAmount)
        internal long IssueRewardCertificate(Member lwmember, RewardDef rdef, DateTime expiryDate, RewardFulfillmentOption rewardFulfillmentOption, string certNmbr, string offerCode, int variantId, string orderNumber, decimal rewardAmount)
        {
            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {
                string methodName = "IssueRewardCertificate";
                string msg = string.Format("Issuing reward {0} for member {1}.", rdef.Name, lwmember.MyKey);

                MemberReward reward = new MemberReward();

                reward.RewardDefId = rdef.Id;
                reward.OfferCode = offerCode;
                reward.MemberId = lwmember.IpCode;

                if (variantId > 0)
                {
                    //ProductVariant pv = LWDataServiceUtil.DataServiceInstance().GetProductVariant(variantId); AEO-2114 
                    using (var contService = _dataUtil.ContentServiceInstance())
                    {
                        ProductVariant pv = contService.GetProductVariant(variantId); //AEO-2114
                        reward.ProductId = pv.ProductId;
                        reward.ProductVariantId = variantId;
                    }
                }
                else
                {
                    reward.ProductId = rdef.ProductId;
                }

                reward.DateIssued = DateTime.Now;
                reward.Expiration = expiryDate;
                reward.AvailableBalance = (long)rewardAmount;

                /* AEO-2114 begin
                if ( rdef.Name.ToUpper() == "AEO REWARDS $10 REWARD" ) {
                    reward.AvailableBalance = 10;
                }
                AEO-2114 end */



                IClientDataObject cert = null;
                //if (string.IsNullOrEmpty(certNmbr))
                //{
                //    if (!string.IsNullOrEmpty(rdef.CertificateTypeCode) && !string.IsNullOrEmpty(RewardCertificateBucket))
                //    {
                //        AttributeSetMetaData meta = service.GetAttributeSetMetaData(RewardCertificateBucket);
                //        IList<IClientDataObject> certs = null;
                //        if (!string.IsNullOrEmpty(CertificateNmbrAttribute))
                //        {
                //            /*
                //             * If this reward is configured to use certificates from the cert bucket and an offercode
                //             * is defined in the UI for the reward, then use that offercode as the type code, otherwise
                //             * use the type code in the reward definitino.
                //             * */
                //            string typeCode = rdef.CertificateTypeCode;
                //            if (!string.IsNullOrEmpty(offerCode))
                //            {
                //                typeCode = offerCode;
                //            }
                //            LWCriterion crit = new LWCriterion(RewardCertificateBucket);
                //            crit.Add(LWCriterion.OperatorType.AND, CertificateTypeCodeAttribute, rdef.CertificateTypeCode, LWCriterion.Predicate.Eq);
                //            crit.Add(LWCriterion.OperatorType.AND, CertificateStatusAttribute, "0", LWCriterion.Predicate.Eq);
                //            LWQueryBatchInfo batchInfo = new LWQueryBatchInfo() { BatchSize = 1, StartIndex = 0 };

                //            //certs = service.GetAttributeSetObjects((IAttributeSetContainer)null, meta, crit, batchInfo, false, true);  // AEO-74 Upgrade 4.5 here -----------SCJ
                //            certs = service.GetAttributeSetObjects((IAttributeSetContainer)null, meta, crit, null, false, true);

                //        }
                //        if (certs != null && certs.Count > 0)
                //        {
                //            cert = certs[0];
                //        }
                //        else
                //        {
                //            msg = string.Format("No more reward certificates of type {0} left ", rdef.CertificateTypeCode);
                //            logger.Error(className, methodName, msg);
                //            throw new LWRulesException(msg);
                //        }
                //    }
                //    else
                //    {
                //        logger.Trace(className, methodName, "No certificate required for issuing reward " + rdef.Name);
                //    }
                //}

                try
                {
                    if (string.IsNullOrEmpty(certNmbr))
                    {
                        if (cert != null)
                        {
                            reward.CertificateNmbr = cert.GetAttributeValue(CertificateNmbrAttribute).ToString();
                            cert.SetAttributeValue(CertificateStatusAttribute, 1);
                            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                            //service.CreateMemberReward(reward, cert);
                            service.CreateMemberReward(reward);
                            // AEO-74 Upgrade 4.5 changes END here -----------SCJ

                        }
                        else
                        {
                            service.CreateMemberReward(reward);
                        }
                    }
                    else
                    {
                        msg = string.Format("Using certificate number {0} as provided.", certNmbr);
                        logger.Trace(className, methodName, msg);
                        reward.CertificateNmbr = certNmbr;
                        service.CreateMemberReward(reward);
                    }
                    return reward.Id;
                }
                catch (Exception ex)
                {
                    logger.Error(className, methodName, "Error issuing reward.", ex);
                    throw ex;
                }
                finally
                {
                }
            }
        }

        //internal void ConsumePoints(Member lwmember, VirtualCard lwvirtualcard, RewardDef rdef, PointType pt, long rewardId, double PointsToConsume) // AEO-74 Upgrade 4.5 changes  here -----------SCJ
        internal void ConsumePoints(Member lwmember, VirtualCard lwvirtualcard, RewardDef rdef, PointType pt, long rewardId, decimal PointsToConsume)
        {
            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {
                // consume points.            
                long rowkey = rewardId;
                List<string> pointTypes = new List<string>();

                decimal pointsLeftToConsume = PointsToConsume; // AEO-74 Upgrade 4.5 changes  here -----------SCJ
                decimal consumed = 0;// AEO-74 Upgrade 4.5 changes here -----------SCJ

                IList<PointType> pType = new List<PointType>();
                IList<PointEvent> pEvent = new List<PointEvent>();

                pointTypes = new List<string>(rdef.PointType.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                foreach (string ptype in pointTypes)
                {
                    PointType pointType = service.GetPointType(ptype);
                    pType.Add(pointType);
                }

                pEvent = service.GetAllPointEvents();

                // AEO-454 Begin

                RewardDef rewardDef = rdef;
                String notesText = string.Empty;

                /* AEO-2114 begin
                if ( rewardDef != null ) {
                    if (    rewardDef.Name.ToUpper() == "AEO REWARDS $5 REWARD" || 
                            rewardDef.Name.ToUpper() == "$5 - REWARD" ||
                            rewardDef.Name.ToUpper() == "AEO REWARDS $10 REWARD" ||
                             rewardDef.Name.ToUpper().Contains ("STACKED") ) { // AEO-880 begin & end
                        notesText = "Reward issued";
                    }
                }
                AEO-2114 begin */

                consumed = service.ConsumePoints(lwmember.LoyaltyCards, pType, pEvent, DateTime.Now, PointsToConsume,
                     PointTransactionOwnerType.Reward, notesText, rdef.Id, rewardId);

                // AEO-454 end

                if (consumed == 0)
                    throw new LWException(string.Format("Could not consume {0} points for member id {1}", PointsToConsume.ToString(), lwmember.MyKey.ToString()));
            }
        }

        internal void CheckLowCertificateThreshold(Member lwmember, RewardDef rdef)
        {
            return;
        }
    }
}
