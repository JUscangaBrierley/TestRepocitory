using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class GlobalDao : DaoBase<Global>
	{
		public GlobalDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}
				
		public Global Retrieve(long id)
		{
			return GetEntity(id);
		}

		public Global Retrieve(string name)
		{
			return Database.FirstOrDefault<Global>("select * from LW_CLGlobal where lower(name) = lower(@0)", name);
		}

		public List<Global> RetrieveAll()
		{
			return Database.Fetch<Global>("select * from LW_CLGlobal");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
