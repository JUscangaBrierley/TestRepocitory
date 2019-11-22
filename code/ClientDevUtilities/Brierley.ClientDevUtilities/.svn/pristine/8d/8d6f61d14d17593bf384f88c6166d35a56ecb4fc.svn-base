using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Passbook
{
	public class GetCouponPass : OperationProviderBase
	{
		private const string _className = "GetCouponPass";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public GetCouponPass() : base("GetCouponPass") { }

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			string methodName = "Invoke";

			if (parms == null || parms.Length == 0)
			{
				string errMsg = "No coupon id provided to lookup.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			long couponId = Convert.ToInt64(parms[0]);
			Member m = token.CachedMember;
			MemberCoupon mc = LoyaltyService.GetMemberCoupon(couponId);
			if (mc != null)
			{
				return MGPassbookUtil.CreateCouponPass(mc, LWDataServiceUtil.GetServiceConfiguration());
			}
			else
			{
				string errMsg = string.Format("Member with ipcode {0} does not have a coupon with id {1}.", m.IpCode, couponId);
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}
		}
	}
}