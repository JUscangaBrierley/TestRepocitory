// ----------------------------------------------------------------------------------
// <copyright file="TlogAudit.cs" company="Brierley and Partners">
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
    using System.Text;
    using System.IO;

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
    /// Class TlogAudit
    /// </summary>
    public class TlogAudit : IDAPOutputProvider
    {
        /// <summary>
        /// Description is used for point event
        /// </summary>
        private string description = string.Empty;
        private int auditTxnCount = 0;
        private int auditLoyaltyCount = 0;
        private int auditThreshold = 0;
        private string jobEmailTo = string.Empty;
        private string environment = string.Empty;
        private string outputPath = @"e:\AmericanEagle\Files\Outbound\";
        private string auditFileName = "BP_TlogAudit_" + DateTime.Now.ToString("MMddyyyy") + ".csv";
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        public struct AuditRecord
        {
            public int StoreNumber;
            public int AETransactionCount;
            public int AELoyaltyCount;
            public int BPTransactionCount;
            public int BPLoyaltyCount;
        }
        public IList<AuditRecord> lstAuditRecords = new List<AuditRecord>();

        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.    
        /// </summary>
        public void Dispose()
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string subject = string.Empty;
            StringBuilder sb = new StringBuilder();
            StreamWriter sw;
            string outString = string.Empty;

            // Tracing for starting of method
            this.logger.Trace(this.className, methodName, "Starts");

            this.logger.Trace(this.className, methodName, "Txn Audit Count: " + auditTxnCount.ToString());
            this.logger.Trace(this.className, methodName, "Loyalty Audit Count: " + auditLoyaltyCount.ToString());

            if ((auditTxnCount + auditLoyaltyCount) > auditThreshold)
            {
                sw = new StreamWriter(outputPath + auditFileName);
                this.logger.Trace(this.className, methodName, "Audit Count out of balance: ");
                sb.Append("Stores out of balance\n\r\n\r");
                sb.Append("Store#\tAE Txn Count\tAE Loyalty Count\tBP Txn Count\tBP LoyaltyCount\n\r");
                sw.WriteLine("Store#,AE Txn Count,AE Loyalty Count,BP Txn Count,BP LoyaltyCount");
                foreach (AuditRecord audit in lstAuditRecords)
                {
                    this.logger.Trace(this.className, methodName, "Store#: " + audit.StoreNumber.ToString());
                    this.logger.Trace(this.className, methodName, "AETransactionCount: " + audit.AETransactionCount.ToString());
                    this.logger.Trace(this.className, methodName, "AELoyaltyCount: " + audit.AELoyaltyCount.ToString());
                    this.logger.Trace(this.className, methodName, "BPTransactionCount: " + audit.BPTransactionCount.ToString());
                    this.logger.Trace(this.className, methodName, "BPLoyaltyCount: " + audit.BPLoyaltyCount.ToString());
                    sb.Append(string.Format("{0}\t{1}\t\t{2}\t\t\t{3}\t\t{4}", audit.StoreNumber, audit.AETransactionCount, audit.AELoyaltyCount, audit.BPTransactionCount, audit.BPLoyaltyCount) + "\n\r");
                    sw.WriteLine (string.Format("{0},{1},{2},{3},{4}", audit.StoreNumber, audit.AETransactionCount, audit.AELoyaltyCount, audit.BPTransactionCount, audit.BPLoyaltyCount));
                }
                subject = "Tlog Audit Out of Balance: " + DateTime.Now.ToShortDateString() + " (" + environment + ")";

                AEEmail.SendEmail_SMTP("aejobs@brierley.com", jobEmailTo, subject, sb.ToString());
                sw.Close();
            }

            // Tracing for starting of method
            this.logger.Trace(this.className, methodName, "End");
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
            string methodName = MethodBase.GetCurrentMethod().Name;
            string configKey = "TlogAuditThreshold";
            string configKey2 = "JobsEmailTo";
            using (var dtService = _dataUtil.DataServiceInstance())
            {
                this.logger.Trace(this.className, methodName, "Starts");
                ClientConfiguration clientConfig = dtService.GetClientConfiguration(configKey);
                if (clientConfig == null)
                {
                    throw new Exception(configKey + " not found in Client Config");
                }
                auditThreshold = Int32.Parse(clientConfig.Value);

                clientConfig = dtService.GetClientConfiguration(configKey2);
                if (clientConfig == null)
                {
                    throw new Exception(configKey2 + " not found in Client Config");
                }
                jobEmailTo = clientConfig.Value;
                this.logger.Trace(this.className, methodName, configKey + ": " + auditThreshold.ToString());
                this.logger.Trace(this.className, methodName, configKey2 + ": " + jobEmailTo);

                environment = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil().DataServiceInstance().Environment;

                if (environment == "Development")
                {
                    outputPath = @"c:\DevRoot\AmericanEagle\Files\Output\";
                }
                this.logger.Trace(this.className, methodName, "Ends");
            }
        }

        /// <summary>
        /// This method is called to process the messages in the batch
        /// </summary>
        /// <param name="messageBatch">String List</param>
        public void ProcessMessageBatch(List<string> messageBatch)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            int AETransactionCount = 0;
            int AELoyaltyCount = 0;
            int BPTransactionCount = 0;
            int BPLoyaltyCount = 0;

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
                XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/Global/TlogAudit");
                if (null != xmlNode)
                {


                    this.logger.Trace(this.className, methodName, "AEStoreNumber: " + xmlNode.Attributes["AEStoreNumber"].Value);
                    this.logger.Trace(this.className, methodName, "AETransactionCount: " + xmlNode.Attributes["AETransactionCount"].Value);
                    this.logger.Trace(this.className, methodName, "AELoyaltyCount: " + xmlNode.Attributes["AELoyaltyCount"].Value);
                    this.logger.Trace(this.className, methodName, "BPTransactionCount: " + xmlNode.Attributes["BPTransactionCount"].Value);
                    this.logger.Trace(this.className, methodName, "BPLoyaltyCount: " + xmlNode.Attributes["BPLoyaltyCount"].Value);

                    AETransactionCount = Int32.Parse(xmlNode.Attributes["AETransactionCount"].Value);
                    AELoyaltyCount = Int32.Parse(xmlNode.Attributes["AELoyaltyCount"].Value);
                    BPTransactionCount = Int32.Parse(xmlNode.Attributes["BPTransactionCount"].Value);
                    BPLoyaltyCount = Int32.Parse(xmlNode.Attributes["BPLoyaltyCount"].Value);

                    if (AETransactionCount != BPTransactionCount)
                    {
                        ++auditTxnCount;
                        AuditRecord audit = new AuditRecord();
                        audit.AELoyaltyCount = AELoyaltyCount;
                        audit.AETransactionCount = AETransactionCount;
                        audit.BPLoyaltyCount = BPLoyaltyCount;
                        audit.BPTransactionCount = BPTransactionCount;
                        audit.StoreNumber = Int32.Parse(xmlNode.Attributes["AEStoreNumber"].Value);
                        lstAuditRecords.Add(audit);
                    }
                    if (AELoyaltyCount != BPLoyaltyCount)
                    {
                        ++auditLoyaltyCount;
                    }

                }
                // Logging for ending of method
                this.logger.Trace(this.className, methodName, "Ends");
            }
            catch (Exception ex)
            {
                // Logging for exception
                this.logger.Error(this.className, methodName, ex.Message);
            }
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown()
        {
            return 0;
            //  throw new System.NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    }
}
