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
    using System.Xml;
    using Brierley.ClientDevUtilities.LWGateway;
    #endregion

    #region Class Defination
    /// <summary>
    /// GetAccountActivity Custom Method Class
    /// Inherited by OperationProviderBase
    /// </summary>
    public class AEConnectedGetAccountActivity : OperationProviderBase
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
        private int rewardsearned = 0;
        private string basicPoints = string.Empty;
        private string bonusPoints = string.Empty;
        private string totalPoints = string.Empty;
        private string startingDate = string.Empty;
        private string endingDate = string.Empty;
        //private string rewardLevel = string.Empty;

        #endregion

        #region Construction
        /// <summary>
        /// Initializes a new instance of the AEGetAccountActivity class by 
        /// invoking base class constructor
        /// </summary>
        public AEConnectedGetAccountActivity()
            : base("AEConnectedGetAccountActivity")
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
                    throw new LWOperationInvocationException("No parameters provided for AmericanEagle AEConnectedGetAccountActivity.") { ErrorCode = 3300 };
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

                Member member = _LoyaltyData.LoadMemberFromLoyaltyID(loyaltyIdNumber);
                IList<MemberReward> memberRewards = _LoyaltyData.GetMemberRewards(member, null);

                foreach  (MemberReward x in memberRewards)
                {
                    RewardDef rew = _dataUtil.ContentServiceInstance().GetRewardDef(x.RewardDefId);
                    if(rew != null)
                    {
                        if ( rew.Name.ToLower().Contains( "connected" ) ) {
                            rewardsearned++;
                        }
                    }
                }                

                // PI 30364 - Dollar reward program - End  
                responseArgs = new APIArguments();
                responseArgs.Add("LoyaltyNumber", loyaltyIdNumber);
                responseArgs.Add("TotalPageCount", totalPageCount.ToString());
                responseArgs.Add("BasicPoints", basicPoints);
                responseArgs.Add("BonusPoints", bonusPoints);
                responseArgs.Add("TotalPoints", totalPoints);
                responseArgs.Add("NumberOfRewardsEarned", rewardsearned.ToString()); // aeo-1197 begin & end
                responseArgs.Add("StartDate", startingDate);
                responseArgs.Add("EndDate", endingDate);

                APIStruct[] transactionArray = new APIStruct[headers.Count];
                int transCount = 0;
                foreach (TransactionHeaderInternal transactionItem in headers)
                {
                    APIArguments rparms = new APIArguments();
                    rparms.Add("TransactionNumber", transactionItem.TransactionNumber);
                    rparms.Add("StoreName", transactionItem.StoreName);
                    rparms.Add("PurchaseDate", transactionItem.PurchaseDate.ToShortDateString());
                    rparms.Add("Description", transactionItem.Description);
                    rparms.Add("TotalPoints", transactionItem.TotalPoints);
                    rparms.Add("TransactionAmount", transactionItem.TransactionAmount);
                    rparms.Add("JeansPurchased", transactionItem.JeansPurchased.ToString());
                    rparms.Add("BrasPurchased", transactionItem.BraPurchased.ToString());


                    APIStruct transactionHeader = new APIStruct() { Name = "AEConnectedTransactionHeader", IsRequired = false, Parms = rparms };
                    transactionArray[transCount++] = transactionHeader;
                }

                if (transactionArray != null && transactionArray.Length > 0)
                {
                    responseArgs.Add("AEConnectedTransactionHeaders", transactionArray);
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

                if ( ex.Message.Contains("DateTime") ) {
                    bool errorInDates = false;
                    try {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(parms);
                        
                        XmlNode startDateNode = xmlDoc.GetElementsByTagName("Parm")[2];
                        XmlNode endDateNode   = xmlDoc.GetElementsByTagName("Parm")[3];

                        if (! DateTime.TryParse(startDateNode.InnerText, out startDate) ) {
                            errorInDates = true;
                            throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.InvalidStartDate)) { ErrorCode = (int)ResponseCode.InvalidStartDate};
                            
                        }

                        if ( !DateTime.TryParse(endDateNode.InnerText, out endDate) ) {
                            errorInDates = true;
                            throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.InvalidEndDate)) { ErrorCode = (int)ResponseCode.InvalidEndDate };
                        }
                    }
                    catch ( Exception ex2 ) {
                        
                        logger.Error(this.className, methodName, ex2.Message);
                        if ( errorInDates ) {
                            throw ex2;
                        }
                        else {
                            throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.TechnicalDifficulties)) { ErrorCode = (int)ResponseCode.TechnicalDifficulties };
                        }

                    }
                         
                }

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
            // Build the Criterion object with the list of rowkeys of txnHeaders
            LWCriterion crit = new LWCriterion("TxnHeader");
            foreach (string key in rowKeys)
            {
                if (key != null && key.Trim().Length > 0)
                {
                    crit.Add(LWCriterion.OperatorType.AND, "RowKey", key, LWCriterion.Predicate.Eq);
                }
            }

            var txnHeaders = _dataUtil.LoyaltyDataServiceInstance().GetAttributeSetObjects(virtualCard, "TxnHeader", crit, null, false);

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
                            var store = _dataUtil.ContentServiceInstance().GetStoreDef(txnHeader.TxnStoreId);
                            if (store != null)
                            {
                                th.StoreName = store.StoreName;
                            }
                        }
                    }
                }
            }
        }
        private void LoadAccountActivityData(Member member, DateTime _startDate, DateTime _endDate, int initIndex, out IList<TransactionHeaderInternal> _headers)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            logger.Trace(className, methodName, "Begin");
            _headers = new List<TransactionHeaderInternal>();
            IList<IClientDataObject> txnHeaders = new List<IClientDataObject>();
            ILoyaltyDataService Service = _dataUtil.LoyaltyDataServiceInstance();
            IDataService _data = _dataUtil.DataServiceInstance();           

            bool isPilot = true;

            DateTime startDate = _startDate.AddSeconds(-1);
            txnHeaders = AccountActivityUtil.GetAccountActivitySummary(member, _startDate, _endDate, true, Utilities.IncludeExpired(_startDate, member), null); 
            
            if (txnHeaders != null && txnHeaders.Count > 0)
            {
                foreach (IClientDataObject cdo in txnHeaders)
                {
                    TxnHeader txnHeader = (TxnHeader)cdo;
                    var th = new TransactionHeaderInternal();
                    th.RowKey = txnHeader.RowKey;

                    if(txnHeader.TxnTypeId == 8)
                    {
                        continue;
                    }

                    /* AEO-1197 */
                    th.TransactionAmount = txnHeader.TxnAmount == null ? 
                        decimal.Zero.ToString():
                        txnHeader.TxnAmount.ToString();

                    LWCriterion crit = new LWCriterion("TxnDetailItem");
                    crit.Add(LWCriterion.OperatorType.AND, "TxnHeaderId", txnHeader.TxnHeaderId, LWCriterion.Predicate.Eq);


                    var txndetails = Service.GetAttributeSetObjects(null, "TxnDetailItem", crit, null, true);
                                   
                    if  (!( txndetails == null || txndetails.Count == 0 ) )   {

                        List<string> braClassCodes = new List<string>();
                        List<string> jeansClassCodes = new List<string>();
                        try {
                            braClassCodes.AddRange(_data.GetClientConfiguration("BraPromoClassCodes").Value.Split(';'));
                        }
                        catch ( Exception ex ) {
                            logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Bra Class Codes not defined: " + ex.Message);
                        }
                        try {
                            jeansClassCodes.AddRange(_data.GetClientConfiguration("JeansPromoClassCodes").Value.Split(';'));
                        }
                        catch ( Exception ex ) {
                            logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Jeans Class Codes not defined: " + ex.Message);
                        }


                        foreach ( TxnDetailItem lotmp in txndetails ) {
                            if ( braClassCodes.IndexOf(lotmp.DtlClassCode) >= 0) {
                                th.BraPurchased++;
                            }
                            if (jeansClassCodes.IndexOf(lotmp.DtlClassCode) >= 0) {
                                th.JeansPurchased++;
                            }
                        }         
                    }                   

                    /* AEO-1197 */
                    logger.Trace(className, methodName, "txnHeader.RowKey: " + txnHeader.RowKey.ToString());

                    th.HeaderId = txnHeader.TxnHeaderId;
                    /* AEO-5225 BEGIN */
                    if ((txnHeader.OrderNumber != null) && (txnHeader.OrderNumber != "0") && (txnHeader.ShipDate != null)) //AEO-5225 Updated Condition
                    {
                        th.TransactionNumber = txnHeader.OrderNumber;
                        th.StoreName = "Web Order";
                    }
                    else if ((txnHeader.OrderNumber != null) && (txnHeader.OrderNumber != "0") && (txnHeader.TxnNumber == "0")) //AEO-5225 Updated Condition
                    {
                        th.TransactionNumber = txnHeader.OrderNumber;
                        th.StoreName = "Web Order";  //AEO-5225 Changed value of StoreName to "Web Order" 
                    }
                    /* AEO-5225 END */
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
                    
                    th.PurchaseDate = txnHeader.TxnDate;
                    logger.Trace(className, methodName, "th.TransactionNumber: " + th.TransactionNumber);


                    //points
                    if (cdo.HasTransientProperty("PointsHistory"))
                    {
                        IList<PointTransaction> points = (IList<PointTransaction>)cdo.GetTransientProperty("PointsHistory");
                        logger.Trace(className, methodName, "PointsHistory: " + points.Count.ToString());
                        foreach (var point in points)
                        {
                            logger.Trace(className, methodName, "points: " + point.Points.ToString());
                            string pointType = Service.GetPointType(point.PointTypeId).Name;
                            string pointEvent = Service.GetPointEvent(point.PointEventId).Name;

                            // PI 21845, aali, show the bonus or adjustment points as separate headers

                            // AEO-Redesign-2015 Begin
                            bool isBonus = isPilot ?
                                ( pointType == "AEO Connected Bonus Points" || ( pointType == "Bonus Points" ) || ( pointType == "Adjustment Bonus Points" ) || pointType == "Adjustment Points"  || pointType == "AEO Connected Engagement Points"):
                                ((pointType == "Bonus Points") || (pointType == "Adjustment Bonus Points") || pointType == "Adjustment Points") ;


                            if (isBonus)
                            // AEO-Redesign-2015 End
                            {
                               
                                    var bonus_th = new TransactionHeaderInternal();
                                    bonus_th.PurchaseDate = point.TransactionDate.AddSeconds(1);
                                    bonus_th.TotalPoints = point.Points.ToString();
                                    bonus_th.PostDate = point.PointAwardDate;
                                    bonus_th.PointType = Service.GetPointType(point.PointTypeId).Name;
                                    bonus_th.Description = Service.GetPointEvent(point.PointEventId).Name;

                                    _headers.Add(bonus_th);
                            
                            }
                            else
                            {
                                if (Service.GetPointEvent(point.PointEventId).Name.ToUpper() != "NETSPEND")
                                {
                                    th.TotalPoints = point.Points.ToString();
                                    th.PostDate = point.PointAwardDate;
                                    th.PointType = Service.GetPointType(point.PointTypeId).Name;
                                    th.Description = Service.GetPointEvent(point.PointEventId).Name;
                                }
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
                                giftHeader.PurchaseDate = point.TransactionDate.AddSeconds(1);
                                giftHeader.TotalPoints = point.Points.ToString();
                                giftHeader.PostDate = point.PointAwardDate;
                                giftHeader.PointType = Service.GetPointType(point.PointTypeId).Name;
                                giftHeader.Description = Service.GetPointEvent(point.PointEventId).Name;
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
                    
                    PointType pType = Service.GetPointType("AEO Connected Points");
                    PointEvent pEvent = Service.GetPointEvent("Returned Reward Points");
                    pTypeIDs[0] = pType.ID;
                    pEventIDs[0] = pEvent.ID;

                    IList<PointTransaction> returnRewards = Service.GetPointTransactionsByPointTypePointEvent(member, _startDate, _endDate, txnTypes,
                                                                        pTypeIDs, pEventIDs, null, null, null, false, null); // AEO-1197 begin & end

                   
                    foreach (PointTransaction item in returnRewards)
                    {
                        TransactionHeaderInternal returnRewardHeader = new TransactionHeaderInternal();
                        returnRewardHeader.PurchaseDate = item.TransactionDate.AddSeconds(1);
                        returnRewardHeader.TotalPoints = item.Points.ToString();
                        returnRewardHeader.PostDate = item.PointAwardDate;
                        returnRewardHeader.PointType = Service.GetPointType(item.PointTypeId).Name;
                        returnRewardHeader.Description = Service.GetPointEvent(item.PointEventId).Name;
                        _headers.Add(returnRewardHeader);

                    }
                    //2015 Program Redesign - Adding code to show the returned reward points. - end
                }
            }

            //PI23097 - bonus not displayed
            IList<PointType> pointTypes = Service.GetAllPointTypes();
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

            IList<PointTransaction> filteredList = Service.GetPointTransactionsByPointTypePointEvent(member, _startDate, _endDate, null, pointTypeIDs, null, Brierley.FrameWork.Common.PointTransactionOwnerType.Unknown, -1, null, Utilities.IncludeExpired(_startDate, member), null); // PI 30364 - Dollar reward program - an extra parameter is added in Utilities.IncludeExpired() method.

            if (filteredList != null && filteredList.Count > 0)
            {
                logger.Trace(className, methodName, "Load Point Txns: " + filteredList.Count.ToString());
               
                foreach (PointTransaction txn in filteredList)
                {
                        var th = new TransactionHeaderInternal();
                        th.PurchaseDate = txn.TransactionDate;
                        th.TotalPoints = txn.Points.ToString();
                        th.PostDate = txn.PointAwardDate;
                        th.PointType = Service.GetPointType(txn.PointTypeId).Name;
                        th.Description = Service.GetPointEvent(txn.PointEventId).Name;

                        _headers.Add(th);
               
                }
            }

            logger.Trace(className, methodName, "Sort the list");
            
            IList<TransactionHeaderInternal> newHeaders = _headers.OrderByDescending(x => x.PurchaseDate).ThenByDescending(x => x.TransactionNumber).ThenByDescending(x => x.StoreName).ToList<TransactionHeaderInternal>();


            if ( newHeaders.Count >= initIndex +1 ) {
                IList<TransactionHeaderInternal> _retval = new List<TransactionHeaderInternal>();
                
                for ( int count = initIndex , i = 0;  i < 20 && count <= newHeaders.Count-1; count++, i++ ) {
                    _retval.Add(newHeaders[count]);
                }
                _headers = _retval;
            }
            else {
                _headers.Clear();
            }
            
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
                    startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
                    // Make sure to reach the day end of the last day of quarter
                    endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);
                   /* AEO-1197 begin & end   startingPoints = Utilities.GetStartingPoints(member, startDate, endDate);*/
                    basicPoints = Utilities.GetBasicPoints(member, startDate, endDate);
                    bonusPoints = Utilities.GetBonusPoints(member, startDate, endDate);
                    totalPoints = Utilities.GetTotalPoints(member, startDate, endDate);
                    startingDate = startDate.ToShortDateString();
                    endingDate = endDate.ToShortDateString();
                }

                // Get Total Page Count
                // Use this code to return the total number of transactions and then use this to get the total page count.  
             
                var pointTxnCount = dataservice.GetPointTransactions(member, startDate, endDate,null, null, 0, 999999,Utilities.IncludeExpired(startDate, member)); // PI 30364 - Dollar reward program - an extra parameter is added in Utilities.IncludeExpired() method.
                
                var virtualCard = Utilities.GetVirtualCard(member);
                loyaltyIdNumber = virtualCard.LoyaltyIdNumber;

                // Get Total Page Count
             
                int remainder = 0;
                totalPageCount = pointTxnCount.Count / 20;
                remainder = pointTxnCount.Count % 20;
                if (remainder > 0)
                {
                    ++totalPageCount;
                }


                if (pageNumber > totalPageCount)
                {
                    return ResponseCode.NoTransactionDataAvailable;
                }

                // This code will return a list of transactions and points that are tied to a TxnHeader.  
                int startIndex = pageNumber == 1 ? 0 : ((pageNumber - 1) * 20); // AEO-1197


                LoadAccountActivityData(member, startDate, endDate, startIndex, out headers);
               

                // API 3.0.9 (The system checks for account activity during the date specified in the 'startDate' field) 
                if (headers == null || headers.Count == 0)
                {
                    return ResponseCode.NoTransactionDataAvailable;
                }

        
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

        
            /// Gets or sets RowKey of Transaction
            /// </summary>
            public virtual long RowKey { get; set; }

            /// <summary>
            /// Gets or sets Description of Transaction
            /// </summary>
            public virtual string Description { get; set; }

            /// <summary>
            /// Gets or sets Pints of Transaction
            /// </summary>
            public virtual string TotalPoints { get; set; }

            /// <summary>
            /// Gets or sets PurchaseDate of Transaction
            /// </summary>
            public virtual DateTime PurchaseDate { get; set; }

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
            public  virtual string TransactionAmount { get; set;}
            public  virtual int JeansPurchased { get; set;}
            public  virtual int BraPurchased { get; set;}

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
