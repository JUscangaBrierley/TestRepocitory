using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class TriggerUserEventLogDao : DaoBase<TriggerUserEventLog>
	{
		public TriggerUserEventLogDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public TriggerUserEventLog Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<long> Retrieve(long memberId, string sortExpression, bool ascending)
		{
			string sql = "select Id from LW_TriggerUserEventLog where MemberId = @0";
			if (!string.IsNullOrEmpty(sortExpression))
			{
				sql += " order by " + sortExpression + (ascending ? "asc" : "desc");
			}
			return Database.Fetch<long>(sql, memberId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
