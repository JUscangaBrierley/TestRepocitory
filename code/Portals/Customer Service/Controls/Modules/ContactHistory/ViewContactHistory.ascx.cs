using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.LWModules.ContactHistory.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Ipc;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.ContactHistory
{
	public partial class ViewContactHistory : ModuleControlBase, IIpcEventHandler
	{
		#region fields
		private const string _className = "ViewContactHistory";
		private const string _modulePath = "~/Controls/Modules/ContactHistory/ViewContactHistory.ascx";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private ContactHistoryConfiguration _config = null;
		private IDynamicGridProvider _grdContactHistoryProvider = null;
		private AspDynamicGrid _grdContactHistory = null;
		private bool _memberSelected = false;
		private const int _maxDateRanges = 500;
		#endregion

		#region page life cycle
		protected override void OnInit(EventArgs e)
		{
			const string methodName = "OnInit";
			try
			{
				base.OnInit(e);

				_memberSelected = PortalState.CurrentMember != null;
				if (!_memberSelected)
				{
					return;
				}

				_config = ConfigurationUtil.GetConfiguration<ContactHistoryConfiguration>(ConfigurationKey);
				if (_config == null)
				{
					_config = new ContactHistoryConfiguration();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}

				h2Title.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.TitleResourceKey);

				btnSearch.ValidationGroup = reqFromDate.ValidationGroup = reqToDate.ValidationGroup = ValidationGroup;

				// contact history grid
				if (string.IsNullOrEmpty(_config.ProviderClassName) || string.IsNullOrEmpty(_config.ProviderAssemblyName))
				{
					_logger.Trace(_className, methodName, "No provider has been set. Using default");
					_grdContactHistoryProvider = new DefaultContactHistoryGridProvider() { ParentControl = _modulePath };
					_grdContactHistoryProvider.SetSearchParm("Config", _config);
				}
				else
				{
					_grdContactHistoryProvider = (IDynamicGridProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
					if (_grdContactHistoryProvider == null)
					{
						throw new Exception(
							string.Format("Unable to create the grid provider class {0} from assembly {1}", 
								_config.ProviderClassName, 
								_config.ProviderAssemblyName)
						);
					}
					_grdContactHistoryProvider.ParentControl = _modulePath;
					_grdContactHistoryProvider.SetSearchParm("Config", _config);
				}
				//_grdContactHistoryProvider.FilteringEnabled = _config.EnableGridFiltering;
				_grdContactHistory = new AspDynamicGrid() { AutoRebind = false, CreateTopPanel = false };
				_grdContactHistory.Provider = _grdContactHistoryProvider;
				((AspGridProviderBase)_grdContactHistoryProvider).ValidationGroup = ValidationGroup;
				((AspGridProviderBase)_grdContactHistoryProvider).ValidationError += delegate(string message, Control offender) { AddInvalidField(message, offender); };
				phContactHistoryGrid.Controls.Add(_grdContactHistory);

                customToDateValidator.ValidationGroup = btnSearch.ValidationGroup = ValidationGroup;

				lblNoResults.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.EmptyResultMessageResourceKey);

				if (!IsPostBack)
				{
					PopulateDateControls(_config.DateDisplayType, _config.MinimumDateRange, _config.DatesToDisplay);
				}
				//_grdContactHistory.Rebind();
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

			_grdContactHistory.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
			_grdContactHistory.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
			_grdContactHistory.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };

			IpcManager.RegisterEventHandler("MemberSelected", this, false);
			IpcManager.RegisterEventHandler("MemberUpdated", this, false);

			if (!_memberSelected || _grdContactHistoryProvider == null)
			{
				return;
			}
			RebindGrid();
		}

		protected override void OnPreRender(EventArgs e)
		{
			if (_memberSelected && _grdContactHistoryProvider != null)
			{
				lblNoResults.Visible = _grdContactHistoryProvider.GetNumberOfRows() < 1;
			}
			base.OnPreRender(e);
		}
		#endregion

		#region event handlers
		protected void btnSearch_Click(object sender, EventArgs e)
		{
			//hdnExpandedRows.Value = string.Empty;
		}

		void ddlDateRange_SelectedIndexChanged(object sender, EventArgs e)
		{
			//hdnExpandedRows.Value = string.Empty;
		}
		#endregion

		#region private methods
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
				_grdContactHistory.SetSearchParm("FromDate", dpFromDate.SelectedDate);
				_grdContactHistory.SetSearchParm("ToDate", dpToDate.SelectedDate.Value);
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
				_grdContactHistory.SetSearchParm("FromDate", startdate);
				_grdContactHistory.SetSearchParm("ToDate", endDate);
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

		private void RebindGrid()
		{
			switch (_config.DateDisplayType)
			{
				case DateDisplayTypes.None:
					break;

				case DateDisplayTypes.TextBoxRange:
					_grdContactHistory.SetSearchParm("FromDate", dpFromDate.SelectedDate);
					_grdContactHistory.SetSearchParm("ToDate", dpToDate.SelectedDate);
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
							_grdContactHistory.SetSearchParm("FromDate", start);
							_grdContactHistory.SetSearchParm("ToDate", end);
						}
					}
					break;
			}

			_grdContactHistory.Rebind();
			lblNoResults.Visible = _grdContactHistoryProvider != null && _grdContactHistoryProvider.GetNumberOfRows() < 1;
		}
		#endregion

		#region ModuleControlBase overrides
		protected override bool ControlRequiresTelerikSkins()
		{
			return false;
		}
		#endregion

		#region IIpcEventHandler implementation
		public ModuleConfigurationKey GetConfigurationKey()
		{
			return ConfigurationKey;
		}

		public void HandleEvent(IpcEventInfo info)
		{
			const string methodName = "HandleEvent";
			_logger.Trace(_className, methodName, "Begin");
			if (
				info.PublishingModule != base.ConfigurationKey &&
				(info.EventName == "MemberSelected" || info.EventName == "MemberUpdated") &&
				_config != null
				)
			{
				RebindGrid();
			}
			_logger.Trace(_className, methodName, "End");
		}
		#endregion
	}
}
