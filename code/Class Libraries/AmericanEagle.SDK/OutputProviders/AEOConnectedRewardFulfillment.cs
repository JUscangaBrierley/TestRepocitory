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
using Brierley.Clients.AmericanEagle.DataModel;
using System.Collections.Specialized;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.OutputProviders
{

    class CertsOutFile
    {
        public string MEMBER_REWARD_ID { get; set; }
        public string ISSUE_DATE { get; set; }
        public string CUST_NUMBER { get; set; }
        public string REWARD_TYPE { get; set; }
        public string LOYALTY_NUMBER { get; set; }

        public string toFormattedString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(this.MEMBER_REWARD_ID);
            s.Append("|");
            s.Append(this.CUST_NUMBER);
            s.Append("|");
            s.Append(this.LOYALTY_NUMBER);
            s.Append("|");
            s.Append(this.REWARD_TYPE);
            s.Append("|");
            s.Append(this.ISSUE_DATE);
            s.AppendLine();

            return s.ToString();
        }
    }


    public class AEOConnectedRewardFulfillment : IDAPOutputProvider
    {
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private const string categoryName = "AEO Connected Rewards"; // AEO-1441

        //private string emailFileName = "AEO_Out_Rewards_YYYYMMDD.txt";
        private string path = string.Empty;
        private DateTime processDate = DateTime.Now;
        public const string FieldSeparator = "|";
        private long rowsWrittenEmail = 0;

        //RKG - changed to call custom rule to only reward certificates to members with points outside of 2 week hold period.
        private string ruleName = "AEO Connected $@ Reward";

        private const string extraAccessRuleName = "AEO Connected Extra Access $@ Reward";
        private const string braRuleName = "B5G1 Bra Reward";
        private const string jeanRuleName = "B5G1 Jean Reward";

        private Dictionary<String, int> rewardsFullAccess = new Dictionary<string, int>();
        private Dictionary<String, int> rewardsExtraAccess = new Dictionary<string, int>();
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;



        private int getRewardType(string rewardName)
        {
            int rewardType = 1;

            if (rewardName.Equals(braRuleName))
                return 9;
            if (rewardName.Equals(jeanRuleName))
                return 10;

            for (int i = 10; i <= 60; i += 5)
            {
                if (i % 10 == 5 && i % 15 != 0)
                    continue;

                if (rewardName.Contains(string.Format("${0}", i)))
                {
                    return rewardType;
                }
                rewardType++;
            }

            return -1;
        }

        private void processMemberIds(List<long> memberRewardIDs, List<string> rewardNames, Member member)
        {
            if (memberRewardIDs.Count == 0)
            {
                return;
            }

            int rewardType;

            //   StreamWriter sw = new StreamWriter(emailFileName, true);
            //   CertsOutFile fileLine = new CertsOutFile();

            // AEO-2114
            /*
           IList<IClientDataObject> loDetails = member.GetChildAttributeSets("MemberDetails");
           MemberDetails details = (loDetails == null || loDetails.Count == 0 ? null : loDetails[0]) as MemberDetails;
           */
            // AEO-2114
            // customer number & loyalty number

            VirtualCard lCard = null;
            //MemberReward memberReward = null;

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

            int i = 0; // AEO-2114
            foreach (long memberRewardId in memberRewardIDs)
            {
                //memberReward = LWDataServiceUtil.DataServiceInstance(true).GetMemberReward(memberRewardId); AEO-2114
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                    MethodBase.GetCurrentMethod().Name,
                    "Member Reward = " + memberRewardId);

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                    MethodBase.GetCurrentMethod().Name,
                    "Member Reward is " + memberRewardId == null ? "null" : "not null");

                // collect data before writing to the output file.

                // rewardType = getRewardType(memberReward.RewardDef.Name.Trim());

                rewardType = getRewardType(rewardNames[i]); // AEO-2114
                if (rewardType == -1)
                {
                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                        MethodBase.GetCurrentMethod().Name,
                        "Reward for member " + member.IpCode + " is not valid for: " +
                        rewardNames[i]);
                    continue; // Go on with the loop but log the bad reward
                }
                /*    fileLine.MEMBER_REWARD_ID = memberRewardId.ToString();
                    fileLine.CUST_NUMBER = lCard.LinkKey.ToString();
                    fileLine.LOYALTY_NUMBER = lCard.LoyaltyIdNumber == null ? "" 
                                                            : lCard.LoyaltyIdNumber;
                    fileLine.REWARD_TYPE = rewardType.ToString();
                    fileLine.ISSUE_DATE = String.Format("{0:dd-MMM-yyyy}", 
                                            this.processDate.ToString("yyyyMMdd"));
                    sw.Write(fileLine.toFormattedString());
                    sw.Flush();*/
                rowsWrittenEmail++;
                i++; // AEO-2114
            }
            //sw.Close();
        }

        public void Initialize(NameValueCollection globals, NameValueCollection args, long jobId, DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

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

            if (null != ConfigurationManager.AppSettings["ProcessDate"])
            {
                string strProcessDate = ConfigurationManager.AppSettings["ProcessDate"];
                DateTime.TryParse(strProcessDate, out processDate);
            }
            /*
              emailFileName =  path + emailFileName.Replace("YYYYMMDD",this.processDate.ToString("yyyyMMdd"));

              string[] files = { emailFileName };

              foreach ( string filestr in files ) {
                  StreamWriter sw = new StreamWriter(filestr, true);

                  // we need to write the header
                  sw.Write(this.getHeaderline());
                  sw.Flush();
                  sw.Close();
              }
              */
            using (var lwService = _dataUtil.ContentServiceInstance())
            {
                Category tmpCatgerory = lwService.GetCategory(0,
                    AEOConnectedRewardFulfillment.categoryName);

                if (tmpCatgerory == null)
                {
                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward Category not defined.");
                    throw new ApplicationException("Reward Category not defined");
                }

                IList<RewardDef> rewardsDef = lwService.GetRewardDefsByCategory(tmpCatgerory.ID);


                rewardsDef = rewardsDef.OrderByDescending(o => o.HowManyPointsToEarn).ToList();

                if ((rewardsDef == null) || (rewardsDef.Count == 0))
                {
                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "There are not reward definitions for category " + tmpCatgerory.Name);
                    throw new ApplicationException("There are not reward definitions for category " + tmpCatgerory.Name);
                }

                IList<RewardDef> rewardsDeff = new List<RewardDef>();

                foreach (RewardDef rew in rewardsDef)
                {
                    if (rew.Name != "$10 AEO Pilot Transition Reward")
                    {
                        rewardsDeff.Add(rew);
                    }
                }

                rewardsDef = rewardsDeff;


                foreach (RewardDef rew in rewardsDef)
                {
                    if (rew.Name.Contains("Extra Access"))
                    {
                        this.rewardsExtraAccess.Add(rew.Name, rew != null ? (int)rew.HowManyPointsToEarn : 0);
                    }
                    else
                    {
                        this.rewardsFullAccess.Add(rew.Name, rew != null ? (int)rew.HowManyPointsToEarn : 0);
                    }

                }

            }


            return;
        }


        public void ProcessMessageBatch(List<string> messageBatch)
        {
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            //long ipCode = 0; // AEO-2653
            //string rowType = null; /* Need to see if we want a default rowType */ // AEO-2653
            //int number = 60; // AEO-2653
            //int incrementNumber = 0; // AEO-2653
            //int rowPoints = int.MinValue; // AEO-2114 // AEO-2653
            //string tierlevel = string.Empty; // AEO-2114 // AEO-2653
            //Boolean isExtraAccess = false; // AEO-2114 // AEO-2653
            //Boolean isFullAccess = false; // AEO-2114 // AEO-2653
            //Boolean issueReward = false; // AEO-2114 // AEO-2653
            //string tmpRulename; // AEO-2653


            List<long> memberRewardIDs = new List<long>();
            List<string> memberRewardNames = new List<string>(); //AEO-2114

            try
            {

                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    //// Loading XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                //XmlNode xmlNode = doc.SelectSingleNode("Rewards/Reward"); // AEO-2653
                // AEO-2653 Begin
                var xmlNodeList = doc.SelectNodes("Rewards/Reward");
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    long ipCode = 0;
                    string rowType = null; /* Need to see if we want a default rowType */
                    int number = 60;
                    int incrementNumber = 0;
                    int rowPoints = int.MinValue; // AEO-2114
                    string tierlevel = string.Empty; // AEO-2114
                    Boolean isExtraAccess = false; // AEO-2114
                    Boolean isFullAccess = false; // AEO-2114
                    Boolean issueReward = false; // AEO-2114
                    string tmpRulename = null;
                    // AEO-2653 End
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        switch (node.Name.ToUpper())
                        {

                            case "IPCODE":
                                ipCode = long.Parse(node.InnerText);
                                break;
                            case "REWARDTYPE":
                                rowType = node.InnerText;
                                break;

                            // AEO-2114 begin
                            case "POINTS":
                                rowPoints = int.Parse(node.InnerText);
                                break;
                            case "TIERLEVEL":
                                tierlevel = node.InnerText.Trim();
                                break;
                            // aeo-2114 end

                            case "CARDTYPE":
                                break;


                            default:
                                ipCode = 0;
                                break;
                        }
                    }
                    try // AEO-2716 begin
                    {
                        if (xmlNode != null && ipCode > 0)
                        {
                            //We have a valid IPCode so get the member 
                            Member member;
                            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                            {
                                member = lwService.LoadMemberFromIPCode(ipCode); // AEO-2114
                            }
                            if (member == null)
                            {
                                // Log error when member not found
                                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().Name,
                                    "Member Not Found for IPCODE - " + xmlNode.Attributes["IPCODE"].Value);
                            }
                            else
                            {
                                //Calculate points

                                if (rowType == "B" || rowType == "J")
                                {
                                    // issue as many rewards as possible
                                    tmpRulename = rowType == "B" ? braRuleName : jeanRuleName;
                                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().Name,
                                        "Executing rule: " + tmpRulename);

                                    ExecuteRule(member, out memberRewardIDs, out memberRewardNames, tmpRulename);

                                    // Write to file
                                    processMemberIds(memberRewardIDs, memberRewardNames, member);
                                    continue;
                                    //return;
                                }

                                // AEO-2114 begin
                                if (tierlevel == "Extra Access")
                                {
                                    incrementNumber = 15;
                                    ruleName = "AEO Connected Extra Access $@ Reward";
                                    isExtraAccess = true;
                                    isFullAccess = false;
                                }
                                else if (tierlevel == "Full Access")
                                {
                                    incrementNumber = 10;
                                    ruleName = "AEO Connected $@  Reward";
                                    isExtraAccess = false;
                                    isFullAccess = true;
                                }
                                // AEO-2114 end
                                else
                                {
                                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                                        MethodBase.GetCurrentMethod().Name,
                                        "Member " + member.IpCode + " has no tier");
                                    // return;
                                    continue;
                                }

                                int lower = isExtraAccess ? 15 : 10;

                                while (number >= lower)
                                {


                                    // get the rule name
                                    tmpRulename = ruleName.Replace("@", number.ToString());


                                    // AEO-2114
                                    int pointsneeded = int.MaxValue;

                                    if (isExtraAccess)
                                    {
                                        this.rewardsExtraAccess.TryGetValue(tmpRulename, out pointsneeded);
                                    }
                                    else
                                    {
                                        this.rewardsFullAccess.TryGetValue(tmpRulename, out pointsneeded);
                                    }

                                    if (rowPoints >= pointsneeded)
                                    {

                                        //We have a valid member, so execute the IssueReward rule and then expire all the Returns that were part of a consumption.  

                                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                                            MethodBase.GetCurrentMethod().Name,
                                            "Executing rule: " + tmpRulename);

                                        ExecuteRule(member, out memberRewardIDs, out memberRewardNames, tmpRulename);

                                        // Write to file
                                        processMemberIds(memberRewardIDs, memberRewardNames, member);

                                        foreach (long x in memberRewardIDs)
                                        {
                                            rowPoints -= pointsneeded;
                                        }
                                    }

                                    // AEO-2114

                                    // try  to issue the next stacked reward
                                    number -= incrementNumber;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().Name,
                                    "Error:" + ex.Message);
                    }
                    // AEO-2716 end
                }
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().Name,
                                    "End");
            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,
                    MethodBase.GetCurrentMethod().Name,
                    "Error: " + ex.Message);

                // throw new Exception(ex.Message);
            }
        }

        public void ExecuteRule(Member member, out List<long> memberRewardIds, out List<string> rewardnames, string RewardName)
        {
            memberRewardIds = new List<long>();
            rewardnames = new List<string>(); // AEO-2114
            try
            {
                // VirtualCard v = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard); AEO-2114

                Dictionary<string, string> additionalFields = new Dictionary<string, string>();
                additionalFields.Add("RewardType", RewardName); // pass the reward type
                additionalFields.Add("TypeCode", "");

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                    MethodBase.GetCurrentMethod().Name,
                    "Member: " + member.IpCode);

                //Create a context object and assign the member and MemberDetails attribute set
                //to be used in invoking the IssueReward for this reward and the AwardPoints 
                //rule associated with the View Earned Reward 

                ContextObject cobj = new ContextObject();
                cobj.Owner = member;
                cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];
                cobj.Environment = additionalFields.ToDictionary(pair => pair.Key, pair => (object)pair.Value);

                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    RuleTrigger ruleTrigger = lwService.GetRuleByName(RewardName); //AEO-2114


                    if (ruleTrigger == null)
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,
                            MethodBase.GetCurrentMethod().Name,
                            "Rule: (" + RewardName + ") Not Defined");

                        throw new Exception(RewardName + " Rule Not Defined",
                            new Exception(RewardName + " Rule Not Defined"));
                    }
                    else
                    {
                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                            MethodBase.GetCurrentMethod().Name,
                            "Rule: (" + ruleTrigger.RuleName + ")");
                    }


                    lwService.Execute(ruleTrigger, cobj); // AEO-2114
                }
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                    MethodBase.GetCurrentMethod().Name,
                    "Rule executed");

                foreach (ContextObject.RuleResult result in cobj.Results)
                {
                    if (result.Detail.Contains("id="))
                    {
                        result.Detail = result.Detail.Substring(3);
                    }
                    long rewardID = long.Parse(result.Detail);
                    memberRewardIds.Add(rewardID);
                    rewardnames.Add(RewardName); // AEO-2114

                }

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name,
                    MethodBase.GetCurrentMethod().Name,
                    "number of rewards issued =" + memberRewardIds.Count.ToString());

            }
            catch (Exception ex)
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,
                    MethodBase.GetCurrentMethod().Name,
                    "Error: " + ex.Message);

                // AEO-2716 begin
                // throw new Exception(ex.Message, new Exception("System Error")); 
                // AEO-2716 end
            }
        }

        private string getHeaderline()
        {
            StringBuilder tmp = new StringBuilder();

            tmp.Append("MemberRewardID|Customer_NBR|Loyalty_Number|RewardType|IssueDate");
            tmp.AppendLine();

            return tmp.ToString();
        }

        public int Shutdown()
        {
            return 0;
        }

        public void Dispose()
        {

            return;
        }


    }


}