using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class LWEventDao : DaoBase<LWEvent>
	{
		public LWEventDao(Database database, ServiceConfig config) : base(database, config)
		{
		}

		public LWEvent Retrieve(long id)
		{
			return Database.FirstOrDefault<LWEvent>("select * from LW_Event where id = @0", id);
		}

		public LWEvent Retrieve(string name)
		{
			return Database.FirstOrDefault<LWEvent>("select * from LW_Event where name = @0", name);
		}

		public List<LWEvent> RetrieveChangedObjects(DateTime since, bool userDefinedOnly)
		{
            return Database.Fetch<LWEvent>("select * from LW_Event where UpdateDate >= @0 and UserDefined in (1,@1)", since, userDefinedOnly);
		}

		public List<LWEvent> RetrieveAll(bool userDefinedOnly)
		{
			return Database.Fetch<LWEvent>("select * from LW_Event where UserDefined in (1,@0)", userDefinedOnly);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
