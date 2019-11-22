using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PointsSummaryDao : DaoBase<PointsSummary>
	{
		public PointsSummaryDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public PointsSummary Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<PointsSummary> RetrieveByMember(long memberId)
		{
			return Database.Fetch<PointsSummary>("select * from LW_PointsSummary where MemberId = @0", memberId);
		}

		public PointsSummary RetrieveByMember(long memberId, string pointEvent, string pointType)
		{
			return Database.FirstOrDefault<PointsSummary>("select * from LW_PointsSummary where MemberId = @0 and PointEvent = @1 and PointType = @2", memberId, pointEvent, pointType);
		}

		public List<PointsSummary> RetrieveByMember(long memberId, string[] pointEvents, string[] pointTypes)
		{
			string sql = "select * from LW_PointsSummary where MemberId = @0";
			if (pointEvents != null && pointEvents.Length > 0)
			{
				sql += " and PointEvent in (@pointEvents)";
			}
			if (pointTypes != null && pointTypes.Length > 0)
			{
				sql += " and PointType in (@pointTypes)";
			}

			return Database.Fetch<PointsSummary>("select * from LW_PointsSummary where MemberId = @0 ", memberId, new { pointEvents = pointEvents }, new { pointTypes = pointTypes });
		}

		public int DeleteByMember(long memberId)
		{
			return Database.Execute("delete from LW_PointsSummary where MemberId = @0", memberId);
		}

		public int DeleteByMembers(long[] memberIds)
		{
			int keysRemaining = memberIds.Length;
			int startIdx = 0;
			int nRows = 0;
			while (keysRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(memberIds, ref startIdx, ref keysRemaining);
				nRows += Database.Execute("delete from LW_PointsSummary where MemberId in (@ids)", new { ids = ids });
			}
			return nRows;
		}
	}
}
