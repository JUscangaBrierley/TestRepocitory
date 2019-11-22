using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Xml.Linq;

using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Logging;
// AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
//using Brierley.LoyaltyWare.LWIntegration.Common;
using Brierley.FrameWork.LWIntegration;
// AEO-74 Upgrade 4.5 changes END here -----------SCJ

namespace AmericanEagle.SDK.Interceptors
{
    public class AmericanEagleEnrollmentInterceptor : AmericanEagleInboundInterceptorBase
    {
        private static LWLogger logger = LWLoggerManager.GetLogger("AmericanEagleEnrollmentInterceptor");
        private NameValueCollection parms;

        #region IInboundInterceptor Members

        public void HandleMemberNotFound(LWIntegrationConfig config, System.Xml.Linq.XElement memberNode)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            XElement xmemberNode = memberNode;
        }

        public void Initialize(System.Collections.Specialized.NameValueCollection parameters)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            this.parms = parameters;
        }

        public override Member LoadMember(LWIntegrationConfig config, XElement memberNode)
        {
            logger.Trace("AmericanEagleEnrollmentInterceptor", "LoadMember", "Begin");
            try
            {
                MemberDetails objMemberDetails = null;
                Member subMember = null;
                //IList<Member> lstMember = LWDataServiceUtil.DataServiceInstance(true).GetMembersByName(memberNode.Attribute("FirstName").Value, memberNode.Attribute("LastName").Value, null, new LWQueryBatchInfo()); // AEO-74 Upgrade 4.5 here -----------SCJ
                IList<Member> lstMember = LWDataServiceUtil.DataServiceInstance(true).GetMembersByName(memberNode.Attribute("FirstName").Value, memberNode.Attribute("LastName").Value, null, null);

                // if members found 
                if (lstMember != null && lstMember.Count() > 0)
                {
                    logger.Trace("AmericanEagleEnrollmentInterceptor", "LoadMember", "Found Member");
                    string postalCode = memberNode.Attribute("PrimaryPostalCode").Value;
                    string address1 = memberNode.Element("MemberDetails").Attribute("AddressLineOne").Value;
                    subMember = lstMember.Where(p => p.PrimaryPostalCode == postalCode).FirstOrDefault();

                    // if there is any member exists with same postal code return the same member
                    if (subMember == null)
                    {
                        // This check if address of any existing member matches
                        foreach (Member objMember in lstMember)
                        {
                            objMemberDetails = (MemberDetails)objMember.GetChildAttributeSets("MemberDetails")[0];
                            if (objMemberDetails.AddressLineOne == address1)
                            {
                                subMember = objMember;
                                break;
                            }
                        }
                    }
                }
                logger.Trace("AmericanEagleEnrollmentInterceptor", "LoadMember", "End");

                return subMember;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public override Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode)
        public override Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode, IList<Brierley.FrameWork.ContextObject.RuleResult> results = null)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            IList<IClientDataObject> memberDetails = member.GetChildAttributeSets("MemberDetails");
            MemberDetails mbrDetails = (MemberDetails)memberDetails[0];
            VirtualCard virtualCard = GetVirtualCard(member);

            if (null != mbrDetails)
            {
                // added for redesign project
                bool isPilotMember = Utilities.MemberIsInPilot(mbrDetails.ZipOrPostalCode);

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member: " + member.IpCode.ToString());
                Boolean blnHasEmailBonusCredit = false;
                if (null != mbrDetails.HasEmailBonusCredit)
                {
                    blnHasEmailBonusCredit = mbrDetails.HasEmailBonusCredit.Value;
                }
                if (!String.IsNullOrEmpty(mbrDetails.EmailAddress) && !isPilotMember)
                {
                    if (!blnHasEmailBonusCredit)
                    {
                        Utilities.AddEmailBonus(virtualCard, mbrDetails);
                    }
                    mbrDetails.EmailOptIn = true;
                }
                Boolean blnSMSOptIn = false;
                if (null != mbrDetails.SMSOptIn)
                {
                    blnSMSOptIn = mbrDetails.SMSOptIn.Value;
                }
                if (!String.IsNullOrEmpty(mbrDetails.MobilePhone) && !Utilities.HasMemberEarnedSMSBonus(member) && blnSMSOptIn && !isPilotMember)
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Add SMSBonus: " + mbrDetails.MobilePhone);
                    Utilities.AddSMSBonus(virtualCard, mbrDetails);
                }

                if (isPilotMember)
                {
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Check for tier");

                    MemberTier memberTier = LWDataServiceUtil.DataServiceInstance(true).GetMemberTier(member, DateTime.Today);
                    if (memberTier == null)
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No tier, so add one");
                        member.AddTier("Blue", DateTime.Today, DateTime.Parse("12/31/2199"), "Base");
                    }
                }
                Merge.AddMemberProactiveMerge(member);

                //AEO-238 Begin
                
                ILWDataService service = LWDataServiceUtil.DataServiceInstance(true);


                PointType braCreditPointType = service.GetPointType("Bra Points");
                PointType jeanCreditPointType = service.GetPointType("Jean Points");
                

                PointEvent braCreditPointEvent = service.GetPointEvent("Bra Credit");
                PointEvent jeanCreditPointEvent = service.GetPointEvent("Jeans Credit");;

                if ( mbrDetails.CardType >= 1 && mbrDetails.CardType <=3 ) {
                    LWDataServiceUtil.DataServiceInstance(true).Credit(virtualCard, braCreditPointType,braCreditPointEvent,
                               1, null, DateTime.Today, new DateTime(2199, 12, 31), string.Empty, string.Empty);

                    LWDataServiceUtil.DataServiceInstance(true).Credit(virtualCard, jeanCreditPointType, jeanCreditPointEvent,
                               1, null, DateTime.Today, new DateTime(2199, 12, 31), string.Empty, string.Empty);
                }

             

                //AEO-238 end
            }
            return member;
        }

        public override Member ProcessMemberBeforePopulation(LWIntegrationConfig config, Member member, System.Xml.Linq.XElement memberNode)
        {
            Member smember = null;
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                if (member != null)
                {
                    string strFirstName = memberNode.Attribute("FirstName").Value;
                    string strLastName = memberNode.Attribute("LastName").Value;
                    string strLoyaltyid = memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value;
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "member.Name: " + strFirstName + " " + strLastName);
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "member.LoyaltyID: " + strLoyaltyid);
                    smember = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(strLoyaltyid);
                    if (smember != null)
                    {
                        if (member.IpCode == smember.IpCode)
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Member exists with the same LoyaltyIdNumber");
                            UpdateRecord(ref memberNode, member);
                            //throw new Exception("Member exists with the same LoyaltyIdNumber.");
                        }
                        else
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Loyalty Numbers exists for another member");
                            UpdateRecord(ref memberNode, member);
                            //throw new Exception("Loyalty Numbers exists for another member.");
                        }
                    }
                    else
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "ValidateRecord ");
                        this.ValidateRecord(ref memberNode);
                    }
                }
                else
                {
                    if (smember != null)
                    {
                        throw new Exception("No Match.");
                    }
                    else
                    {
                        this.PopulateRecord(ref memberNode);
                    }
                }

                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                return member;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Member ProcessMemberBeforeSave(LWIntegrationConfig config, Member member, System.Xml.Linq.XElement memberNode)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
            MemberDetails memberDetails = (MemberDetails)mbrDtlObjs[0];
            // Trim dashes and spaces from zip code and then copy to primary postal code
            memberDetails.ZipOrPostalCode = memberDetails.ZipOrPostalCode.Replace("-", "").Replace(" ", "");
            member.PrimaryPostalCode = memberDetails.ZipOrPostalCode;
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            return member;
        }

        //public System.Xml.Linq.XElement ProcessRawXml(LWIntegrationConfig config, System.Xml.Linq.XElement memberNode)
        //{
        //    logger.Trace("AmericanEagleEnrollmentInterceptor", "ProcessRawXml", "Begin");
        //    return memberNode;
        //}

        //public void ValidateOperationParameter(string operationName, string source, string payload)
        //{
        //    logger.Trace("AmericanEagleEnrollmentInterceptor", "ValidateOperationParameter", "Begin");
        //    string soperationName = operationName;
        //}

        #endregion
        #region | Custome Method defination |
        private VirtualCard GetVirtualCard(Member pMember)
        {
            foreach (VirtualCard vc in pMember.LoyaltyCards)
            {
                if (null != vc.LoyaltyIdNumber && vc.IsPrimary)
                {
                    return vc;
                }
            }
            return null;
        }

        /// <summary>
        /// Method which populate the member details and virtual card
        /// </summary>
        /// <param name="memberNode">XML node to be processed</param>
        private void PopulateRecord(ref XElement memberNode)
        {
            long loyaltyNumber;
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            // added for redesign project
            bool isPilotMember = Utilities.MemberIsInPilot(memberNode.Element("MemberDetails").Attribute("ZipOrPostalCode").Value);
            
            // If Loyalty number is null
            if (string.IsNullOrEmpty(memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value))
            {
                // added for redesign project
                if (isPilotMember)
                    loyaltyNumber = LoyaltyCard.GetNextLoyaltyNumber(LoyaltyCard.LoyaltyCardType.AE);
                else
                    loyaltyNumber = LoyaltyCard.GetNextLoyaltyNumber(LoyaltyCard.LoyaltyCardType.Temporary);
            }
            else
            {
                loyaltyNumber = long.Parse(memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value);
            }

            string cardType = string.Empty;
            string strCardOpenDate = string.Empty;
            string strCardCloseDate = string.Empty;

            if (!string.IsNullOrEmpty(memberNode.Element("MemberDetails").Attribute("CardType").Value))
            {
                cardType = memberNode.Element("MemberDetails").Attribute("CardType").Value;
                strCardOpenDate = memberNode.Element("MemberDetails").Attribute("CardOpendate").Value;
                strCardCloseDate = memberNode.Element("MemberDetails").Attribute("CardClosedate").Value;
            }
            string smsOptInDate = memberNode.Element("MemberDetails").Attribute("SMSOptInDate").Value;
            string strState = memberNode.Element("MemberDetails").Attribute("StateOrProvince").Value;
            string strCountryCode = memberNode.Element("MemberDetails").Attribute("Country").Value;
            string strLanguagePref = memberNode.Element("MemberDetails").Attribute("LanguagePreference").Value;
            string strBirthDate = memberNode.Attribute("BirthDate").Value;
            string strEmailAddress = memberNode.Element("MemberDetails").Attribute("EmailAddress").Value;
            string strHomeStoreNmbr = memberNode.Element("MemberDetails").Attribute("HomeStoreId").Value;
            string strExtendedPlayCode = memberNode.Element("MemberDetails").Attribute("ExtendedPlayCode") == null ? string.Empty : memberNode.Element("MemberDetails").Attribute("ExtendedPlayCode").Value;
            Member member = LWDataServiceUtil.DataServiceInstance(true).LoadMemberFromLoyaltyID(loyaltyNumber.ToString());
            IList<IClientDataObject> mbrDetails = member.GetChildAttributeSets("MemberDetails");
            MemberDetails memberDetails = (MemberDetails)mbrDetails[0];

            //Default EmployeeCode to 0
            memberNode.Element("MemberDetails").Add(new XAttribute("EmployeeCode", "0"));


            // AEO-Redesing-2015 Begin
            //Set AITUpdate flag
            memberNode.Element("MemberDetails").Add(new XAttribute("AITUpdate", true));

            // Set AutoIssueReward flag - added for redesign project
            memberNode.Element("MemberDetails").Add(new XAttribute("AutoIssueReward", "1"));

            // Set PendingEmailVerification flag - added for redesign project
            memberNode.Element("MemberDetails").Add(new XAttribute("PendingEmailVerification", "1" ));

            // Set PendingCellVerification flag - added for redesign project
            memberNode.Element("MemberDetails").Add(new XAttribute("PendingCellVerification", "1"));

            memberNode.Element("MemberDetails").Add(new XAttribute("NextEmailReminderDate", DateTime.Now.Date));
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "NextEmailReminderDate-->" + memberNode.Element("MemberDetails").Attribute("NextEmailReminderDate").Value);

            // AEO-Redesing-2015 End

            // Set PassValidation and EmailAddressMailable Flags
            if (!string.IsNullOrEmpty(strEmailAddress))
            {
                if (Utilities.IsEmailValid(strEmailAddress))
                {
                    memberNode.Element("MemberDetails").Add(new XAttribute("PassValidation", true));
                    memberNode.Element("MemberDetails").Add(new XAttribute("EmailAddressMailable", true));
                }
                else
                {
                    memberNode.Element("MemberDetails").Add(new XAttribute("PassValidation", false));
                    memberNode.Element("MemberDetails").Add(new XAttribute("EmailAddressMailable", false));
                    memberNode.Element("MemberDetails").SetAttributeValue("EmailAddress", string.Empty); // AEO-377 - EHP
                }
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("IsPilot {0}", isPilotMember));
            // added for redesign project
            if (isPilotMember)
            {
                 // memberNode.Element("MemberDetails").Add(new XAttribute("PassValidation", false));
                // Set ExtendedPlayCode
                memberNode.Element("MemberDetails").Add(new XAttribute("ExtendedPlayCode", "1"));
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Setting CardType: {0}", cardType));
            // Set card type code
            DateTime dateTime;
            if (!string.IsNullOrEmpty(cardType))
            {
                //RGK - 2015 Program Redesign
                if (!string.IsNullOrEmpty(strCardCloseDate))
                {
                    memberNode.Element("MemberDetails").SetAttributeValue("CardType", string.Empty);

                    // EHP  -   Set ExtendtedPlayCode for Point Conversion
                    Utilities.SetMemberExtendedPlayCode(strHomeStoreNmbr, strExtendedPlayCode, ref memberNode);
                }
                else
                {
                    IList<RefCardType> refCards = Utilities.GetCardType();
                    if (refCards != null)
                    {
                        RefCardType objCard = refCards.Where(p => p.ClientCardType == cardType).FirstOrDefault();
                        if (objCard != null)
                        {
                            memberNode.Element("MemberDetails").SetAttributeValue("CardType", objCard.CardTypeCode.ToString());

                            // EHP  -   Set ExtendtedPlayCode for Point Conversion
                            Utilities.SetMemberExtendedPlayCode(strHomeStoreNmbr, strExtendedPlayCode, ref memberNode);
                        }
                    }
                }
                //RGK - 2015 Program Redesign
            }

            // Set SMSOptIn
            if (!string.IsNullOrEmpty(smsOptInDate))
            {
                memberNode.Element("MemberDetails").Add(new XAttribute("SMSOptIn", "1"));
            }


            // Validating for valid state
            StateValidation objState = new StateValidation();
            if (objState.StateIsValid(strState, strCountryCode))
            {
                string countryCode = objState.GetCountryByState(strState);
                memberNode.Element("MemberDetails").SetAttributeValue("Country", countryCode);
            }

            // Set language preference for member details
            if (string.IsNullOrEmpty(strLanguagePref))
            {
                if (strState == "QC")
                {
                    memberNode.Element("MemberDetails").SetAttributeValue("LanguagePreference", (int)LanguagePref.French);
                }
                else
                {
                    memberNode.Element("MemberDetails").SetAttributeValue("LanguagePreference", (int)LanguagePref.English);
                }
            }
            // Set HHKey and changedby
            memberNode.Element("MemberDetails").Add(new XAttribute("HHKey", LoyaltyCard.GetNextHHKey()));
            memberNode.Element("MemberDetails").Add(new XAttribute("ChangedBy", "Enrollment DAP"));

            // Validating for underage
            if (DateTime.TryParse(strBirthDate, out dateTime))
            {
                TimeSpan ts = DateTime.Now.Date.Subtract(dateTime);
                int yrs = ts.Days / 365;
                if (yrs < 13)
                {
                    memberNode.Element("MemberDetails").Add(new XAttribute("IsUnderAge", "True"));
                }
                else
                {
                    memberNode.Element("MemberDetails").Add(new XAttribute("IsUnderAge", "False"));
                }
            }
            else
            {
                memberNode.Element("MemberDetails").Add(new XAttribute("IsUnderAge", "False"));
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        private int UpdateRecord(ref XElement memberNode, Member currentMember)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            string cardType = string.Empty;
            string strCardOpenDate = string.Empty;
            string strCardCloseDate = string.Empty;

            if (!string.IsNullOrEmpty(memberNode.Element("MemberDetails").Attribute("CardType").Value))
            {
                cardType = memberNode.Element("MemberDetails").Attribute("CardType").Value;
                strCardOpenDate = memberNode.Element("MemberDetails").Attribute("CardOpendate").Value;
                strCardCloseDate = memberNode.Element("MemberDetails").Attribute("CardClosedate").Value;
            }
            string smsOptInDate = memberNode.Element("MemberDetails").Attribute("SMSOptInDate").Value;
            string strAddress1 = memberNode.Element("MemberDetails").Attribute("AddressLineOne").Value;
            string strAddress2 = memberNode.Element("MemberDetails").Attribute("AddressLineTwo").Value;
            string strState = memberNode.Element("MemberDetails").Attribute("StateOrProvince").Value;
            string strCountryCode = memberNode.Element("MemberDetails").Attribute("Country").Value;
            string strFirstName = memberNode.Attribute("FirstName").Value;
            string strLastName = memberNode.Attribute("LastName").Value;
            string strLoyaltyid = memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value;
            string strEmailAddress = memberNode.Element("MemberDetails").Attribute("EmailAddress").Value;
            string strHomeStoreNmbr = memberNode.Element("MemberDetails").Attribute("HomeStoreId").Value;
            string strExtendedPlayCode = memberNode.Element("MemberDetails").Attribute("ExtendedPlayCode") == null ? string.Empty : memberNode.Element("MemberDetails").Attribute("ExtendedPlayCode").Value;
            int mailableFlag = 1;

            IList<IClientDataObject> mbrDetails = currentMember.GetChildAttributeSets("MemberDetails");
            MemberDetails memberDetails = (MemberDetails)mbrDetails[0];

            string pattern = "^[0-9a-zA-Z \\.,'#/-]{1,50}$";

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Validate Loyalty number: " + ((strLoyaltyid == null) ? "is null" : strLoyaltyid));

            // Validating Loyalty number
            if (string.IsNullOrEmpty(strLoyaltyid))
            {
                throw new Exception("Invalid Loyalty number.");
            }
            else
            {
                if (!LoyaltyCard.IsLoyaltyNumberValid(long.Parse(strLoyaltyid)))
                {
                    throw new Exception("Invalid Loyalty number.");
                }
            }
            memberNode.Element("MemberDetails").SetAttributeValue("ChangedBy", "GE Enrollment DAP");

            //Set AITUpdate flag
            memberNode.Element("MemberDetails").Add(new XAttribute("AITUpdate", true));


            // Set card type code
            DateTime dateTime;
            if (!string.IsNullOrEmpty(cardType))
            {
                //RGK - 2015 Program Redesign
                IList<RefCardType> refCards = Utilities.GetCardType();
                if (!string.IsNullOrEmpty(strCardCloseDate))
                {
                    if (memberDetails.CardType == (long)CardType.AECCAndAEVisaMember)
                    {
                        if (cardType == "P")
                        {
                            memberNode.Element("MemberDetails").SetAttributeValue("CardType", "2");
                        }
                        else
                        {
                            memberNode.Element("MemberDetails").SetAttributeValue("CardType", "1");
                        }
                    }
                    else
                    {
                        memberNode.Element("MemberDetails").SetAttributeValue("CardType", string.Empty);
                    }

                    // EHP  -   Set ExtendtedPlayCode for Point Conversion
                    Utilities.SetMemberExtendedPlayCode(strHomeStoreNmbr, strExtendedPlayCode, ref memberNode);
                }
                else
                {
                    if (refCards != null)
                    {
                        RefCardType objCard = refCards.Where(p => p.ClientCardType == cardType).FirstOrDefault();
                        if (objCard != null)
                        {
                            if ((memberDetails.CardType == (long)CardType.AECCMember && cardType == "D") || (memberDetails.CardType == (long)CardType.AEVisaMember && cardType == "P"))
                            {
                                memberNode.Element("MemberDetails").SetAttributeValue("CardType", (long)CardType.AECCAndAEVisaMember);
                            }
                            else
                            {
                                memberNode.Element("MemberDetails").SetAttributeValue("CardType", objCard.CardTypeCode.ToString());
                            }

                            // EHP  -   Set ExtendtedPlayCode for Point Conversion
                            Utilities.SetMemberExtendedPlayCode(strHomeStoreNmbr, strExtendedPlayCode, ref memberNode);
                        }
                    }
                }
                //RGK - 2015 Program Redesign
            }

            return mailableFlag;
        }



        /// <summary>
        /// Method to validate record
        /// </summary>
        /// <param name="memberNode">ref XElement memberNode</param>
        /// <returns>return sMailableFlag;</returns>
        private int ValidateRecord(ref XElement memberNode)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "AddressLineOne: " + memberNode.Element("MemberDetails").Attribute("AddressLineOne").Value);

            string cardType = string.Empty;
            string strCardOpenDate = string.Empty;
            string strCardCloseDate = string.Empty;

            if (!string.IsNullOrEmpty(memberNode.Element("MemberDetails").Attribute("CardType").Value))
            {
                cardType = memberNode.Element("MemberDetails").Attribute("CardType").Value;
                strCardOpenDate = memberNode.Element("MemberDetails").Attribute("CardOpendate").Value;
                strCardCloseDate = memberNode.Element("MemberDetails").Attribute("CardClosedate").Value;
            }
            string smsOptInDate = memberNode.Element("MemberDetails").Attribute("SMSOptInDate").Value;
            string strAddress1 = memberNode.Element("MemberDetails").Attribute("AddressLineOne").Value;
            string strAddress2 = memberNode.Element("MemberDetails").Attribute("AddressLineTwo").Value;
            string strState = memberNode.Element("MemberDetails").Attribute("StateOrProvince").Value;
            string strCountryCode = memberNode.Element("MemberDetails").Attribute("Country").Value;
            string strFirstName = memberNode.Attribute("FirstName").Value;
            string strLastName = memberNode.Attribute("LastName").Value;
            string strLoyaltyid = memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value;
            string strEmailAddress = memberNode.Element("MemberDetails").Attribute("EmailAddress").Value;
            string strHomeStoreNmbr = memberNode.Element("MemberDetails").Attribute("HomeStoreId").Value;
            string strExtendedPlayCode = memberNode.Element("MemberDetails").Attribute("ExtendedPlayCode") == null ? string.Empty : memberNode.Element("MemberDetails").Attribute("ExtendedPlayCode").Value;
            int mailableFlag = 1;

            string pattern = "^[0-9a-zA-Z \\.,'#/-]{1,50}$";


            // added for redesign project
            bool isPilotMember = Utilities.MemberIsInPilot(memberNode.Element("MemberDetails").Attribute("ZipOrPostalCode").Value);

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("IsPilot {0}", isPilotMember));
            // added for redesign project
            if (isPilotMember)
            {
                // memberNode.Element("MemberDetails").Add(new XAttribute("PassValidation", false));
                // Set ExtendedPlayCode
                memberNode.Element("MemberDetails").Add(new XAttribute("ExtendedPlayCode", "1"));
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Validate name: ");
            // Validating first name and last name
            if (string.IsNullOrEmpty(strFirstName) || string.IsNullOrEmpty(strLastName))
            {
                throw new Exception("Name is invalid.");
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Validate Loyalty number: "+( (strLoyaltyid == null) ? "is null" : strLoyaltyid));
            
            // Validating Loyalty number
            if (string.IsNullOrEmpty(strLoyaltyid))
            {
                throw new Exception("Invalid Loyalty number.");
            }
            else
            {
                if (!LoyaltyCard.IsLoyaltyNumberValid(long.Parse(strLoyaltyid)))
                {
                    throw new Exception("Invalid Loyalty number.");
                }
            }
            // Set HHKey and changedby
            memberNode.Element("MemberDetails").SetAttributeValue("HHKey", LoyaltyCard.GetNextHHKey());
            memberNode.Element("MemberDetails").SetAttributeValue("ChangedBy", "Enrollment DAP");

            //Set AITUpdate flag
            memberNode.Element("MemberDetails").Add(new XAttribute("AITUpdate", true));

            // AEO-2015-Redesign Begin
            
            // Set AutoIssueReward flag - added for redesign project
            memberNode.Element("MemberDetails").Add(new XAttribute("AutoIssueReward", "1"));

            // Set PendingEmailVerification flag - added for redesign project
            memberNode.Element("MemberDetails").Add(new XAttribute("PendingEmailVerification", "1"));

            // Set PendingCellVerification flag - added for redesign project
            memberNode.Element("MemberDetails").Add(new XAttribute("PendingCellVerification", "1"));

            memberNode.Element("MemberDetails").Add(new XAttribute("NextEmailReminderDate", DateTime.Now.Date));
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "NextEmailReminderDate-->" + memberNode.Element("MemberDetails").Attribute("NextEmailReminderDate").Value);
            

            // AEO-2015-Redesign End

            // Set PassValidation and EmailAddressMailable Flags
            if (!string.IsNullOrEmpty(strEmailAddress))
            {
                if (Utilities.IsEmailValid(strEmailAddress))
                {
                    memberNode.Element("MemberDetails").Add(new XAttribute("PassValidation", true));
                    memberNode.Element("MemberDetails").Add(new XAttribute("EmailAddressMailable", true));
                }
                else
                {
                    memberNode.Element("MemberDetails").Add(new XAttribute("PassValidation", false));
                    memberNode.Element("MemberDetails").Add(new XAttribute("EmailAddressMailable", false));
                    memberNode.Element("MemberDetails").SetAttributeValue("EmailAddress", string.Empty);  // AEO-377 - EHP 
                }
            }
            // Set SMSOptIn
            if (!string.IsNullOrEmpty(smsOptInDate))
            {
                memberNode.Element("MemberDetails").Add(new XAttribute("SMSOptIn", "1"));
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, string.Format("Setting CardType: {0}", cardType));
            // Set card type code
            DateTime dateTime;
            if (!string.IsNullOrEmpty(cardType))
            {
                //RGK - 2015 Program Redesign
                if (!string.IsNullOrEmpty(strCardCloseDate))
                {
                    memberNode.Element("MemberDetails").SetAttributeValue("CardType", string.Empty);

                    // EHP  -   Set ExtendtedPlayCode for Point Conversion
                    Utilities.SetMemberExtendedPlayCode(strHomeStoreNmbr, strExtendedPlayCode, ref memberNode);
                }
                else
                {
                    IList<RefCardType> refCards = Utilities.GetCardType();
                    if (refCards != null)
                    {
                        RefCardType objCard = refCards.Where(p => p.ClientCardType == cardType).FirstOrDefault();
                        if (objCard != null)
                        {
                            memberNode.Element("MemberDetails").SetAttributeValue("CardType", objCard.CardTypeCode.ToString());

                            // EHP  -   Set ExtendtedPlayCode for Point Conversion
                            Utilities.SetMemberExtendedPlayCode(strHomeStoreNmbr, strExtendedPlayCode, ref memberNode);
                        }
                    }
                }
                //RGK - 2015 Program Redesign
            }



            return mailableFlag;
        }


        #endregion

    }
}
