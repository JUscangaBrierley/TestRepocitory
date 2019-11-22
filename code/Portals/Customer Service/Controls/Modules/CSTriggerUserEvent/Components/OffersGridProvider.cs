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
using Brierley.WebFrameWork.Portal;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.WebFrameWork.Portal.Configuration.Modules;

namespace Brierley.LWModules.CSTriggerUserEvent.Components
{
	public class OffersGridProvider : AspGridProviderBase
	{
		private static string _className = "OffersGridProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_PORTALMODULES);				        
		private CSTriggerUserEventConfiguration _config = null;
        private bool _ruleResultRequired = false;
        private long _eventKey;
        private IList<OfferRow> _rows = new List<OfferRow>();

        #region Helpers
        class OfferRow
        {
            public string RowKey { get; set; }
            public string OfferType { get; set; }
            public string Name { get; set; }
        }
        #endregion

        public OffersGridProvider(long keyVal)
		{
            _eventKey = keyVal;
		}

		public long ParentKey
		{
            set { _eventKey = value; }
		}

		#region Grid properties

		public override string Id
		{
			get { return "grdEventOffers"; }
		}

        protected override string GetGridName()
        {
            return "EventOffers";
        }

		public override DynamicGridColumnSpec[] GetColumnSpecs()
		{
			IList<DynamicGridColumnSpec> colList = new List<DynamicGridColumnSpec>();
			DynamicGridColumnSpec c = new DynamicGridColumnSpec();
			c.Name = "RowKey";
			c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-RowKey.Text", "RowKey");
			c.DataType = typeof(string);
			c.IsKey = true;
			c.IsEditable = false;
			c.IsVisible = false;
			c.IsSortable = false;
			colList.Add(c);

            c = new DynamicGridColumnSpec();
            c.Name = "OfferType";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-OfferType.Text", "Offer Type");
            c.DataType = typeof(string);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            colList.Add(c);

            c = new DynamicGridColumnSpec();
            c.Name = "Name";
            c.DisplayText = ResourceUtils.GetLocalWebResource(ParentControl, Id + "-OfferName.Text", "Name");
            c.DataType = typeof(string);
            c.IsKey = true;
            c.IsEditable = false;
            c.IsVisible = true;
            c.IsSortable = false;
            colList.Add(c);

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
		public override void SetSearchParm(string parmName, object parmValue)
		{
			if (parmName == "Config")
			{
                _config = (CSTriggerUserEventConfiguration)parmValue;
			}
		}

        public override void LoadGridData()
        {
            _rows.Clear();
            TriggerUserEventLog uevent = LoyaltyService.GetTriggerUserEventLog(_eventKey);
            long[] messageIds;
            long[] couponIds;
            long[] bonusIds;
            long[] promotionIds;

            int offersCount = TriggerUserEventUtil.DeserializeResult(uevent.Result, out messageIds, out couponIds, out bonusIds, out promotionIds);
            if (messageIds != null && messageIds.Length > 0)
            {
                IList<MemberMessage> messages = LoyaltyService.GetMemberMessages(messageIds);
                foreach (MemberMessage message in messages)
                {
                    MessageDef def = ContentService.GetMessageDef(message.MessageDefId);
                    OfferRow row = new OfferRow() { RowKey = Guid.NewGuid().ToString(), OfferType = "Message", Name = def.Name };
                    _rows.Add(row);
                }
            }

            if (couponIds != null && couponIds.Length > 0)
            {
                IList<MemberCoupon> coupons = LoyaltyService.GetMemberCoupons(couponIds);
                foreach (MemberCoupon coupon in coupons)
                {
                    CouponDef def = ContentService.GetCouponDef(coupon.CouponDefId);
                    OfferRow row = new OfferRow() { RowKey = Guid.NewGuid().ToString(), OfferType = "Coupon", Name = def.Name };
                    _rows.Add(row);
                }
            }

            if (bonusIds != null && bonusIds.Length > 0)
            {
                IList<MemberBonus> bonuses = LoyaltyService.GetMemberBonuses(bonusIds);
                foreach (MemberBonus bonus in bonuses)
                {
                    BonusDef def = ContentService.GetBonusDef(bonus.BonusDefId);
                    OfferRow row = new OfferRow() { RowKey = Guid.NewGuid().ToString(), OfferType = "Bonus", Name = def.Name };
                    _rows.Add(row);
                }
            }

            if (promotionIds != null && promotionIds.Length > 0)
            {
                IList<MemberPromotion> promotions = LoyaltyService.GetMemberPromotions(promotionIds);
                foreach (MemberPromotion promotion in promotions)
                {
                    Promotion def = ContentService.GetPromotionByCode(promotion.Code);
                    OfferRow row = new OfferRow() { RowKey = Guid.NewGuid().ToString(), OfferType = "Promotion", Name = def.Name };
                    _rows.Add(row);
                }
            }
        }

		public override int GetNumberOfRows()
		{
            return _rows != null ? _rows.Count : 0;            
		}

		public override object GetColumnData(int rowIndex, DynamicGridColumnSpec column)
		{
            OfferRow row = _rows[rowIndex];
            return GetColumnData(row, column);                       
		}
        		
		private object GetColumnData(OfferRow row, DynamicGridColumnSpec column)
		{
            object val = null;

            if (column.Name == "RowKey")
            {
                val = row.RowKey;
            }
            else if (column.Name == "OfferType")
            {
                val = row.OfferType;
            }
            else if (column.Name == "Name")
            {
                val = row.Name;
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
