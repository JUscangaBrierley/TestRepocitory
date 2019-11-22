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
    class AmericanEagleBraIssueRewardUtil
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
            private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        #endregion

        #region Public Fields
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

            public string OfferCode
            {
                get { return offerCode; }
                set { offerCode = value; }
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

            //internal double GetPoints(RewardDef rdef, PointType pt, PointEvent pe, Member lwmember, VirtualCard lwvirtualcard)
                 internal decimal GetPoints(RewardDef rdef, PointType pt, PointEvent pe, Member lwmember, VirtualCard lwvirtualcard)
            {
                string methodName = "GetPoints";
                DateTime from;
                //AEO 72 Changes- incorrect from date picked for first run of year begin --------------SCJ
                if ((DateTime.Now.Month == 1) && (DateTime.Now.Day < 31))
                {
                     from = new DateTime (DateTime.Now.AddYears(-1).Year, 1, 1);
                }
                else
                {
                    from = new DateTime(DateTime.Today.Year, 1, 1);
                }
                //AEO 72 Changes- incorrect from date picked for first run of year end --------------SCJ

                // AEO Redesign 2015 Begin
                // IList<IClientDataObject> loDetails = lwmember.GetChildAttributeSets("MemberDetails");
                DateTime to = DateTime.Today;

                to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);





            /*MemberDetails details = ( loDetails == null || loDetails.Count == 0 ? null : loDetails[0] ) as MemberDetails;

                 if (details != null && details.ExtendedPlayCode == 1 ) {
                    to = DateTime.Today.AddDays(-13);
                 }*/

            // AEO Redesign 2015 End
            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BraRewardDateFrom"]) && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["BraRewardDateTo"]))
                {
                    string strDateFrom = ConfigurationManager.AppSettings["BraRewardDateFrom"];
                    string strDateTo = ConfigurationManager.AppSettings["BraRewardDateTo"];
                    DateTime dateFrom;
                    DateTime dateTo;
                    if (DateTime.TryParse(strDateFrom, out dateFrom))
                    {
                        from = dateFrom;
                    }

                    if (DateTime.TryParse(strDateTo, out dateTo))
                    {
                        to = dateTo;
                    }
                }
                DateTime returnToDate = DateTime.Today;

                IList<PointEvent> pEvent = new List<PointEvent>();
                IList<PointEvent> pReturnEvent = new List<PointEvent>();

                logger.Trace(className, methodName, string.Format("Getting Points for member{0}, from: {1}, to: {2}", lwmember.IpCode, from, to));

                pEvent.Add(service.GetPointEvent("Bra Purchase - 1"));
                pEvent.Add(service.GetPointEvent("Bra Purchase - 15"));
                pReturnEvent.Add(service.GetPointEvent("Bra Return"));
                // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                //double points = 0;
                decimal points = 0;
                long[] PtType = new long[1];
                long[] PtEvent = new long[1];
                // AEO-74 Upgrade 4.5 changes END here -----------SCJ

                if (rdef.PointType != "All")
                {
                    if (pt == null)
                    {
                        string msg = string.Format("No currency specified to calculate points.");
                        LWRulesException ex = new LWRulesException(msg);
                        logger.Error(className, methodName, "Error calculating points.", ex);
                        throw ex;
                    }
                    // PI 27756 - Members with multipe cards having bra issues - Start

                    if (pe == null)
                    {
                        points = lwmember.GetPoints(pt, from, to);
                    }
                    else
                    {
                        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                        //points = lwmember.GetPoints(pt, pe, from, to);  

                        PtType[0] = pt.ID;
                        PtEvent[0] = pe.ID;

                        // AEO-557 Begin
                        if (pt.Name.ToUpper() == "JEAN POINTS")
                        {
                            from = new DateTime(2015, 10, 1);
                        }

                        //IList<PointTransaction> pointsList = service.GetPointTransactions(lwvirtualcard, from, to, null, null, false);
                        // AEO-1925 AH
                        IList<PointTransaction> pointsList = service.GetPointTransactions(lwmember, from, to, null, null, null, null, false);
                        foreach (PointTransaction ptt in pointsList)
                        {
                            if (ptt.TransactionType == PointBankTransactionType.Hold &&
                                ptt.TransactionType != PointBankTransactionType.Consumed &&
                                ptt.Points == ptt.PointsOnHold)
                            {
                                //pointsOnHold += (ptt.Points - ptt.PointsConsumed);
                                continue;
                            }

                            points += (ptt.Points - ptt.PointsConsumed);
                        }
                    }
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

                 //internal double GetOnHoldPoints(RewardDef rdef, PointType pt, PointEvent pe, Member lwmember, VirtualCard lwvirtualcard) // AEO-74 Upgrade 4.5 changes here -----------SCJ
            internal decimal GetOnHoldPoints(RewardDef rdef, PointType pt, PointEvent pe, Member lwmember, VirtualCard lwvirtualcard)
            {
                string methodName = "GetOnHoldPoints";

            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {
                DateTime from = DateTimeUtil.MinValue;
                DateTime to = DateTimeUtil.MaxValue;

                // double onhold = 0; // AEO-74 Upgrade 4.5 changes here -----------SCJ
                decimal onhold = 0;
                if (rdef.PointType != "All")
                {
                    if (pt == null)
                    {
                        string msg = string.Format("No currency specified to check points on hold.");
                        LWRulesException ex = new LWRulesException(msg);
                        logger.Error(className, methodName, "Error getting points on hold.", ex);
                        throw ex;
                    }
                    if (lwvirtualcard == null)
                    {
                        // use all virtual cards
                        if (pe == null)
                        {
                            //onhold = lwmember.GetPointsOnHold(pt, from, to);
                            onhold = service.GetPointsOnHold(lwmember.LoyaltyCards, new List<PointType> { pt }, null, from, to);
                        }
                        else
                        {
                            //onhold = lwmember.GetPointsOnHold(pt, pe, from, to);
                            onhold = service.GetPointsOnHold(lwmember.LoyaltyCards, new List<PointType> { pt }, new List<PointEvent> { pe }, from, to);
                        }
                    }
                    else
                    {
                        if (pe == null)
                        {
                            onhold = service.GetPointsOnHold(new List<VirtualCard> { lwvirtualcard }, new List<PointType> { pt }, null, from, to);
                        }
                        else
                        {
                            onhold = service.GetPointsOnHold(new List<VirtualCard> { lwvirtualcard }, new List<PointType> { pt }, new List<PointEvent> { pe }, from, to);
                        }
                    }
                }
                else
                {
                    if (lwvirtualcard == null)
                    {
                        onhold = service.GetPointsOnHold(new List<VirtualCard> { lwmember.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard) }, null, null, from, to);
                    }
                    else
                    {
                        onhold += service.GetPointsOnHold(new List<VirtualCard> { lwvirtualcard }, null, null, from, to);
                    }
                }
                return onhold;
            }
            }

            // AEO-74 Upgrade 4.5 changes here -----------SCJ
            //internal long IssueRewardCertificate(Member lwmember, RewardDef rdef, DateTime expiryDate, RewardFulfillmentOption rewardFulfillmentOption, string certNmbr, int variantId, string orderNumber, double rewardAmount)
                internal long IssueRewardCertificate(Member lwmember, RewardDef rdef, DateTime expiryDate, RewardFulfillmentOption rewardFulfillmentOption, string certNmbr, int variantId, string orderNumber, decimal rewardAmount)
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
                    using (var contService = _dataUtil.ContentServiceInstance())
                    {
                        ProductVariant pv = contService.GetProductVariant(variantId);
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


                IClientDataObject cert = null;
                if (string.IsNullOrEmpty(certNmbr))
                {
                    if (!string.IsNullOrEmpty(rdef.CertificateTypeCode) && !string.IsNullOrEmpty(RewardCertificateBucket))
                    {
                        AttributeSetMetaData meta = service.GetAttributeSetMetaData(RewardCertificateBucket);
                        IList<IClientDataObject> certs = null;
                        if (!string.IsNullOrEmpty(CertificateNmbrAttribute))
                        {
                            /*
                             * If this reward is configured to use certificates from the cert bucket and an offercode
                             * is defined in the UI for the reward, then use that offercode as the type code, otherwise
                             * use the type code in the reward definitino.
                             * */
                            string typeCode = rdef.CertificateTypeCode;
                            if (!string.IsNullOrEmpty(offerCode))
                            {
                                typeCode = offerCode;
                            }
                            LWCriterion crit = new LWCriterion(RewardCertificateBucket);
                            crit.Add(LWCriterion.OperatorType.AND, CertificateTypeCodeAttribute, rdef.CertificateTypeCode, LWCriterion.Predicate.Eq);
                            crit.Add(LWCriterion.OperatorType.AND, CertificateStatusAttribute, "0", LWCriterion.Predicate.Eq);
                            LWQueryBatchInfo batchInfo = new LWQueryBatchInfo() { BatchSize = 1, StartIndex = 0 };

                            //certs = service.GetAttributeSetObjects((IAttributeSetContainer)null, meta, crit, batchInfo, false, true);  // AEO-74 Upgrade 4.5 here -----------SCJ
                            certs = service.GetAttributeSetObjects((IAttributeSetContainer)null, meta, crit, null, false, true);

                        }
                        if (certs != null && certs.Count > 0)
                        {
                            cert = certs[0];
                        }
                        else
                        {
                            msg = string.Format("No more reward certificates of type {0} left ", rdef.CertificateTypeCode);
                            logger.Error(className, methodName, msg);
                            throw new LWRulesException(msg);
                        }
                    }
                    else
                    {
                        logger.Trace(className, methodName, "No certificate required for issuing reward " + rdef.Name);
                    }
                }

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
                 internal void ConsumePoints(Member lwmember, VirtualCard lwvirtualcard, RewardDef rdef, PointType pt, long rewardId,decimal PointsToConsume)
            {
            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {
                // consume points.            
                long rowkey = rewardId;
                //if (Context.InvokingRow != null)
                //{
                //    aSetCode = Context.InvokingRow.GetMetaData().ID;
                //}
                //double pointsLeftToConsume = PointsToConsume; // AEO-74 Upgrade 4.5 changes  here -----------SCJ
                decimal pointsLeftToConsume = PointsToConsume; // AEO-74 Upgrade 4.5 changes  here -----------SCJ
                //double consumed = 0;// AEO-74 Upgrade 4.5 changes here -----------SCJ
                decimal consumed = 0;// AEO-74 Upgrade 4.5 changes here -----------SCJ

                IList<PointType> pType = new List<PointType>();
                IList<PointEvent> pEvent = new List<PointEvent>();


                pType.Add(service.GetPointType(pt.Name));
                pEvent.Add(service.GetPointEvent("Bra Purchase - 1"));
                pEvent.Add(service.GetPointEvent("Bra Purchase - 15"));
                pEvent.Add(service.GetPointEvent("Bra Return"));
                //AEO-Redesign-2015 Begin
                pEvent.Add(service.GetPointEvent("Jeans Credit"));
                pEvent.Add(service.GetPointEvent("Bra Credit"));
                pEvent.Add(service.GetPointEvent("Jean Purchase"));
                pEvent.Add(service.GetPointEvent("B5G1 Jean Purchase"));
                pEvent.Add(service.GetPointEvent("B5G1 Bra Purchase"));
                //AEO-Redesign-2015 End

                // AEO-258 begin
                pEvent.Add(service.GetPointEvent("Bra AECC Bonus Credit"));
                pEvent.Add(service.GetPointEvent("Jean AECC Bonus Credit"));
                pEvent.Add(service.GetPointEvent("Returned Jean Reward Points"));
                pEvent.Add(service.GetPointEvent("Returned Bra Reward Points"));
                // AEO-258 End


                // Decimal PointsToConsumeD = Convert.ToDecimal(PointsToConsume);
                consumed = service.ConsumePoints(lwmember.LoyaltyCards, pType, pEvent, DateTime.Now, PointsToConsume, PointTransactionOwnerType.Reward, string.Empty, rdef.Id, rewardId);



                if (consumed == 0)
                    throw new LWException(string.Format("Could not consume {0} points for member id {1}", PointsToConsume.ToString(), lwmember.MyKey.ToString()));
            }
            }

            //internal void HoldPoints(Member lwmember, VirtualCard lwvirtualcard, RewardDef rdef, PointType pt, long rewardId, double PointsToHold) // AEO-74 Upgrade 4.5 changes  here -----------SCJ
              internal void HoldPoints(Member lwmember, VirtualCard lwvirtualcard, RewardDef rdef, PointType pt, long rewardId, decimal PointsToHold) // AEO-74 Upgrade 4.5 changes  here -----------SCJ
            {
            using (var service = _dataUtil.LoyaltyDataServiceInstance())
            {
                // hold points.
                //long aSetCode = 1; // Means member reward            
                PointEvent pevent = null;
                string notes = "Points put on hold for member reward.";
                if (rdef.PointEvent != "All")
                {
                    pevent = service.GetPointEvent(rdef.PointEvent);
                }
                if (rdef.PointType != "All")
                {
                    if (lwvirtualcard != null)
                    {
                        if (rdef.PointEvent == "All")
                        {
                            IList<PointEvent> coll = service.GetAllPointEvents();
                            // Go through all point events and keep track of how many points have been consumed.
                            // We have consumed the required number of points, then stop.
                            foreach (PointEvent pe in coll)
                            {
                                service.HoldPoints(lwvirtualcard, pt, pe, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
                            }
                        }
                        else
                        {
                            service.HoldPoints(lwvirtualcard, pt, pevent, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
                        }
                    }
                    else
                    {
                        // consume points from all cards.
                        if (rdef.PointEvent == "All")
                        {
                            IList<PointEvent> coll = service.GetAllPointEvents();
                            foreach (PointEvent pe in coll)
                            {
                                service.HoldPoints(lwmember, pt, pe, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
                            }
                        }
                        else
                        {
                            service.HoldPoints(lwmember, pt, pevent, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
                        }
                    }
                }
                else
                {
                    IList<PointType> coll = service.GetAllPointTypes();
                    foreach (PointType ptype in coll)
                    {
                        if (lwvirtualcard != null)
                        {
                            if (rdef.PointEvent == "All")
                            {
                                IList<PointEvent> pecoll = service.GetAllPointEvents();
                                foreach (PointEvent pe in pecoll)
                                {
                                    service.HoldPoints(lwvirtualcard, ptype, pe, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
                                }
                            }
                            else
                            {
                                service.HoldPoints(lwvirtualcard, ptype, pevent, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
                            }
                        }
                        else
                        {
                            // consume points from all cards.
                            if (rdef.PointEvent == "All")
                            {
                                IList<PointEvent> pecoll = service.GetAllPointEvents();
                                foreach (PointEvent pe in pecoll)
                                {
                                    service.HoldPoints(lwmember, ptype, pe, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
                                }
                            }
                            else
                            {
                                service.HoldPoints(lwmember, ptype, pevent, rdef.HowManyPointsToEarn, DateTime.Now, PointTransactionOwnerType.Reward, rdef.Id, rewardId, notes, null, null);
                            }
                        }
                    }
                }
            }
            }

            internal void CheckLowCertificateThreshold(Member lwmember, RewardDef rdef)
            {
                return;
            }
    }
}
