using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Logging;
using Brierley.WebFrameWork;
using Brierley.WebFrameWork.Portal;
using Brierley.ClientDevUtilities.LWGateway;

//LW 4.1.14 changes
//using Brierley.DNNModules.PortalModuleSDK;
//using DotNetNuke.Entities.Users;

namespace AmericanEagle.SDK.Global
{
    public static class WebUtilities
    {
        private static LWLogger logger = LWLoggerManager.GetLogger("WebUtilities");
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;
        /// <summary>
        /// Method to determine whether logged in user has given right or not
        /// </summary>
        /// <returns>bool value</returns>
        public static bool AllowAppeasementAssignmentToMemeber()
        {
            bool result = false;
            using (var cService = _dataUtil.ContentServiceInstance())
            {
                string assignedRole = WebUtilities.GetCurrentUserRole().ToLower();

                if (assignedRole == "loss prevention" || assignedRole == "synchrony") //AEO-1602
                {
                    result = false;
                }
                else if (assignedRole == "super admin")
                {
                    result = true;
                }
                /* AEO-2173 begin
                else if ( assignedRole == "supervisor" || assignedRole == "csr" ||
                          assignedRole == "synchrony csr" || assignedRole == "Synchrony Admin" ) //AEO-1602 & AEO-1881
                AEO-2173 end */
                else if (assignedRole == "supervisor" || assignedRole == "csr" ||
                          assignedRole == "synchrony csr" || assignedRole == "synchrony admin") //AEO-1602 & AEO-1881 & AEO-2173
                {
                    int appeasementCount = 0;
                    DateTime startDate;
                    DateTime endDate;
                    Member member = WebUtilities.GetLoyaltyMemberFromCache();
                    Utilities.GetProgramDates(member, out startDate, out endDate); // PI 30364 - Dollar reward program - Start
                    IList<MemberReward> memberRewards = member.GetRewards().Where(item => item.DateIssued >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0) && item.DateIssued <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 59, 59)).ToList();

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Number of Member Rewards: " + memberRewards.Count.ToString());
                    foreach (MemberReward reward in memberRewards)
                    {
                        RewardDef rewardDef = cService.GetRewardDef(reward.RewardDefId);
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward: " + rewardDef.Name);
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward attributes: " + rewardDef.Attributes.Count.ToString());
                        if (null != rewardDef && null != rewardDef.Attributes)
                        {
                            //foreach (RewardAttribute attrib in rewardDef.Attributes)
                            //{
                            //    RewardAttributeDef attribDef = dataService.GetRewardAttributeDef(attrib.RewardAttributeDefId);
                            //    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward attribute name: " + attribDef.Name);
                            //    if ((attribDef.Name.ToUpper() == "ISAPPEASEMENTS") && (attrib.Value.ToUpper() == "TRUE"))
                            //    {
                            //        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Appeasement " + rewardDef.Name);
                            //        appeasementCount++;
                            //    }
                            //}
                            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                            //RewardAttribute rewardAttribute = rewardDef.Attributes.Where(attribute => dataService.GetRewardAttributeDef(attribute.RewardAttributeDefId).Name == "IsAppeasement" && attribute.Value.ToUpper() == "TRUE").FirstOrDefault();
                            ContentAttribute rewardAttribute = rewardDef.Attributes.Where(attribute => cService.GetContentAttributeDef(attribute.ContentAttributeDefId).Name == "IsAppeasement" && attribute.Value.ToUpper() == "TRUE").FirstOrDefault();
                            if (rewardAttribute != null)
                            {
                                appeasementCount++;
                            }
                        }
                    }


                    if (appeasementCount >= 2)
                        result = false;
                    else
                        result = true;
                }
            }
            return result;
        }

        //AEO-
        /// <summary>
        /// Method to determine whether logged in user has given right or not
        /// based on the given reward name.
        /// </summary>
        /// <returns>bool value</returns>
        public static bool AllowAppeasementAssignmentToMemeber(string RewardName)
        {
            bool result = false;
            using (var cService = _dataUtil.ContentServiceInstance())
            {
                string assignedRole = WebUtilities.GetCurrentUserRole().ToLower();
                int appeaseLimit = AppeasementLimits.GetLimitFor(assignedRole, RewardName);
                string RewardType = string.Empty;
                string changedBy = string.Empty;
                string userName = WebUtilities.GetCurrentUserName();

                if (assignedRole == "super admin")
                {
                    result = true;
                }
                else
                {
                    int appeasementCount = 0;
                    DateTime startDate;
                    DateTime endDate;
                    Member member = WebUtilities.GetLoyaltyMemberFromCache();
                    Utilities.GetProgramDates(member, out startDate, out endDate); // PI 30364 - Dollar reward program - Start
                    IList<MemberReward> memberRewards = member.GetRewards().Where(item => item.DateIssued >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0) && item.DateIssued <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)).ToList();

                    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Number of Member Rewards: " + memberRewards.Count.ToString());
                    foreach (MemberReward reward in memberRewards)
                    {
                        RewardDef rewardDef = cService.GetRewardDef(reward.RewardDefId);
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward: " + rewardDef.Name);
                        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward attributes: " + rewardDef.Attributes.Count.ToString());
                        if (null != rewardDef && null != rewardDef.Attributes)
                        {
                            //foreach (RewardAttribute attrib in rewardDef.Attributes)
                            //{
                            //    RewardAttributeDef attribDef = dataService.GetRewardAttributeDef(attrib.RewardAttributeDefId);
                            //    logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Reward attribute name: " + attribDef.Name);
                            //    if ((attribDef.Name.ToUpper() == "ISAPPEASEMENTS") && (attrib.Value.ToUpper() == "TRUE"))
                            //    {
                            //        logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Appeasement " + rewardDef.Name);
                            //        appeasementCount++;
                            //    }
                            //}
                            // AEO-74 Upgrade 4.5 changes BEGIN here -----------SCJ
                            //RewardAttribute rewardAttribute = rewardDef.Attributes.Where(attribute => dataService.GetRewardAttributeDef(attribute.RewardAttributeDefId).Name == "IsAppeasement" && attribute.Value.ToUpper() == "TRUE").FirstOrDefault();
                            changedBy = string.IsNullOrEmpty(reward.ChangedBy) ? string.Empty : reward.ChangedBy; // AEO-1972 In case ChangedBy is null, cast to empty string.
                            ContentAttribute rewardAttribute = rewardDef.Attributes.Where(attribute => cService.GetContentAttributeDef(attribute.ContentAttributeDefId).Name == "IsAppeasement" && attribute.Value.ToUpper() == "TRUE").FirstOrDefault();
                            if (rewardAttribute != null && rewardDef.Name == RewardName && changedBy == userName) // AEO-1972 Fix for FSD requirements. Count 
                            {
                                appeasementCount++;
                            }
                        }
                    }


                    if (appeasementCount >= appeaseLimit)
                        result = false;
                    else
                        result = true;
                }
            }
            return result;
        }


        /// <summary>
        /// Method to determine role of logged in user
        /// </summary>
        /// <returns>bool value</returns>
        public static string GetCurrentUserRole()
        {
            using (var cs = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
            {
                string username = WebUtilities.GetCurrentUserName();
                CSAgent agent = cs.GetCSAgentByUserName(username, Brierley.FrameWork.Common.AgentAccountStatus.Active);

                if (null != agent)
                {
                    CSRole role = cs.GetRole(agent.RoleId, true);
                    if (null != role)
                    {
                        return role.Name.Trim();
                    }
                }

                return string.Empty;
            }
        }
        public static Member GetLoyaltyMemberFromCache()
        {
            Member member = (Member)PortalState.GetFromCache("SelectedMember");

            return member;
        }
        public static String GetCurrentUserName()
        {
            string userName = "(anonymous user)";
            //LW 4.1.14 change
            //UserInfo userInfo = UserController.GetCurrentUserInfo();
            CSAgent agent = PortalState.GetLoggedInCSAgent();

            if (agent != null)
            {
                userName = agent.Username;
            }
            return userName;
        }

        public static long GetCurrentUserId()
        {
            long userId = -1;
            string userName = GetCurrentUserName();

            if (userName.Length > 0)
            {
                using (var inst = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                {
                    CSAgent agent = inst.GetCSAgentByUserName(userName, Brierley.FrameWork.Common.AgentAccountStatus.Active);
                    userId = agent.Id;
                }
            }

            return userId;
        }
        /// <summary>
        /// Determine whether reward is replaceable or not.
        /// </summary>
        /// <param name="mbrReward">MemberReward mbrReward</param>
        /// <param name="parmRewardDef">RewardDef parmRewardDef</param>
        /// <returns>1 for replaceable and 0 for not</returns>
        public static int IsRewardReplaceable(MemberReward mbrReward, RewardDef parmRewardDef)
        {
            string description = string.Empty;
            if (null != parmRewardDef)
            {
                description = parmRewardDef.Name != null ? parmRewardDef.Name : string.Empty;
            }

            DateTime qtrStartDate;
            DateTime qtrEndDate;
            // PI 30364 - Dollar reward program - Start
            Member member = WebUtilities.GetLoyaltyMemberFromCache();
            Utilities.GetProgramDates(member, out qtrStartDate, out qtrEndDate);
            // PI 30364 - Dollar reward program - End
            if (mbrReward.DateIssued > qtrStartDate && mbrReward.DateIssued < qtrEndDate)
            {
                //if (mbrReward.OrderNumber != RewardStatus.Merged.ToString())      // AEO-74 Upgrade 4.5 changes here -----------SCJ
                if (mbrReward.LWOrderNumber != RewardStatus.Merged.ToString())      // AEO-74 Upgrade 4.5 changes here -----------SCJ
                {
                    if (mbrReward.RedemptionDate != null)
                    {
                        if (HasRights("ReplaceRewards"))
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        if (description.Contains("Reward"))
                        {
                            if (description.Contains("Bra"))
                            {
                                return 0;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        /// Method to determine whether logged in user has given right or not
        /// </summary>
        /// <param name="parmFunction">right</param>
        /// <returns>True: if has rights, false: no rights</returns>
        public static bool HasRights(string parmFunction)
        {
            string username = GetCurrentUserName();
            using (var cs = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
            {
                CSAgent agent = cs.GetCSAgentByUserName(username, Brierley.FrameWork.Common.AgentAccountStatus.Active);
                if (null == agent)
                {
                    return false;
                }

                CSRole role = cs.GetRole(agent.RoleId, true);
                if (role.HasFunction(parmFunction))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // AEO-1644 Begin
        /// <summary>
        /// Return if the member is able to submit changes
        /// </summary>       
        public static Boolean isMemberAllowedToSubmit()
        {
            Boolean bReturn = false;
            Member member = PortalState.GetFromCache("SelectedMember") as Member;
            if (member != null)
            {
                if (member.MemberStatus == Brierley.FrameWork.Common.MemberStatusEnum.Terminated)
                {
                    bReturn = true;
                }
            }

            return bReturn;
        }
        // AEO-1644 end 

        // AEO-1602 Begin
        /// <summary>
        /// Return if the role is able to only view the page
        /// </summary>  
        public static Boolean isRoleAllowedOnlyToView(string bPage)
        {
            Boolean bReturn = false;
            string assignedRole = WebUtilities.GetCurrentUserRole().ToLower();
            switch (bPage)
            { 
                case "MemberStatus":
                    if (assignedRole == "synchrony")
                    {
                        bReturn = true;
                    }
                    else 
                    {
                        if (assignedRole == "supervisor" || assignedRole == "csr")
                        {
                            Member member = PortalState.GetFromCache("SelectedMember") as Member;
                            if (member != null)
                            {
                                if (member.MemberStatus == Brierley.FrameWork.Common.MemberStatusEnum.Locked)
                                {
                                    bReturn = true;
                                }
                            }
                        }
                    }

                    break;

                case "ViewMemberTier":
                    if (assignedRole == "loss prevention" /*|| assignedRole == "synchrony" AEO-1881 MMV*/)
                    {
                        
                        bReturn = true;
                    }

                    // AEO-1881 MMV begin
                    using (var cs = Brierley.FrameWork.Data.LWDataServiceUtil.CSServiceInstance())
                    {
                        if (assignedRole.Contains("synchrony"))
                        {

                            CSAgent agent = cs.GetCSAgentById(WebUtilities.GetCurrentUserId());

                            if (agent != null)
                            {

                                CSRole role = cs.GetRole(agent.RoleId, true);
                                if (role != null)
                                {

                                    IList<CSFunction> privileges = role.Functions;
                                    if (privileges != null && privileges.Count != 0)
                                    {

                                        Boolean updateAllowed = false;

                                        foreach (CSFunction p in privileges)
                                        {
                                            if (p.Name.ToUpper() == "MODIFYTIER")
                                            {
                                                updateAllowed = true;
                                                break;
                                            }

                                        }

                                        if (!updateAllowed)
                                        {
                                            bReturn = true;
                                        }


                                    }
                                }
                            }
                        }
                    }
                    // AEO-1881 MMV end

                    break;

                case "ViewAwardPoints":
                    if (assignedRole == "loss prevention" || assignedRole == "synchrony")
                    {
                        bReturn = true;
                    }
                    break;

                case "ViewRequestCredit":
                    if (assignedRole == "loss prevention" || assignedRole == "synchrony")
                    {
                        bReturn = true;
                    }
                    break;

                case "ViewRewardAppeasement":
                    if ( assignedRole != "csr" && assignedRole != "supervisor" &&
                         assignedRole != "super admin" && assignedRole != "synchrony admin" &&
                         assignedRole != "synchrony csr" ) // AEO-1881 begin & end
                    {
                        bReturn = true;
                    }
                    break;

                default:
                    bReturn = false;
                    break;
            }

            return bReturn;
        }
        // AEO-1602 end    
    }

}
