using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class StepDao : DaoBase<Step>
	{
		public StepDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}
		
		public Step Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<Step> Retrieve(string name)
		{
			return Database.Fetch<Step>("select * from LW_CLStep where lower(UIName) = lower(@0)", name);
		}

		public List<Step> RetrieveAll()
		{
			return Database.Fetch<Step>("select * from LW_CLStep");
		}

		public List<Step> RetrieveAllByCampaignID(long campaignId)
		{
			return Database.Fetch<Step>("select * from LW_CLStep where CampaignId = @0", campaignId);
		}

		public Step RetrieveByTableId(long tableId)
		{
			return Database.FirstOrDefault<Step>("select * from LW_CLStep where OutputTableId = @0", tableId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
