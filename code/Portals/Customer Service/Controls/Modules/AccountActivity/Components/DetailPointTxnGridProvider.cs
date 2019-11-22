using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal.Configuration.Modules;
using System.Reflection;

namespace Brierley.LWModules.AccountActivity.Components
{
    public class DetailPointTxnGridProvider : AspGridProviderBase
    {
        #region Fields
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);

        private long _dtlRowKey;
        private IList<PointTransaction> _txns = null;
        #endregion

		public AccountActivityConfiguration Config { get; set; }

        public DetailPointTxnGridProvider(long keyVal)
        {
            _dtlRowKey = keyVal;            
        }

        public long ParentKey 
        {
            set { _dtlRowKey = value; }
        }
        
        #region Grid properties

        public override string Id
        {
            get { return "grdDetailPointTxn"; }
        }

        protected override string GetGridName()
        {
            return "DetailPointTxn";
        }

        public override DynamicGridColumnSpec[] GetColumnSpecs()
        {

			IList<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();
			DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = "Id";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-Id.Text", "Id");
			c.DataType = typeof(long);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;
			c.IsSortable = false;
			colList.Add(c);

			foreach (ConfigurationItem attribute in Config.PointTxnFieldsToShow)
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

        public override string GetEmptyGridMessage()
        {
			return ResourceUtils.GetLocalWebResource(ParentControl, Id + "-EmptyResultMessage.Text");
        }
        #endregion
        
        #region Grid Data Source
        
        public override void LoadGridData()
        {
            IList<TransactionDetail> details = (IList<TransactionDetail>)PortalState.GetFromCache("Details");
            if (details != null && details.Count > 0)
            {
                foreach (TransactionDetail detail in details)
                {
                    if (detail.RowKey == _dtlRowKey)
                    {
                        if (detail.DetailRecord.HasTransientProperty("PointsHistory"))
                        {
                            _txns = (IList<PointTransaction>)detail.DetailRecord.GetTransientProperty("PointsHistory");
                            //_txns = detail.PointTransactions;
                        }
                    }
                }
            } 
        }

        public override int GetNumberOfRows()
        {
            return _txns != null ? _txns.Count : 0;
        }

        public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
        {
            object val = null;
            PointTransaction pt = _txns[rowIndex];

			if (column.Name == "Id")
			{
				return pt.Id;
			}

			ConfigurationItem item = null;
			foreach (var i in Config.PointTxnFieldsToShow)
			{
				if (i.DataKey == column.Name)
				{
					item = i;
					break;
				}
			}

			if (item == null)
			{
				return null;
			}

            switch (item.AttributeName)
            {
                case "VcKey":
                    // get the loyalty id
                    Member member = PortalState.CurrentMember;
                    VirtualCard vc = member.GetLoyaltyCard(pt.VcKey);
                    val = vc.LoyaltyIdNumber;
                    break;
                case "Description":
                    PointEvent pe = LoyaltyService.GetPointEvent(pt.PointEventId);
                    val = pe.Name;
                    break;
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

        public override void SaveGridData(DynamicGridColumnSpec[] columns, AspDynamicGrid.GridAction gridAction)
        {
            throw new NotImplementedException();
        }
        #endregion        
    }
}
