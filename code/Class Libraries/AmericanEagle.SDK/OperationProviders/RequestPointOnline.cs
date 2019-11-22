// ----------------------------------------------------------------------
// <copyright file="RequestPointOnline.cs" company="Brierely and Partners">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace AmericanEagle.SDK.OperationProviders
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
    using Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders;
    using Brierley.FrameWork.Common.Exceptions;
    using Brierley.ClientDevUtilities.LWGateway;
    /// <summary>
    /// Class RequestPointOnline
    /// </summary>
    public class AERequestPointOnline : OperationProviderBase
    {
        /// <summary>
        /// Logger for GetLoyaltyProfile
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil(); ///AEO-2630
        /// <summary>
        /// Name of current class
        /// </summary>
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Initializes a new instance of the AERequestPointOnline class
        /// </summary>
        public AERequestPointOnline() : base("AERequestPointOnline")
        {
        }

        /// <summary>
        /// Method Invoke
        /// </summary>
        /// <param name="source">string source</param>
        /// <param name="parms">string parms</param>
        /// <returns>string value</returns>
        public override string Invoke(string source, string parms)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string response = string.Empty;
            string loyaltyNumberString = string.Empty;
            string orderNumber = string.Empty;
            string orderAmount = string.Empty;

            logger.Trace(className, methodName, "Starting " + methodName);

            // If passed in parameters are blank or null then raise an exception reporting the same.
            if (string.IsNullOrEmpty(parms))
            {
                //LW 4.1.14 change
                //throw new LWOperationInvocationException("No parameters provided for GetLoyaltyProfile") { ErrorCode = 3300 };
                throw new LWIntegrationException("No parameters provided for GetLoyaltyProfile") { ErrorCode = 3300 };
            }

            try
            {
                // Requirement API API 5.0.2
                // De-serialize input parameters and check their validity by calling the Validation function.
                APIArguments apiArguments = SerializationUtils.DeserializeRequest(Name, Config, parms);
                ResponseCode validationResult = this.ValidateInputs(apiArguments);
                ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();

                // If validation is successful [Response code is normal] proceed with next activities otherwise throw and exception
                // stating the reason of validation failure
                if (validationResult == ResponseCode.Normal)
                {
                    loyaltyNumberString = apiArguments["LoyaltyNumber"] as string;
                    loyaltyNumberString = loyaltyNumberString.Trim();

                    orderNumber = apiArguments["OrderNumber"] as string;
                    orderAmount = apiArguments["OrderAmount"] as string;

                    // Requirement API 5.0.5
                    // Load a member using the passed in LoyaltyMember as input parameter. If a member is found then proceed with
                    // next level of activities otherwise throw an exception stating the fact that "The member was not found"
                    Member member = _LoyaltyData.LoadMemberFromLoyaltyID(loyaltyNumberString);
                    if (null != member)
                    {
                        // Requirement API 5.0.6 
                        // If member member's status is not terminated then proceed to if block for next set of activities
                        // Otherwise throw an exception stating that member is terminated
                        if (!((int)member.MemberStatus == (int)MemberStatusAE.Terminated || (int)member.MemberStatus == (int)MemberStatusAE.Closed))
                        {
                            APIArguments responseData = new APIArguments();

                            // Try to load member's Primary Card if primary card is not available then load the most recently
                            // issued card also log that primary virtual card does not exist.
                            VirtualCard loyaltyCard = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                            if (loyaltyCard == null)
                            {
                                loyaltyCard = member.GetLoyaltyCardByType(VirtualCardSearchType.MostRecentIssued);
                                logger.Trace(className, methodName, "No primary virtual card exists.");
                            }

                            // Requirement API 5.0.13
                            // Add the LoyaltyIDNumber to Response
                            responseData.Add("LoyaltyNumber", loyaltyCard.LoyaltyIdNumber);

                            //PI14540 - disallow Get Points request for txn that has already been requested
                            int intResult = LoyaltyTransaction.SearchReceiptDetails(null, null, null, null, orderAmount, orderNumber);

                            if (intResult == (int)(ReceiptStatus.AlreadyRequested) | intResult == (int)(ReceiptStatus.AlreadyPosted))
                            {
                                /* NR  AEO-1202 begin */
                                ResponseCode rCode = intResult == ( (int) ReceiptStatus.AlreadyRequested) ?
                                                                                 ResponseCode.RequestPointsTransactionAlreadyRequested:
                                                                                ResponseCode.RequestPointsTransactionAlreadyPosted;

                                String errorMsg = Definitions.GetResponseMessage(rCode);
                                throw new LWIntegrationException(errorMsg) { ErrorCode = (int) rCode };
                                 
                            }

                            // RKG - 11/10/14 The Jira number is AEO-34; the PI is 29741.
                            // With new Tlog processing in the database we are changing all request for points in api and CS Portal to automatically go to the queue because we 
                            // no longer will do automatic lookups.
                            Dictionary<string, string> searchParams = new Dictionary<string, string>();
                            searchParams.Add("txt_OrderNumber", orderNumber.ToString());
                            // searchParams.Add("txt_TenderAmount", double.Parse(orderAmount).ToString("#,###.00")); // AEO-74 Upgrade 4.5 changes here -----------SCJ
                            searchParams.Add("txt_TenderAmount", decimal.Parse(orderAmount).ToString("#,###.00"));
                            LoyaltyTransaction.CreateMemberReceipt(member, searchParams, Global.TransactionType.Online, "System", (long)ReceiptStatus.Processing);
                            responseData.Add("ResponseMessage", "Transaction Added to Queue");
                            response = SerializationUtils.SerializeResult(Name, this.Config, responseData);

                        }
                        else
                        {
                            // Requirement API 5.0.12
                            // Throw an exception stating "Loyalty account terminated"
                            throw new LWIntegrationException(Definitions.GetResponseMessage(ResponseCode.LoyaltyAccountTerminated)) { ErrorCode = Convert.ToInt32(ResponseCode.LoyaltyAccountTerminated) };
                        }
                    }
                    else
                    {
                        // Requirement API 5.0.12
                        // Throw an exception stating "Loyalty account not found"
                        throw new LWIntegrationException(Definitions.GetResponseMessage(ResponseCode.LoyaltyAccountNotFound)) { ErrorCode = Convert.ToInt32(ResponseCode.LoyaltyAccountNotFound) };
                    }
                }
                else
                {
                    // Requirement API 5.0.12
                    // Throw an exception stating the reason of validation failure.
                    throw new LWIntegrationException(Definitions.GetResponseMessage(validationResult)) { ErrorCode = Convert.ToInt32(validationResult) };
                }
            }
            catch (LWIntegrationException exConfig)
            {
                logger.Error(className, methodName, exConfig.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(className, methodName, ex.Message);
            }

            logger.Trace(className, methodName, "Successfully completed " + methodName);
            return response;
        }

        /// <summary>
        /// Method ValidateInputs. Validates input arguments to the API and return a Normal response if validation is successful otherwise
        /// returns appropriate Response code stating the reason of validation failure.
        /// </summary>
        /// <param name="apiArguments">APIArguments apiArguments</param>
        /// <returns>bool result</returns>
        private ResponseCode ValidateInputs(APIArguments apiArguments)
        {
            long loyaltyNumber = 0L;
            string methodName = MethodBase.GetCurrentMethod().Name;

            logger.Trace(className, methodName, "Starting " + methodName);

            if (null != apiArguments)
            {
                // Requirement API 5.0.2, API 5.0.3, API 5.0.4
                // Check if LoyaltyNumber is present in provided arguments if yes then check its validity if its valid then proceed with
                // next validation check otherwise return appropriate response code.
                if (apiArguments.ContainsKey("LoyaltyNumber"))
                {
                    if (!(long.TryParse(apiArguments["LoyaltyNumber"] as string, out loyaltyNumber) && LoyaltyCard.IsLoyaltyNumberValid(loyaltyNumber)))
                    {
                        return ResponseCode.InvalidLoyaltyNumber;
                    }
                }
                else
                {
                    return ResponseCode.LoyaltyNumberRequired;
                }

                // Requirement API 5.0.8 and API 5.0.9
                // Check if OrderNumber is present in provided arguments if yes then check its validity if its valid then proceed with
                // next validation check otherwise return appropriate response code. Validity criteria is order number must be a alphanumeric
                // code with a length ranging from 5 to 8
                if (apiArguments.ContainsKey("OrderNumber"))
                {
                    if (!Utilities.IsOrderNumberValid(apiArguments["OrderNumber"] as string, 5, 10))
                    {
                        return ResponseCode.InvalidOrderNumber;
                    }
                }
                else
                {
                    return ResponseCode.OrderNumberRequired;
                }

                // Requirement API 5.0.8 and API 5.0.9
                // Check if OrderAmount is present in provided arguments if yes then check its validity if its valid then proceed with
                // otherwise return appropriate response code. Validation criteria is order amount must be less than 10000 and can have optional
                // 2 decimal places. Commas are also allowed.
                if (apiArguments.ContainsKey("OrderAmount"))
                {
                    if (!Utilities.IsOrderAmountValid(apiArguments["OrderAmount"] as string, 9999.99))
                    {
                        return ResponseCode.InvalidOrderAmount;
                    }
                }
                else
                {
                    return ResponseCode.OrderAmountRequired;
                }
            }

            logger.Trace(className, methodName, "Validation Successful. Successfully completed " + methodName);
            return ResponseCode.Normal;
        }
    }
}
