namespace AmericanEagle.SDK.Interceptors
{
    using System;
    using System.Collections.Specialized;
    using System.Xml.Linq;
    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyWare.LWIntegration.Common; 
    using Brierley.FrameWork.LWIntegration;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Common;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Class contains the interceptor code for Card Loader file processing.
    /// </summary>
    public class AmericanEagleCardLoaderInterceptor : AmericanEagleInboundInterceptorBase
    {
        /// <summary>
        /// Object of Data Service
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("AmericanEagleCardLoaderInterceptor");

        /// <summary>
        /// Initialization method definition
        /// </summary>
        /// <param name="parameters">NameValueCollection parameters</param>
        public override void Initialize(NameValueCollection parameters)
        {
            base.Initialize(parameters);
        }
       
        /// <summary>
        /// ProcessMemberBeforePopulation method implementation
        /// </summary>
        /// </summary>
        /// <param name="config">LWIntegrationConfig config</param>
        /// <param name="memberNode">XElement memberNode</param>
        /// <returns>return member</returns>
        public override Member ProcessMemberBeforePopulation(LWIntegrationConfig  config, Member member, System.Xml.Linq.XElement memberNode)
        {
            // PI 29668 - Adding a validation on new loyalty id number coming, if it exceeds 14 digits or not valid then reject the record
            string fileName = memberNode.Attribute("RejectedRecordsFileName").Value;
            StreamWriter rejectsFile = new StreamWriter(fileName, true);
            logger.Trace("AmericanEagleCardLoaderInterceptor", "ProcessMemberBeforePopulation", "Begin");
            try
            {
                if (!string.IsNullOrEmpty(memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value))
                {
                    long newLoyaltyNumber = 0;
                    string oldLoyaltyNumber = string.Empty;

                    long.TryParse(memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value, out newLoyaltyNumber);


                    if (memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value.Length > 14 || SDK.Global.LoyaltyCard.IsLoyaltyNumberValid(newLoyaltyNumber) == false)
                    {
                        rejectsFile.WriteLine(memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value);
                        memberNode.Element("MemberCardReplacements").Remove();
                        memberNode.Element("VirtualCard").Remove();
                        memberNode.Element("MemberDetails").Remove();
                    }
                    else
                    {
                        Member newMem = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(Convert.ToString(newLoyaltyNumber));
                        if (newMem != null)
                        {//check if there is a primary virtual card set,if not then set it.
                            VirtualCard newVc = Utilities.GetVirtualCard(newMem);
                            VirtualCard primVc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                            if (newVc != null)
                            {
                                if (primVc != null)
                                {
                                    if (newVc.LoyaltyIdNumber == primVc.LoyaltyIdNumber)
                                    { // incoming loyaltynumber is primary,do nothing
                                    }
                                    else
                                    {
                                        //incoming is not primary,let code below mark the primary to false
                                    }
                                }
                                else// primVc is null so then mark incoming as primary, because the DAP batch processing will fail this process and not go to after save method
                                {
                                    newVc.IsPrimary = true;
                                    LWDataServiceUtil.DataServiceInstance(true).SaveMember(newMem);
                                }
                            }
                           
                        }
                        else //incoming newMem is null so mark current primary as false.
                        {
                            if (!string.IsNullOrEmpty(memberNode.Element("MemberCardReplacements").Attribute("OldLoyaltyIdNumber").Value))
                            {
                                oldLoyaltyNumber = memberNode.Element("MemberCardReplacements").Attribute("OldLoyaltyIdNumber").Value;
                                Member mem = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(oldLoyaltyNumber);
                                if (mem != null)
                                {
                                    VirtualCard vc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                                    if (vc != null)
                                    {
                                        vc.IsPrimary = false; // setting the current isprimary card to false.
                                        LWDataServiceUtil.DataServiceInstance(true).SaveMember(member);
                                    }
                                }
                            }
                        }  

                    }
                   
                   }
                rejectsFile.Close();
                logger.Trace("AmericanEagleCardLoaderInterceptor", "ProcessMemberBeforePopulation", "End");
                return member;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode, IList<Brierley.FrameWork.ContextObject.RuleResult> results = null)
        {
            VirtualCard vc = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
            if (vc != null)
            { }
            else // because there are no primary for the member
            {
                vc = member.GetLoyaltyCardByType(VirtualCardSearchType.MostRecentIssued);
                member.MarkVirtualCardAsPrimary(vc);
            }

            return member;
        }
    }
}
