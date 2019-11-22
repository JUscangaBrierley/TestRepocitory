using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class LanguageDao : DaoBase<LanguageDef>
	{
		public LanguageDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public LanguageDef Retrieve(string culture)
		{
			return Database.FirstOrDefault<LanguageDef>("select * from LW_LanguageDef where Culture = @0", culture);
		}

		public List<LanguageDef> RetrieveAll()
		{
			return Database.Fetch<LanguageDef>("select * from LW_LanguageDef");
		}
	}
}
