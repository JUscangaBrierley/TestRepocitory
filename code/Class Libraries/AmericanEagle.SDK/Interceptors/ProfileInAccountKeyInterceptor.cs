// AEO-5140 [Francisco Sosa Herrera]

using Brierley.ClientDevUtilities.LWGateway;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Interfaces;
using Brierley.FrameWork.LWIntegration;
using Brierley.FrameWork.LWIntegration.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Xml.Linq;

namespace AmericanEagle.SDK.Interceptors
{
    class ProfileInAccountKeyInterceptor : IInboundInterceptor
    {
        #region Fields

        /// <summary>
        /// Contains the Namevalue collection of parameters 
        /// </summary>
        private NameValueCollection parms;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private static LWLogger logger = LWLoggerManager.GetLogger("ProfileInUpdateInterceptor");
        #endregion

        #region Default Implementation of Interface Methods

        /// <summary>
        /// Initialize with any parameters
        /// </summary>
        /// <param name="parameters">List of parameters</param>
        public virtual void Initialize(NameValueCollection parameters)
        {
            this.parms = parameters;
        }

        /// <summary>
        /// Method that can used to propess non existence of member
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="memberNode">Current node from xml</param>
        public void HandleMemberNotFound(LWIntegrationConfig config, System.Xml.Linq.XElement memberNode)
        {
        }

        /// <summary>
        /// This method is called to load a member if so directive in the configuration file.
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="memberNode">Current node from xml</param>
        /// <returns>Return null</returns>
        public virtual Member LoadMember(LWIntegrationConfig config, XElement memberNode)
        {
            return null;
        }

        /// <summary>
        /// Method that can to process the raw xml. for e.g if we need to any new field which is not there in xml
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="memberNode">Membernode from XML file</param>
        /// <returns>Return current member node</returns>
        public virtual XElement ProcessRawXml(LWIntegrationConfig config, XElement memberNode)
        {
            return memberNode;
        }

        /// <summary>
        /// Method used to process the member before generating the process XML finally used to save the member
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="member">Member to be processed</param>
        /// <param name="memberNode">Membernode from XML file</param>
        /// <returns>Returns the processed member</returns>        
        public virtual Member ProcessMemberBeforePopulation(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            /*
            String tmp;
            DateTime dateissued = member.LoyaltyCards[0].DateIssued;

            if (dateissued == DateTime.Now)
            {
                tmp = "";
            }
            */

            return member;
        }

        /// <summary>
        /// Method used to process the member just before save
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="member">Member to be processed</param>
        /// <param name="memberNode">Membernode from XML file</param>
        /// <returns>Returns the processed member</returns>
        public virtual Member ProcessMemberBeforeSave(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            long rowkey = long.Parse(memberNode.Attribute("ROWKEY").Value);
            string vckey = memberNode.Attribute("VCKEY").Value;
            long parent = long.Parse(memberNode.Attribute("PARENTROWKEY").Value);
            string accountkey = memberNode.Attribute("ACCOUNTKEY").Value;
            DateTime created = DateTime.Parse(memberNode.Attribute("CREATEDATE").Value);
            DateTime updated = DateTime.Parse(memberNode.Attribute("UPDATEDATE").Value);

            using (ILoyaltyDataService ldService = _dataUtil.LoyaltyDataServiceInstance())
            {

                //ldService.SaveMember(member);

                    ldService.Database.Execute("insert into bp_ae.ats_synchronyaccountkey (a_rowkey, statuscode, a_vckey, a_parentrowkey, a_accountkey, createdate, updatedate) values (@0, 0, @1, @2, @3, @4, @5)",
                    rowkey, vckey, parent, accountkey, created, updated);

                // ldService.Database.Execute("COMMIT");

                // End AEO-5253
            }

            // member.LoyaltyCards[0].DateIssued = DateTime.Parse(LWIntegrationUtilities.GetValueByPath(memberNode, "Member/VirtualCard/DATEISSUED"));
            // member.LoyaltyCards[0].DateRegistered = DateTime.Parse(LWIntegrationUtilities.GetValueByPath(memberNode, "Member/VirtualCard/DATEREGISTERED"));
            // member.LoyaltyCards[0].LinkKey = long.Parse(LWIntegrationUtilities.GetValueByPath(memberNode, "Member/VirtualCard/LINKKEY"));

            return member;
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        /// <summary>
        /// Mehtod used to process the member after save. for e.g. send some notification after save
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="member">Member to be processed</param>
        /// <param name="memberNode">Membernode from XML file</param>
        /// <returns>Returns the processed member</returns>

        //public virtual Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode)
        public virtual Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode, IList<Brierley.FrameWork.ContextObject.RuleResult> results = null)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {

            return member;
        }
        public void ValidateOperationParameter(string operationName, string source, string payload)
        {
            throw new System.NotImplementedException();
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        #endregion

    }
}

// End AEO-5140
