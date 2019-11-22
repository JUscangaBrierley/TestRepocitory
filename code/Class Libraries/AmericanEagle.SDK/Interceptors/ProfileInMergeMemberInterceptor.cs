using Brierley.ClientDevUtilities.LWGateway;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
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
    class ProfileInMergeMemberInterceptor : IInboundInterceptor
    {
        #region Fields

        /// <summary>
        /// Contains the Namevalue collection of parameters 
        /// </summary>
        private NameValueCollection parms;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        private static LWLogger logger = LWLoggerManager.GetLogger("ProfileInMerMemberInterceptor");
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
            Member memberTo;
            Member memberFrom;
            Member memberMerged;
            int vcTocount, vcFromCount, vcToPrimary, vcFromPrimary;
            bool banFrom;

            vcToPrimary = 0;
            vcFromPrimary = 0;

            long ipcodeFrom = long.Parse(LWIntegrationUtilities.GetValueByPath(memberNode, "Member/FROMIPCODE"));

            memberTo = member;

            using (ILoyaltyDataService ldService = _dataUtil.LoyaltyDataServiceInstance())
            {
                memberFrom = ldService.LoadMemberFromIPCode(ipcodeFrom);
            }

            vcTocount = memberTo.LoyaltyCards.Count;
            vcFromCount = memberFrom.LoyaltyCards.Count;

            for(int i = 0; i < vcTocount; i++)
            {
                if (memberTo.LoyaltyCards[i].IsPrimary)
                    vcToPrimary = i;
            }

            for (int i = 0; i < vcFromCount; i++)
            {
                if (memberFrom.LoyaltyCards[i].IsPrimary)
                    vcFromPrimary = i;
            }

            if( memberTo.LoyaltyCards[vcToPrimary].DateRegistered > memberFrom.LoyaltyCards[vcFromPrimary].DateRegistered)
            {
                banFrom = false;
            }
            else
            {
                banFrom = true; 
            }

            int x;

            List<IClientDataObject> details = memberTo.GetChildAttributeSets("MemberDetails");

            if (details != null && details.Count > 0)
            {
                x = 0;
                foreach (IClientDataObject detail in details)
                {
                    member.GetChildAttributeSets("MemberDetails")[x].SetAttributeValue("AITUpdate", true);
                    x++;
                }
            }

            List<IClientDataObject> detailsFrom = memberFrom.GetChildAttributeSets("MemberDetails");

            if (detailsFrom != null && detailsFrom.Count > 0)
            {
                x = 0;
                foreach (IClientDataObject detailFrom in detailsFrom)
                {
                    member.GetChildAttributeSets("MemberDetails")[x].SetAttributeValue("AITUpdate", true);
                    x++;
                }
            }

            using (ILoyaltyDataService ldService = _dataUtil.LoyaltyDataServiceInstance())
            {

                MemberMergeOptions MergeOptions = new MemberMergeOptions();

                MergeOptions.MemberProfile_Name = false;
                MergeOptions.MemberProfile_MailingAddress = false;
                MergeOptions.MemberProfile_PrimaryPhoneNumber = false;
                MergeOptions.PointBalance = false;
                MergeOptions.VirtualCards = true;
                MergeOptions.FromPrimaryIsNewPrimaryVirtualCard = banFrom;
                MergeOptions.MemberTiers = true;
                MergeOptions.MemberRewards = true;
                MergeOptions.MemberPromotions = true;
                MergeOptions.MemberBonuses = true;
                MergeOptions.MemberCoupons = true;

                int contVC;

                contVC = memberFrom.LoyaltyCards.Count;

                for (int i = 0; i < contVC; i++)
                {
                    memberFrom.LoyaltyCards[i].LinkKey = memberTo.LoyaltyCards[0].LinkKey;
                }

                memberMerged = ldService.MergeMember(memberFrom, memberTo, null, null, DateTime.ParseExact("12/31/2199", "dd/mm/yyyy", null), MergeOptions);
            }

            return memberMerged;
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        /// <summary>
        /// Mehtod used to process the member after save. for e.g. send some notification after save
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="member">Member to be processed</param>
        /// <param name="memberNode">Membernode from XML file</param>
        /// <returns>Returns the processed member</returns>

        public virtual Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode, IList<Brierley.FrameWork.ContextObject.RuleResult> results = null)
        {
            long fromipcode = long.Parse(LWIntegrationUtilities.GetValueByPath(memberNode, "Member/FROMIPCODE"));
            long fromloyalty = long.Parse(LWIntegrationUtilities.GetValueByPath(memberNode, "Member/fromloyaltyid"));

            using (ILoyaltyDataService ldService = _dataUtil.LoyaltyDataServiceInstance())
            {

                ldService.Database.Execute(
                        "INSERT INTO bp_ae.ats_membermergehistory (a_rowkey,  a_ipcode,  a_parentrowkey,  a_fromloyaltyid,  a_fromipcode, a_changedby,  statuscode, createdate, updatedate, Last_Dml_Id) values ( seq_rowkey.nextval, @0, @1, @2, @3, 'DAP Merge', 0, @4, @5, LAST_DML_ID#.Nextval )",
                        member.IpCode, member.IpCode, fromloyalty, fromipcode, DateTime.Now, DateTime.Now
                    );

            }
            return member;
        }
        public void ValidateOperationParameter(string operationName, string source, string payload)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}

