using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;

using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;


namespace Brierley.LWModules.CFAccountActivity.Components.List
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
    }
    #endregion

    public class TxnHeaderListProvider : AspListProviderBase
    {
        #region Fields
        private CFAccountActivityConfig _config = null;
        private List<Dictionary<string, object>> _parms = null;
        private IList<TransactionHeader> _headers = null;
        private bool _ruleResultRequired = false;        
        #endregion        

        #region Private Helpers
        private object GetColumnData(TransactionHeader header, string columnName)
        {
            object val = null;

            if (columnName == "RowKey")
            {
                return header.TxnHeader.RowKey;
            }

            ConfigurationItem item = null;
            foreach (var i in _config.HeaderFieldsToShow)
            {
                if (i.AttributeName == columnName)
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
					val = header.TxnPoints.ToString("0.#####");
                    break;
                case "RuleExecution":
                    val = header.RuleResult;
                    break;
                default:
                    object hValue = header.TxnHeader.GetAttributeValue(item.AttributeName);
                    if (hValue != null)
                    {
						if (hValue is decimal)
						{
							val = ((decimal)hValue).ToString("0.#####");
						}
                        else if (hValue is DateTime)
                        {
                            val = ((DateTime)hValue).ToShortDateString();
                        }
                        else
                        {
                            val = hValue != null ? hValue.ToString() : string.Empty;
                        }
                    }
                    break;
            }
            return val;
        }
        #endregion

        #region List Properties
        public override string Id
        {
            get { return "lstCFAccountActivity_SalexTxns"; }
        }

        public override IEnumerable<DynamicListItem> GetListItemSpecs()
        {            
            var items = new List<DynamicListItem>();

            var c = new DynamicListColumnSpec("RowKey", "RowKey", typeof(long), null, false, false, false) { IsKey = true };
            items.Add(c);

            foreach (ConfigurationItem attribute in _config.HeaderFieldsToShow)
            {
                string displayText = string.Empty;
				if (!string.IsNullOrEmpty(attribute.ResourceKey))
				{
					displayText = ResourceUtils.GetLocalWebResource(ParentControl, attribute.ResourceKey, attribute.DisplayText);
				}
				else
				{
					displayText = attribute.DisplayText;
				}

                c = new DynamicListColumnSpec(attribute.AttributeName, displayText) { BeginHtml = attribute.BeginHtml, EndHtml = attribute.EndHtml };
                c.IsKey = false;

                if (attribute.AttributeName == "RuleExecution")
                {
                    _ruleResultRequired = true;
                }

                c.CssClass = string.IsNullOrEmpty(attribute.LabelCssClass) ? attribute.ControlCSSClass : attribute.LabelCssClass;

                items.Add(c);
            }
            return items;
        }

		public override IEnumerable<DynamicListItem> GetViewItemSpecs()
		{
			return GetListItemSpecs();
		}

        public override string GetEmptyListMessage()
        {
			string errMsg = "No transactions found.";
            if (!string.IsNullOrEmpty(_config.EmptyResultMessageResourceKey))
            {
                errMsg = ResourceUtils.GetLocalWebResource(ParentControl, _config.EmptyResultMessageResourceKey, errMsg);                
            }

            return errMsg;
        }

        public override string GetAppPanelTotalText(int totalRecords)
        {
            if (totalRecords == 0)
            {
                return ResourceUtils.GetLocalWebResource(ParentControl, "NoSalesActivity.Text", "No Sales Activity");
            }
            else
            {
                return string.Format("Total: {0} ", totalRecords);
            }
        }

        #endregion

        #region Data Source Related
        public override void SetSearchParm(string parmName, object parmValue)
        {
            if (parmName == "Config")
            {
                _config = (CFAccountActivityConfig)parmValue;                
            }
        }

        public override void SetSearchParm(List<Dictionary<string, object>> parms)
        {
            _parms = parms;            
            base.SetSearchParm(parms);
        }

        public List<Dictionary<string, object>> GetSearchParms()
        {
            return _parms;
        }

        public override void LoadListData()
        {
            _headers = new List<TransactionHeader>();
            Member member = PortalState.CurrentMember;

            DateTime? startDate = null, endDate = null;

            if (_parms != null)
            {
                foreach (var dict in _parms)
                {
                    if (dict.ContainsKey("FromDate"))
                    {
                        startDate = DateTime.Parse(dict["FromDate"].ToString());                        
                    }
                    if (dict.ContainsKey("ToDate"))
                    {
                        endDate = DateTime.Parse(dict["ToDate"].ToString());                        
                    }
                }
            }

            IList<IClientDataObject> txnHeaders = AccountActivityUtil.GetAccountActivitySummary(member, startDate, endDate, true, _config.ShowExpiredTransactions, new LWQueryBatchInfo() { BatchSize = 1000, StartIndex = 0 });

            if (txnHeaders != null && txnHeaders.Count > 0)
            {
                foreach (IClientDataObject cdo in txnHeaders)
                {                    
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
                            th.TxnPoints += point.Points;
                        }
                    }
                    _headers.Add(th);
                    //txnMap.Add(rowKey, th);
                }
            }
            if (_headers != null && _headers.Count > 0)
            {
                _headers = _headers.OrderByDescending(x => x.TxnDate).ToList();
            }
        }

        public override int GetNumberOfRows()
        {
            return _headers != null ? _headers.Count : 0;            
        }
        
        public override object GetColumnData(int rowIndex, DynamicListColumnSpec column)
        {
            var header = _headers[rowIndex];

            if (column.Name == "RowKey")
            {
                return header.TxnHeader.RowKey;
            }

            return GetColumnData(header, column.Name);
        }
        
        public override void SaveListData(IEnumerable<DynamicListColumnSpec> columns, AspDynamicList.ListActions listAction)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}