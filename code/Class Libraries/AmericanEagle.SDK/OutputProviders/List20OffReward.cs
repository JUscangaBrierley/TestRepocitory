//--------------------------------------------------------------------------------
// <copyright file="List20OffReward" company="Brierley and Partners">
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
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.DataAcquisition.Core;
    using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
    using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;
    using Brierley.Clients.AmericanEagle.DataModel;
    using System.IO;
    using System.Configuration;
    using System.Text;
    using Brierley.ClientDevUtilities.LWGateway;

    public class List20OffReward : IDAPOutputProvider
    {
       
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private string emailOutFileName = "AEORW_EM_AECC_20_YYYYMMDDHHMMSS.txt";
        private string smsOutFileName   = "AEORW_SM_AECC_20_YYYYMMDDHHMMSS.txt";
        private string directOutFileName  = "AEORW_DM_AECC_20_YYYYMMDDHHMMSS.txt";
        private FileUtils utils = new FileUtils();      
        private long ipcode = 0L;
        private string path = string.Empty;
        private DateTime processDate = DateTime.Now;
        private string jobname = string.Empty;

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);

        public void Dispose()
        {
        }

     
        public void Initialize(NameValueCollection globals,
            NameValueCollection args, long jobId, DAPDirectives config, 
            NameValueCollection parameters,
            DAPPerformanceCounterUtil performUtil)
        {

            if ( globals.HasKeys() ) {
                string[] values = globals.GetValues("JobName");
                if ( values != null && values.Length > 0 && values[0] != null 
                    && values[0].Trim().Length > 0 ) {

                    this.jobname = values[0];
                }
            }

            if ( null == ConfigurationManager.AppSettings["FilePath"] )
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No path defined in app.config");
                throw new Exception("No path defined in app.config");
            }
            else
            {
              
                path = ConfigurationManager.AppSettings["FilePath"];
                path = path + "\\";
                if ( !Directory.Exists(path) )
                {
                    Directory.CreateDirectory(path);
                }
            }

            if ( null != ConfigurationManager.AppSettings["ProcessDate"] )
            {
                string strProcessDate = ConfigurationManager.AppSettings["ProcessDate"];
                DateTime.TryParse(strProcessDate, out processDate);
            }

            string timeStampStr = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.emailOutFileName  = path + this.emailOutFileName.Replace("YYYYMMDDHHMMSS", this.processDate.ToString ("yyyyMMddHHmmss"));
            this.smsOutFileName    = path + this.smsOutFileName.Replace("YYYYMMDDHHMMSS", this.processDate.ToString ("yyyyMMddHHmmss"));
            this.directOutFileName = path + this.directOutFileName.Replace("YYYYMMDDHHMMSS", this.processDate.ToString ("yyyyMMddHHmmss"));

            
           
            StreamWriter sw = new StreamWriter(this.emailOutFileName, false);
            sw.Write(this.getHeaderline());
            sw.Flush();

            sw = new StreamWriter(this.smsOutFileName, false);
            sw.Write(this.getHeaderline());
            sw.Flush();

            sw = new StreamWriter(this.directOutFileName, false);
            sw.Write(this.getHeaderline());
            sw.Flush();

            sw.Close();


        }



         public void ProcessMessageBatch(List<string> messageBatch)        {

            string methodName = MethodBase.GetCurrentMethod().Name;
            try
            {
               
                this.logger.Trace(this.className, methodName, "Starts");
                
                XmlDocument doc = new XmlDocument();

                foreach (string str in messageBatch)  {               
                    doc.LoadXml(str);
                }
               
                XmlNodeList xmlNodes = doc.SelectNodes("Rewards/Reward");
                FlatFileLine loTmpLine = new FlatFileLine();

                foreach ( XmlNode xmlNode in xmlNodes ) {
                    if ( null != xmlNode ) {


                        foreach ( XmlNode node in xmlNode.ChildNodes ) {

                            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "NodeName:" + node.Name);


                            switch ( node.Name ) {


                                case "ipcode":

                                    if ( !long.TryParse(node.InnerText, out ipcode) ) {
                                        throw new Exception("invalid ipcode number =" + node.InnerText);
                                    }
                                    break;

                            }
                        }




                        // Get member
                        Member member;
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            member = lwService.LoadMemberFromIPCode(ipcode);
                        }
                        if ( null == member ) {
                            // Log error when member not found
                            throw new Exception(this.className + " " + methodName + " Member Not Found for ipcode - " + ipcode.ToString());
                        }

                        IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
                        MemberDetails memberDetails = mbrDtlObjs[0] as MemberDetails;

                        if ( memberDetails == null ) {
                            throw new Exception("It was not possible to retrieve details for the ipcode " + member.IpCode);
                        }


                        // collect data before writing to the output file.

                        FlatFileLine fileLine = loTmpLine;


                        // customber number & loyalty number


                        VirtualCard lCard = null;
                        foreach ( VirtualCard loCard in member.LoyaltyCards ) {
                            if ( loCard.Status == VirtualCardStatusType.Active && loCard.IsPrimary ) {
                                lCard = loCard;
                                break;
                            }
                        }

                        if ( lCard == null ) {
                            throw new Exception("It was not possible to determine the loyaltynumber because no primary virtual card was found for ipcode=" + member.IpCode);

                        }
                        else {
                            fileLine.CUSTOMER_NBR = lCard.LinkKey.ToString();
                            fileLine.LOYALTY_NUMBER = lCard.LoyaltyIdNumber == null ? "" : lCard.LoyaltyIdNumber;
                        }


                        // firstname, lastnames & tier status

                        fileLine.FNAME = member.FirstName == null ? "" : member.FirstName;
                        fileLine.LNAME = member.LastName == null ? "" : member.LastName;

                        MemberTier tierTmp = member.GetTier(DateTime.Now.Date);

                        fileLine.TIER_STATUS = tierTmp == null || tierTmp.TierDef == null ? string.Empty : tierTmp.TierDef.Name;
                        switch ( fileLine.TIER_STATUS.ToUpper().Trim() ) {
                            case "BLUE":
                                fileLine.TIER_STATUS = "1";
                                break;
                            case "SILVER":
                                fileLine.TIER_STATUS = "2";
                                break;
                            case "GOLD":
                                fileLine.TIER_STATUS = "3";
                                break;
                            case "SELECT":
                                fileLine.TIER_STATUS = "4";
                                break;
                        }

                        fileLine.BIRTHDATE = member.BirthDate == null ? "" : String.Format("{0:dd-MMM-yyyy}", member.BirthDate);


                        //IList<IClientDataObject> loDetails = member.GetChildAttributeSets("MemberDetails");

                        // MemberDetails memberDetails = ( loDetails == null || loDetails.Count == 0 ? null : loDetails[0] ) as MemberDetails;

                        // member details fields

                        fileLine.email = memberDetails == null || memberDetails.EmailAddress == null ? "" : memberDetails.EmailAddress;
                        fileLine.MOBILE_NUMBER = memberDetails == null || memberDetails.MobilePhone == null ? "" : memberDetails.MobilePhone;
                        fileLine.ADDRESS1 = memberDetails == null || memberDetails.AddressLineOne == null ? "" : memberDetails.AddressLineOne;
                        fileLine.ADDRESS2 = memberDetails == null || memberDetails.AddressLineTwo == null ? "" : memberDetails.AddressLineTwo;
                        fileLine.CITY = memberDetails == null || memberDetails.City == null ? "" : memberDetails.City;
                        fileLine.MOBILE_NUMBER = memberDetails == null || memberDetails.MobilePhone == null ? "" : memberDetails.MobilePhone;
                        fileLine.STATE = memberDetails == null || memberDetails.StateOrProvince == null ? "" : memberDetails.StateOrProvince;
                        fileLine.ZIP = memberDetails == null || memberDetails.ZipOrPostalCode == null ? "" : memberDetails.ZipOrPostalCode;
                        fileLine.REGION = memberDetails == null || memberDetails.Country == null ? "" : memberDetails.Country;
                        fileLine.LANGUAGE_PREFERENCE = memberDetails == null || memberDetails.LanguagePreference == null ? "0" : memberDetails.LanguagePreference;
                        fileLine.GENDER = memberDetails == null || memberDetails.Gender == null ? "0" : memberDetails.Gender;
                        fileLine.STORE_LOYALTY = memberDetails == null ? "" : memberDetails.HomeStoreID.ToString();
                        fileLine.CARD_TYPE = memberDetails == null || memberDetails.CardType == null ? "" : memberDetails.CardType.ToString();

                        // memberrewards fields

                        // memberrewards fields
                        string typeCodeKey = "20AwardTypeCode";
                        string typeCode = string.Empty;

                        ClientConfiguration clientConfiguration;
                        using (var dtService = _dataUtil.DataServiceInstance())
                        {
                            clientConfiguration = dtService.GetClientConfiguration(typeCodeKey);
                        }
                        if ( clientConfiguration == null ) {

                            logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, " Typecode not found: " + typeCodeKey);
                            throw new Exception(" Typecode not found" + typeCodeKey);

                        }

                        typeCode = clientConfiguration.Value;

                        string offerTypeCode = string.Empty;
                        FileUtils.OfferType offer = FileUtils.OfferType.DM_20;

                        if ( memberDetails.PendingCellVerification == 0 && memberDetails.MobilePhone != null && memberDetails.MobilePhone.Length > 0 ) // AEO-258 begin & end
                   {
                            fileLine.DELIVERY_CHANNEL = "2";
                            offer = FileUtils.OfferType.SMS_20;
                            offerTypeCode = utils.GetOffer(typeCode, 1, offer);

                        }

                        if ( memberDetails.PendingEmailVerification == 0 && memberDetails.EmailAddress != null && memberDetails.EmailAddress.Length > 0 ) {

                            fileLine.DELIVERY_CHANNEL = "1";
                            offer = FileUtils.OfferType.Email_20;
                            offerTypeCode = utils.GetOffer(typeCode, 1, offer);
                        }

                        if ( memberDetails.PendingEmailVerification == 1 &&
                             memberDetails.PendingCellVerification == 1 &&
                            ( memberDetails.CardType == 1 || memberDetails.CardType == 2 || memberDetails.CardType == 3 ) ) {

                            fileLine.DELIVERY_CHANNEL = "3";
                            offer = FileUtils.OfferType.DM_20;
                            offerTypeCode = utils.GetOffer(typeCode, 1, offer);

                        }



                        this.logger.Trace(this.className, methodName, "Getting codes for: " + offerTypeCode);


                        String certificateNumber = String.Empty;
                        String offerCode = string.Empty;
                        DateTime expirationDate = DateTime.Now;

                        if ( offerTypeCode != string.Empty ) {
                            utils.GetNextRewardCertificateNumber(member.IpCode, offerTypeCode, out certificateNumber, out offerCode, out expirationDate);
                        }


                        fileLine.THREE_DIGIT_REWARDS_CODE = offerCode;
                        fileLine.TWENTY_DIGIT_REWARD_CODE = certificateNumber;
                        fileLine.REWARD_EXP_DATE = new DateTime(2016, 12, 31).ToString("dd-MMM-yyyy");
                        fileLine.REWARD_TYPE = string.Empty;

                        fileLine.eid = string.Empty;


                        // points

                        IList<IClientDataObject> loBalances = member.GetChildAttributeSets("MemberPointBalances");

                        MemberPointBalances balances = ( loBalances == null || loBalances.Count == 0 ? null : loBalances[0] ) as MemberPointBalances;


                        fileLine.POINTS_BALANCE = balances == null ? "0" : balances.TotalPoints.ToString();
                        fileLine.POINTS_NEEDED_FOR_NEXT_REWARD = balances == null ? "0" : balances.PointsToNextReward.ToString();
                        fileLine.NUMBER_OF_BRAS_PURCHASED = balances == null ? "0" : balances.BraCurrentPurchased.ToString();
                        fileLine.NUMBER_OF_JEANS_PURCHASED = balances == null ? "0" : balances.JeansRollingBalance.ToString();


                        fileLine.CREDITS_TO_NEXT_FREE_BRA = ( 5 - int.Parse(fileLine.NUMBER_OF_BRAS_PURCHASED) ).ToString();
                        fileLine.CREDITS_TO_NEXT_FREE_JEAN = ( 5 - int.Parse(fileLine.NUMBER_OF_BRAS_PURCHASED) ).ToString();




                        string tmpEID = string.Empty;

                        string tmpCampaignID = string.Empty;

                        fileLine.eid = tmpEID == null ? string.Empty : tmpEID;
                        fileLine.CAMPAIGN_ID = tmpCampaignID == null ? string.Empty : tmpCampaignID;


                        RewardDef def5;
                        RewardDef defB5G1Jean;
                        RewardDef defB5G1Bra;

                        using (var contService = _dataUtil.ContentServiceInstance())
                        {
                            def5 = contService.GetRewardDef("AEO Rewards $5 Reward");
                            defB5G1Jean = contService.GetRewardDef("B5G1 Jean Reward");
                            defB5G1Bra = contService.GetRewardDef("B5G1 Bra Reward");
                        }

                        IList<MemberReward> rewards5;
                        IList<MemberReward> rewardsB5G1Bra;
                        IList<MemberReward> rewardsB5G1Jean;

                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            rewards5 = lwService.GetMemberRewardsByDefId(member, def5.Id);
                            rewardsB5G1Bra = lwService.GetMemberRewardsByDefId(member, defB5G1Bra.Id);
                            rewardsB5G1Jean = lwService.GetMemberRewardsByDefId(member, defB5G1Jean.Id);
                        }

                        long countRewards5 = 0;
                        long countRewardsBra = 0;
                        long countRewardsJean = 0;

                        foreach ( MemberReward loReward in rewards5 ) {
                            if ( ( loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date )
                                 && ( loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date ) ) {
                                countRewards5++;
                            }
                        }

                        foreach ( MemberReward loReward in rewardsB5G1Bra ) {
                            if ( ( loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date )
                                 && ( loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date ) ) {
                                countRewardsBra++;
                            }
                        }


                        foreach ( MemberReward loReward in rewardsB5G1Jean ) {
                            if ( ( loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date )
                                 && ( loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date ) ) {
                                countRewardsJean++;
                            }
                        }

                        fileLine.NUMBER_OF_ACTIVE_5_OFF_REWARD = countRewards5.ToString();
                        fileLine.NUMBER_OF_ACTIVE_FREE_BRA_REWARD = countRewardsBra.ToString();
                        fileLine.NUMBER_OF_ACTIVE_FREE_JEANS_REWARD = countRewardsJean.ToString();


                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "determining which file to write");
                        // if cell verification is not pending then add cell phone
                        // and write to the cell file

                        if ( memberDetails.PendingCellVerification == 0 && memberDetails.MobilePhone != null && memberDetails.MobilePhone.Length > 0 ) // AEO-258 begin & end
                   {

                            fileLine.DELIVERY_CHANNEL = "2";

                            StreamWriter sw = new StreamWriter(this.smsOutFileName, true);

                            sw.Write(fileLine.toFormattedString(FileFormat.Cell));
                            sw.Flush();
                            sw.Close();

                        }

                        if ( memberDetails.PendingEmailVerification == 0 && memberDetails.EmailAddress != null && memberDetails.EmailAddress.Length > 0 ) {

                            fileLine.DELIVERY_CHANNEL = "1";


                            StreamWriter sw = new StreamWriter(this.emailOutFileName, true);

                            sw.Write(fileLine.toFormattedString(FileFormat.Email));
                            sw.Flush();
                            sw.Close();

                        }

                        if ( memberDetails.PendingEmailVerification == 1 &&
                             memberDetails.PendingCellVerification == 1 &&
                            ( memberDetails.CardType == 1 || memberDetails.CardType == 2 || memberDetails.CardType == 3 ) ) {
                            fileLine.DELIVERY_CHANNEL = "3";

                            StreamWriter sw = new StreamWriter(this.directOutFileName, true);

                            sw.Write(fileLine.toFormattedString(FileFormat.Direct));
                            sw.Flush();
                            sw.Close();

                        }


                    }
                    else {
                        this.logger.Error(this.className, methodName, "xml node not found");
                    }

                    this.logger.Trace(this.className, methodName, "Ends");
                }
                
             }
            catch (Exception ex)
             {
                // Logging for exception
                this.logger.Error(this.className, methodName, ex.Message);
             }
        }

     

        private string getHeaderline ( )
        {


            StringBuilder tmp = new StringBuilder();

            tmp.Append("DELIVERY_CHANNEL|CUSTOMER_NBR|LOYALTY_NUMBER|EMAIL|MOBILE_NUMBER|FNAME|LNAME|ADDRESS1|ADDRESS2|CITY|STATE|ZIP|COUNTRY|AUTH_CD|SUC_CD|CAMPAIGN_TYPE|CAMPAIGN_EXP_DATE|");
            tmp.Append("EID|CAMPAIGN_ID|LANGUAGE_PREFERENCE|GENDER|BIRTHDATE|STORE_LOYALTY|TIER_STATUS|POINTS_BALANCE|POINTS_NEEDED_FOR_NEXT_REWARD|NUMBER_OF_BRAS_PURCHASED|CREDITS_TO_NEXT_FREE_BRA|NUMBER_OF_JEANS_PURCHASED|CREDITS_TO_NEXT_FREE_JEAN|NUMBER_OF_ACTIVE_5_OFF_REWARD|NUMBER_OF_ACTIVE_FREE_JEANS_REWARD|");
            tmp.Append("NUMBER_OF_ACTIVE_FREE_BRA_REWARD|COMMUNICATION_ID|COMM_PLAN_ID|COLLATERAL_ID|PACKAGE_ID|");
            tmp.Append("STEP_ID|MESSAGE_ID|SEG_ID|SEG_NM|AAP_FLAG|CARD_TYPE|RUN_ID|LEAD_KEY_ID|SITE_URL|ENABLE_PASSBOOK_PASS|TIMESTAMP");

            tmp.AppendLine();


            return tmp.ToString();
        }


          public IList<string> ExecuteRule ( Member member, string ruleName )    {
            try    {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    IList<string> retVal = new List<string>();

                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member: " + member.IpCode);

                    //Create a context object and assign the member and MemberDetails attribute set
                    //to be used in invoking the IssueReward for this reward and the AwardPoints 
                    //rule associated with the View Earned Reward 

                    ContextObject cobj = new ContextObject();
                    cobj.Owner = member;
                    cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];


                    RuleTrigger ruleTrigger = lwService.GetRuleByName(ruleName);

                    if (ruleTrigger == null)
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule: (" + ruleName + ") Not Defined");
                        throw new Exception(ruleName + " Rule Not Defined", new Exception(ruleName + " Rule Not Defined"));
                    }
                    else
                    {
                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule: (" + ruleTrigger.RuleName + ")");
                    }


                    lwService.Execute(ruleTrigger, cobj);

                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule executed");

                    foreach (ContextObject.RuleResult result in cobj.Results)
                    {
                        retVal.Add(result.Detail);
                    }

                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "rewardsIssued=" + retVal.Count);
                    return retVal;
                }
            }
            catch ( Exception ex )
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
                throw new Exception(ex.Message, new Exception("System Error"));
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
