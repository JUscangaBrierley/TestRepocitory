using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class UserAgentMapDao : DaoBase<UserAgentMap>
	{
		public UserAgentMapDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public UserAgentMap Retrieve(long id)
		{
			return GetEntity(id);
		}

		public UserAgentMap Retrieve(string userAgent)
		{
			return Database.FirstOrDefault<UserAgentMap>("select * from LW_UserAgentMap where UserAgent = @0", userAgent);
		}

		public List<UserAgentMap> RetrieveAll()
		{
			return Database.Fetch<UserAgentMap>("select * from LW_UserAgentMap");
		}

		public List<UserAgentMap> RetrieveAll(DateTime changedSince)
		{
			return Database.Fetch<UserAgentMap>("select * from LW_UserAgentMap where UpdateDate >= @0", changedSince);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
