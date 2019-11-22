using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using AmericanEagle.SDK.Global;

namespace Brierley.Clients.AmericanEagle.Processors.ProcessRequestCredit
{


    /// <summary>
    /// Class Processor
    /// </summary>
    public class Processor 
    {
        /// <summary>
        /// string fileName
        /// </summary>
        private static string fileName = "ProcessRequestCreditJob " + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";

        /// <summary>
        /// string jobName
        /// </summary>
        private static string jobName = "ProcessRequestCreditJob";

        /// <summary>
        /// Logger for Processor
        /// </summary>
        private static LWLogger logger;

        /// <summary>
        /// Holds Class Name
        /// </summary>
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Holds Organization Name
        /// </summary>
        private string organizationName = null;

        /// <summary>
        /// Holds Environment Name
        /// </summary>
        private string environmentName = null;

        /// <summary>
        /// Data service instance
        /// </summary>
        private ILWDataService dataService;

        /// <summary>
        /// Initializes a new instance of the Processor class
        /// </summary>
        public Processor()
        {
            this.organizationName = Convert.ToString(ConfigurationManager.AppSettings["OrganizationName"]);
            if (string.IsNullOrEmpty(this.organizationName))
            {
                throw new Exception("OrganizationName key is either empty or not exists in app.config.");
            }

            // Checking environmentName key
            this.environmentName = Convert.ToString(ConfigurationManager.AppSettings["Environment"]);
            if (string.IsNullOrEmpty(this.environmentName))
            {
                throw new Exception("Environment key is either empty or not exists in app.config.");
            }

            LWConfigurationUtil.SetCurrentEnvironmentContext(this.organizationName, this.environmentName);
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            this.dataService = LWDataServiceUtil.DataServiceInstance(true);
            logger = LWLoggerManager.GetLogger("Processor");
        }

        /// <summary>
        /// Method Process
        /// </summary>
        public void Process()
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            DateTime startDate = DateTime.Parse("10/1/2011");
            int numberFound = 0;
            int numberExpired = 0;

            try
            {
                logger.Trace(className, methodName, "Begin " + methodName);

                long numberOfRecords = 0L;
                AsyncJob asyncJob = Utilities.StartJob(this.dataService, string.Empty, jobName);

                logger.Trace(className, methodName, "Get MemberReceipts");
                LWCriterion criteria = new LWCriterion("MemberReceipts");
                criteria.Add(LWCriterion.OperatorType.AND, "StatusCode", "1", LWCriterion.Predicate.Eq);
                criteria.Add(LWCriterion.OperatorType.AND, "CreateDate", startDate, LWCriterion.Predicate.Ge);
                criteria.AddOrderBy("CreateDate", LWCriterion.OrderType.Ascending);

                IList<IClientDataObject> memberReceipts = this.dataService.GetAttributeSetObjects(null, "MemberReceipts", criteria, new LWQueryBatchInfo(), false);

                foreach (MemberReceipts receipt in memberReceipts)
                {
                    IList<IClientDataObject> historyRecords = null;
                    HistoryTxnDetail transactionDetail = null;
                    Member member = null;
                    long rowKey = receipt.RowKey;
                    long memberIpCode = receipt.IpCode;

                    if (this.ValidateReceipt(receipt))
                    {
                        //Porceed only when it is not expired
                        if (receipt.ExpirationDate < DateTime.Now)
                        {
                            ((IClientDataObject)receipt).StatusCode = (long)ReceiptStatus.NoMatch;
                            ((IClientDataObject)receipt).IsDirty = true;
                            this.dataService.SaveAttributeSetObject(receipt);

                            // Send Email Here 
                            SendEmail(memberIpCode, receipt, false);
                            logger.Trace(className, methodName, "Expire Receipt Processed with IpCode : " + memberIpCode);
                            numberExpired++;
                        }
                        else
                        {
                            // PI 30364 - Dollar reward program - Start
                            member = this.dataService.LoadMemberFromIPCode(memberIpCode);
                            historyRecords = LoyaltyTransaction.SearchForHistoryTxnDetails(receipt.TxnNumber, receipt.TxnStoreID, receipt.TxnRegisterNumber, receipt.TxnDate, receipt.TenderAmount.ToString(), receipt.OrderNumber, null);
                            // PI 30364 - Dollar reward program - End
                            transactionDetail = historyRecords.FirstOrDefault() as HistoryTxnDetail;

                            if (null == transactionDetail)
                            {
                                logger.Trace(className, methodName, "Txn Not Found");
                            }
                            else
                            {
                                //member = this.dataService.LoadMemberFromIPCode(memberIpCode); This line commented out for PI 30364 - Dollar reward program
                                if (member != null)
                                {
                                    logger.Trace(className, methodName, "Txn Found - Add Txn");

                                    //PI15227 - check if AddLoyaltyTransaction is successful before changing StatusCode
                                    bool isSuccess;
                                    LoyaltyTransaction.AddLoyaltyTransaction(member, transactionDetail.TxnHeaderID, ProcessId.RequestCreditProcesssed, "Request Credit", out isSuccess);
                                    logger.Trace(className, methodName, "isSuccess: " + isSuccess);

                                    if (isSuccess == true)
                                    {
                                        ((IClientDataObject)receipt).StatusCode = (long)ReceiptStatus.Posted;
                                        ((IClientDataObject)receipt).IsDirty = true;
                                        receipt.TxnHeaderID = transactionDetail.TxnHeaderID;
                                        this.dataService.SaveAttributeSetObject(receipt);
                                        logger.Trace(className, methodName, "Loyalty Transaction added for member with IpCode : " + memberIpCode);
                                        SendEmail(memberIpCode, receipt, true);
                                        numberFound++;
                                    }                                
                                    else
                                    { 
                                        //PI14342 - Change Status from Processing to Posted if GetPoint txn has already been fulfilled by other actions
                                        foreach (VirtualCard vc in member.LoyaltyCards)
                                        {
                                            IList<IClientDataObject> hdrs = vc.GetChildAttributeSets("TxnHeader");
                                            if (hdrs != null && hdrs.Count > 0 && vc.IpCode == memberIpCode)
                                            {
                                                foreach (IClientDataObject hdr in hdrs)
                                                {
                                                    TxnHeader txnHeader = (TxnHeader)hdr;

                                                    if (txnHeader.TxnHeaderId != null && txnHeader.TxnHeaderId == transactionDetail.TxnHeaderID)
                                                    {
                                                        logger.Trace(className, methodName, "TxnHeaderId: " + txnHeader.TxnHeaderId);
                                                        ((IClientDataObject)receipt).StatusCode = (long)ReceiptStatus.Posted;
                                                        ((IClientDataObject)receipt).IsDirty = true;
                                                        receipt.TxnHeaderID = transactionDetail.TxnHeaderID;
                                                        this.dataService.SaveAttributeSetObject(receipt);
                                                    }
                                                }
                                            }

                                        } 

                                    }
                                }
                                else
                                {
                                    logger.Trace(className, methodName, "Member not found with IpCode : " + memberIpCode);
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Error(className, methodName, "Required Parameter not found in member receipt. Receipt Type : " + receipt.ReceiptType.ToString());
                    }

                    numberOfRecords++;
                }

                Utilities.StopJob(this.dataService, asyncJob.JobId, numberOfRecords);
                StringBuilder sb = new StringBuilder();
                sb.Append("Number of Records Processed: " + numberOfRecords.ToString() + "\n\r\n\r");
                sb.Append("Number of Records Found: " + numberFound.ToString() + "\n\r\n\r");
                sb.Append("Number of Records Expired: " + numberExpired.ToString() + "\n\r\n\r");

                logger.Trace(className, methodName, "End " + methodName);
            }
            catch (Exception ex)
            {
                logger.Error(className, methodName, ex.Message, ex);
                SendJobEmail("Request Credit Job Error in Process", ex.Message);
                throw;
            }
        }

        private void SendEmail(long ipcode, MemberReceipts receipt, bool txnFound)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;

            logger.Trace(className, methodName, "Begin");

            try
            {
                Member member = this.dataService.LoadMemberFromIPCode(ipcode);
                IList<IClientDataObject> memberDetails = member.GetChildAttributeSets("MemberDetails");
                MemberDetails mbrDetails = (MemberDetails)memberDetails[0];
                if (member != null)
                {
                    string loyaltyIdNumber = Utilities.GetVirtualCard(member).LoyaltyIdNumber;
                    Dictionary<string, string> additionalFields = new Dictionary<string, string>();

                    if (txnFound)
                    {
                        additionalFields.Add("firstname", member.FirstName);
                        AEEmail.SendEmail(member, EmailType.RequestCreditReceiptFound, additionalFields, mbrDetails.EmailAddress);
                    }
                    else
                    {
                        if (receipt.ReceiptType == (Int64)ReceiptTypes.InStore)
                        {
                            DateTime txnDate = (DateTime)receipt.TxnDate;
                            additionalFields.Add("firstname", member.FirstName);
                            additionalFields.Add("loyaltynumber", loyaltyIdNumber);
                            additionalFields.Add("storenumber", receipt.TxnStoreID);
                            additionalFields.Add("registernumber", receipt.TxnRegisterNumber);
                            additionalFields.Add("TransactionNumber", receipt.TxnNumber);
                            additionalFields.Add("TransactionDate", txnDate.ToShortDateString());
                            AEEmail.SendEmail(member, EmailType.RequestCreditExpiredInStore, additionalFields, mbrDetails.EmailAddress);
                        }
                        else
                        {
                            additionalFields.Add("firstname", member.FirstName);
                            additionalFields.Add("loyaltynumber", loyaltyIdNumber);
                            additionalFields.Add("OrderNumber", receipt.OrderNumber);
                            additionalFields.Add("OrderAmount", receipt.TenderAmount.ToString());
                            AEEmail.SendEmail(member, EmailType.RequestCreditExpiredWeb, additionalFields, mbrDetails.EmailAddress);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(className, methodName, ex.Message);
                SendJobEmail("Request Credit Job Error in Send Member Email", ex.Message);
            }
            logger.Trace(className, methodName, "End");
        }
        /// <summary>
        /// Method ValidateReceipt
        /// </summary>
        /// <param name="receipt">MemberReceipt object to validate</param>
        /// <returns>true if valid receipt else false</returns>
        private bool ValidateReceipt(MemberReceipts receipt)
        {
            bool isValidReceipt = true;

            if (receipt.ReceiptType == (long)ReceiptTypes.InStore)
            {
                if (string.IsNullOrEmpty(receipt.TxnNumber) || string.IsNullOrEmpty(receipt.TxnStoreID) || string.IsNullOrEmpty(receipt.TxnRegisterNumber) || receipt.TenderAmount == 0.0 || receipt.TxnDate == null)
                {
                    isValidReceipt = false;
                }
            }
            else if (receipt.ReceiptType == (long)ReceiptTypes.Web)
            {
                if (receipt.TenderAmount == 0.0)
                {
                    isValidReceipt = false;
                }

                if (string.IsNullOrEmpty(receipt.OrderNumber))
                {
                    isValidReceipt = false;
                }
            }
            else
            {
                isValidReceipt = false;
            }

            return isValidReceipt;
        }
        public void SendJobEmail(string subject, string body)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            StringBuilder sb = new StringBuilder();
            string outString = string.Empty;

            string configKey2 = "JobsEmailTo";
            string jobEmailTo = string.Empty;


            ClientConfiguration clientConfig = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration(configKey2);
            if (clientConfig == null)
            {
                throw new Exception(configKey2 + " not found in Client Config");
            }
            jobEmailTo = clientConfig.Value;

            // Tracing for starting of method
            logger.Trace(className, methodName, "Starts");


            AEEmail.SendEmail_SMTP("aejobs@brierley.com", jobEmailTo, subject, body);
            
            logger.Trace(className, methodName, "End");
        }

    }
}
