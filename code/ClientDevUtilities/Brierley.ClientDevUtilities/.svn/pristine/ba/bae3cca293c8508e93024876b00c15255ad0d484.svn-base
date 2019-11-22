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
	public class RuleExecutionLogDao : DaoBase<RuleExecutionLog>
	{
		public RuleExecutionLogDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public RuleExecutionLog Retrieve(string jobId)
		{
			return GetEntity(jobId);
		}

		public List<RuleExecutionLog> Retrieve(long memberId, RuleExecutionStatus? status, PointTransactionOwnerType? ownerType, long? ownerId, long[] rowkeys, DateTime? fromDate, DateTime? toDate)
		{
			string sql = "select * from LW_RuleExecutionLog where MemberId = @0";

			if (status != null)
			{
				sql += " and ExecutionStatus = @1";
			}
			if (ownerType != null)
			{
				sql += " and OwnerType = @2";
			}
			if (ownerId != null)
			{
				sql += " and OwnerId = @3";
			}
			if (rowkeys != null && rowkeys.Length > 0)
			{
				sql += " and RowKey in (@keys)";
			}
			if (fromDate != null)
			{
				sql += " and CreateDate >= @5";
			}
			if (toDate != null)
			{
				sql += " and CreateDate < @6";
			}
			return Database.Fetch<RuleExecutionLog>(sql, new object[] { memberId, status.GetValueOrDefault().ToString(), ownerType.GetValueOrDefault().ToString(), ownerId, new { keys = rowkeys }, fromDate, toDate });
		}

		public void Delete(string jobId)
		{
			DeleteEntity(jobId);
		}
	}
}
