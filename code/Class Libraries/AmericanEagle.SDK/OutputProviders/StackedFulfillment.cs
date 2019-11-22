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
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.OutputProviders
{
    public class StackedFulfillment : IDAPOutputProvider
    {
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);


        private string emailFileName = "AEORW_EM_BACKLOG_YYYYMMDDHHMMSS.txt";
        private string path = string.Empty;
        private DateTime processDate = DateTime.Now;
        public const string FieldSeparator = "|";
        private long rowsWrittenEmail = 0;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        //RKG - changed to call custom rule to only reward certificates to members with points outside of 2 week hold period.
        private const string ruleName = "AEO Rewards $@ Stacked Reward";

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


          
            emailFileName =  path + emailFileName.Replace("YYYYMMDDHHMMSS",this.processDate.ToString("yyyyMMddHHmmss"));

            string[] files = { emailFileName };

            foreach ( string filestr in files ) {
                StreamWriter sw = new StreamWriter(filestr, true);
                              
               // we need to write the header
                sw.Write(this.getHeaderline());
                sw.Flush();
                sw.Close();
            }
         
            return;
        }


        public void ProcessMessageBatch ( List<string> messageBatch )
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
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            //We have a valid IPCode so get the member 
                            Member member = lwService.LoadMemberFromIPCode(ipCode);
                            if (member == null)
                            {
                                // Log error when member not found
                                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Not Found for IPCODE - " + xmlNode.Attributes["IPCODE"].Value);

                            }
                            else
                            {
                                //We have a valid member, so execute the IssueReward rule and then expire all the Returns that were part of a consumption.  
                                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Executing rule: " + ruleName);
                                //Calculate points

                                int number = 100;

                                while (number >= 5)
                                {

                                    // get the rule name
                                    string tmpRulename = ruleName.Replace("@", number.ToString());

                                    // issue as many rewards as possible
                                    ExecuteRule(member, out memberRewardIDs, tmpRulename, "Stacked" + number.ToString());

                                    // write to the file
                                    foreach (long memberRewardId in memberRewardIDs)
                                    {
                                        MemberReward memberReward = lwService.GetMemberReward(memberRewardId);
                                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Reward = " + memberReward.Id);
                                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member Reward is " + memberReward == null ? "null" : "not null");
                                        // collect data before writing to the output file.

                                        StackedFlatFileLine fileLine = new StackedFlatFileLine();
                                        IList<IClientDataObject> loDetails = member.GetChildAttributeSets("MemberDetails");
                                        MemberDetails details = (loDetails == null || loDetails.Count == 0 ? null : loDetails[0]) as MemberDetails;
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
                                            fileLine.email = details == null || details.EmailAddress == null ? "" : details.EmailAddress;
                                            fileLine.CUSTOMER_NBR = lCard.LinkKey.ToString();
                                            fileLine.LOYALTY_NUMBER = lCard.LoyaltyIdNumber == null ? "" : lCard.LoyaltyIdNumber;
                                            fileLine.REWARD_EXP_DATE = memberReward.Expiration == null ? "" : String.Format("{0:dd-MMM-yyyy}", memberReward.Expiration);
                                            fileLine.THREE_DIGIT_REWARDS_CODE = memberReward.OfferCode;
                                            fileLine.TWENTY_DIGIT_REWARD_CODE = memberReward.CertificateNmbr;
                                            using (var dtService = _dataUtil.DataServiceInstance())
                                            {
                                                fileLine.eid = dtService.GetClientConfiguration("EID_StackedReward").Value;
                                            }
                                            fileLine.OfferValue = memberReward.RewardDef.Name.Substring(13, 3).Replace("S", "");
                                            StreamWriter sw = new StreamWriter(emailFileName, true);
                                            sw.Write(fileLine.toFormattedString());
                                            sw.Flush();
                                            sw.Close();
                                            rowsWrittenEmail++;
                                        }

                                    }

                                    // try  to issue the next stacked reward
                                    number -= 5;
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

        public void ExecuteRule ( Member member, out List<long> memberRewardIds, string RewardName , string typeCode)
        {
            try
            {
                Dictionary<string, string> additionalFields = new Dictionary<string, string>();
                additionalFields.Add("TypeCode", typeCode); //keep using 5DDM
                additionalFields.Add("RewardType", RewardName); // pass the reward type

                memberRewardIds = new List<long>();

                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member: " + member.IpCode);

                //Create a context object and assign the member and MemberDetails attribute set
                //to be used in invoking the IssueReward for this reward and the AwardPoints 
                //rule associated with the View Earned Reward 


                ContextObject cobj = new ContextObject();
                cobj.Owner = member;
                cobj.InvokingRow = member.GetChildAttributeSets("MemberDetails")[0];
                cobj.Environment = additionalFields.ToDictionary(pair => pair.Key, pair => (object)pair.Value);

                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    RuleTrigger ruleTrigger = lwService.GetRuleByName(RewardName);

                    if (ruleTrigger == null)
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Rule: (" + RewardName + ") Not Defined");
                        throw new Exception(RewardName + " Rule Not Defined", new Exception(RewardName + " Rule Not Defined"));
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

        private string getHeaderline()
        {
            StringBuilder tmp = new StringBuilder();

            tmp.Append("LoyaltyID|CustomerID|EmailAddress|Auth_CD|SUC_CD|Campaign_ExpDate|EID|OfferValue");           
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