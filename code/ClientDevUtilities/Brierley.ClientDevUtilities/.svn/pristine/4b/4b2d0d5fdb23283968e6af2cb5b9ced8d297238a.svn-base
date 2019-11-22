using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
	public class AddAndRedeemMemberCoupon : OperationProviderBase
	{
		public AddAndRedeemMemberCoupon() : base("AddAndRedeemMemberCoupon") { }

		public override string Invoke(string source, string parms)
		{
			if (string.IsNullOrEmpty(parms))
			{
				throw new LWOperationInvocationException("No parameters provided to AddAndRedeemMemberCoupon.", 3300);
			}

			try
			{
				long couponDefinitionId = 0;
				DateTime dateIssued = DateTime.Now;
				DateTime? startDate = null;
				DateTime? expiryDate = null;
				int? displayOrder = null;
				DateTime? redemptionDate = null;
				bool ignoreViolations = false;

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

				if (args.ContainsKey("CouponDefinitionId"))
				{
					couponDefinitionId = (long)args["CouponDefinitionId"];
				}

				if (couponDefinitionId < 1)
				{
					throw new LWOperationInvocationException("No coupon definition id provided.", 3383);
				}

				if (args.ContainsKey("DateIssued"))
				{
					dateIssued = (DateTime)args["DateIssued"];
				}

				if (args.ContainsKey("StartDate"))
				{
					startDate = (DateTime)args["StartDate"];
				}

				if (args.ContainsKey("ExpiryDate"))
				{
					expiryDate = (DateTime)args["ExpiryDate"];
				}

				if (args.ContainsKey("DisplayOrder"))
				{
					displayOrder = (int)args["DisplayOrder"];
				}

				if (args.ContainsKey("RedemptionDate"))
				{
					redemptionDate = (DateTime)args["RedemptionDate"];
				}

				if (args.ContainsKey("IgnoreViolations"))
				{
					ignoreViolations = (bool)args["IgnoreViolations"];
				}

				int timesUsed = args.ContainsKey("TimesUsed") ? (int)args["TimesUsed"] : 1;

				Member member = LoadMember(args);

				var coupon = ContentService.GetCouponDef(couponDefinitionId);
				if (coupon == null)
				{
					throw new LWOperationInvocationException("No coupon definition found with " + couponDefinitionId + ".", 3369);
				}

				MemberCoupon c = new MemberCoupon()
				{
					CouponDefId = couponDefinitionId,
					DateIssued = dateIssued,
					DisplayOrder = displayOrder,
					ExpiryDate = expiryDate ?? coupon.ExpiryDate,
					MemberId = member.IpCode,
					StartDate = startDate ?? coupon.StartDate
				};

				LoyaltyDataService.CreateMemberCoupon(c);

				CouponUtil.RedeemCoupon(c.ID, timesUsed, redemptionDate ?? DateTime.Now, ignoreViolations);

				APIArguments responseArgs = new APIArguments();
				responseArgs.Add("MemberCouponId", c.ID);

				string response = SerializationUtils.SerializeResult(Name, Config, responseArgs);
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
