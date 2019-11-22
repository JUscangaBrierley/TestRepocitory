using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberTierDao : DaoBase<MemberTier>
	{
		public TierDao TierDao { get; set; }
		
		public MemberTierDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		private void Populate(MemberTier tier)
		{
			if (tier != null)
			{
				tier.TierDef = TierDao.Retrieve(tier.TierDefId);
			}
		}

		public MemberTier Retrieve(long tierId)
		{
			MemberTier ret = Database.FirstOrDefault<MemberTier>("where id = @0", tierId);
			if (ret != null)
			{
				Populate(ret);
			}
			return ret;
		}

		public List<MemberTier> RetrieveByMember(long ipCode)
		{
			return Database.Fetch<MemberTier>("where MemberId = @0 order by Id desc", ipCode);
		}

		public MemberTier RetrieveByMember(long ipCode, DateTime date)
		{
			var ret = Database.FirstOrDefault<MemberTier>("where MemberId = @0 and FromDate <= @1 and ToDate >= @1 order by Id desc", ipCode, date);
			if(ret != null)
			{
				Populate(ret);
			}
			return ret;
		}

		public MemberTier RetrieveByMember(long ipCode, string tierName, DateTime date)
		{
			var ret = Database.FirstOrDefault<MemberTier>(
				"select mt.* from LW_MemberTiers mt inner join LW_Tiers t on t.TierId = mt.TierId where mt.MemberId = @0 and FromDate <= @1 and ToDate >= @1 and t.TierName = @2 order by Id desc", 
				ipCode, 
				date, 
				tierName);
			if (ret != null)
			{
				Populate(ret);
			}
			return ret;
		}

		public List<MemberTier> RetrieveAll()
		{
			return Database.Fetch<MemberTier>("");
		}

		public void Delete(long tierID)
		{
			DeleteEntity(tierID);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MemberTiers where MemberId = @0", memberId);
		}

		public int DeleteByMembers(long[] memberIds)
		{
			int keysRemaining = memberIds.Length;
			int startIdx = 0;
			int nRows = 0;
			using (var txn = Database.GetTransaction())
			{
				while (keysRemaining > 0)
				{
					long[] ids = LimitInClauseList<long>(memberIds, ref startIdx, ref keysRemaining);
					nRows += Database.Execute("delete from LW_MemberTiers where MemberId in (@ipcodes)", new { ipcodes = ids });
				}
				txn.Complete();
			}
			return nRows;
		}
	}
}
