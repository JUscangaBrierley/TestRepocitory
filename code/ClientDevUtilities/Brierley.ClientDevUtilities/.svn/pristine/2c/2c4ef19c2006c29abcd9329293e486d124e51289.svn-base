using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MemberOrderDao : DaoBase<MemberOrder>
	{
		public MemberOrderDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public MemberOrder Retrieve(long orderId)
		{
			return GetEntity(orderId);
		}

		public MemberOrder RetrieveByOrderNumber(string orderNmbr)
		{
			return Database.FirstOrDefault<MemberOrder>("select * from lw_MemberOrder where OrderNumber = @0", orderNmbr);
		}

		public List<MemberOrder> RetrieveByOrderNumbers(string[] orderNmbrs)
		{
            if (orderNmbrs == null || orderNmbrs.Length == 0)
                return new List<MemberOrder>();
			return RetrieveByArray<string>("select * from lw_MemberOrder where OrderNumber in (@0)", orderNmbrs);
		}

		public List<MemberOrder> RetrieveByMember(long ipCode, LWQueryBatchInfo batchInfo)
		{
			string sql = "select o.* from LW_MemberOrder o where MemberId = @0 order by CreateDate desc";
			var parameters = new object[] { ipCode };
			ApplyBatchInfo(batchInfo, ref sql, ref parameters);
			return Database.Fetch<MemberOrder>(sql, parameters);
		}

		public void Delete(long orderID)
		{
			DeleteEntity(orderID);
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_MemberOrder where MemberId = @0", memberId);
		}

		public int DeleteByMembers(long[] memberIds)
		{
			int keysRemaining = memberIds.Length;
			int startIdx = 0;
			int ret = 0;
			while (keysRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(memberIds, ref startIdx, ref keysRemaining);
				ret += Database.Execute("delete from LW_MemberOrder where MemberId in (@ids)", new { ids = ids });
			}
			return ret;
		}
	}
}
