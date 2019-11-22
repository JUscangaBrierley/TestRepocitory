// ----------------------------------------------------------------------------------
// <copyright file="SMSErrorLog.cs" company="Brierley and Partners">
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
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.DataAcquisition.Core;
    using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
    using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;
    using Brierley.Clients.AmericanEagle.DataModel;

    /// <summary>
    /// Class VendorEmailEventError
    /// </summary>
    public class SMSErrorLog : IDAPOutputProvider
    {
        /// <summary>
        /// Description is used for point event
        /// </summary>
        private string description = string.Empty;

        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        // private string strLoyaltyNumber = string.Empty;
        /// <summary>
        /// Stores the LoyaltyNumber we are going to find
        /// </summary>
        private string strLoyaltyNumber = string.Empty;

        //private string strShortCode = string.Empty;
        /// <summary>
        /// Stores the ShortCode that is to be verified against
        /// </summary>
        private string strShortCode = string.Empty;

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);

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
        }

        /// <summary>
        /// This method is called to process the messages in the batch
        /// </summary>
        /// <param name="messageBatch">String List</param>
        public void ProcessMessageBatch(IList<string> messageBatch)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
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

                // check for valid xml data
                if (null != xmlNode)
                {
                    //Vendor Inputs-Loyalty number and new email address are captured
                    strLoyaltyNumber = xmlNode.Attributes["LoyaltyIdNumber"].Value.Trim();
                    strShortCode = (xmlNode.Attributes["ShortCode"].Value).Trim().ToLower();

                    if (!string.IsNullOrEmpty(strLoyaltyNumber))
                    {
                        // Get member
                        Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(strLoyaltyNumber);

                        if (member == null)
                        {
                            // Log error when member not found
                            this.logger.Error(this.className, methodName, "Member Not Found for Loyalty Number - " + xmlNode.Attributes["LoyaltyIdNumber"].Value);
                        }

                        IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
                        MemberDetails memberDetails = (MemberDetails)mbrDtlObjs[0];

                        if (memberDetails != null)
                        {
                            ClientConfiguration objClientConfiguration = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("CampaignID_SMSReward");
                            string _strSMSReward = Convert.ToString(objClientConfiguration.Value);

                            ClientConfiguration objClientConfiguration2 = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("CampaignID_SMSBraReward");
                            string _strBraReward = Convert.ToString(objClientConfiguration2.Value);

                            ClientConfiguration objClientConfiguration3 = LWDataServiceUtil.DataServiceInstance(true).GetClientConfiguration("CampaignID_SMSJeanReward");
                            string _strJeansReward = Convert.ToString(objClientConfiguration3.Value);

                            memberDetails.PendingCellVerification = 1;

                            RewardDef _reward_bra = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("B5G1 Bra Reward");
                            RewardDef _reward_jean = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("B5G1 Jean Reward");
                            RewardDef _reward_5 = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef("AEO Rewards $5 Reward");
                            Boolean _bra_reward = false;
                            Boolean _jean_reward = false;
                            Boolean _5_reward = false;

                            //Check to see if they ever received the reward first
                            IList<MemberReward> memberRewards = LWDataServiceUtil.DataServiceInstance(true).GetMemberRewards(member, null);
                            foreach (MemberReward mr in memberRewards)
                            {
                                RewardDef RewardDef = LWDataServiceUtil.DataServiceInstance(true).GetRewardDef(mr.RewardDefId);

                                if (RewardDef.Name == _reward_bra.Name)
                                {
                                    _bra_reward = true;
                                }
                                else if (RewardDef.Name == _reward_jean.Name)
                                {
                                    _jean_reward = true;
                                }
                                else if (RewardDef.Name == _reward_5.Name)
                                {
                                    _5_reward = true;
                                }
                            }

                            VirtualCard virtualCard = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);

                            if ((strShortCode == _strSMSReward) && (memberDetails.PendingEmailVerification != 0))
                            {
                                string _pointType = "AEO Rewards Base Points";
                                string _pointEvent = "Returned Reward Points";

                                if (_5_reward)
                                {
                                    AddPointsBack(_pointType, _pointEvent, member, virtualCard, memberDetails, "1000", methodName);
                                }
                                else
                                {
                                    this.logger.Error(this.className, methodName, "no reward ever claimed for $5.");
                                }
                            }
                            else if ((strShortCode == _strBraReward) && (memberDetails.PendingEmailVerification != 0))
                            {
                                string _pointType = "Bra Points";
                                string _pointEvent = "Returned Bra Reward Points";

                                if (_bra_reward)
                                {
                                    AddPointsBack(_pointType, _pointEvent, member, virtualCard, memberDetails, "5", methodName);
                                }
                                else
                                {
                                    this.logger.Error(this.className, methodName, "no reward ever claimed for bra's.");
                                }
                            }
                            else if ((strShortCode == _strJeansReward) && (memberDetails.PendingEmailVerification != 0))
                            {
                                string _pointType = "Jean Points";
                                string _pointEvent = "Returned Jean Reward Points";

                                if (_jean_reward)
                                {
                                    AddPointsBack(_pointType, _pointEvent, member, virtualCard, memberDetails, "5", methodName);
                                }
                                else
                                {
                                    this.logger.Error(this.className, methodName, "no reward ever claimed for jeans.");
                                }
                            }
                            else
                            {
                                this.logger.Error(this.className, methodName, "No reward to process.");
                            }

                            // Save member Information to Database
                            LWDataServiceUtil.DataServiceInstance(true).SaveMember(member);
                        }
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
            //  throw new System.NotImplementedException();
        }

        public void AddPointsBack(String _pointType, string _pointEvent, Member member, VirtualCard virtualCard, MemberDetails memberDetails, string _point, string methodName)
        {

            long[] pointTypeIDs = new long[1];
            long[] pointEventIDs = new long[1];
            PointType pointType = null;
            PointEvent pointEvent = null;

            if (Utilities.GetPointTypeAndEvent(_pointType, _pointEvent, out pointEvent, out pointType))
            {
                pointTypeIDs[0] = pointType.ID;
                pointEventIDs[0] = pointEvent.ID;

                if (member.GetPoints(pointTypeIDs, pointEventIDs, DateTime.Today.AddDays(-14), DateTime.Today.AddDays(-13).AddTicks(-1)) <= 0)
                {
                    //Return points for the reward
                    AddPoints(virtualCard, memberDetails, new DateTime(2199, 12, 30), DateTime.Now.AddDays(-14), _pointType, _pointEvent, "Invalid email on reward", _point);
                }
                else
                {
                    this.logger.Error(this.className, methodName, "This reward has already been refunded today.");
                }
            }
            else
            {
                this.logger.Error(this.className, methodName, "GetPointTypeAndEvent not found");
            }
        }

        public static void AddPoints(VirtualCard virtualCard, MemberDetails memberDetails, DateTime EndDate, DateTime transactionDate, string pointType, string pointEvent, string note, string strPoints)
        {
            Decimal dblPoints = 0;
            Decimal.TryParse(strPoints, out dblPoints);

            Utilities.AddBonusPoints(virtualCard, pointType, pointEvent, dblPoints, EndDate, transactionDate, note);
        }
    }
}
