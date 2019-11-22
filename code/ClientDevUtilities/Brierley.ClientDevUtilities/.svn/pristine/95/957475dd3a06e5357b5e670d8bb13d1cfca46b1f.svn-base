using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ContentAttributeDefDao : DaoBase<ContentAttributeDef>
	{
		public ContentAttributeDefDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public ContentAttributeDef Retrieve(long id)
		{
			return GetEntity(id);
		}

		public ContentAttributeDef Retrieve(string name)
		{
			return Database.FirstOrDefault<ContentAttributeDef>("select * from LW_ContentAttributeDef where lower(Name) = lower(@0)", name);
		}

		public List<ContentAttributeDef> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<ContentAttributeDef>("select * from LW_ContentAttributeDef where UpdateDate >= @0", since);
		}

		public List<ContentAttributeDef> RetrieveAll()
		{
			return Database.Fetch<ContentAttributeDef>("select * from LW_ContentAttributeDef");
		}

		public List<ContentAttributeDef> RetrieveAll(ContentObjType contentType)
		{
			return Database.Fetch<ContentAttributeDef>("select * from LW_ContentAttributeDef where ContentTypes like @0", string.Format("%{0}%", contentType.ToString()));
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
