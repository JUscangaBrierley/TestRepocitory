//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//using Brierley.FrameWork.Data;
//using Brierley.FrameWork.Data.DomainModel;

//using Brierley.FrameWork.LWIntegration;

//using Brierley.WebFrameWork.Controls.List;

//namespace Brierley.LWModules.Coupons.Components.List
//{
//	internal class RedeemCouponCommand : ICustomListAction
//	{
//		#region Fields
//		private string _name = "Redeem";
//		private string _cssClass = "redeem";
//		private string _ctrId = "redeem_coupon";
//		private AspListProviderBase _listProvider = null;
//		#endregion

//		internal RedeemCouponCommand(AspListProviderBase listProvider)
//		{
//			_listProvider = listProvider;
//		}

//		#region Properties
//		public string CommandName
//		{
//			get { return _name; }
//		}
//		public string CssClass
//		{
//			get { return _cssClass; }
//		}
//		public string ControlId
//		{
//			get { return _ctrId; }
//		}
//		public string Text
//		{
//			get { return _name; }
//		}
//		#endregion

//		#region Methods
//		public void HandleCommand(System.Web.UI.Page page, object key)
//		{
//			//long id = long.Parse(key.ToString());
//			//CouponUtil.RedeemCoupon(id, 1, null);
//		}
//		#endregion

//	}
//}