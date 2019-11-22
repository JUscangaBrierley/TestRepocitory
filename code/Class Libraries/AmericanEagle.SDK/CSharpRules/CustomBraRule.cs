using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.Collections.Specialized;  // AEO-74 Upgrade 4.5 changes here -----------SCJ
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data;
using Brierley.LoyaltyWare.RulesProcessor;
using Brierley.FrameWork.Common.Logging;
using Brierley.ClientDevUtilities.LWGateway;


namespace AmericanEagle.SDK.CSharpRules
{
    public class CustomBraRule : ICSharpRuleClass
    {
        private static LWLogger logger = LWLoggerManager.GetLogger("Utilities");
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil();
        #region ICSharpRuleClass Members

        /// <summary>
        /// This Custom Rule will be used for two purposes
        /// 1. Set the expiration date on all point transactions that were included in this Reward run to expire today.  So any returns that were used in this run will not be included in the next run
        /// 2. If any point txns were not consumed (The member did not have an exact multiple of 5) then the remaining points that were not consumed will be converted over to the next point type
        ///     so it will be included in the next reward run (either the 1st or 15th).
        /// </summary>
        /// <param name="directive"></param>
        /// <param name="container"></param>
        public void Invoke(RulesProcessorDirectives.RuleDirective directive, IAttributeSetContainer container)
        {
            DateTime processDate = DateTime.Today;
            bool conversionOneTime = false;
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.Today.AddDays(1);
            int index = 0;
            bool ConsumedTxnsExist = false;
            long braReturnEventId = 0;
            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
            //double totalConsumedPoints = 0;
            decimal totalConsumedPoints = 0;
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ

            try
            {
                using (var lDataService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    Member member = container as Member;

                    if (null != ConfigurationManager.AppSettings["ProcessDate"])
                    {
                        string strProcessDate = ConfigurationManager.AppSettings["ProcessDate"];
                        DateTime.TryParse(strProcessDate, out processDate);
                    }
                    if (null != ConfigurationManager.AppSettings["ConversionOneTime"])
                    {
                        string strconversionOneTime = ConfigurationManager.AppSettings["ConversionOneTime"];
                        Boolean.TryParse(strconversionOneTime, out conversionOneTime);
                    }

                    //Go back 1 month to get all unexpired bra points
                    fromDate = DateTime.Parse("1/1/" + DateTime.Today.Year.ToString());
                    toDate = DateTime.Today.AddDays(1);

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "CustomBraRule for : " + member.IpCode);

                    IList<PointEvent> pointEvents = lDataService.GetAllPointEvents();
                    IList<PointType> pointTypes = lDataService.GetAllPointTypes();
                    braReturnEventId = lDataService.GetPointEvent("Bra Return").ID;

                    long[] pointTypeIds = new long[1];
                    long[] pointEventIds = new long[2];

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
                    if (conversionOneTime)
                    {
                        pointEventIds = new long[3];
                        foreach (PointEvent pt in pointEvents)
                        {
                            if ((pt.Name == "Bra Purchase - 1") || (pt.Name == "Bra Purchase - 15") || (pt.Name == "Bra Return"))
                            {
                                pointEventIds[index] = pt.ID;
                                ++index;
                            }
                        }
                    }
                    else
                    {
                        foreach (PointEvent pt in pointEvents)
                        {
                            if (processDate.Day == 1)
                            {
                                if ((pt.Name == "Bra Purchase - 1") || (pt.Name == "Bra Return"))
                                {
                                    pointEventIds[index] = pt.ID;
                                    ++index;
                                }
                            }
                            if (processDate.Day == 15)
                            {
                                if ((pt.Name == "Bra Purchase - 15") || (pt.Name == "Bra Return"))
                                {
                                    pointEventIds[index] = pt.ID;
                                    ++index;
                                }
                            }
                        }
                    }

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Get Point Txns from : " + fromDate.ToShortDateString() + " to " + toDate.ToShortDateString());
                    //Get all the Bra Point Txns.
                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                    //IList<PointTransaction> braTxns = LWDataServiceUtil.DataServiceInstance(true).GetPointTransactionsByPointTypePointEvent(member,fromDate, toDate, pointTypeIds, pointEventIds, null, null, null, null, false);
                    IList<PointTransaction> braTxns = lDataService.GetPointTransactionsByPointTypePointEvent(member, fromDate, toDate, null, pointTypeIds, pointEventIds, null, null, null, false, null);
                    // AEO-74 Upgrade 4.5 changes END here -----------SCJ

                    ConsumedTxnsExist = DoConsumedTxnExist(braTxns, conversionOneTime, out totalConsumedPoints);

                    if (conversionOneTime)
                    {
                        ProcessOneTimeConversion(braTxns, braReturnEventId, ConsumedTxnsExist, processDate, totalConsumedPoints);
                        return;
                    }
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
                        }
                        else
                        {
                            txn.ExpirationDate = processDate;
                        }
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Updating current Point Txn's Expiration Date to Today : " + DateTime.Today.ToShortDateString());
                        lDataService.UpdatePointTransaction(txn);

                        //This will create another txn for the next run if the point txn was not consumed and it was a credit.  We don't want to do this for returns.  Returns we want to just expire.
                        if ((txn.PointsConsumed == 0) && (txn.TransactionType == Brierley.FrameWork.Common.PointBankTransactionType.Credit))
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Point Txn has not been consumed and is a Credit");
                            PointTransaction newTxn = new PointTransaction();
                            PointEvent newEvent = null;
                            newTxn = txn;
                            if (processDate.Day == 1)
                            {
                                newEvent = lDataService.GetPointEvent("Bra Purchase - 15");
                            }
                            if (processDate.Day == 15)
                            {
                                newEvent = lDataService.GetPointEvent("Bra Purchase - 1");
                            }
                            newTxn.PointEventId = newEvent.ID;
                            newTxn.ExpirationDate = DateTime.Parse("1/2/" + processDate.AddYears(1).Year.ToString());
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Creating new Point Txn for Event : " + newEvent.Name);
                            lDataService.CreatePointTransaction(newTxn);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //private void ProcessOneTimeConversion(IList<PointTransaction> braTxns, long braReturnEventId, bool ConsumedTxnsExist, DateTime processDate, double totalConsumedPoints)
          private void ProcessOneTimeConversion(IList<PointTransaction> braTxns, long braReturnEventId, bool ConsumedTxnsExist, DateTime processDate, decimal totalConsumedPoints)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            using (var lDataService = _dataUtil.LoyaltyDataServiceInstance())
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: ");

                var newbraTxns = braTxns.OrderBy(x => x.TransactionDate);

                braTxns = newbraTxns.ToList();

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
                    }
                    else
                    {
                        if (txn.TransactionType == Brierley.FrameWork.Common.PointBankTransactionType.Credit)
                        {
                            if (txn.PointsConsumed == 0)
                            {
                                if (totalConsumedPoints > 0)
                                {
                                    txn.PointsConsumed = 1;
                                }
                                totalConsumedPoints--;
                            }
                        }
                        txn.ExpirationDate = processDate;
                    }
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Updating current Point Txn's Expiration Date to Today : " + DateTime.Today.ToShortDateString());
                    lDataService.UpdatePointTransaction(txn);

                    //This will create another txn for the next run if the point txn was not consumed and it was a credit.  We don't want to do this for returns.  Returns we want to just expire.
                    if ((txn.PointsConsumed == 0) && (txn.TransactionType == Brierley.FrameWork.Common.PointBankTransactionType.Credit))
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Point Txn has not been consumed and is a Credit");
                        PointTransaction newTxn = new PointTransaction();
                        PointEvent newEvent = null;
                        newTxn = txn;
                        if (processDate.Day == 1)
                        {
                            newEvent = lDataService.GetPointEvent("Bra Purchase - 15");
                        }
                        if (processDate.Day == 15)
                        {
                            newEvent = lDataService.GetPointEvent("Bra Purchase - 1");
                        }
                        newTxn.PointEventId = newEvent.ID;
                        newTxn.ExpirationDate = DateTime.Parse("1/2/" + processDate.AddYears(1).Year.ToString());
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Creating new Point Txn for Event : " + newEvent.Name);
                        lDataService.CreatePointTransaction(newTxn);
                    }
                }
            }
        }

        /// <summary>
        /// Check to see if any of the txns were consumed today.  If so, then return true.
        /// </summary>
        /// <param name="braTxns"></param>
        /// <returns></returns>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //private bool DoConsumedTxnExist(IList<PointTransaction> braTxns, bool conversionOneTime, out double totalConsumedPoints)
        private bool DoConsumedTxnExist(IList<PointTransaction> braTxns, bool conversionOneTime, out decimal totalConsumedPoints)
            // AEO-74 Upgrade 4.5 changes END here -----------SCJ
            {
                bool returnValue = false;
                //bool returnsExists = false;
                totalConsumedPoints = 0;

                foreach (PointTransaction txn in braTxns)
                {
                    if (conversionOneTime)
                    {
                        //Loop through all txns and see if we have any that were consumed at any time since the is a one-time fix for conversion data
                        if ((txn.TransactionType == Brierley.FrameWork.Common.PointBankTransactionType.Consumed) && (txn.TransactionDate.ToShortDateString() != DateTime.Today.ToShortDateString()))
                        {
                            returnValue = true;

                            totalConsumedPoints += Math.Abs(txn.Points);

                        }
                        //Loop through all txns and see if we have any that were returned because we need to account for those also.
                        if (txn.TransactionType == Brierley.FrameWork.Common.PointBankTransactionType.Debit)
                        {
                            //returnsExists = true;

                            totalConsumedPoints += Math.Abs(txn.Points);

                        }
                    }
                    else
                    {
                        //Loop through all txns and see if we have any that were consumed today
                        if ((txn.TransactionType == Brierley.FrameWork.Common.PointBankTransactionType.Consumed) && (txn.TransactionDate.ToShortDateString() == DateTime.Today.ToShortDateString()))
                        {
                            returnValue = true;
                            break;
                        }
                    }
                }
                return returnValue;
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public void Cleanup()
        { 
        }
        public bool Initialize(RulesProcessorDirectives.RuleDirective directive, NameValueCollection nvParms)
        {
            return true;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ

        #endregion
    }
}
