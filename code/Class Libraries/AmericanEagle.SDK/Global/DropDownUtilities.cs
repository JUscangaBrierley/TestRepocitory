//-----------------------------------------------------------------------
// <copyright file="DropDownUtilities.cs" company="B+P">
//     Copyright (c) B+P. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace AmericanEagle.SDK.Global
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.UI.WebControls;
    using Brierley.ClientDevUtilities.LWGateway;
    //using Brierley.DNNModules.PortalModuleSDK.Controls;
    using Brierley.FrameWork.Common.Logging;
    using Brierley.FrameWork.Data;
    using Brierley.FrameWork.Data.DomainModel;
    #endregion

    /// <summary>
    /// Class for drop down list methods
    /// </summary>
    public class DropDownUtilities
    {
        /// <summary>
        /// Object for logging
        /// </summary>
        private static LWLogger logger = LWLoggerManager.GetLogger("RewardHistory");
        private static ILWDataServiceUtil _dataUtil = Brierley.ClientDevUtilities.LWGateway.LWDataServiceUtil.Instance;

        /// <summary>
        /// Method to get quarters
        /// </summary>
        /// <param name="member">Member member</param>        
        /// <returns>Number of quarters</returns>
        public static int NumberOfQuarters(Member member)
        {
            int qtr = 0;
            DateTime dt = new DateTime(1, 1, 1);
            try
            {
                if (null != member)
                {
                    using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                    {
                        IList<MemberReward> lstRewards = ldService.GetMemberRewards(member, null);
                        if (lstRewards.Count > 0)
                        {
                            DateTime minDate = lstRewards.Min(x => x.DateIssued);
                            qtr = ((DateTime.Now.Year - minDate.Year) + 1) * 4;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw ex;
            }

            return qtr;
        }

        /// <summary>
        /// Returns list items for specified date
        /// </summary>
        /// <param name="pdtDate">DateTime pdtDate</param>
        /// <returns>List item to be filled in dropdown</returns>
        public static ListItem GetListItem(DateTime pdtDate)
        {
            return new ListItem(GetQuarterDate(pdtDate, true) + " to " + DropDownUtilities.GetQuarterDate(pdtDate, false), GetQuarterDate(pdtDate, true) + " to " + GetQuarterDate(pdtDate, false));
        }
        /// <summary>
        /// Returns the start date and end quarter date of specified date
        /// </summary>
        /// <param name="parmDtDate">DateTime parmDtDate</param>
        /// <param name="parmBlnIsStart">True: for first date of quarter False: for last date of quarter</param>
        /// <returns>Date string</returns>
        public static string GetQuarterDate(DateTime parmDtDate, bool parmBlnIsStart)
        {
            string strRetDate = string.Empty;
            int parmIntYear = parmDtDate.Year;
            try
            {
                switch (parmDtDate.Month)
                {
                    case 1:
                    case 2:
                    case 3:
                        if (parmBlnIsStart)
                        {
                            strRetDate = "1/1/" + parmIntYear.ToString();
                        }
                        else
                        {
                            strRetDate = "3/31/" + parmIntYear.ToString();
                        }

                        break;
                    case 4:
                    case 5:
                    case 6:
                        if (parmBlnIsStart)
                        {
                            strRetDate = "4/1/" + parmIntYear.ToString();
                        }
                        else
                        {
                            strRetDate = "6/30/" + parmIntYear.ToString();
                        }

                        break;
                    case 7:
                    case 8:
                    case 9:
                        if (parmBlnIsStart)
                        {
                            strRetDate = "7/1/" + parmIntYear.ToString();
                        }
                        else
                        {
                            strRetDate = "9/30/" + parmIntYear.ToString();
                        }

                        break;
                    default:
                        if (parmBlnIsStart)
                        {
                            strRetDate = "10/1/" + parmIntYear.ToString();
                        }
                        else
                        {
                            strRetDate = "12/31/" + parmIntYear.ToString();
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw ex;
            }

            return strRetDate;
        }


        /// <summary>
        /// Remove date ranges when this.member does not acquire any points
        /// </summary>
        /// <param name="parmList">Entire list of ranges</param>
        /// <param name="member">Member member</param>
        /// <returns>List of list items</returns>
        public static List<ListItem> FilterListItems(List<ListItem> parmList, Member member)
        {
            List<ListItem> li = new List<ListItem>();
            try
            {
                foreach (ListItem item in parmList)
                {
                    string strDateRange = item.Text;
                    if (!string.IsNullOrEmpty(strDateRange))
                    {
                        string strStartDate = strDateRange.Substring(0, strDateRange.IndexOf("to"));
                        string strEndDate = strDateRange.Substring(strDateRange.IndexOf("to") + 3);
                        DateTime dateStartDate = DateTime.Now;
                        DateTime dateEndDate = DateTime.Now;
                        DateTime.TryParse(strStartDate, out dateStartDate);
                        DateTime.TryParse(strEndDate, out dateEndDate);
                        using (var ldService = _dataUtil.LoyaltyDataServiceInstance())
                        {
                            IList<MemberReward> lstRewards = ldService.GetMemberRewards(member, null);
                            if (lstRewards.Count != 0)
                            {
                                li.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex);
                throw ex;
            }

            return li;
        }
    }
}