using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Configuration;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

using AmericanEagle.SDK.Global;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.OutputProviders
{
    public class BraRewards : IDAPOutputProvider
    {
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        //RKG - changed to call custom rule to only reward certificates to members with points outside of 2 week hold period.
        private const string ruleName = "Bra Reward";

        public void Initialize(System.Collections.Specialized.NameValueCollection globals, System.Collections.Specialized.NameValueCollection args, long jobId, DAPDirectives config, System.Collections.Specialized.NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
            return;
        }

        public void ProcessMessageBatch(List<string> messageBatch)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            long ipCode = 0;

            try
            {
                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    // Loding XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("BraRewards/BraReward");
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    switch (node.Name.ToUpper())
                    {
                        case "IPCODE":
                            ipCode = long.Parse(node.InnerText);
                            break;
                        default:
                            ipCode = 0;
                            break;
                    }
                }

                if (null != xmlNode)
                {
                    if (ipCode > 0)
                    {
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            //We have a valid IPCode so get the member 
                            Member member = lwService.LoadMemberFromIPCode(ipCode);
                            if (null == member)
                            {
                                // Log error when member not found
                                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for IPCODE - " + xmlNode.Attributes["IPCODE"].Value);
                            }
                            else
                            {
                                //We have a valid member, so execute the IssueReward rule and then expire all the Returns that were part of a consumption.  
                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Executing rule: " + ruleName);

                                //long memberRewardId = ExecuteRule(member);
                                long memberRewardId = ExecuteRule(member);

                                //ExpireReturns(member);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
            }

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        public long ExecuteRule(Member member)        
        {
            long memberRewardId = 0;

            try
            {

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: Member: " + member.IpCode);

                //Create a context object and assign the member and MemberDetails attribute set
                //to be used in invoking the IssueReward for this reward and the AwardPoints 
                //rule associated with the View Earned Reward 
                ContextObject cobj = new ContextObject();
                cobj.Owner = member;
                cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    RuleTrigger ruleTrigger = lwService.GetRuleByName(ruleName);
                    if (ruleTrigger == null)
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule: (" + ruleName + ") Not Defined");
                        throw new Exception(ruleName + " Rule Not Defined", new Exception(ruleName + " Rule Not Defined"));
                    }

                    //Execute the rule
                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                    //memberRewardId = LWDataServiceUtil.DataServiceInstance(true).Execute(ruleTrigger, cobj, 0);
                    lwService.Execute(ruleTrigger, cobj);
                    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                    return memberRewardId;
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
                throw new Exception(ex.Message, new Exception("System Error"));
            }

        }
        public void ExpireReturns(Member member)
        {
            DateTime processDate = DateTime.Today;
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.Today.AddDays(1);
            int index = 0;
            bool ConsumedTxnsExist = false;
            long braReturnEventId = 0;
            //double totalConsumedPoints = 0;// AEO-74 Upgrade 4.5 changes here -----------SCJ
            decimal totalConsumedPoints = 0;// AEO-74 Upgrade 4.5 changes here -----------SCJ

            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {

                    //Go back 1 month to get all unexpired bra points
                    fromDate = DateTime.Parse("1/1/" + DateTime.Today.Year.ToString());
                    toDate = DateTime.Today.AddDays(1).AddSeconds(-1);

                    IList<PointEvent> pointEvents = lwService.GetAllPointEvents();
                    IList<PointType> pointTypes = lwService.GetAllPointTypes();
                    braReturnEventId = lwService.GetPointEvent("Bra Return").ID;

                    long[] pointTypeIds = new long[1];
                    long[] pointEventIds = new long[3];

                    //We want to look at all point events, but only want to look for Bra Points
                    foreach (PointType pt in pointTypes)
                    {
                        if (pt.Name == "Bra Points")
                        {
                            pointTypeIds[index] = pt.ID;
                            ++index;
                        }
                    }

                    index = 0;
                    foreach (PointEvent pt in pointEvents)
                    {
                        if ((pt.Name == "Bra Purchase - 1") || (pt.Name == "Bra Return") || (pt.Name == "Bra Purchase - 15"))
                        {
                            pointEventIds[index] = pt.ID;
                            ++index;
                        }
                    }

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get Point Txns from : " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString());
                    //Get all the Bra Point Txns.
                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                    //IList<PointTransaction> braTxns = LWDataServiceUtil.DataServiceInstance(true).GetPointTransactionsByPointTypePointEvent(member, fromDate, toDate, pointTypeIds, pointEventIds, null, null, null, null, false);
                    IList<PointTransaction> braTxns = lwService.GetPointTransactionsByPointTypePointEvent(member, fromDate, toDate, null, pointTypeIds, pointEventIds, null, null, null, false, null);
                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ

                    ConsumedTxnsExist = DoConsumedTxnExist(braTxns, out totalConsumedPoints);


                    foreach (PointTransaction txn in braTxns)
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Processing Point Txn : " + txn.Id.ToString());
                        //We need to look at all point txns including consumption records so we can expire those along with the normal purchase records.
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Point Txn is either Credit or Debit: EventId - " + braReturnEventId.ToString());

                        //If the txn is a Bra Return and there were some txns that were consumed in this run today then go ahead and expire the return points. 
                        //If we have returns but the member didn't qualify for a reward then we don't want to expire the return.  We want the return to be available for the next run
                        if (txn.PointEventId == braReturnEventId)
                        {
                            if (ConsumedTxnsExist)
                            {
                                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Point Txn is Return and there are consumptions so expire today.");
                                txn.ExpirationDate = processDate;
                            }
                            else
                            {
                                txn.ExpirationDate = DateTime.Parse("1/2/" + processDate.AddYears(1).Year.ToString());
                            }
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Updating current Point Txn's Expiration Date to Today : " + DateTime.Today.ToShortDateString());
                            lwService.UpdatePointTransaction(txn);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// Check to see if any of the txns were consumed today.  If so, then return true.
        /// </summary>
        /// <param name="braTxns"></param>
        /// <returns></returns>
        //private bool DoConsumedTxnExist(IList<PointTransaction> braTxns, out double totalConsumedPoints) // AEO-74 Upgrade 4.5 changes here -----------SCJ
        private bool DoConsumedTxnExist(IList<PointTransaction> braTxns, out decimal totalConsumedPoints) 
        {
            bool returnValue = false;
            totalConsumedPoints = 0;

            foreach (PointTransaction txn in braTxns)
            {
                //Loop through all txns and see if we have any that were consumed today
                if ((txn.TransactionType == Brierley.FrameWork.Common.PointBankTransactionType.Consumed) && (txn.TransactionDate.ToShortDateString() == DateTime.Today.ToShortDateString()))
                {
                    returnValue = true;
                    break;
                }
            }
            return returnValue;
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown()
        {
            return 0;
            //        throw new System.NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ

        public void Dispose()
        {
            return;
        }
    }
}
