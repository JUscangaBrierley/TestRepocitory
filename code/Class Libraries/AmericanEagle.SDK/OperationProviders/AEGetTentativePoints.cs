// ----------------------------------------------------------------------
// <copyright file="GetAccountSummary.cs" company="Brierely and Partners">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace AmericanEagle.SDK.OperationProviders
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;

    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.FrameWork.Common.Exceptions;
    using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
    using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
    using Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Brierley.ClientDevUtilities.LWGateway;
    /// <summary>
    /// Class TxnDetail
    /// </summary>
    class TransactionDetail
    {
        
        public String SKUNumber { get; set; }
        public String ClassCode { get; set; }
        public int Quantity { get; set; }
        public Decimal FinalSalePrice { get; set; }

    }
  
    /// <summary>
    /// Class TxnTender
    /// </summary>
    class TransactionTender
    {
        
        public int TenderType { get; set; }
        public Decimal TenderAmount { get; set; }

    }
    /// <summary>
    /// Class TentativePoints
    /// </summary>
    public class AEGetTentativePoints : OperationProviderBase
    {
        /// <summary>
        /// Logger for TentativePoints
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);
        private static ILWDataServiceUtil _dataUtil = new Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil();//AEO-2630
        /// <summary>
        /// className is used for logging
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private string methodName = MethodBase.GetCurrentMethod().Name;
        /// <summary>
        /// response-outbound parameters
        /// </summary>
        private string response = string.Empty;
        Member member;
        /// <summary>
        /// totalPoints-output parameter
        /// </summary>
        private decimal totalPoints = 0;
        /// <summary>
        /// loyaltyNumber of the member
        /// </summary>
        private string loyaltyNumber = string.Empty;
        /// <summary>
        /// transactionDate
        /// </summary>
        DateTime transactionDate = DateTime.MinValue;
        /// <summary>
        /// transactionNumber
        /// </summary>
        private string transactionNumber = string.Empty;
        /// <summary>
        /// response-outbound parameters
        /// </summary>
        private long storeNumber = 0;
        /// <summary>
        /// response-outbound parameters
        /// </summary>
        private decimal QPA = 0;
        /// <summary>
        /// BraCount-outbound parameters
        /// </summary>
        private decimal BraCount = 0;
        /// <summary>
        /// JeanCount-outbound parameters
        /// </summary>
        private decimal JeanCount = 0;
        /// <summary>
        /// transtndCount- tender counter
        /// /// </summary>
        private int transtndCount = 0;
        /// <summary>
        /// AECCMultiplier - Multiplier for AECC Bonus
        /// </summary>
        private int AECCMultiplier = 5;
        /// <summary>
        /// brapromocodes-list of bra classcodes in the bra promotion
        /// </summary>
        private string[] brapromocodes = null;
        /// <summary>
        /// jeanpromocodes list of jeans classcodes in the jeans promotion
        /// </summary>
        private string[] jeanpromocodes = null;
        /// <summary>
        /// txntenders array struct
        /// </summary>
        private APIStruct[] txntenders;
        /// <summary>
        ///  Results list of executed header rules 
        /// </summary>
        private List<Brierley.FrameWork.ContextObject.RuleResult> results = new List<Brierley.FrameWork.ContextObject.RuleResult>();
        /// <summary>
        ///  Results list of executed tender rules 
        /// </summary>
        private List<Brierley.FrameWork.ContextObject.RuleResult> ttresults = new List<Brierley.FrameWork.ContextObject.RuleResult>();
        /// <summary>
        /// Initializes a new instance of the TentativePoints class
        /// </summary>
        public AEGetTentativePoints()
            : base("AEGetTentativePoints")
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
            TxnHeader txnHeader = new TxnHeader();
            TxnTender[] TT = null;
            decimal tenderAmount = 0;
            decimal AECCAmount = 0;
            float percentAECC;
            ILoyaltyDataService _LoyaltyData = _dataUtil.LoyaltyDataServiceInstance();
            IDataService _DataService = _dataUtil.DataServiceInstance();

            logger.Trace(this.className, methodName, "Starting ");
            
            //Get Bra Promocodes     
            try
            {  
                brapromocodes = _DataService.GetClientConfiguration("BraPromoClassCodes").Value.Split(';');
            }
            catch (Exception ex)
            {  
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Bra Class Codes not defined: " + ex.Message);      
            }
            
            //Get Jean Promocodes      
            try
            {      
                jeanpromocodes = _DataService.GetClientConfiguration("JeansPromoClassCodes").Value.Split(';');           
            }
            catch (Exception ex)
            {      
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Jean Class Codes not defined: " + ex.Message);            
            }
            try
            {
                //Validate Inputs here- will raise exception if inputs are invalid
                ValidateInputs(source, parms);
               
                logger.Trace(this.className, methodName, "Getting Member");
                member = _LoyaltyData.LoadMemberFromLoyaltyID(loyaltyNumber);
                if (null != member)
                {
                    if (Convert.ToInt16(member.MemberStatus) == (int)MemberStatus.Active)
                    {
                        VirtualCard vc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                        if (vc == null)
                        {
                            vc = member.GetLoyaltyCardByType(VirtualCardSearchType.MostRecentIssued);
                            logger.Trace(this.className, methodName, "No primary virtual card exists");
                        }
                        
                        txnHeader.StoreNumber = storeNumber;
                        txnHeader.TxnDate = transactionDate;
                        txnHeader.TxnQualPurchaseAmt = QPA;
                        txnHeader.TxnTypeId = 1;
                        txnHeader.BrandId = "1";  //Set the brandid as an AE purchase
                        txnHeader.TxnHeaderId = "123456789";
                        txnHeader.TxnRegisterNumber = "001";
                        txnHeader.TxnNumber = "148323";
                        txnHeader.VcKey = vc.VcKey;
                        txnHeader.Parent = vc;

                        if ((transtndCount > 0) && (txntenders.Count() > 0))
                        {
                            // For adding each Tender array to txnheader
                            TT = new TxnTender[txntenders.Count()];
                            int TTcount = 0;
                            if (null != txntenders)
                            {
                                foreach (APIStruct txtnd in txntenders)
                                {
                                    TT[TTcount] = new TxnTender();
                                    TT[TTcount].TenderType = (int)txntenders[TTcount].Parms["TenderType"];
                                    TT[TTcount].TxnDate = transactionDate;
                                    decimal.TryParse(txntenders[TTcount].Parms["TenderAmount"].ToString(), out tenderAmount);
                                    TT[TTcount].TenderAmount = Math.Round(tenderAmount, MidpointRounding.AwayFromZero);
                                    TT[TTcount].RoundedAmount = 0; //Division by 0 is not allowed, so move 0 to RoundedAmount
                                    
                                    // If the QPA isn't 0 then we set Rounded Amount
                                    if (QPA != 0) 
                                    {
                                        TT[TTcount].RoundedAmount = (long)TT[TTcount].TenderAmount; // QPA * (ratio of tender amount over the QPA) * 5(::tenderbonusamount::)
                                    }

                                    // These probably need to be changed from magical to actual values.
                                    TT[TTcount].TxnHeaderId = "123456789";
                                    TT[TTcount].TxnTenderId = new Random().Next(111111111, 999999999).ToString();
                                    txnHeader.AddChildAttributeSet(TT[TTcount]);
                                    TTcount++;
                                }
                            }
                        }
                        vc.AddChildAttributeSet(txnHeader);
                        logger.Trace(this.className, methodName, "Saving Member");

                        //Header Rules Processing
                        foreach (RuleTrigger rule in txnHeader.GetMetaData().RuleTriggers)
                        {
                            if (rule.InvocationType == Enum.GetName(typeof(RuleInvocationType), RuleInvocationType.AfterInsert) && 
                                rule.Rule.ToString() == "AmericanEagle.CustomRules.AEAwardPoints" && rule.IsActive == true)
                            {
                                try
                                {
                                    _LoyaltyData.Execute(rule, vc, txnHeader, results, RuleExecutionMode.Simulation);
                                }
                                catch ( Exception exc )
                                {
                                    logger.Error(this.className, methodName, "Rule==>" + rule.RuleName);
                                    logger.Error(this.className, methodName,"TxnHEader==>"+ txnHeader.RowKey);
                                    logger.Error(this.className, methodName, exc.Message);
                                    logger.Error(this.className, methodName, exc.TargetSite.GetType().FullName + "---" + exc.TargetSite.Name+ "---" + exc.Source);
                                    logger.Error(this.className, methodName, exc.StackTrace);
                                    throw;
                                }                               
                            }
                        }

                        //Tender Rules Processing only  if tenders are there.
                        tenderAmount = 0;
                        if ((null != TT))
                        {
                            foreach (TxnTender txntend in TT)
                            {
                                if (txntend.TenderType == 75 || txntend.TenderType == 78)
                                {
                                    AECCAmount += txntend.TenderAmount;
                                }

                                tenderAmount += txntend.TenderAmount;
                            }

                            percentAECC = (float)(AECCAmount/ tenderAmount);
                            AECCAmount = Math.Round((decimal)percentAECC * QPA, MidpointRounding.AwayFromZero) * AECCMultiplier;
                        }

                        logger.Trace(this.className, methodName, "Member Saved.  Results: " + results.Count.ToString());

                        foreach (Brierley.FrameWork.ContextObject.RuleResult result in results)
                        {
                            logger.Trace(this.className, methodName, "Result Name: " + result.Name);
                            if (result.GetType() == typeof(Brierley.FrameWork.Rules.AwardPointsRuleResult))
                            {
                                Brierley.FrameWork.Rules.AwardPointsRuleResult rule = (Brierley.FrameWork.Rules.AwardPointsRuleResult)result;
                                totalPoints += rule.PointsAwarded;
                            }
                        }
                        totalPoints += AECCAmount;
                        
                        APIArguments responseParams = new APIArguments();
                        // Add the totalPoints, BraCount and JeanCount to Response
                        responseParams.Add("TotalPoints", totalPoints.ToString());
                        responseParams.Add("TotalBraCredits", BraCount);
                        responseParams.Add("TotalJeanCredits", JeanCount);
                        // Serialize the response date to string format.
                        response = SerializationUtils.SerializeResult(Name, this.Config, responseParams);
                    }
                    else if (Convert.ToInt16(member.MemberStatus) == (int)MemberStatus.Terminated)
                    {
                        throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.LoyaltyAccountTerminated)) { ErrorCode = Convert.ToInt32(ResponseCode.LoyaltyAccountTerminated) };
                    }
                }
               
                else
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.LoyaltyAccountNotFound)) { ErrorCode = Convert.ToInt32(ResponseCode.LoyaltyAccountNotFound) };
                }
            }
            catch (LWIntegrationCfgException exConfig)
            {
                logger.Error(this.className, methodName, exConfig.Message);
                throw;
            }
          
            logger.Trace(this.className, methodName, "Successfully completed ");
            return response;
        }

        #region ValidateinboundParams
        public void ValidateInputs(string source, string parameters)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string GiftCardClassCode = "9911";
            Decimal finalsales = new Decimal(); // temp variable for the sales price

            // If passed in parameters are blank or null then raise an exception reporting the same.
            if (string.IsNullOrEmpty(parameters))
            {
                throw new LWIntegrationCfgException("No parameters provided for AEGetTentativePoints") { ErrorCode = 3300 };
            }

            logger.Trace(this.className, methodName, "Validating LoyaltyNumber");
            IContentService _ContentService = _dataUtil.ContentServiceInstance();
            // Requirement API 8.0.2 and API 8.0.3
            // De-serialize input parameters and get LoyaltyNumber from input data. if input data is null or input arguments
            // do not contain LoyaltyNumber throw an exception reporting the same.

            APIArguments apiArguments = null;

            try {
                 apiArguments = SerializationUtils.DeserializeRequest(Name, Config, parameters);
            }
            catch (Exception ex)
            {

                // here we catch the problem when the date in invalid..in this case the datetime parameter
                // can't be desearialized, an BadFormat exception is thrown and the word 'DateTime' is part of the message.
                // we have no other way to detect the field that failed, here we only have one datetime parameter
                //

                bool errorInFields = false;
                logger.Error(this.className, methodName, ex.Message);

                if ( ex.Message.Contains("DateTime") ) {
                    errorInFields = true;
                            throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.InvalidTransactionDate))
                                { ErrorCode = (int)ResponseCode.InvalidTransactionDate };
                }
                else if ( ex.Message.Contains("Input string was not in a correct format") ) {
                    errorInFields  = true;
                    try {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(parameters);

                        XmlNodeList nodes = xmlDoc.GetElementsByTagName("Parm");

                        foreach ( XmlNode tmpNode in nodes ) {
                            foreach ( XmlAttribute attr in tmpNode.Attributes ) {
                                if ( attr.Name.ToLower() == "name" && attr.Value.ToLower() == "finalsaleprice" ) {
                                    string value = tmpNode.InnerText;
                                    decimal tmpValue = Decimal.MinusOne;

                                    if ( (value == null) || value == string.Empty ) {
                                        throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.TransactionFinalSaleAmountRequired)) { ErrorCode = (int)ResponseCode.TransactionFinalSaleAmountRequired };
                                    }
                                        

                                    if ( !Decimal.TryParse(value, out tmpValue))  {
                                        throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.InvalidFinalSalePrice)) { ErrorCode = (int)ResponseCode.InvalidFinalSalePrice };
                                    }
                                   
                                }

                                if (attr.Name.ToLower() == "name" &&  attr.Value.ToLower() == "tenderamount")
                                {
                                    string value = tmpNode.InnerText;
                                    decimal tmpValue = Decimal.MinusOne;

                                    if ( ( value == null ) || value == string.Empty ) {
                                        throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.TenderAmountRequired)) { ErrorCode = (int)ResponseCode.TenderAmountRequired };
                                    }


                                    if ( !Decimal.TryParse(value, out tmpValue) ) {
                                        throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.TenderAmountRequired)) { ErrorCode = (int)ResponseCode.TenderAmountRequired };
                                    }

                                }
                            }
                        }
                        
                    }
                    catch ( Exception ex2 ) {

                        logger.Error(this.className, methodName, ex2.Message);
                        if ( errorInFields ) {
                            throw ex2;
                        }
                        else {
                            throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.TechnicalDifficulties)) { ErrorCode = (int)ResponseCode.TechnicalDifficulties };
                        }

                    }
                    
                }

                throw new LWOperationInvocationException(Definitions.GetResponseMessage(ResponseCode.TechnicalDifficulties)) 
                    { ErrorCode = (int)ResponseCode.TechnicalDifficulties };
            }           

            if (null != apiArguments && apiArguments.ContainsKey("LoyaltyNumber"))
            {
                loyaltyNumber = apiArguments["LoyaltyNumber"] as string;
                loyaltyNumber = loyaltyNumber.Trim();


                long loyaltyNumberLong;
                if (long.TryParse(loyaltyNumber, out loyaltyNumberLong))
                {
                    // API 3.0.5 (The loyalty number must be numeric with 14 digits and pass the check-digit algorithm)
                    if (!LoyaltyCard.IsLoyaltyNumberValid(loyaltyNumberLong))
                    {
                        throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidLoyaltyNumber)) { ErrorCode = (int)ResponseCode.InvalidLoyaltyNumber };
                    }
                }
                else
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidLoyaltyNumber)) { ErrorCode = (int)ResponseCode.InvalidLoyaltyNumber };
                }

            }
            else
            {
                throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.LoyaltyNumberRequired)) { ErrorCode = Convert.ToInt32(ResponseCode.LoyaltyNumberRequired) };
            }

            logger.Trace(this.className, methodName, "Validating TransactionDate");

            try
            {
                DateTime txnDate = System.DateTime.Now ;
                if (null != apiArguments && apiArguments.ContainsKey("TransactionDate"))
                {
                    txnDate = (DateTime)apiArguments["TransactionDate"];//System.DateTime.Parse("13/01/2017");

                     // Check if date is in correct format
                     if (!DateTime.TryParseExact(Convert.ToDateTime(txnDate).ToString("MM/dd/yyyy"), "MM/dd/yyyy",
                         System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out txnDate))
                     {
                         logger.Trace(this.className, methodName, " apiArguments[\"TransactionDate\"] = " + apiArguments["TransactionDate"] + " is not valid.");
                         throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidTransactionDate))
                         {
                             ErrorCode = Convert.ToInt32(ResponseCode.InvalidTransactionDate)
                         };
                     }
                }


                logger.Trace(this.className, methodName, "TransactionDate: " + txnDate);
                transactionDate = (null != txnDate) ? txnDate : System.DateTime.Now; // if for somereason a null gets passed in from API, we reassign the date to be sysdate
            }
            catch
            {
                throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidTransactionDate)) { ErrorCode = Convert.ToInt32(ResponseCode.InvalidTransactionDate) };
            }

            logger.Trace(this.className, methodName, "Validating StoreNumber");
            if (null != apiArguments && apiArguments.ContainsKey("StoreNumber")
                && _ContentService.GetStoreDef(apiArguments["StoreNumber"].ToString()).Count > 0)
            {
                storeNumber = ((long)apiArguments["StoreNumber"] != 0) ? (long)apiArguments["StoreNumber"] : 659;
            }
            else
            {
                throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidStoreNumber)) { ErrorCode = Convert.ToInt32(ResponseCode.InvalidStoreNumber) };
            }

            // TxnDetail
            TransactionDetail TxnDetail = new TransactionDetail();
            if ((null != apiArguments && apiArguments.ContainsKey("TransactionDetail")))
            {
                APIStruct[] txndetails = (APIStruct[])apiArguments["TransactionDetail"];

                int transdtlCount = 0;
                foreach (APIStruct txndtl in txndetails)
                {
                    if (txndetails != null && txndetails[transdtlCount].Parms != null && txndetails[transdtlCount].Name == "TransactionDetail")
                    {
                        if (txndetails[transdtlCount].Parms.ContainsKey("SKUNumber"))
                        {
                            TxnDetail.SKUNumber = txndetails[transdtlCount].Parms["SKUNumber"].ToString().Trim(); 
                        }

                        if (txndetails[transdtlCount].Parms.ContainsKey("Quantity"))
                        {
                            TxnDetail.Quantity = ((int)txndetails[transdtlCount].Parms["Quantity"] != 0) ? (int)txndetails[transdtlCount].Parms["Quantity"] : 1;
                        }
                        else
                        {
                            TxnDetail.Quantity = 1;
                        }

                        if ( TxnDetail.Quantity > 0)                               
                        {
                            if (txndetails[transdtlCount].Parms.ContainsKey("FinalSalePrice") && txndetails[transdtlCount].Parms["FinalSalePrice"] != null)
                            {
                                if (Regex.IsMatch(txndetails[transdtlCount].Parms["FinalSalePrice"].ToString(), @"[a-zA-Z]"))
                                {
                                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidFinalSalePrice))
                                    {
                                        ErrorCode = Convert.ToInt32(ResponseCode.InvalidFinalSalePrice)
                                    };
                                }

                                Decimal.TryParse(txndetails[transdtlCount].Parms["FinalSalePrice"].ToString(), out finalsales);
                                TxnDetail.FinalSalePrice = finalsales;


                                //if final sale price is less than 0, we need to set it to zero
                                TxnDetail.FinalSalePrice = TxnDetail.FinalSalePrice < 0 ? 0 : TxnDetail.FinalSalePrice;
                                QPA += Math.Round((TxnDetail.FinalSalePrice) * TxnDetail.Quantity, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.TransactionFinalSaleAmountRequired)) { ErrorCode = Convert.ToInt32(ResponseCode.TransactionFinalSaleAmountRequired) };
                            }
                              
                        }
                        
                        if (txndetails[transdtlCount].Parms.ContainsKey("ClassCode"))
                        {
                            TxnDetail.ClassCode = txndetails[transdtlCount].Parms["ClassCode"].ToString().Trim();

                            // Remove gift cards from being counted in the QPA
                            if (TxnDetail.ClassCode == GiftCardClassCode)
                            {
                                QPA -= Math.Round((TxnDetail.FinalSalePrice) * TxnDetail.Quantity, MidpointRounding.AwayFromZero);
                            }

                            if (brapromocodes.Contains(TxnDetail.ClassCode))
                            { 
                                //BraCount++;
                                if (TxnDetail.Quantity > 0)
                                    {
                                        BraCount = BraCount + (TxnDetail.Quantity ); 
                                    }
                            }

                            if (jeanpromocodes.Contains(TxnDetail.ClassCode))
                            { 
                                //JeanCount++;
                                if (TxnDetail.Quantity > 0)
                                {
                                    JeanCount = JeanCount + (TxnDetail.Quantity);
                                }
                            }
                        }

                        
                    }
                    else
                    {
                        throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.NoTransactionDataAvailable)) { ErrorCode = Convert.ToInt32(ResponseCode.NoTransactionDataAvailable) };
                    }

                    transdtlCount++;
                }
            }
            else
            {
                throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.NoTransactionDataAvailable)) { ErrorCode = Convert.ToInt32(ResponseCode.NoTransactionDataAvailable) };
            }

            TransactionTender TxnTender = new TransactionTender();

            if (apiArguments != null && apiArguments.ContainsKey("TransactionTender"))
            {
                txntenders = (APIStruct[])apiArguments["TransactionTender"];
                 transtndCount = 0;
                foreach (APIStruct txtnd in txntenders)
                {
                    if (txntenders != null && txntenders[transtndCount].Parms != null && txntenders[transtndCount].Name == "TransactionTender")
                    {
                        if (txntenders[transtndCount].Parms.ContainsKey("TenderType"))
                        {
                            TxnTender.TenderType = (int)txntenders[transtndCount].Parms["TenderType"];
                        }
                        else
                        {
                            throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidFinalSalePrice)) { ErrorCode = Convert.ToInt32(ResponseCode.InvalidFinalSalePrice) };
                        }
                        if (txntenders[transtndCount].Parms.ContainsKey("TenderAmount") && txntenders[transtndCount].Parms["TenderAmount"] != null)
                        {
                            if (Regex.IsMatch(txntenders[transtndCount].Parms["TenderAmount"].ToString(), @"[a-zA-Z]"))
                            {
                                throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.TenderAmountRequired))
                                {
                                    ErrorCode = Convert.ToInt32(ResponseCode.TenderAmountRequired) // Probably need an InvalidTenderAmount Response Code
                                };
                            }

                            decimal.TryParse(txntenders[transtndCount].Parms["TenderAmount"].ToString(), out finalsales);
                            TxnTender.TenderAmount = finalsales;
                        }
                        else
                        {
                            throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.TenderAmountRequired)) { ErrorCode = Convert.ToInt32(ResponseCode.TenderAmountRequired) };
                        }
                    }
                    else
                    {
                        throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidFinalSalePrice)) { ErrorCode = Convert.ToInt32(ResponseCode.InvalidFinalSalePrice) };
                    }
                    transtndCount++;
                }
            }
        }
        #endregion 

    }

   

}
