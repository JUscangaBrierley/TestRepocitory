using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PromoMappingFileDao : DaoBase<PromoMappingFile>
	{
		public PromoMappingFileDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public PromoMappingFile Retrieve(long id)
		{
			return GetEntity(id);
		}

		public PromoMappingFile Retrieve(string name)
		{
			return Database.FirstOrDefault<PromoMappingFile>("select * from LW_PromoMappingFile where lower(name) = lower(@0)", name);
		}

		public List<PromoMappingFile> RetrieveAll()
		{
			return Database.Fetch<PromoMappingFile>("select * from LW_PromoMappingFile");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
