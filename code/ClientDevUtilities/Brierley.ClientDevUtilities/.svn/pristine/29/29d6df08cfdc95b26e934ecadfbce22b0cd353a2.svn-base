using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PointEventDao : DaoBase<PointEvent>
	{
		public PointEventDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public PointEvent Retrieve(long pointEventId)
		{
			return GetEntity(pointEventId);
		}

		public PointEvent Retrieve(string name)
		{
			return Database.FirstOrDefault<PointEvent>("select * from LW_PointEvent where name = @0", name);
		}

		public List<PointEvent> Retrieve(string[] names)
		{
            if (names == null || names.Length == 0)
                return new List<PointEvent>();
            return RetrieveByArray<string>("select * from LW_PointEvent where name in (@0)", names);
		}

		public List<PointEvent> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<PointEvent>("select * from LW_PointEvent where UpdateDate >= @0", since);
		}

		public List<PointEvent> RetrieveAll()
		{
			return Database.Fetch<PointEvent>("select * from LW_PointEvent");
		}

		public void Delete(long pointEventId)
		{
			DeleteEntity(pointEventId);
		}
	}
}
