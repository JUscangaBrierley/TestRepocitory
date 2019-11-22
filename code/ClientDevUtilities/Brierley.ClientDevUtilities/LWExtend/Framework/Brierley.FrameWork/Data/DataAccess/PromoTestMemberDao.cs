using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PromoTestMemberDao : DaoBase<PromoTestMember>
	{
		public PromoTestMemberDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public PromoTestMember Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<PromoTestMember> RetrieveBySet(long setId)
		{
			return Database.Fetch<PromoTestMember>("select * from LW_PromoTestMember where SetId = @0", setId);
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public void DeleteBySetId(long setId)
		{
			Database.Execute("delete from LW_PromoTestMember where SetId = @0", setId);
		}
	}
}