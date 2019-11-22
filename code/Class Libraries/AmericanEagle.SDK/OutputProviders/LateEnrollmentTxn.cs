// ----------------------------------------------------------------------------------
// <copyright file="LateMemberTxn.cs" company="Brierley and Partners">
//     Copyright statement. All right reserved
// </copyright>
// ----------------------------------------------------------------------------------
namespace AmericanEagle.SDK.OutputProviders
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Xml;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.DataAcquisition.Core;
    using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
    using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;
    using Brierley.ClientDevUtilities.LWGateway;

    /// <summary>
    /// Class LateEnrollmentTxn
    /// </summary>
    public class LateEnrollmentTxn : IDAPOutputProvider
    {
        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.    
        /// </summary>
        public void Dispose()
        {
            //Reset the ProactiveMergeStartDate variable in client configuration table
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "LateEnrollStartDate - Begin");
            try
            {
                using (var dtService = _dataUtil.DataServiceInstance())
                {
                    ClientConfiguration config = dtService.GetClientConfiguration("LateEnrollStartDate");
                    string startDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
                    config.Value = startDate;
                    dtService.UpdateClientConfiguration(config);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "LateEnrollStartDate - End");
        }


        /// <summary>
        /// This method is called to initialize the message dispatcher
        /// </summary>
        /// <param name="globals">NameValueCollection globals</param>
        /// <param name="args">NameValueCollection args</param>
        /// <param name="jobId">long jobId</param>
        /// <param name="config">DAPDirectives config</param>
        /// <param name="parameters">NameValueCollection parameters</param>
        /// <param name="performUtil">DAPPerformanceCounterUtil performUtil</param>
        public void Initialize(NameValueCollection globals, NameValueCollection args, long jobId, DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
        }

                /// <summary>
        /// This method is called to process the messages in the batch
        /// </summary>
        /// <param name="messageBatch">String List</param>
        public void ProcessMessageBatch(List<string> messageBatch)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            try
            {
                // Tracing for starting of method
                this.logger.Trace(this.className, methodName, "Starts");

                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    // Loding XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/Global/LateMember");
                string loyaltyIDNumber = string.Empty;
                string txnHeaderId = string.Empty;

                if (null != xmlNode)
                {
                    loyaltyIDNumber = xmlNode.Attributes["LoyaltyIDNumber"].Value;
                    txnHeaderId = xmlNode.Attributes["TxnHeaderId"].Value;

                    if (!string.IsNullOrEmpty(loyaltyIDNumber))
                    {
                        // Get member
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            Member member = lwService.LoadMemberFromLoyaltyID(loyaltyIDNumber);
                            if (null != member)
                            {
                                //// Apply a search on HistroyTxnDetail table to find a transaction with the the provided
                                //// input parameters LoyaltyId,  ProcessId = 2
                                //LWCriterion criteria = new LWCriterion("HistoryTxnDetail");
                                //criteria.Add(LWCriterion.OperatorType.AND, "TxnLoyaltyID", loyaltyIDNumber, LWCriterion.Predicate.Eq);
                                //criteria.Add(LWCriterion.OperatorType.AND, "ProcessID", 2, LWCriterion.Predicate.Eq);

                                ////Need to add the date ranges to speed up the query so the search won't search across date partitions.
                                //DateTime startDate = DateTime.Now;
                                //DateTime endDate = DateTime.Now;

                                //Utilities.GetQuarterDates(out startDate, out endDate);
                                //this.logger.Trace(this.className, methodName, "startDate: " + startDate.ToShortDateString());
                                //this.logger.Trace(this.className, methodName, "endDate: " + endDate.ToShortDateString());
                                //criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", startDate, LWCriterion.Predicate.Ge);
                                //criteria.Add(LWCriterion.OperatorType.AND, "TxnDate", endDate, LWCriterion.Predicate.Le);

                                //IList<IClientDataObject> historyRecords = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "HistoryTxnDetail", criteria, new LWQueryBatchInfo(), true);

                                this.logger.Trace(this.className, methodName, "txnHeaderId: " + txnHeaderId);
                                //// If a transaction is found proceed to if block for next set of activities otherwise skip the record
                                //foreach(IClientDataObject obj in historyRecords)
                                //{
                                //    HistoryTxnDetail historyDetail = (HistoryTxnDetail)obj;
                                LoyaltyTransaction.AddLoyaltyTransaction(member, txnHeaderId, ProcessId.ProcessedforLoyalty, "Late Enroll");
                                //}
                            }
                            else
                            {
                                // Log error when member not found
                                this.logger.Error(this.className, methodName, "Member Not Found for Loyalty Number : " + loyaltyIDNumber);
                            }
                        }
                    }
                    else
                    {
                        // Log error when blank loyalty number
                        this.logger.Error(this.className, methodName, "Blank LoyaltyNumber not allowed");
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message, ex);
                throw;
            }
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown()
        {
            return 0;
            //            throw new System.NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    }
}
