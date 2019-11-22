namespace Brierley.AEModules.RequestCredit.Components
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Common.Logging;
    using AmericanEagle.SDK.Global;

    /// <summary>
    /// Class SearchTxn
    /// </summary>
    public class SearchTxn
    {

        /// <summary>
        /// Logger to trace or debug 
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("SearchTxn");

        /// <summary>
        /// Method for searching the transaction
        /// </summary>
        /// <param name="searchParms">Dictionary parameter</param>
        /// <returns>list search result</returns>
        public IList<IClientDataObject> SearchTransaction(Dictionary<string, string> searchParms, string selectedValue)
        {
            // Holds history records
            IList<IClientDataObject> oHistoryRecords = null;
            string txnNumber = string.Empty;
            string storeNumber = string.Empty;
            string registerNumber = string.Empty;
            DateTime? txnDate = null;
            string tenderAmount = string.Empty;
            string orderNumber = string.Empty;


            // Holds LW criteria for history transaction details
            LWCriterion lwCriteria = new LWCriterion("HistoryTxnDetail");
            if (searchParms.Count > 0)
            {
                foreach (KeyValuePair<string, string> de in searchParms)
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Key: " + de.Key + ", Value: " + de.Value);
                    if (de.Key.ToUpper().Contains("TXNDATE"))
                    {
                        txnDate = DateTime.Parse(de.Value);
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "txnDate: " + txnDate.Value.ToShortDateString());
                    }
                    else if (de.Key.ToUpper().Contains("TXNNUMBER"))
                    {
                        txnNumber = de.Value;
                    }
                    else if (de.Key.ToUpper().Contains("TXNREGISTERNUMBER"))
                    {
                        registerNumber = de.Value;
                    }
                    else if (de.Key.ToUpper().Contains("ORDERNUMBER"))
                    {
                        orderNumber = de.Value;
                    }
                    else if (de.Key.ToUpper().Contains("TENDERAMOUNT"))
                    {
                        tenderAmount = de.Value;
                    }
                    else if (de.Key.ToUpper().Contains("STOREID"))
                    {
                        int storeID = int.Parse(de.Value);
                        storeNumber = storeID.ToString();
                        //string[] AttributeName = de.Key.Split('_');
                        //if (AttributeName[1].ToUpper().Contains("STOREID"))
                        //{
                        //    //lwCriteria.AddDistinct(AttributeName[1]);
                        //    //lwCriteria.Add(LWCriterion.OperatorType.AND, "StoreNumber", storeID, LWCriterion.Predicate.Eq);
                        //}
                        //else
                        //{
                        //    lwCriteria.AddDistinct(AttributeName[1]);
                        //    lwCriteria.Add(LWCriterion.OperatorType.AND, AttributeName[1], de.Value, LWCriterion.Predicate.Eq);
                        //}
                    }
                }
                oHistoryRecords = LoyaltyTransaction.SearchForHistoryTxnDetails(txnNumber, storeNumber, registerNumber, txnDate, tenderAmount, orderNumber, selectedValue, null); // PI 30364 - Dollar reward program - an extra parameter is added in LoyaltyTransaction.SearchForHistoryTxnDetails() method.                    
                //oHistoryRecords = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "HistoryTxnDetail", lwCriteria, new LWQueryBatchInfo(), false);
            }
            return oHistoryRecords;
        }

        /// <summary>
        /// Method for searching the member receipts
        /// </summary>
        /// <param name="searchParms">Dictionary parameter</param>
        /// <returns>int</returns>
        public int SearchReceipts(Dictionary<string, string> searchParms)
        {
            //PI14540 - disallow Get Points request for txn that has already been requested
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            string txnNumber = string.Empty;
            string storeNumber = string.Empty;
            string registerNumber = string.Empty;
            DateTime txnDate = DateTime.MinValue;
            string tenderAmount = string.Empty;
            string orderNumber = string.Empty;
            int intResult = 0;

            // Holds LW criteria for history transaction details
            LWCriterion lwCriteria = new LWCriterion("MemberReceipts");
            if (searchParms.Count > 0)
            {
                foreach (KeyValuePair<string, string> de in searchParms)
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Key: " + de.Key + ", Value: " + de.Value);
                    if (de.Key.ToUpper().Contains("TXNDATE"))
                    {
                        txnDate = DateTime.Parse(de.Value);
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "txnDate: " + txnDate.ToShortDateString());
                    }
                    else if (de.Key.ToUpper().Contains("TXNNUMBER"))
                    {
                        txnNumber = de.Value;
                    }
                    else if (de.Key.ToUpper().Contains("TXNREGISTERNUMBER"))
                    {
                        registerNumber = de.Value;
                    }
                    else if (de.Key.ToUpper().Contains("ORDERNUMBER"))
                    {
                        orderNumber = de.Value;
                    }
                    else if (de.Key.ToUpper().Contains("TENDERAMOUNT"))
                    {
                        tenderAmount = de.Value;
                    }
                    else if (de.Key.ToUpper().Contains("STOREID"))
                    {
                        int storeID = int.Parse(de.Value);
                        storeNumber = storeID.ToString();
                    }
                }

                intResult = LoyaltyTransaction.SearchReceiptDetails(txnNumber, storeNumber, registerNumber, txnDate, tenderAmount, orderNumber);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return intResult;
        }


    }
}