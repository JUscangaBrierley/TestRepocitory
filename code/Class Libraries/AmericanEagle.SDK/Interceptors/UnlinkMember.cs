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


using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;

namespace AmericanEagle.SDK.Interceptors
{
    class UnlinkMember : IMemberSaveInterceptor<MemberProfileConfig>
    {
        #region IMemberSaveInterceptor<MemberProfileConfig> Members

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
        // public int AfterSave(Member Member, MemberProfileConfig ModuleConfiguration, ref string errorMessage)
        public void AfterSave(Member Member, MemberProfileConfig moduleConfiguration, ref string errorMessage)
        
        {
            CSNote note = new CSNote();
            note.Note = "Account unlinked from AE.com.";
            note.MemberId = Member.IpCode;
            using (var inst = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
            {
                note.CreatedBy = WebUtilities.GetCurrentUserId();
                inst.CreateNote(note);
            }
           // return 0;
        }
        
       //  public int BeforePopulate(Member Member, MemberProfileConfig moduleConfiguration, ref string errorMessage)
        public  Dictionary<string, ConfigurationItem> BeforePopulate(Member Member, MemberProfileConfig moduleConfiguration)
        
        {
           // return 0;
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
       // public  int BeforeSave(Member Member, MemberProfileConfig moduleConfiguration, ref string errorMessage)
        public  Dictionary<string, ConfigurationItem> BeforeSave(Member Member, MemberProfileConfig moduleConfiguration)
        {
            IList<IClientDataObject> memberDetails = Member.GetChildAttributeSets("MemberDetails");
            MemberDetails mbrDetails = (MemberDetails)memberDetails[0];

            if (null != mbrDetails)
            {
                // Unlink from ae.com
                if ((mbrDetails.MemberSource == (int)MemberSource.OnlineAEEnrolled) || (mbrDetails.MemberSource == (int)MemberSource.OnlineAERegistered))
                {
                    mbrDetails.MemberSource = (int)MemberSource.CSPortalUnlinked;
                }
                mbrDetails.ChangedBy = WebUtilities.GetCurrentUserName();
                Member.ChangedBy = WebUtilities.GetCurrentUserName();
            }
          //  return 0;
            return null;
        }
        // AEO-74 Upgrade 4.5 changes END here -----------SCJ

        #endregion
    }
}
