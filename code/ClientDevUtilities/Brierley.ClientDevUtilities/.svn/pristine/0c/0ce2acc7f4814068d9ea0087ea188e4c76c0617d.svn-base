using System;
using System.Collections.Generic;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class AudienceDao : DaoBase<Audience>
	{
		public AudienceDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public Audience Retrieve(long id)
		{
			return GetEntity(id);
		}

		public Audience Retrieve(string name)
		{
			return Database.FirstOrDefault<Audience>("select * from LW_CLAudience where lower(AudienceName) = lower(@0)", name);
		}
		
		public List<Audience> RetrieveAll()
		{
			return Database.Fetch<Audience>("select * from LW_CLAudience");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
