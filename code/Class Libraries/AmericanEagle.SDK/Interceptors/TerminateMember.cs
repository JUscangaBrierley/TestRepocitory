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

//LW 4.1.14 change
//using Brierley.DNNModules.PortalModuleSDK;
using Brierley.WebFrameWork.Interceptors;

//LW 4.1.14 change
//using Brierley.DNNModules.MemberProfile;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Interceptors
{
    public class TerminateMember : IMemberSaveInterceptor<MemberProfileConfig>
    {

        private const string ClassName = "TerminateMember";
        private LWLogger logger = LWLoggerManager.GetLogger(ClassName);
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
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
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: Member IpCode - " + pMember.IpCode.ToString());
            return null;
            //return 0;
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
            using (var lwService = _dataUtil.LoyaltyDataServiceInstance())
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: Member IpCode - " + pMember.IpCode.ToString());

                lwService.CancelOrTerminateMember(pMember, DateTime.Today, String.Empty, true, new MemberCancelOptions());
                pMember.FirstName = "Account Terminated";
                pMember.LastName = "Account Terminated";
                pMember.PrimaryEmailAddress = string.Empty;
                pMember.MemberCloseDate = DateTime.Now; // AEO-374

                // Getting MemberDetails for Member object
                MemberDetails memberDetails = new MemberDetails();
                memberDetails = pMember.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;

                if (memberDetails != null)
                {
                    memberDetails.EmailAddress = string.Empty;
                    memberDetails.AddressLineOne = "Account Terminated";
                    memberDetails.AddressLineTwo = "Account Terminated";
                    memberDetails.City = "Account Terminated";
                    memberDetails.AITUpdate = true; //AEO-374
                                                    // Unlink from ae.com
                    if ((memberDetails.MemberSource == (int)MemberSource.OnlineAEEnrolled) || (memberDetails.MemberSource == (int)MemberSource.OnlineAERegistered))
                    {
                        memberDetails.MemberSource = (int)MemberSource.CSPortalUnlinked;
                    }


                }
                //  return 0;
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
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: Member IpCode - " + pMember.IpCode.ToString());
            CSNote note = new CSNote();
            note.Note = "As a result of an audit, this member's Points have been removed and account terminated, as authorized by Store Operations Personnel, Store Operations Management and AAP Marketing. Please contact your Supervisor with any questions.";
            note.MemberId = pMember.IpCode;
            using (var inst = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
            {
                note.CreatedBy = WebUtilities.GetCurrentUserId();
                inst.CreateNote(note);
            }
            Merge.UpdateMemberProactiveMergeMemberStatus(pMember, MemberStatusEnum.Terminated);
           // return 0;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        #endregion

    }
}
