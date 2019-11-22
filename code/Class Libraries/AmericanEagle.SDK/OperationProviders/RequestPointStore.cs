//-----------------------------------------------------------------------
// <copyright file="RequestPointStore.cs" company="Brierley + Partners">
//     Copyright Brierley + Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace AmericanEagle.SDK.OperationProviders
{
    #region | Name Space |
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;

    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
    using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
    using Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders;
    using Brierley.FrameWork.Common.Exceptions;
    using Brierley.ClientDevUtilities.LWGateway;
    #endregion

    #region Class Defination
    /// <summary>
    /// RequestPointsStore Custom Method Class
    /// Inherited by OperationProviderBase
    /// </summary>
    public class AERequestPointStore : OperationProviderBase
    {
        #region Fields
        /// <summary>
        /// logger is used for proper logging
        /// </summary> 
        private static LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        /// <summary>
        /// className is used for logging
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        #endregion

        #region Construction
        /// <summary>
        /// Initializes a new instance of the RequestPointStore class by 
        /// invoking base class constructor
        /// </summary>
        public AERequestPointStore()
            : base("AERequestPointStore")
        {
        }
        #endregion

        #region Overriden Methods
        /// <summary>
        /// Invokes basic functionality of the Custom method "RequestPointStore"
        /// </summary>
        /// <param name="source">source as string</param>
        /// <param name="parms">list of parameters</param>
        /// <returns>returns string</returns>
        public override string Invoke(string source, string parms)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            try
            {
                logger.Trace(this.className, methodName, "Starting Invoke");
                ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();

                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for AmericanEagle RequestPointsStore.") { ErrorCode = 3300 };
                }

                // Capture the input arguments
                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                
                // responsArgs used to create response object
                APIArguments responseArgs = new APIArguments();

                Member member;
                HistoryTxnDetail transactionDetail;
                bool isResponsMessageSet = false;

                // Initialize Instance of dataservice               
                ResponseCode validationStatus = this.ValidateInputs(args, _LoyaltyData, out member, out transactionDetail);
                
                if (validationStatus == ResponseCode.Normal)
                {
                    if (transactionDetail != null)
                    {
                        //PI14342 - Post the Realtime GetPoint transaction in the GetPoint page
                        bool isSuccess = false;
                        LoyaltyTransaction.AddLoyaltyTransaction(member, transactionDetail.TxnHeaderID, ProcessId.RequestCreditProcesssed, "RequestPointStore api", out isSuccess);
       
                        if (isSuccess == true)
                        {
                            Dictionary<string, string> transactionSearchParams = new Dictionary<string, string>();
                            transactionSearchParams.Add("txt_TxnDate", transactionDetail.TxnDate.ToString());
                            transactionSearchParams.Add("txt_TxnNumber", transactionDetail.TxnNumber);
                            transactionSearchParams.Add("txt_TxnRegisterNumber", transactionDetail.TxnRegisterNumber);
                            transactionSearchParams.Add("txt_TxnStoreID", transactionDetail.StoreNumber);
                            transactionSearchParams.Add("txt_TenderAmount", string.Format("{0:#####.00}", (decimal)(transactionDetail.TenderAmount)));
                            LoyaltyTransaction.CreateMemberReceipt(member, transactionSearchParams, Global.TransactionType.Store, "System", (long)ReceiptStatus.Posted);


                            // API 4.0.9
                            // To do - Send mail to member
                            // The System will trigger an automated email to the member
                            // notifying that the points have been added to their account (Email=RequestCreditReceiptFound)
                            Utilities.CreateCSNote("Transaction in the amount of " + transactionDetail.TenderAmount + " was requested", member.IpCode, -1);
                        }
                    }
                    else
                    {
                        // API 4.0.9
                        isResponsMessageSet = true;
                        responseArgs.Add("ResponseMessage", "Transaction added to queue");
                    }
                }
                else
                {
                    // API 4.0.14 appropriate error message should be passed to ae website based on error code provided.
                    throw new LWOperationInvocationException(Definitions.GetResponseMessage(validationStatus)) { ErrorCode = (int)validationStatus };
                }

                // API 4.0.15 response should contain primary LoyaltyNumberID of account
                VirtualCard virtualCard = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                if (virtualCard == null)
                {
                    virtualCard = member.GetLoyaltyCardByType(VirtualCardSearchType.MostRecentIssued);
                    logger.Trace(this.className, methodName, "No primary virtual card exists.");
                }

                responseArgs.Add("LoyaltyNumber", (virtualCard != null ? virtualCard.LoyaltyIdNumber : string.Empty));
                if (!isResponsMessageSet)
                {
                    responseArgs.Add("ResponseMessage", "Normal");
                }

                string response = SerializationUtils.SerializeResult(Name, Config, responseArgs);
                logger.Trace(this.className, methodName, "End Invoke");
                return response;
            }
            catch (LWOperationInvocationException exConfig)
            {
                logger.Error(className, methodName, exConfig.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(className, methodName, ex.Message);
                throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.TechnicalDifficulties)) { ErrorCode = (int)ResponseCode.TechnicalDifficulties };
            }
        }

        /// <summary>
        /// Cleanup method is used to cleanup resources
        /// </summary>
        protected override void Cleanup()
        {
        }

        #region | Custome Method defination |

        /// <summary>
        /// Validate all Input based on Buisness Rule
        /// </summary>
        /// <param name="args">Basic arguments provided as Input</param>
        /// <param name="dataservice">Instance of Member based on LoyaltyNumberId</param>
        /// <param name="member">Instance of dataservice passed</param>
        /// <param name="transactionDetail">Instance of transactionDetail retuned by the function</param>
        /// <returns>Validation Status as Response Code</returns>
        private ResponseCode ValidateInputs(APIArguments args, ILoyaltyDataService dataservice, out Member member, out HistoryTxnDetail transactionDetail)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            logger.Trace(this.className, methodName, "Starting ");
            member = null;
            transactionDetail = null;
            string loyaltyNumber = args.ContainsKey("LoyaltyNumber") ? (string)args["LoyaltyNumber"] : string.Empty;
            string registerNumber = args.ContainsKey("RegisterNumber") ? (string)args["RegisterNumber"] : string.Empty;
            string storeNumber = args.ContainsKey("StoreNumber") ? (string)args["StoreNumber"] : string.Empty;
            string totalPayment = args.ContainsKey("TotalPayment") ? (string)args["TotalPayment"] : string.Empty;
            string transactionNumber = args.ContainsKey("TransactionNumber") ? (string)args["TransactionNumber"] : string.Empty;

            loyaltyNumber = loyaltyNumber.Trim();

            // API 4.0.8 Validate Inputs
            // Validate TransactionDate
            DateTime transactionDate = DateTime.MinValue;
            if (!args.ContainsKey("TransactionDate"))
            {
                return ResponseCode.TransactionDateRequired;
            }
            try
            {
                transactionDate = DateTime.Parse(args["TransactionDate"].ToString());
            }
            catch
            {
                throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.InvalidTransactionDate)) { ErrorCode = (int)ResponseCode.InvalidTransactionDate };
            }
            
            // Validation TransactionDate must belong to current quarter API 4.0.8, 4.0.13
            DateTime qtrStartDate;
            DateTime qtrEndDate;

            // PI 30364 - Dollar reward program - Start
            Utilities.GetProgramDates(null, out qtrStartDate, out qtrEndDate);
            // PI 30364 - Dollar reward program - End
           
            if (transactionDate < qtrStartDate || transactionDate > qtrEndDate)
            {
                return ResponseCode.PurchaseNotValidForCurrentEarningPeriod;
            }

            // Validation LoyaltyNumber API 4.0.2,4.0.3,4.0.4,4.0.7
            if (loyaltyNumber.Trim().Length == 0)
            {
                return ResponseCode.LoyaltyNumberRequired;
            }

            long loyaltyNumberLong;
            if (long.TryParse(loyaltyNumber, out loyaltyNumberLong))
            {
                if (!LoyaltyCard.IsLoyaltyNumberValid(loyaltyNumberLong))
                {
                    // API 4.0.4 Show corresponding error when loyaltyId is not valid
                    return ResponseCode.InvalidLoyaltyNumber;
                }
            }
            else
            {
                return ResponseCode.InvalidLoyaltyNumber;
            }

            // API 4.0.8
            // Validate StoreNumber
            if (registerNumber.Length == 0)
            {
                return ResponseCode.RegisterNumberRequired;
            }

            if (!Utilities.IsRegisterNumberValid(registerNumber))
            {
                return ResponseCode.InvalidRegisterNumber;
            }

            // Validate StoreNumber
            if (storeNumber.Length == 0)
            {
                return ResponseCode.StoreNumberRequired;
            }

            if (!Utilities.IsStoreNumberValid(storeNumber))
            {
                return ResponseCode.InvalidStoreNumber;
            }

            // Validate TotalPayment
            if (totalPayment.Length == 0)
            {
                return ResponseCode.TotalPaymentRequired;
            }

            if (!Utilities.IsTotalPaymentValid(totalPayment))
            {
                return ResponseCode.InvalidTotalPayment;
            }

            // replace comma - as comma is allowed in input
            totalPayment = totalPayment.Replace(",", string.Empty);

            // Validate TransactionNumber
            if (transactionNumber.Trim().Length == 0)
            {
                return ResponseCode.TransactionNumberRequired;
            }

            if (!Utilities.IsTransactionNumberValid(transactionNumber))
            {
                return ResponseCode.InvalidTransactionNumber;
            }
            
            // Get Member based on LoyaltyNumber
            member = dataservice.LoadMemberFromLoyaltyID(loyaltyNumber);
            if (member != null)
            {
                // Validate Member Status --API 4.0.6
                if ((int)member.MemberStatus == (int)MemberStatusAE.Terminated || (int)member.MemberStatus == (int)MemberStatusAE.Closed)
                {
                    return ResponseCode.LoyaltyAccountTerminated;
                }

                // API 4.0.5 The loyalty number must already exist in the loyalty database as a card already linked to an ae.com account
                MemberDetails memberDetail = memberDetail = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;

                if (memberDetail != null)
                {
                    //AEO-4156 Adding MemberSource that change since National rollout
                    if (!(memberDetail.MemberSource == (int)MemberSource.Web || memberDetail.MemberSource == (int)MemberSource.MobileApp
                        || memberDetail.MemberSource == (int)MemberSource.POSCredit || memberDetail.MemberSource == (int)MemberSource.POSOpos
                        || memberDetail.MemberSource == (int)MemberSource.POSMpos || memberDetail.MemberSource == (int)MemberSource.POSIDevice
                        || memberDetail.MemberSource == (int)MemberSource.CSR || memberDetail.MemberSource == (int)MemberSource.SYFSynchrony))
                    {
                        // Based on B+P suggestion (by email dated-Friday, August 05, 2011 4:36 AM)
                        // API 4.0.5 The loyalty number must already exist in the loyalty database as a card already linked to an ae.com account.
                        return ResponseCode.LoyaltyAccountNotFound;
                    }
                }

                //PI14540 - disallow Get Points request for txn that has already been requested
                int intResult = LoyaltyTransaction.SearchReceiptDetails(transactionNumber, storeNumber, registerNumber, transactionDate, totalPayment, null);

                if (intResult == (int)(ReceiptStatus.AlreadyRequested) | intResult == (int)(ReceiptStatus.AlreadyPosted))
                {
                    if (intResult == (int)(ReceiptStatus.AlreadyPosted))
                    {
                        return ResponseCode.RequestPointsTransactionAlreadyPosted;
                    }
                    else
                    {
                        return ResponseCode.RequestPointsTransactionAlreadyRequested;
                    }
                }

                // RKG - 11/10/14 The Jira number is AEO-34; the PI is 29741.
                // With new Tlog processing in the database we are changing all request for points in api and CS Portal to automatically go to the queue because we 
                // no longer will do automatic lookups.
                Dictionary<string, string> transactionSearchParams = new Dictionary<string, string>();
                transactionSearchParams.Add("txt_TxnDate", transactionDate.ToString());
                transactionSearchParams.Add("txt_TxnNumber", transactionNumber);
                transactionSearchParams.Add("txt_TxnRegisterNumber", registerNumber);
                transactionSearchParams.Add("txt_TxnStoreID", storeNumber);
                transactionSearchParams.Add("txt_TenderAmount", totalPayment);
                LoyaltyTransaction.CreateMemberReceipt(member, transactionSearchParams, Global.TransactionType.Store, "System", (long)ReceiptStatus.Processing);                

            }
            else
            {
                // Validate Loyalty Number --API 4.0.5
                return ResponseCode.LoyaltyAccountNotFound;
            }

            logger.Trace(this.className, methodName, "End ");
            return ResponseCode.Normal;
        }
        #endregion

        #endregion
    }
    #endregion
}
