using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.LWIntegration
{
	public class CouponUtil
	{
		private const string _className = "CouponUtil";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public class RedemptionViolationReasons
		{
			public CouponRedemptionViolation Reason { get; set; }
			public int? MaxAllowed { get; set; }

			public RedemptionViolationReasons(CouponRedemptionViolation reason, long? maxAllowed = null)
			{
				Reason = reason;
				if (maxAllowed.HasValue)
				{
					//hack: total uses on a coupon is a long, but other intervals are using a more reasonable int32. In the unlikely 
					//event that a coupon may be used by an individual more than 2,147,483,647 times, we'll cast the number down to
					//int.MaxValue instead.
					if (maxAllowed.Value > int.MaxValue)
					{
						MaxAllowed = int.MaxValue;
					}
					else
					{
						MaxAllowed = Convert.ToInt32(maxAllowed.Value);
					}
				}
			}
		}

		public static MemberCoupon RedeemCoupon(long id, int timesUsed, DateTime? redeemDate, bool ignoreViolations = false)
		{
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				MemberCoupon mc = service.GetMemberCoupon(id);
				if (mc == null)
				{
					throw new LWException(string.Format("No coupon could be located using id {0}.", id)) { ErrorCode = 3370 };
				}
				return RedeemCoupon(mc, timesUsed, redeemDate, ignoreViolations);
			}
		}

		public static MemberCoupon RedeemCoupon(string certNmbr, int timesUsed, DateTime? redeemDate, bool ignoreViolations = false)
		{
			using (var service = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				MemberCoupon mc = service.GetMemberCouponByCertNmbr(certNmbr);
				if (mc == null)
				{
					throw new LWException(string.Format("No coupon could be located using cert number {0}.", certNmbr)) { ErrorCode = 3370 };
				}
				return RedeemCoupon(mc, timesUsed, redeemDate, ignoreViolations);
			}
		}

		public static MemberCoupon RedeemCoupon(MemberCoupon mc, int timesUsed, DateTime? redeemDate, bool ignoreViolations = false)
		{
			if (!redeemDate.HasValue)
			{
				redeemDate = DateTime.Now;
			}

			using (var contentService = LWDataServiceUtil.ContentServiceInstance())
			using (var loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				CouponDef coupon = contentService.GetCouponDef(mc.CouponDefId);

				//check whether or not this coupon can be redeemed. If the caller has chosen to ignore violations, the coupon will 
				//be redeemed no matter what, but the violation will be recorded and persisted with the redemption record.
				var violations = new List<RedemptionViolationReasons>();
				bool isRedeemable = IsCouponRedeemable(mc, coupon, timesUsed, redeemDate.Value, out violations);
				CouponRedemptionViolation violation = 0;
				foreach (var v in violations)
				{
					violation |= v.Reason;
				}

				if (!isRedeemable && !ignoreViolations)
				{
					//for CDIS & Mobile Gateway, we'll throw an exception on the first violation we encounter. 
					if (violation.HasFlag(CouponRedemptionViolation.TotalAllowedRedemptionsExceeded))
					{
						throw new LWException(string.Format("{0} Uses allowed for the coupon. Using it {1} times would exceed the allowed uses.", coupon.UsesAllowed, timesUsed)) { ErrorCode = 3228 };
					}

					//add additional violations here - these are new...


					if (violation.HasFlag(CouponRedemptionViolation.MemberCouponNotActive))
					{
						throw new LWException("Coupon cannot be redeemed because it is not active.") { ErrorCode = 3371 };
					}

					if (violation.HasFlag(CouponRedemptionViolation.CouponInKilledStatus))
					{
						throw new LWException("Coupon cannot be redeemed because it has been killed.") { ErrorCode = 4343 };
					}

					if (violation.HasFlag(CouponRedemptionViolation.MemberCouponExpired))
					{
						throw new LWException("Coupon cannot be redeemed because it has expired.") { ErrorCode = 4345 };
					}

					if (violation.HasFlag(CouponRedemptionViolation.RedemptionsPerDayExceeded))
					{
						throw new LWException("Coupon cannot be redeemed because it has reached the maximum number of redemptions per day.") { ErrorCode = 4346 };
					}

					if (violation.HasFlag(CouponRedemptionViolation.RedemptionsPerWeekExceeed))
					{
						throw new LWException("Coupon cannot be redeemed because it has reached the maximum number of redemptions per week.") { ErrorCode = 4347 };
					}

					if (violation.HasFlag(CouponRedemptionViolation.RedemptionsPerMonthExceeded))
					{
						throw new LWException("Coupon cannot be redeemed because it has reached the maximum number of redemptions per month.") { ErrorCode = 4348 };
					}

					if (violation.HasFlag(CouponRedemptionViolation.RedemptionsPerYearExceeded))
					{
						throw new LWException("Coupon cannot be redeemed because it has reached the maximum number of redemptions per year.") { ErrorCode = 4349 };
					}
				}

				using (var txn = loyaltyService.Database.GetTransaction())
				{
					for (int i = 1; i <= timesUsed; i++)
					{
						var redemption = new MemberCouponRedemption()
						{
							MemberCouponId = mc.ID,
							DateRedeemed = redeemDate.Value
						};

						//we've already checked to see if redeeming will cause any violations, but each individual redemption may 
						//trigger a unique violation (e.g., first redemption violates day rule, second violates day and week, third
						//violates day, week and total allowed). We want to store the actual violation that was triggered for the 
						//individual redemption.
						if (ignoreViolations && timesUsed > 1)
						{
							violations = new List<RedemptionViolationReasons>();
							isRedeemable = IsCouponRedeemable(mc, coupon, 1, redeemDate.Value, out violations);
							violation = 0;
							foreach (var v in violations)
							{
								violation |= v.Reason;
							}
						}
						if (violation != CouponRedemptionViolation.None)
						{
							redemption.Violation = violation;
						}

						loyaltyService.CreateMemberCouponRedemption(redemption);
					}

					mc.TimesUsed += timesUsed;
					if (mc.TimesUsed >= coupon.UsesAllowed)
					{
						mc.Status = CouponStatus.Redeemed;
					}
					mc.DateRedeemed = redeemDate.Value;
					loyaltyService.UpdateMemberCoupon(mc);

					txn.Complete();
				}
			}
			return mc;
		}

		public static MemberCoupon UnRedeemCoupon(MemberCoupon mc)
		{
			using (var contentService = LWDataServiceUtil.ContentServiceInstance())
			using (var loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				using (var txn = loyaltyService.Database.GetTransaction())
				{
					CouponDef coupon = contentService.GetCouponDef(mc.CouponDefId);

					if (mc.HasExpired())
					{
						throw new LWException(string.Format("Coupon with Id {0} is expired. It cannot be unredeemed.", mc.ID)) { ErrorCode = 3371 };
					}

					if (mc.Status == CouponStatus.Killed)
					{
						throw new LWException(string.Format("Coupon with Id {0} has been killed. It cannot be unredeemed.", mc.ID)) { ErrorCode = 3371 };
					}

					if (mc.TimesUsed <= 0)
					{
						throw new LWException(string.Format("Coupon with Id {0} has not yet been used. It cannot be unredeemed.", mc.ID)) { ErrorCode = 3371 };
					}

					var redemptions = loyaltyService.GetCouponRedemptions(mc.ID).Where(o => !o.DateUnredeemed.HasValue);
					if (redemptions.Count() == 0)
					{
						throw new LWException(string.Format("Coupon with Id {0} has not yet been used. It cannot be unredeemed.", mc.ID)) { ErrorCode = 3371 };
					}

					var lastRedemption = redemptions.OrderBy(o => o.DateRedeemed).Last();
					lastRedemption.DateUnredeemed = DateTime.Now;
					loyaltyService.UpdateMemberCouponRedemption(lastRedemption);

					if (mc.TimesUsed > 0)
					{
						mc.TimesUsed--;
					}
					mc.DateRedeemed = null;
					mc.Status = CouponStatus.Active;

					loyaltyService.UpdateMemberCoupon(mc);

					txn.Complete();
				}
				return mc;
			}
		}

		public static bool IsCouponRedeemable(MemberCoupon memberCoupon, int timesUsed, DateTime? redeemDate, out List<RedemptionViolationReasons> violations)
		{
			return IsCouponRedeemable(memberCoupon, timesUsed, redeemDate.GetValueOrDefault(DateTime.Now), out violations);
		}

		public static bool IsCouponRedeemable(MemberCoupon memberCoupon, int timesUsed, DateTime redeemDate, out List<RedemptionViolationReasons> violations)
		{
			CouponDef coupon = null;
			using (var contentService = LWDataServiceUtil.ContentServiceInstance())
			{
				coupon = contentService.GetCouponDef(memberCoupon.CouponDefId);
				if (coupon == null)
				{
					throw new Exception(string.Format("Coupon with id {0} does not exist.", memberCoupon.CouponDefId));
				}
			}
			return IsCouponRedeemable(memberCoupon, coupon, timesUsed, redeemDate, out violations);
		}

		public static bool IsCouponRedeemable(MemberCoupon memberCoupon, CouponDef coupon, int timesUsed, DateTime redeemDate, out List<RedemptionViolationReasons> violations)
		{
			if (memberCoupon == null)
			{
				throw new ArgumentNullException("memberCoupon");
			}

			if (coupon == null)
			{
				throw new ArgumentNullException("coupon");
			}

			if (timesUsed < 1)
			{
				throw new ArgumentOutOfRangeException("timesUsed", "timesUsed must be greater than zero");
			}

			violations = new List<RedemptionViolationReasons>();

			if (memberCoupon.StartDate > redeemDate)
			{
				violations.Add(new RedemptionViolationReasons(CouponRedemptionViolation.MemberCouponNotActive));
			}

			if (memberCoupon.ExpiryDate.HasValue && memberCoupon.ExpiryDate.Value <= redeemDate)
			{
				violations.Add(new RedemptionViolationReasons(CouponRedemptionViolation.MemberCouponExpired));
			}

			if (memberCoupon.Status == CouponStatus.Killed)
			{
				violations.Add(new RedemptionViolationReasons(CouponRedemptionViolation.CouponInKilledStatus));
			}

			//for backward compatibility, we have to honor both the times used and the history when times used is different from the redemption 
			//history count. This only counts toward the total uses allowed, since we cannot match each usage to a date.
			if (coupon.UsesAllowed > 0 && memberCoupon.TimesUsed + timesUsed > coupon.UsesAllowed)
			{
				violations.Add(new RedemptionViolationReasons(CouponRedemptionViolation.TotalAllowedRedemptionsExceeded, coupon.UsesAllowed));
			}

			int usesPerDay = coupon.UsesPerDay.GetValueOrDefault();
			int usesPerWeek = coupon.UsesPerWeek.GetValueOrDefault();
			int usesPerMonth = coupon.UsesPerMonth.GetValueOrDefault();
			int usesPerYear = coupon.UsesPerYear.GetValueOrDefault();

			if (
				usesPerDay > 0 ||
				usesPerWeek > 0 ||
				usesPerMonth > 0 ||
				usesPerYear > 0)
			{
				using (var loyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance())
				{
					var history = loyaltyService.GetCouponRedemptions(memberCoupon.ID).Where(o => o.DateUnredeemed == null);

					if (coupon.UsesAllowed > 0 && violations.Count(o => o.Reason == CouponRedemptionViolation.TotalAllowedRedemptionsExceeded) == 0 && history.Count(o => o.DateUnredeemed == null) + timesUsed > coupon.UsesAllowed)
					{
						violations.Add(new RedemptionViolationReasons(CouponRedemptionViolation.TotalAllowedRedemptionsExceeded, coupon.UsesAllowed));
					}

					if (usesPerDay > 0 && history.Count(o => o.DateRedeemed.Date == redeemDate.Date) + timesUsed > usesPerDay)
					{
						violations.Add(new RedemptionViolationReasons(CouponRedemptionViolation.RedemptionsPerDayExceeded, coupon.UsesPerDay));
					}

					if (usesPerWeek > 0 && history.Count(o => o.DateRedeemed.Date >= redeemDate.AddDays((int)redeemDate.DayOfWeek * -1) && o.DateRedeemed < redeemDate.AddDays(7 - (int)redeemDate.DayOfWeek)) + timesUsed > usesPerWeek)
					{
						violations.Add(new RedemptionViolationReasons(CouponRedemptionViolation.RedemptionsPerWeekExceeed, coupon.UsesPerWeek));
					}

					if (usesPerMonth > 0 && history.Count(o => o.DateRedeemed.Month == redeemDate.Month && o.DateRedeemed.Year == redeemDate.Year) + timesUsed > usesPerMonth)
					{
						violations.Add(new RedemptionViolationReasons(CouponRedemptionViolation.RedemptionsPerMonthExceeded, coupon.UsesPerMonth));
					}

					if (usesPerYear > 0 && history.Count(o => o.DateRedeemed.Year == redeemDate.Year) + timesUsed > usesPerYear)
					{
						violations.Add(new RedemptionViolationReasons(CouponRedemptionViolation.RedemptionsPerYearExceeded, coupon.UsesPerYear));
					}
				}
			}
			return violations.Count == 0;
		}
	}
}
