using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class FulfillmentProviderDao : DaoBase<FulfillmentProvider>
	{
		public FulfillmentProviderDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public FulfillmentProvider Retrieve(long id)
		{
			return GetEntity(id);
		}

		public FulfillmentProvider Retrieve(string name)
		{
			return Database.FirstOrDefault<FulfillmentProvider>("select * from LW_FulfillmentProvider where Name = @0", name);
		}

		public List<FulfillmentProvider> RetrieveAll()
		{
			return Database.Fetch<FulfillmentProvider>("select * from LW_FulfillmentProvider");
		}
	}
}
