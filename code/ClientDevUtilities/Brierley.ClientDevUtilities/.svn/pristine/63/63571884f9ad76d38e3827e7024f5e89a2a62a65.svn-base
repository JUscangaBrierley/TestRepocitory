using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class CacheRefreshDao : DaoBase<CacheRefresh>
	{
		public CacheRefreshDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public List<CacheRefresh> RetrieveAll()
		{
			return Database.Fetch<CacheRefresh>("select * from LW_CacheRefresh");
		}

		public CacheRefresh RetrieveMostRecent()
		{
			string sql = "select r.* from LW_CacheRefresh r order by r.CreateDate desc";
			var args = new object[0];
			ApplyBatchInfo(new LWQueryBatchInfo(0, 1), ref sql, ref args);
			return Database.FirstOrDefault<CacheRefresh>(sql, args);
		}
	}
}
