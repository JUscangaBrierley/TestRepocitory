//--------------------------------------------------------------------------------
// <copyright file="VendorEmailUpdate.cs" company="Brierley and Partners">
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
    using System.IO;
    using System.Configuration;
    using System.Text;

    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.DataAcquisition.Core;
    using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
    using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;

    /// <summary>
    /// Class VendorEmailBirthday
    /// </summary>
    public class VendorEmailBirthday : IDAPOutputProvider
    {

        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        private string outfilename = "AEORW_EM_Brthday_YYYYMMDDHHMMSS.txt";

        private string jobname = "";

        // private string strLoyaltyNumber = string.Empty;
        /// <summary>
        /// Stores the email address that is to be changed
        /// </summary>
        private string strLoyaltyNumber = string.Empty;


        private string path = string.Empty;
        private DateTime processDate = DateTime.Now;

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);


        private FileUtils utils = new FileUtils();
        private const string typeCode = "BDAY";

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.    
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// This method is called to initialize the message dispatcher
        /// </summary>
        /// <param name="globals">NameValueCollection globals</param>
        /// <param name="args">NameValueCollection args</param>
        /// <param name="jobId">long jobId</param>
        /// <param name="config">DAPDirectives config</param>
        /// <param name="parameters">NameValueCollection parameters</param>
        /// <param name="performUtil">DAPPerformanceCounterUtil performUtil</param>
        public void Initialize(NameValueCollection globals, NameValueCollection args, long jobId, DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {


            if (globals.HasKeys())
            {
                string[] values = globals.GetValues("JobName");
                if (values != null && values.Length > 0 && values[0] != null && values[0].Trim().Length > 0)
                {
                    this.jobname = values[0];
                }
            }

            if (null == ConfigurationManager.AppSettings["FilePath"])
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No path defined in app.config");
                throw new Exception("No path defined in app.config");
            }
            else
            {

                path = ConfigurationManager.AppSettings["FilePath"];
                path = path + "\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            if (ConfigurationManager.AppSettings["ProcessDate"] != null)
            {
                string strProcessDate = ConfigurationManager.AppSettings["ProcessDate"];
                DateTime.TryParse(strProcessDate, out processDate);
            }

            this.outfilename = path + this.outfilename;
            this.outfilename = this.outfilename.Replace("YYYYMMDDHHMMSS", this.processDate.ToString("yyyyMMddHHmmss"));

            StreamWriter sw = new StreamWriter(this.outfilename, false);

            sw.Write(this.getHeaderline());
            sw.Flush();

            sw.Close();


        }

        /// <summary>
        /// This method is called to process the messages in the batch
        /// </summary>
        /// <param name="messageBatch">String List</param>
        public void ProcessMessageBatch(IList<string> messageBatch)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            string certificateNumber = string.Empty;
            string offerCode = string.Empty;
            string birthdayTypeCode = string.Empty;
            DateTime expirationDate = DateTime.MinValue;

            try
            {
                // Tracing for starting of method
                this.logger.Trace(this.className, methodName, "Starts");
                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    // Loding XML
                    doc.LoadXml(str);
                }
                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("CheetahMailBirthday/Global");

                // check for valid xml data
                if (null != xmlNode)
                {
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        switch (node.Name.ToUpper())
                        {

                            case "LOYALTYIDNUMBER":
                                strLoyaltyNumber = node.InnerText;
                                break;
                            default:
                                strLoyaltyNumber = string.Empty;
                                break;
                        }
                    }

                    //Vendor Inputs-Loyalty number and new email address are captured
                    //strLoyaltyNumber = xmlNode.Attributes["LoyaltyIdNumber"].Value.Trim();

                    if (!string.IsNullOrEmpty(strLoyaltyNumber))
                    {
                        // Get member
                        Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(strLoyaltyNumber);

                        if (null == member)
                        {
                            // Log error when member not found
                            throw new Exception(this.className + " " + methodName + " Member Not Found for Loyalty Number - " + strLoyaltyNumber.ToString());
                        }

                        IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
                        MemberDetails memberDetails = mbrDtlObjs[0] as MemberDetails;

                        if (memberDetails != null)
                        {

                            VirtualCard vc = member.GetLoyaltyCard(this.strLoyaltyNumber);

                            if (vc == null)
                            {
                                throw new Exception("It was not possible to retrieve the Virtual Card from LoyaltyNumber=" + this.strLoyaltyNumber);
                            }

                            if (Utilities.isInPilot(memberDetails.ExtendedPlayCode) ) // point conversion
                            {

                                //we should write to a file
                                FlatFileLine fileLine = new FlatFileLine();

                                fileLine.LOYALTY_NUMBER = this.strLoyaltyNumber;
                                fileLine.eid = "1";

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
                                    throw new Exception("It was not posibble to determine the loyaltynumber because no primary card was found for ipcode=" + member.IpCode);

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

                                fileLine.email = memberDetails.EmailAddress.Trim();
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

                                string EIDCredit = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("EID_EmailBirthday_Credit").Value;
                                string EIDNoCredit = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("EID_EmailBirthday_NoCredit").Value;


                                if (details.CardType == null || details.CardType == 0)
                                {
                                    fileLine.eid = EIDNoCredit == null ? string.Empty : EIDNoCredit;
                                    birthdayTypeCode = string.Concat(typeCode, "15");
                                }
                                else
                                {

                                    if (details.CardType >= 1 && details.CardType <= 3)
                                    {
                                        fileLine.eid = EIDCredit == null ? string.Empty : EIDCredit;
                                        birthdayTypeCode = string.Concat(typeCode, "20");
                                    }
                                    else
                                    {
                                        fileLine.eid = string.Empty;
                                    }

                                }

                                // memberrewards fields
                                string offerTypeCode = utils.GetOffer(birthdayTypeCode, DateTime.Today.Month, FileUtils.OfferType.Email);

                                this.logger.Trace(this.className, methodName, "Getting codes for: " + offerTypeCode);
                                utils.GetNextRewardCertificateNumber(member.IpCode, offerTypeCode, out certificateNumber, out offerCode, out expirationDate);

                                fileLine.THREE_DIGIT_REWARDS_CODE = offerCode;
                                fileLine.TWENTY_DIGIT_REWARD_CODE = certificateNumber;
                                fileLine.REWARD_EXP_DATE = expirationDate.ToString("dd-MMM-yyyy");
                                fileLine.REWARD_TYPE = string.Empty;

                                fileLine.CAMPAIGN_ID = string.Empty;

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

                                foreach (MemberReward loReward in rewards5)
                                {
                                    if ((loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date)
                                            && (loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date)
                                        && (loReward.FPCancellationNumber == string.Empty))
                                    {
                                        countRewards5++;
                                    }
                                }

                                foreach (MemberReward loReward in rewardsB5G1Bra)
                                {
                                    if ((loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date)
                                            && (loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date)
                                        && (loReward.FPCancellationNumber == string.Empty))
                                    {
                                        countRewardsBra++;
                                    }
                                }


                                foreach (MemberReward loReward in rewardsB5G1Jean)
                                {
                                    if ((loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > this.processDate.Date)
                                            && (loReward.Expiration == null || loReward.Expiration.Value.Date > this.processDate.Date)
                                        && (loReward.FPCancellationNumber == string.Empty || loReward.FPCancellationNumber == null))
                                    {
                                        countRewardsJean++;
                                    }
                                }

                                fileLine.NUMBER_OF_ACTIVE_5_OFF_REWARD = countRewards5.ToString();
                                fileLine.NUMBER_OF_ACTIVE_FREE_BRA_REWARD = countRewardsBra.ToString();
                                fileLine.NUMBER_OF_ACTIVE_FREE_JEANS_REWARD = countRewardsJean.ToString();

                                fileLine.DELIVERY_CHANNEL = "1";

                                StreamWriter sw = new StreamWriter(this.outfilename, true);

                                sw.Write(fileLine.toFormattedString(FileFormat.CheetaMailValidatioFileLoad));
                                sw.Flush();
                                sw.Close();
                            }


                        }
                        else
                        {
                            throw new Exception("It was not possible to retrieve details for the member " + this.strLoyaltyNumber);
                        }
                    }
                    else
                    {
                        throw new Exception("The lotaltynumebr is empty " + this.strLoyaltyNumber);
                    }

                }
                else
                {
                    // Logging for null xml node
                    this.logger.Error(this.className, methodName, "xml node not found");
                }

                // Logging for ending of method
                this.logger.Trace(this.className, methodName, "Ends");
            }
            catch (Exception ex)
            {
                // Logging for exception
                this.logger.Error(this.className, methodName, ex.Message);
            }
        }

        private string getHeaderline()
        {


            StringBuilder tmp = new StringBuilder();

            tmp.Append("DELIVERY_CHANNEL|CUSTOMER_NBR|LOYALTY_NUMBER|EMAIL|MOBILE_NUMBER|FNAME|LNAME|ADDRESS1|ADDRESS2|CITY|STATE|ZIP|COUNTRY|AUTH_CD|SUC_CD|CAMPAIGN_TYPE|CAMPAIGN_EXP_DATE|");
            tmp.Append("EID|CAMPAIGN_ID|LANGUAGE_PREFERENCE|GENDER|BIRTHDATE|STORE_LOYALTY|TIER_STATUS|POINTS_BALANCE|POINTS_NEEDED_FOR_NEXT_REWARD|NUMBER_OF_BRAS_PURCHASED|CREDITS_TO_NEXT_FREE_BRA|NUMBER_OF_JEANS_PURCHASED|CREDITS_TO_NEXT_FREE_JEAN|NUMBER_OF_ACTIVE_5_OFF_REWARD|NUMBER_OF_ACTIVE_FREE_JEANS_REWARD|");
            tmp.Append("NUMBER_OF_ACTIVE_FREE_BRA_REWARD|COMMUNICATION_ID|COMM_PLAN_ID|COLLATERAL_ID|PACKAGE_ID|");
            tmp.Append("STEP_ID|MESSAGE_ID|SEG_ID|SEG_NM|AAP_FLAG|CARD_TYPE|RUN_ID|LEAD_KEY_ID|SITE_URL|ENABLE_PASSBOOK_PASS|TIMESTAMP");

            tmp.AppendLine();


            return tmp.ToString();
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
