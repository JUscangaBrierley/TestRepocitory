using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;

using Brierley.WebFrameWork.Controls.List;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.CFAccountActivity.Components.List
{
    public class OrphansTxnListProvider : AspListProviderBase
    {
        #region Fields
        private const string _className = "OrphanTxnGridProvider";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

        private CFAccountActivityConfig _config = null;
        private List<Dictionary<string, object>> _parms = null;
        private IList<PointTransaction> _orphans = null;
        #endregion

        #region Private Helpers        
        #endregion

        #region List Properties
        public override string Id
        {
            get { return "lstCFAccountActivity_Orphans"; }
        }

        public override IEnumerable<DynamicListItem> GetListItemSpecs()
        {
            var items = new List<DynamicListItem>();

            var c = new DynamicListColumnSpec("Id", "Id", typeof(long), null, false, false, false) { IsKey = true };
            items.Add(c);

            foreach (ConfigurationItem attribute in _config.OrphanTxnFieldsToShow)
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

				c = new DynamicListColumnSpec(attribute.AttributeName, displayText) { BeginHtml = attribute.BeginHtml, EndHtml = attribute.EndHtml };
                c.IsKey = false;
                
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
            string msg = "No Point Transactions";
            if (totalRecords == 0)
            {
                return ResourceUtils.GetLocalWebResource(ParentControl, "NoPointsMessage.Text", msg);
            }
            else
            {
                msg = "Total";
                return string.Format(ResourceUtils.GetLocalWebResource(ParentControl, "Total.Text", msg) + " {0} ", totalRecords);
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
            else if (parmName == "SearchParms")
            {
                _parms = (List<Dictionary<string, object>>)parmValue;
            }            
        }
        
        public override void LoadListData()
        {
            string methodName = "LoadGridData";

            Member member = PortalState.CurrentMember;
            if (member != null && member.LoyaltyCards != null && member.LoyaltyCards.Count > 0)
            {
                try
                {
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

                    _orphans = AccountActivityUtil.GetOtherPointsHistory(member, startDate, endDate, _config.OrphanTxnTypesFilter, _config.OrphanPointTypesFilter, _config.OrphanPointEventsFilter, _config.ShowExpiredTransactions, _config.AggregateOrphanPoints, new LWQueryBatchInfo() { BatchSize = 1000, StartIndex = 0 });
                    if (_orphans != null && _orphans.Count > 0)
                    {
                        _orphans = _orphans.OrderByDescending(x => x.TransactionDate).ToList();
                    }
                }
                catch (LWException ex)
                {
                    if (ex.ErrorCode != 3230)
                    {
                        _logger.Error(_className, methodName, "Error loading grid data.", ex);
                        throw;
                    }
                    else
                    {
                        _logger.Error(_className, methodName, ex.Message);
                    }
                }
            }
            else
            {
                _orphans = null;
            }
        }

        public override int GetNumberOfRows()
        {
            return _orphans != null ? _orphans.Count : 0;
        }

        public override object GetColumnData(int rowIndex, DynamicListColumnSpec column)
        {
            object val = null;
            PointTransaction pt = _orphans[rowIndex];

            if (column.Name == "Id")
            {
                return pt.Id;
            }

            //ConfigurationItem item = null;
            //foreach (var i in _config.OrphanTxnFieldsToShow)
            //{
            //    if (i.DataKey == column.Name)
            //    {
            //        item = i;
            //        break;
            //    }
            //}

            //if (item == null)
            //{
            //    return null;
            //}

            switch (column.Name)
            {
                case "VcKey":
                    // get the loyalty id
                    Member member = PortalState.CurrentMember;
                    VirtualCard vc = member.GetLoyaltyCard(pt.VcKey);
                    val = vc.LoyaltyIdNumber;
                    break;
                case "PointTypeId":
                    PointType c = LoyaltyService.GetPointType(pt.PointTypeId);
                    val = c.Name;
                    break;
                case "PointEventId":
                    PointEvent pe = LoyaltyService.GetPointEvent(pt.PointEventId);
                    val = pe.Name;
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
                default:
                    PropertyInfo prop = pt.GetType().GetProperty(column.Name);
                    object pval = prop.GetValue(pt, null);
                    if (pval is DateTime)
                    {
                        val = ((DateTime)pval).ToShortDateString();
                    }
                    else if (pval != null)
                    {
						if (pval is decimal)
						{
							val = ((decimal)pval).ToString("0.#####");
						}
						else
						{
							val = pval.ToString();
						}
                    }
                    break;
            }
            return val;
        }

        public override void SaveListData(IEnumerable<DynamicListColumnSpec> columns, AspDynamicList.ListActions listAction)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}