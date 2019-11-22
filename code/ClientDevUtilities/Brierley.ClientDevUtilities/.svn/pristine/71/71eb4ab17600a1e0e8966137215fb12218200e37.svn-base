using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class SyncJobDao : DaoBase<SyncJob>
	{
		public SyncJobDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public SyncJob Retrieve(long id)
		{
			return GetEntity(id);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
