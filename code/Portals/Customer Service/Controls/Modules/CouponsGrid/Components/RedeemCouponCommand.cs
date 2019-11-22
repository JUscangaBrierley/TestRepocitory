using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;
using Brierley.WebFrameWork.Controls.Grid;
using Brierley.FrameWork.Common;

namespace Brierley.LWModules.CouponsGrid.Components
{
    internal class RedeemCouponCommand : ICustomGridAction
    {
        #region Fields
        private string _name = "Redeem";
        private string _cssClass = "redeem";
        private string _ctrId = "redeem_coupon";
        private string _text;
        private AspGridProviderBase _gridProvider = null;
        #endregion

        internal RedeemCouponCommand(AspGridProviderBase gridProvider)
        {
            _gridProvider = gridProvider;
            _text = ResourceUtils.GetLocalWebResource(_gridProvider.ParentControl, "CommandRedeem.Text", "Redeem");
        }

        #region Properties
        public string CommandName
        {
            get { return _name; }
        }
        public string CssClass
        {
            get { return _cssClass; }
        }
        public string ControlId
        {
            get { return _ctrId; }
        }
        public string Text
        {
            get { return _text; }
        }
        #endregion

        #region Methods
        public void HandleCommand(System.Web.UI.Page page, object key)
        {
            //long id = long.Parse(key.ToString());
            //CouponUtil.RedeemCoupon(id, 1, null);
        }
        #endregion

    }
}