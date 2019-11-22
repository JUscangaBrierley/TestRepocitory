using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;
using PetaPoco.Internal;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberPromotionDao : DaoBase<MemberPromotion>
	{
		public MemberPromotionDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}
		
		public MemberPromotion Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<MemberPromotion> Retrieve(long memberId, string code)
		{
			return Database.Fetch<MemberPromotion>("select * from LW_MemberPromotion where MemberId = @0 and code = @1", memberId, code);
		}

		public List<MemberPromotion> Retrieve(string code)
		{
			return Database.Fetch<MemberPromotion>("select * from LW_MemberPromotion where code = @0", code);
		}

		public long HowManyMembers(string code)
		{
			return Database.ExecuteScalar<long>("select count(*) from LW_MemberPromotion where code = @0", code);
		}

		public long HowManyMemberPromotions(long memberId)
		{
			return Database.ExecuteScalar<long>("select count(*) from LW_MemberPromotion where MemberId = @0", memberId);
		}

		public List<MemberPromotion> RetrieveByMember(long memberId, LWQueryBatchInfo batchInfo, bool unExpiredOnly)
		{
			string sql = string.Empty;
			if (unExpiredOnly)
			{
				sql = string.Format("select m.* from LW_MemberPromotion m, LW_Promotion p where m.MemberId = @0 and m.Code = p.Code and (p.EndDate is null or p.EndDate > @1)");
			}
			else
			{
				sql = string.Format("select m.* from LW_MemberPromotion m where m.MemberId = @0");
			}

			object[] args = new object[] { memberId, DateTime.Now };

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}
			return Database.Fetch<MemberPromotion>(sql, args);
		}

		public List<MemberPromotion> Retrieve(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<MemberPromotion>();
            return RetrieveByArray<long>("select * from LW_MemberPromotion where Id in (@0)", ids);
		}

		public List<MemberPromotion> RetrieveAll()
		{
			return Database.Fetch<MemberPromotion>("select * from LW_MemberPromotion");
		}

		public bool MemberInPromotionList(string code, long memberId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberPromotion where code = @0 and MemberId = @1", code, memberId) > 0;
		}

		public bool MemberInPromotionList(long memberId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberPromotion where MemberId = @0", memberId) > 0;
		}

		public bool MemberEnrolledInPromotion(string code, long memberId)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_MemberPromotion where code = @0 and MemberId = @1 and Enrolled = 1", code, memberId) > 0;
		}

		public void Delete(long promotionId)
		{
			DeleteEntity(promotionId);
		}

		public void Delete(string code)
		{
			Database.Execute("delete from LW_MemberPromotion where Code = @0", code);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MemberPromotion where MemberId = @0", memberId);
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
					ret += Database.Execute("delete from LW_MemberPromotion where MemberId in (@ids)", new { ids = ids });
				}
				txn.Complete();
			}
			return ret;
		}
	}
}
