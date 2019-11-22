// ----------------------------------------------------------------------------------
// <copyright file="BonusLoader.cs" company="Brierley and Partners">
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
    using Brierley.ClientDevUtilities.LWGateway;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.DataAcquisition.Core;
    using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
    using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

    /// <summary>
    /// Class BonusLoader
    /// </summary>
    public class BonusLoader : IDAPOutputProvider
    {
        /// <summary>
        /// Description is used for point event
        /// </summary>
        private string description = string.Empty;

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
        public void ProcessMessageBatch(List<string> messageBatch)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            try
            {
                // Tracing for starting of method
                this.logger.Error(this.className, methodName, "Starts");
                PointEvent pointEvent = null;
                PointType pointType = null;

                // Create the XmlDocument.
                XmlDocument doc = new XmlDocument();
                foreach (string str in messageBatch)
                {
                    // Loding XML
                    doc.LoadXml(str);
                }

                // Get XML Node
                XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/Global");
                if (null != xmlNode)
                {
                    using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        if (!string.IsNullOrEmpty(xmlNode.Attributes["Points"].Value))
                        {
                            pointEvent = lwService.GetPointEvent(this.description);
                            if (pointEvent == null)
                            {
                                this.logger.Trace(this.className, methodName, "Point Event: (" + this.description + ") doesn't exist so create it.");
                                pointEvent = new PointEvent();
                                pointEvent.Description = this.description;
                                pointEvent.Name = this.description;
                                lwService.CreatePointEvent(pointEvent);
                            }
                            // Check whether point type and point events are exist or not
                            if (Utilities.GetPointTypeAndEvent("Bonus Points", this.description, out pointEvent, out pointType))
                            {
                                // Get member
                                Member member = lwService.LoadMemberFromLoyaltyID(xmlNode.Attributes["LoyaltyNumber"].Value);
                                if (null == member)
                                {
                                    // Log error when member not found
                                    this.logger.Error(this.className, methodName, "Member Not Found for Loyalty Number - " + xmlNode.Attributes["LoyaltyNumber"].Value);
                                }
                                else
                                {
                                    // Getr virtual card
                                    VirtualCard virtualCard = Utilities.GetVirtualCard(member);

                                    // Credit the points
                                    Utilities.AddBonusPoints(virtualCard, pointType.Name, pointEvent.Name, Convert.ToInt64(xmlNode.Attributes["Points"].Value));
                                }
                            }
                        }
                        else
                        {
                            // Assign description from flat file
                            this.description = xmlNode.Attributes["LoyaltyNumber"].Value;
                        }
                    }
                }
                else
                {
                    // Logging for null xml node
                    this.logger.Error(this.className, methodName, "xml node not found");
                }

                // Logging for ending of method
                this.logger.Error(this.className, methodName, "Ends");
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
            //   throw new System.NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    }
}
