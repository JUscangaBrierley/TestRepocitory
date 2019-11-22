using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.FrameWork.Pdf;

namespace Brierley.LWModules.CSTriggerUserEvent.Components
{	
    public class TriggerUserEventGridProvider : AspGridProviderBase, INestedGridProvider
	{
		#region Fields
        private static string _className = "TriggerUserEventGridProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private DateTime _startDate = DateTimeUtil.MinValue;
		private DateTime _endDate = DateTimeUtil.MaxValue;
		private string _eventFilter = null;
		private CSTriggerUserEventConfiguration _config = null;
        private IList<TriggerUserEventLog> _events = new List<TriggerUserEventLog>();
        private bool _ruleResultRequired = false;
        private IList<long> _eventIds = null;
		#endregion

		#region Helpers        
        private TriggerUserEventLog LoadEvent(long id)
        {
            TriggerUserEventLog uevent = (from x in _events where x.Id == id select x).FirstOrDefault();
            if (uevent == null)
            {
                uevent = LoyaltyService.GetTriggerUserEventLog(id);
                _events.Add(uevent);
            }
            return uevent;
        }

        private object GetColumnData(TriggerUserEventLog userEvent, DynamicGridColumnSpec column)
		{
			object val = null;

			if (column.Name == "Id")
			{
                val = userEvent.Id;
			}
            else if (column.Name == "EventName")
            {
                LWEvent e = LoyaltyService.GetLWEventByName(userEvent.EventName);
                val = e.DisplayText;
            }
            else if (column.Name == "Channel")
            {
                val = userEvent.Channel;
            }
            else if (column.Name == "EventTime")
            {
                val = userEvent.CreateDate;
            }            
			return val;
		}
                
		#endregion

		#region Child Grid
		public IDynamicGridProvider GetChildGridProvider(object keyVal)
		{
			string methodName = "GetChildGridProvider";

			if (_config != null &&
				!string.IsNullOrEmpty(_config.OffersProviderAssemblyName) &&
				!string.IsNullOrEmpty(_config.OffersProviderClassName))
			{
                object childProvider = ClassLoaderUtil.CreateInstance(_config.OffersProviderAssemblyName, _config.OffersProviderClassName);
				if (childProvider != null)
				{
					IDynamicGridProvider p = childProvider as IDynamicGridProvider;
					if (p != null)
					{
                        p.ParentControl = "~/Controls/Modules/CSTriggerUserEvent/ViewCSTriggerUserEvent.ascx";
					}
					PropertyInfo pinfo = childProvider.GetType().GetProperty("ParentKey");
					if (pinfo != null)
					{
						pinfo.SetValue(childProvider, keyVal, null);
						((IDynamicGridProvider)childProvider).SetSearchParm("Config", _config);
						return (IDynamicGridProvider)childProvider;
					}
					else
					{
						string errMsg = string.Format("Child grid for Offers does not define property ParentKey.");
						_logger.Error(_className, methodName, errMsg);
						throw new LWException(errMsg);
					}
				}
				else
				{
					string errMsg = string.Format("Error creating child grid for Offers.");
					_logger.Error(_className, methodName, errMsg);
					throw new LWException(errMsg);
				}
			}
			else
			{
                OffersGridProvider childProvider = new OffersGridProvider((long)keyVal);
                childProvider.ParentControl = "~/Controls/Modules/CSTriggerUserEvent/ViewCSTriggerUserEvent.ascx";
				childProvider.SetSearchParm("Config", _config);
				return childProvider;
			}
		}

		public NestingTypes NestingType
		{
			get { return NestingTypes.DataBound; }
		}

		public bool HasChildren(object keyVal)
		{
			
			long rowkey = (long)keyVal;
            TriggerUserEventLog uevent = LoadEvent(rowkey);
            long[] messageIds;
            long[] couponIds;
            long[] bonusIds;
            long[] promotionIds;

            int offersCount = TriggerUserEventUtil.DeserializeResult(uevent.Result, out messageIds, out couponIds, out bonusIds, out promotionIds);
            return offersCount > 0 ? true : false;			
		}
		#endregion

		#region Grid Properties

		public override string Id
		{
			get { return "grdTriggerUserEvent"; }
		}

        protected override string GetGridName()
        {
            return "TriggerUserEvent";
        }

		public override DynamicGridColumnSpec[] GetColumnSpecs()
		{
			IList<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();
			
            DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = "Id";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-RowKey.Text", "Id");
			c.DataType = typeof(long);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;
			c.IsSortable = false;
			colList.Add(c);

            c = new DynamicGridColumnSpec();
            c.Name = "EventName";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-EventName.Text", "Event");
            c.DataType = typeof(string);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = true;
            colList.Add(c);

            c = new DynamicGridColumnSpec();
            c.Name = "Channel";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Channel.Text", "Channel");
            c.DataType = typeof(string);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = true;
            colList.Add(c);

            c = new DynamicGridColumnSpec();
            c.Name = "EventTime";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-EventTime.Text", "Event Time");
            c.DataType = typeof(string);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = true;
            colList.Add(c);
			
			return colList.ToArray<DynamicGridColumnSpec>();
		}

		public override bool IsGridEditable()
		{
			return false;
		}

        public override bool IsSummaryMode()
        {
            return true;
        }

        public override string GetEmptyGridMessage()
        {
            string errMsg = string.Empty;
            if (_config != null && !string.IsNullOrEmpty(_config.EmptyResultMessageResourceKey))
            {
                errMsg = ResourceUtils.GetLocalWebResource(ParentControl, _config.EmptyResultMessageResourceKey, "NO user events found.");
            }
            return errMsg;
        }

		#endregion

		#region Data Sources
		public override void SetSearchParm(string parmName, object parmValue)
		{
			if (string.IsNullOrWhiteSpace(parmName))
			{
				_startDate = DateTimeUtil.MinValue;
				_endDate = DateTimeUtil.MaxValue;
			}
			else if (parmName == "FromDate")
			{
				_startDate = parmValue != null ? (DateTime)parmValue : DateTimeUtil.MinValue;
			}
			else if (parmName == "ToDate")
			{
				_endDate = parmValue != null ? (DateTime)parmValue : DateTimeUtil.MaxValue;
				_endDate = _endDate.AddDays(1).AddTicks(-1);
			}
			else if (parmName == "Config")
			{
				_config = (CSTriggerUserEventConfiguration)parmValue;
			}
		}

        public override void LoadGridData()
        {
            LoadGridData(null,true);
        }

        public override void LoadGridData(string sortExpression = null, bool ascending = true)
		{
            Member member = PortalState.CurrentMember;
            if (member != null)
            {
                _eventIds = LoyaltyService.GetTriggerUserEventLogIds(member.IpCode, sortExpression, ascending);
            }

            if (_eventIds != null && _eventIds.Count > 0)
            {
                PortalState.PutInCache("Events", _eventIds);
            }
		}

		public override int GetNumberOfRows()
		{
            return _eventIds.Count;
		}

		public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
		{
            if (column.Name == "Id")
            {
                return _eventIds[rowIndex];
            }
            TriggerUserEventLog uevent = LoadEvent(_eventIds[rowIndex]);
            return GetColumnData(uevent, column);           
		}

		public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Filtering
		public override List<DynamicGridFilter> GetFilters()
		{
			var filters = new List<DynamicGridFilter>();
			filters.Add(new DynamicGridFilter("Activity:", FilterDisplayTypes.DropDownList, "-- Select --", "Sale", "Return", "Adjustment", "Appeasement"));
			return filters;
		}

		public override void SetFilter(string filterName, string filterValue)
		{
			_eventFilter = filterValue;
		}
		#endregion

		#region Printing
        /// <summary>
        /// Since this is a nested grid, regular prinitng mechanism through  the grid.  Hence this is specifically
        /// designed to be controlled form the ViewAccoutnActivity module.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
		public override bool GeneratePdfFromData(string fileName)
		{
			bool success = true;
                        
            //AttributeSetMetaData hdrMeta = loyaltyService.GetAttributeSetMetaData("TxnHeader");
            //AttributeSetMetaData dtlMeta = loyaltyService.GetAttributeSetMetaData("TxnDetailItem");
            //if (_headers != null && _headers.Count > 0)
            //{
            //    PdfDocument doc = (PdfDocument)PortalState.GetFromCache("AccountActivityPdf");
            //    bool first = true;
            //    foreach (TransactionHeader header in _headers)
            //    {
            //        if (first)
            //        {
            //            PortalState.PutInCache("AccountActivityPdf", doc);
            //            doc.EmptyLine();
            //            first = false;
            //        }
            //        doc.WriteLine("Txn Id: " + GetColumnData(header, "RowKey"));
            //        doc.LineSeparator();
            //        foreach (ConfigurationItem attribute in _config.HeaderFieldsToShow)
            //        {
            //            object value = GetColumnData(header, attribute.DataKey);
            //            if (value != null)
            //            {
            //                string displayText = GetAttributeDisplayText(attribute);
            //                doc.WriteLine(displayText + " : " + value.ToString());
            //            }
            //        }

            //        foreach (TransactionDetail detail in header.Details)
            //        {
            //            if (detail.PointTransaction != null)
            //            {
            //                doc.FontStyle = PdfDocument.PdfFontStyle.Italic;
            //                doc.NewLine();
            //                doc.WriteLine(hdrMeta.DisplayText + " - Loyalty Currency Transaction");
            //                doc.FontStyle = PdfDocument.PdfFontStyle.Normal;
            //                foreach (ConfigurationItem attribute in _config.DetailFieldsToShow)
            //                {
            //                    if (attribute.AttributeType == ItemTypes.PointTransaction)
            //                    {
            //                        object value = GetPointTransactionData(detail.PointTransaction, attribute.DataKey);
            //                        if (value != null)
            //                        {
            //                            string displayText = GetAttributeDisplayText(attribute);
            //                            doc.WriteLine("    " + displayText + " : " + value.ToString());
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                doc.FontStyle = PdfDocument.PdfFontStyle.Italic;
            //                doc.NewLine();
            //                doc.WriteLine(dtlMeta.DisplayText);
            //                doc.FontStyle = PdfDocument.PdfFontStyle.Normal;
            //                foreach (ConfigurationItem attribute in _config.DetailFieldsToShow)
            //                {
            //                    object value = GetTxnDetailData(detail, attribute.DataKey);
            //                    if (value != null)
            //                    {
            //                        string displayText = GetAttributeDisplayText(attribute);
            //                        doc.WriteLine("    " + displayText + ":" + value.ToString());
            //                    }
            //                }
            //            }
            //        }
            //        doc.NewLine();
            //    }
            //}
			return success;
		}
		#endregion
	}
}
