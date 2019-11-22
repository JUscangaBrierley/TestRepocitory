using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Ipc;

using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data;
using Brierley.Clients.AmericanEagle.DataModel;
using AmericanEagle.SDK.Global;
using Brierley.ClientDevUtilities.LWGateway;

namespace Brierley.AEModules.MemberCustomDropdown
{
    public partial class ViewMemberCustomDropdown : ModuleControlBase, IIpcEventHandler
    {
        private static LWLogger _logger = LWLoggerManager.GetLogger("MemberCustomDropdown");
        private static ILWDataServiceUtil _dataUtil = new ClientDevUtilities.LWGateway.LWDataServiceUtil(); //AEO-2630
        private const int numberOfQuarters = 13;
        private Member member = null;

        public ModuleConfigurationKey GetConfigurationKey()
        {
            return ConfigurationKey;
        }
        public void HandleEvent(IpcEventInfo info)
        {
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Handling IPC event: " + info.EventName);
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "PageName: " + PortalState.CurrentPage.Name);

            try
            {                
                if (PortalState.CurrentPage.Name == "Account Summary")
                {
                    this.SetStartDateNR();
                }

                if (PortalState.CurrentPage.Name == "Account Activity")
                {
                    member = PortalState.GetFromCache("SelectedMember") as Member;
                    //Save date for AccountGridProvider.cs
                    ListItem item = this.GetFirstRangeDates();
                    PortalState.PutInCache("MemberAccountDate", "Init:" + item.Text);
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "PutInCache MemberAccountDate: " + "Init:" + item.Text);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "PageName: " + PortalState.CurrentPage.Name);

                if (PortalState.CurrentPage.Name == "Account Summary")
                {
                    tdPointBalance.Visible = false;
                    tdBrandPurchased.Visible = false;
                    tdMonthlyPointBalance.Visible = false;
                    ddlDate.Visible = false;

                    //AEO-2108 Begin - CSPortal Date Drop Down
                    this.SetStartDateNR();
                    //AEO-2108 End
                }

                if (PortalState.CurrentPage.Name == "Account Activity")
                {
                    member = PortalState.GetFromCache("SelectedMember") as Member;

                    if (!IsPostBack)
                    {
                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "PageLoad - not postback: ");
                        this.Bind_Dropdown();
                        PortalState.PutInCache("AccountActivityDate", ddlDate.SelectedItem.Value); // AEO-432 MMV Begin & end
                    }
                    else
                    {
                        _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "PostBack: " + ddlDate.SelectedItem.Value);
                    }

                    // PI 30364 - Dollar reward program - Start
                    String strDateRange = ddlDate.SelectedItem.Value;
                    DateTime dtStartDate = DateTime.Now;
                    DateTime dtEndDate = DateTime.Now;
                    tdMonthlyPointBalance.Visible = true;
                    Utilities.GetRangeDates(strDateRange, ref dtStartDate, ref dtEndDate);
                    lblMonthlyPointBalanceValue.Text = Convert.ToString(Utilities.GetPointsBalance(member, dtStartDate, dtEndDate)); //Current Point Balance for your selection
                    lblPointBalanceValue.Text = Convert.ToString(Utilities.GetPointsBalance(member, new DateTime(1990, 1, 1), dtEndDate)); //Current point Balance
                    DisplayPurchaseBrands(dtStartDate, dtEndDate);
                    //AEO-1793 End
                }

            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
            }
        }
        
        private void DisplayPurchaseBrands(DateTime dtStartDate, DateTime dtEndDate)
        {
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                Member member = PortalState.GetFromCache("SelectedMember") as Member;
                if (member != null)
                {
                    IList<IClientDataObject> lstMemberBrand = member.GetChildAttributeSets("MemberBrand");
                    Decimal dblPointBalance = member.GetPoints(dtStartDate, dtEndDate);
                    if (dblPointBalance > 0)
                    {
                        foreach (MemberBrand brand in lstMemberBrand)
                        {
                            if (null != ChkLBrandPurchased.Items.FindByValue(brand.BrandID.ToString()))
                            {
                                ChkLBrandPurchased.Items.FindByValue(brand.BrandID.ToString()).Selected = true;
                            }
                        }
                    }
                }
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
                ShowWarning("An unexpected error has occurred. Click on your browser's 'back' button to retry. If the error persists, please contact the appropriate support personnel.");
            }
        }

        /// <summary>
        /// Bind dropdown with number of quarters saved in config
        /// </summary>
        private void BindDropdown()
        {
            List<ListItem> lc = new List<ListItem>();
            DateTime dtStartDate = DateTime.Now;
            DateTime dtEndDate = DateTime.Now;
            //int i = 0;
            // PI 30364 - Dollar reward program changes, Start
            int numofQuarters = numberOfQuarters;
            // AEO-73 : CS Portal runtime error - Start
            bool isPilotMember = false;
            member = PortalState.GetFromCache("SelectedMember") as Member;
            // AEO-57 Changes start
            if (member != null)
            {
                MemberDetails mbrDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                isPilotMember = (mbrDetails != null && Utilities.MemberIsInPilot(mbrDetails.ExtendedPlayCode) ); // AEO-410
            }
            if (isPilotMember)
            {
                for ( int i = 0; i < numofQuarters; i++ )
                {
                    DateTime quarterEndDate = DateTime.Now.AddMonths(-i * 3);
                    DateTime.TryParse(GetQuarterDate(DateTime.Now.AddMonths(-i * 3), false), out quarterEndDate);
                                      
                        lc.Add(GetListItem(quarterEndDate));
                  
                }
                // PI 30364 - Dollar reward program changes, End
                // AEO-57 Changes End
                // First quarter start date
                DateTime.TryParse(lc[lc.Count - 1].Text.Substring(0, lc[lc.Count - 1].Text.IndexOf("to")), out dtStartDate);
                // Last quarter end date
                DateTime.TryParse(lc[0].Text.Substring(lc[0].Text.IndexOf("to") + 3), out dtEndDate);
                dtEndDate = dtEndDate.AddDays(1).AddSeconds(-1);
                lc = FilterListItems(lc, dtStartDate, dtEndDate);
            }
            else
            {
                // AEO-73 : CS Portal runtime error - End
                for (int i = 0; i < numofQuarters; i++)
                {
                    DateTime quarterEndDate = DateTime.Now.AddMonths(-i * 3);
                    DateTime.TryParse(GetQuarterDate(DateTime.Now.AddMonths(-i * 3), false), out quarterEndDate);

                    if (isPilotMember == true && (quarterEndDate > Utilities.DollarRewardsProgramStartDate && quarterEndDate < Utilities.DollarRewardsProgramEndDate))
                    {
                        lc.Add(GetListItem(quarterEndDate, isPilotMember));
                        lc.Add(GetListItem(quarterEndDate.AddMonths(-1), isPilotMember));
                        lc.Add(GetListItem(quarterEndDate.AddMonths(-2), isPilotMember));
                    }
                    else
                    {
                        lc.Add(GetListItem(quarterEndDate));
                    }
                }
                // PI 30364 - Dollar reward program changes, End
                // AEO-57 Changes End
                // First quarter start date
                DateTime.TryParse(lc[lc.Count - 1].Text.Substring(0, lc[lc.Count - 1].Text.IndexOf("to")), out dtStartDate);
                // Last quarter end date
                DateTime.TryParse(lc[0].Text.Substring(lc[0].Text.IndexOf("to") + 3), out dtEndDate);
                dtEndDate = dtEndDate.AddDays(1).AddSeconds(-1);
                lc = FilterListItems(lc, dtStartDate, dtEndDate);
            }
            if (lc.Count == 0)
            {
                lc.Add(GetListItem(DateTime.Today));
            }
            ddlDate.DataSource = lc;
            ddlDate.DataBind();
      
           
        }

        /// <summary>
        /// Returns list items for specified date
        /// </summary>
        /// <param name="pdtDate">Date</param>
        /// <returns>Listitem to be filled in dropdown</returns>
        private ListItem GetListItem(DateTime pdtDate, bool isPilotMember = false)  // AEO-57 Changes
        {
            // AEO-57 Changes
            return new ListItem(GetProgramDate(pdtDate, true, isPilotMember) + " to " + GetProgramDate(pdtDate, false, isPilotMember), GetProgramDate(pdtDate, true, isPilotMember) + " to " + GetProgramDate(pdtDate, false, isPilotMember)); // PI 30364 - Dollar reward program
        }
        /// <summary>
        /// Returns the start date and end quarter date of specified date
        /// </summary>
        /// <param name="pDtDate">Date</param>
        /// <param name="pBlnIsStart">True: for first date of quarter False: for last date of quarter</param>
        /// <returns>Date string</returns>
        private String GetQuarterDate(DateTime pDtDate, Boolean pBlnIsStart)
        {
            String StrRetDate = String.Empty;
            Int32 pIntYear = pDtDate.Year;
            switch (pDtDate.Month)
            {
                case 1:
                case 2:
                case 3:
                    if (pBlnIsStart)
                    {
                        StrRetDate = "1/1/" + pIntYear.ToString();
                    }
                    else
                    {
                        StrRetDate = "3/31/" + pIntYear.ToString();
                    }
                    break;
                case 4:
                case 5:
                case 6:
                    if (pBlnIsStart)
                    {
                        StrRetDate = "4/1/" + pIntYear.ToString();
                    }
                    else
                    {
                        StrRetDate = "6/30/" + pIntYear.ToString();
                    }
                    break;
                case 7:
                case 8:
                case 9:
                    if (pBlnIsStart)
                    {
                        StrRetDate = "7/1/" + pIntYear.ToString();
                    }
                    else
                    {
                        StrRetDate = "9/30/" + pIntYear.ToString();
                    }
                    break;
                default:
                    if (pBlnIsStart)
                    {
                        StrRetDate = "10/1/" + pIntYear.ToString();
                    }
                    else
                    {
                        StrRetDate = "12/31/" + pIntYear.ToString();
                    }
                    break;
            }
            return StrRetDate;
        }
        /// <summary>
        /// Returns the start date and end date of month of specified date
        /// Added for PI 30364 - Quick Wins - Phase I loyalty enhancements - Dollar Rewards
        /// </summary>
        /// <param name="pDtDate"></param>
        /// <param name="pBlnIsStart"></param>
        /// <returns></returns>
        private String GetProgramDate(DateTime pDtDate, Boolean pBlnIsStart, Boolean isPilotMember = false) // AEO-57 Changes
        {
            String StrRetDate = String.Empty;
            Int32 pIntYear = pDtDate.Year;
            Int32 pIntMonth = pDtDate.Month;
            Int32 pDay = 31;

            if (isPilotMember)
            {
                if (pBlnIsStart)
                {
                    StrRetDate = pDtDate.Month + "/" + "1/" + pIntYear.ToString();
                }
                else
                {
                    switch (pIntMonth)
                    {
                        case 2:
                            if (pIntYear % 4 == 0)
                            {
                                pDay = 29;
                            }
                            else
                            {
                                pDay = 28;
                            }
                            break;

                        case 4:
                        case 6:
                        case 9:
                        case 11:
                            pDay = 30;
                            break;
                        default:
                            pDay = 31;
                            break;
                    }
                    StrRetDate = pDtDate.Month + "/" + pDay + "/" + pIntYear.ToString();
                }
            }
            else
            {
                StrRetDate = GetQuarterDate(pDtDate, pBlnIsStart);
            }
            return StrRetDate;
        }

        /// <summary>
        /// Remove date ranges when member does not aquire any points
        /// </summary>
        /// <param name="pList">Entire list of ranges</param>
        /// <param name="firstQuarterStartDate">First quarter start date</param>
        /// <param name="lastQuarterEndDate">Last quarter end date</param>
        /// <returns>List of list items</returns>
        private List<ListItem> FilterListItems(List<ListItem> pList, DateTime firstQuarterStartDate, DateTime lastQuarterEndDate)
        {
            List<ListItem> li = new List<ListItem>();
            try
            {
                using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance()) {
                    // Fetch all the point transactions of the member between start date of first quarter and end date of last quarter
                    IList<PointTransaction> objMemberPointTransactions = dataService.GetPointTransactions(member, firstQuarterStartDate, lastQuarterEndDate, string.Empty, string.Empty, 0, int.MaxValue, true);

                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "firstQuarterStartDate: " + firstQuarterStartDate.ToShortDateString() + ", lastQuarterEndDate: " + lastQuarterEndDate.ToShortDateString() + ", Count: " + objMemberPointTransactions.Count().ToString());

                    foreach (ListItem item in pList)
                    {
                        if (!pList[0].Text.Equals(item.Text))
                        {
                            String strDateRange = item.Text;
                            if (!String.IsNullOrEmpty(strDateRange))
                            {
                                String strStartDate = strDateRange.Substring(0, strDateRange.IndexOf("to"));
                                String strEndDate = strDateRange.Substring(strDateRange.IndexOf("to") + 3);
                                DateTime dtStartDate = DateTime.Now;
                                DateTime dtEndDate = DateTime.Now;
                                DateTime.TryParse(strStartDate, out dtStartDate);
                                DateTime.TryParse(strEndDate, out dtEndDate);
                                dtEndDate = dtEndDate.AddDays(1).AddSeconds(-1);
                                // Add only quarters in which member earn any points
                                var lTransCount = from pt in objMemberPointTransactions
                                                  where pt.TransactionDate >= dtStartDate && pt.TransactionDate <= dtEndDate
                                                  select pt.RowKey;

                                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "StartDate: " + dtStartDate.ToShortDateString() + ", EndDate: " + dtEndDate.ToShortDateString() + ", Count: " + lTransCount.Count().ToString());
                                if (lTransCount.Count() > 0)
                                {
                                    li.Add(item);
                                }
                            }
                        }
                        else
                        {
                            li.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNegative(ex.ToString());
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return li;
        }

        private void DisplayPointBalance()
        {
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "Begin");
            try
            {
                // AEO-73 : CS Portal runtime error - Start
                bool isPilotMember = false;
                Member member = PortalState.GetFromCache("SelectedMember") as Member;

                if (member != null)
                {

                    MemberDetails mbrDetails = member.GetChildAttributeSets("MemberDetails").FirstOrDefault() as MemberDetails;
                    isPilotMember = (mbrDetails != null && Utilities.MemberIsInPilot( mbrDetails.ExtendedPlayCode) && DateTime.Now >= Utilities.DollarRewardsProgramStartDate && DateTime.Now <= Utilities.DollarRewardsProgramEndDate); //AEO-401
                    // AEO-73 : CS Portal runtime error - End
                    String strDateRange = ddlDate.SelectedItem.Value;
                    String strStartDate = strDateRange.Substring(0, strDateRange.IndexOf("to"));
                    String strEndDate = strDateRange.Substring(strDateRange.IndexOf("to") + 3);
                    DateTime dtStartDate = DateTime.Now;
                    DateTime dtEndDate = DateTime.Now;
                    DateTime.TryParse(strStartDate, out dtStartDate);
                    DateTime.TryParse(strEndDate, out dtEndDate);
                    // PI 30364 - Dollar reward program, Start
                    Utilities.GetProgramStartEndDates(member, ref dtStartDate, ref dtEndDate);
                    DateTime.TryParse(strEndDate, out dtEndDate);
                    // PI 30364 - Dollar reward program, End
                    dtEndDate = dtEndDate.AddDays(1);
                    dtEndDate = dtEndDate.AddSeconds(-1);

                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "StartDate: " + dtStartDate.ToShortDateString() + ", EndDate: " + dtEndDate.ToShortDateString());

                    // AEO-532 BEGIN

                    if ( isPilotMember ) {
                        dtStartDate = DateTime.Compare(dtStartDate, new DateTime(1990,1,1)) > 0 ? new DateTime(1990,1,1) : startDate;
                        dtEndDate = DateTime.Now.Date.AddDays(1);
                    }
                    // AEO-532 END

                    lblPointBalanceValue.Text = Convert.ToString(Utilities.GetPointsBalance(member, dtStartDate, dtEndDate));
                }
                _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "End");
            }
            catch (Exception ex)
            {
                ShowNegative(ex.ToString());
                _logger.Error(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        protected void ddlDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            PortalState.PutInCache("MemberAccountDate", ddlDate.SelectedItem.Value);
            _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "PutInCache MemberAccountDate: " + ddlDate.SelectedItem.Value.ToString());
            IpcManager.PublishEvent("MemberUpdated", new ModuleConfigurationKey(PortalModules.Custom, "MemberUpdatedConfig"), member);
        }

        //AEO-1793 Begin - Date drop down Member Account Summary
        /// <summary>
        /// Bind dropdown
        /// </summary>
        private void Bind_Dropdown()
        {
            try
            {
                List<ListItem> lc = new List<ListItem>();
                string sAllActivity = string.Empty;
                sAllActivity = new DateTime(1990, 1, 1, 00, 00, 00).ToString() + " to " + new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).ToString();

                lc.Add(GetFirstRangeDates()); //Range of dates from the current year
                lc = this.GetMonthsRangeDates(lc);//Rolling 12 Months from the current Month
                lc = this.GetYearsRangeDates(lc);//Years from members activity
                lc.Add(new ListItem("All Activity", sAllActivity));//All Activity
                foreach (ListItem Li in lc)
                {
                    ddlDate.Items.Add(new ListItem(Li.Text, Li.Value));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Range of dates from the current year
        /// </summary>
        private ListItem GetFirstRangeDates()
        {
            try
            {
                DateTime dtStartDate = DateTime.Now;
                DateTime dtEndDate = DateTime.Now;

                dtStartDate = new DateTime(dtStartDate.Year, 1, 1, 00, 00, 00);
                dtEndDate = new DateTime(dtEndDate.Year, dtEndDate.Month, 1).AddMonths(1).AddDays(-1);
                return (new ListItem(dtStartDate.ToShortDateString() + " to " + dtEndDate.ToShortDateString(), dtStartDate.ToShortDateString() + " to " + dtEndDate.ToShortDateString()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Rolling 12 Months from the current Month
        /// </summary>
        private List<ListItem> GetMonthsRangeDates(List<ListItem> pList)
        {
            try
            {
                DateTime dtbaseDate, dttempDate;
                DateTime dtStartDate = DateTime.Now;
                DateTime dtEndDate = DateTime.Now;
                DateTime.TryParse(string.Format("{0}/{1}/{2}", DateTime.Now.Month, 1, DateTime.Now.Year), out dtbaseDate);

                for (int i = 0; i < 13; i++)
                {
                    dttempDate = dtbaseDate.AddMonths(-i);
                    dtStartDate = new DateTime(dttempDate.Year, dttempDate.Month, 1, 00, 00, 00);
                    dtEndDate = dtStartDate.AddMonths(1).AddDays(-1);
                    pList.Add(new ListItem(dttempDate.ToString("MMMM") + ' ' + dttempDate.Year.ToString(), dtStartDate.ToShortDateString() + " to " + dtEndDate.ToShortDateString()));
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            return pList;
        }

        /// <summary>
        /// Years from members activity
        /// </summary>
        private List<ListItem> GetYearsRangeDates(List<ListItem> pList)
        {
            try
            {
                IList<PointTransaction> PointTransactions = new List<PointTransaction>();
                DateTime dtStartDate = DateTime.Now;
                DateTime dtEndDate = DateTime.Now;
                DateTime dtTxnDateMin = DateTime.Now;
                int iYearStart = 0;
                int iYearEnd = 0;
                string sRangeDate = string.Empty;

                //Get end year
                if (pList.Count() > 0)
                {
                    sRangeDate = pList[pList.Count() - 1].Value;
                }
                Utilities.GetRangeDates(sRangeDate, ref dtStartDate, ref dtEndDate);
                iYearEnd = dtEndDate.Year;

                //Range of dates for getting all members transaction less the txn from current year
                dtStartDate = new DateTime(1990, 1, 1, 00, 00, 00);
                dtEndDate = new DateTime(dtEndDate.Year, 12, 31, 23, 59, 59);

                using (ILoyaltyDataService dataService = _dataUtil.LoyaltyDataServiceInstance()) {
                    //Get trx from the member
                    IList<PointTransaction> objMemberPointTransactions = dataService.GetPointTransactions(member, dtStartDate, dtEndDate, string.Empty, string.Empty, 0, int.MaxValue, true);

                    if (objMemberPointTransactions.Count() > 0)
                    {
                        //Get Start Year
                        dtTxnDateMin = objMemberPointTransactions.Min(x => x.TransactionDate);
                        iYearStart = dtTxnDateMin.Year;

                        //Get the years when the member has activity
                        while (iYearStart <= iYearEnd)
                        {
                            dtStartDate = new DateTime(iYearEnd, 1, 1, 00, 00, 00);
                            dtEndDate = new DateTime(iYearEnd, 12, 31, 23, 59, 59);

                            var lTransCount = from pt in objMemberPointTransactions
                                              where pt.TransactionDate >= dtStartDate && pt.TransactionDate <= dtEndDate
                                              select pt.RowKey;

                            if (lTransCount.Count() > 0)
                            {
                                pList.Add(new ListItem(dtStartDate.Year.ToString(), dtStartDate.ToShortDateString() + " to " + dtEndDate.ToShortDateString()));
                            }

                            iYearEnd = iYearEnd - 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pList;
        }
        //AEO-1793 End

        //AEO-2108 Begin - CSPortal Date Drop Down
        /// <summary>
        /// Load the global value for account summary page
        /// </summary>
        private void SetStartDateNR()
        {
            try
            {

                using (IDataService dataService = _dataUtil.DataServiceInstance())
                {
                    ClientConfiguration objClientConfiguration = null;
                    DateTime startDate = DateTime.MinValue;
                    objClientConfiguration = dataService.GetClientConfiguration("NationalRolloutStartDate");
                    startDate = DateTime.Parse(objClientConfiguration.Value);
                    PortalState.PutInCache("NationalRolloutStartDate", startDate.ToShortDateString());
                    _logger.Trace(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, "PutInCache NationalRolloutStartDate: " + startDate.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }               
        }

        //AEO-2108 End
    }
}
