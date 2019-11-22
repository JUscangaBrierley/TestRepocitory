// ----------------------------------------------------------------------------------
// <copyright file="UpdateMemberInterceptor.cs" company="Brierley and Partners">
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
    //using Brierley.LoyaltyWare.LWIntegration.Common.Exceptions;
    using Brierley.FrameWork.LWIntegration;
    using Brierley.FrameWork.Common.Exceptions;
    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
using System.Collections;
    using Brierley.ClientDevUtilities.LWGateway;
    #endregion

    /// <summary>
    /// Class AddMemberInterceptor
    /// </summary>
    public class UpdateMemberInterceptor : AmericanEagleInboundInterceptorBase
    {
        /// <summary>
        /// Stores Class Name
        /// </summary>
        private string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private long previousBaseBrandId = 0;
        private int memberSourceCode = 0;
        private string emailBeforePopulate = string.Empty;
        private string oldEmailAddressBeforePopulate = string.Empty;
        // added for redesign project
        private string cellBeforePopulate = string.Empty;

        /// <summary>
        /// Reference to logger 
        /// </summary>
        private LWLogger logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        /// <summary>
        /// Boolean variable to store current members SMSOptIn before save value
        /// </summary>
        private bool preSMSOptIn;
        private bool isNameAndAddresChanged;
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// Private class to hold backup of loyalty card with IsPrimaryStatus
        /// </summary>
        private class LoyaltyCardBackup
        {
            public string LoyaltyIdNumber { get; set; }
            public bool IsPrimary { get; set; }
        }
        List<LoyaltyCardBackup> cardsBackup = null;

        /// <summary>
        /// Load Member events
        /// </summary>
        /// <param name="config">LWIntegrationConfig config</param>
        /// <param name="memberNode">XElement memberNode</param>
        /// <returns>return member list</returns>
        public override Member LoadMember(LWIntegrationConfig config, XElement memberNode)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            try
            {
                this.logger.Trace(this.className, methodName, "Starts");

                // Validates Loyalty number against API 7.0.5: The loyalty number must be present in the API call.
                if (null == memberNode.Element("VirtualCard"))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.LoyaltyNumberRequired)) { ErrorCode = (int)ResponseCode.LoyaltyNumberRequired };
                }

                if (null == memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber"))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.LoyaltyNumberRequired)) { ErrorCode = (int)ResponseCode.LoyaltyNumberRequired };
                }

                if (memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value.Trim().Length == 0)
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.LoyaltyNumberRequired)) { ErrorCode = (int)ResponseCode.LoyaltyNumberRequired };
                }

                // Validates Loyalty number against API 7.0.6-7.0.7: Loyalty number check
                long loyaltyNumberLong;
                if (long.TryParse(memberNode.Element("VirtualCard").Attribute("LoyaltyIdNumber").Value.Trim(), out loyaltyNumberLong))
                {
                    if (!LoyaltyCard.IsLoyaltyNumberValid(loyaltyNumberLong))
                    {
                        throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidLoyaltyNumber)) { ErrorCode = (int)ResponseCode.InvalidLoyaltyNumber };
                    }
                }
                else
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidLoyaltyNumber)) { ErrorCode = (int)ResponseCode.InvalidLoyaltyNumber };
                }

                Member member;
                using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    member = lwService.LoadMemberFromLoyaltyID(loyaltyNumberLong.ToString());
                }
                if (null == member)
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.LoyaltyAccountNotFound)) { ErrorCode = (int)ResponseCode.LoyaltyAccountNotFound };
                }

                // Validate member agaunst API 7.0.8: The loyalty number must not have an account status of ‘Terminated’ or ‘Archived’
                if (member.MemberStatus == MemberStatusEnum.Terminated)
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.LoyaltyAccountTerminated)) { ErrorCode = (int)ResponseCode.LoyaltyAccountTerminated };
                }

                IList<IClientDataObject> objs = member.GetChildAttributeSets("MemberDetails");

                MemberDetails memberDetails = (MemberDetails)objs[0];

                previousBaseBrandId = (long)memberDetails.BaseBrandID;

                // Get the member source before population because its gets nullify during population/save
                if (memberDetails != null && memberDetails.MemberSource.HasValue)
                {
                    memberSourceCode = memberDetails.MemberSource.Value;
                }

                // Get member email address
                if (memberDetails != null)
                {
                    this.emailBeforePopulate = memberDetails.EmailAddress;
                    this.oldEmailAddressBeforePopulate = memberDetails.OldEmailAddress;
                    // added for redesign project- Jira Ticket# AEO-160
                    if (!string.IsNullOrEmpty(memberDetails.MobilePhone))
                    {
                        this.cellBeforePopulate = memberDetails.MobilePhone;
                    }
                }

                this.cardsBackup = new List<LoyaltyCardBackup>();
                // Backup IsPrimary status of all virtual cards on loading
                foreach (Brierley.FrameWork.Data.DomainModel.VirtualCard card in member.LoyaltyCards)
                {
                    this.cardsBackup.Add(new LoyaltyCardBackup { LoyaltyIdNumber=card.LoyaltyIdNumber, IsPrimary=card.IsPrimary});
                }

                this.logger.Trace(this.className, methodName, "Ends");
                return member;
            }
            catch (LWIntegrationCfgException ex)
            {
                this.logger.Trace(this.className, methodName, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                this.logger.Trace(this.className, methodName, ex.Message);
                throw new LWIntegrationCfgException(ex.Message) { ErrorCode = 1 };
            }
        }

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
            try
            {
                this.logger.Trace(this.className, methodName, "Starts");

                // Validation against API 7.0.10: First Name, Last Name, Address Line 1, City, State, Postal Code, Country Code and Email Address should be valid
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
                if (null == memberNode.Element("MemberDetails"))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.AddressLine1Required)) { ErrorCode = (int)ResponseCode.AddressLine1Required };
                }

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

                // Validate HomePhone
                if (!Utilities.IsPhoneValid(memberNode.Element("MemberDetails").Attribute("HomePhone").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidHomePhone)) { ErrorCode = (int)ResponseCode.InvalidHomePhone };
                }

                if (Convert.ToBoolean(memberNode.Element("MemberDetails").Attribute("SMSOptIn").Value))
                {
                    if (memberNode.Element("MemberDetails").Attribute("MobilePhone").Value.Trim().Length == 0)
                    {
                        throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.MobilePhoneNumberRequiredForSMSOptIn)) { ErrorCode = (int)ResponseCode.MobilePhoneNumberRequiredForSMSOptIn };
                    }
                }

                // Validate MobilePhone
                if (!Utilities.IsPhoneValid(memberNode.Element("MemberDetails").Attribute("MobilePhone").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidMobilePhone)) { ErrorCode = (int)ResponseCode.InvalidMobilePhone };
                }

                // Validate Gender
                if (!Utilities.IsGenderValid(memberNode.Element("MemberDetails").Attribute("Gender").Value))
                {
                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidGender)) { ErrorCode = (int)ResponseCode.InvalidGender };
                }

                // Validate LanguagePreference
                if (!Utilities.IsLanguagePreferenceValid(memberNode.Element("MemberDetails").Attribute("LanguagePreference").Value))
                {
                    //RKG pi#15470 - From Gavin
                    //We need to keep it as an Unrequired field (per the SRD from Universal Registration)
                    //Validate the data coming across (0, 1, 2)
                    //Allow for blanks and nulls since it isn’t a required field (plus we confirm the db has many profiles where Lang Pref is null).  

                    throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.InvalidLanguagePreference)) { ErrorCode = (int)ResponseCode.InvalidLanguagePreference };
                }

                // Set AddressMailable against API 7.0.11: Status is updated to ‘Mailable’
                memberNode.Element("MemberDetails").SetAttributeValue("AddressMailable", true);

                // Set EmailAddressMailable against API 7.0.11: Status will be changed to ‘Deliverable’
                memberNode.Element("MemberDetails").SetAttributeValue("EmailAddressMailable", true);
                memberNode.Element("MemberDetails").SetAttributeValue("PassValidation", true);

                if (NeedsAITUpdate(member, memberNode))
                {
                    // Set AITUpdate flag
                    memberNode.Element("MemberDetails").SetAttributeValue("AITUpdate", true);
                    isNameAndAddresChanged = true;
                }

                //Need to set the BaseBrandId to what was stored in the Loadmember in case somehow AE sends us a change for the BaseBrandId
                memberNode.Element("MemberDetails").SetAttributeValue("BaseBrandId", previousBaseBrandId);

                MemberDetails memberDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                if (null != memberDetails)
                {
                    
                    // Validation against API 7.0.15: If the smsOptin field is populated with a ‘1’ and the mobilePhone value is not populated in the UpdateProfile API call, an error of ‘Mobile Phone Number Required for SMS Opt In’ shall be returned in response to the call
                    if (Convert.ToBoolean(memberNode.Element("MemberDetails").Attribute("SMSOptIn").Value))
                    {
                        if (string.IsNullOrEmpty(memberDetails.MobilePhone) && memberNode.Element("MemberDetails").Attribute("MobilePhone").Value.Trim().Length == 0)
                        {
                            throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.MobilePhoneNumberRequiredForSMSOptIn)) { ErrorCode = (int)ResponseCode.MobilePhoneNumberRequiredForSMSOptIn };
                        }

                        if (memberDetails.SMSOptIn == false || null == memberDetails.SMSOptIn || null == memberDetails.SmsOptInDate)
                        {
                            this.preSMSOptIn = false;
                            memberDetails.SMSOptIn = true;
                            memberDetails.SmsOptInDate = DateTime.Now;
                        }
                        else
                        {
                            //PI12014 - Set flag to prevent multiple smsOptin bonus
                            this.preSMSOptIn = true;
                        }
              
                    }                    

                    // Set HomePhone against API 7.0.17: Value will be removed from the Brierley database
                    if (string.IsNullOrEmpty(memberNode.Element("MemberDetails").Attribute("HomePhone").Value))
                    {
                        memberDetails.HomePhone = string.Empty;
                    }

                    // Set MobilePhone against API 7.0.17 : Value will be removed from the Brierley database
                    if (string.IsNullOrEmpty(memberNode.Element("MemberDetails").Attribute("MobilePhone").Value))
                    {
                        memberDetails.MobilePhone = string.Empty;
                    }

                    // Mobile Phone changes and sms optin so change the date
                    if (memberNode.Element("MemberDetails").Attribute("MobilePhone").Value != memberDetails.MobilePhone)
                    {
                        if (memberDetails.SMSOptIn == true)
                        {
                            memberDetails.SmsOptInDate = DateTime.Now;
                        }
                    }

                    // Set Gender against API 7.0.17: Value will be removed from the Brierley database
                    if (string.IsNullOrEmpty(memberNode.Element("MemberDetails").Attribute("Gender").Value))
                    {
                        memberDetails.Gender = string.Empty;
                    }

                }

                this.logger.Trace(this.className, methodName, "Ends");
                return member;
            }
            catch (LWIntegrationCfgException ex)
            {
                this.logger.Trace(this.className, methodName, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                this.logger.Trace(this.className, methodName, ex.Message);
                throw new LWIntegrationCfgException(Definitions.GetResponseMessage(ResponseCode.TechnicalDifficulties)) { ErrorCode = (int)ResponseCode.TechnicalDifficulties };
            }
        }
        public override Member ProcessMemberBeforeSave(LWIntegrationConfig config, Member member, XElement memberNode)
        {
            string methodName = MethodBase.GetCurrentMethod().Name;
            this.logger.Trace(className, methodName, "Begin");

            IList<IClientDataObject> mbrDtlObjs = member.GetChildAttributeSets("MemberDetails");
            MemberDetails memberDetails = (MemberDetails)mbrDtlObjs[0];

            this.logger.Trace(className, methodName, "Update Member Primary PostalCode to " + memberDetails.ZipOrPostalCode);
            // Trim dashes and spaces from zip code and then copy to primary postal code
            memberDetails.ZipOrPostalCode = memberDetails.ZipOrPostalCode.Replace("-", "").Replace(" ", "");
            member.PrimaryPostalCode = memberDetails.ZipOrPostalCode;

            member.ChangedBy = "UpdateMember api";
            memberDetails.ChangedBy = "UpdateMember api";

            // Nullify the PrimaryEmailAddress of member object after assigning it to EmailAddress of member details object
            if (!string.IsNullOrEmpty(member.PrimaryEmailAddress))
            {
                memberDetails.EmailAddress = member.PrimaryEmailAddress;
                member.PrimaryEmailAddress = null;
            }
            // Saving the member source got at the population stage
            memberDetails.MemberSource = memberSourceCode;

            // Update IsPrimary on all of the loyalty cards to its orginal value
            foreach (LoyaltyCardBackup backup in this.cardsBackup)
            {
                Brierley.FrameWork.Data.DomainModel.VirtualCard cardToUpdate = member.LoyaltyCards.FirstOrDefault(c => c.LoyaltyIdNumber == backup.LoyaltyIdNumber);

                if (cardToUpdate != null)
                {
                    cardToUpdate.IsPrimary = backup.IsPrimary;
                }
            }
            // Reload Old Email Address
            memberDetails.OldEmailAddress = this.oldEmailAddressBeforePopulate;
            // Update the old email address
            if (!string.IsNullOrEmpty(this.emailBeforePopulate) && this.emailBeforePopulate != memberDetails.EmailAddress)
            {
                memberDetails.OldEmailAddress = this.emailBeforePopulate;
            }            
            
            // added for redesign project
            if (this.emailBeforePopulate != memberDetails.EmailAddress.ToLower()) //Set to lowercase the Email Jira Ticket# AEO-160
            {
                memberDetails.PendingEmailVerification = 1;
                memberDetails.NextEmailReminderDate = DateTime.Today.AddDays(1);//AEO-153(Redesign 2015) Begin END
            }
            
            if (this.cellBeforePopulate != memberDetails.MobilePhone)
            {
                memberDetails.PendingCellVerification = 1;
            }

            this.logger.Trace(className, methodName, "End");
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
            try
            {
                this.logger.Trace(this.className, methodName, "Starts");

                // Getting MemberDetails for Member object
                MemberDetails memberDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                if (null != memberDetails)
                {
                    // Adding SMS bonus against API 7.0.14: Mobile bonus Points shall be applied to the member account.  
                    if (Convert.ToBoolean(memberNode.Element("MemberDetails").Attribute("SMSOptIn").Value))
                    {
                        if (!this.preSMSOptIn)
                        {
                            VirtualCard virtualCard = member.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard);
                            if (null == virtualCard)
                            {
                                virtualCard = member.GetLoyaltyCardByType(VirtualCardSearchType.MostRecentIssued);
                                this.logger.Trace(this.className, methodName, "No primary virtual card exists.");
                            }

                            if (null != virtualCard)
                            {
                                Boolean blnSMSOptIn = false;

                                if (memberDetails.SMSOptIn.HasValue)
                                {
                                    blnSMSOptIn = memberDetails.SMSOptIn.Value;
                                }

                                if (!String.IsNullOrEmpty(memberDetails.MobilePhone) && !Utilities.HasMemberEarnedSMSBonus(member) && blnSMSOptIn == true && !Utilities.MemberIsInPilot(memberDetails.ExtendedPlayCode)) // AEO-401
                                {
                                    Utilities.AddSMSBonus(virtualCard, memberDetails);
                                }
                            }
                        }
                    }
                }

                if (isNameAndAddresChanged)
                {
                    Merge.UpdateMemberProactiveMerge(member);
                }
                this.logger.Trace(this.className, methodName, "Ends");
                return member;
            }
            catch (LWIntegrationCfgException ex)
            {
                this.logger.Trace(this.className, methodName, ex.Message);
                throw new LWIntegrationCfgException(ex.Message) { ErrorCode = 1 };
            }
            catch (Exception ex)
            {
                this.logger.Trace(this.className, methodName, ex.Message);
                throw new LWIntegrationCfgException(ex.Message) { ErrorCode = 1 };
            }
        }
        /// <summary>
        /// If any of the name or address attributes change then set return true to set the AITUpdate flag.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberNode"></param>
        /// <returns></returns>
        private bool NeedsAITUpdate(Member member, XElement memberNode)
        {

            IList<IClientDataObject> lstMemberDetails = member.GetChildAttributeSets("MemberDetails");

            MemberDetails memberDetails = (MemberDetails)lstMemberDetails[0];

            //RO- AEO-345 We need to override the value of AddressLineTwo to an empty string in case the value is NULL
            if (string.IsNullOrEmpty(memberDetails.AddressLineTwo))
                memberDetails.AddressLineTwo = "";

            if (memberNode.Element("MemberDetails").Attribute("AddressLineOne").Value.Trim().ToUpper() != memberDetails.AddressLineOne.Trim().ToUpper()) 
            {
                return true;
            }
            if ((memberNode.Element("MemberDetails").Attribute("AddressLineTwo") != null) && (memberNode.Element("MemberDetails").Attribute("AddressLineTwo").Value.Trim().Length > 0))
            {
                if (memberNode.Element("MemberDetails").Attribute("AddressLineTwo").Value.Trim().ToUpper() != memberDetails.AddressLineTwo.Trim().ToUpper())
                {
                    return true;
                }
            }
            if (memberNode.Element("MemberDetails").Attribute("City").Value.Trim().ToUpper() != memberDetails.City.Trim().ToUpper())
            {
                return true;
            }
            if (memberNode.Element("MemberDetails").Attribute("StateOrProvince").Value.Trim().ToUpper() != memberDetails.StateOrProvince.Trim().ToUpper())
            {
                return true;
            }
            if (memberNode.Element("MemberDetails").Attribute("ZipOrPostalCode").Value.Trim().ToUpper() != memberDetails.ZipOrPostalCode.Trim().ToUpper())
            {
                return true;
            }
            if (memberNode.Attribute("FirstName").Value != member.FirstName)
            {
                return true;
            }
            if (memberNode.Attribute("LastName").Value.Trim().ToUpper() != member.LastName.Trim().ToUpper())
            {
                return true;
            }

            return false;
        }

    }
}
