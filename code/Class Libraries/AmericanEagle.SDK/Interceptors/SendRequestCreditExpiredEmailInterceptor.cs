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
    class SendRequestCreditExpiredEmailInterceptor : IInboundInterceptor
    {
        #region Fields

        /// <summary>
        /// Contains the Namevalue collection of parameters 
        /// </summary>
        private NameValueCollection parms;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private static LWLogger logger = LWLoggerManager.GetLogger("SendRequestCreditExpiredEmailInterceptor");
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
            // EmailType emailType = EmailType.RequestCreditReceived;
            // EmailType emailType = EmailType.RequestCreditExpiredWeb;
            // EmailType emailType = EmailType.RequestCreditExpiredInStore;
            EmailType emailType;

            long ipcode = long.Parse(LWIntegrationUtilities.GetValueByPath(memberNode, "Member/IPCODE"));
            string firstname = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/firstname_query");
            string nmail = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/emailaddress_query");
            int transaction_type = int.Parse(LWIntegrationUtilities.GetValueByPath(memberNode, "Member/transaction_type"));

            string loyaltyid = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/loyaltyidnumber_query");
            string orderamnt = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/txnamount");
            string ordernmbr = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/ordernumber");

            string store = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/storenumber");
            string register = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/registernumber");
            string txnNumber = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/txnnumber");
            string txnDate = LWIntegrationUtilities.GetValueByPath(memberNode, "Member/txndate");

            Dictionary<string, string> additionalFields = new Dictionary<string, string>();

            if (transaction_type == 1)
            {
                emailType = EmailType.RequestCreditExpiredInStore;

                additionalFields.Add("FirstName", firstname);
                additionalFields.Add("storenumber", store);
                additionalFields.Add("loyaltynumber", loyaltyid);
                additionalFields.Add("registernumber", register);
                additionalFields.Add("transactionNumber", txnNumber);
                additionalFields.Add("transactionDate", txnDate);
            }
            else
            {
                emailType = EmailType.RequestCreditExpiredWeb;

                additionalFields.Add("FirstName", firstname);
                additionalFields.Add("Loyaltynumber", loyaltyid);
                additionalFields.Add("OrderAmount", orderamnt);
                additionalFields.Add("OrderNumber", ordernmbr);
            }

            try
            {
                AEEmail.SendEmail(member, emailType, additionalFields, nmail);
            }
            catch (Exception ex)
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }




            /*
            additionalFields.Add("FirstName", member.FirstName);
            additionalFields.Add("Loyaltynumber", member.LoyaltyCards[0].LoyaltyIdNumber);
            additionalFields.Add("OrderAmount", "12312414");
            additionalFields.Add("OrderNumber", "9999213291");

            AEEmail.SendEmail(member, emailType, additionalFields, "fsosa@brierley.com");
            AEEmail.SendEmail(member, emailType, additionalFields, "gcampos@brierley.com");
            */


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

