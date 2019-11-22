using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Configuration;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.DataAcquisition.Core;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;


using AmericanEagle.SDK.Global;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.OutputProviders
{
    public class Reward10Fulfillment : IDAPOutputProvider
    {
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        private string cellFileName = "AEORW_SM_10RWARD_YYYYMMDDHHMMSS.txt";
        private string emailFileName = "AEORW_EM_10RWARD_YYYYMMDDHHMMSS.txt";
        private string path = string.Empty;
        private DateTime processDate = DateTime.Now;
        public const string FieldSeparator = "|";
        private long rowsWrittenCell = 0;
        private long rowsWrittenEmail = 0;


        //RKG - changed to call custom rule to only reward certificates to members with points outside of 2 week hold period.
        private const string ruleName = "AEO $10 Reward";

        public void Initialize ( NameValueCollection globals, NameValueCollection args, 
            long jobId, DAPDirectives config, NameValueCollection parameters, 
            DAPPerformanceCounterUtil performUtil )
        {

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, 
                MethodBase.GetCurrentMethod().Name, "Begin");

            if ( null == ConfigurationManager.AppSettings["FilePath"] )
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, 
                    MethodBase.GetCurrentMethod().Name, "No path defined in app.config");
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


            cellFileName =   path  + cellFileName.Replace("YYYYMMDDHHMMSS",this.processDate.ToString("yyyyMMddHHmmss"));
            emailFileName =  path + emailFileName.Replace("YYYYMMDDHHMMSS",this.processDate.ToString("yyyyMMddHHmmss"));

            string[] files = { cellFileName, emailFileName };

            foreach ( string filestr in files ) {
                StreamWriter sw = new StreamWriter(filestr, true);
                              
                sw.Write(this.getHeaderline());
                sw.Flush();
               
                sw.Close();
            }
         
            return;
        }


        public void ProcessMessageBatch (List<string> messageBatch)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            long ipCode = 0;
            List<long> memberRewardIDs = new List<long>();

            try
            {


                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach ( string str in messageBatch )
                {
                    //// Loading XML
                    doc.LoadXml(str);

                }

                // Get XML Node


                XmlNode xmlNode = doc.SelectSingleNode("Rewards/Reward");
                foreach ( XmlNode node in xmlNode.ChildNodes )
                {
                    switch ( node.Name.ToUpper() )
                    {

                        case "IPCODE":
                            ipCode = long.Parse(node.InnerText);
                            break;
                        default:
                            ipCode = 0;
                            break;
                    }
                }

                if ( xmlNode != null )
                {
                    if ( ipCode > 0 )
                    {
                        //We have a valid IPCode so get the member 
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            Member member = lwService.LoadMemberFromIPCode(ipCode);
                            if (member == null)
                            {
                                // Log error when member not found
                                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for IPCODE - " + xmlNode.Attributes["IPCODE"].Value);

                            }
                            else
                            {
                                //We have a valid member, so execute the IssueReward rule and then expire all the Returns that were part of a consumption.  
                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().Name, "Executing rule: " + ruleName);

                                ExecuteRule(member, out memberRewardIDs);

                                foreach (long memberRewardId in memberRewardIDs)
                                {
                                    MemberReward memberReward = lwService.GetMemberReward(memberRewardId);


                                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Reward = " + memberReward.Id);
                                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Reward is " + memberReward == null ? "null" : "not null");


                                    // collect data before writing to the output file.

                                    FlatFileLine fileLine = new FlatFileLine();

                                    // customber number & loyalty number


                                    VirtualCard lCard = null;
                                    foreach (VirtualCard loCard in member.LoyaltyCards)
                                    {
                                        if (loCard.Status == VirtualCardStatusType.Active && loCard.IsPrimary)
                                        {
                                            lCard = loCard;
                                            break;
                                        }
                                    }

                                    if (lCard == null)
                                    {
                                        throw new Exception("It was not possible to determine the loyaltynumber because no primary card was found for ipcode=" + member.IpCode);

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

                                    MemberDetails details = (loDetails == null || loDetails.Count == 0 ? null : loDetails[0]) as MemberDetails;

                                    // member details fields



                                    MemberStatusEnum stat = member.MemberStatus;


                                    switch (stat)
                                    {
                                        case MemberStatusEnum.Active:
                                            fileLine.AAP_FLAG = "1";
                                            break;
                                        case MemberStatusEnum.Merged:
                                            fileLine.AAP_FLAG = "2";
                                            break;
                                        case MemberStatusEnum.Terminated:
                                            fileLine.AAP_FLAG = "3";
                                            break;

                                        case MemberStatusEnum.NonMember:
                                        case MemberStatusEnum.PreEnrolled:
                                            fileLine.AAP_FLAG = "4";
                                            break;
                                        case MemberStatusEnum.Locked:
                                            fileLine.AAP_FLAG = "5";
                                            break;
                                    }

                                    if (fileLine.AAP_FLAG == string.Empty)
                                    {
                                        if (details.PendingCellVerification == 1 && details.PendingEmailVerification == 1)
                                        {
                                            fileLine.AAP_FLAG = "4";
                                        }
                                    }



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


                                    fileLine.REWARD_TYPE = memberReward.RewardDef == null ? string.Empty : memberReward.RewardDef.Name;
                                    if (fileLine.REWARD_TYPE == "AEO Rewards $10 Reward")
                                    {
                                        fileLine.REWARD_TYPE = "1";
                                    }
                                    else {

                                        List<string> brapromotions = new List<string>(new string[] { "Bra Reward", "Bra Reward-15", "Bra Reward-1", "B5G1 Bra Reward" });
                                        List<string> jeanpromotions = new List<string>(new string[] { "B5G1 Jean Reward" });

                                        if (brapromotions.Contains(fileLine.REWARD_TYPE))
                                        {
                                            fileLine.REWARD_TYPE = "2";
                                        }
                                        else {
                                            if (jeanpromotions.Contains(fileLine.REWARD_TYPE))
                                            {
                                                fileLine.REWARD_TYPE = "3";
                                            }
                                            else
                                            {
                                                fileLine.REWARD_TYPE = string.Empty;

                                            }
                                        }
                                    }

                                    // points

                                    IList<IClientDataObject> loBalances = member.GetChildAttributeSets("MemberPointBalances");

                                    MemberPointBalances balances = (loBalances == null || loBalances.Count == 0 ? null : loBalances[0]) as MemberPointBalances;


                                    fileLine.POINTS_BALANCE = balances == null ? "0" : balances.TotalPoints.ToString();
                                    fileLine.POINTS_NEEDED_FOR_NEXT_REWARD = balances == null ? "0" : balances.PointsToNextReward.ToString();
                                    fileLine.NUMBER_OF_BRAS_PURCHASED = balances == null ? "0" : balances.BraCurrentPurchased.ToString();
                                    fileLine.NUMBER_OF_JEANS_PURCHASED = balances == null ? "0" : balances.JeansRollingBalance.ToString();
                                    fileLine.CREDITS_TO_NEXT_FREE_BRA = "0";
                                    fileLine.CREDITS_TO_NEXT_FREE_JEAN = "0";

                                    // from configuration
                                    string tmpEID;
                                    string tmpCampaignID;

                                    using (var dtService = _dataUtil.DataServiceInstance())
                                    {
                                        tmpEID = dtService.GetClientConfiguration("$10_EID_EmailReward").Value;
                                        tmpCampaignID = dtService.GetClientConfiguration("$10_CampaignID_SMSReward").Value;
                                    }

                                    // from reward definition

                                    using (var contService = _dataUtil.ContentServiceInstance())
                                    {
                                        RewardDef def5 = contService.GetRewardDef("AEO Rewards $10 Reward");
                                        RewardDef defB5G1Jean = contService.GetRewardDef("B5G1 Jean Reward");
                                        RewardDef defB5G1Bra = contService.GetRewardDef("B5G1 Bra Reward");


                                        IList<MemberReward> rewards10 = lwService.GetMemberRewardsByDefId(member, def5.Id);
                                        IList<MemberReward> rewardsB5G1Bra = lwService.GetMemberRewardsByDefId(member, defB5G1Bra.Id);
                                        IList<MemberReward> rewardsB5G1Jean = lwService.GetMemberRewardsByDefId(member, defB5G1Jean.Id);


                                        long countRewards10 = 0;
                                        long countRewardsBra = 0;
                                        long countRewardsJean = 0;

                                        foreach (MemberReward loReward in rewards10)
                                        {
                                            if ((loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date)
                                                 && (loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date))
                                            {
                                                countRewards10++;
                                            }
                                        }

                                        foreach (MemberReward loReward in rewardsB5G1Bra)
                                        {
                                            if ((loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date)
                                                 && (loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date))
                                            {
                                                countRewardsBra++;
                                            }
                                        }


                                        foreach (MemberReward loReward in rewardsB5G1Jean)
                                        {
                                            if ((loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date)
                                                 && (loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date))
                                            {
                                                countRewardsJean++;
                                            }
                                        }


                                        fileLine.NUMBER_OF_ACTIVE_5_OFF_REWARD = countRewards10.ToString();
                                        fileLine.NUMBER_OF_ACTIVE_FREE_BRA_REWARD = countRewardsBra.ToString();
                                        fileLine.NUMBER_OF_ACTIVE_FREE_JEANS_REWARD = countRewardsJean.ToString();
                                    }
                                    if (loDetails != null && loDetails.Count > 0)
                                    {

                                        MemberDetails loTmp = (MemberDetails)loDetails[0];

                                        if (loTmp.MobilePhone == null)
                                        {
                                            loTmp.MobilePhone = string.Empty;
                                        }
                                        if (loTmp.EmailAddress == null)
                                        {
                                            loTmp.EmailAddress = string.Empty;
                                        }

                                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "determining which file to write");
                                        // if cell verification is not pending then add cell phone
                                        // and write to the cell file

                                        if (loTmp.PendingCellVerification == 0)   /* AEO-999 AH */
                                        {

                                            fileLine.DELIVERY_CHANNEL = "2";
                                            fileLine.eid = string.Empty;
                                            fileLine.CAMPAIGN_ID = tmpCampaignID;

                                            StreamWriter sw = new StreamWriter(cellFileName, true);


                                            sw.Write(fileLine.toFormattedString(FileFormat.Cell));
                                            sw.Flush();
                                            sw.Close();
                                            rowsWrittenCell++;
                                        }

                                        //if ( loTmp.PendingEmailVerification== null || loTmp.PendingEmailVerification == 0  ) // AEO-557
                                        if (loTmp.EmailAddress != string.Empty) /*AEO-999 AH*/
                                        {

                                            fileLine.DELIVERY_CHANNEL = "1";
                                            fileLine.eid = tmpEID;
                                            fileLine.CAMPAIGN_ID = string.Empty;

                                            StreamWriter sw = new StreamWriter(emailFileName, true);

                                            sw.Write(fileLine.toFormattedString(FileFormat.Email));
                                            sw.Flush();
                                            sw.Close();
                                            rowsWrittenEmail++;

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,
                    MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);

                throw new Exception(ex.Message);
            }

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                MethodBase.GetCurrentMethod().Name, "End");

        }

        public void ExecuteRule ( Member member, out List<long> memberRewardIds)
        {
            try
            {
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    Dictionary<string, string> additionalFields = new Dictionary<string, string>();
                    additionalFields.Add("TypeCode", "5D"); //keep using 5DDM
                    additionalFields.Add("RewardType", "$10"); // pass the reward type
                    memberRewardIds = new List<long>();

                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                        MethodBase.GetCurrentMethod().Name, "Member: " + member.IpCode);

                    //Create a context object and assign the member and MemberDetails attribute set
                    //to be used in invoking the IssueReward for this reward and the AwardPoints 
                    //rule associated with the View Earned Reward 

                    ContextObject cobj = new ContextObject();
                    cobj.Owner = member;
                    cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];
                    cobj.Environment = additionalFields.ToDictionary(pair => pair.Key, pair => (object)pair.Value);


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
                        long rewardID = long.Parse(result.Detail);
                        memberRewardIds.Add(rewardID);
                    }

                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "number of rewards issued =" + memberRewardIds.Count.ToString());
                }
            }
            catch ( Exception ex )
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
                throw new Exception(ex.Message, new Exception("System Error"));
            }
        }

        private string getHeaderline ( ) {
            StringBuilder tmp = new StringBuilder();

            tmp.Append("DELIVERY_CHANNEL|CUSTOMER_NBR|LOYALTY_NUMBER|EMAIL|MOBILE_NUMBER|FNAME|LNAME|ADDRESS1|ADDRESS2|CITY|STATE|ZIP|COUNTRY|AUTH_CD|SUC_CD|CAMPAIGN_TYPE|CAMPAIGN_EXP_DATE|");
            tmp.Append("EID|CAMPAIGN_ID|LANGUAGE_PREFERENCE|GENDER|BIRTHDATE|STORE_LOYALTY|TIER_STATUS|POINTS_BALANCE|POINTS_NEEDED_FOR_NEXT_REWARD|NUMBER_OF_BRAS_PURCHASED|CREDITS_TO_NEXT_FREE_BRA|NUMBER_OF_JEANS_PURCHASED|CREDITS_TO_NEXT_FREE_JEAN|NUMBER_OF_ACTIVE_5_OFF_REWARD|NUMBER_OF_ACTIVE_FREE_JEANS_REWARD|");
            tmp.Append("NUMBER_OF_ACTIVE_FREE_BRA_REWARD|COMMUNICATION_ID|COMM_PLAN_ID|COLLATERAL_ID|PACKAGE_ID|");           
            tmp.Append("STEP_ID|MESSAGE_ID|SEG_ID|SEG_NM|AAP_FLAG|CARD_TYPE|RUN_ID|LEAD_KEY_ID|SITE_URL|ENABLE_PASSBOOK_PASS|TIMESTAMP");

            tmp.AppendLine();

            return tmp.ToString();
        }

        public int Shutdown ( )
        {
            return 0;
        }

        public void Dispose ( )
        {
            return;
        }
    }

    
}