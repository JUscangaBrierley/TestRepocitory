using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class SkinDao : DaoBase<Skin>
	{
		public SkinDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public Skin Retrieve(long id)
		{
			return Database.FirstOrDefault<Skin>("select * from LW_Skin where id = @0", id);
		}

		public Skin Retrieve(string name)
		{
			return Database.FirstOrDefault<Skin>("select * from LW_Skin where Name = @0", name);
		}

		public List<Skin> RetrieveAll()
		{
			return Database.Fetch<Skin>("select * from LW_Skin");
		}

		public List<Skin> RetrieveAll(DateTime changedSince)
		{
			return Database.Fetch<Skin>("select * from LW_Skin where UpdateDate >= @0", changedSince);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
