// ----------------------------------------------------------------------------------
// <copyright file="MemberStatusUpdate.cs" company="Brierley and Partners">
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
    using Brierley.Clients.AmericanEagle.DataModel;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    using Brierley.LoyaltyWare.DataAcquisition.Core;
    using Brierley.LoyaltyWare.DataAcquisition.Core.Output;
    using Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters;

    /// <summary>
    /// Class MemberStatusUpdate
    /// </summary>
    public class MemberStatusUpdate : IDAPOutputProvider
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
        /// Stores Variable LoyaltyNumber
        /// </summary>
        private string strLoyaltyNumber = string.Empty;

        /// <summary>
        /// Stores Variable Status
        /// </summary>
        private string strStatus = string.Empty;

        /// <summary>
        /// Stores Variable EmployeeID
        /// </summary>
        private long strEmployeeID = long.MinValue;

        /// <summary>
        /// Stores Variable Reason
        /// </summary>
        private string strReason = string.Empty;

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
                        strLoyaltyNumber = xmlNode.Attributes["LoyaltyNumber"].Value.Trim();
                        strStatus = xmlNode.Attributes["Status"].Value.Trim();
                        strEmployeeID = Convert.ToInt64(xmlNode.Attributes["EmployeeID"].Value.Trim());
                        strReason = xmlNode.Attributes["Reason"].Value.Trim();

                        // Get member
                        Member member = lwService.LoadMemberFromLoyaltyID(strLoyaltyNumber);

                        if (member == null)
                        {
                            // Log error when member not found
                            this.logger.Error(this.className, methodName, "Member Not Found for Loyalty Number - " + xmlNode.Attributes["LoyaltyIdNumber"].Value);
                        }
                        else
                        {
                            if ((strStatus == "Terminated") || (strStatus == "1"))
                            {
                                lwService.CancelOrTerminateMember(member, DateTime.Today, String.Empty, true, new MemberCancelOptions());
                            }
                            else if ((strStatus == "Frozen") || (strStatus == "2"))
                            {
                                member.NewStatus = MemberStatusEnum.Locked;
                                member.NewStatusEffectiveDate = DateTime.Today;
                            }
                            else if ((strStatus == "Active") || (strStatus == "3"))
                            {
                                member.NewStatus = MemberStatusEnum.Active;
                                member.NewStatusEffectiveDate = DateTime.Today;
                            }

                            string _note = "Changed member status to '" + strStatus + "'. Reason: " + strReason;

                            using (var ilwcsservice = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                            {
                                CSNote note = new CSNote();
                                note.Note = _note;
                                note.MemberId = member.IpCode;
                                note.CreateDate = DateTime.Now;
                                note.CreatedBy = strEmployeeID;
                                ilwcsservice.CreateNote(note);
                            }

                            member.FirstName = "Account Terminated";
                            member.LastName = "Account Terminated";
                            member.PrimaryEmailAddress = string.Empty;
                            // Getting MemberDetails for Member object
                            IList<IClientDataObject> clientObjects = member.GetChildAttributeSets("MemberDetails");

                            IList<MemberDetails> memberDetails = new List<MemberDetails>();
                            MemberDetails memberDetail = (MemberDetails)clientObjects[0];

                            if (memberDetails != null)
                            {
                                memberDetail.EmailAddress = string.Empty;
                                memberDetail.AddressLineOne = "Account Terminated";
                                memberDetail.AddressLineTwo = "Account Terminated";
                                memberDetail.City = "Account Terminated";

                                // Unlink from ae.com
                                if ((memberDetail.MemberSource == (int)MemberSource.OnlineAEEnrolled) || (memberDetail.MemberSource == (int)MemberSource.OnlineAERegistered))
                                {
                                    memberDetail.MemberSource = (int)MemberSource.CSPortalUnlinked;
                                }
                            }

                            // Save member Information to Database
                            lwService.SaveMember(member);
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

        public int Shutdown()
        {
            return 0;
        }
    }
}