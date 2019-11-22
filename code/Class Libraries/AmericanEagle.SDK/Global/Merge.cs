using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.Clients.AmericanEagle.DataModel;
using Brierley.ClientDevUtilities.LWGateway;

namespace AmericanEagle.SDK.Global
{
    public static class Merge
    {
        private static LWLogger logger = LWLoggerManager.GetLogger("Merge");
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        public static string FromLoyaltyID { get; set; }
        public static bool IsFromProactiveMerge { get; set; }

        /// <summary>
        /// Merge member accounts
        /// </summary>
        public static void MergeMember(Member fromMember, Member toMember, MemberMergeOptions options, PointEvent pointEvent, PointType pointType, string changedBy, long createdBy)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                if (Merge.IsFromProactiveMerge && fromMember.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard) != null)
                {
                    Merge.FromLoyaltyID = fromMember.GetLoyaltyCardByType(VirtualCardSearchType.PrimaryCard).LoyaltyIdNumber;
                }
                ActionBeforeMerge(fromMember, toMember, changedBy);

                using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    ldService.MergeMember(fromMember, toMember, pointEvent, pointType, DateTime.Now, options);
                }

                if (string.IsNullOrEmpty(Merge.FromLoyaltyID))
                {
                    Merge.FromLoyaltyID = GetFromLoyaltyID(fromMember);
                }
                ActionAfterMerge(fromMember, toMember, changedBy);
                //AddNotes(toMember, createdBy, "Account# " + GetFromLoyaltyID(fromMember).Trim() + " merged with this account");
                AddNotes(toMember, createdBy, "Account# " + FromLoyaltyID.Trim() + " merged with this account");
                MergeCsNotes(fromMember, toMember);
                Merge.FromLoyaltyID = string.Empty;
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }


        /// <summary>
        /// Add record to ats_MemberProactiveMerge table when new member is added during addMember or DAP Enrollment.
        /// </summary>
        public static void AddMemberProactiveMerge(Member member)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            MemberProactiveMerge proactiveMerge = new MemberProactiveMerge();

            try
            {
                if (null != member && proactiveMerge != null)
                {
                    // Getting MemberDetails for Member object
                    MemberDetails memberDetails = new MemberDetails();
                    memberDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                    if (null != memberDetails)
                    {
                        string strAddress1 = memberDetails.AddressLineOne.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty).Trim().ToLower();
                        int addressLength = 7;

                        if (strAddress1.Length < 7)
                        {
                            addressLength = strAddress1.Length;
                        }
                        proactiveMerge.AddressLineOne = strAddress1.Substring(0, addressLength);
                        // Trim dashes and spaces from zip code and then copy to primary postal code
                        memberDetails.ZipOrPostalCode = memberDetails.ZipOrPostalCode.Replace("-", "").Replace(" ", "");
                        member.PrimaryPostalCode = memberDetails.ZipOrPostalCode;
                        proactiveMerge.ZipCode = memberDetails.ZipOrPostalCode.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty).Replace("-", string.Empty).Trim().ToLower();
                        // PI 28118 - Change verification rules for Proactive Merge (use first 5 digits of US Zipcode)
                        if (memberDetails.Country == "USA" && proactiveMerge.ZipCode.Length > 5)
                        {
                            proactiveMerge.ZipCode = proactiveMerge.ZipCode.Substring(0, 5);
                        }
                    }

                    string strFirstName = member.FirstName.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty).Trim().ToLower();
                    int nameLength = 4;

                    if (strFirstName.Length < 4)
                    {
                        nameLength = strFirstName.Length;
                    }

                    proactiveMerge.IpCode = member.IpCode;
                    proactiveMerge.FirstName = strFirstName.Substring(0, nameLength);
                    proactiveMerge.LastName = member.LastName.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty).Trim().ToLower();
                    proactiveMerge.MemberCreateDate = member.MemberCreateDate;
                    proactiveMerge.MemberStatus = (int)(member.MemberStatus);
                    proactiveMerge.ChangedBy = member.ChangedBy;
                    member.AddChildAttributeSet(proactiveMerge);
                    using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        ldService.SaveMember(member);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }


        /// <summary>
        /// update record in ats_MemberProactiveMerge table if profile is changed during RegisterMember or UpdateProfile.
        /// </summary>
        public static void UpdateMemberProactiveMerge(Member member)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    MemberDetails memberDetails = new MemberDetails();
                    memberDetails = GetMemberDetails(member);

                    LWCriterion crit = new LWCriterion("MemberProactiveMerge");
                    crit.Add(LWCriterion.OperatorType.AND, "IpCode", member.IpCode, LWCriterion.Predicate.Eq);
                    //IList<IClientDataObject> lstMemberProactiveMerge = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "MemberProactiveMerge", crit, new LWQueryBatchInfo(), false);   // AEO-74 Upgrade 4.5 here -----------SCJ
                    IList<IClientDataObject> lstMemberProactiveMerge = ldService.GetAttributeSetObjects(null, "MemberProactiveMerge", crit, null, false);
                    MemberProactiveMerge proactiveMerge = new MemberProactiveMerge();

                    string strFirstName = string.Empty;
                    string strAddress1 = string.Empty;
                    int nameLength = 4;
                    int addressLength = 7;

                    if (null != lstMemberProactiveMerge && lstMemberProactiveMerge.Count > 0 && null != memberDetails && null != proactiveMerge)
                    {
                        for (int i = 0; i < lstMemberProactiveMerge.Count; i++)
                        {
                            proactiveMerge = (MemberProactiveMerge)lstMemberProactiveMerge[i];

                            strFirstName = member.FirstName.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty).Trim().ToLower();
                            strAddress1 = memberDetails.AddressLineOne.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty).Trim().ToLower();
                            if (strAddress1.Length < 7)
                            {
                                addressLength = strAddress1.Length;
                            }

                            if (strFirstName.Length < 4)
                            {
                                nameLength = strFirstName.Length;
                            }

                            proactiveMerge.FirstName = strFirstName.Substring(0, nameLength);
                            proactiveMerge.LastName = member.LastName.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty).Trim().ToLower();
                            proactiveMerge.AddressLineOne = strAddress1.Substring(0, addressLength);
                            proactiveMerge.ZipCode = memberDetails.ZipOrPostalCode.Replace(" ", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("'", string.Empty).Replace("-", string.Empty).Trim().ToLower();
                            // PI 28118 - Change verification rules for Proactive Merge (use first 5 digits of US Zipcode)
                            if (memberDetails.Country == "USA" && proactiveMerge.ZipCode.Length > 5)
                            {
                                proactiveMerge.ZipCode = proactiveMerge.ZipCode.Substring(0, 5);
                            }
                            proactiveMerge.MemberCreateDate = member.MemberCreateDate;
                            proactiveMerge.MemberStatus = (int)(member.MemberStatus);
                            proactiveMerge.ChangedBy = member.ChangedBy;
                            member.AddChildAttributeSet(proactiveMerge);
                            ldService.SaveMember(member);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// update MemberStatus in ats_MemberProactiveMerge table if member's status is changed.
        /// </summary>
        public static void UpdateMemberProactiveMergeMemberStatus(Member member, MemberStatusEnum memberStatus)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            try
            {
                using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    LWCriterion crit = new LWCriterion("MemberProactiveMerge");
                    crit.Add(LWCriterion.OperatorType.AND, "IpCode", member.IpCode, LWCriterion.Predicate.Eq);
                    //   IList<IClientDataObject> lstMemberProactiveMerge = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "MemberProactiveMerge", crit, new LWQueryBatchInfo(), false); // AEO-74 Upgrade 4.5 here -----------SCJ
                    IList<IClientDataObject> lstMemberProactiveMerge = ldService.GetAttributeSetObjects(null, "MemberProactiveMerge", crit, null, false); // AEO-74 Upgrade 4.5 here -----------SCJ

                    if (null != lstMemberProactiveMerge && lstMemberProactiveMerge.Count > 0)
                    {
                        MemberProactiveMerge proactiveMerge = new MemberProactiveMerge();

                        for (int i = 0; i < lstMemberProactiveMerge.Count; i++)
                        {
                            proactiveMerge = (MemberProactiveMerge)lstMemberProactiveMerge[i];
                            proactiveMerge.MemberStatus = (int)memberStatus;

                            if (memberStatus == MemberStatusEnum.Terminated)
                            {
                                proactiveMerge.ChangedBy = "TerminateMember";
                            }
                            else if (memberStatus == MemberStatusEnum.Disabled)
                            {
                                proactiveMerge.ChangedBy = "Merge";
                            }
                            else
                            {
                                proactiveMerge.ChangedBy = member.ChangedBy;
                            }

                            member.AddChildAttributeSet(proactiveMerge);
                            ldService.SaveMember(member);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Retrieve record from ats_MemberProactiveMerge table
        /// </summary>
        /// An optional argument has been added for PI 31467 - Change matching criteria for Google Wallet links
        public static IList<IClientDataObject> GetMemberProactiveMerge(string firstName, string lastName, string addressLineOne, string zipCode, bool isNotLink = true)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            IList<IClientDataObject> listProactiveMerge = null;
            try
            {
                LWCriterion crit = new LWCriterion("MemberProactiveMerge");
                crit.Add(LWCriterion.OperatorType.AND, "FirstName", firstName, LWCriterion.Predicate.Eq);
                crit.Add(LWCriterion.OperatorType.AND, "LastName", lastName, LWCriterion.Predicate.Eq);
                //PI 31467-Change matching criteria for Google Wallet links, Start changes
                if (isNotLink)
                {
                    crit.Add(LWCriterion.OperatorType.AND, "AddressLineOne", addressLineOne, LWCriterion.Predicate.Eq);
                    crit.Add(LWCriterion.OperatorType.AND, "ZipCode", zipCode, LWCriterion.Predicate.Eq);
                }
                //PI 31467-Change matching criteria for Google Wallet links, End changes
                crit.Add(LWCriterion.OperatorType.AND, "MemberStatus", MemberStatusEnum.Active, LWCriterion.Predicate.Eq);
                crit.AddOrderBy("MemberCreateDate", LWCriterion.OrderType.Descending);
                //listProactiveMerge = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "MemberProactiveMerge", crit, new LWQueryBatchInfo(), false);  // AEO-74 Upgrade 4.5 here -----------SCJ
                using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                    listProactiveMerge = ldService.GetAttributeSetObjects(null, "MemberProactiveMerge", crit, null, false);
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");
            return listProactiveMerge;
        }

        /// <summary>
        /// Defines action before merging the accounts
        /// </summary>
        public static void ActionBeforeMerge(Member fromMember, Member toMember, string changedBy)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                MergeMemberReceipts(fromMember, toMember, changedBy);
                MergeMemberCardReplacements(fromMember, toMember);
                MergeMemberMergeHistory(fromMember, toMember, changedBy);
                MemberDetails fromMemberDetails = GetMemberDetails(fromMember);
                MemberDetails toMemberDetails = GetMemberDetails(toMember);
                SetMemberSource(fromMemberDetails, toMemberDetails);
                SetMemberDetailProperties(fromMemberDetails, toMemberDetails);

                SetMemberProperties(fromMember, toMember);
                //PI17661-Nullify the PrimaryEmailAddress of toMember object after assigning it to EmailAddress of its details object
                if (toMemberDetails != null && !string.IsNullOrEmpty(toMember.PrimaryEmailAddress))
                {
                    toMemberDetails.EmailAddress = toMember.PrimaryEmailAddress;
                    toMember.PrimaryEmailAddress = null;
                }
                // PI 29670 - When merging 2 accounts, set the GW flag to 1 if the flag is set for any accounts that are merged.
                if ((fromMemberDetails.GwLinked.HasValue && fromMemberDetails.GwLinked.Value == 1) || toMemberDetails.GwLinked.HasValue && toMemberDetails.GwLinked.Value == 1)
                {
                    toMemberDetails.GwLinked = 1;
                }
                // PI 30364 - Dollar reward program changes - Start
                if  ( ( Utilities.isInPilot ( fromMemberDetails.ExtendedPlayCode)) || Utilities.isInPilot ( toMemberDetails.ExtendedPlayCode)) // AEO-Point Conversion
                {
                    //AEO-757 BEGIN
                    // Set the extended play code to one
                    if (Merge.IsFromProactiveMerge) {
                        toMemberDetails.ExtendedPlayCode = 1;
                    }
                    //AEO-757 END

                    MemberDollarRewardOptOut memberDollarRewardOptOut = new MemberDollarRewardOptOut();
                    VirtualCard primaryVirtualCardFrom = Utilities.GetVirtualCard(fromMember);
                    VirtualCard primaryVirtualCardTo = Utilities.GetVirtualCard(toMember);
                    memberDollarRewardOptOut.FromLoyaltynumber = primaryVirtualCardFrom.LoyaltyIdNumber;
                    memberDollarRewardOptOut.ToLoyaltynumber = primaryVirtualCardTo.LoyaltyIdNumber;
                    memberDollarRewardOptOut.Action = "Merge";
                    toMember.AddChildAttributeSet(memberDollarRewardOptOut);
                    
                }

                // aeo-757 BEGIN
                if (Merge.IsFromProactiveMerge)
                {
                    fromMemberDetails.ExtendedPlayCode = toMemberDetails.ExtendedPlayCode;
                }
                // AEO-757 END

                // PI 30364 - Dollar reward program changes - End
                // added for redesign project
                //AEO574 changes Begin --------------------------SCJ commenting below lines, becase they are moved to inpilot check further below
                //if ((fromMemberDetails != null && fromMemberDetails.NetSpend.HasValue) && (toMemberDetails != null && toMemberDetails.NetSpend.HasValue))
                //{
                //    toMemberDetails.NetSpend = fromMemberDetails.NetSpend.Value + toMemberDetails.NetSpend.Value;
                //}
                // end adding for redesign project


                // AEO-503 Begin
                if ( Utilities.isInPilot(fromMemberDetails.ExtendedPlayCode) && Utilities.isInPilot(toMemberDetails.ExtendedPlayCode) ) 
                {

                    //AEO574 changes Begin --------------------------SCJ
                    if ((fromMemberDetails != null && fromMemberDetails.NetSpend.HasValue) && (toMemberDetails != null && toMemberDetails.NetSpend.HasValue))
                    {

                        toMemberDetails.NetSpend = fromMemberDetails.NetSpend.Value + toMemberDetails.NetSpend.Value;
                       
                    }
                    
                    MemberTier toMemtier = toMember.GetTier(DateTime.Now.Date);
                    TierDef toMemtierdf = toMemtier == null ? null : toMemtier.TierDef;
                    if (toMemtier != null)
                    {

                    }
                    else
                    {
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "No Tier exists in toMember,so adding blue base");
                        //Add an  Blue Base Tier, since no tier exists, but the from date is the day before,so that if the same tier is going to get expired, the fromdate and todate would be the same 
                        toMember.AddTier("Blue", DateTime.Today.AddMinutes(-1), DateTime.Parse("12/31/2199"), "Base");
                    }
                    //AEO574 changes END --------------------------SCJ
                    MergeMemberTiers(fromMember, toMember, changedBy); 

                }
              
                // AEO-503 End
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }


        /// <summary>
        /// Defines action after merging the accounts
        /// </summary>
        public static void ActionAfterMerge(Member fromMember, Member toMember, string changedby)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    IList<MemberReward> listFromMemberAfterMergeReward = fromMember.GetRewards();
                    MemberDetails toMemberDetails = GetMemberDetails(toMember);
                    foreach (MemberReward item in listFromMemberAfterMergeReward)
                    {
                        item.MemberId = toMember.IpCode;
                        item.ChangedBy = "Merged";
                        ldService.UpdateMemberReward(item);
                    }

                    //PI21346 - Merge Member Brand
                    IList<IClientDataObject> listFromMemberBrands = fromMember.GetChildAttributeSets("MemberBrand");

                    if (listFromMemberBrands != null && listFromMemberBrands.Count > 0)
                    {
                        for (int i = 0; i < listFromMemberBrands.Count; i++)
                        {
                            MemberBrand fromMemberBrand = (MemberBrand)listFromMemberBrands[i];
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "fromIPCode, toIPCode: " + fromMember.IpCode + " " + toMember.IpCode);
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "fromMemberBrand.BrandID: " + fromMemberBrand.BrandID);

                            //Add brand flags which fromMember has but toMemeber doesn't to the toMember's.
                            if (fromMemberBrand.BrandID != null && !checkForBrandFlagInToMember(toMember, (long)fromMemberBrand.BrandID))
                            {
                                MemberBrand memberBrandToAdd = new MemberBrand();
                                memberBrandToAdd.BrandID = fromMemberBrand.BrandID;
                                memberBrandToAdd.ChangedBy = "Merged";
                                memberBrandToAdd.CreateDate = DateTime.Now;
                                memberBrandToAdd.UpdateDate = DateTime.Now;
                                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Added BrandId: " + fromMemberBrand.BrandID);
                                toMember.AddChildAttributeSet(memberBrandToAdd);
                            }
                        }
                    }

                    // Add a merge history
                    MemberMergeHistory merge = new MemberMergeHistory();
                    merge.ChangedBy = changedby;
                    merge.FromIPCode = fromMember.IpCode;
                    //merge.FromLoyaltyID = GetFromLoyaltyID(fromMember).Trim();
                    merge.FromLoyaltyID = Merge.FromLoyaltyID.Trim();
                    toMember.AddChildAttributeSet(merge);
                    ldService.SaveMember(toMember);
                    MergeMemberPromoQueue(fromMember, toMember);
                    UpdateMemberProactiveMergeMemberStatus(fromMember, MemberStatusEnum.Disabled);
                    // added for redesign project
                    if (toMemberDetails != null && toMemberDetails.NetSpend.HasValue)
                    {  //AEO574 changes Begin --------------------------SCJ
                        MemberTier MemTier = toMember.GetTier(DateTime.Now.Date);
                        if (MemTier != null)
                        {

                            if (toMemberDetails.NetSpend.Value >= 250)
                            {

                                //if (toMember.IsInTier("Blue"))
                                if (MemTier.TierDef.Name.ToUpper().Trim() == "BLUE")
                                {
                                    toMember.AddTier("Silver", DateTime.Today, DateTime.Parse("12/31/2199"), "MembersMerged");
                                }
                            }
                            if (toMemberDetails.NetSpend.Value < 250)
                            {
                                if ((MemTier.TierDef.Name.ToUpper().Trim() == "SILVER") && (MemTier.Description == null))
                                {
                                    toMember.AddTier("Blue", DateTime.Today, DateTime.Parse("12/31/2199"), "MembersMerged"); //Tier Downgrade because of Merge netspend below value.
                                }

                                else if ((MemTier.TierDef.Name.ToUpper().Trim() == "SILVER") && (!MemTier.Description.Contains("Nomination")) && (MemTier.Description != "Pilot Test Group"))
                                {
                                    toMember.AddTier("Blue", DateTime.Today, DateTime.Parse("12/31/2199"), "MembersMerged"); //Tier Downgrade because of Merge netspend below value.
                                }
                            }
                        }
                    }
                    //AEO574 changes End --------------------------SCJ

                    // end adding for redesign project
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// merge member card replacements
        /// </summary>
        public static void MergeMemberCardReplacements(Member fromMember, Member toMember)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                IList<IClientDataObject> lstMemberCardReplacements = fromMember.GetChildAttributeSets("MemberCardReplacements");
                foreach (MemberCardReplacements item in lstMemberCardReplacements)
                {
                    MemberCardReplacements memberCardReplacements = new MemberCardReplacements();
                    memberCardReplacements.IpCode = toMember.IpCode;
                    memberCardReplacements.LoyaltyIDNumber = item.LoyaltyIDNumber;
                    memberCardReplacements.DateReceived = item.DateReceived;
                    memberCardReplacements.CHANGEDBY = item.CHANGEDBY;
                    memberCardReplacements.StatusCode = item.StatusCode;
                    memberCardReplacements.CreateDate = item.CreateDate;
                    memberCardReplacements.UpdateDate = item.UpdateDate;
                    memberCardReplacements.LastDmlId = item.LastDmlId;
                    toMember.AddChildAttributeSet(memberCardReplacements);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Merge all history 
        /// </summary>
        public static void MergeMemberMergeHistory(Member fromMember, Member toMember, string changedBy)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            IList<IClientDataObject> lstMemberMergeHistory = fromMember.GetChildAttributeSets("MemberMergeHistory");
            foreach (MemberMergeHistory item in lstMemberMergeHistory)
            {
                MemberMergeHistory memberMergeHistory = new MemberMergeHistory();
                memberMergeHistory.IpCode = toMember.IpCode;
                memberMergeHistory.ChangedBy = item.ChangedBy;
                memberMergeHistory.FromIPCode = item.FromIPCode;
                memberMergeHistory.FromLoyaltyID = item.FromLoyaltyID;  //PI15929
                toMember.AddChildAttributeSet(memberMergeHistory);
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Add notes after merge
        /// </summary>
        /// <param name="notes">Note text to merge</param>
        /// 

        public static void AddNotes(Member toMember, long createdby, string notes)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            CSNote note = new CSNote();
            try
            {
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                using (var cs = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                {
                    note.Note = notes;
                    note.MemberId = toMember.IpCode;
                    note.CreateDate = DateTime.Now;
                    note.CreatedBy = createdby;
                    cs.CreateNote(note);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }


        /// <summary>
        /// Merge CS notes after merge
        /// </summary>
        public static void MergeCsNotes(Member fromMember, Member toMember)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            CSNote fromCSNote = null;

            try
            {
                DateTime startDate = DateTime.Parse("1/1/2004");
                DateTime endDate = Convert.ToDateTime(DateTime.Now.ToString());
                using (var ilwcsservice = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                {
                    IList<CSNote> lstCsNotes = ilwcsservice.GetNotesByMember(fromMember.IpCode, startDate, endDate);

                    if (null != lstCsNotes && lstCsNotes.Count > 0)
                    {
                        for (int i = 0; i < lstCsNotes.Count; i++)
                        {
                            fromCSNote = (CSNote)lstCsNotes[i];
                            fromCSNote.MemberId = toMember.IpCode;
                            ilwcsservice.UpdateNote(fromCSNote);
                        }
                    }
                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Returns member details of a _member from cache
        /// </summary>
        /// <param name="member">Loyalty member</param>
        /// <returns>Loyalty member details</returns>
        public static MemberDetails GetMemberDetails(Member member)
        {
            MemberDetails memberDetails = null;

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                IList<IClientDataObject> lstMemberAttributes = member.GetChildAttributeSets("MemberDetails");
                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "lstMemberAttributes.Count: " + lstMemberAttributes.Count.ToString());
                if (null != lstMemberAttributes && lstMemberAttributes.Count > 0)
                {
                    memberDetails = (MemberDetails)lstMemberAttributes[0];
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "end");

            return memberDetails;
        }


        /// <summary>
        /// Set all null properties of to member which is not null in from member
        /// </summary>
        public static void SetMemberProperties(Member fromMember, Member toMember)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                // If Member data exists in the from Member but not in the to Member 
                foreach (PropertyInfo pi in fromMember.GetType().GetProperties())
                {
                    if (pi.CanRead)
                    {
                        object fromMemberAttributeValue = pi.GetValue(fromMember, null);
                        PropertyInfo toMemberPropertyInfo = toMember.GetType().GetProperty(pi.Name);
                        object toMemberAttributeValue = toMemberPropertyInfo.GetValue(toMember, null);
                        if (null != fromMemberAttributeValue && null == toMemberAttributeValue)
                        {
                            if (toMemberPropertyInfo.CanWrite)
                            {
                                toMemberPropertyInfo.SetValue(toMember, fromMemberAttributeValue, null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Set all null properties of to member details which is not null in from member details
        /// </summary>
        /// <param name="fromMemberDetails">Source member details </param>
        /// <param name="toMemberDetails">Destination member details</param>
        public static void SetMemberDetailProperties(MemberDetails fromMemberDetails, MemberDetails toMemberDetails)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                // If MemberDetails data exists in the from Member but not in the to Member 
                foreach (PropertyInfo pi in fromMemberDetails.GetType().GetProperties())
                {
                    if (pi.CanRead)
                    {
                        object fromMemberDetailsAttributeValue = pi.GetValue(fromMemberDetails, null);
                        PropertyInfo toMemberDetailsPropertyInfo = toMemberDetails.GetType().GetProperty(pi.Name);
                        object toMemberDetailsAttributeValue = toMemberDetailsPropertyInfo.GetValue(toMemberDetails, null);
                        if (null != fromMemberDetailsAttributeValue && null == toMemberDetailsAttributeValue )
                        {
                            if (toMemberDetailsPropertyInfo.CanWrite)
                            {
                                // AEO-757 begin
                                if ((Merge.IsFromProactiveMerge))
                                    toMemberDetailsPropertyInfo.SetValue(toMemberDetails, fromMemberDetailsAttributeValue, null);
                                else {
                                    if (pi.Name.ToLower() != "extendedplaycode")
                                    {
                                        toMemberDetailsPropertyInfo.SetValue(toMemberDetails, fromMemberDetailsAttributeValue, null);
                                    }                                    
                                }
                                // AEO-757 end
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }


        /// <summary>
        /// Sets the member source of destination member from source member
        /// </summary>
        /// <param name="fromMemberDetails">Source member details</param>
        /// <param name="toMemberDetails">Destination member details</param>
        public static void SetMemberSource(MemberDetails fromMemberDetails, MemberDetails toMemberDetails)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                if (null != fromMemberDetails && null != fromMemberDetails.MemberSource && fromMemberDetails.MemberSource.HasValue && ((fromMemberDetails.MemberSource == (int)MemberSource.OnlineAEEnrolled) || (fromMemberDetails.MemberSource == (int)MemberSource.OnlineAERegistered)))
                {
                    toMemberDetails.SetAttributeValue("MemberSource", fromMemberDetails.MemberSource);
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }

            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// <summary>
        /// Merge member receipts
        /// </summary>
        public static void MergeMemberReceipts(Member fromMember, Member toMember, string changedBy)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                {
                    LWCriterion crit = new LWCriterion("MemberReceipts");
                    crit.Add(LWCriterion.OperatorType.AND, "IpCode", fromMember.IpCode, LWCriterion.Predicate.Eq);
                    //   IList<IClientDataObject> lstMemberReceipts = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "MemberReceipts", crit, new LWQueryBatchInfo(), false); // AEO-74 Upgrade 4.5 here -----------SCJ
                    IList<IClientDataObject> lstMemberReceipts = ldService.GetAttributeSetObjects(null, "MemberReceipts", crit, null, false); // AEO-74 Upgrade 4.5 here -----------SCJ

                    if (null != lstMemberReceipts && lstMemberReceipts.Count > 0)
                    {
                        for (int i = 0; i < lstMemberReceipts.Count; i++)
                        {
                            MemberReceipts memberReceipt = (MemberReceipts)lstMemberReceipts[i];
                            memberReceipt.IpCode = toMember.IpCode;
                            memberReceipt.CHANGEDBY = changedBy;
                            fromMember.AddChildAttributeSet(memberReceipt);
                            ldService.SaveMember(fromMember);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// AEO-503 Begin
        /// <summary>
        /// Merge member tier
        /// </summary>
        /// 
        public static void MergeMemberTiers ( Member fromMember, Member toMember, string changedBy ) {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try {


                MemberTier mtf = fromMember.GetTier( DateTime.Now.Date);
                MemberTier mtt = toMember.GetTier(DateTime.Now.Date);
                TierDef tdf = mtf == null ? null : mtf.TierDef;
                TierDef tdt = mtt == null ? null : mtt.TierDef;

                using (var service = _dataUtil.LoyaltyDataServiceInstance())
                {

                    if (tdf != null)
                    {

                        if (tdt == null)
                        {
                            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "tdf != null and tdt == null");
                            toMember.MoveTierToMember(mtf);
                        }
                        else
                        {

                            if (tdf.Id == tdt.Id)
                            {

                                logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "tdf == tdt");
                                if (tdf.Description != null)
                                {
                                    if (tdf.Name.ToUpper().Trim() == "SILVER" && (mtf.Description.Contains("Nomination") || (mtf.Description == "Pilot Test Group"))
                                        && (!mtt.Description.Contains("Nomination") || mtt.Description != "Pilot Test Group"))
                                    {
                                        mtt.Description = (mtf.Description == null ? mtt.Description : mtf.Description);

                                    }
                                }
                                else
                                { }
                                mtt.ToDate = (DateTimeUtil.LessThan(mtt.ToDate, mtf.ToDate) ? mtf.ToDate : mtt.ToDate);
                                service.UpdateMemberTier(mtt);

                                mtf.ToDate = System.DateTime.Now.AddMinutes(-1.0);
                                service.UpdateMemberTier(mtf);

                            }
                            else
                            {

                                if (tdf.Name.ToUpper().Trim() == "SILVER")
                                {

                                    if (tdt.Name.ToUpper().Trim() == "BLUE")
                                    {

                                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "tdf == SILVER");
                                        //AEO-574 changes Begin here--------------------------SCJ
                                        //  toMember.MoveTierToMember(mtf);  Move Tier does not copy the member tier description ,so it stays null
                                        toMember.AddTier(mtf.TierDef.Name, mtf.FromDate, mtf.ToDate, mtf.Description);
                                        //AEO-574 changes end here--------------------------SCJ  
                                    }

                                }
                                else
                                {
                                    mtf.ToDate = System.DateTime.Now.AddMinutes(-1.0);
                                    service.UpdateMemberTier(mtf);
                                }

                            }
                        }

                    }

                }
            }
            catch ( Exception ex ) {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        /// AEO-503 End

        /// <summary>
        /// Merge member bra promo queue
        /// </summary>
        public static void MergeMemberPromoQueue(Member fromMember, Member toMember)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                using (var service = _dataUtil.LoyaltyDataServiceInstance())
                {
                    LWCriterion crit = new LWCriterion("MemberPromoQueue");
                    crit.Add(LWCriterion.OperatorType.AND, "IpCode", fromMember.IpCode, LWCriterion.Predicate.Eq);
                    //IList<IClientDataObject> lstMemberPromoQueue = LWDataServiceUtil.DataServiceInstance(true).GetAttributeSetObjects(null, "MemberPromoQueue", crit, new LWQueryBatchInfo(), false); // AEO-74 Upgrade 4.5 here -----------SCJ
                    IList<IClientDataObject> lstMemberPromoQueue = service.GetAttributeSetObjects(null, "MemberPromoQueue", crit, null, false); // AEO-74 Upgrade 4.5 here -----------SCJ

                    if (null != lstMemberPromoQueue && lstMemberPromoQueue.Count > 0)
                    {
                        for (int i = 0; i < lstMemberPromoQueue.Count; i++)
                        {
                            MemberPromoQueue promoQueue = (MemberPromoQueue)lstMemberPromoQueue[i];
                            promoQueue.IpCode = toMember.IpCode;
                            fromMember.AddChildAttributeSet(promoQueue);
                            service.SaveMember(fromMember);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw;
            }
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
        }

        public static String GetFromLoyaltyID(Member member)
        {
            foreach (VirtualCard vc in member.LoyaltyCards)
            {
                if (!vc.IsPrimary)
                {
                    return vc.LoyaltyIdNumber;
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Check if brand flag exists in ToMember's Member Brand records
        /// </summary>
        public static bool checkForBrandFlagInToMember(Member toMember, long brandID)
        {
            logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");

            IList<IClientDataObject> listToMemberBrands = toMember.GetChildAttributeSets("MemberBrand");
            if (listToMemberBrands != null && listToMemberBrands.Count > 0)
            {
                for (int i = 0; i < listToMemberBrands.Count; i++)
                {
                    MemberBrand toMemberBrand = (MemberBrand)listToMemberBrands[i];
                    if (toMemberBrand.BrandID != null && toMemberBrand.BrandID == brandID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }


    
}
