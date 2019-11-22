using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ChannelDao : DaoBase<ChannelDef>
	{
		public ChannelDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public void Delete(string name)
		{
			ChannelDef channel = Retrieve(name);
			if (channel != null)
			{
				DeleteEntity(channel);
			}
		}

		public ChannelDef Retrieve(string name)
		{
			return Database.FirstOrDefault<ChannelDef>("select * from LW_ChannelDef where Name = @0", name);
		}

		public List<ChannelDef> RetrieveAll()
		{
			return Database.Fetch<ChannelDef>("select * from LW_ChannelDef");
		}
	}
}
