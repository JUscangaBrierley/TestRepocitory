using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PromoDataFileDao : DaoBase<PromoDataFile>
	{
		public PromoDataFileDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public PromoDataFile Retrieve(long id)
		{
			return GetEntity(id);
		}

		public PromoDataFile Retrieve(string name)
		{
			return Database.FirstOrDefault<PromoDataFile>("select * from LW_PromoDataFile where lower(name) = lower(@0)", name);
		}

		public List<PromoDataFile> RetrieveAll()
		{
			return Database.Fetch<PromoDataFile>("select * from LW_PromoDataFile");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
