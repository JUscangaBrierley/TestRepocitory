using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PointTypeDao : DaoBase<PointType>
	{
		public PointTypeDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public PointType Retrieve(long pointTypeId)
		{
			return GetEntity(pointTypeId);
		}

		public PointType Retrieve(string name)
		{
			return Database.FirstOrDefault<PointType>("select * from LW_PointType where Name = @0", name);
		}

		public List<PointType> Retrieve(string[] names)
		{
            if (names == null || names.Length == 0)
                return new List<PointType>();
			return RetrieveByArray<string>("select * from LW_PointType where Name in (@0)", names);
		}

		public List<PointType> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<PointType>("select * from LW_PointType where UpdateDate >= @0", since);
		}

		public List<PointType> RetrieveAll()
		{
			return Database.Fetch<PointType>("select * from LW_PointType order by ConsumptionPriority");
		}

		public void Delete(long pointTypeID)
		{
			DeleteEntity(pointTypeID);
		}
	}
}
