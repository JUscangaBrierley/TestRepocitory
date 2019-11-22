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

namespace Brierley.LWModules.VisitMap.Components
{
	public class DefaultVisitMapGridProvider : AspGridProviderBase, IDynamicGridProvider, IVisitMapProvider
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

		#region AspGridProviderBase overrides

        protected override string GetGridName()
        {
            return "VisitMap";
        }

		public override DynamicGridColumnSpec[] GetColumnSpecs()
		{
			DynamicGridColumnSpec[] columns = new DynamicGridColumnSpec[_numColumns];

			DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = ColumnName.a_txnheaderid.ToString();
			c.DisplayText = "Id";
			c.DataType = typeof(long);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;
			c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
			c.IsSortable = false;
			columns[(int)ColumnName.a_txnheaderid] = c;

			c = new DynamicGridColumnSpec();
			c.Name = ColumnName.TxnDate.ToString();
			c.DisplayText = "Date";
			c.DataType = typeof(string);
			c.IsEditable = false;
			c.IsVisible = true;
			c.IsSortable = true;
			columns[(int)ColumnName.TxnDate] = c;

			c = new DynamicGridColumnSpec();
			c.Name = ColumnName.ActivityDescription.ToString();
			c.DisplayText = "Location";
			c.DataType = typeof(string);
			c.IsEditable = false;
			c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
			c.IsVisible = true;
			c.IsSortable = true;
			columns[(int)ColumnName.ActivityDescription] = c;

			c = new DynamicGridColumnSpec();
			c.Name = ColumnName.IsQualifyingSpend.ToString();
			c.DisplayText = "Qualifying Spend";
			c.DataType = typeof(string);
			c.IsEditable = false;
			c.EditControlType = DynamicGridColumnSpec.TEXTBOX;
			c.IsVisible = true;
			c.IsSortable = true;
			columns[(int)ColumnName.IsQualifyingSpend] = c;

			return columns;
		}

		public override bool IsGridEditable()
		{
			return false;
		}

		public override void SetSearchParm(string parmName, object parmValue)
		{
			if (string.IsNullOrWhiteSpace(parmName))
			{
				_startDate = DateTimeUtil.MinValue;
				_endDate = DateTimeUtil.MaxValue;
			}
			else if (parmName == "FromDate")
			{
				_startDate = (DateTime)parmValue;
			}
			else if (parmName == "ToDate")
			{
				_endDate = (DateTime)parmValue;
				_endDate = _endDate.AddDays(1).AddTicks(-1);
			}
		}

		public override void LoadGridData()
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

		public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
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
					val = header.ActivityName;
					break;

				case "IsQualifyingSpend":
					if (header.IsQualifyingSpend) 
						val = "<img src=\"/Skin/images/visit_map/checkmark_16x16.png\" />";
					else 
						val = string.Empty;
					break;
			}
			return val;
		}

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IVisitMapProvider implementation
		public bool HasGridProvider()
		{
			return true;
		}

		public IDynamicGridProvider GetGridProvider()
		{
			return this;
		}

		public bool HasListProvider()
		{
			return false;
		}

		public AspListProviderBase GetListProvider()
		{
			return null;
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
				foreach (var header in _headers) {
					if (header.Store == null) continue;
					if (processedStoreIDs.Contains(header.Store.StoreId)) continue;
					processedStoreIDs.Add(header.Store.StoreId);

					if (maxLocations < 1 || numLocations < maxLocations)
					{
						markers += "%7C" + header.Store.Latitude + "," + header.Store.Longitude;
					}
					numLocations++;
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
