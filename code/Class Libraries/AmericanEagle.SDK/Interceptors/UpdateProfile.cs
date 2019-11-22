using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.Cache;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork.Interceptors;

//LW 4.1.14 change
//using Brierley.DNNModules.MemberProfile;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Interceptors
{
    public class UpdateProfile : IMemberSaveInterceptor<MemberProfileConfig>
    {

        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        String _FirstNameBeforePopulate = string.Empty;
        String _LastNameBeforePopulate = string.Empty;
        String _CountryBeforePopulate = String.Empty;
        String _Address1BeforePopulate = String.Empty;
        String _Address2BeforePopulate = String.Empty;
        String _CityBeforePopulate = String.Empty;
        String _StateBeforePopulate = String.Empty;
        String _ZipBeforePopulate = String.Empty;
        Boolean _IsEmailUndeliverableBeforePopulate = true;
        String _EmailBeforePopulate = String.Empty;
        String _pointType = string.Empty;
        String _pointEvent = string.Empty;
        String _mobilePhoneBeforePopulate = string.Empty;
        private static LWLogger logger = LWLoggerManager.GetLogger("UpdateProfile");
        private const int PilotProgram = 1; // Program Redesign 2015 (MMV004)

        // Program Redesign 2015 (MMV004) Begin 


        /// <summary>
        /// Returns True if the Member whose details are passed as parameter
        /// has ExtendedPlayCode field set to 1.  
        /// </summary>
        /// <param name="pMemeberDetails"> Collection of data that contains the member details</param>
        /// <returns>True is the member detail named ExtendedPlayCode is equal to 1</returns>
        private bool isPilotProgram ( MemberDetails pMemeberDetails )
        {
            bool retval = false;

            retval = pMemeberDetails != null && Utilities.isInPilot(pMemeberDetails.ExtendedPlayCode); // AEO-Point Conversion
                      // pMemeberDetails.ExtendedPlayCode == UpdateProfile.PilotProgram;

           
            return retval;
        }

        // Program Redesign 2015 (MMV004) End



        #region Interface Implementation Methods

        /// <summary>
        /// Called before any postback data is loaded into the member object.
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="ModuleConfiguration">
        ///		The configuration object for the current executing module.
        /// </param>
        /// int - zero for success, else a number which maps to an error code in the resource file: MemberBeforeSave_[return code].Text
        /// Mobile profile does not use resource files for error messages. For mobile profile validation, pass the error message back in errorMessage.
        /// <remarks>
        /// If successfull (0), then the member will be populated with postback data. Otherwise, the member is not populated and the error message is displayed.
        /// </remarks>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //  public virtual int BeforePopulate(Member pMember, MemberProfileConfig moduleConfiguration, ref string errorMessage)
        public virtual Dictionary<string, ConfigurationItem> BeforePopulate(Member pMember, MemberProfileConfig moduleConfiguration)
        {
            //This was retaining the information we already had entered on a previos attempt to change the email
            // and so the email we get on the Before Populate in order to get the real email we need to go to the DB
            //IList<IClientDataObject> memberDetails = pMember.GetChildAttributeSets("MemberDetails");
            Member member;
            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                member = lwService.LoadMemberFromIPCode(pMember.IpCode);
            }

            IList<IClientDataObject> memberDetails = member.GetChildAttributeSets("MemberDetails");
            MemberDetails _MemberDetailsBeforePopulate = memberDetails[0] as MemberDetails;
            _EmailBeforePopulate = _MemberDetailsBeforePopulate.EmailAddress == null ? string.Empty : _MemberDetailsBeforePopulate.EmailAddress;
            return null;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ

        /// <summary>
        /// Called before a member is saved.
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="ModuleConfiguration">
        ///		The configuration object for the current executing module.
        /// </param>
        /// <returns>
        /// int - zero for success, else a number which maps to an error code in the resource file: MemberBeforeSave_[return code].Text
        /// Mobile profile does not use resource files for error messages. For mobile profile validation, pass the error message back in errorMessage.
        /// </returns>
        /// <remarks>
        /// If successfull (0), then the save method will be called on the member object. 
        /// Otherwise, the member is not saved and the error message is displayed.
        /// </remarks>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        // public virtual int BeforeSave(Member pMember, MemberProfileConfig moduleConfiguration, ref string errorMessage)
        public virtual Dictionary<string, ConfigurationItem> BeforeSave(Member pMember, MemberProfileConfig moduleConfiguration)
        {
            int _errorCode = 9999;
            string _errorMessage = string.Empty;
            string _errorSource = string.Empty;

            Dictionary<string, ConfigurationItem> errors = new Dictionary<string, ConfigurationItem>();

            try
            {
                IList<IClientDataObject> memberDetails = pMember.GetChildAttributeSets("MemberDetails");
                MemberDetails mbrDetails = (MemberDetails)memberDetails[0];

                if (null != mbrDetails)
                {
                    if(string.IsNullOrEmpty(mbrDetails.EmailAddress.Trim()))
                    {
                        string msg = "AEMessage|The Email address is Empty. Please enter a valid e-mail Address.";
                        LWException EmptyEmailException = new LWException(msg);
                        EmptyEmailException.ErrorCode = 240;
                        throw EmptyEmailException;
                    }

                    if (_EmailBeforePopulate.ToLower() != mbrDetails.EmailAddress.ToLower())
                    {
                        LWCriterion critDupEmail = new LWCriterion("MemberDetails");
                        critDupEmail.Add(LWCriterion.OperatorType.OR, "EmailAddress", mbrDetails.EmailAddress, LWCriterion.Predicate.Eq);  //Member's IpCode
                        //Object for Member Details
                        critDupEmail.Add(LWCriterion.OperatorType.OR, "EmailAddress", mbrDetails.EmailAddress.ToLower(), LWCriterion.Predicate.Eq);  //Member's IpCode
                        critDupEmail.Add(LWCriterion.OperatorType.OR, "EmailAddress", mbrDetails.EmailAddress.ToUpper(), LWCriterion.Predicate.Eq);  //Member's IpCode
                        //Object for Member Details
                        IList<IClientDataObject> objDuplicateEmail;
                        using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            objDuplicateEmail = lwService.GetAttributeSetObjects(null, "MemberDetails", critDupEmail, null, false);
                        }
                        //We determine if we must throw the duplicate email exception
                        if(objDuplicateEmail != null)
                        {
                            if(objDuplicateEmail.Count > 0)
                            {
                                string msg = "AEMessage|The Email address entered already belongs to another account. Please enter a different e-mail Address.";
                                LWException DupEmailException = new LWException(msg);
                                DupEmailException.ErrorCode = 250;
                                throw DupEmailException;
                            }
                        }

                        mbrDetails.OldEmailAddress = _EmailBeforePopulate;
                        mbrDetails.AITUpdate = true;
                        mbrDetails.EmailAddressUpdateDate = DateTime.Now;

                        mbrDetails.ChangedBy = WebUtilities.GetCurrentUserName();
                        pMember.ChangedBy = WebUtilities.GetCurrentUserName();
                        pMember.PrimaryEmailAddress = null;
                    }

                    mbrDetails.EmailAddress = mbrDetails.EmailAddress.ToLower();
                }
            }
            catch (Exception e)
            {
                if(e.GetType() == typeof(LWException))
                {
                    LWException ex = (LWException)e;
                    _errorMessage = string.Format("There was an error processing the member data, error: {0}", ex.Message);
                    logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, _errorMessage);
                    errors.Add(ex.Message, null);
                    return errors;

                }
                _errorMessage = string.Format("There was an error processing the member data, error: {0}", e.Message);
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, _errorMessage);
                throw new LWException(_errorMessage) { ErrorCode = _errorCode };
            }

            return null;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ


        /// <summary>
        /// Called after a member has been successfully saved.
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="ModuleConfiguration">
        ///		The configuration object for the current executing module.
        /// </param>
        /// <returns>
        /// int - zero for success, else a number which maps to an error code in the resource file: MemberAfterSave_[return code];.Text
        /// Mobile profile does not use resource files for error messages. For mobile profile validation, pass the error message back in errorMessage.
        /// </returns>
        /// <remarks>
        /// The member has already been saved at this point, so any error returned by this method will have no effect other than displaying the message.
        /// </remarks>
        // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
        //public virtual int AfterSave(Member pMember, MemberProfileConfig moduleConfiguration, ref string errorMessage)
        public virtual void AfterSave(Member pMember, MemberProfileConfig moduleConfiguration, ref string errorMessage)
        {
            IList<IClientDataObject> memberDetails = pMember.GetChildAttributeSets("MemberDetails");
            MemberDetails mbrDetails = memberDetails[0] as MemberDetails;
            CSNote note = new CSNote();
            note.Note = "Profile Updated.";
            note.MemberId = pMember.IpCode;
            using (var inst = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
            {
                note.CreatedBy = WebUtilities.GetCurrentUserId();
                inst.CreateNote(note);
            }
            //email was not there prior to save but now saved with good email address
            //email was undeliverable prior to save but now saved with good email address
            if (String.IsNullOrEmpty(_EmailBeforePopulate) && !String.IsNullOrEmpty(mbrDetails.EmailAddress) || _IsEmailUndeliverableBeforePopulate && mbrDetails.EmailAddressMailable.HasValue)
            {
                SendUpdateProfileTriggerdEMail(pMember, mbrDetails.EmailAddress);
            }

            //if (_isNameAndAddressChanged)
            //{
            //    Merge.UpdateMemberProactiveMerge(pMember);
            //}

            //  return 0;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        //private bool NameAndAddressChanged(Member member, MemberDetails mbrDetails)
        //{
        //    if (!String.IsNullOrEmpty(_FirstNameBeforePopulate) && !String.IsNullOrEmpty(member.FirstName) && (_FirstNameBeforePopulate.Trim().ToUpper() != member.FirstName.Trim().ToUpper()))
        //    {
        //        return true;
        //    }
        //    if (!String.IsNullOrEmpty(_LastNameBeforePopulate) && !String.IsNullOrEmpty(member.LastName) && (_LastNameBeforePopulate.Trim().ToUpper() != member.LastName.Trim().ToUpper()))
        //    {
        //        return true;
        //    }
        //    if (IsAddressChanged(mbrDetails))
        //    {
        //        return true;
        //    }
        //    // added for redesign project
        //    if ((_EmailBeforePopulate != mbrDetails.EmailAddress) && (_mobilePhoneBeforePopulate != mbrDetails.MobilePhone))
        //    {
        //        return true;
        //    }
        //    // end adding for redesign project
        //    return false;
        //}
        //private Boolean IsAddressChanged(MemberDetails pmbrDetails)
        //{
        //    if (!String.IsNullOrEmpty(_CountryBeforePopulate) && !String.IsNullOrEmpty(pmbrDetails.Country) && (_CountryBeforePopulate.Trim().ToUpper() != pmbrDetails.Country.Trim().ToUpper()))
        //    {
        //        return true;
        //    }
        //    if (!String.IsNullOrEmpty(_Address1BeforePopulate) && !String.IsNullOrEmpty(pmbrDetails.AddressLineOne) && (_Address1BeforePopulate.Trim().ToUpper() != pmbrDetails.AddressLineOne.Trim().ToUpper()))
        //    {
        //        return true;
        //    }
        //    if (!String.IsNullOrEmpty(_Address2BeforePopulate) && !String.IsNullOrEmpty(pmbrDetails.AddressLineTwo) && (_Address2BeforePopulate.Trim().ToUpper() != pmbrDetails.AddressLineTwo.Trim().ToUpper()))
        //    {
        //        return true;
        //    }
        //    if (!String.IsNullOrEmpty(_CityBeforePopulate) && !String.IsNullOrEmpty(pmbrDetails.City) && (_CityBeforePopulate.Trim().ToUpper() != pmbrDetails.City.Trim().ToUpper()))
        //    {
        //        return true;
        //    }
        //    if (!String.IsNullOrEmpty(_StateBeforePopulate) && !String.IsNullOrEmpty(pmbrDetails.StateOrProvince) && (_StateBeforePopulate.Trim().ToUpper() != pmbrDetails.StateOrProvince.Trim().ToUpper()))
        //    {
        //        return true;
        //    }
        //    if (!String.IsNullOrEmpty(_ZipBeforePopulate) && !String.IsNullOrEmpty(pmbrDetails.ZipOrPostalCode) && (_ZipBeforePopulate.Trim().ToUpper() != pmbrDetails.ZipOrPostalCode.Trim().ToUpper()))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
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
        private void SendUpdateProfileTriggerdEMail(Member member, string emailAddress)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            Dictionary<string, string> additionalFields = new Dictionary<string, string>();
            additionalFields.Add("FirstName", member.FirstName);

            AEEmail.SendEmail(member, EmailType.UpdateProfile, additionalFields, emailAddress);
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }
        #endregion

    }
}
