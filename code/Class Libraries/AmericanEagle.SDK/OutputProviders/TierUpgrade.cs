// ----------------------------------------------------------------------------------
// <copyright file="TierUpgrade.cs" company="Brierley and Partners">
//     Copyright statement. All right reserved
// </copyright>
// ----------------------------------------------------------------------------------
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
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.OutputProviders
{
    public class TierUpgrade : IDAPOutputProvider
    {
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
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);

        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// This method is called to process the messages in the batch
        /// </summary>
        /// <param name="messageBatch">String List</param>
        public void ProcessMessageBatch(List<string> messageBatch)
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
                    LoyaltyNumber = xmlNode.Attributes["LoyaltyNumber"].Value.Trim();
                    Tier = (xmlNode.Attributes["Tier"].Value).Trim().ToLower();
                    ValidationRequiredFlag = xmlNode.Attributes["ValidationRequiredFlag"].Value; 
                    EliteReasonCode = xmlNode.Attributes["EliteReasonCode"].Value;

                    //if they don't send us a reason code, then default to AE Nomination.
                    if (EliteReasonCode.Length == 0)
                    {
                        EliteReasonCode = "3";
                    }

                    if (!string.IsNullOrEmpty(LoyaltyNumber))
                    {
                        // Get member
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            Member member = lwService.LoadMemberFromLoyaltyID(LoyaltyNumber);

                            if (null == member)
                            {
                                // Log error when member not found
                                this.logger.Error(this.className, methodName, "Member Not Found for Loyalty Number - " + LoyaltyNumber);
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
                            crit.Add(LWCriterion.OperatorType.AND, "ReasonCode", EliteReasonCode, LWCriterion.Predicate.Eq);

                            IList<IClientDataObject> objRefTiersReason = lwService.GetAttributeSetObjects(null, "RefTierReason", crit, null, false);

                            if (objRefTiersReason != null)
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

                            if (memberDetails != null)
                            {
                                if (memberDetails.ExtendedPlayCode != 1)
                                {
                                    memberDetails.ExtendedPlayCode = 1;
                                    memberDetails.PendingCellVerification = 1;
                                }

                                if (ValidationRequiredFlag == "1")
                                {
                                    memberDetails.PendingEmailVerification = 1;
                                    memberDetails.NextEmailReminderDate = DateTime.Today.AddDays(1);
                                }
                                lwService.SaveMember(member);
                            }
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

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        public int Shutdown()
        {
            return 0;
            //  throw new System.NotImplementedException();
        }

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
            System.Console.WriteLine("Stop");
        }
    }
}
