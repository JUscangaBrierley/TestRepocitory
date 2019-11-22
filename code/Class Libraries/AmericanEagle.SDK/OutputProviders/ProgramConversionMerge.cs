// ----------------------------------------------------------------------------------
// <copyright file="ProactiveMerge.cs" company="Brierley and Partners">
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
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.DataAcquisition.Core;
    using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
    using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

    /// <summary>
    /// Class ProgramConversionMerge
    /// </summary>
    public class ProgramConversionMerge : IDAPOutputProvider
    {
        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private string PrimaryLid = string.Empty;
        private string SecondaryLid = string.Empty;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.    
        /// </summary>
        /// 
        public void Dispose()
        {
            return;
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
                XmlNode xmlNode = doc.SelectSingleNode("Merges/Merge");


                foreach (XmlNode node in xmlNode.ChildNodes)
                {

                    this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "NodeName:" + node.Name);


                    switch (node.Name)
                    {
                        case "primarylid":
                            PrimaryLid = node.InnerText;
                            break;

                        case "secondarylid":
                            SecondaryLid = node.InnerText;
                            break;

                    }
                }


                if (null != xmlNode)
                {

                    if (String.IsNullOrEmpty(this.PrimaryLid)) {
                        throw new Exception("Primary Loyalty Number is empty");
                    }

                    if (String.IsNullOrEmpty(this.SecondaryLid))         {
                        throw new Exception("Secondary Loyalty Number is empty");
                    }

                    Member fromMember = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(this.SecondaryLid);
                    Member toMember = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(this.PrimaryLid);

                    if (null != fromMember && null != toMember)
                    {
                        MemberMergeOptions options = new MemberMergeOptions();
                        options.MemberProfile_Name = false;
                        options.MemberProfile_MailingAddress = false;
                        options.MemberProfile_PrimaryPhoneNumber = false;
                        options.PointBalance = false;
                        options.MemberRewards = false;
                        options.MemberTiers = false;
                        options.MemberCoupons = false;
                        options.MemberPromotions = false;
                        options.VirtualCards = true;
                        options.FromPrimaryIsNewPrimaryVirtualCard = false;
                        CSAgent agent = LWDataServiceUtil.CSServiceInstance().GetCSAgentByUserName("system", Brierley.FrameWork.Common.AgentAccountStatus.Active);
                        long agentId = 0;
                        if (agent != null)
                        {
                            agentId = agent.Id;
                        }
                        Merge.IsFromProactiveMerge = false;
                        Merge.MergeMember(fromMember, toMember, options, null, null, "system", agentId);
                    }
                    else  {
                        // Log error when member not found
                        this.logger.Error(this.className, methodName, "Member Not Found for Loyalty Number : " + Utilities.GetLoyaltyIDNumber(fromMember) + " or " + Utilities.GetLoyaltyIDNumber(toMember));
                    }


                }
                this.logger.Trace(this.className, methodName, "End");
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message, ex);
                throw;
            }
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
