// ----------------------------------------------------------------------------------
// <copyright file="AddMemberInterceptor.cs" company="Brierley and Partners">
//     Copyright statement. All right reserved
// </copyright>
// ----------------------------------------------------------------------------------

namespace AmericanEagle.SDK.Interceptors
{
    #region | Name Space |
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Collections.Generic;

    using Brierley.Clients.AmericanEagle.DataModel;
    using AmericanEagle.SDK.Global;
    using Brierley.FrameWork.Common;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
    //using Brierley.LoyaltyWare.LWIntegration.Common;
    using Brierley.FrameWork.LWIntegration;
    // using Brierley.LoyaltyWare.LWIntegration.Common.Exceptions;
    using Brierley.FrameWork.Common.Exceptions;
    using Brierley.ClientDevUtilities.LWGateway;

    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
    #endregion

    /// <summary>
    /// Class AddMemberInterceptor
    /// </summary>
    public class AddMemberInterceptor : AmericanEagleInboundInterceptorBase
    {
        /// <summary>
        /// Stores Class Name
        /// </summary>
        private const string ClassName = "AddMemberInterceptor";

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(ClassName);
        private bool hasSMSOptedIn = false;
        private bool hasValidMobilePhone = false;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// Process member before population
        /// </summary>
        /// <param name="config">LWIntegrationConfig config</param>
        /// <param name="member">Member member</param>
        /// <param name="memberNode">XElement memberNode</param>
        /// <returns>Member object</returns>
        public override Member ProcessMemberBeforePopulation(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;

            // Tracing for starting of method
            this.logger.Trace(ClassName, methodName, "Starts");

            try
            {
                // Validation against API 2.0.2
                // Validate FirstName
                if ((memberNode.Attribute("FirstName") == null) || (memberNode.Attribute("FirstName").Value.Trim().Length == 0))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.FirstNameRequired)) { ErrorCode = (int)ResponseCode.FirstNameRequired };
                }

                // AEO-500 Begin
                string strValue = memberNode.Attribute("FirstName").Value;
                if (!Utilities.IsNameValid(ref strValue))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidFirstName)) { ErrorCode = (int)ResponseCode.InvalidFirstName };
                }
                memberNode.Attribute("FirstName").SetValue(strValue);

                // AEO-500 End

                // Validate LastName
                if ((memberNode.Attribute("LastName") == null) || (memberNode.Attribute("LastName").Value.Trim().Length == 0))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.LastNameRequired)) { ErrorCode = (int)ResponseCode.LastNameRequired };
                }

                // AEO-500 Begin
                 strValue = memberNode.Attribute("LastName").Value;
                if (!Utilities.IsNameValid(ref strValue))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidLastName)) { ErrorCode = (int)ResponseCode.InvalidLastName };
                }

                memberNode.Attribute("LastName").SetValue(strValue);
                // AEO-500 End

                // Validate BirthDate
                if ((memberNode.Attribute("BirthDate") == null) || (memberNode.Attribute("BirthDate").Value.Trim().Length == 0))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.BirthDateRequired)) { ErrorCode = (int)ResponseCode.BirthDateRequired };
                }

                this.logger.Trace(ClassName, methodName, " memberNode.Attribute('BirthDate') = " + memberNode.Attribute("BirthDate").Value );

                DateTime birthDate = DateTime.MaxValue;
                if (!DateTime.TryParseExact(Convert.ToDateTime(memberNode.Attribute("BirthDate").Value).ToString("MM/dd/yyyy"), "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out birthDate))
                {
                    this.logger.Trace(ClassName, methodName, " memberNode.Attribute('BirthDate') = " + memberNode.Attribute("BirthDate").Value + " is not valid.");
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidBirthDate)) { ErrorCode = (int)ResponseCode.InvalidBirthDate };
                }

                // Check Birthdate must be at least 13 years prior to current date 
                this.logger.Trace(ClassName, methodName, " birthdate = " + birthDate.ToString ("MM/dd/yyyy"));

                TimeSpan diff = DateTime.Now - birthDate;
                int years = diff.Days / 366;
                DateTime workingDate = birthDate.AddYears(years);
                while (workingDate.AddYears(1) <= DateTime.Now)
                {
                    workingDate = workingDate.AddYears(1);
                    years++;
                    this.logger.Trace(ClassName, methodName, " workingdate = " + workingDate.ToString("MM/dd/yyyy"));

                }

                if (years < 13)
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidBirthDate)) { ErrorCode = (int)ResponseCode.InvalidBirthDate };
                }

                if (memberNode.Element("MemberDetails") == null)
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.AddressLine1Required)) { ErrorCode = (int)ResponseCode.AddressLine1Required };
                }

                // Validate EmailAddress
                if ((memberNode.Element("MemberDetails").Attribute("EmailAddress") == null) || (memberNode.Element("MemberDetails").Attribute("EmailAddress").Value.Trim().Length == 0))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.EmailAddressRequired)) { ErrorCode = (int)ResponseCode.EmailAddressRequired };
                }

                if (!Utilities.IsEmailValid(memberNode.Element("MemberDetails").Attribute("EmailAddress").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidEmailAddress)) { ErrorCode = (int)ResponseCode.InvalidEmailAddress };
                }

                // Validate Address1
                if ((memberNode.Element("MemberDetails").Attribute("AddressLineOne") == null) || (memberNode.Element("MemberDetails").Attribute("AddressLineOne").Value.Trim().Length == 0))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.AddressLine1Required)) { ErrorCode = (int)ResponseCode.AddressLine1Required };
                }

                if (!Utilities.IsAddressValid(memberNode.Element("MemberDetails").Attribute("AddressLineOne").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidAddressLine1)) { ErrorCode = (int)ResponseCode.InvalidAddressLine1 };
                }

                // Validate Address2
                if ((memberNode.Element("MemberDetails").Attribute("AddressLineTwo") != null) && (memberNode.Element("MemberDetails").Attribute("AddressLineTwo").Value.Trim().Length > 0))
                {
                    if (!Utilities.IsAddressValid(memberNode.Element("MemberDetails").Attribute("AddressLineTwo").Value))
                    {
                        throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidAddressLine2)) { ErrorCode = (int)ResponseCode.InvalidAddressLine2 };
                    }
                }

                // Validate City
                if ((memberNode.Element("MemberDetails").Attribute("City") == null) || (memberNode.Element("MemberDetails").Attribute("City").Value.Trim().Length == 0))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.CityRequired)) { ErrorCode = (int)ResponseCode.CityRequired };
                }

                if (!Utilities.IsCityValid(memberNode.Element("MemberDetails").Attribute("City").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidCity)) { ErrorCode = (int)ResponseCode.InvalidCity };
                }

                // Validate State
                if ((memberNode.Element("MemberDetails").Attribute("StateOrProvince") == null) || (memberNode.Element("MemberDetails").Attribute("StateOrProvince").Value.Trim().Length == 0))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.StateRequired)) { ErrorCode = (int)ResponseCode.StateRequired };
                }

                memberNode.Element("MemberDetails").SetAttributeValue("StateOrProvince", memberNode.Element("MemberDetails").Attribute("StateOrProvince").Value.Trim().ToUpper());
                if (!Utilities.IsStateValid(memberNode.Element("MemberDetails").Attribute("StateOrProvince").Value, memberNode.Element("MemberDetails").Attribute("Country").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidState)) { ErrorCode = (int)ResponseCode.InvalidState };
                }

                // Validate PostalCode
                if ((memberNode.Element("MemberDetails").Attribute("ZipOrPostalCode") == null) || (memberNode.Element("MemberDetails").Attribute("ZipOrPostalCode").Value.Trim().Length == 0))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.PostalCodeRequired)) { ErrorCode = (int)ResponseCode.PostalCodeRequired };
                }

                if (!Utilities.IsPostalCodeValid(memberNode.Element("MemberDetails").Attribute("ZipOrPostalCode").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidPostalCode)) { ErrorCode = (int)ResponseCode.InvalidPostalCode };
                }

                // Validate CountryCode
                if ((memberNode.Element("MemberDetails").Attribute("Country") == null) || (memberNode.Element("MemberDetails").Attribute("Country").Value.Trim().Length == 0))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.CountryCodeRequired)) { ErrorCode = (int)ResponseCode.CountryCodeRequired };
                }

                memberNode.Element("MemberDetails").SetAttributeValue("Country", memberNode.Element("MemberDetails").Attribute("Country").Value.Trim().ToUpper());
                if (!Utilities.IsCountryCodeValid(memberNode.Element("MemberDetails").Attribute("Country").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidCountryCode)) { ErrorCode = (int)ResponseCode.InvalidCountryCode };
                }

                // Validate BaseBrand
                if (memberNode.Element("MemberDetails").Attribute("BaseBrandID").Value.Trim().Length == 0)
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.BaseBrandRequired)) { ErrorCode = (int)ResponseCode.BaseBrandRequired };
                }

                if (!Utilities.IsBaseBrandValid(memberNode.Element("MemberDetails").Attribute("BaseBrandID").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidBaseBrand)) { ErrorCode = (int)ResponseCode.InvalidBaseBrand };
                }
                // Set Language Preference
                if (memberNode.Element("MemberDetails").Attribute("StateOrProvince").Value == "QC")
                {
                    memberNode.Element("MemberDetails").SetAttributeValue("LanguagePreference", (int)LanguagePref.French);
                }
                else
                {
                    memberNode.Element("MemberDetails").SetAttributeValue("LanguagePreference", (int)LanguagePref.English);
                }

                // Set EmailAddressMailable
                memberNode.Element("MemberDetails").SetAttributeValue("BaseBrandID", Definitions.GetBaseBrandId(memberNode.Element("MemberDetails").Attribute("BaseBrandID").Value));

                // PI#30016 , Rizwan, Set MemberSource, Start
                if (memberNode.Element("MemberDetails").Attribute("GwLinked") != null && memberNode.Element("MemberDetails").Attribute("GwLinked").Value == "1")
                {
                    memberNode.Element("MemberDetails").SetAttributeValue("MemberSource", (int)MemberSource.GWLinked);
                }
                else
                {
                    memberNode.Element("MemberDetails").SetAttributeValue("MemberSource", (int)MemberSource.OnlineAEEnrolled);
                }
                // PI#30016 , Rizwan, Set MemberSource, End

                // Set EmailAddressMailable
                memberNode.Element("MemberDetails").SetAttributeValue("EmailAddressMailable", true);

                // Set PassValidation
                memberNode.Element("MemberDetails").SetAttributeValue("PassValidation", true);

                // Set AddressMailable
                memberNode.Element("MemberDetails").SetAttributeValue("AddressMailable", true);

                // Set EmailOptIn
                memberNode.Element("MemberDetails").SetAttributeValue("EmailOptIn", true);

                // Set DirectMailOptIn
                memberNode.Element("MemberDetails").SetAttributeValue("DirectMailOptIn", true);

                // Set EmailOptInDate
                memberNode.Element("MemberDetails").SetAttributeValue("EmailOptInDate", DateTime.Now);

                //PI17022 - Validate MobilePhone    
                hasValidMobilePhone = false;
                if (memberNode.Element("MemberDetails").Attribute("MobilePhone") != null && memberNode.Element("MemberDetails").Attribute("MobilePhone").Value.Trim().Length > 0)
                {
                    if (!Utilities.IsPhoneValid(memberNode.Element("MemberDetails").Attribute("MobilePhone").Value))
                    {
                        throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidMobilePhone)) { ErrorCode = (int)ResponseCode.InvalidMobilePhone };
                    }
                    else
                    {
                        //Set mobile phone
                        memberNode.Element("MemberDetails").SetAttributeValue("MobilePhone", memberNode.Element("MemberDetails").Attribute("MobilePhone").Value.Trim());
                        hasValidMobilePhone = true;
                    }
                }

                //PI17022 - SMSOptin can be set only if vaid MobilePhone exists
                hasSMSOptedIn = false;
                if (memberNode.Element("MemberDetails").Attribute("SMSOptIn") != null && (Convert.ToBoolean(memberNode.Element("MemberDetails").Attribute("SMSOptIn").Value)))
                {
                    if (!hasValidMobilePhone)
                    {
                        throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.MobilePhoneNumberRequiredForSMSOptIn)) { ErrorCode = (int)ResponseCode.MobilePhoneNumberRequiredForSMSOptIn };
                    }
                    else
                    {
                        hasSMSOptedIn = true;
                        memberNode.Element("MemberDetails").SetAttributeValue("SMSOptInDate", DateTime.Now);
                    }
                }

                // Set DirectMailOptInDate
                memberNode.Element("MemberDetails").SetAttributeValue("DirectMailOptInDate", DateTime.Now);

                // Set HHKey
                memberNode.Element("MemberDetails").SetAttributeValue("HHKey", LoyaltyCard.GetNextHHKey());

                // Set AITUpdate flag
                memberNode.Element("MemberDetails").SetAttributeValue("AITUpdate", true);

                // Set AutoIssueReward flag - added for redesign project
                memberNode.Element("MemberDetails").SetAttributeValue("AutoIssueReward", 1);

                // Set MemberStatus - added for redesign project
                //memberNode.Attribute("MemberStatus").SetValue(MemberStatus.Locked);

                // Set PendingEmailVerification flag - added for redesign project
                memberNode.Element("MemberDetails").SetAttributeValue("PendingEmailVerification", 1);

                // Set PendingCellVerification flag - added for redesign project
                memberNode.Element("MemberDetails").SetAttributeValue("PendingCellVerification", 1);

                // added for redesign project

                this.logger.Trace(ClassName, methodName, "NextEmailReminderDate Begin");
                memberNode.Element("MemberDetails").SetAttributeValue("NextEmailReminderDate", DateTime.Now.Date.AddDays(1));// AEO-Redesign 2015 Begin & End
                this.logger.Trace(ClassName, methodName, "NextEmailReminderDate End");

                string tempLoyaltyIdNumber = "";
                bool isPilotMember = Utilities.MemberIsInPilot(memberNode.Element("MemberDetails").Attribute("ZipOrPostalCode").Value);
                if (isPilotMember)
                {
                    tempLoyaltyIdNumber = Convert.ToString(LoyaltyCard.GetNextLoyaltyNumber(LoyaltyCard.LoyaltyCardType.AE));
                                   
                }
                else
                {
                    tempLoyaltyIdNumber = Convert.ToString(LoyaltyCard.GetNextLoyaltyNumber(LoyaltyCard.LoyaltyCardType.Temporary));
                }

                // Creating new virtual card
                VirtualCard memberVirtualCard = member.CreateNewVirtualCard();
                memberVirtualCard.DateIssued = DateTime.Now;
                memberVirtualCard.IsPrimary = true;
                memberVirtualCard.LoyaltyIdNumber = tempLoyaltyIdNumber;

                // Adding card for replacement 
                MemberCardReplacements replaceCards = new MemberCardReplacements();
                // added for redesign project
                if (isPilotMember)
                {
                    replaceCards.StatusCode = (long)CardReplaceStatus.SendToAE;
                    replaceCards.LoyaltyIDNumber = tempLoyaltyIdNumber;
                    replaceCards.IsTemporary = true;
                    replaceCards.CHANGEDBY = "AddMember api";
                    member.AddChildAttributeSet(replaceCards);
                    // Set PassValidation 
                    // memberNode.Element("MemberDetails").SetAttributeValue("PassValidation", false);
                    // Set ExtendedPlayCode
                    memberNode.Element("MemberDetails").SetAttributeValue("ExtendedPlayCode", 1);
                }
                else
                {
                    replaceCards.StatusCode = (long)CardReplaceStatus.ScheduleForReplacement;
                    replaceCards.LoyaltyIDNumber = tempLoyaltyIdNumber;
                    replaceCards.IsTemporary = true;
                    replaceCards.CHANGEDBY = "AddMember api";
                    member.AddChildAttributeSet(replaceCards);
                    replaceCards = new MemberCardReplacements();
                    replaceCards.StatusCode = (long)CardReplaceStatus.Original;
                    replaceCards.LoyaltyIDNumber = tempLoyaltyIdNumber;
                    replaceCards.CHANGEDBY = "AddMember api";
                    member.AddChildAttributeSet(replaceCards);
                }
            }
            catch (LWIntegrationCfgException configEx)
            {
                throw configEx;
            }
            catch (Exception ex)
            {
                logger.Error(ClassName, methodName, ex.Message);
                throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.TechnicalDifficulties)) { ErrorCode = (int)ResponseCode.TechnicalDifficulties };
            }

            // Tracing for ending of method
            this.logger.Trace(ClassName, methodName, "Ends");

            return member;
        }

        public override Member ProcessMemberBeforeSave(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            this.logger.Trace(ClassName, methodName, "Begin");

            IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
            MemberDetails memberDetails = (MemberDetails)mbrDtlObjs[0];
            this.logger.Trace(ClassName, methodName, "Update Member Primary PostalCode to " + memberDetails.ZipOrPostalCode);
            // Trim dashes and spaces from zip code and then copy to primary postal code
            memberDetails.ZipOrPostalCode = memberDetails.ZipOrPostalCode.Replace("-", "").Replace(" ", "");
            member.PrimaryPostalCode = memberDetails.ZipOrPostalCode;
            if ((member.PrimaryEmailAddress != null) && (member.PrimaryEmailAddress.Length > 0))
            {
                memberDetails.EmailAddress = member.PrimaryEmailAddress;
                member.PrimaryEmailAddress = null;
            }

            this.logger.Trace(ClassName, methodName, "Get Next HHKey ");
            member.ChangedBy = "AddMember api";
            memberDetails.ChangedBy = "AddMember api";



            this.logger.Trace(ClassName, methodName, "End");
            return member;
        }
        /// <summary>
        /// Process member after save
        /// </summary>
        /// <param name="config">LWIntegrationConfig config</param>
        /// <param name="member">Member member</param>
        /// <param name="memberNode">XElement memberNode</param>
        /// <returns>Member object</returns>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public override Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode)
        public override Member ProcessMemberAfterSave(LWIntegrationConfig config, Member member, XElement memberNode, IList<Brierley.FrameWork.ContextObject.RuleResult> results = null)
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            this.logger.Trace(ClassName, methodName, "Starts");

            // Getting MemberDetails for Member object
            MemberDetails memberDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
            if (null != memberDetails)
            {
                // Add Email Bonus
                bool blnHasEmailBonusCredit = false;
                if (null != memberDetails.HasEmailBonusCredit)
                {
                    blnHasEmailBonusCredit = memberDetails.HasEmailBonusCredit.Value;
                }

                // added for redesign project
                bool isPilotMember = Utilities.MemberIsInPilot(memberDetails.ZipOrPostalCode);
                
                VirtualCard virtualCard = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                if (!String.IsNullOrEmpty(memberDetails.EmailAddress) && !blnHasEmailBonusCredit && !isPilotMember)
                {
                    this.logger.Trace(ClassName, methodName, "email bonus ");
                    Utilities.AddEmailBonus(virtualCard, memberDetails);
                }

                //PI17022 - Add smsbouns
                if (hasSMSOptedIn && hasValidMobilePhone && !isPilotMember)
                {
                    Utilities.AddSMSBonus(virtualCard, memberDetails);
                }

                // PI30016 , Rizwan, Add Google Wallet Bonus, Start
                int? intGWLinked = memberDetails.GwLinked;
                int? intHasGWBonusCredit = memberDetails.HasGWBonusCredit;
                if (intGWLinked.HasValue && intGWLinked.Value == 1 && (!intHasGWBonusCredit.HasValue || intHasGWBonusCredit.Value != 1))
                {
                    Utilities.AddGWBonus(virtualCard, memberDetails);
                }
                // PI30016 , Rizwan, Add Google Wallet Bonus, End

                if (isPilotMember)
                {
                    member.AddTier("Blue", DateTime.Today, DateTime.Parse("12/31/2199"), "Base");
                }
            }
            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                lwService.SaveMember(member);
            }
            Merge.AddMemberProactiveMerge(member);

            this.logger.Trace(ClassName, methodName, "Ends");
            return member;
        }
    }
}
