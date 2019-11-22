using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;

using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.LWModules.CSTriggerUserEvent.Components;

namespace Brierley.LWModules.CSTriggerUserEvent
{
    public partial class ViewCSTriggerUserEvent : ModuleControlBase
    {
        #region Fields
        private const string _className = "ViewCSTriggerUserEvent";
        private const string _modulePath = "~/Controls/Modules/CSTriggerUserEvent/ViewCSTriggerUserEvent.ascx";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
        private CSTriggerUserEventConfiguration _config = null;
        private IDynamicGridProvider _eventsGridProvider = null;
        private AspDynamicGrid _gridEvents = null;

        private Dictionary<GridView, IDynamicGridProvider> _nestedGrids = new Dictionary<GridView, IDynamicGridProvider>();
        
        private bool _memberSelected = false;
        private const int _maxDateRanges = 500;
        #endregion

        #region Helpers
        private bool IsAjaxPostback()
        {
            return Page.Request.Form["keyVal"] != null && Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        private DynamicGridColumnSpec GetKeyColumn()
        {
            if (_gridEvents.Columns != null)
            {
                foreach (DynamicGridColumnSpec column in _gridEvents.Columns)
                {
                    if (column.IsKey)
                    {
                        return column;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Page Lifecycle

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

                _config = ConfigurationUtil.GetConfiguration<CSTriggerUserEventConfiguration>(ConfigurationKey);

                if (_config == null)
                {
                    _logger.Error(_className, methodName, string.Format("Missing configuration for module {0}.", ConfigurationKey));
                    return;
                }

                h2Title.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.TitleResourceKey);                

                reqFromDate.ValidationGroup = reqToDate.ValidationGroup = btnSearch.ValidationGroup = ValidationGroup;
                customToDateValidator.ValidationGroup = btnSearch.ValidationGroup = ValidationGroup;

                #region Events Grid
                //Sales Txn Grid Details
                lblEventsLabel.Text = "Triggered User Events";                
                if (string.IsNullOrEmpty(_config.EventsProviderClassName) || string.IsNullOrEmpty(_config.EventsProviderAssemblyName))
                {
                    _logger.Trace(_className, methodName, "No provider has been set. Using default");
                    _eventsGridProvider = new TriggerUserEventGridProvider() { ParentControl = _modulePath };
                    _eventsGridProvider.SetSearchParm("Config", _config);
                }
                else
                {
                    _eventsGridProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.EventsProviderAssemblyName, _config.EventsProviderClassName);
                    if (_eventsGridProvider == null)
                    {
                        _logger.Error(_className, methodName,
                            string.Format("Unable to create the grid provider class {0} from assembly {1}",
                            _config.EventsProviderClassName, _config.EventsProviderAssemblyName));
                    }
                    _eventsGridProvider.ParentControl = _modulePath;
                    _eventsGridProvider.SetSearchParm("Config", _config);
                }
                _eventsGridProvider.FilteringEnabled = _config.EnableGridFiltering;

                lblNoResults.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey);
                #endregion
               
                if (IsAjaxPostback())
                {
                    try
                    {
                        _logger.Trace(_className, methodName, "AJAX post begin");
                        StringBuilder output = new StringBuilder();
                        bool hasSubtotal = false;
                        double subtotal = 0;
                        if (_eventsGridProvider is INestedGridProvider && Request.Form["keyVal"] != null)
                        {
                            output.Append("<table>");

                            var childProvider = ((INestedGridProvider)_eventsGridProvider).GetChildGridProvider(Request.Form["keyVal"]);
                            var specs = childProvider.GetColumnSpecs();
                            childProvider.LoadGridData();

                            hasSubtotal = (from x in specs where x != null && !string.IsNullOrEmpty(x.SubtotalFormat) select x).Count() > 0;

                            if (childProvider.GetNumberOfRows() == 0)
                            {
                                output.AppendFormat("<tr><td>{0}</td></tr>", ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey));
                            }
                            else
                            {
                                output.AppendLine("<tr>");
                                foreach (var spec in specs)
                                {
                                    if (spec.IsVisible)
                                    {
                                        output.Append("<th>");
                                        output.Append(spec.DisplayText);
                                        output.Append("</th>");
                                    }
                                }
                                output.AppendLine("</tr>");

                                for (int i = 0; i < childProvider.GetNumberOfRows(); i++)
                                {
                                    output.AppendLine("<tr>");
                                    foreach (var spec in specs)
                                    {
                                        if (spec.IsVisible)
                                        {
                                            object data = childProvider.GetColumnData(i, spec).ToString();
                                            output.Append("<td>");
                                            output.Append(HttpContext.Current.Server.HtmlEncode(data.ToString()) ?? string.Empty);
                                            output.Append("</td>");

                                            if (!string.IsNullOrEmpty(spec.SubtotalFormat))
                                            {
                                                subtotal += Convert.ToDouble(data);
                                            }

                                        }
                                    }
                                    output.AppendLine("</tr>");
                                }
                                if (hasSubtotal)
                                {
                                    string format = (from x in specs where !string.IsNullOrEmpty(x.SubtotalFormat) select x.SubtotalFormat).FirstOrDefault();
                                    output.Append("<tr><td class=\"DynamicGridTotal\" colspan=\"" + specs.Length + "\">");
                                    output.AppendFormat(format, subtotal);
                                    output.Append("</td></tr>");
                                }
                            }
                            output.Append("</table>");
                        }

                        Response.ContentType = "application/json; charset=utf-8";
                        Response.Clear();
                        Response.Write(output.ToString());

                        _logger.Trace(_className, methodName, "AJAX post end");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(_className, "AJAX Post", ex.Message, ex);
                    }
                    finally
                    {
                        Response.Flush();
                        Response.End();
                    }
                }

                if (!IsAjaxPostback())
                {
                    _logger.Trace(_className, methodName, "_config = CSTriggerUserEventConfiguration.GetConfiguration(ModuleId); - " + ConfigurationKey);

                    #region Create Sales Txn Grid
                    _gridEvents = new AspDynamicGrid() { AutoRebind = false, EnableViewState = false, CreateTopPanel = false };
                    _gridEvents.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
                    _gridEvents.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
                    _gridEvents.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
                    _gridEvents.Provider = _eventsGridProvider;
                    if (_eventsGridProvider != null && _eventsGridProvider is INestedGridProvider)
                    {
                        TemplateField actionField = new TemplateField();
                        actionField.ItemTemplate = new ToggleNestedTemplate(null);
                        _gridEvents.Grid.Columns.Insert(0, actionField);
                    }
                    _gridEvents.Grid.RowDataBound += new GridViewRowEventHandler(Grid_RowDataBound);
                    _gridEvents.Grid.PageIndexChanging += delegate { hdnExpandedRows.Value = string.Empty; };
                    phCSTriggerUserEvent.Controls.Add(_gridEvents);
                    #endregion
                    
                    if (!IsPostBack)
                    {
                        PopulateDateControls(_config.DateDisplayType, _config.MinimumDateRange, _config.DatesToDisplay);
                        lnkPrintGrid.Visible = _config.EnablePrinting;
                    }

                    _gridEvents.Rebind();                    
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, ex.Message, ex);
                throw;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ddlDateRange.SelectedIndexChanged += new EventHandler(ddlDateRange_SelectedIndexChanged);
            btnSearch.Click += new EventHandler(btnSearch_Click);
            btnSearch.Text = ResourceUtils.GetLocalWebResource(_modulePath, "btnSearch.Text");

            //IpcManager.RegisterEventHandler("MemberSelected", this, false);
            //IpcManager.RegisterEventHandler("MemberUpdated", this, false);

            //lnkPrintGrid.Click += new EventHandler(lnkPrintGrid_Click);

            //if (!_memberSelected || _salesTxnGridProvider == null)
            //{
            //    return;
            //}
            if (!IsAjaxPostback())
            {
                _gridEvents.Rebind();
            }
        }
        #endregion

        #region Event Handlers

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            hdnExpandedRows.Value = string.Empty;
        }

        void ddlDateRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            hdnExpandedRows.Value = string.Empty;
        }

        protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                IDynamicGridProvider provider = null;
                GridView gridView = null;

                if (sender == _gridEvents.Grid)
                {
                    provider = _eventsGridProvider;
                    gridView = _gridEvents.Grid;
                    if (!_nestedGrids.ContainsKey(_gridEvents.Grid))
                    {
                        _nestedGrids.Add(_gridEvents.Grid, _eventsGridProvider);
                    }
                }
                else
                {
                    foreach (var gv in _nestedGrids.Keys)
                    {
                        if (gv == sender)
                        {
                            provider = _nestedGrids[gv];
                            gridView = gv;
                            break;
                        }
                    }
                }
                if (provider is INestedGridProvider && e.Row.RowType == DataControlRowType.DataRow)
                {
                    string keyCol = GetKeyColumn().Name;
                    int rowIndex = e.Row.DataItemIndex;

                    object keyVal = ((DataRowView)e.Row.DataItem).DataView.Table.DefaultView[rowIndex][keyCol];

                    var lnkToggleNested = (System.Web.UI.HtmlControls.HtmlAnchor)e.Row.FindControl("lnkToggleNested");
                    if (((INestedGridProvider)provider).HasChildren(keyVal))
                    {
                        lnkToggleNested.Attributes.Add("onclick", "return ToggleNested(this, '" + keyVal + "');");
                        lnkToggleNested.HRef = "#";
                    }
                    else
                    {
                        lnkToggleNested.Visible = false;
                    }

                    IDynamicGridProvider p = ((INestedGridProvider)provider).GetChildGridProvider(keyVal);
                    if (p != null)
                    {
                        GridViewRow row = new GridViewRow(-1, -1, DataControlRowType.DataRow, DataControlRowState.Normal);
                        row.Attributes.Add("style", "display:none;");
                        var td = new TableCell() { ColumnSpan = gridView.Columns.Count };
                        row.Cells.Add(td);

                        //If preferred method is to not use ajax, grids can be nested as the rows are bound.
                        //There is a performance hit when creating the entire grid at bind time, but this
                        //may be necessary (ajax implementation only supports one level of nesting)
                        if (((INestedGridProvider)provider).NestingType == NestingTypes.DataBound)
                        {
                            AspDynamicGrid grdChild = new AspDynamicGrid() { CreateTopPanel = false, EnableViewState = false };
                            grdChild.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
                            grdChild.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
                            grdChild.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
                            grdChild.Provider = p;
                            grdChild.Grid.RowDataBound += new GridViewRowEventHandler(Grid_RowDataBound);
                            if (p != null && p is INestedGridProvider)
                            {
                                TemplateField actionField = new TemplateField();
                                actionField.ItemTemplate = new ToggleNestedTemplate(null);
                                grdChild.Grid.Columns.Insert(0, actionField);
                            }
                            _nestedGrids.Add(grdChild.Grid, p);
                            td.Controls.Add(grdChild);
                        }
                        Table tbl = (e.Row.Parent as Table);
                        tbl.Rows.Add(row);
                    }
                    else
                    {
                        lnkToggleNested.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, "Grid_RowDataBound", ex.Message, ex);
                throw;
            }
        }
        #endregion

        #region Date Related
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
                var startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                dpFromDate.SelectedDate = startdate;
                dpToDate.SelectedDate = endDate;
                _gridEvents.SetSearchParm("FromDate", dpFromDate.SelectedDate);
                _gridEvents.SetSearchParm("ToDate", dpToDate.SelectedDate.Value);                
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
                _gridEvents.SetSearchParm("FromDate", startdate);
                _gridEvents.SetSearchParm("ToDate", endDate);                
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
        #endregion

    }
}