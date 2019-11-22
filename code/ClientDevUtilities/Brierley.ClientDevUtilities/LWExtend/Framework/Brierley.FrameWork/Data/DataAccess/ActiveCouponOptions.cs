using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DataAccess
{
	/// <summary>
	/// Used to relay rules to the coupon DAO classes for determining whether or not a coupon is considered active.
	/// </summary>
	public class ActiveCouponOptions
	{
		// LW-2526 coupons valid on certain days and times
		// when specified, the coupon definitions are restricted by StartDate and ExpiryDate. If the specified date is outside of the 
		// coupon's start and expiry date, then the coupon is not returned in the result. Otherwise, the start and expiry date are ignored.
		public DateTime? RestrictDate { get; set; }

		// LW-2666 - As a Marketer, I want to limit the overall number of times a coupon can be used
		public bool RestrictTotalRedemptions { get; set; }

		// LW-2868 restrict on monetary budget
		public bool RestrictBudget { get; set; }

		// LW-2878 restrict "coming soon" coupons
		// when true, coupons with a VisibleDate less than or equal to the specified date (or current system date, if none specified) will be returned
		// in the result. Coupons with a VisibleDate greater than the specified date are not returned.
		// When false, VisibleDate is ignored.
		public bool RestrictVisibleDate { get; set; }

		// LW-2874 global coupons
		// when true, global coupon definitions are not returned.
		// when false, globals coupons are returned. If a member is supplied with the method call, then the global coupons will be evaluated against the member to ensure
		// all other redemption rules/restrictions are met before considering the coupon to be active
		public GlobalCouponRestriction RestrictGlobalCoupons { get; set; }

		// LW-2895 coupon considered inactive if the member has used up all available redemptions (total allowed, intervals not considered)
		public bool RestrictTotalMemberRedemptions { get; set; }

		// LW-2867 coupon considered inactive if the member has used up available redemptions for a specific time interval
		public bool RestrictIntervalRedemptions { get; set; }

		public ActiveCouponOptions()
		{
			RestrictGlobalCoupons = GlobalCouponRestriction.None;
			
			//todo: null or current system date? Leaving null for now, so no restrictions by default; consistent with other properties
			//RestrictDates = DateTime.Now;
		}

		public static ActiveCouponOptions Default
		{
			get
			{
				return new ActiveCouponOptions();
			}
		}

		public static ActiveCouponOptions RestrictGlobal
		{
			get
			{
				return new ActiveCouponOptions() { RestrictGlobalCoupons = GlobalCouponRestriction.RestrictGlobal };
			}
		}

		public static ActiveCouponOptions OnlyGlobal
		{
			get
			{
				return new ActiveCouponOptions() { RestrictGlobalCoupons = GlobalCouponRestriction.RestrictNonGlobal };
			}
		}
	}
}
