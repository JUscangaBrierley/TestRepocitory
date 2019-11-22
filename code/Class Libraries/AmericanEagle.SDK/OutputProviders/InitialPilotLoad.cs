// ----------------------------------------------------------------------------------
// <copyright file="VendorEmailEventError.cs" company="Brierley and Partners">
//     Copyright statement. All right reserved
// </copyright>
// ----------------------------------------------------------------------------------

namespace AmericanEagle.SDK.OutputProviders
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using AmericanEagle.SDK.Global;
    using AmericanEagle.SDK.OutputProviders;
    using Brierley.Clients.AmericanEagle.DataModel;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.DataAcquisition.Core;
    using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
    using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;


    class InitialPilotLoad : IDAPOutputProvider
    {

        private string mobileFileName = "AEORW_SM_Plaunch_YYYYMMDDHHMMSS.txt";
        private string emailFileName = "AEORW_EM_Plaunch_YYYYMMDDHHMMSS.txt";

        /// <summary>
        /// Loyalty Number of the member
        /// </summary>
        private string LoyaltyNumber = string.Empty;

        /// <summary>
        /// Either Silver, Gold, or Select to upgrade
        /// </summary>
        private string Tier = string.Empty;

        /// <summary>
        /// Validation Required Flag
        /// </summary>
        private string ValidationRequiredFlag = string.Empty;

        /// <summary>
        /// Elite Reason Code
        /// </summary>
        private string EliteReasonCode = string.Empty;

        /// <summary>
        /// AECC Card Holder Flag
        /// </summary>
        private string aECC_Cardholder = string.Empty;

        /// <summary>
        /// Point Type
        /// </summary>
        private  string sPointType = string.Empty ;

        /// <summary>
        /// Point Event
        /// </summary>
        private string sPointEvent = string.Empty;

         /// <summary>
        /// Point Event Obj
        /// </summary>
          private  PointEvent pointEvent = null;

        /// <summary>
        /// Point Type Obj
        /// </summary>
          private PointType pointType = null;



        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);

        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

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
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            string fileDirectory = ConfigurationManager.AppSettings["FilePath"];

            if (null == fileDirectory)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No path defined in app.config");
                throw new Exception("No path defined in app.config");
            }

            fileDirectory = fileDirectory + "\\";
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            mobileFileName = Path.Combine(fileDirectory, mobileFileName.Replace("YYYYMMDDHHMMSS", DateTime.Now.ToString("yyyyMMddHHmmss")));
            emailFileName = Path.Combine(fileDirectory, emailFileName.Replace("YYYYMMDDHHMMSS", DateTime.Now.ToString("yyyyMMddHHmmss")));

            // we allways create the  2 possible output files
            StreamWriter sw = new StreamWriter(mobileFileName, false);
            sw.Close();

            StreamWriter sw2 = new StreamWriter(emailFileName, false);
            sw2.Close();

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// This method is called to process the messages in the batch
        /// </summary>
        /// <param name="messageBatch">String List</param>
        public void ProcessMessageBatch(IList<string> messageBatch)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            DateTime processDate = DateTime.Now;

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
                XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/Global");

                //We create the headers if are not created
                CreateHeadersIfNotCreated();

                // check for valid xml data
                if (null != xmlNode)
                {
                    LoyaltyNumber = xmlNode.Attributes["LoyaltyNumber"].Value.Trim();
                    Tier = (xmlNode.Attributes["Tier"].Value).Trim().ToLower();
                    ValidationRequiredFlag = xmlNode.Attributes["ValidationRequiredFlag"].Value;
                    EliteReasonCode = xmlNode.Attributes["EliteReasonCode"].Value;
                    aECC_Cardholder = xmlNode.Attributes["AECC_Cardholder"].Value;
                    int _EID  = 0;

                    //if they don't send us a reason code, then default to AE Nomination.
                    if (EliteReasonCode.Length == 0)
                    {
                        EliteReasonCode = "3";
                    }

                    if (!string.IsNullOrEmpty(LoyaltyNumber))
                    {
                        // Get member
                        Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(LoyaltyNumber);

                        if (null == member)
                        {
                            // Log error when member not found
                            this.logger.Error(this.className, methodName, "Member Not Found for Loyalty Number - " + LoyaltyNumber);
                            return;
                        }

                        string tierName = string.Empty;
                        switch (Tier)
                        {
                            case "1":
                                tierName = "Blue";
                                break;
                            case "2":
                                tierName = "Silver";
                                break;
                            case "3":
                                tierName = "Gold";
                                break;
                            case "4":
                                tierName = "Select";
                                break;
                            default:
                                tierName = "Blue";
                                break;
                        }

                        LWCriterion crit = new LWCriterion("RefTierReason");
                        crit.Add(LWCriterion.OperatorType.AND, "ReasonCode", EliteReasonCode , LWCriterion.Predicate.Eq);

                        IList<IClientDataObject> objRefTiersReason = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "RefTierReason", crit, null, false);

                        if(objRefTiersReason!= null)
                        {
                            RefTierReason RefTierReason = (RefTierReason)objRefTiersReason[0];
                            if (objRefTiersReason.Count > 0)
                            {                                
                                member.AddTier(tierName, DateTime.Today, DateTime.Parse("12/31/2199"), RefTierReason.Description);
                            }
                            else
                            {
                                member.AddTier(tierName, DateTime.Today, DateTime.Parse("12/31/2199"), "Base");
                            }
                        }

                        //Get the member details
                        IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
                        MemberDetails memberDetails = (MemberDetails)mbrDtlObjs[0];
                        VirtualCard vc = member.GetLoyaltyCard(LoyaltyNumber);
                        IList<IClientDataObject> loBalances = member.GetChildAttributeSets("MemberPointBalances");

                        MemberPointBalances balances = ( loBalances == null || loBalances.Count == 0 ? null : loBalances[0] ) as MemberPointBalances;


                        if (memberDetails != null)
                        {
                            memberDetails.AITUpdate = true;

                            if (memberDetails.ExtendedPlayCode != 1)
                            {
                                memberDetails.ExtendedPlayCode = 1;
                            }

                            if (ValidationRequiredFlag =="1")
                            {
                                if ((memberDetails.CardType == (long) CardType.AECCMember)
                                    || (memberDetails.CardType == (long)CardType.AECCAndAEVisaMember)
                                    || (memberDetails.CardType == (long)CardType.AEVisaMember))
                                {
                                    _EID = 267075;
                                }
                                else
                                {
                                    _EID = 267074;
                                   
                                };

                                memberDetails.PendingEmailVerification = 1;
                                memberDetails.NextEmailReminderDate = DateTime.Today.AddDays(15);
                            }
                            else if (ValidationRequiredFlag =="0") 
                            {
                                if ((memberDetails.CardType == (long)CardType.AECCMember)
                                    || (memberDetails.CardType == (long)CardType.AECCAndAEVisaMember)
                                    || (memberDetails.CardType == (long)CardType.AEVisaMember))
                                {
                                    _EID = 267076;
                                }
                                else
                                {
                                    _EID = 267003;
                                }
                            }

                            if ((aECC_Cardholder == Convert.ToString((long)CardType.AECCMember))
                                      || (aECC_Cardholder == Convert.ToString((long)CardType.AECCAndAEVisaMember))
                                      || (aECC_Cardholder == Convert.ToString((long)CardType.AEVisaMember)))
                            {       /*Issue an initial Bra and Jean Credit for a aeCC Card holder*/
                                IssueInitialBraPoint(member);
                                IssueInitialJeanPoint(member);
                                long cardType = 0;
                                long.TryParse(aECC_Cardholder, out cardType);
                                memberDetails.CardType = cardType;
                            }
                            memberDetails.PendingCellVerification = 1;
                            LWDataServiceUtil.DataServiceInstance(true).SaveMember(member);
                        }

                        



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
                            if ( ( loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > processDate.Date )
                                    && ( loReward.Expiration == null || loReward.Expiration.Value.Date > processDate.Date )
                                && ( loReward.FPCancellationNumber == string.Empty ) )
                            {
                                countRewards5++;
                            }
                        }

                        foreach ( MemberReward loReward in rewardsB5G1Bra )
                        {
                            if ( ( loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > processDate.Date )
                                    && ( loReward.Expiration == null || loReward.Expiration.Value.Date > processDate.Date )
                                && ( loReward.FPCancellationNumber == string.Empty ) )
                            {
                                countRewardsBra++;
                            }
                        }


                        foreach ( MemberReward loReward in rewardsB5G1Jean )
                        {
                            if ( ( loReward.RedemptionDate == null || loReward.RedemptionDate.Value.Date > processDate.Date )
                                    && ( loReward.Expiration == null || loReward.Expiration.Value.Date > processDate.Date )
                                    && ( loReward.FPCancellationNumber == string.Empty || loReward.FPCancellationNumber == null ) )
                            {
                                countRewardsJean++;
                            }
                        }

                        FlatFileLine loTmpLine  = new FlatFileLine();
                        FlatFileLine fileLine = loTmpLine;

                        fileLine.CUSTOMER_NBR = vc.LinkKey.ToString();
                        fileLine.LOYALTY_NUMBER = vc.LoyaltyIdNumber;
                        fileLine.email = memberDetails.EmailAddress;
                        fileLine.MOBILE_NUMBER = memberDetails.MobilePhone;
                        fileLine.FNAME = member.FirstName;
                        fileLine.LNAME = member.LastName;
                        fileLine.ADDRESS1 = memberDetails.AddressLineOne;
                        fileLine.ADDRESS2 = memberDetails.AddressLineTwo;
                        fileLine.CITY = memberDetails.City;
                        fileLine.STATE = memberDetails.StateOrProvince;
                        fileLine.ZIP = memberDetails.ZipOrPostalCode;
                        fileLine.REGION = memberDetails.Country;
                        fileLine.THREE_DIGIT_REWARDS_CODE = null;
                        fileLine.TWENTY_DIGIT_REWARD_CODE = null;
                        fileLine.REWARD_TYPE = null;
                        fileLine.REWARD_EXP_DATE = null;
                        fileLine.eid = _EID.ToString();
                        fileLine.CAMPAIGN_ID = "2000";
                        fileLine.LANGUAGE_PREFERENCE = (memberDetails.LanguagePreference == null) ? "0" : memberDetails.LanguagePreference;
                        fileLine.GENDER = (memberDetails.Gender == null) ? "0" :  memberDetails.Gender;
                        DateTime birthday = member.BirthDate ?? DateTime.Now;                     
                        fileLine.BIRTHDATE = birthday.ToString("dd-MMM-yyyy");
                        fileLine.STORE_LOYALTY = memberDetails.HomeStoreID.ToString();
                        switch (tierName)
                        {
                            case "Blue":
                                fileLine.TIER_STATUS = "1";
                                break;
                            case "Silver":
                                fileLine.TIER_STATUS = "2";
                                break;
                            case "Gold":
                                fileLine.TIER_STATUS = "3";
                                break;
                            case "Select":
                                fileLine.TIER_STATUS = "4";
                                break;
                        }

                        fileLine.POINTS_BALANCE = balances == null ? "0" : balances.TotalPoints.ToString();
                        fileLine.POINTS_NEEDED_FOR_NEXT_REWARD = balances == null ? "0" : balances.PointsToNextReward.ToString();
                        fileLine.NUMBER_OF_BRAS_PURCHASED = balances == null ? "0" : balances == null ? "0" : balances.BraCurrentPurchased.ToString();
                        fileLine.CREDITS_TO_NEXT_FREE_BRA = balances == null ? "0" : (balances.BraCurrentPurchased - 5).ToString();
                        fileLine.NUMBER_OF_JEANS_PURCHASED = balances == null ? "0" : balances.JeansRollingBalance.ToString();;
                        fileLine.CREDITS_TO_NEXT_FREE_JEAN = balances == null ? "0" : (balances.BraCurrentPurchased - 5).ToString();
                        fileLine.NUMBER_OF_ACTIVE_5_OFF_REWARD = countRewards5.ToString();
                        fileLine.NUMBER_OF_ACTIVE_FREE_JEANS_REWARD = countRewardsJean.ToString();
                        fileLine.NUMBER_OF_ACTIVE_FREE_BRA_REWARD = countRewardsBra.ToString();
                        fileLine.COMMUNICATION_ID = null;
                        fileLine.COMM_PLAN_ID = null;
                        fileLine.COLLATERAL_ID = null;
                        fileLine.PACKAGE_ID = null;
                        fileLine.STEP_ID = null;
                        fileLine.MESSAGE_ID = null;
                        fileLine.SEG_ID = null;
                        fileLine.SEG_NM = null;
                        
                        switch (member.MemberStatus)
                            {
                                case MemberStatusEnum.Active :
                                    if (ValidationRequiredFlag == "1")
                                    {
                                        fileLine.AAP_FLAG = "4";
                                    }
                                    else
                                    {
                                        fileLine.AAP_FLAG = "1";
                                    }
                                    break;
                                case MemberStatusEnum.Disabled: fileLine.AAP_FLAG = "2"; break;
                                case MemberStatusEnum.Terminated: fileLine.AAP_FLAG = "3"; break;
                                case MemberStatusEnum.Locked: fileLine.AAP_FLAG = "5"; break;
                            }
                        
                        fileLine.CARD_TYPE = memberDetails.CardType.ToString();
                       
                        fileLine.RUN_ID  = null;
                        fileLine.LEAD_KEY_ID  = null;
                        fileLine.SITE_URL  = null;
                        fileLine.ENABLE_PASSBOOK_PASS  = null;
                        fileLine.TIMESTAMP  = null;

                        fileLine.DELIVERY_CHANNEL = "2";
                        StreamWriter sw = new StreamWriter(mobileFileName, true);
                        sw.Write(fileLine.toFormattedString(FileFormat.Email));

                        sw.Flush();
                        sw.Close();

                        fileLine.DELIVERY_CHANNEL = "1";
                        StreamWriter sw2 = new StreamWriter(emailFileName, true);
                        sw2.Write(fileLine.toFormattedString(FileFormat.Email));

                        sw2.Flush();
                        sw2.Close();

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

        public int Shutdown()
        {
            return 0;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.    
        /// </summary>
        public void Dispose()
        {
        }


        private void CreateHeadersIfNotCreated()
        {
            FileInfo mobileFile = new FileInfo(mobileFileName);
            FileInfo emailFile = new FileInfo(emailFileName);

            if (mobileFile.Length == 0)
            {
                StreamWriter sw = new StreamWriter(mobileFileName, true);

                sw.Write(this.getHeaderline());
                sw.Flush();
                sw.Close();
            }

            if (emailFile.Length == 0)
            {
                StreamWriter sw = new StreamWriter(emailFileName, true);

                sw.Write(this.getHeaderline());
                sw.Flush();
                sw.Close();
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

        private void IssueInitialBraPoint(Member member)
        {
            sPointType = "Bra Points";
            sPointEvent = "Bra AECC Bonus Credit";
             
            if (Utilities.GetPointTypeAndEvent(sPointType, sPointEvent, out pointEvent, out pointType))
            {
                //get the primary virtual card for the current member
                VirtualCard objVCard = member.GetLoyaltyCardByType(Brierley.FrameWork.Common.VirtualCardSearchType.PrimaryCard);
                if (objVCard == null)
                {
                    throw new Exception("Virtual Card is empty");
                }

                Utilities.AddBonusPoints(objVCard, pointType.Name, pointEvent.Name, 1, DateTime.Parse("12/31/2199"));
            }
        
        
        }

        private void IssueInitialJeanPoint(Member member)
        {
            sPointType = "Jean Points";
            sPointEvent = "Jean AECC Bonus Credit";

            if (Utilities.GetPointTypeAndEvent(sPointType, sPointEvent, out pointEvent, out pointType))
            {
                //get the primary virtual card for the current member
                VirtualCard objVCard = member.GetLoyaltyCardByType(Brierley.FrameWork.Common.VirtualCardSearchType.PrimaryCard);
                if (objVCard == null)
                {
                    throw new Exception("Virtual Card is empty");
                }

                Utilities.AddBonusPoints(objVCard, pointType.Name, pointEvent.Name, 1, DateTime.Parse("12/31/2199"));
            }


        }
    }
}
