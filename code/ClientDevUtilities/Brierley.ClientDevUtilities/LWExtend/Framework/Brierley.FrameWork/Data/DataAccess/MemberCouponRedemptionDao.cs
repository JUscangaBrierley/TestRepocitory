using System;
using System.Collections.Generic;
using System.Diagnostics;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberCouponRedemptionDao : DaoBase<MemberCouponRedemption>
	{
		public MemberCouponRedemptionDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public MemberCouponRedemption Unredeem(long memberCouponId)
		{
			var redemption = Database.FirstOrDefault<MemberCouponRedemption>("select * from LW_MemberCouponRedemption where MemberCouponId = @0 and DateUnredeemed is null", memberCouponId);
			if (redemption != null)
			{
				redemption.DateUnredeemed = DateTime.Now;
			}
			return redemption;
		}

		public List<MemberCouponRedemption> Retrieve(long memberCouponId)
		{
			return Database.Fetch<MemberCouponRedemption>("select * from LW_MemberCouponRedemption where MemberCouponId = @0", memberCouponId);
		}

		public MemberCouponRedemption RetrieveLastRedemption(long memberCouponId)
		{
			return Database.FirstOrDefault<MemberCouponRedemption>("select * from LW_MemberCouponRedemption where MemberCouponId = @0 and DateUnredeemed is null order by RedemptionDate desc", memberCouponId);
		}
	}
}
