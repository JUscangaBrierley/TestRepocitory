using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class LangChanContentDao : DaoBase<LangChanContent>
	{
		public LangChanContentDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public List<LangChanContent> Retrieve(ContentObjType type, long refId)
		{
			return Database.Fetch<LangChanContent>("select * from LW_LangChanContent where LangChanType = @0 and RefId = @1", type.ToString(), refId);
		}

		public List<LangChanContent> Retrieve(ContentObjType type, long[] refIds)
		{
			int idsRemaining = refIds.Length;
			int startIndex = 0;
			List<LangChanContent> contents = new List<LangChanContent>();
			while (idsRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(refIds, ref startIndex, ref idsRemaining);
				var set = Database.Fetch<LangChanContent>("select * from LW_LangChanContent where LangChanType = @0 and RefId in (@ids)", type.ToString(), new { ids = ids });
				if (set != null && set.Count > 0)
				{
					contents.AddRange(set);
				}
			}
			return contents;
		}

		public List<LangChanContent> Retrieve(ContentObjType? type, LanguageDef language, ChannelDef channel)
		{
            string sql = "select * from LW_LangChanContent where 1 = 1";
            List<object> args = new List<object>();

            if(type.HasValue)
            {
                sql += "and LangChanType = @" + args.Count;
                args.Add(type.ToString());
            }

            if(language != null)
            {
                sql += "and LanguageCulture = @" + args.Count;
                args.Add(language.Language);
            }

            if(channel != null)
            {
                sql += "and Channel = @" + args.Count;
                args.Add(channel.Name);
            }

            return Database.Fetch<LangChanContent>(sql, args.ToArray());
		}

		public void Delete(ContentObjType type, long refId)
		{
			Database.Execute("delete from LW_LangChanContent where LangChanType = @0 and RefId = @1", type.ToString(), refId);
		}
	}
}
