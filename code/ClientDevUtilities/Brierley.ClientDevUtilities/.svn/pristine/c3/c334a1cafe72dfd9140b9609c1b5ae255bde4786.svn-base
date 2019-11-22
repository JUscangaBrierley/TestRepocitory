using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberBonusDao : DaoBase<MemberBonus>
	{
		public MemberBonusDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public int ExpireUnexpiredUncompletedBonus(long bonusId, DateTime newDate)
		{
			return Database.Execute("update LW_MemberBonus set ExpiryDate = @0 where m.BonusDefId = @1 and m.Status <> 'Completed' and (m.ExpiryDate = null or m.ExpiryDate > @2)", newDate, bonusId, DateTime.Now);
		}

		public int ExtendExpiredUncompletedBonus(long bonusId, DateTime newDate)
		{
			return Database.Execute("update LW_MemberBonus set ExpiryDate = @0 where m.BonusDefId = @1 and m.Status <> 'Completed' and (m.ExpiryDate = <= @2)", newDate, bonusId, DateTime.Now);
		}

		public long HowManyActiveBonuses(long memberId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberBonus m inner join LW_BonusDef d on m.BonusDefId = d.Id where m.MemberId = @0 and m.status <> 'Completed'", memberId);
		}

		public long HowManyCompletedByType(long defId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberBonus where BonusDefId = @0 and Status = 'Completed'", defId);
		}

		public long HowManyCompletedByType(long memberId, long defId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberBonus where MemberId = @0 and BonusDefId = @1 and Status = 'Completed'", memberId, defId);
		}

		public long HowManyReferralsCompleted(long defId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberBonus where BonusDefId = @0 and ReferralCompleted = 1", defId);
		}

		public long HowManyBonusesByType(long memberId, long defId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberBonus where MemberId = @0 and BonusDefId = @1", memberId, defId);
		}

		public long HowManyUnexpiredAndUncompletedBonuses(long defId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberBonus where BonusDefId = @0 and Status <> 'Completed' and (ExpiryDate = null or ExpiryDate > @1)", defId, DateTime.Now);
		}

		public long HowManyExpiredAndUncompletedBonuses(long defId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberBonus where BonusDefId = @0 and Status <> 'Completed' and (ExpiryDate <= @1)", defId, DateTime.Now);
		}

		public MemberBonus Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<MemberBonus> RetrieveByMember(long ipCode, LWQueryBatchInfo batchInfo)
		{
			return RetrieveByMember(ipCode, new List<MemberBonusStatus> { MemberBonusStatus.Issued, MemberBonusStatus.Viewed }, false, null, batchInfo);
		}

		public List<MemberBonus> RetrieveByMember(long ipCode, IEnumerable<MemberBonusStatus> statuses, bool activeOnly, long? definitionId, LWQueryBatchInfo batchInfo)
        {
            if (statuses == null || statuses.Count() == 0)
            {
                statuses = new List<MemberBonusStatus>() { MemberBonusStatus.Issued, MemberBonusStatus.Viewed };
            }

            var parameters = new List<object> { ipCode, DateTime.Now, new { statuses = statuses.Select(o => o.ToString()) } };
			string sql = "select m.* from LW_MemberBonus m where MemberId = @0 ";
			if (activeOnly)
			{
                sql += " and StartDate <= @1 and(ExpiryDate is null or ExpiryDate > @1)";
			}

            if (statuses != null && statuses.Count() > 0)
            {
                sql += " and Status in(@statuses)";
            }
			if (definitionId.HasValue && definitionId.Value > 0)
			{
				parameters.Add(definitionId.Value);
				sql += " and BonusDefId = @2";
			}
			sql += "order by DisplayOrder";
			var args = parameters.ToArray();
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<MemberBonus>(sql, args);
		}

		public List<MemberBonus> Retrieve(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<MemberBonus>();
            return RetrieveByArray<long>("select * from LW_MemberBonus where Id in (@0) order by DisplayOrder", ids);
		}

		public List<MemberBonus> RetrieveAll()
		{
			return Database.Fetch<MemberBonus>("select * from LW_MemberBonus");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MemberBonus where MemberId = @0", memberId);
		}

		public int DeleteByMembers(long[] memberIds)
		{
			int keysRemaining = memberIds.Length;
			int startIdx = 0;
			int nRows = 0;
			while (keysRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(memberIds, ref startIdx, ref keysRemaining);
				nRows += Database.Execute("delete from LW_MemberBonus where MemberId in (@ids)", new { ids = ids });
			}
			return nRows;
		}
	}
}
