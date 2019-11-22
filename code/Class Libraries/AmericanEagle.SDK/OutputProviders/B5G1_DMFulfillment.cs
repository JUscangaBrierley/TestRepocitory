using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Configuration;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

using AmericanEagle.SDK.Global;
using Brierley.Clients.AmericanEagle.DataModel;

namespace  AmericanEagle.SDK.OutputProviders
{
    class B5G1_DMFulfillment : IDAPOutputProvider
    {

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private string path = string.Empty;
        private DateTime processDate = DateTime.Now;

        private string directFileName = "AEORW_DM_B5G1_RW_YYYYMMDDHHMMSS.txt";

        private long rowsWrittenDirect = 0;

        private const string braRuleName = "B5G1 Bra Reward";
        private const string jeanRuleName = "B5G1 Jean Reward";

        #region IDAPOutputProvider Members

        public void Initialize(System.Collections.Specialized.NameValueCollection globals, System.Collections.Specialized.NameValueCollection args, long jobId, DAPDirectives config, System.Collections.Specialized.NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");



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

            directFileName = path +directFileName.Replace("YYYYMMDDHHMMSS", DateTime.Now.ToString("yyyyMMddHHmmss"));

            // we allways create the  1 possible output files
            //
            
            string[] files = { directFileName };

            foreach ( string filestr in files )
            {
                StreamWriter sw = new StreamWriter(filestr, true);

                sw.Write(this.getHeaderline());
                sw.Flush();

                sw.Close();
            }
         

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }



        public void ProcessMessageBatch(IList<string> messageBatch)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            string rowType = string.Empty;
            long ipcode = 0;

            try
            {
                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    // Loding XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("BraRewards/BraReward");

                //FlatFileLine loTmpLine  = new FlatFileLine(); AEO-557

                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                                    
                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "NodeName:" + node.Name);

                  
                    switch (node.Name)
                    {
                        case "type":
                            rowType = node.InnerText;
                            break;

                        case "IPCODE":
                            ipcode = long.TryParse(node.InnerText, out ipcode )? ipcode : -1;

                            break;
                   
                    }
                }

                if (null != xmlNode)
                {
                    if ( ipcode > 0)
                    {
                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Retrieving member");

                        Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromIPCode (ipcode); //AEO-Redesegin-2015 Begin & End

                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member:" + member.FirstName +","+member.LastName);
                        if (null == member)
                        {
                            // Log error when member not found
                            this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for ipcode - " + ipcode);
                        }
                        else
                        {
                            string ruleName = rowType == "Jean"? jeanRuleName: braRuleName; //AEO-Redesign-2015 Begin & End

                            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Executing rule: " + ruleName);

                            IList<string> rewardsIds= ExecuteRule(member, ruleName); // AEO-Redesign-2015 Begin & End

                            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name,
                                "Member Rewards Issued = " +rewardsIds.Count);


                            // AEO-Redesign-2015 Begin

                            if ( rewardsIds.Count != 0 ) {

                                
                                //bool firstReward = true; AEO-557

                                foreach ( string ruleResult in rewardsIds ) {

                                    int index = ruleResult.IndexOf("id=");
                                    string tmpId = ruleResult.Substring(index + "id=".Length);

                                    long memberRewardId = 0;

                                    if (long.TryParse ( tmpId, out memberRewardId) ){

                                        // AEO-Redesign-2015 Begin

                                        MemberReward memberReward = LWDataServiceUtil.DataServiceInstance(true).GetMemberReward(memberRewardId);

                                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Reward = " + memberReward.Id);
                                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Reward is " + memberReward == null ? "null" : "not null");

                                        // look for a valid barcode and unassigned
                                        //

                                        if (memberReward != null)
                                        {

                                           // if ( firstReward ) {
                                           //     firstReward = !firstReward;

                                                // collect data before writing to the output file.

                                                // FlatFileLine fileLine = loTmpLine; 
                                                FlatFileLine fileLine = new FlatFileLine(); //AEO-557

                                                // customber number & loyalty number

                                                VirtualCard lCard = null;
                                                foreach ( VirtualCard loCard in member.LoyaltyCards )
                                                {
                                                    if ( loCard.Status == VirtualCardStatusType.Active && loCard.IsPrimary )
                                                    {
                                                        lCard = loCard;
                                                        break;
                                                    }
                                                }

                                                if ( lCard == null )
                                                {
                                                    throw new Exception("It was not posibble to determine the loyaltynumber because no primary virtual card was found for ipcode=" + member.IpCode);

                                                }
                                                else
                                                {
                                                    fileLine.CUSTOMER_NBR = lCard.LinkKey.ToString();
                                                    fileLine.LOYALTY_NUMBER = lCard.LoyaltyIdNumber == null ? "" : lCard.LoyaltyIdNumber;
                                                }


                                                // firstname, lastnames & tier status

                                                fileLine.FNAME = member.FirstName == null ? "" : member.FirstName;
                                                fileLine.LNAME = member.LastName == null ? "" : member.LastName;

                                                MemberTier tierTmp = member.GetTier(DateTime.Now.Date);

                                                fileLine.TIER_STATUS = tierTmp == null || tierTmp.TierDef == null ? string.Empty : tierTmp.TierDef.Name;
                                                switch (fileLine.TIER_STATUS.ToUpper().Trim())
                                                {
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


                                                IList<IClientDataObject> loDetails = member.GetChildAttributeSets("MemberDetails");

                                                MemberDetails details = ( loDetails == null || loDetails.Count == 0 ? null : loDetails[0] ) as MemberDetails;

                                                // member details fields

                                                fileLine.email = details == null || details.EmailAddress == null ? "" : details.EmailAddress;
                                                fileLine.MOBILE_NUMBER = details == null || details.MobilePhone == null ? "" : details.MobilePhone;
                                                fileLine.ADDRESS1 = details == null || details.AddressLineOne == null ? "" : details.AddressLineOne;
                                                fileLine.ADDRESS2 = details == null || details.AddressLineTwo == null ? "" : details.AddressLineTwo;
                                                fileLine.CITY = details == null || details.City == null ? "" : details.City;
                                                fileLine.MOBILE_NUMBER = details == null || details.MobilePhone == null ? "" : details.MobilePhone;
                                                fileLine.STATE = details == null || details.StateOrProvince == null ? "" : details.StateOrProvince;
                                                fileLine.ZIP = details == null || details.ZipOrPostalCode == null ? "" : details.ZipOrPostalCode;
                                                fileLine.REGION = details == null || details.Country == null ? "" : details.Country;
                                                fileLine.LANGUAGE_PREFERENCE = details == null || details.LanguagePreference == null ? "0" : details.LanguagePreference;
                                                fileLine.GENDER = details == null || details.Gender == null ? "0" : details.Gender;
                                                fileLine.STORE_LOYALTY = details == null ? "" : details.HomeStoreID.ToString();
                                                fileLine.CARD_TYPE = details == null || details.CardType == null ? "" : details.CardType.ToString();

                                                // memberrewards fields

                                                fileLine.THREE_DIGIT_REWARDS_CODE = memberReward.OfferCode;
                                                fileLine.TWENTY_DIGIT_REWARD_CODE = memberReward.CertificateNmbr;
                                                fileLine.REWARD_EXP_DATE = memberReward.Expiration == null ? "" : String.Format("{0:dd-MMM-yyyy}", memberReward.Expiration);

                                                //AEO-Redesign-2015 Begin
                                                if ( memberReward.RewardDef != null )
                                                {

                                                    if ( memberReward.RewardDef.Name == "AEO Rewards $5 Reward" )
                                                    {
                                                        fileLine.REWARD_TYPE = "1";
                                                    }
                                                    else if ( memberReward.RewardDef.Name == "B5G1 Jean Reward" )
                                                    {
                                                        fileLine.REWARD_TYPE = "3";
                                                    }
                                                    else if ( memberReward.RewardDef.Name == "B5G1 Bra Reward" )
                                                    {
                                                        fileLine.REWARD_TYPE = "2";
                                                    }
                                                    else
                                                    {
                                                        fileLine.REWARD_TYPE = memberReward.RewardDef.Name;
                                                    }
                                                }
                                                else
                                                {

                                                    fileLine.REWARD_TYPE = string.Empty;
                                                }
                                                //AEO-Redesign-2015 End


                                                // points

                                                IList<IClientDataObject> loBalances = member.GetChildAttributeSets("MemberPointBalances");

                                                MemberPointBalances balances = ( loBalances == null || loBalances.Count == 0 ? null : loBalances[0] ) as MemberPointBalances;

                                                fileLine.POINTS_BALANCE = balances == null ? "0" : balances.TotalPoints.ToString();
                                                fileLine.POINTS_NEEDED_FOR_NEXT_REWARD = balances == null ? "0" : balances.PointsToNextReward.ToString();
                                                fileLine.NUMBER_OF_BRAS_PURCHASED = balances == null ? "0" : balances.BraCurrentPurchased.ToString();
                                                fileLine.NUMBER_OF_JEANS_PURCHASED = balances == null ? "0" : balances.JeansRollingBalance.ToString();

                                                //mmv 08nov2015 begin
                                                fileLine.CREDITS_TO_NEXT_FREE_BRA = ( 5 - int.Parse(fileLine.NUMBER_OF_BRAS_PURCHASED) ).ToString();
                                                fileLine.CREDITS_TO_NEXT_FREE_JEAN = ( 5 - int.Parse(fileLine.NUMBER_OF_BRAS_PURCHASED) ).ToString();
                                                //mmv 08nov2015 begin
                                                

                                                // from configuration

                                                // AEO-258 Begin
                                                if ( rowType == "Jean" ) {
                                                    fileLine.CAMPAIGN_ID = "JEANS004";
                                                }
                                                else {
                                                    fileLine.CAMPAIGN_ID = "BRAS003";
                                                }

                                                // AEO-258 End

                                                // AEO-Redesign-2015 Begin
                                                string tmpEID = ( rowType == "Jean" ) ?
                                                    LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("EID_EmailJeanReward").Value
                                                    : LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("EID_EmailBraReward").Value;
                                                //AEO-Redesign-2015 End


                                                string tmpCampaignID = ( rowType == "Jean" ) ?
                                                    LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("CampaignID_SMSJeanReward").Value
                                                    : LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("CampaignID_SMSJeanReward").Value;

                                                fileLine.eid = tmpEID == null ? string.Empty : tmpEID;
                                                /* AEO-258 Begin
                                                fileLine.CAMPAIGN_ID = tmpCampaignID == null ? string.Empty : tmpCampaignID; 
                                                AEO-258 eND */
                                                // from reward definition


                                                RewardDef def5 = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Rewards $5 Reward");
                                                RewardDef defB5G1Jean = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("B5G1 Jean Reward");
                                                RewardDef defB5G1Bra = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("B5G1 Bra Reward");


                                                IList<MemberReward> rewards5 = LWDataServiceUtil.DataServiceInstance(true).GetMemberRewardsByDefId(member, def5.Id);
                                                IList<MemberReward> rewardsB5G1Bra = LWDataServiceUtil.DataServiceInstance(true).GetMemberRewardsByDefId(member, defB5G1Bra.Id);
                                                IList<MemberReward> rewardsB5G1Jean = LWDataServiceUtil.DataServiceInstance(true).GetMemberRewardsByDefId(member, defB5G1Jean.Id);


                                                long countRewards5 = 0;
                                                long countRewardsBra = 0;
                                                long countRewardsJean = 0;

                                                foreach ( MemberReward loReward in rewards5 )
                                                {
                                                    if ( ( loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date )
                                                         && ( loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date ))
                                                    {
                                                        countRewards5++;
                                                    }
                                                }

                                                foreach ( MemberReward loReward in rewardsB5G1Bra )
                                                {
                                                    if ( ( loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date )
                                                         && ( loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date ))
                                                    {
                                                        countRewardsBra++;
                                                    }
                                                }


                                                foreach ( MemberReward loReward in rewardsB5G1Jean )
                                                {
                                                    if ( ( loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date )
                                                         && ( loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date ))
                                                    {
                                                        countRewardsJean++;
                                                    }
                                                }

                                                fileLine.NUMBER_OF_ACTIVE_5_OFF_REWARD = countRewards5.ToString();
                                                fileLine.NUMBER_OF_ACTIVE_FREE_BRA_REWARD = countRewardsBra.ToString();
                                                fileLine.NUMBER_OF_ACTIVE_FREE_JEANS_REWARD = countRewardsJean.ToString();

                                                if ( loDetails != null && loDetails.Count > 0 )
                                                {

                                                    MemberDetails loTmp = (MemberDetails)loDetails[0];


                                                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "determining which file to write");
                                                    // if cell verification is not pending then add cell phone
                                                    // and write to the cell file
                                                    fileLine.DELIVERY_CHANNEL = "3";

                                                    StreamWriter sw = new StreamWriter(directFileName, true);

                                                    sw.Write(fileLine.toFormattedString(FileFormat.Direct));
                                                    sw.Flush();
                                                    sw.Close();
                                                    rowsWrittenDirect++;
                                                }
                                            //}
                                        }
                                        else
                                        {
                                            throw new Exception("No reward bar code valid for " + this.processDate.Date.ToString("MM/dd/yyyy") + "type code :");
                                        }
                                    }
                                }
                            }                         
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
            }

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown()
        {


           

            return 0;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        #endregion


        public void Dispose ( )
        {
            return;
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

        public IList<string> ExecuteRule ( Member member, string ruleName )
        {
            try
            {

                Dictionary<string, string> additionalFields = new Dictionary<string, string>(); 

                if(ruleName.Contains("Jean"))
                {
                    additionalFields.Add("TypeCode", "JEANDM");
                }

                if (ruleName.Contains("Bra"))
                {
                    additionalFields.Add("TypeCode", "BRADM");
                }

                IList<string> retVal =new List<string>();

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member: " + member.IpCode);

                //Create a context object and assign the member and MemberDetails attribute set
                //to be used in invoking the IssueReward for this reward and the AwardPoints 
                //rule associated with the View Earned Reward 

                ContextObject cobj = new ContextObject();
                cobj.Owner = member;
                cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];
                cobj.Environment = additionalFields.ToDictionary(pair => pair.Key, pair => (object)pair.Value);


                RuleTrigger ruleTrigger = LWDataServiceUtil.DataServiceInstance(true).GetRuleByName(ruleName);

                if ( ruleTrigger == null )
                {
                    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule: (" + ruleName + ") Not Defined");
                    throw new Exception(ruleName + " Rule Not Defined", new Exception(ruleName + " Rule Not Defined"));
                }
                else
                {
                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule: (" + ruleTrigger.RuleName + ")");
                }


                LWDataServiceUtil.DataServiceInstance(true).Execute(ruleTrigger, cobj);

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule executed");

                foreach ( ContextObject.RuleResult result in cobj.Results ) {
                    retVal.Add(result.Detail);
                }
                
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "rewardsIssued=" + retVal.Count);
                return retVal;

            }
            catch ( Exception ex )
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
                throw new Exception(ex.Message, new Exception("System Error"));
            }

        }
    }
}