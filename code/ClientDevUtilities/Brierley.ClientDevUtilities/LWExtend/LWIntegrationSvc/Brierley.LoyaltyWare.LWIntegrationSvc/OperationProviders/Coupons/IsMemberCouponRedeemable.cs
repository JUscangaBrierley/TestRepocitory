using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
	public class IsMemberCouponRedeemable : OperationProviderBase
	{
		public IsMemberCouponRedeemable() : base("IsMemberCouponRedeemable") { }

		public override string Invoke(string source, string parms)
		{
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to IsMemberCouponRedeemable.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

				long id = args.ContainsKey("MemberCouponId") ? (long)args["MemberCouponId"] : 0;
				if (id == 0)
				{
					throw new LWOperationInvocationException(string.Format("No coupon id provided.", id)) { ErrorCode = 3218 };
				}

				DateTime? redeemDate = args.ContainsKey("RedemptionDate") ? (DateTime?)args["RedemptionDate"] : null;
				int timesUsed = args.ContainsKey("TimesUsed") ? (int)args["TimesUsed"] : 1;

				MemberCoupon mc = LoyaltyDataService.GetMemberCoupon(id);
				if (mc == null)
				{
					throw new LWOperationInvocationException(string.Format("No coupon could be located using id {0}.", id)) { ErrorCode = 3370 };
				}

				var violations = new List<CouponUtil.RedemptionViolationReasons>();
				bool redeemable = CouponUtil.IsCouponRedeemable(mc, timesUsed, redeemDate, out violations);

				APIArguments responseArgs = new APIArguments();

				responseArgs.Add("Redeemable", redeemable);
				if (violations.Count > 0)
				{
					responseArgs.Add("Violations", violations.Select(o => o.Reason.ToString()).ToArray());
				}

				response = SerializationUtils.SerializeResult(Name, Config, responseArgs);

				Dictionary<string, object> context = new Dictionary<string, object>();
				PostProcessSuccessfullInvocation(context);

				return response;
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
	}
}
