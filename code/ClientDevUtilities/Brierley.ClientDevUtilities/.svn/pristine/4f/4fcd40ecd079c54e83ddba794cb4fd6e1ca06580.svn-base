using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ArchiveObjectDao : DaoBase<ArchiveObject>
	{
		public ArchiveObjectDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public ArchiveObject Retrieve(long id)
		{
			return Database.FirstOrDefault<ArchiveObject>("where id = @0", id);
		}

		public List<ArchiveObject> RetrieveByGroup(long groupId, long runNumber)
		{
			return Database.Fetch<ArchiveObject>("where GroupId = @0 and RunNumber = @1", groupId, runNumber);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
