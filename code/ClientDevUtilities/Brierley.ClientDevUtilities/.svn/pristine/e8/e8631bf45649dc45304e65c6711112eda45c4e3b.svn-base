using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class TextBlockDao : ContentDefDaoBase<TextBlock>
	{
		public TextBlockDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.TextBlock)
		{
		}

        public TextBlock Retrieve(long id)
        {
			var ret = GetEntity(id);
			if (ret != null)
			{
				PopulateContent(ret);
			}
			return ret;
        }

        public TextBlock Retrieve(string name)
        {
			var ret = Database.FirstOrDefault<TextBlock>("select * from LW_TextBlock where name = @0", name);
            if (ret != null)
            {
				PopulateContent(ret);
            }
			return ret;
        }

        public List<TextBlock> RetrieveAll()
        {
			var ret = Database.Fetch<TextBlock>("select * from LW_TextBlock order by Name");
			if (ret.Count > 0)
			{
				PopulateContent(ret, true);
			}
			return ret;
        }

        public List<TextBlock> RetrieveAll(DateTime changedSince)
        {
			var ret = Database.Fetch<TextBlock>("select * from LW_TextBlock where UpdateDate >= @0 order by Name", changedSince);
			if (ret.Count > 0)
			{
				PopulateContent(ret, true);
			}
			return ret;
        }

        public void Delete(long id)
        {
			DeleteEntity(id);
        }

        public void Delete(string name)
        {
            TextBlock entity = Retrieve(name);
            if (entity != null)
            {
				DeleteEntity(entity.Id);
            }
        }
	}
}
