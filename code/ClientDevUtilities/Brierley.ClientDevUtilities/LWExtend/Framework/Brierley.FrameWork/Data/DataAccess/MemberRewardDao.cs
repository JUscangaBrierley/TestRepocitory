using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberRewardDao : DaoBase<MemberReward>
	{
		private const string _className = "MemberRewardDao";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public RewardDao RewardDao { get; set; }

		public MemberRewardDao(Database database, ServiceConfig config, RewardDao rewardDao)
			: base(database, config)
		{
			if (rewardDao == null)
			{
				throw new ArgumentNullException("rewardDao");
			}
			RewardDao = rewardDao;
		}

		public long HowMany(long ipCode, DateTime from, DateTime to)
		{
			var args = new object[] { ipCode, DateTimeUtil.GetBeginningOfDay(from), DateTimeUtil.GetEndOfDay(to) };
			string sql = "select count(*) from LW_MemberRewards m where m.MemberId = @0 and m.DateIssued between @1 and @2";
			return Database.ExecuteScalar<long>(sql, args);
		}

		public MemberReward Retrieve(long rewardId)
		{
			MemberReward ret = GetEntity(rewardId);
			Populate(ret);
			return ret;
		}

		public List<MemberReward> Retrieve(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<MemberReward>();
            return RetrieveByArray<long>("select * from LW_MemberRewards where id in (@0)", ids);
		}

		public List<long> RetrieveIds(
			long[] ipCodes,
			long? categoryId,
			DateTime? from,
			DateTime? to,
			DateTime? changedSince,
			bool unRedeemedOnly,
			bool unExpiredOnly)
		{
			const string methodName = "RetrieveIds";

			var parameters = new List<object>();
            StringBuilder sql = new StringBuilder("select m.Id from LW_MemberRewards m");
			if (categoryId != null)
			{
				sql.Append(", LW_Product p where m.ProductId = p.Id");
			}
            else
            {
                sql.Append(" where 1=1");
            }

			if (ipCodes != null && ipCodes.Length > 0)
			{
                sql.Append(" and MemberId in (@ipcodes)");
				parameters.Add(new { ipcodes = ipCodes });
			}

			if (categoryId != null)
			{
                sql.AppendFormat(" and p.CategoryId = @{0}", parameters.Count.ToString());
				parameters.Add(categoryId);
			}

			if (from != null)
			{
                sql.AppendFormat(" and m.DateIssued >= @{0}", parameters.Count.ToString());
				parameters.Add(DateTimeUtil.GetBeginningOfDay((DateTime)from));
			}

			if (to != null)
			{
                sql.AppendFormat(" and m.DateIssued < @{0}", parameters.Count.ToString());
				parameters.Add(DateTimeUtil.GetEndOfDay((DateTime)to));
			}

			if (unRedeemedOnly)
			{
				sql.Append(" and m.RedemptionDate is null");
			}

			if (unExpiredOnly)
			{
                sql.AppendFormat(" and m.Expiration > @{0}", parameters.Count.ToString());
				parameters.Add(DateTime.Now);
			}

			if (changedSince != null)
			{
                sql.AppendFormat(" and m.UpdateDate >= @{0}", parameters.Count.ToString());
				parameters.Add(DateTimeUtil.GetBeginningOfDay((DateTime)changedSince));
			}

            sql.Append(" order by m.DateIssued desc, m.CreateDate desc");

			_logger.Debug(_className, methodName, "Query = " + sql);

			Stopwatch timer = new Stopwatch();
			timer.Start();
            var ret = Database.Fetch<long>(sql.ToString(), parameters.ToArray());
			timer.Stop();
			long count = ret != null ? ret.Count : 0;
			_logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", count, timer.ElapsedMilliseconds));
			return ret;
		}

        public List<long> RetrieveIds(
            long[] ipCodes,
            long[] rewardDefIds,
            long[] excludeRewardDefIds,
            DateTime? from,
            DateTime? to,
            DateTime? redeemedFrom,
            DateTime? redeemedTo,
            bool? redeemed,
            bool? expired)
        {
            const string methodName = "RetrieveIds";

            var parameters = new List<object>();
            StringBuilder sql = new StringBuilder("select m.Id from LW_MemberRewards m where 1=1");

            if (ipCodes != null && ipCodes.Length > 0)
            {
                sql.Append(" and MemberId in (@ipcodes)");
                parameters.Add(new { ipcodes = ipCodes });
            }

            if (rewardDefIds != null && rewardDefIds.Length > 0)
            {
                sql.AppendFormat(" and m.rewarddefid in (@rewardDefIds)", parameters.Count);
                parameters.Add(new { rewardDefIds = rewardDefIds });
            }

            if (excludeRewardDefIds != null && excludeRewardDefIds.Length > 0)
            {
                sql.AppendFormat(" and m.rewarddefid not in (@excludeRewardDefIds)", parameters.Count);
                parameters.Add(new { excludeRewardDefIds = excludeRewardDefIds });
            }

            if (from != null)
            {
                sql.AppendFormat(" and m.DateIssued >= @{0}", parameters.Count.ToString());
                parameters.Add(DateTimeUtil.GetBeginningOfDay((DateTime)from));
            }

            if (to != null)
            {
                sql.AppendFormat(" and m.DateIssued < @{0}", parameters.Count.ToString());
                parameters.Add(DateTimeUtil.GetEndOfDay((DateTime)to));
            }

            if (redeemedFrom != null)
            {
                sql.AppendFormat(" and m.RedemptionDate >= @{0}", parameters.Count.ToString());
                parameters.Add(DateTimeUtil.GetBeginningOfDay((DateTime)redeemedFrom));
            }

            if (redeemedTo != null)
            {
                sql.AppendFormat(" and m.RedemptionDate < @{0}", parameters.Count.ToString());
                parameters.Add(DateTimeUtil.GetEndOfDay((DateTime)redeemedTo));
            }

            if (redeemed != null)
            {
                if(redeemed.Value)
                    sql.AppendFormat(" and m.RedemptionDate is not null");
                else
                    sql.AppendFormat(" and m.RedemptionDate is null");
            }

            if (expired != null)
            {
                if (expired.Value)
                    sql.Append(" and m.Expiration <= sysdate ");
                else
                    sql.Append(" and m.Expiration > sysdate");
            }

            sql.Append(" order by m.DateIssued desc, m.CreateDate desc");

            _logger.Debug(_className, methodName, "Query = " + sql);

            Stopwatch timer = new Stopwatch();
            timer.Start();
            var ret = Database.Fetch<long>(sql.ToString(), parameters.ToArray());
            timer.Stop();
            long count = ret != null ? ret.Count : 0;
            _logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", count, timer.ElapsedMilliseconds));
            return ret;
        }


        public List<MemberReward> RetrieveByMember(long ipCode, LWQueryBatchInfo batchInfo)
		{
			var args = new object[]{ ipCode };
			string sql = "select r.* from LW_MemberRewards r where MemberId = @0 order by DateIssued desc";
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<MemberReward>(sql, args);
		}

		public MemberReward RetrieveByCert(string certNmbr)
		{
			return Database.FirstOrDefault<MemberReward>("select * from LW_MemberRewards where certificateNmbr = @0 order by DateIssued desc", certNmbr);
		}

		public List<MemberReward> RetrieveByOrderNumber(string orderNumber)
		{
			return Database.Fetch<MemberReward>("select * from LW_MemberRewards where LWOrderNumber = @0 order by DateIssued desc", orderNumber);
		}

		public List<MemberReward> RetrieveByFPOrderNumber(string orderNumber)
		{
			return Database.Fetch<MemberReward>("select * from LW_MemberRewards where FPOrderNumber = @0 order by DateIssued desc", orderNumber);
		}

		public List<MemberReward> RetrieveUnexpiredAndUnredeemedByMember(long ipCode)
		{
			return Database.Fetch<MemberReward>("select * from LW_MemberRewards where MemberId = @0 and RedemptionDate is null and Expiration > @1 order by DateIssued desc", ipCode, DateTime.Now);
		}

		public List<MemberReward> RetrieveByDefId(long ipCode, long rewardDefId)
		{
			return Database.Fetch<MemberReward>("select * from LW_MemberRewards where MemberId = @0 and RewardDefId = @1 order by DateIssued desc", ipCode, rewardDefId);
		}

		public List<MemberReward> RetrieveAll(LWQueryBatchInfo batchInfo)
		{
			var args = new object[0];
			string sql = "select r.* from LW_MemberRewards r";
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<MemberReward>(sql, args);
		}

		public void Delete(long rewardId)
		{
			DeleteEntity(rewardId);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MemberRewards where MemberId = @0", memberId);
		}

		public int DeleteByMembers(long[] memberIds)
		{
			int keysRemaining = memberIds.Length;
			int startIdx = 0;
			int ret = 0;
			while (keysRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(memberIds, ref startIdx, ref keysRemaining);
				ret += Database.Execute("delete from LW_MemberRewards where MemberId in (@ids)", new { ids = ids });
			}
			return ret;
		}

		private void Populate(MemberReward reward)
		{
			if (reward != null)
			{
				reward.RewardDef = RewardDao.Retrieve(reward.RewardDefId);
			}
		}
	}
}
