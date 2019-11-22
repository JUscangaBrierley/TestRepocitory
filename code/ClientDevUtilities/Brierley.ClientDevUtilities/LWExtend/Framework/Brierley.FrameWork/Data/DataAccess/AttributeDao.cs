using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement;
using PetaPoco;
using DM = Brierley.FrameWork.CampaignManagement.DomainModel;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class AttributeDao : DaoBase<DM.Attribute>
	{
		public AttributeDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public DM.Attribute Retrieve(long id)
		{
			return GetEntity(id);
		}

		public DM.Attribute Retrieve(string name)
		{
			return Database.FirstOrDefault<DM.Attribute>("select * from LW_CLAttribute where lower(Name) = lower(@0)", name);
		}

		public List<DM.Attribute> RetrieveByType(AttributeTypes attributeType)
		{
			return Database.Fetch<DM.Attribute>("select * from LW_CLAttribute where AttributeType = @0", attributeType);
		}

		public List<DM.Attribute> RetrieveAll()
		{
			return Database.Fetch<DM.Attribute>("select * from LW_CLAttribute");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
