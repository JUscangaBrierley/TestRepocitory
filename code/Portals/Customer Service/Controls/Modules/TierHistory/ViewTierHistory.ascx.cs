using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.LWModules.TierHistory.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Brierley.LWModules.TierHistory
{
    public partial class ViewTierHistory : ModuleControlBase
    {
        private const string _className = "ViewTierHistory";
        private const string _modulePath = "~/Controls/Modules/TierHistory/ViewTierHistory.ascx";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

        private bool _memberSelected = false;
        private TierHistoryConfiguration _config = null;
        private IDynamicGridProvider _tierHistoryGridProvider = null;
        private AspDynamicGrid _gridTierHistory = null;

        private const int _maxDateRanges = 500;

        protected override bool ControlRequiresTelerikSkins()
        {
            return false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            const string methodName = "OnInit";
            try
            {
                _memberSelected = PortalState.CurrentMember != null;
                if (!_memberSelected)
                {
                    return;
                }

                _config = ConfigurationUtil.GetConfiguration<TierHistoryConfiguration>(ConfigurationKey);

                if (_config == null)
                {
                    _logger.Error(_className, methodName, string.Format("Missing configuration for module {0}.", ConfigurationKey));
                    return;
                }

                h2Title.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.TitleResourceKey);

                reqFromDate.ValidationGroup = reqToDate.ValidationGroup = btnSearch.ValidationGroup = ValidationGroup;
                customToDateValidator.ValidationGroup = btnSearch.ValidationGroup = ValidationGroup;

                if (string.IsNullOrEmpty(_config.ProviderClassName) || string.IsNullOrEmpty(_config.ProviderAssemblyName))
                {
                    _logger.Trace(_className, methodName, "No provider has been set. Using default");
                    _tierHistoryGridProvider = new DefaultTierHistoryGridProvider() { ParentControl = _modulePath };
                    _tierHistoryGridProvider.SetSearchParm("Config", _config);
                }
                else
                {
                    _tierHistoryGridProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
                    if (_tierHistoryGridProvider == null)
                    {
                        _logger.Error(_className, methodName,
                            string.Format("Unable to create the grid provider class {0} from assembly {1}",
                            _config.ProviderClassName, _config.ProviderAssemblyName));
                    }
                    _tierHistoryGridProvider.ParentControl = _modulePath;
                    _tierHistoryGridProvider.SetSearchParm("Config", _config);
                }
                lblNoResults.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey);

                _gridTierHistory = new AspDynamicGrid() { AutoRebind = false, EnableViewState = false, CreateTopPanel = false };
                _gridTierHistory.ShowPositive += delegate (object sndr, string message) { ShowPositive(message); };
                _gridTierHistory.ShowNegative += delegate (object sndr, string message) { ShowNegative(message); };
                _gridTierHistory.ShowWarning += delegate (object sndr, string message) { ShowWarning(message); };
                _gridTierHistory.Provider = _tierHistoryGridProvider;
                phTierHistory.Controls.Add(_gridTierHistory);

                if (!IsPostBack)
                {
                    PopulateDateControls(_config.DateDisplayType, _config.MinimumDateRange, _config.DatesToDisplay);
                }

                _gridTierHistory.Rebind();
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, ex.Message, ex);
                throw;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnSearch.Text = ResourceUtils.GetLocalWebResource(_modulePath, "btnSearch.Text");

            if (!_memberSelected || _tierHistoryGridProvider == null)
            {
                return;
            }

            RebindGrid();
        }

        private void RebindGrid()
        {
            switch (_config.DateDisplayType)
            {
                case DateDisplayTypes.None:
                    break;
                case DateDisplayTypes.TextBoxRange:
                    _gridTierHistory.SetSearchParm("FromDate", dpFromDate.SelectedDate);
                    _gridTierHistory.SetSearchParm("ToDate", dpToDate.SelectedDate);
                    break;
                case DateDisplayTypes.DropDownByMonth:
                case DateDisplayTypes.DropDownByQuarter:
                case DateDisplayTypes.DropDownByWeek:
                case DateDisplayTypes.DropDownByYear:
                    DateTime start = DateTime.Today;
                    DateTime end = DateTime.Today;
                    if (!string.IsNullOrEmpty(ddlDateRange.SelectedValue))
                    {
                        var split = ddlDateRange.SelectedValue.Split('|');
                        if (split.Length == 2 && DateTime.TryParse(split[0], out start) && DateTime.TryParse(split[1], out end))
                        {
                            _gridTierHistory.SetSearchParm("FromDate", start);
                            _gridTierHistory.SetSearchParm("ToDate", end);
                        }
                    }
                    break;
            }

            _gridTierHistory.Rebind();
            lblNoResults.Visible = _tierHistoryGridProvider != null && _tierHistoryGridProvider.GetNumberOfRows() < 1;
        }

        private void PopulateDateControls(DateDisplayTypes displayType, DateTime? minimumDateRange, int? datesToDisplay)
        {
            if (_config.DateDisplayType == DateDisplayTypes.None)
            {
                pnlDateTextBox.Visible = false;
                pnlDateRange.Visible = false;
            }
            else if (_config.DateDisplayType == DateDisplayTypes.TextBoxRange)
            {
                pnlDateTextBox.Visible = true;
                pnlDateRange.Visible = false;
                var startdate = _config.MinimumDateRange.GetValueOrDefault(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                var endDate = _config.MaximumDateRange.GetValueOrDefault(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)));
                dpFromDate.SelectedDate = startdate;
                dpToDate.SelectedDate = endDate;
                _gridTierHistory.SetSearchParm("FromDate", dpFromDate.SelectedDate);
                _gridTierHistory.SetSearchParm("ToDate", dpToDate.SelectedDate.Value);
            }
            else
            {
                pnlDateTextBox.Visible = false;
                pnlDateRange.Visible = true;

                List<KeyValuePair<DateTime, DateTime>> dates = null;
                switch (_config.DateDisplayType)
                {
                    case DateDisplayTypes.DropDownByWeek:
                        dates = GetWeeks(minimumDateRange, datesToDisplay);
                        break;
                    case DateDisplayTypes.DropDownByMonth:
                        dates = GetMonths(minimumDateRange, datesToDisplay);
                        break;
                    case DateDisplayTypes.DropDownByQuarter:
                        dates = GetQuarters(minimumDateRange, datesToDisplay);
                        break;
                    case DateDisplayTypes.DropDownByYear:
                        dates = GetYears(minimumDateRange, datesToDisplay);
                        break;
                }

                foreach (var range in dates)
                {
                    ListItem li = new ListItem();
                    li.Value = range.Key.ToShortDateString() + "|" + range.Value.ToShortDateString();
                    switch (displayType)
                    {
                        case DateDisplayTypes.DropDownByWeek:
                        case DateDisplayTypes.DropDownByQuarter:
                            li.Text = range.Key.ToShortDateString() + " To " + range.Value.ToShortDateString();
                            break;
                        case DateDisplayTypes.DropDownByYear:
                            li.Text = range.Key.Year.ToString();
                            break;
                        case DateDisplayTypes.DropDownByMonth:
                            li.Text = range.Key.ToString("MMMM yyyy");
                            break;
                    }

                    ddlDateRange.Items.Add(li);
                }

                var startdate = dates[0].Key;
                var endDate = dates[0].Value;
                _gridTierHistory.SetSearchParm("FromDate", startdate);
                _gridTierHistory.SetSearchParm("ToDate", endDate);
            }
        }

        private List<KeyValuePair<DateTime, DateTime>> GetWeeks(DateTime? minDate, int? datesToDisplay)
        {
            var ret = new List<KeyValuePair<DateTime, DateTime>>();

            DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime start = DateTime.Today;
            while (start.DayOfWeek != firstDayOfWeek)
            {
                start = start.AddDays(-1);
            }

            while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
            {
                ret.Add(new KeyValuePair<DateTime, DateTime>(start, start.AddDays(6)));
                start = start.AddDays(-7);
            }
            return ret;
        }

        private List<KeyValuePair<DateTime, DateTime>> GetQuarters(DateTime? minDate, int? datesToDisplay)
        {
            var ret = new List<KeyValuePair<DateTime, DateTime>>();
            int quarterStartMonth = 0;
            switch (DateTime.Today.Month)
            {
                case 1:
                case 2:
                case 3:
                    quarterStartMonth = 1;
                    break;
                case 4:
                case 5:
                case 6:
                    quarterStartMonth = 4;
                    break;
                case 7:
                case 8:
                case 9:
                    quarterStartMonth = 7;
                    break;
                case 10:
                case 11:
                case 12:
                    quarterStartMonth = 10;
                    break;
            }
            DateTime start = new DateTime(DateTime.Today.Year, quarterStartMonth, 1);

            while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
            {
                DateTime end = start.AddMonths(3).AddDays(-1);
                ret.Add(new KeyValuePair<DateTime, DateTime>(start, end));
                start = start.AddMonths(-3);
            }
            return ret;

        }

        private List<KeyValuePair<DateTime, DateTime>> GetMonths(DateTime? minDate, int? datesToDisplay)
        {
            var ret = new List<KeyValuePair<DateTime, DateTime>>();

            DateTime start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
            {
                DateTime end = start.AddMonths(1).AddDays(-1);
                ret.Add(new KeyValuePair<DateTime, DateTime>(start, end));
                start = start.AddMonths(-1);
            }
            return ret;

        }

        private List<KeyValuePair<DateTime, DateTime>> GetYears(DateTime? minDate, int? datesToDisplay)
        {
            var ret = new List<KeyValuePair<DateTime, DateTime>>();

            DateTime start = new DateTime(DateTime.Today.Year, 1, 1);

            while (start >= minDate.GetValueOrDefault(DateTimeUtil.MinValue) && ret.Count < datesToDisplay.GetValueOrDefault(_maxDateRanges))
            {
                DateTime end = new DateTime(start.Year, 12, 31);
                ret.Add(new KeyValuePair<DateTime, DateTime>(start, end));
                start = start.AddYears(-1);
            }
            return ret;
        }
    }
}