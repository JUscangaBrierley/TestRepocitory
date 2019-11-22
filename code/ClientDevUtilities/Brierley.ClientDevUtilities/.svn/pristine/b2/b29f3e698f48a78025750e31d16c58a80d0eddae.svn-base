using System;
using System.Collections.Generic;
using System.Diagnostics;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberCouponDao : DaoBase<MemberCoupon>
	{
		private const string _className = "MemberCouponDao";

		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public MemberCouponDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public long HowManyActiveCoupons(long memberId)
		{
			return Database.ExecuteScalar<long>("select count(*) from LW_MemberCoupon m, LW_CouponDef c where m.MemberId = @0 and m.CouponDefId = c.Id and m.ExpiryDate > @1 and m.TimesUsed < c.UsesAllowed", memberId, DateTime.Now);
		}

		public long HowManyCouponsByType(long memberId, long defId)
		{
			return Database.ExecuteScalar<long>("select count(*) from LW_MemberCoupon where MemberId = @0 and CouponDefId = @1", memberId, defId);
		}

		public MemberCoupon Retrieve(long couponId)
		{
			return GetEntity(couponId);
		}

		public IEnumerable<MemberCoupon> Retrieve(long memberId, long defId)
		{
			return Database.Fetch<MemberCoupon>("select * from LW_MemberCoupon where MemberId = @0 and CouponDefId = @1", memberId, defId);
		}

		public List<MemberCoupon> Retrieve(long[] ids)
		{
			if (ids == null || ids.Length == 0)
				return new List<MemberCoupon>();
            return RetrieveByArray<long>("select * from LW_MemberCoupon where id in (@0) order by DisplayOrder", ids);
		}

		public List<MemberCoupon> RetrieveAll()
		{
			return Database.Fetch<MemberCoupon>("select * from LW_MemberCoupon");
		}

		public List<long> RetrieveIds(
			long[] ipCodes,
			DateTime? from,
			DateTime? to,
			bool unExpiredOnly)
		{
			const string methodName = "RetrieveIds";

			var parameters = new List<object>();

			string sql = "select id from LW_MemberCoupon ";
			bool hasWhere = false;

			if (ipCodes != null && ipCodes.Length > 0)
			{
				sql += (hasWhere ? " and " : " where ") + "MemberId in (@ipcodes)";
				parameters.Add(new { ipcodes = ipCodes });
				hasWhere = true;
			}

			if (from != null)
			{
				sql += (hasWhere ? " and " : " where ") + "DateIssued >= @" + parameters.Count.ToString();
				parameters.Add(DateTimeUtil.GetBeginningOfDay((DateTime)from));
				hasWhere = true;
			}

			if (to != null)
			{
				sql += (hasWhere ? " and " : " where ") + "DateIssued < @" + parameters.Count.ToString();
				parameters.Add(DateTimeUtil.GetEndOfDay((DateTime)to));
				hasWhere = true;
			}

			if (unExpiredOnly)
			{
				sql += (hasWhere ? " and " : " where ") + "Expiration > @" + parameters.Count.ToString();
				parameters.Add(DateTime.Now);
			}

			sql += " order by DateIssued desc";
			_logger.Debug(_className, methodName, "Query = " + sql);

			Stopwatch timer = new Stopwatch();
			timer.Start();
			var ret = Database.Fetch<long>(sql, parameters.ToArray());
			timer.Stop();
			_logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", ret.Count, timer.ElapsedMilliseconds));
			return ret;
		}

		[Obsolete]
		public List<MemberCoupon> RetrieveByMember(long ipCode, LWQueryBatchInfo batchInfo, bool unRedeemedOnly, bool activeOnly, DateTime? from, DateTime? to)
		{
			var parameters = new List<object>() { ipCode };
			string sql = "select c.* from LW_MemberCoupon c where MemberId = @0";

			if (from != null)
			{
				sql += " and DateIssued >= @" + parameters.Count.ToString();
				parameters.Add(from.Value);
			}
			if (to != null)
			{
				sql += " and DateIssued < @" + parameters.Count.ToString();
				parameters.Add(to.Value);
			}
			if (activeOnly)
			{
				// get only un-expired
				sql += string.Format(" and StartDate <= @{0} and (ExpiryDate is null or ExpiryDate > @{0})", parameters.Count.ToString());
				parameters.Add(DateTime.Now);
			}
			if (unRedeemedOnly)
			{
				sql += " and DateRedeemed is null";
			}

			var args = parameters.ToArray();
			ApplyBatchInfo(batchInfo, ref sql, ref args);

			return Database.Fetch<MemberCoupon>(sql, args);
		}

		public PetaPoco.Page<MemberCoupon> Retrieve(long ipCode, ActiveCouponOptions options, long page, long resultsPerPage)
		{
			const string couponSelect = @"
				select 
						  coalesce(c.Id, 0) as Id, 
						  coalesce(c.CouponDefId, d.Id) as CouponDefId, 
						  c.CertificateNmbr, 
						  coalesce(c.MemberId, 0) as MemberId, 
						  coalesce(c.TimesUsed, 0) as TimesUsed, 
						  coalesce(c.DateIssued, sysdate) as DateIssued, 
						  c.DateRedeemed, 
						  coalesce(c.StartDate, d.StartDate) as StartDate, 
						  coalesce(c.ExpiryDate, d.ExpiryDate) as ExpiryDate, 
						  coalesce(c.DisplayOrder, d.DisplayOrder) as DisplayOrder, 
						  c.CampaignId, 
						  c.Status, 
						  c.MTouchId, 
						  coalesce(c.CreateDate, sysdate) as CreateDate, 
						  coalesce(c.UpdateDate, sysdate) as UpdateDate
				from 
						  LW_CouponDef d 
						  left join LW_MemberCoupon c on c.CouponDefId = d.Id
				where 
						  ";

			if (options == null)
			{
				options = ActiveCouponOptions.Default;
			}

			var parameters = new List<object>() { ipCode, options.RestrictDate.GetValueOrDefault() };

			string sql = couponSelect;

			switch (options.RestrictGlobalCoupons)
			{
				case GlobalCouponRestriction.None:
					// this is a little ridiculous. We're selecting a coupon definition disguised as a member coupon. There are no restrictions 
					// on global coupons, meaning we should return any globals. 
					sql += "c.MemberId = @0 or (c.MemberId is null and d.IsGlobal = 1)";
					break;
				case GlobalCouponRestriction.RestrictGlobal:
					// Keep global coupons that haven't been assigned to the member out of the list
					sql += "c.MemberId = @0";
					break;
				case GlobalCouponRestriction.RestrictNonGlobal:
					// the caller wants to show only global coupons as member coupons. Cats and dogs, this one. Swap the select from LW_MemberCoupon with the global select
					sql += "c.MemberId is null and d.IsGlobal = 1";
					break;
			}

			if (options.RestrictDate.HasValue)
			{
				sql += " and coalesce(c.StartDate, d.StartDate) <= @1 and(coalesce(c.ExpiryDate, d.ExpiryDate) is null or coalesce(c.ExpiryDate, d.ExpiryDate) > @1)";
			}

			return Database.Page<MemberCoupon>(page, resultsPerPage, sql, parameters.ToArray());
		}

		public List<MemberCoupon> RetrieveByMemberAndTypeCode(long memberId, string typeCode, LWQueryBatchInfo batchInfo)
		{
			object[] args = new object[] { memberId, typeCode };
			string sql = string.Format("select m.* from LW_MemberCoupon m, LW_CouponDef d where m.MemberId = @0 and m.CouponDefId = d.Id and d.CouponTypeCode = @1");
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<MemberCoupon>(sql, args);
		}

		public MemberCoupon RetrieveByCertNmbr(string certNmbr)
		{
			return Database.FirstOrDefault<MemberCoupon>("select * from LW_MemberCoupon where CertificateNmbr = @0", certNmbr);
		}

		public void Delete(long couponId)
		{
			DeleteEntity(couponId);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MemberCoupon where MemberId = @0", memberId);
		}

		public int DeleteByMembers(long[] memberIds)
		{
			int keysRemaining = memberIds.Length;
			int startIdx = 0;
			int ret = 0;
			using (var txn = Database.GetTransaction())
			{
				while (keysRemaining > 0)
				{
					long[] ids = LimitInClauseList<long>(memberIds, ref startIdx, ref keysRemaining);
					ret += Database.Execute("delete from MemberCoupon where MemberId in (@ids)", new { ids = ids });
				}
				txn.Complete();
			}
			return ret;
		}
	}
}
