using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PromoTemplateDao  : DaoBase<PromoTemplate>
	{
		public PromoTemplateDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public PromoTemplate Retrieve(long id)
		{
			return GetEntity(id);
		}

		public PromoTemplate Retrieve(string name)
		{
			return Database.FirstOrDefault<PromoTemplate>("select * from LW_PromoTemplate where lower(name) = lower(@0)", name);
		}

		public List<PromoTemplate> RetrieveAll()
		{
			return Database.Fetch<PromoTemplate>("select * from LW_PromoTemplate");
		}

		public List<string> RetrievePromoTemplateTypes()
		{
			return Database.Fetch<string>("select distinct TemplateType from LW_PromoTemplate order by TemplateType");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
