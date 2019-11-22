using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ContentAttributeDao : DaoBase<ContentAttribute>
	{
		public ContentAttributeDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public ContentAttribute Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<ContentAttribute> RetrieveByRefId(ContentObjType contentType, long refId)
		{
			return Database.Fetch<ContentAttribute>("select * from LW_ContentAttribute where ContentType = @0 and RefId = @1", contentType.ToString(), refId);
		}

		public List<ContentAttribute> RetrieveByRefIds(ContentObjType contentType, long[] refIds)
		{
            if (refIds == null || refIds.Length == 0)
                return new List<ContentAttribute>();

            int idsRemaining = refIds.Length;
            int startIndex = 0;
            List<ContentAttribute> contents = new List<ContentAttribute>();
            while (idsRemaining > 0)
            {
                long[] ids = LimitInClauseList<long>(refIds, ref startIndex, ref idsRemaining);
                var set = Database.Fetch<ContentAttribute>("select * from LW_ContentAttribute where ContentType = @0 and RefId in (@ids)", contentType.ToString(), new { ids = ids });
                if (set != null && set.Count > 0)
                {
                    contents.AddRange(set);
                }
            }
            return contents;
        }

		public List<ContentAttribute> RetrieveByContentAttributeDef(ContentObjType contentType, long attDefId)
		{
			return Database.Fetch<ContentAttribute>("select * from LW_ContentAttribute where ContentType = @0 and ContentAttributeDefId = @1", contentType.ToString(), attDefId);
		}

		public List<ContentAttribute> RetrieveAll()
		{
			return Database.Fetch<ContentAttribute>("select * from LW_ContentAttribute");
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

		public void DeleteByRefId(ContentObjType contentType, long refId)
		{
			Database.Execute("delete from LW_ContentAttribute where ContentType = @0 and RefId = @1", contentType.ToString(), refId);
		}
	}
}
