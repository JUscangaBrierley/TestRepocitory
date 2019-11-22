using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class GlobalAttributeDao : DaoBase<GlobalAttribute>
	{
		public GlobalAttributeDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}
		
		public GlobalAttribute Retrieve(long id)
		{
			return GetEntity(id);
		}

		public GlobalAttribute Retrieve(string attributeName)
		{
			return Database.FirstOrDefault<GlobalAttribute>("select * from LW_CLGlobalAttribute where lower(AttributeName) = lower(@0)", attributeName);
		}

		public GlobalAttribute Retrieve(long globalId, string attributeName)
		{
			return Database.FirstOrDefault<GlobalAttribute>(
				"select * from LW_CLGlobalAttribute where GlobalId = @0 and lower(AttributeName) = lower(@1)", 
				globalId, 
				attributeName);
		}
		
		public List<GlobalAttribute> RetrieveAllByGlobalId(long globalId)
		{
			return Database.Fetch<GlobalAttribute>("select * from LW_CLGlobalAttribute where GlobalId = @0", globalId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public void DeleteByGlobalId(long globalId)
		{
			Database.Execute("delete from LW_CLGlobalAttribute where GlobalId = @0", globalId);
		}
	}
}
