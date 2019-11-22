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
using AmericanEagle.SDK.Global;

namespace AmericanEagle.SDK.Interceptors
{
    class TestEmailInterceptor : IInboundInterceptor
    {
        #region Fields

        /// <summary>
        /// Contains the Namevalue collection of parameters 
        /// </summary>
        private NameValueCollection parms;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private static LWLogger logger = LWLoggerManager.GetLogger("TestEmailInterceptor");
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
            //EmailType emailType = EmailType.RequestCreditReceived;
            // EmailType emailType = EmailType.RequestCreditExpiredWeb;
            EmailType emailType = EmailType.RequestCreditExpiredInStore;

            Dictionary<string, string> additionalFields = new Dictionary<string, string>();

            /*
             additionalFields.Add("FirstName", member.FirstName);

            AEEmail.SendEmail(member, emailType, additionalFields, "fsosa@brierley.com");
            AEEmail.SendEmail(member, emailType, additionalFields, "gcampos@brierley.com");
            */

            additionalFields.Add("storenumber", member.FirstName);
            additionalFields.Add("loyaltynumber", member.LoyaltyCards[0].LoyaltyIdNumber);
            additionalFields.Add("registernumber", member.LoyaltyCards[0].LoyaltyIdNumber);
            additionalFields.Add("transactionNumber", "12312414");
            additionalFields.Add("transactionDate", "9999213291");

            AEEmail.SendEmail(member, emailType, additionalFields, "fsosa@brierley.com");
            AEEmail.SendEmail(member, emailType, additionalFields, "gcampos@brierley.com");


            return null;
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
