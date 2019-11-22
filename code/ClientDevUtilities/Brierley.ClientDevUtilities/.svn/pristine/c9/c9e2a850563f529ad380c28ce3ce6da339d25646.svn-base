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
	public class MemberStoreDao : DaoBase<MemberStore>
	{
		public MemberStoreDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public long HowManyPreferredStores(long memberId)
		{
			return Database.ExecuteScalar<long>("select count(*) from LW_MemberStore m where MemberId = @0", memberId);
		}

		public MemberStore Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<MemberStore> RetrieveByMember(long ipCode)
		{
			return Database.Fetch<MemberStore>("select * from LW_MemberStore where MemberId = @0", ipCode);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MemberStore where MemberId = @0", memberId);
		}

		public int DeleteByMembers(long[] memberIds)
		{
			int keysRemaining = memberIds.Length;
			int startIdx = 0;
			int nRows = 0;
			while (keysRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(memberIds, ref startIdx, ref keysRemaining);
				nRows += Database.Execute("delete from LW_MemberStore where MemberId in (@ids)", new { ids = ids });
			}
			return nRows;
		}
	}
}
