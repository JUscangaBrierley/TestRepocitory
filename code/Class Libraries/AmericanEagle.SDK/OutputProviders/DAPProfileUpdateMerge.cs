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
    using System.Text;
    using Brierley.ClientDevUtilities.LWGateway;

    /// <summary>
    /// Class ProactiveMerge
    /// </summary>
    public class DAPProfileUpdateMerge : IDAPOutputProvider
    {
        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.    
        /// </summary>
        /// 
        public void Dispose ( ) {
            //Reset the ProactiveMergeStartDate variable in client configuration table
            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, 
                MethodBase.GetCurrentMethod().Name,
                "ProfileUpdateMerge - Begin");

            this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, 
                MethodBase.GetCurrentMethod().Name,
                "ProfileUpdateMerge - End");
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
        public void Initialize ( NameValueCollection globals, 
                                 NameValueCollection args, 
                                long jobId, 
                                DAPDirectives config,
                                NameValueCollection parameters,
                                DAPPerformanceCounterUtil performUtil ) {
        }

        /// <summary>
        /// This method is called to process the messages in the batch
        /// </summary>
        /// <param name="messageBatch">String List</param>
        public void ProcessMessageBatch ( List<string> messageBatch ) {

            string fromloyalty = string.Empty;
            string toloyalty = string.Empty;

            string methodName = MethodBase.GetCurrentMethod().Name;

            try {

                // Tracing for starting of method
                this.logger.Trace(this.className, methodName, "Starts");

                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach ( string str in messageBatch ) {
                    // Loding XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("Merges/Merge");

                if (null != xmlNode)
                {

                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {

                        this.logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "NodeName:" + node.Name);


                        switch (node.Name)
                        {
                            case "from":
                                fromloyalty = node.InnerText;
                                break;

                            case "to":
                                toloyalty = node.InnerText;

                                break;

                        }
                    }

                    using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        Member fromMember = lwService.LoadMemberFromLoyaltyID(fromloyalty);
                        Member toMember = lwService.LoadMemberFromLoyaltyID(toloyalty);

                        if (toMember != null && fromMember != null)
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
                            CSAgent agent;
                            using (var svc = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                            {
                                agent = svc.GetCSAgentByUserName("system", AgentAccountStatus.Active);
                            }
                            long agentId = 0;
                            if (agent != null)
                            {
                                agentId = agent.Id;
                            }
                            Merge.IsFromProactiveMerge = false;
                            Merge.MergeMember(fromMember, toMember, options, null, null, "system", agentId);


                            LWCriterion crit = new LWCriterion("ProfileUpdateMerge");
                            AttributeSetMetaData meta = lwService.GetAttributeSetMetaData("ProfileUpdateMerge");
                            crit.Add(LWCriterion.OperatorType.AND, "FROMLOYALTYID", fromloyalty, LWCriterion.Predicate.Eq);
                            crit.Add(LWCriterion.OperatorType.AND, "TOLOYALTYID", toloyalty, LWCriterion.Predicate.Eq);
                            crit.Add(LWCriterion.OperatorType.AND, "Status", 0, LWCriterion.Predicate.Eq);
                            LWQueryBatchInfo lWQueryBatchInfo = new LWQueryBatchInfo();
                            lWQueryBatchInfo.BatchSize = 1;
                            lWQueryBatchInfo.StartIndex = 0;

                            IList<IClientDataObject> rows = lwService.GetAttributeSetObjects(null,
                                "ProfileUpdateMerge", crit, lWQueryBatchInfo, false, false);

                            if (rows == null || rows.Count == 0)
                            {
                                this.logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name,
                                    MethodBase.GetCurrentMethod().Name,
                                    "Error: ProfileUpdateMerge row does not exist. From:" + fromloyalty + " To:" + toloyalty);

                                throw new Exception("Error: ProfileUpdateMerge row does not exist. From:" + fromloyalty + " To:" + toloyalty);
                            }
                            else
                            {
                                ProfileUpdateMerge aRow = (ProfileUpdateMerge)rows[0];
                                aRow.Status = 1;
                                lwService.SaveAttributeSetObject(aRow, null, RuleExecutionMode.Real, false);
                            }

                        }
                        else
                        {
                            StringBuilder errorMsg = new StringBuilder();

                            if (fromMember == null)
                            {
                                errorMsg.AppendLine("FROM loyaltynumber does not exist :" + fromloyalty);
                            }

                            if (toMember == null)
                            {
                                errorMsg.AppendLine("TO loyaltynumber does not exist :" + toloyalty);
                            }

                            throw new Exception(errorMsg.ToString());
                        }

                    }
                    this.logger.Trace(this.className, methodName, "End");
                }
            }
            catch ( Exception ex ) {
                this.logger.Error(ex.Message, ex);
                throw;
            }
        }
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown ( ) {
            return 0;
            //  throw new System.NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    }
}
