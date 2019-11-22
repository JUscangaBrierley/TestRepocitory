//-----------------------------------------------------------------------
// <copyright file="GetAccountActivity.cs" company="Brierley + Partners">
//     Copyright Brierley + Partners. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace AmericanEagle.SDK.OperationProviders
{
    #region | Name Space |
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.FrameWork.LWIntegration.Util;
    using Brierley.FrameWork.Common.Exceptions;   
    using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
    using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
    using Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders;
    using Brierley.ClientDevUtilities.LWGateway;
    #endregion

    #region Class Defination
    /// <summary>
    /// GetAccountActivity Custom Method Class
    /// Inherited by OperationProviderBase
    /// </summary>
    public class AEGetAccountActivity : OperationProviderBase
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
        private string startingPoints = string.Empty;
        private string basicPoints = string.Empty;
        private string bonusPoints = string.Empty;
        private string totalPoints = string.Empty;
        private string startingDate = string.Empty;
        private string endingDate = string.Empty;
        private string rewardLevel = string.Empty;

        #endregion

        #region Construction
        /// <summary>
        /// Initializes a new instance of the AEGetAccountActivity class by 
        /// invoking base class constructor
        /// </summary>
        public AEGetAccountActivity()
            : base("AEGetAccountActivity")
        {
        }
        #endregion

        #region Overriden Methods
        /// <summary>
        /// Invokes basic functionality of the Custom method "GetAccountActivity"
        /// </summary>
        /// <param name="source">source as string</param>
        /// <param name="parms">list of parameters</param>
        /// <returns>returns string</returns>
        public override string Invoke(string source, string parms)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            string loyaltyIdNumber = string.Empty;
            try
            {
                logger.Trace(this.className, methodName, "Starting Invoke");
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided for AmericanEagle GetAccountActivity.") { ErrorCode = 3300 };
                }

                // Capture the input arguments
                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

                APIArguments responseArgs;

                // TransactionHeader is the output struct for the AccountActivityList you will return in the response
                IList<TransactionHeaderInternal> headers = new List<TransactionHeaderInternal>();

                // Get Totalpagecount
                int totalPageCount;

                // Initialize Instance of dataservice
                ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();

                foreach (KeyValuePair<string, object> argument in args)
                {
                    logger.Trace(this.className, methodName, string.Format("Key: {0}, value: {1}", argument.Key, argument.Value));
                }
                ResponseCode validationStatus = this.ValidateInputs(args, _LoyaltyData, out totalPageCount, out headers, out loyaltyIdNumber);

                if (validationStatus != ResponseCode.Normal)
                {
                    // API 3.0.8 (If the loyalty number sent in the GetAccountActivity API call does not meet the above criteria, an error will be returned in the response with the corresponding error code)
                    throw new LWOperationInvocationException(Definitions.GetResponseMessage(validationStatus)) { ErrorCode = (int)validationStatus };
                }

                // responsArgs used to create response object
                // PI 30364 - Dollar reward program - Start
                Member member = _LoyaltyData.LoadMemberFromLoyaltyID(loyaltyIdNumber);
                MemberDetails memberDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                responseArgs = new APIArguments();
                if (memberDetails != null)
                {
                    Utilities.GetProgramDates(member, out startDate, out endDate);
                    // PI 30364 - Dollar reward program - Start
                    if (Utilities.isInPilot(memberDetails.ExtendedPlayCode)) // Pilot conversion
                    {
                        responseArgs.Add("DollarRewardMember", memberDetails.ExtendedPlayCode.Value.ToString());
                    }
                    else
                    {
                        responseArgs.Add("DollarRewardMember", "0");
                    }

                }
                // PI 30364 - Dollar reward program - End  
                responseArgs.Add("LoyaltyNumber", loyaltyIdNumber);
                responseArgs.Add("TotalPageCount", totalPageCount.ToString());
                responseArgs.Add("StartingPoints", startingPoints);
                responseArgs.Add("BasicPoints", basicPoints);
                responseArgs.Add("BonusPoints", bonusPoints);
                responseArgs.Add("TotalPoints", totalPoints);
                responseArgs.Add("StartDate", startingDate);
                responseArgs.Add("EndDate", endingDate);
                responseArgs.Add("RewardLevel", rewardLevel);

                APIStruct[] transactionArray = new APIStruct[headers.Count];
                int transCount = 0;
                foreach (TransactionHeaderInternal transactionItem in headers)
                {
                    APIArguments rparms = new APIArguments();
                    rparms.Add("TransactionNumber", transactionItem.TransactionNumber);
                    rparms.Add("StoreName", transactionItem.StoreName);
                    rparms.Add("PurchaseDate", transactionItem.PurchaseDate);
                    rparms.Add("Description", transactionItem.Description);
                    rparms.Add("Points", transactionItem.Points);
                    rparms.Add("StoreNumber", transactionItem.StoreNumber);
                    APIStruct transactionHeader = new APIStruct() { Name = "TransactionHeader", IsRequired = false, Parms = rparms };
                    transactionArray[transCount++] = transactionHeader;
                }

                if (transactionArray != null && transactionArray.Length > 0)
                {
                    responseArgs.Add("TransactionHeaders", transactionArray);
                }

                response = SerializationUtils.SerializeResult(Name, Config, responseArgs);
                logger.Trace(this.className, methodName, "End Invoke");
                return response;
            }
            catch (LWOperationInvocationException ex)
            {
                logger.Error(this.className, methodName, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(this.className, methodName, ex.Message);
                throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.TechnicalDifficulties)) { ErrorCode = (int)ResponseCode.TechnicalDifficulties };
            }
        }

        /// <summary>
        /// Cleanup method is used to cleanup resources
        /// </summary>
        protected override void Cleanup()
        {
        }
        #endregion

        #region | Custome Method defination |

        /// <summary>
        /// GetTxnHistory sub routine fills TransactionheaderInternal item from TxnHeader item based on condition
        /// </summary>
        /// <param name="rowKeys">array of strings</param>
        /// <param name="header">Lists of TransactionHeaderInternal items</param>
        /// <param name="virtualCard">virtual card instance used to get TxnHeader item</param>
        private void GetTxnHistory(string[] rowKeys, ref IList<TransactionHeaderInternal> header, VirtualCard virtualCard)
        {
            try
            {
                ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();
                IContentService _ContentService = _dataUtil.ContentServiceInstance();

                // Build the Criterion object with the list of rowkeys of txnHeaders
                LWCriterion crit = new LWCriterion("TxnHeader");
                foreach (string key in rowKeys)
                {
                    if (key != null && key.Trim().Length > 0)
                    {
                        crit.Add(LWCriterion.OperatorType.AND, "RowKey", key, LWCriterion.Predicate.Eq);
                    }
                }

                var txnHeaders = _LoyaltyData.GetAttributeSetObjects(virtualCard, "TxnHeader", crit, null, false);

                // Loop through the txnHeaders and determine if the txn is for online or store (txn.OrderNumber length > 0 is web).  If Store then get store information.  
                if ((txnHeaders != null) && (txnHeaders.Count > 0))
                {
                    foreach (IClientDataObject cdo in txnHeaders)
                    {
                        var txnHeader = (TxnHeader)cdo;
                        var th = TransactionHeaderInternal.GetTransactionHeaderByRowKey(header, cdo.RowKey);
                        if (th != null)
                        {
                            if ((txnHeader.OrderNumber != null) && (txnHeader.OrderNumber.Length > 0))
                            {
                                th.TransactionNumber = txnHeader.OrderNumber;
                                th.StoreName = "Web Order";
                            }
                            else
                            {
                                th.TransactionNumber = txnHeader.TxnNumber;

                                // Get Store information using the following
                                var store = _ContentService.GetStoreDef(txnHeader.TxnStoreId);
                                if (store != null)
                                {
                                    th.StoreName = store.StoreName;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("AEGetAccountActivity", "GetTxnHistory", ex.Message);
            }
        }
        private void LoadAccountActivityData(Member member, DateTime _startDate, DateTime _endDate, int pageNumber, out IList<TransactionHeaderInternal> _headers)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            logger.Trace(className, methodName, "Begin");
            _headers = new List<TransactionHeaderInternal>();
            IList<IClientDataObject> txnHeaders = new List<IClientDataObject>();
            ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();
            // AEO-Redesign-2015 Begin
            IList<IClientDataObject> loDetails = member.GetChildAttributeSets("MemberDetails");
            MemberDetails details = ( loDetails == null || loDetails.Count == 0 ? null : loDetails[0] ) as MemberDetails;
            bool isPilot = details != null ? Utilities.isInPilot(details.ExtendedPlayCode): false; // pilot conversion
            // AEO-Redesign-2015 End

            DateTime startDate = _startDate.AddSeconds(-1);

            txnHeaders = AccountActivityUtil.GetAccountActivitySummary(member, _startDate, _endDate, true, Utilities.IncludeExpired(_startDate, member), null); // PI 30364 - Dollar reward program - an extra parameter is added in Utilities.IncludeExpired() method.
            
            if (txnHeaders != null && txnHeaders.Count > 0)
            {
                foreach (IClientDataObject cdo in txnHeaders)
                {
                    TxnHeader txnHeader = (TxnHeader)cdo;
                    var th = new TransactionHeaderInternal();
                    th.RowKey = txnHeader.RowKey;
                    logger.Trace(className, methodName, "txnHeader.RowKey: " + txnHeader.RowKey.ToString());

                    th.HeaderId = txnHeader.TxnHeaderId;
                    if ((txnHeader.OrderNumber != null) && (txnHeader.OrderNumber.Length > 0))
                    {
                        th.TransactionNumber = txnHeader.OrderNumber;
                        th.StoreName = "Web Order";
                    }
                    else
                    {
                        th.TransactionNumber = txnHeader.TxnNumber;
                        if (cdo.HasTransientProperty("Store"))
                        {
                            logger.Trace(className, methodName, "Store Exists: ");
                            var store = (StoreDef)cdo.GetTransientProperty("Store");
                            if (store != null)
                            {
                                th.StoreName = string.Format("{0}", store.StoreName);
                            }
                            th.StoreNumber = string.Format("{0}", store.StoreNumber);
                        }
                    }
                    
                    th.PurchaseDate = txnHeader.TxnDate.ToShortDateString();
                    logger.Trace(className, methodName, "th.TransactionNumber: " + th.TransactionNumber);


                    //points
                    if (cdo.HasTransientProperty("PointsHistory"))
                    {
                        IList<PointTransaction> points = (IList<PointTransaction>)cdo.GetTransientProperty("PointsHistory");
                        logger.Trace(className, methodName, "PointsHistory: " + points.Count.ToString());
                        foreach (var point in points)
                        {
                            logger.Trace(className, methodName, "points: " + point.Points.ToString());
                            string pointType = _LoyaltyData.GetPointType(point.PointTypeId).Name;
                            string pointEvent = _LoyaltyData.GetPointEvent(point.PointEventId).Name;

                            // PI 21845, aali, show the bonus or adjustment points as separate headers

                            // AEO-Redesign-2015 Begin
                            bool isBonus = isPilot ?
                                ( pointType == "AEO Connected Bonus Points" || ( pointType == "Bonus Points" ) || ( pointType == "Adjustment Bonus Points" ) || pointType == "Adjustment Points" ) :
                                ((pointType == "Bonus Points") || (pointType == "Adjustment Bonus Points") || pointType == "Adjustment Points") ;


                            if (isBonus)
                            // AEO-Redesign-2015 End
                            {
                                var bonus_th = new TransactionHeaderInternal();
                                bonus_th.PurchaseDate = point.TransactionDate.AddSeconds(1).ToShortDateString();
                                bonus_th.Points = point.Points.ToString();
                                bonus_th.PostDate = point.PointAwardDate;
                                bonus_th.PointType = _LoyaltyData.GetPointType(point.PointTypeId).Name;
                                bonus_th.Description = _LoyaltyData.GetPointEvent(point.PointEventId).Name;

                                _headers.Add(bonus_th);
                            }
                            else
                            {
                                th.Points = point.Points.ToString();
                                th.PostDate = point.PointAwardDate;
                                th.PointType = _LoyaltyData.GetPointType(point.PointTypeId).Name;
                                th.Description = _LoyaltyData.GetPointEvent(point.PointEventId).Name;
                            }
                        }
                    }
                    _headers.Add(th);
                    // PI 27532, Akbar, Gift Card bonus points - start
                    IList<PointTransaction> giftCardPoints = Utilities.GetGiftCardBonusPoints(txnHeader);
                    if (giftCardPoints != null && giftCardPoints.Count > 0)
                    {
                        foreach (var point in giftCardPoints)
                        {
                            TransactionHeaderInternal giftHeader = new TransactionHeaderInternal();
                            giftHeader.PurchaseDate = point.TransactionDate.AddSeconds(1).ToShortDateString();
                            giftHeader.Points = point.Points.ToString();
                            giftHeader.PostDate = point.PointAwardDate;
                            giftHeader.PointType = _LoyaltyData.GetPointType(point.PointTypeId).Name;
                            giftHeader.Description = _LoyaltyData.GetPointEvent(point.PointEventId).Name;
                            _headers.Add(giftHeader);
                        }
                    }
                    // PI 27532, Akbar, Gift Card bonus points - end

                    //2015 Program Redesign - Adding code to show the returned reward points. - begin
                    Brierley.FrameWork.Common.PointBankTransactionType[] txnTypes = new Brierley.FrameWork.Common.PointBankTransactionType[2];
                    txnTypes[0] = Brierley.FrameWork.Common.PointBankTransactionType.Credit;
                    txnTypes[1] = Brierley.FrameWork.Common.PointBankTransactionType.Debit;
                    long[] pTypeIDs = new long[1];
                    long[] pEventIDs = new long[1];
                    
                    PointType pType = _LoyaltyData.GetPointType("AEO Connected Points");
                    PointEvent pEvent = _LoyaltyData.GetPointEvent("Returned Reward Points");
                    pTypeIDs[0] = pType.ID;
                    pEventIDs[0] = pEvent.ID;

                    IList<PointTransaction> returnRewards = _LoyaltyData.GetPointTransactionsByPointTypePointEvent(member, _startDate, _endDate, txnTypes, pTypeIDs, pEventIDs, null, null, null, false, null);
                    foreach (PointTransaction item in returnRewards)
                    {
                        TransactionHeaderInternal returnRewardHeader = new TransactionHeaderInternal();
                        returnRewardHeader.PurchaseDate = item.TransactionDate.AddSeconds(1).ToShortDateString();
                        returnRewardHeader.Points = item.Points.ToString();
                        returnRewardHeader.PostDate = item.PointAwardDate;
                        returnRewardHeader.PointType = _LoyaltyData.GetPointType(item.PointTypeId).Name;
                        returnRewardHeader.Description = _LoyaltyData.GetPointEvent(item.PointEventId).Name;
                        _headers.Add(returnRewardHeader);

                    }
                    //2015 Program Redesign - Adding code to show the returned reward points. - end
                }
            }
            //PI23097 - bonus not displayed
            IList<PointType> pointTypes = _LoyaltyData.GetAllPointTypes();
            long[] pointTypeIDs = new long[pointTypes.Count];
            int index = 0;

            
            foreach (PointType pt in pointTypes)
            {
                // PI 21845, aali, include the adjustment and bonus adjustment points in the display list

                // AEO-Redesign-2015 Begin
                bool isExcluded = isPilot ?
                  ( pt.Name.ToUpper() == "AEO CONNECTED POINTS" ) || ( ( pt.Name.ToUpper() == "BASIC POINTS" ) || ( pt.Name.ToUpper() == "STARTINGPOINTS" ) || ( pt.Name.ToUpper().Contains("BRA") ) || ( pt.Name.ToUpper().Contains("JEAN") ) ) :
                   ( ( pt.Name.ToUpper() == "BASIC POINTS" ) || ( pt.Name.ToUpper() == "STARTINGPOINTS" ) || ( pt.Name.ToUpper().Contains("BRA") ) || ( pt.Name.ToUpper().Contains("JEAN") ) );

                if ( isExcluded )
                    // AEO-Redesign-2015 End
                {
                    //Exclude these PointTypes
                }
                else
                {
                    pointTypeIDs[index] = pt.ID;
                    logger.Trace(className, methodName, "pointTypeIDs: " + pointTypeIDs[index].ToString());
                    ++index;
                }
            }

            IList<PointTransaction> filteredList = _LoyaltyData.GetPointTransactionsByPointTypePointEvent(member, _startDate, _endDate, null, pointTypeIDs, null, Brierley.FrameWork.Common.PointTransactionOwnerType.Unknown, -1, null, Utilities.IncludeExpired(_startDate, member), null); // PI 30364 - Dollar reward program - an extra parameter is added in Utilities.IncludeExpired() method.

            if (filteredList != null && filteredList.Count > 0)
            {
                logger.Trace(className, methodName, "Load Point Txns: " + filteredList.Count.ToString());             
                foreach (PointTransaction txn in filteredList)
                {
                    var th = new TransactionHeaderInternal();
                    th.PurchaseDate = txn.TransactionDate.ToShortDateString();
                    th.Points = txn.Points.ToString();
                    th.PostDate = txn.PointAwardDate;
                    th.PointType = _LoyaltyData.GetPointType(txn.PointTypeId).Name;
                    th.Description = _LoyaltyData.GetPointEvent(txn.PointEventId).Name;
                    
                    _headers.Add(th);
                }
            }

            logger.Trace(className, methodName, "Sort the list");
            
            var newHeaders = _headers.OrderBy(x => x.PurchaseDate).ThenBy(x => x.TransactionNumber).ThenBy(x => x.StoreName);

            _headers = newHeaders.ToList();

            logger.Trace(className, methodName, "End");
        }

        /// <summary>
        /// Validate all Input based on Buisness Rule
        /// </summary>
        /// <param name="args">Basic arguments provided as Input</param>
        /// <param name="dataservice">Instance of Member based on LoyaltyNumberId</param>
        /// <param name="totalPageCount">returns TotalPagecount</param>
        /// <param name="headers">Instance of (List of TransactionHeaderInternal) passed</param>
        /// <returns>Validation Status as Response Code</returns>
        private ResponseCode ValidateInputs(APIArguments args, ILoyaltyDataService dataservice, out int totalPageCount, out IList<TransactionHeaderInternal> headers, out string loyaltyIdNumber)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            totalPageCount = 0;
            logger.Trace(this.className, methodName, "Starting ");
            headers = new List<TransactionHeaderInternal>();
            loyaltyIdNumber = string.Empty;
            string loyaltyNumber = args.ContainsKey("LoyaltyNumber") ? (string)args["LoyaltyNumber"] : string.Empty;
            int pageNumber = args.ContainsKey("PageNumber") ? (int)args["PageNumber"] : 0;

            // API 3.0.1 Validate Inputs
            // Validation LoyaltyNumber API 3.0.3,3.0.4,,3.0.5
            // API 3.0.4 (The loyalty number must be present in the API call)
            loyaltyNumber = loyaltyNumber.Trim();
            if (loyaltyNumber.Trim().Length == 0)
            {
                return ResponseCode.LoyaltyNumberRequired;
            }

            long loyaltyNumberLong;
            if (long.TryParse(loyaltyNumber, out loyaltyNumberLong))
            {
                // API 3.0.5 (The loyalty number must be numeric with 14 digits and pass the check-digit algorithm)
                if (!LoyaltyCard.IsLoyaltyNumberValid(loyaltyNumberLong))
                {
                    return ResponseCode.InvalidLoyaltyNumber;
                }
            }
            else
            {
                return ResponseCode.InvalidLoyaltyNumber;
            }

            // Validate StartDate
            DateTime startDate = DateTime.MinValue;
            if (!args.ContainsKey("StartDate"))
            {
                return ResponseCode.StartDateRequired;
            }

            if (!DateTime.TryParse(args["StartDate"].ToString(), out startDate))
            {
                return ResponseCode.InvalidStartDate;
            }

            if (startDate == DateTime.MinValue)
            {
                return ResponseCode.InvalidStartDate;
            }
            else
            {
                // PI 30364 - Dollar reward program - Start
                Member dollarRewardMember = dataservice.LoadMemberFromLoyaltyID(loyaltyNumber);
                if (null != dollarRewardMember)
                {
                    MemberDetails memberDetails = dollarRewardMember.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                    if (startDate >= Utilities.DollarRewardsProgramStartDate && startDate <= Utilities.DollarRewardsProgramEndDate && (memberDetails.ExtendedPlayCode.Value == 3)) // AEO-57 catering ExtendedPlayCode=3 instead of 1
                    {
                        if (!(startDate.Day == 1))
                        {
                            return ResponseCode.InvalidStartDate;
                        }
                    }
                    else
                    {
                        // API 3.0.10 (The account balance detail shall be retrieved for the earning period specified by the "startDate")
                        if (!((startDate.Day == 1) && ((startDate.Month == 1) || (startDate.Month == 4) || (startDate.Month == 7) || (startDate.Month == 10))))
                        {
                            // API 3.0.14 (If the value of the 'startDate' is not a valid quarter start date, an error response shall be returned with the corresponding error code.)
                            return ResponseCode.InvalidStartDate;
                        }

                    }
                    // PI 30364 - Dollar reward program - End
                }
                else// AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                {
                    // Requirement API 8.0.7
                    // Throw an exception stating "Loyalty account not found"
                    return ResponseCode.LoyaltyAccountNotFound;
                }
                // AEO-74 Upgrade 4.5 changes end here -----------SCJ

            }

            // Validate EndDate
            DateTime endDate = DateTime.MinValue;
            if (!args.ContainsKey("EndDate"))
            {
                return ResponseCode.EndDateRequired;
            }

            if (!DateTime.TryParse(args["EndDate"].ToString(), out endDate))
            {
                return ResponseCode.InvalidEndDate;
            }

            if (endDate == DateTime.MinValue)
            {
                return ResponseCode.InvalidEndDate;
            }

            // Validate PageNumber
            if (pageNumber == 0)
            {
                return ResponseCode.PageNumberRequired;
            }

            if (pageNumber < 0 || pageNumber > 999)
            {
                // API 3.0.15 (If the value of the 'pageNumber' is not a valid numeric value, an error response shall be returned with the corresponding error code)
                return ResponseCode.InvalidPageNumber;
            }


            // Get Member based on LoyaltyNumber
            // API 
            var member = dataservice.LoadMemberFromLoyaltyID(loyaltyNumber);
            if (member != null)
            {
                // API 3.0.7 (The loyalty number must not have an account status of 'Terminated' or 'Archived')
                // Archived is not supported in current version of Loyaltyware.
                if (member.MemberStatus == MemberStatusEnum.Terminated)
                {
                    return ResponseCode.LoyaltyAccountTerminated;
                }

                // Validate MemberSource -API 3.0.6
                var memberDetail = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;

                if (memberDetail != null)
                {
                    if (memberDetail.MemberSource == (int)MemberSource.OnlineAEEnrolled)
                    {
                        //valid member source
                    }
                    else if (memberDetail.MemberSource == (int)MemberSource.OnlineAERegistered)
                    {
                        //valid member source
                    }
                    else
                    {
                        // Based on B+P suggestion (by email dated-Friday, August 05, 2011 4:36 AM)
                        // API 3.0.6 (The loyalty number must already exist in the loyalty database as a card already registered at ae.com)
                        return ResponseCode.LoyaltyAccountNotFound;
                    }
                    // PI 30364 - Dollar reward program
                    startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                    // Make sure to reach the day end of the last day of quarter
                    endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                    startingPoints = Utilities.GetStartingPoints(member, startDate, endDate);
                    basicPoints = Utilities.GetBasicPoints(member, startDate, endDate);
                    bonusPoints = Utilities.GetBonusPoints(member, startDate, endDate);
                    totalPoints = Utilities.GetTotalPoints(member, startDate, endDate);
                    startingDate = startDate.ToShortDateString();
                    endingDate = endDate.ToShortDateString();

                    // Requirement API 8.0.13
                    // Check if member details is not null and EmployeeCode is equal is 1, in this scenario add Empty RewardLevel to 
                    // response message otherwise get RewardLevel from Utilities method and add it to response.
                    if (null != memberDetail && memberDetail.EmployeeCode == 1)
                    {
                        rewardLevel = string.Empty;
                    }
                    else
                    {
                        rewardLevel = Utilities.GetCurrentRewardLevel(member, startDate, endDate);
                    }
                }

                // Get Total Page Count
                // Use this code to return the total number of transactions and then use this to get the total page count.  
                var pointTxnCount = dataservice.GetPointTransactions(member, startDate, endDate,null, null, 0, 999999,Utilities.IncludeExpired(startDate, member)); // PI 30364 - Dollar reward program - an extra parameter is added in Utilities.IncludeExpired() method.
                
                var virtualCard = Utilities.GetVirtualCard(member);
                loyaltyIdNumber = virtualCard.LoyaltyIdNumber;

                // Get Total Page Count
                //totalPageCount = (pointTxnCount.Count / 10) + 1;
                int remainder = 0;
                totalPageCount = pointTxnCount.Count / 10;
                remainder = pointTxnCount.Count % 10;
                if (remainder > 0)
                {
                    ++totalPageCount;
                }


                // API 3.0.16 (If the value of the 'pageNumber is not a valid page number for the loyalty number in 
                // the call, a response of "No Transaction Data Available" will be sent in the response.
                // Example: Loyalty account has 1 page of transaction data, but 'pageNumber' sent in the call = '2' 
                // response of 'No Transaction Data Available' shall be returned)
                if (pageNumber > totalPageCount)
                {
                    return ResponseCode.NoTransactionDataAvailable;
                }

                // This code will return a list of transactions and points that are tied to a TxnHeader.  
                //int startIndex = pageNumber == 1 ? 0 : ((pageNumber - 1) * 10) + 1;
                int startIndex = pageNumber == 1 ? 0 : ((pageNumber - 1) * 10);


                LoadAccountActivityData(member, startDate, endDate, startIndex, out headers);
                // API 3.0.9 (The system checks for account activity during the date specified in the 'startDate' field) 
                if (headers == null || headers.Count == 0)
                {
                    return ResponseCode.NoTransactionDataAvailable;
                }

                //if (rowKeys.Count() > 0)
                //{
                //    this.GetTxnHistory(rowKeys, ref headers, virtualCard);
                //}
            }
            else
            {
                return ResponseCode.LoyaltyAccountNotFound;
            }

            logger.Trace(this.className, methodName, "End ");
            return ResponseCode.Normal;
        }

        #endregion

        #region Private Class
        /// <summary>
        /// Internal Class TransactionHeaderInternal by inheriting TransactionHeadersStruct
        /// The intention of this class is to extent property of existing class only.
        /// </summary>
        private class TransactionHeaderInternal
        {
            /// <summary>
            /// Gets or sets RowKey of Transaction
            /// </summary>
            public virtual long RowKey { get; set; }

            /// <summary>
            /// Gets or sets Description of Transaction
            /// </summary>
            public virtual string Description { get; set; }

            /// <summary>
            /// Gets or sets RowKey of Transaction
            /// </summary>
            public virtual string Points { get; set; }

            /// <summary>
            /// Gets or sets PurchaseDate of Transaction
            /// </summary>
            public virtual string PurchaseDate { get; set; }

            /// <summary>
            /// Gets or sets StoreName of Transaction
            /// </summary>
            public virtual string StoreName { get; set; }

            /// <summary>
            /// Gets or sets TransactionNumber of Transaction
            /// </summary>
            public virtual string TransactionNumber { get; set; }
            public virtual DateTime PostDate { get; set; }
            public virtual string PointType { get; set; }
            public virtual string HeaderId { get; set; }
            public virtual string StoreNumber { get; set; }
            public virtual long VcKey { get; set; }

            /// <summary>
            /// Static method used to get TransactionHeaderInternal item based on rowKey
            /// </summary>
            /// <param name="header">List of TransactionHeaderInternal</param>
            /// <param name="rowKey">rowkey used for matching corresponding value in TransactionHeaderInternal item</param>
            /// <returns>returns instance of matching TransactionHeaderInternal</returns>
            public static TransactionHeaderInternal GetTransactionHeaderByRowKey(IList<TransactionHeaderInternal> header, long rowKey)
            {
                return header.Where(ht => ht.RowKey == rowKey).FirstOrDefault();
            }
        }
        #endregion
    }
    #endregion
}
