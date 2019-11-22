using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Text.RegularExpressions;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.OutputProviders
{


    #region "ADDHOCREWARD"
public class AdHocRewards
    {
        public string LoyaltyNumber { get; set; }
        public string Offercode { get; set; } //AUTH_CD
        public string CertificateNumber{ get; set; } //SUC
        public string ExpirationDate { get; set; }
        public string StartDate { get; set; }

        public  string error { get; set; }
        public int documenttype { get; set; }

        public string IssueDate { get; set; }

        public string toFormattedString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(this.LoyaltyNumber);
            s.Append("|");
            s.Append(this.Offercode);
            s.Append("|");
            s.Append(this.CertificateNumber);
            s.Append("|");
            s.Append(this.StartDate);
            s.Append("|");
            s.Append(this.ExpirationDate);
            
            if(this.error!=null)
            {
             if(this.error.Length > 0 )
             {
                s.Append("| error : " + this.error);
               
             }
            }
            
            s.AppendLine();
            return s.ToString();
        }
       

       


    }
   #endregion
    
    public class AEOConnectedAdHocReward : IDAPOutputProvider
    {

        #region "Variables"
        //enum FileType { Error = -1, Birthday = 1, TwentyShop = 2 }; // AEO-2652
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        //private string className = MethodBase.GetCurrentMethod().DeclaringType.Name; // AEO-2652
        //private DateTime processDate = DateTime.Now; // AEO-2652
        //private string filenamecreatedexeption = String.Empty; // AEO-2652
        //private string filenamecreatedreward = String.Empty; // AEO-2652

        private string path = string.Empty;
        //SERVICE TO ACCESS TO DATABASE
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        //Dictionary<string, AdHocRewards> RewardFails =new Dictionary<string, AdHocRewards>(); // AEO-2652
        //Dictionary<string, AdHocRewards> Rewardsuccess = new Dictionary<string, AdHocRewards>(); // AEO-2652

        #endregion


        #region "IMPLEMENTATION"

        public void ProcessMessageBatch(List<string> messageBatch)
        {
            //this.RewardFails.Clear(); // AEO-2652
            //this.Rewardsuccess.Clear(); // AEO-2652
            try
            {
                AdHocRewards classFields = new AdHocRewards();
                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    //// Loading XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                //XmlNode xmlNode = doc.SelectSingleNode("Rewards/Reward"); // AEO-2652
                var xmlNodeList = doc.SelectNodes("Rewards/Reward"); // AEO-2652           
                foreach (XmlNode xmlNode in xmlNodeList) // AEO-2652
                {
                    AdHocRewards Fields = new AdHocRewards();

                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        switch (node.Name.ToUpper())
                        {

                            case "LOYALTYNUMBER":
                                Fields.LoyaltyNumber = node.InnerText.Trim();
                                break;
                            case "OFFERCODE": //(AUTH_CODE)
                                Fields.Offercode = node.InnerText.Trim();
                                break;
                            case "CERTIFICATENUMBER": //(SUC)
                                Fields.CertificateNumber = node.InnerText.Trim();
                                break;
                            case "STARTDATE":
                                Fields.StartDate = node.InnerText.Trim();
                                break;
                            case "EXPIRATIONDATE":
                                Fields.ExpirationDate = node.InnerText.Trim();
                                break;
                            case "DOCUMENTTYPE":
                                int numValue;
                                bool parsed = Int32.TryParse(node.InnerText.Trim(), out numValue);
                                Fields.documenttype = numValue;
                                break;
                            case "ISSUEDATE":
                                Fields.IssueDate = node.InnerText.Trim();
                                break;

                            default:
                                break;
                        }

                    }
                    if (!String.IsNullOrWhiteSpace(Fields.LoyaltyNumber)
                        && !String.IsNullOrWhiteSpace(Fields.Offercode)
                        && !String.IsNullOrWhiteSpace(Fields.CertificateNumber)
                        && !String.IsNullOrWhiteSpace(Fields.ExpirationDate)
                        && !String.IsNullOrWhiteSpace(Fields.StartDate)
                        && !String.IsNullOrWhiteSpace(Fields.IssueDate)
                        )
                    {
                        try
                        {
                            using (var contServices = _dataUtil.ContentServiceInstance())
                            {
                                LWQueryBatchInfo batchInfo = new LWQueryBatchInfo() { BatchSize = 1, StartIndex = 0 };
                                IList<RewardDef> RewardMemembers = contServices.GetRewardDefsByCertificateType(Fields.Offercode);
                                if (RewardMemembers != null)
                                {
                                    if (RewardMemembers.Count > 0)
                                    {
                                        try
                                        {
                                            using (var lwServices = _dataUtil.LoyaltyDataServiceInstance())
                                            {
                                                RewardDef memberr = RewardMemembers.FirstOrDefault();
                                                Member memberv = lwServices.LoadMemberFromLoyaltyID(Fields.LoyaltyNumber);
                                                if (memberv != null)

                                                {


                                                    //CREATE THE REWARD 
                                                    MemberReward newReward = new MemberReward();
                                                    newReward.RewardDef = memberr;
                                                    newReward.RewardDefId = memberr.Id;
                                                    newReward.ProductId = memberr.ProductId;
                                                    DateTime tmpexpiration = DateTime.MinValue;
                                                    DateTime tmpstartdate = DateTime.MinValue;
                                                    DateTime tmpissuedate = DateTime.MinValue;

                                                    bool saved = true;

                                                    //VALIDATE THE STARTDATE
                                                    bool formatstardate = DateTime.TryParse(Fields.StartDate, out tmpstartdate);
                                                    if (formatstardate)
                                                    {

                                                        newReward.FulfillmentDate = tmpstartdate;
                                                    }
                                                    bool formatdateexp = DateTime.TryParse(Fields.ExpirationDate, out tmpexpiration);
                                                    if (formatdateexp)
                                                    {
                                                        //add 11:59:59 PM TO THE DATE
                                                        var dt = new DateTime(tmpexpiration.Year, tmpexpiration.Month, tmpexpiration.Day, 23, 59, 59);
                                                        newReward.Expiration = dt;
                                                    }


                                                    /*SET MEMBERID*/
                                                    newReward.MemberId = memberv.IpCode;
                                                    newReward.OfferCode = Fields.Offercode;
                                                    newReward.CertificateNmbr = Fields.CertificateNumber;

                                                    bool formatissue = DateTime.TryParse(Fields.IssueDate, out tmpissuedate);
                                                    var issdate = new DateTime(tmpissuedate.Year, tmpissuedate.Month, tmpissuedate.Day, 00, 00, 00);
                                                    newReward.DateIssued = issdate;
                                                    //CREATE THE REWARD
                                                    if (saved)
                                                    {

                                                        try
                                                        {
                                                            lwServices.CreateMemberReward(newReward);
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error While Creating The Member Reward  : " + e.Message);
                                                        }
                                                    }
                                                }
                                                else
                                                {

                                                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "  MEMBER LOYALTY NUMBER NOT FOUND " + Fields.LoyaltyNumber);

                                                }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "  ERROR " + e.Message);
                                        }

                                    }
                                    else
                                    {
                                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "  ERROR REWARD DOES NOT EXIST " + Fields.Offercode);

                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            //WRITE NOT REWARDDEF NOT FOUND
                            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "  ERROR " + e.Message);
                        }
                    }
                    else
                    {
                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, " ALL FIELDS ARE REQUIRED ");
                    }
                }
            }
            catch(Exception e)
            {
                this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error" + e.Message);
            }
          
        }

        public void Initialize(NameValueCollection globals, NameValueCollection args, long jobId, DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil)
        {
            //this.filenamecreatedexeption = String.Empty; // AEO-2652
            //this.filenamecreatedreward = String.Empty; // AEO-2652
        }
        public void Dispose()
        {

            //throw new NotImplementedException();
        }
        public int Shutdown()
        {
            // throw new NotImplementedException();
            return 0;
           

        }

        #endregion



       





    }
}
