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
	
    public class AEOConnectedRewardCertIn : IDAPOutputProvider
    {
        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// Stores Variable LoyaltyNumber
        /// </summary>
        private string strLoyaltyNumber = string.Empty;


        //private string inputFileName = "AEO_In_Type_YYYYMMDD.txt";
        private DateTime processDate = DateTime.Now;
        //private long rowsWrittenCell = 0;
        //private long rowsWrittenEmail = 0;


        public void Initialize ( NameValueCollection globals, NameValueCollection args, long jobId, DAPDirectives config, NameValueCollection parameters, DAPPerformanceCounterUtil performUtil  )
        {   
        }


        public void ProcessMessageBatch ( List<string> messageBatch )
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            long memberRewardId = 0L;
            string auth_cd = null;
            string suc_cd = null;




            
            DateTime Campaign_Exp_Date=DateTime.MinValue;
            DateTime fulfillmentDate=DateTime.MinValue;

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            try
            {
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    doc.LoadXml(str);
                }

                XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/Global");
               

               

                if (xmlNode == null)
                {
                    this.logger.Error(this.className, methodName, "Xml node not found");
                    this.logger.Trace(this.className, methodName, "Ends");
                    return;
                }

                // Get attributes from xml
               
                memberRewardId = long.Parse(xmlNode.Attributes["MemberRewardID"].Value.Trim());
                //VALIDATE THE COLUMN AuthCode AEO-2237
                auth_cd = xmlNode.SelectSingleNode("@AuthCode").Value.Trim() ?? String.Empty;
                if (String.IsNullOrWhiteSpace(auth_cd))
                {
                    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error, authcode not valid  in  MembeRewardId :  " + memberRewardId );
                    return;
                }
                //VALIDATE THE COLUMN SucCode AEO-2237
                suc_cd = xmlNode.SelectSingleNode("@SucCode").Value.Trim() ?? String.Empty;
                if (String.IsNullOrWhiteSpace(suc_cd))
                {
                    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error, suc.code not valid  in  MembeRewardId :  " + memberRewardId);
                    return;
                }
                //VALIDATE THE COLUMN CampaignExpirationDate AEO-2237
                string valCampaignExpirationDate  = xmlNode.SelectSingleNode("@CampaignExpirationDate").Value.Trim() ?? String.Empty;
                if (!String.IsNullOrWhiteSpace(valCampaignExpirationDate))
                {
                    bool succesCampaignExpirationDate = DateTime.TryParse(xmlNode.Attributes["CampaignExpirationDate"].Value.Trim(), out Campaign_Exp_Date);
                    if (!succesCampaignExpirationDate)
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error, CampaignExpirationDate  format date not valid in MembeRewardId :  " + memberRewardId);
                        return;
                    }
                }
                else
                {
                    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error, CampaignExpirationDate  value  not valid in MembeRewardId :  " + memberRewardId);
                    return;
                }

                //AEO-2076 -- VALIDATION NULL fulfillmentDate
                //SELECT  all the title elements that have an attribute named AEO-2076
                //Try to get de column StartDate from XML node , if fails then  the fulfillmentDate takes value Datetime function
                string valStartDate = xmlNode.SelectSingleNode("@StartDate").Value.Trim() ?? String.Empty;
                if (!String.IsNullOrWhiteSpace(valStartDate))
                {
                    bool successtartDate = DateTime.TryParse(xmlNode.Attributes["StartDate"].Value.Trim(), out fulfillmentDate);
                    if (!successtartDate)
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error, StartDate  format date not valid in MembeRewardId :  " + memberRewardId);
                        return;
                    }

                }else
                {
                    this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error, StartDate  value  not valid in MembeRewardId :  " + memberRewardId);
                    return;
                }
                // Load the member reward and current member
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    MemberReward memberReward = null;
                    try
                    {
                        memberReward = lwService.GetMemberReward(memberRewardId);
                    }
                    catch (Exception)
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error, MembeRewardId " + memberRewardId + " not found"); memberReward = null;
                    }
                    if (memberReward != null)
                    {

                        Member member = lwService.LoadMemberFromIPCode(memberReward.MemberId);

                        // Load attributes to member reward
                        if (member != null)
                        {


                            memberReward.CertificateNmbr = suc_cd;
                            memberReward.OfferCode = auth_cd;
                            memberReward.Expiration = new DateTime(Campaign_Exp_Date.Year, Campaign_Exp_Date.Month, Campaign_Exp_Date.Day, 23, 59, 59); //AEO-2597
                            memberReward.FulfillmentDate = fulfillmentDate;
                            // Save member
                            lwService.SaveMember(member);
                            lwService.UpdateMemberReward(memberReward);
                        }
                        else
                        {
                            this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: Member with memberRewardId " + memberRewardId + " not found ");
                        }

                    }
                    else
                    {
                        this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: MemberRewardId with id " + memberRewardId + " not found ");
                    }

                    // Logging for ending of method
                    this.logger.Trace(this.className, methodName, "Ends");
                }
            }
            catch ( Exception ex )
            {
                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Error: " + ex.Message);
             throw new Exception(ex.Message);
            }

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");

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