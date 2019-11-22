using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Reflection;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

using Brierley.FrameWork.Pdf;

namespace Brierley.LWModules.AccountActivity.Components
{
	#region Utility Classes
	public class TransactionHeader
	{
		public DateTime TxnDate { get; set; }
		public string StoreName { get; set; }
		public string StoreCity { get; set; }
		public string StoreState { get; set; }
		public decimal TxnPoints { get; set; }
        public string RuleResult { get; set; }
		public IClientDataObject TxnHeader { get; set; }
		public IList<TransactionDetail> Details = new List<TransactionDetail>();
	}

	public class TransactionDetail
	{
		public long RowKey { get; set; }
        public PointTransaction PointTransaction { get; set; }
        public IClientDataObject DetailRecord { get; set; }
        public string RuleResult { get; set; }
	}
	#endregion

	public class TxnHeaderGridProvider : AspGridProviderBase, INestedGridProvider
	{
		#region Fields
		private static string _className = "TxnHeaderGridProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);
		private DateTime _startDate = DateTimeUtil.MinValue;
		private DateTime _endDate = DateTimeUtil.MaxValue;
		private string _activityFilter = null;
		private AccountActivityConfiguration _config = null;
		private IList<TransactionHeader> _headers = null;
        private bool _ruleResultRequired = false;
		#endregion

		#region Helpers
        private string GetAttributeDisplayText(ConfigurationItem attribute)
        {
            string displayText = string.IsNullOrEmpty(attribute.DisplayText) ? attribute.AttributeName : attribute.DisplayText; ;
            if (!string.IsNullOrEmpty(attribute.ResourceKey))
            {
                displayText = ResourceUtils.GetLocalWebResource(ParentControl, attribute.ResourceKey, attribute.DisplayText);
            }
            return displayText;
        }

		private object GetColumnData(TransactionHeader header, string columnName)
		{
            string methodName = "GetColumnData";

			object val = null;

			if (columnName == "RowKey")
			{
				return header.TxnHeader.RowKey;
			}

			ConfigurationItem item = null;
			foreach (var i in _config.HeaderFieldsToShow)
			{
				if (i.DataKey == columnName)
				{
					item = i;
					break;
				}
			}

			switch (item.AttributeName)
			{
                case "LoyaltyId":
                    VirtualCard vc = PortalState.CurrentMember.GetLoyaltyCard(header.TxnHeader.ParentRowKey);
                    if (vc != null)
                    {
                        val = vc.LoyaltyIdNumber;
                    }
                    break;
				case "StoreName":
					val = header.StoreName;
					break;
				case "StoreCity":
					val = header.StoreCity;
					break;
				case "StoreState":
					val = header.StoreState;
					break;
				case "TotalPoints":
					val = header.TxnPoints; //.ToString();
					break;
                case "RuleExecution":
                    val = header.RuleResult;
                    break;
				default:
                    if (item.AttributeType == ItemTypes.BScript)
                    {
                        if (!string.IsNullOrWhiteSpace(item.BScriptExpression))
                        {
                            try
                            {
                                ContextObject ctx = new ContextObject() { Owner = header.TxnHeader.Parent, InvokingRow = header.TxnHeader };
                                ExpressionFactory exprF = new ExpressionFactory();
                                Expression expression = exprF.Create(item.BScriptExpression);
                                val = expression.evaluate(ctx);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(_className, methodName, string.Format("Error evaluating {0}.", item.AttributeName), ex);
                                throw;
                            }
                        }
                    }
                    else
                    {
                        val = header.TxnHeader.GetAttributeValue(item.AttributeName);
                    }					
					break;
			}
			return val;
		}

        /// <summary>
        /// This operation is essentially a copy of the GetColumnData method of the TxnDetailGridProvider.
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="columnKey"></param>
        /// <returns></returns>
        private object GetPointTransactionData(PointTransaction pt, string columnKey)
        {
            object val = null;

            ConfigurationItem item = null;
            foreach (var i in _config.DetailFieldsToShow)
            {
                if (i.DataKey == columnKey)
                {
                    item = i;
                    break;
                }
            }

            if (item == null || item.AttributeType != ItemTypes.PointTransaction)
            {
                return null;
            }

            switch (item.AttributeName)
            {
                case "PointTypeId":
                    PointType c = LoyaltyService.GetPointType(pt.PointTypeId);
                    val = c.Name;
                    break;
                case "PointEventId":
                    PointEvent pe2 = LoyaltyService.GetPointEvent(pt.PointEventId);
                    val = pe2.Name;
                    break;
                case "PromoName":
                    if (!string.IsNullOrEmpty(pt.PromoCode))
                    {
                        Promotion p = LoyaltyService.GetPromotionByCode(pt.PromoCode);
                        if (p != null)
                        {
                            val = p.Name;
                        }
                    }
                    break;
                case "RuleExecution":
                    // since there is no such attribute here but this may be called so just return empty string.
                    val = string.Empty;
                    break;
                default:
                    PropertyInfo prop = typeof(PointTransaction).GetProperty(item.AttributeName);
                    object pval = prop.GetValue(pt, null);
                    if (pval is DateTime)
                    {
                        val = ((DateTime)pval).ToShortDateString();
                    }
                    else if (pval != null)
                    {
                        val = pval.ToString();
                    }
                    break;
            }
            return val;
        }

        /// <summary>
        /// This operation is essentially a copy of the GetColumnData method of the TxnDetailGridProvider. 
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="columnKey"></param>
        /// <returns></returns>
        private object GetTxnDetailData(TransactionDetail detail, string columnKey)
        {
            object val = null;
            if (columnKey == "RowKey")
            {
                return detail.RowKey;
            }

            ConfigurationItem item = null;
            foreach (var i in _config.DetailFieldsToShow)
            {
                if (i.DataKey == columnKey)
                {
                    item = i;
                    break;
                }
            }

            if (item == null || item.AttributeType == ItemTypes.PointTransaction)
            {
                return null;
            }

            object hValue = detail.DetailRecord.GetAttributeValue(item.AttributeName);
            if (hValue is DateTime)
            {
                val = ((DateTime)hValue).ToShortDateString();
            }
            else
            {
                val = hValue != null ? hValue.ToString() : string.Empty;
            }
            if (item.AttributeName == "DtlProductId" && val != null)
            {
                if (!string.IsNullOrEmpty(val.ToString()))
                {
                    long productId = long.Parse(val.ToString());
                    Product p = ContentService.GetProduct(productId);
                    if (p != null)
                    {
                        val = p.ShortDescription;
                    }
                }
            }
            return val;
        }
		#endregion

		#region Child Grid
		public IDynamicGridProvider GetChildGridProvider(object keyVal)
		{
			string methodName = "GetChildGridProvider";

			if (_config != null &&
				!string.IsNullOrEmpty(_config.DetailsProviderAssemblyName) &&
				!string.IsNullOrEmpty(_config.DetailsProviderClassName))
			{
				object childProvider = ClassLoaderUtil.CreateInstance(_config.DetailsProviderAssemblyName, _config.DetailsProviderClassName);
				if (childProvider != null)
				{
					IDynamicGridProvider p = childProvider as IDynamicGridProvider;
					if (p != null)
					{
						p.ParentControl = "~/Controls/Modules/AccountActivity/ViewAccountActivity.ascx";
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
						string errMsg = string.Format("Child grid for TxnHeaders does not define property ParentKey.");
						_logger.Error(_className, methodName, errMsg);
						throw new LWException(errMsg);
					}
				}
				else
				{
					string errMsg = string.Format("Error creating child grid for TxnHeaders.");
					_logger.Error(_className, methodName, errMsg);
					throw new LWException(errMsg);
				}
			}
			else
			{
				TxnDetailGridProvider childProvider = new TxnDetailGridProvider((long)keyVal);
				childProvider.ParentControl = "~/Controls/Modules/AccountActivity/ViewAccountActivity.ascx";
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
			if (_config.DetailFieldsToShow.Count == 0)
			{
				return false;
			}

			long rowkey = (long)keyVal;
			foreach (TransactionHeader hdr in _headers)
			{
				if (hdr.TxnHeader.RowKey == rowkey)
				{
					return hdr.Details.Count > 0 ? true : false;
				}
			}
			return false;
		}
		#endregion

		#region Grid Properties

		public override string Id
		{
			get { return "grdTxnHeader"; }
		}

        protected override string GetGridName()
        {
            return "TxnHeader";
        }

		public override DynamicGridColumnSpec[] GetColumnSpecs()
		{
			IList<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();
			DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = "RowKey";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-RowKey.Text", "Id");
			c.DataType = typeof(long);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;
			c.IsSortable = false;
			colList.Add(c);

			foreach (ConfigurationItem attribute in _config.HeaderFieldsToShow)
			{
				string displayText = string.Empty;
				if (!string.IsNullOrEmpty(attribute.ResourceKey))
				{
					displayText = ResourceUtils.GetLocalWebResource(ParentControl, attribute.ResourceKey, attribute.DisplayText);
				}
				if (string.IsNullOrEmpty(displayText))
				{
					displayText = string.IsNullOrEmpty(attribute.DisplayText) ? attribute.AttributeName : attribute.DisplayText;
				}

                if (attribute.AttributeName == "RuleExecution")
                {
                    _ruleResultRequired = true;
                }

				c = new DynamicGridColumnSpec();
				c.Name = attribute.DataKey;
				c.DisplayText = displayText;
                c.FormatString = attribute.Format;
				c.IsKey = false;
				c.IsEditable = false;
				c.IsVisible = true;
                c.IsSortable = attribute.IsSortable;
				colList.Add(c);
			}
			return colList.ToArray<DynamicGridColumnSpec>();
		}

		public override bool IsGridEditable()
		{
			return false;
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
				_config = (AccountActivityConfiguration)parmValue;
			}
		}

		public override void LoadGridData()
		{
			_headers = new List<TransactionHeader>();
			Member member = PortalState.CurrentMember;

			IList<IClientDataObject> txnHeaders = AccountActivityUtil.GetAccountActivitySummary(member, _startDate, _endDate, true, _config.ShowExpiredTransactions, new LWQueryBatchInfo() { BatchSize = 1000, StartIndex = 0 });

			AttributeSetContainer[] parents = null;
			Dictionary<long, TransactionHeader> txnMap = new Dictionary<long, TransactionHeader>();

			if (txnHeaders != null && txnHeaders.Count > 0)
			{
				parents = new AttributeSetContainer[txnHeaders.Count];
				int idx = 0;
				foreach (IClientDataObject cdo in txnHeaders)
				{
					parents[idx++] = (AttributeSetContainer)cdo;

					var th = new TransactionHeader() { TxnHeader = cdo };
					long rowKey = cdo.RowKey;
					object transactionDate = cdo.GetAttributeValue("TxnDate");
					if (transactionDate != null)
					{
						th.TxnDate = Convert.ToDateTime(transactionDate);
					}
					if (cdo.HasTransientProperty("Store"))
					{
						var store = (StoreDef)cdo.GetTransientProperty("Store");
						if (store != null)
						{
							th.StoreName = store.StoreName;
							th.StoreCity = store.City;
							th.StoreState = store.StateOrProvince;
						}
					}
                    if (_ruleResultRequired)
                    {
                        long[] rowkeys = new long[] { cdo.MyKey };
                        IList<RuleExecutionLog> logs = LoyaltyService.GetRuleExecutionLogs(
                            member.IpCode, null,
                            PointTransactionOwnerType.AttributeSet, cdo.GetMetaData().ID,
                            rowkeys, null, null);
                        if (logs != null && logs.Count > 0)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            foreach (RuleExecutionLog log in logs)
                            {
                                if (_config.DisplayRuleExecutionLog(log))
                                {
                                    if (sb.Length > 0)
                                    {
                                        sb.Append("\n");
                                    }
                                    sb.Append(log.Detail);
                                }
                            }
                            th.RuleResult = sb.ToString();
                        }
                    }
					if (cdo.HasTransientProperty("PointsHistory"))
					{
						IList<PointTransaction> points = (IList<PointTransaction>)cdo.GetTransientProperty("PointsHistory");
						foreach (var point in points)
						{
							TransactionDetail detail = new TransactionDetail()
							{
								RowKey = point.Id,
                                PointTransaction = point                                
							};
							th.Details.Add(detail);
							th.TxnPoints += point.Points;
						}
					}
					_headers.Add(th);
					txnMap.Add(rowKey, th);
				}
			}

			// Retrive the details
			if (parents != null)
			{
				IList<IClientDataObject> details = AccountActivityUtil.GetAccountActivityDetails(parents, true, _config.ShowExpiredTransactions);
				if (details != null && details.Count > 0)
				{
					foreach (IClientDataObject dObj in details)
					{
						TransactionHeader hdr = txnMap[dObj.ParentRowKey];
						TransactionDetail detail = new TransactionDetail()
						{
							RowKey = dObj.RowKey,
                            DetailRecord = dObj
						};                        
						hdr.Details.Add(detail);
					}
				}
			}

			PortalState.PutInCache("Headers", _headers);
		}

		public override int GetNumberOfRows()
		{
			return _headers.Count;
		}

		public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
		{
			var header = _headers[rowIndex];

			if (column.Name == "RowKey")
			{
				return header.TxnHeader.RowKey;
			}

			return GetColumnData(header, column.Name);
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
			_activityFilter = filterValue;
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

            AttributeSetMetaData hdrMeta= LoyaltyService.GetAttributeSetMetaData("TxnHeader");
            AttributeSetMetaData dtlMeta= LoyaltyService.GetAttributeSetMetaData("TxnDetailItem");

            if (_headers != null && _headers.Count > 0)
            {
                PdfDocument doc = (PdfDocument)PortalState.GetFromCache("AccountActivityPdf");
                bool first = true;
                foreach (TransactionHeader header in _headers)
                {
                    if (first)
                    {
                        PortalState.PutInCache("AccountActivityPdf", doc);
                        doc.EmptyLine();
                        first = false;
                    }
                    doc.WriteLine("Txn Id: " + GetColumnData(header, "RowKey"));
                    doc.LineSeparator();
                    foreach (ConfigurationItem attribute in _config.HeaderFieldsToShow)
                    {
                        object value = GetColumnData(header, attribute.DataKey);
                        if (value != null)
                        {
                            string displayText = GetAttributeDisplayText(attribute);
                            doc.WriteLine(displayText + " : " + value.ToString());
                        }
                    }

                    foreach (TransactionDetail detail in header.Details)
                    {
                        if (detail.PointTransaction != null)
                        {
                            doc.FontStyle = PdfDocument.PdfFontStyle.Italic;
                            doc.NewLine();
                            doc.WriteLine(hdrMeta.DisplayText + " - Loyalty Currency Transaction");
                            doc.FontStyle = PdfDocument.PdfFontStyle.Normal;
                            foreach (ConfigurationItem attribute in _config.DetailFieldsToShow)
                            {
                                if (attribute.AttributeType == ItemTypes.PointTransaction)
                                {
                                    object value = GetPointTransactionData(detail.PointTransaction, attribute.DataKey);
                                    if (value != null)
                                    {
                                        string displayText = GetAttributeDisplayText(attribute);
                                        doc.WriteLine("    " + displayText + " : " + value.ToString());
                                    }
                                }
                            }
                        }
                        else
                        {
                            doc.FontStyle = PdfDocument.PdfFontStyle.Italic;
                            doc.NewLine();
                            doc.WriteLine(dtlMeta.DisplayText);
                            doc.FontStyle = PdfDocument.PdfFontStyle.Normal;
                            foreach (ConfigurationItem attribute in _config.DetailFieldsToShow)
                            {
                                object value = GetTxnDetailData(detail, attribute.DataKey);
                                if (value != null)
                                {
                                    string displayText = GetAttributeDisplayText(attribute);
                                    doc.WriteLine("    " + displayText + ":" + value.ToString());
                                }
                            }
                        }
                    }
                    doc.NewLine();
                }
            }
			return success;
		}
		#endregion
	}
}
