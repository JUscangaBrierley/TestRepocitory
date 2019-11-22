using System;
using System.Collections.Generic;
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


using AmericanEagle.SDK.Global;
using Brierley.Clients.AmericanEagle.DataModel;
namespace AmericanEagle.SDK.OutputProviders
{
    public class BirthdayRewardFulfillment : IDAPOutputProvider
    {
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        private string cellFileName   = "AEORW_SM_Brthday_YYYYMMDDHHMMSS.txt";
        private string emailFileName =  "AEORW_EM_Brthday_YYYYMMDDHHMMSS.txt";
        private string path = string.Empty;
        private DateTime processDate = DateTime.Now;
        public const string FieldSeparator = "|";
        private long rowsWrittenCell = 0;
        private long rowsWrittenEmail = 0;
        private FileUtils utils = new FileUtils();


        //RKG - changed to call custom rule to only reward certificates to members with points outside of 2 week hold period.
        private const string ruleName = "AEO Birthday Reward";
        private string typeCode = "BDAY";

        private String linkkey = string.Empty;
        private String cardtype = string.Empty;
        private string pendingemailverification = string.Empty;
        private string pendingcellverification = string.Empty;
        private string emailaddress = string.Empty;
        private string mobilephone = string.Empty;
        private string addresslineone = string.Empty;
        private string addresslinetwo = string.Empty;
        private string city = string.Empty;
        private string stateorprovince = string.Empty;
        private string ziporpostalcode = string.Empty;
        private string country = string.Empty;
        private string languagepreference = string.Empty;
        private string gender = string.Empty;
        private string homestoreid = string.Empty;
        private string firstname = string.Empty;
        private string lastname = string.Empty;
        private string birthdate = string.Empty;
        private string memberstatus = string.Empty;
        private string totalpoints = string.Empty;
        private string pointstonextreward = string.Empty;
        private string bracurrnetpurchased = string.Empty;
        private string jeansrollingbalance = string.Empty;
        private string b5g1jean = string.Empty;
        private string b5g1bra = string.Empty;
        private string aeo5reward = string.Empty;

        public void Initialize ( System.Collections.Specialized.NameValueCollection globals, System.Collections.Specialized.NameValueCollection args, long jobId, DAPDirectives config, System.Collections.Specialized.NameValueCollection parameters, DAPPerformanceCounterUtil performUtil )
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


        public void ProcessMessageBatch ( IList<string> messageBatch )
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
             string methodName = MethodBase.GetCurrentMethod().Name;
            string certificateNumber = string.Empty;
            string offerCode = string.Empty;
            DateTime expirationDate = DateTime.Now;

            string lid = string.Empty;
            string birthdayTypeCode = string.Empty;
            List<long> memberRewardIDs = new List<long>();

            try
            {

                this.typeCode = "BDAY";

                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach ( string str in messageBatch )
                {
                    //// Loading XML
                    doc.LoadXml(str);

                }

                // Get XML Node


                XmlNode xmlNode = doc.SelectSingleNode("BirthdayReward/Global");
                foreach ( XmlNode node in xmlNode.ChildNodes )
                {
                    switch ( node.Name.ToUpper() )
                    {

                        case "LOYALTYIDNUMBER":
                            lid = node.InnerText.Trim();
                            break;

                        case "LINKKEY":
                            linkkey = node.InnerText.Trim();
                            break;
                        case "CARDTYPE":
                            cardtype = node.InnerText.Trim();
                            break;
                        case "PENDINGEMAILVERIFICATION":
                            pendingemailverification = node.InnerText.Trim();
                            break;
                        case "PENDINGCELLVERIFICATION":
                            pendingcellverification = node.InnerText.Trim();
                            break;

                        case "EMAILADDRESS":
                            emailaddress = node.InnerText.Trim();
                            break;

                        case "MOBILEPHONE":
                            mobilephone = node.InnerText.Trim();
                            break;

                        case "ADDRESSLINEONE":
                            addresslineone = node.InnerText.Trim();
                            break;

                        case "ADDRESSLINETWO":
                            addresslinetwo = node.InnerText.Trim();
                            break;

                        case "CITY":
                            city = node.InnerText.Trim();
                            break;

                        case "STATEORPROVINCE":
                            stateorprovince = node.InnerText.Trim();
                            break;


                        case "ZIPORPOSTALCODE":
                            ziporpostalcode = node.InnerText.Trim();
                            break;

                        case "COUNTRY":
                            country = node.InnerText.Trim();
                            break;

                        case "LANGUAGEPREFERENCE":
                            languagepreference = node.InnerText.Trim();
                            break;


                        case "HOMESTOREID":
                            homestoreid = node.InnerText.Trim();
                            break;

                        case "FIRSTNAME":
                            firstname = node.InnerText.Trim();
                            break;

                        case "LASTNAME":
                            lastname = node.InnerText.Trim();
                            break;

                        case "GENDER":
                            gender = node.InnerText.Trim();
                            break;

                        case "BIRTHDATE":
                            birthdate = node.InnerText.Trim();
                            break;

                        case "MEMBERSTATUS":
                            memberstatus = node.InnerText.Trim();
                            break;

                        case "TOTALPOINTS":
                            totalpoints = node.InnerText.Trim();
                            break;

                        case "POINTSTONEXTREWARD":
                            pointstonextreward = node.InnerText.Trim();
                            break;

                        case "BRACURRENTPURCHASED":
                            bracurrnetpurchased = node.InnerText.Trim();
                            break;

                        case "JEANSROLLINGBALANCE":
                            jeansrollingbalance = node.InnerText.Trim();
                            break;

                        case "B5G1JEAN":
                            b5g1jean = node.InnerText.Trim();
                            break;

                        case "B5G1BRA":
                            b5g1bra = node.InnerText.Trim();
                            break;

                        case "AEO5REWARD":
                            aeo5reward = node.InnerText.Trim();
                            break;

                        default:
                            lid = string.Empty;
                            break;
                    }
                }

                if ( xmlNode != null )
                {
                    if ( lid.Length > 0 )
                    {
                        //We have a valid lid so get the member 
                        Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(lid);
                        if ( member == null )
                        {
                            // Log error when member not found
                            this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for LID - " + xmlNode.Attributes["LoyaltyIdNumber"].Value);

                        }
                        else
                        {

                            if ( pendingcellverification != null && pendingcellverification == "0" && (mobilephone == null || mobilephone == "" )) {
                                return;
                                
                            }


                            if ( pendingemailverification != null && pendingemailverification == "0" && ( emailaddress == null || emailaddress == "" ) ) {
                                return;
                            }


                         
                                                       
                            string offerType = String.Empty;
                            if ( pendingcellverification != null && pendingcellverification == "0" ) {
                                offerType = "BirthdaySM";
                            }
                            else if (pendingemailverification != null && pendingemailverification == "0" ) {
                                offerType = "BirthdayEM";
                            }

                            if ( cardtype == null || cardtype=="" || cardtype == "0" ) {
                               
                                typeCode = string.Concat(typeCode, "15");
                            }
                            else {

                                if ( cardtype == "1" || cardtype == "2" || cardtype == "3" ) {
                                    
                                    typeCode = string.Concat(typeCode, "20");
                                }
                               

                            }
                           

                            //We have a valid member, so execute the IssueReward rule and then expire all the Returns that were part of a consumption.  
                            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Executing rule: " + ruleName);

                            ExecuteRule(member, offerType,out memberRewardIDs);

                            foreach (long memberRewardId in memberRewardIDs)
                            {
                                MemberReward memberReward = LWDataServiceUtil.DataServiceInstance(true).GetMemberReward(memberRewardId);


                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Reward = " + memberReward.Id);
                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Reward is " + memberReward == null ? "null" : "not null");


                                // collect data before writing to the output file.

                                FlatFileLine fileLine= new FlatFileLine();                             
                                
                                // customber number & loyalty number
                                fileLine.CUSTOMER_NBR = linkkey;
                                fileLine.LOYALTY_NUMBER = lid;


                                // firstname, lastnames & tier status
                                fileLine.FNAME = firstname == null ? "" : firstname;
                                fileLine.LNAME = lastname == null ? "" : lastname;
                            
                            
                                MemberTier tierTmp = member.GetTier(DateTime.Now.Date);

                                fileLine.TIER_STATUS = tierTmp == null || tierTmp.TierDef == null ? string.Empty : tierTmp.TierDef.Name;
                                switch ( fileLine.TIER_STATUS.ToUpper().Trim() )
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

                                fileLine.BIRTHDATE = birthdate == null ? "" : birthdate;


                            
                                // member details fields



                                string stat = memberstatus;


                                switch ( stat )
                                {
                                    case "1":
                                        fileLine.AAP_FLAG = "1";
                                        break;
                                    case "6":
                                        fileLine.AAP_FLAG = "2";
                                        break;
                                    case "3":
                                        fileLine.AAP_FLAG = "3";
                                        break;

                                    case "5":
                                    case "7":
                                        fileLine.AAP_FLAG = "4";
                                        break;
                                    case "4":
                                        fileLine.AAP_FLAG = "5";
                                        break;
                                }

                                if ( fileLine.AAP_FLAG == string.Empty )
                                {
                                    if ( pendingcellverification == "1" && pendingemailverification == "1" )
                                    {
                                        fileLine.AAP_FLAG = "4";
                                    }
                                }

                                fileLine.MOBILE_NUMBER = mobilephone == null ? "" : mobilephone;
                                fileLine.email = emailaddress == null ? "" : emailaddress;
                                fileLine.ADDRESS1 = addresslineone == null ? "" : addresslineone;
                                fileLine.ADDRESS2 = addresslinetwo == null ? "" : addresslinetwo;
                                fileLine.CITY = city == null ? "" : city;
                                fileLine.MOBILE_NUMBER = mobilephone == null ? "" : mobilephone;
                                fileLine.STATE = stateorprovince == null ? "" : stateorprovince;
                                fileLine.ZIP = ziporpostalcode == null ? "" : ziporpostalcode;
                                fileLine.REGION = country == null ? "" : country;
                                fileLine.LANGUAGE_PREFERENCE = languagepreference == null ? "0" : languagepreference;
                                fileLine.GENDER = gender == null ? "0" : gender;
                                fileLine.STORE_LOYALTY = homestoreid;
                                fileLine.CARD_TYPE = cardtype == null ? "" : cardtype;


                                // memberrewards fields

                                fileLine.THREE_DIGIT_REWARDS_CODE = memberReward.OfferCode;
                                fileLine.TWENTY_DIGIT_REWARD_CODE = memberReward.CertificateNmbr;
                                fileLine.REWARD_EXP_DATE = memberReward.Expiration == null ? "" : String.Format("{0:dd-MMM-yyyy}", memberReward.Expiration);
                                fileLine.REWARD_TYPE = string.Empty;

                                // points

                                fileLine.POINTS_BALANCE = totalpoints == null ? "0" : totalpoints;
                                fileLine.POINTS_NEEDED_FOR_NEXT_REWARD = pointstonextreward == null ? "0" : pointstonextreward;
                                fileLine.NUMBER_OF_BRAS_PURCHASED = bracurrnetpurchased == null ? "0" : bracurrnetpurchased;
                                fileLine.NUMBER_OF_JEANS_PURCHASED = jeansrollingbalance == null ? "0" : jeansrollingbalance;
                                fileLine.CREDITS_TO_NEXT_FREE_BRA = ( 5 - int.Parse(fileLine.NUMBER_OF_BRAS_PURCHASED) ).ToString();
                                fileLine.CREDITS_TO_NEXT_FREE_JEAN = ( 5 - int.Parse(fileLine.NUMBER_OF_BRAS_PURCHASED) ).ToString();

                                // from configuration

                                string CampaignCredit = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("CampaignID_SMSBirthday_Credit").Value;
                                string CampaignNoCredit = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("CampaignID_SMSBirthday_NoCredit").Value;
                                string EIDCredit = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("EID_EmailBirthday_Credit").Value;
                                string EIDNoCredit = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("EID_EmailBirthday_NoCredit").Value;



                                if ( cardtype == null || ! ( cardtype == "1" || cardtype == "2" || cardtype == "3" )) {
                                    fileLine.CAMPAIGN_ID = CampaignNoCredit == null ? string.Empty : CampaignNoCredit;
                                    fileLine.eid = EIDNoCredit == null ? string.Empty : EIDNoCredit;
                                 
                                }
                                else {

                                    if ( cardtype == "1" || cardtype == "2" || cardtype == "3" ) {
                                        fileLine.CAMPAIGN_ID = CampaignCredit == null ? string.Empty : CampaignCredit;
                                        fileLine.eid = EIDCredit == null ? string.Empty : EIDCredit;
                                      
                                    }
                                    else {
                                        fileLine.CAMPAIGN_ID = string.Empty;
                                        fileLine.eid = string.Empty;
                                    }

                                }
                                     

                                // from reward definition


                                // from reward definition                             

                                fileLine.NUMBER_OF_ACTIVE_5_OFF_REWARD = aeo5reward == null ? "0" : aeo5reward;
                                fileLine.NUMBER_OF_ACTIVE_FREE_BRA_REWARD = b5g1bra == null ? "0" : b5g1bra;
                                fileLine.NUMBER_OF_ACTIVE_FREE_JEANS_REWARD = b5g1jean == null ? "0" : b5g1jean;

                                

                               if (mobilephone == null)
                               {
                                   mobilephone = string.Empty;
                               }
                               if (emailaddress == null)
                               {
                                   emailaddress = string.Empty;
                               }
                               
                               

                               this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "determining which file to write");
                               // if cell verification is not pending then add cell phone
                               // and write to the cell file

                               if ( pendingcellverification != null &&  pendingcellverification == "0" ) // AEO-557 
                               {
                                   String tmpEID = fileLine.eid;

                                   fileLine.DELIVERY_CHANNEL = "2";
                                   fileLine.eid = string.Empty;

                                   

                                   StreamWriter sw = new StreamWriter(cellFileName, true);


                                   sw.Write(fileLine.toFormattedString(FileFormat.Cell));
                                   sw.Flush();
                                   sw.Close();
                                   rowsWrittenCell++;

                                   fileLine.eid = tmpEID;
                               }


                               if ( pendingemailverification != null && pendingemailverification == "0" ) // AEO-557
                               {
                                       String tmpCampaignID = fileLine.CAMPAIGN_ID;


                                       fileLine.DELIVERY_CHANNEL = "1";
                                       fileLine.CAMPAIGN_ID = string.Empty;
                                       StreamWriter sw = new StreamWriter(emailFileName, true);

                                       sw.Write(fileLine.toFormattedString(FileFormat.Email));
                                       sw.Flush();
                                       sw.Close();
                                       rowsWrittenEmail++;

                                       fileLine.CAMPAIGN_ID = tmpCampaignID;

                               }
                               
                               
                            }
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
                throw new Exception(ex.Message);
            }

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");

        }

        public void ExecuteRule ( Member member, String offerType , out List<long> memberRewardIds ) {
            try {
                Dictionary<string, string> additionalFields = new Dictionary<string, string>();
                additionalFields.Add("TypeCode", typeCode);
                additionalFields.Add("OfferType", offerType);


                // --- FIX for SMS file
                if ( offerType == "BirthdaySM" ) {
                    additionalFields.Add("MonthOffset", "1");
                }
                else {
                    additionalFields.Add("MonthOffset", "1");
                }
                  
                                // --- FIX for SMS file
                memberRewardIds = new List<long>();

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member: " + member.IpCode);

                //Create a context object and assign the member and MemberDetails attribute set
                //to be used in invoking the IssueReward for this reward and the AwardPoints 
                //rule associated with the View Earned Reward 

                ContextObject cobj = new ContextObject();
                cobj.Owner = member;
                cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];
                cobj.Environment = additionalFields.ToDictionary(pair => pair.Key, pair => (object)pair.Value);

                RuleTrigger ruleTrigger = LWDataServiceUtil.DataServiceInstance(true).GetRuleByName(ruleName);

                if ( ruleTrigger == null ) {
                    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule: (" + ruleName + ") Not Defined");
                    throw new Exception(ruleName + " Rule Not Defined", new Exception(ruleName + " Rule Not Defined"));
                }
                else {
                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule: (" + ruleTrigger.RuleName + ")");
                }


                LWDataServiceUtil.DataServiceInstance(true).Execute(ruleTrigger, cobj);

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule executed");

                foreach ( ContextObject.RuleResult result in cobj.Results ) {

                    if ( result.Detail != null && result.Detail.IndexOf("id=", 0) >= 0 ) {
                        result.Detail = result.Detail.Remove(result.Detail.IndexOf("id=", 0), 3);
                        long rewardID = long.Parse(result.Detail);
                        memberRewardIds.Add(rewardID);
                    }
                    
                }

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "number of rewards issued =" + memberRewardIds.Count.ToString());

            }
            catch ( Exception ex ) {
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