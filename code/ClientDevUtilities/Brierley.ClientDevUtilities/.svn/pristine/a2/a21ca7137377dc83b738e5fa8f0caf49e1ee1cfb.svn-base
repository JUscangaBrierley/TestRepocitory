using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PromoTestSetDao : DaoBase<PromoTestSet>
	{
		public PromoTestSetDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public PromoTestSet Retrieve(long id)
		{
			return GetEntity(id);
		}

		public PromoTestSet Retrieve(string name)
		{
			return Database.FirstOrDefault<PromoTestSet>("select * from LW_PromoTestSet where lower(SetName) = lower(@0)", name);
		}

		public List<PromoTestSet> RetrieveByFolder(long folderId)
		{
			return Database.Fetch<PromoTestSet>("select * from LW_PromoTestSet where FolderId = @0", folderId);
		}

		public List<PromoTestSet> RetrievePopulated()
		{
			return Database.Fetch<PromoTestSet>("select * from LW_PromoTestSet where SetId in (select SetId from LW_PromoTestMember)");
		}

		public List<PromoTestSet> RetrieveAll()
		{
			return Database.Fetch<PromoTestSet>("select * from LW_PromoTestSet");
		}

		public List<PromoTestSet> RetrieveAll(DateTime changedSince)
		{
			return Database.Fetch<PromoTestSet>("select * from LW_PromoTestSet where UpdateDate >= @0", changedSince);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}