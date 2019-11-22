// ----------------------------------------------------------------------------------
// <copyright file="VendorEmailUpdate.cs" company="Brierley and Partners">
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
    /// Class VendorEmailUpdate
    /// </summary>
    public class VendorEmailUpdate : IDAPOutputProvider
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
        /// Stores the email address that is to be changed
        /// </summary>
        private string strLoyaltyNumber = string.Empty;

        //private string emailbeforeChange = string.Empty;
        /// <summary>
        /// Stores the email address that is to be changed
        /// </summary>
        private string emailBeforeUpdate = string.Empty;

        //private string emailAfterChange = string.Empty;
        /// <summary>
        /// Stores the email address that is to be changed
        /// </summary>
        private string emailAfterChange = string.Empty;
        //private string strChangedBy = string.Empty;
        /// <summary>
        /// Stores the changed by field in member details
        /// </summary>
        /// 
       // private string strChangedBy = ("Vendor Email Update on:" + (DateTime.Now).ToString());
        private string strChangedBy = string.Empty;
        //private bool Headerrec = false;
        /// <summary>
        /// flag for header rec
        /// </summary>
        private bool Headerrec = true;

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
                XmlNode xmlNode = doc.SelectSingleNode("AttributeSets/Member/VirtualCard");
                XmlNode xmlNode2 = doc.SelectSingleNode("AttributeSets/Member/MemberDetails");
                // check for valid xml data
                if (null != xmlNode && null != xmlNode2)
                  {
                      //Vendor Inputs-Loyalty number and new email address are captured
                     strLoyaltyNumber = xmlNode.Attributes["LoyaltyIdNumber"].Value.Trim();
                     emailAfterChange = (xmlNode2.Attributes["EmailAddress"].Value).Trim().ToLower();
                     strChangedBy = xmlNode2.Attributes["ChangedBy"].Value;      //set value of changed by to vendor change and updated date

                     if (!string.IsNullOrEmpty(strLoyaltyNumber))
                      {
                          // Get member
                          Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(strLoyaltyNumber);

                          if (null == member)
                            {
                              // Log error when member not found
                              this.logger.Error(this.className, methodName, "Member Not Found for Loyalty Number - " + xmlNode.Attributes["LoyaltyIdNumber"].Value);
                            }

                          IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
                          MemberDetails memberDetails = (MemberDetails)mbrDtlObjs[0];

                          if (memberDetails != null  )
                            {
                              //Change to be made only if member is not linked to AE.com and is not terminated
                                if ((memberDetails.MemberSource != (int)MemberSource.OnlineAEEnrolled) && (memberDetails.MemberSource != (int)MemberSource.OnlineAERegistered) && (member.MemberStatus != MemberStatusEnum.Terminated)) 
                                {
                                       this.emailBeforeUpdate = memberDetails.EmailAddress;
                                       memberDetails.OldEmailAddress = memberDetails.EmailAddress;
                                       memberDetails.EmailAddress = emailAfterChange;
                                       memberDetails.ChangedBy =  strChangedBy;
                                      // Save member Information to Database
                                      LWDataServiceUtil.DataServiceInstance(true).SaveMember(member);
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
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    }
}
