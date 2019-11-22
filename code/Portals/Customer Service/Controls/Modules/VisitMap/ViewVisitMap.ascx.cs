using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.LWModules.VisitMap.Components;
using Brierley.WebFrameWork.Controls;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Controls.VisitMap;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using Brierley.WebFrameWork.SocialNetwork.Controls;
using Mobile = Brierley.WebFrameWork.Controls.Mobile;

namespace Brierley.LWModules.VisitMap
{
	public partial class ViewVisitMap : ModuleControlBase
	{
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private const string _className = "ViewVisitMap";
		private const string _modulePath = "~/Controls/Modules/VisitMap/ViewVisitMap.ascx";
		private const int _maxDateRanges = 500;
		protected VisitMapConfig _config = null;
		private IVisitMapProvider _provider = null;
		private AspDynamicList _list = null;
		private AspDynamicGrid _grid = null;
		private ListControl _dateList = null;
		private Label _lblVisitListShareMessage = new Label();
		private int _numVisits = 0;

		protected int NumVisits
		{
			get { return _numVisits; }
		}

		protected string NumOrdinalVisits
		{
			get
			{
				if (_numVisits <= 0) return "0th";
				string visitsStr = _numVisits.ToString();
				if (_numVisits != 11 && visitsStr.EndsWith("1")) return visitsStr + "st";
				if (_numVisits != 12 && visitsStr.EndsWith("2")) return visitsStr + "nd";
				if (_numVisits != 13 && visitsStr.EndsWith("3")) return visitsStr + "rd";
				return visitsStr + "th";
			}
		}

		protected string GoogleUrl
		{
			get
			{
				string ret = "http://maps.googleapis.com/maps/api/js";
				string forceSsl = ConfigurationManager.AppSettings["ForceSSL"];
				if (!string.IsNullOrEmpty(forceSsl) && forceSsl.Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					ret = ret.Replace("http://", "https://");
				}
				return ret;
			}
		}

		protected double MapCenterLat
		{
			get
			{
				return _config.MapCenterLat;
			}
		}

		protected double MapCenterLong
		{
			get
			{
				return _config.MapCenterLong;
			}
		}

		protected int MapZoom
		{
			get
			{
				return _config.MapZoom;
			}
		}

		protected string MapType
		{
			get
			{
				return _config.MapType.ToString();
			}
		}

		protected string MarkerUrl
		{
			get
			{
				return _provider.GetMarkerImageUrl(_config.MarkerType, _config.UserSpecifiedMarkerURL);
			}
		}

		protected string MapDivId
		{
			get
			{
				return _config.MapDivID;
			}
		}

		protected string MapShareUrl
		{
			get
			{
				return _provider.GetVisitMapShareUrl();
			}
		}

		protected string StoreMarkers
		{
			get
			{
				string storeMarkers = string.Empty;
				if (_config.ShowStoreMarkers)
				{
					if (_provider != null)
					{
						IList<string> jsonStores = null;
						if (chkAll.Checked)
						{
							jsonStores = _provider.GetJSonStoresAll();
						}
						else if (chkVisited.Checked)
						{
							jsonStores = _provider.GetJSonStoresVisited();
						}
						else if (chkNotVisited.Checked)
						{
							jsonStores = _provider.GetJSonStoresNotVisited();
						}
						else if (chkQualifyingSpend.Checked)
						{
							jsonStores = _provider.GetJSonStoresWithQualifiedSpend();
						}
						else
						{
							jsonStores = _provider.GetJSonStoresAll();
						}
						if (jsonStores != null && jsonStores.Count > 0)
						{
							foreach (var jsonStore in jsonStores)
							{
								if (!string.IsNullOrEmpty(jsonStore))
								{
									storeMarkers += string.Format("_visitMapConfig.storeLocations.push({0});{1}", jsonStore, Environment.NewLine);
								}
							}
						}
					}
				}
				return storeMarkers;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			const string methodName = "OnInit";
			try
			{
				if (PortalState.CurrentMember == null)
				{
					return;
				}

				InitializeConfig();

				if (string.IsNullOrEmpty(_config.ProviderClassName) || string.IsNullOrEmpty(_config.ProviderAssemblyName))
				{
					_logger.Trace(_className, methodName, "No provider has been set. Using default list provider.");
                    _provider = new DefaultVisitMapListProvider() { ParentControl = _modulePath };
                }
				else
				{
					_logger.Trace(_className, methodName, string.Format("Loading user-specified provider: {0} / {1}", _config.ProviderAssemblyName, _config.ProviderClassName));
					_provider = (IVisitMapProvider)ClassLoaderUtil.CreateInstance(_config.ProviderAssemblyName, _config.ProviderClassName);
				}
				_provider.SetVisitMapConfig(_config);

				switch (_config.InitialMapFilter)
				{
					default:
					case "visited":
						chkVisited.Checked = true;
						break;
					case "notvisited":
						chkNotVisited.Checked = true;
						break;
					case "qualspend":
						chkQualifyingSpend.Checked = true;
						break;
					case "all":
						chkAll.Checked = true;
						break;
				}

				pchFilter.Visible = _config.UseMapFilter;

				PopulateDateControls(_config.DateDisplayType, _config.MinimumDateRange, _config.DatesToDisplay);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			const string methodName = "OnLoad";
			try
			{
				if (PortalState.CurrentMember == null)
				{
					return;
				}

				lblMapFilter.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.MapFilterLabelResourceKey);
				chkVisited.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.VisitedCheckboxLabelResourceKey);
				chkVisited.CheckedChanged += new EventHandler(chkVisited_CheckedChanged);
				chkNotVisited.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.UnvisitedCheckboxLabelResourceKey);
				chkNotVisited.CheckedChanged += new EventHandler(chkNotVisited_CheckedChanged);
				chkQualifyingSpend.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.QualifyingSpendCheckboxLabelResourceKey);
				chkQualifyingSpend.CheckedChanged += new EventHandler(chkQualifyingSpend_CheckedChanged);
				chkAll.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.AllCheckboxLabelResourceKey);
				chkAll.CheckedChanged += new EventHandler(chkAll_CheckedChanged);

				if (!IsAjaxPostback() && _provider.HasListProvider())
				{
					_list = new AspDynamicList()
					{
						Title = ResourceUtils.GetLocalWebResource(_modulePath, _config.VisitListHeaderLabelResourceKey),
						WrapInApplicationPanel = true,
						EnableViewState = false
					};
					_list.ShowPositive += delegate(object sndr, string message) { ShowPositive(message); };
					_list.ShowNegative += delegate(object sndr, string message) { ShowNegative(message); };
					_list.ShowWarning += delegate(object sndr, string message) { ShowWarning(message); };
					_list.Provider = _provider.GetListProvider();
					try
					{
						phVisitList.Controls.Add(_list);
					}
					catch (Exception ex)
					{
						_logger.Error(_className, methodName, "Unexpected exception adding list control: " + ex.Message, ex);
						throw;
					}

					RebindList();

					_numVisits = GetNumVisits();
				}

			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			const string methodName = "OnPreRender";
			try
			{
				if (PortalState.CurrentMember != null && _provider != null)
				{
					int numRows = GetNumVisits();
					if(_lblVisitListShareMessage.Visible && _config != null)
					{
						_lblVisitListShareMessage.Text = 
						ResourceUtils.GetLocalWebResource(_modulePath, _config.VisitListShareMessageLabelResourceKey)
						.Replace("##NUMVISITS##", NumVisits.ToString())
						.Replace("##NUMORDINALVISITS##", NumOrdinalVisits);
					}

				}
				base.OnPreRender(e);
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Unexpected exception: " + ex.Message, ex);
				throw;
			}
		}

		private int GetNumVisits()
		{
			int numVisits = 0;
			if (_provider != null)
			{
				numVisits = _provider.GetNumVisits();
			}
			return numVisits;
		}

		protected override bool ControlRequiresTelerikSkins()
		{
			return false;
		}

		protected void chkVisited_CheckedChanged(object sender, EventArgs e)
		{
			if (!chkVisited.Checked)
			{
				chkVisited.Checked = true;
			}
			else
			{
				chkNotVisited.Checked = false;
				chkQualifyingSpend.Checked = false;
				chkAll.Checked = false;
			}
		}

		protected void chkNotVisited_CheckedChanged(object sender, EventArgs e)
		{
			if (!chkNotVisited.Checked)
			{
				chkNotVisited.Checked = true;
			}
			else
			{
				chkVisited.Checked = false;
				chkQualifyingSpend.Checked = false;
				chkAll.Checked = false;
			}
		}

		protected void chkQualifyingSpend_CheckedChanged(object sender, EventArgs e)
		{
			if (!chkQualifyingSpend.Checked)
			{
				chkQualifyingSpend.Checked = true;
			}
			else
			{
				chkVisited.Checked = false;
				chkNotVisited.Checked = false;
				chkAll.Checked = false;
			}
		}

		protected void chkAll_CheckedChanged(object sender, EventArgs e)
		{
			if (!chkAll.Checked)
			{
				chkAll.Checked = true;
			}
			else
			{
				chkVisited.Checked = false;
				chkNotVisited.Checked = false;
				chkQualifyingSpend.Checked = false;
			}
		}


		private void InitializeConfig()
		{
			if (_config == null)
			{
				_config = ConfigurationUtil.GetConfiguration<VisitMapConfig>(ConfigurationKey);
				if (_config == null)
				{
					_config = new VisitMapConfig();
					ConfigurationUtil.SaveConfiguration(ConfigurationKey, _config);
				}
			}
		}

		private bool IsAjaxPostback()
		{
			//return Page.Request.Form["keyVal"] != null && Page.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
			return false;
		}

		private void PopulateDateControls(DateDisplayTypes displayType, DateTime? minimumDateRange, int? datesToDisplay)
		{
			if (_config.DateDisplayType == DateDisplayTypes.None)
			{
				pnlDateRange.Visible = false;
			}
			else
			{
				pnlDateRange.Visible = true;
				lblDateRange.Text = ResourceUtils.GetLocalWebResource(_modulePath, _config.DateRangeLabelResourceKey);

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

				if (dates != null)
				{
					if (dates.Count >= 7)
					{
						_dateList = new DropDownList();
					}
					else
					{
						_dateList = new Mobile.RadioButtonList();
					}
					_dateList.AutoPostBack = true;
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

						_dateList.Items.Add(li);
					}
					pchDateList.Controls.Add(_dateList);

					var startdate = dates[0].Key;
					var endDate = dates[0].Value;
					if (_grid != null)
					{
						_grid.SetSearchParm("FromDate", startdate);
						_grid.SetSearchParm("ToDate", endDate);
					}
					if (_list != null)
					{
						_list.SetSearchParm("FromDate", startdate);
						_list.SetSearchParm("ToDate", endDate);
					}
				}
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

		private void RebindGrid()
		{
			switch (_config.DateDisplayType)
			{
				case DateDisplayTypes.None:
					break;

				case DateDisplayTypes.DropDownByMonth:
				case DateDisplayTypes.DropDownByQuarter:
				case DateDisplayTypes.DropDownByWeek:
				case DateDisplayTypes.DropDownByYear:
					DateTime start = DateTime.Today;
					DateTime end = DateTime.Today;
					if (!string.IsNullOrEmpty(_dateList.SelectedValue))
					{
						var split = _dateList.SelectedValue.Split('|');
						if (split.Length == 2 && DateTime.TryParse(split[0], out start) && DateTime.TryParse(split[1], out end))
						{
							_grid.SetSearchParm("FromDate", start);
							_grid.SetSearchParm("ToDate", end);
						}
					}
					break;
			}

			_grid.Rebind();
			int numRows = GetNumVisits();
		}

		private void RebindList()
		{
			switch (_config.DateDisplayType)
			{
				case DateDisplayTypes.None:
					break;

				case DateDisplayTypes.DropDownByMonth:
				case DateDisplayTypes.DropDownByQuarter:
				case DateDisplayTypes.DropDownByWeek:
				case DateDisplayTypes.DropDownByYear:
					DateTime start = DateTime.Today;
					DateTime end = DateTime.Today;
					if (!string.IsNullOrEmpty(_dateList.SelectedValue))
					{
						var split = _dateList.SelectedValue.Split('|');
						if (split.Length == 2 && DateTime.TryParse(split[0], out start) && DateTime.TryParse(split[1], out end))
						{
							_list.SetSearchParm("FromDate", start);
							_list.SetSearchParm("ToDate", end);
						}
					}
					break;
			}

			_list.Rebind();
			int numRows = GetNumVisits();
		}
	}
}
