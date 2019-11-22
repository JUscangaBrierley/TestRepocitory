using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class LibMessageLogDao : DaoBase<LIBMessageLog>
	{
		public LibMessageLogDao(Database database, ServiceConfig config) : base(database, config)
		{
		}

		public LIBMessageLog Retrieve(long id)
		{
			return Database.FirstOrDefault<LIBMessageLog>("select * from LW_LibMessageLog where id = @0", id);
		}

		public List<LIBMessageLog> RetrieveByJobNumber(long jobNumber)
		{
			return Database.Fetch<LIBMessageLog>("select * from LW_LibMessageLog where JobNumber = @0", jobNumber);
		}

		public int HowMany(long jobNumber)
		{
			return Database.ExecuteScalar<int>("select count(*) from LW_LibMessageLog where JobNumber = @0", jobNumber);
		}
	}
}
