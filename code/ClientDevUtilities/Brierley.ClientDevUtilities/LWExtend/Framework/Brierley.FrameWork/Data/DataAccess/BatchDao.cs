using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class BatchDao : DaoBase<Batch>
	{
		public BatchDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public bool Exists(string batchName, long? excludingId = null)
		{
			string sql = "select count(*) from LW_Batch where name = @0";
			if (excludingId.GetValueOrDefault() > 0)
			{
				sql += " and Id <> @1";
			}
			return Database.ExecuteScalar<int>(sql, batchName, excludingId) > 0;
		}

		public Batch Retrieve(long batchID)
		{
			return GetEntity(batchID);
		}

		public Batch Retrieve(string batchName)
		{
			return Database.FirstOrDefault<Batch>("select * from LW_Batch where name = @0", batchName);
		}

		public List<Batch> RetrieveAll()
		{
			return Database.Fetch<Batch>("select * from LW_Batch order by lower(name)");
		}

		public List<Batch> RetrieveAll(DateTime changedSince)
		{
			return Database.Fetch<Batch>("select * from LW_Batch where UpdateDate >= @0 order by lower(name)", changedSince);
		}

		public List<Batch> RetrieveAllActive()
		{
			return Database.Fetch<Batch>("select * from LW_Batch where StartDate <= @0 and EndDate >= @0 order by lower(name)", DateTime.Now);
		}

		public void Delete(long batchId)
		{
			DeleteEntity(batchId);
		}
	}
}
