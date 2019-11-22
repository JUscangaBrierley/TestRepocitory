// -------------------------------------------------------------------------------------------
// <copyright file="AmericanEagleUpdateProfileInterceptor.cs" company="Brierely and Partners">
//     Copyright statement. All right reserved
// </copyright>
// -------------------------------------------------------------------------------------------
namespace AmericanEagle.SDK.Interceptors
{
    #region
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Xml.Linq;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.Cache;
    using Brierley.FrameWork.Data.DomainModel;
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyWare.LWIntegration.Common;
    using Brierley.FrameWork.Common.Exceptions;
    using Brierley.FrameWork.LWIntegration;
    using Brierley.ClientDevUtilities.LWGateway;

    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    #endregion

    /// <summary>
    /// class AmericanEagleUpdateProfileInterceptor
    /// </summary>
    public class AmericanEagleUpdateProfileInterceptor : AmericanEagleInboundInterceptorBase
    {
        /// <summary>
        /// Logger for AmericanEagleUpdateProfileInterceptor
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_DAP_SERVICE);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        /// <summary>
        /// Method ProcessMemberBeforePopulation
        /// </summary>
        /// <param name="config">LWIntegrationConfig config</param>
        /// <param name="member">Member member</param>
        /// <param name="memberNode">XElement memberNode</param>
        /// <returns>Member object</returns>
        public override Member ProcessMemberBeforePopulation(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            DateTime lastUpdateProfileSent = DateTime.MinValue;
            string lastUpdateProfileSentString;
            using (var dService = _dataUtil.DataServiceInstance())
            {
                lastUpdateProfileSentString = dService.GetClientConfigProp("LastAITProfileSentDate");
            }
            string loyaltyNumber = memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value;

            logger.Trace("AmericanEagleUpdateProfileInterceptor", "ProcessMemberBeforePopulation", "Begin");
            logger.Trace("AmericanEagleUpdateProfileInterceptor", "ProcessMemberBeforePopulation", "Processing Member: " + loyaltyNumber);

            try
            {
                if (!string.IsNullOrEmpty(lastUpdateProfileSentString) && DateTime.TryParse(lastUpdateProfileSentString, out lastUpdateProfileSent))
                {
                    using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        member = lwService.LoadMemberFromLoyaltyID(loyaltyNumber);
                    }
                    if (null != member)
                    {
                        MemberDetails memberDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                        if (null != memberDetails)
                        {
                            if ((memberDetails.AITUpdate == null) || (memberDetails.AITUpdate == false))
                            {
                                if (this.ValidateData(memberNode))
                                {
                                    logger.Trace("AmericanEagleUpdateProfileInterceptor", "ProcessMemberBeforePopulation", "MemberDetails Updated");
                                    memberNode.Element("MemberDetails").SetAttributeValue("ChangedBy", "Profile Updates Processor");
                                    memberNode.SetAttributeValue("ChangedBy", "Profile Updates Processor");
                                }
                            }
                            else
                            {
                                //rkg
                                //suppress this error for now until we can have time to research.  this is filling up the log files.
                                //throw new Exception("Profile information is stale");
                            }
                        }
                        else
                        {
                            throw new Exception("Member details not found for member with LoyaltyID " + loyaltyNumber);
                        }
                    }
                    else
                    {
                        throw new Exception("Member not found with LoyaltyID " + loyaltyNumber);
                    }
                }
                else
                {
                    throw new Exception("Invalid/Blank LastUpdateProfileSent" + lastUpdateProfileSentString);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
            logger.Trace("AmericanEagleUpdateProfileInterceptor", "ProcessMemberBeforePopulation", "End");

            return member;
        }

        /// <summary>
        /// Method ValidateData
        /// </summary>
        /// <param name="memberNode">XElement memberNode</param>
        /// <returns>bool : false if validation fails else true</returns>
        private bool ValidateData(XElement memberNode)
        {
            logger.Trace("AmericanEagleUpdateProfileInterceptor", "ValidateData", "Begin");
            try
            {
                string addressLineOne = memberNode.Element("MemberDetails").Attribute("AddressLineOne").Value;
                string addressLineTwo = memberNode.Element("MemberDetails").Attribute("AddressLineTwo").Value;

                if (!Utilities.IsAddressValid(addressLineOne)) // AEO-1067 AH
                {
                    throw new Exception("AddressOne is not valid");
                }

                if (!Utilities.IsAddressValid(addressLineTwo))
                {
                    throw new Exception("AddressTwo is not valid");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
            logger.Trace("AmericanEagleUpdateProfileInterceptor", "ValidateData", "End");

            return true;
        }
    }
}
