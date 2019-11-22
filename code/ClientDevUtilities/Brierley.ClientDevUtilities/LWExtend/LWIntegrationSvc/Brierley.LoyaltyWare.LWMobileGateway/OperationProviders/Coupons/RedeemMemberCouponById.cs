using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Coupons
{
	public class RedeemMemberCouponById : OperationProviderBase
	{
		private const string _className = "RedeemMemberCouponById";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public RedeemMemberCouponById() : base("RedeemMemberCouponById") { }

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			string methodName = "Invoke";

			if (parms == null || parms.Length != 3)
			{
				string errMsg = "Invalid parameters provided for RedeemMemberCouponById.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			long couponId = -1;
			string couponIdStr = (string)parms[0];
			if (!string.IsNullOrEmpty(couponIdStr))
			{
				couponId = long.Parse(couponIdStr);
			}
			else
			{
				string errMsg = string.Format("No coupon id provided.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 3218 };
			}

			DateTime redeemDate = DateTime.Now;
			string redeemDateStr = (string)parms[1];
			if (!string.IsNullOrEmpty(redeemDateStr))
			{
				redeemDate = DateTime.Parse(redeemDateStr);
			}

			string timesUSedStr = (string)parms[2];
			int timesUsed = !string.IsNullOrEmpty(timesUSedStr) ? int.Parse(timesUSedStr) : 1;

			MemberCoupon mc = CouponUtil.RedeemCoupon(couponId, timesUsed, redeemDate);

			CouponDef coupon = ContentService.GetCouponDef(mc.CouponDefId);

			return coupon.UsesAllowed - mc.TimesUsed;
		}
	}
}