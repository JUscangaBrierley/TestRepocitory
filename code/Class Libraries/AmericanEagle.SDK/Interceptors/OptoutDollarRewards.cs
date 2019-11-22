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
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Interceptors
{
    /// <summary>
    /// A new interceptor for the Update Profile page to cater the Dollar Rewards Opt-Out module. 
    /// </summary>
    class OptoutDollarRewards : IMemberSaveInterceptor<MemberProfileConfig>
    {
        private const string ClassName = "OptoutDollarRewards";
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
       // public virtual int BeforeSave(Member pMember, MemberProfileConfig moduleConfiguration, ref string errorMessage)
        public virtual Dictionary<string, ConfigurationItem> BeforeSave(Member pMember, MemberProfileConfig moduleConfiguration)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin: Member IpCode - " + pMember.IpCode.ToString());

            // Getting MemberDetails for Member object
            MemberDetails memberDetails = new MemberDetails();
            memberDetails = pMember.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;

            if (memberDetails != null)
            {
                if ( Utilities.isInPilot(memberDetails.ExtendedPlayCode) ) // AEO-Point Conversion
                {
                    // Set the extended play code to zero
                    memberDetails.ExtendedPlayCode = 0;
                    MemberDollarRewardOptOut memberDollarRewardOptOut = new MemberDollarRewardOptOut();
                    VirtualCard primaryVirtualCard = Utilities.GetVirtualCard(pMember);
                    memberDollarRewardOptOut.FromLoyaltynumber = primaryVirtualCard.LoyaltyIdNumber;
                    memberDollarRewardOptOut.ToLoyaltynumber = primaryVirtualCard.LoyaltyIdNumber;
                    memberDollarRewardOptOut.Action = "OptOut";
                    pMember.AddChildAttributeSet(memberDollarRewardOptOut);
                    // No need to update expiration date of transactions because this will be handled with stored procedure POINTADJUSTMENT_PKG.Proc_adjust_expdateoptout 
                    //UpdatePointsExpiration(pMember);
                }
            }
          //  return 0;
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
            note.Note = "Dollar Rewards program Opt-Out";
            note.MemberId = pMember.IpCode;
            using (var inst = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
            {
                note.CreatedBy = WebUtilities.GetCurrentUserId();
                inst.CreateNote(note);
            }
            //return 0;
        }
         // AEO-74 Upgrade 4.5 changes END here -----------SCJ
        #endregion

        #region Private methods

        /// <summary>
        /// Method to set expiration date of all the transactions, of the opt out member, to the end of current quarter.
        /// </summary>
        /// <param name="member"></param>
        private void UpdatePointsExpiration(Member member)
        {
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.Today.AddDays(1);
            int index = 0;

            try
            {
                using (var lDataService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    int year = DateTime.Now.Year;
                    if (year == 2014)
                    {
                        fromDate = new DateTime(year, 07, 01);
                        toDate = new DateTime(year, 12, 31, 23, 59, 59);
                    }
                    else
                    {
                        fromDate = new DateTime(year, 1, 1);
                        toDate = new DateTime(year, 12, 31, 23, 59, 59);
                    }
                    IList<PointType> pointTypes = lDataService.GetAllPointTypes();
                    long[] pointTypeIds = new long[6];

                    index = 0;
                    foreach (PointType pt in pointTypes)
                    {
                        if (pt.Name == "Basic Points" || pt.Name == "Bonus Points" || pt.Name == "StartingPoints" ||
                            pt.Name == "CS Points" || pt.Name == "Adjustment Points" || pt.Name == "Adjustment Bonus Points")
                        {
                            pointTypeIds[index] = pt.ID;
                            ++index;
                        }
                    }

                    // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                    //IList<PointTransaction> txns = LWDataServiceUtil.DataServiceInstance(true).GetPointTransactionsByPointTypePointEvent(member, fromDate, toDate, pointTypeIds, null, null, null, null, null, false);
                    IList<PointTransaction> txns = lDataService.GetPointTransactionsByPointTypePointEvent(member, fromDate, toDate, null, pointTypeIds, null, null, null, null, false, null);
                    // AEO-74 Upgrade 4.5 changes END here -----------SCJ
                    DateTime quarterStartDate = DateTime.Now;
                    DateTime quarterEndDate = DateTime.Now;
                    Utilities.GetQuarterDates(out quarterStartDate, out quarterEndDate);
                    DateTime expirationdate = new DateTime(quarterEndDate.Year, quarterEndDate.Month, quarterEndDate.Day, 23, 59, 59);

                    foreach (PointTransaction txn in txns)
                    {
                        if (txn.ExpirationDate >= DateTime.Now)
                        {
                            txn.ExpirationDate = expirationdate;
                        }
                        lDataService.UpdatePointTransaction(txn);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
