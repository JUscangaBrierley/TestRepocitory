using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Newtonsoft.Json;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Controls.VisitMap;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using System.Text;

namespace Brierley.LWModules.VisitMap.Components
{
	public class DefaultVisitMapListProvider : AspListProviderBase, IVisitMapProvider
	{
		#region fields
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private enum ColumnName
		{
			a_txnheaderid = 0, TxnDate, ActivityDescription, IsQualifyingSpend
		}
		private int _numColumns = Enum.GetNames(typeof(ColumnName)).Length;
		private DateTime _startDate = DateTimeUtil.MinValue;
		private DateTime _endDate = DateTimeUtil.MaxValue;
		private List<VisitMapTransactionHeader> _headers = null;
		private IList<StoreDef> _stores = null;
		private VisitMapConfig _config = null;
		#endregion

		#region AspListProviderBase overrides

		#region list properties
		public override string Id
		{
			get { return "lstStoreVisits"; }
		}

		public override IEnumerable<DynamicListItem> GetListItemSpecs()
		{
			var colList = new List<DynamicListItem>();

			DynamicListColumnSpec c = new DynamicListColumnSpec();
			c.Name = ColumnName.a_txnheaderid.ToString();
			c.DisplayText = null;
			c.DataType = typeof(long);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;
			colList.Add(c);

			c = new DynamicListColumnSpec();
			c.Name = ColumnName.TxnDate.ToString();
			c.BeginHtml = @"<div class=""row""><div class=""col-lg-3 col-md-3 col-sm-3 col-xs-4 list-left"">";
			c.EndHtml = "</div>";
			c.DisplayText = null;
			c.DataType = typeof(string);
			c.CssClass = "visit-date";
			c.IsEditable = false;
			c.IsVisible = true;
			colList.Add(c);

			c = new DynamicListColumnSpec();
			c.Name = ColumnName.ActivityDescription.ToString();
			c.DisplayText = null;
			c.DataType = typeof(string);
			c.BeginHtml = @"<div class=""col-lg-5 col-md-5 col-sm-5 col-xs-8 list-middle"">";
			c.EndHtml = @"</div>";
			c.IsEditable = false;
			c.IsVisible = true;
			colList.Add(c);

			c = new DynamicListColumnSpec();
			c.Name = ColumnName.IsQualifyingSpend.ToString();
			c.DisplayText = null;
			c.BeginHtml = @"<div class=""col-lg-4 col-md-4 col-sm-4 col-xs-12 list-right"">";
			c.EndHtml = @"</div></div>";
			c.DataType = typeof(string);
			c.CssClass = "visit-spend";
			c.IsEditable = false;
			c.IsVisible = true;
			colList.Add(c);

			return colList;
		}

		public override bool IsButtonVisible(ListCommand commandName)
		{
			if (commandName == ListCommand.View ||
				commandName == ListCommand.Print)
			{
				return true;
			}
			else
			{
				return false;
			};
		}

		public override string GetEmptyListMessage()
		{
            return ResourceUtils.GetLocalWebResource("~/Controls/Modules/VisitMap/ViewVisitMap.ascx", "EmptyResultMessage.Text");
        }

        public override string GetAppPanelTotalText(int totalRecords)
        {
            if (totalRecords == 0)
            {
                return ResourceUtils.GetLocalWebResource(ParentControl, "NoVisits.Text", "No Visits."); 
            }
            else
            {
                return string.Format("Total: {0} ", totalRecords);
            }
        }
		#endregion

		#region list data source
		public override void SetSearchParm(string parmName, object parmValue)
		{
			if (string.IsNullOrWhiteSpace(parmName))
			{
				_startDate = DateTimeUtil.MinValue;
				_endDate = DateTimeUtil.MaxValue;
			}
			else
			{
				switch (parmName)
				{
					case "FromDate":
						_startDate = (DateTime)parmValue;
						break;

					case "ToDate":
						_endDate = (DateTime)parmValue;
						_endDate = _endDate.AddDays(1).AddTicks(-1);
						break;

					case "Config":
						_config = (VisitMapConfig)parmValue;
						break;
				}
			}
		}

		public override void LoadListData()
		{
			_headers = new List<VisitMapTransactionHeader>();
			Member member = PortalState.CurrentMember;

            int maxRows = StringUtils.FriendlyInt32(LWConfigurationUtil.GetConfigurationValue("VisitMapMaxStoreRows"), 10000);
            _stores = ContentService.GetAllStoreDefs(new LWQueryBatchInfo() { StartIndex = 0, BatchSize = maxRows });

			IList<IClientDataObject> txnHeaders = AccountActivityUtil.GetAccountActivitySummary(member, _startDate, _endDate, true, false, new LWQueryBatchInfo() { BatchSize = 1000, StartIndex = 0 });
			if (txnHeaders != null && txnHeaders.Count > 0)
			{
				foreach (IClientDataObject cdo in txnHeaders)
				{
					var th = new VisitMapTransactionHeader();
					th.RowKey = cdo.RowKey;
					object headerId = cdo.GetAttributeValue("TxnHeaderId");
					if (headerId == null) continue;

					th.HeaderId = headerId as string;
					if (!cdo.HasTransientProperty("Store")) continue;

					var store = (StoreDef)cdo.GetTransientProperty("Store");
					if (store == null) continue;

					th.Store = store;
					th.ActivityName = string.Format("{0} ({1}, {2})", store.StoreName, store.City, store.StateOrProvince);

					object transactionDate = cdo.GetAttributeValue("TxnDate");
					if (transactionDate != null)
					{
						th.TransactionDate = Convert.ToDateTime(transactionDate);
					}

					object spend = cdo.GetAttributeValue("TxnAmount");
					if (spend != null)
					{
						th.Spend = Convert.ToDouble(spend);
					}

					th.IsQualifyingSpend = false;
					try
					{
						object qualifyingSpend = cdo.GetAttributeValue("TxnQualPurchaseAmt");
						if (qualifyingSpend != null)
						{
							double amount = Convert.ToDouble(qualifyingSpend);
							if (amount > 0) th.IsQualifyingSpend = true;
						}
					}
					catch { }

					_headers.Add(th);
				}
			}
		}

		public override int GetNumberOfRows()
		{
			return _headers != null ? _headers.Count : 0;
		}

		public override object GetColumnData(int rowIndex, DynamicListColumnSpec column)
		{
			object val = null;
			var header = _headers[rowIndex];

			switch (column.Name)
			{
				case "a_txnheaderid":
					val = header.HeaderId;
					break;

				case "TxnDate":
					val = header.TransactionDate.ToShortDateString();
					break;

				case "ActivityDescription":
					StringBuilder sb = new StringBuilder();
					sb.Append("<div class=\"store\">");
					sb.Append("<h3 class=\"name\">").Append(StringUtils.FriendlyString(header.Store.StoreName)).Append("</h3>");
					sb.Append("<p>");
					sb.Append("<span class=\"address1\">").Append(StringUtils.FriendlyString(header.Store.AddressLineOne)).Append("</span>");
					sb.Append("<span class=\"address2\">").Append(StringUtils.FriendlyString(header.Store.AddressLineTwo)).Append("</span>");
					sb.Append("<span class=\"citystatezip\">")
						.Append(StringUtils.FriendlyString(header.Store.City))
						.Append(", ")
						.Append(StringUtils.FriendlyString(header.Store.StateOrProvince))
						.Append(" ")
						.Append(StringUtils.FriendlyString(header.Store.ZipOrPostalCode))
						.Append("</span>");
					sb.Append("<span class=\"country\">").Append(StringUtils.FriendlyString(header.Store.Country)).Append("</span>");
					sb.Append("</p>");
					sb.Append("</div>");
					val = sb.ToString();
					break;

				case "IsQualifyingSpend":
					double spend = (header.IsQualifyingSpend ? header.Spend : 0);
					string formatString = StringUtils.FriendlyString(
						ResourceUtils.GetLocalWebResource("~/Controls/Modules/VisitMap/ViewVisitMap.ascx", "VisitListQualifyingAmountFormatString.Text"),
						"Qualifying Amount: {0:C}");
					val = string.Format(formatString, spend);
					break;
			}
			return val;
		}

		public override void SaveListData(IEnumerable<DynamicListColumnSpec> columns, AspDynamicList.ListActions listAction)
		{
			throw new NotImplementedException();
		}
		#endregion

		#endregion

		#region IVisitMapProvider implementation
		public bool HasGridProvider()
		{
			return false;
		}

		public IDynamicGridProvider GetGridProvider()
		{
			return null;
		}

		public bool HasListProvider()
		{
			return true;
		}

		public AspListProviderBase GetListProvider()
		{
			return this;
		}

		public int GetNumVisits()
		{
			return GetNumberOfRows();
		}

		public void SetVisitMapConfig(VisitMapConfig config)
		{
			_config = config;
		}

		public virtual IList<string> GetJSonStoresVisited()
		{
			IList<string> result = new List<string>();			
			if (_headers != null && _headers.Count > 0)
			{
				List<long> processedStoreIDs = new List<long>();
				foreach (var header in _headers)
				{
					if (header.Store == null) continue;
					if (processedStoreIDs.Contains(header.Store.StoreId)) continue;
					processedStoreIDs.Add(header.Store.StoreId);

					string jsonStore = JsonConvert.SerializeObject(header.Store);
					if (!string.IsNullOrEmpty(jsonStore))
					{
						result.Add(jsonStore);
					}
				}
			}
			return result;
		}

		public virtual IList<string> GetJSonStoresNotVisited()
		{
			IList<string> result = new List<string>();

			if (_headers == null || _headers.Count < 1)
			{
				return GetJSonStoresAll();
			}

			if (_stores != null && _stores.Count > 0)
			{
				foreach (var store in _stores)
				{
					if (store.Latitude == null || store.Longitude == null) continue;

					bool visited = false;
					foreach (var header in _headers)
					{
						if (header.Store != null && header.Store.StoreId == store.StoreId)
						{
							visited = true;
							break;
						}
					}
					if (visited) continue;

					string jsonStore = JsonConvert.SerializeObject(store);
					if (!string.IsNullOrEmpty(jsonStore))
					{
						result.Add(jsonStore);
					}
				}
			}
			return result;
		}

		public virtual IList<string> GetJSonStoresWithQualifiedSpend()
		{
			IList<string> result = new List<string>();
			if (_headers != null && _headers.Count > 0)
			{
				List<long> processedStoreIDs = new List<long>();
				foreach (var header in _headers)
				{
					if (header.Store == null) continue;
					if (!header.IsQualifyingSpend) continue;
					if (processedStoreIDs.Contains(header.Store.StoreId)) continue;
					processedStoreIDs.Add(header.Store.StoreId);

					string jsonStore = JsonConvert.SerializeObject(header.Store);
					if (!string.IsNullOrEmpty(jsonStore))
					{
						result.Add(jsonStore);
					}
				}
			}
			return result;
		}

		public virtual IList<string> GetJSonStoresAll()
		{
			IList<string> result = new List<string>();
			if (_stores != null && _stores.Count > 0)
			{
				foreach (var store in _stores)
				{
					if (store.Latitude == null || store.Longitude == null) continue;

					string jsonStore = JsonConvert.SerializeObject(store);
					if (!string.IsNullOrEmpty(jsonStore))
					{
						result.Add(jsonStore);
					}
				}
			}
			return result;
		}

		public virtual string GetMarkerImageUrl(MarkerType markerType, string userSpecifiedURL)
		{
			string result = string.Empty;
			switch (markerType)
			{
				default:
				case MarkerType.Default:
					break;

				case MarkerType.RedDot:
					result = "/Skin/images/visit_map/red.gif";
					break;

				case MarkerType.UserSpecifiedURL:
					result = userSpecifiedURL;
					break;

				case MarkerType.RedBalloon:
					result = "http://maps.google.com/mapfiles/ms/micons/red-dot.png";
					break;

				case MarkerType.GreenBalloon:
					result = "http://maps.google.com/mapfiles/ms/micons/green-dot.png";
					break;

				case MarkerType.LightBlueBalloon:
					result = "http://maps.google.com/mapfiles/ms/micons/ltblue-dot.png";
					break;

				case MarkerType.BlueBalloon:
					result = "http://maps.google.com/mapfiles/ms/micons/blue-dot.png";
					break;

				case MarkerType.YellowBalloon:
					result = "http://maps.google.com/mapfiles/ms/micons/yellow-dot.png";
					break;

				case MarkerType.PurpleBalloon:
					result = "http://maps.google.com/mapfiles/ms/micons/purple-dot.png";
					break;

				case MarkerType.PinkBalloon:
					result = "http://maps.google.com/mapfiles/ms/micons/pink-dot.png";
					break;
			}
			return result;
		}

		public virtual string GetVisitMapShareUrl()
		{
			string result = "http://maps.google.com/";
			if (_config != null)
			{
				string width = StringUtils.FriendlyString(_config.SharedMapWidth, "512");
				width = Regex.Replace(width, "[^0-9]", "");
				string height = StringUtils.FriendlyString(_config.SharedMapHeight, "512");
				height = Regex.Replace(height, "[^0-9]", "");

				string markers = string.Empty;
				List<long> processedStoreIDs = new List<long>();
				int maxLocations = _config.SharedMapMaxLocations;
				int numLocations = 0;
				if (_headers != null && _headers.Count > 0)
				{
					foreach (var header in _headers)
					{
						if (header.Store == null) continue;
						if (processedStoreIDs.Contains(header.Store.StoreId)) continue;
						processedStoreIDs.Add(header.Store.StoreId);

						if (maxLocations < 1 || numLocations < maxLocations)
						{
							markers += "%7C" + header.Store.Latitude + "," + header.Store.Longitude;
						}
						numLocations++;
					}
				}

				result = string.Format("http://maps.googleapis.com/maps/api/staticmap?size={0}x{1}&maptype={2}&sensor=false",
					width, height, _config.SharedMapType.ToString().ToLower());

				// specify center and zoom if there are no markers
				if (!_config.SharedMapAutoCenterAndZoom || string.IsNullOrEmpty(markers))
				{
					result += string.Format("&center={0},{1}&zoom={2}",
						_config.SharedMapCenterLat, _config.SharedMapCenterLong, _config.SharedMapZoom);
				}
				if (!string.IsNullOrEmpty(markers)) result += "&markers=" + markers;

			}
			return result;
		}
		#endregion
	}
}
