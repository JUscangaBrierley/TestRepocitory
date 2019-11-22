using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class MessageDao : ContentDefDaoBase<MessageDef>
	{
		public MessageDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.Message)
		{
		}

		public MessageDef Retrieve(long id)
		{
			MessageDef message = GetEntity(id);
			if (message != null)
			{
				PopulateContent(message);
			}
			return message;
		}

		public MessageDef Retrieve(string name)
		{
			MessageDef message = Database.FirstOrDefault<MessageDef>("select * from LW_MessageDef where Name = @0", name);
			if (message != null)
			{
				PopulateContent(message);
			}
			return message;
		}

		public List<MessageDef> RetrieveAll(LWQueryBatchInfo batchInfo)
		{
			string sql = "select m.* from LW_MessageDef m";
			var args = new object[0];

			ApplyBatchInfo(batchInfo, ref sql, ref args);
			var ret = Database.Fetch<MessageDef>(sql, args);
			if (ret != null && ret.Count > 0)
			{
				PopulateContent(ret, true);
			}
			return ret;
		}

		public List<MessageDef> Retrieve(long[] idList, bool populateAttributes)
		{
            if (idList == null || idList.Length == 0)
                return new List<MessageDef>();
            var ret = RetrieveByArray<long>("select * from LW_MessageDef where id in (@0)", idList);
			if (ret != null)
			{
				PopulateContent(ret, populateAttributes);
			}
			return ret;
		}

		public List<MessageDef> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<MessageDef>("select * from LW_MessageDef where UpdateDate >= @0", since);
		}

		public int HowManyMessages(List<Dictionary<string, object>> parms)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			IList<long> ids = GetContentIds(parms, flags);
			return ids != null ? ids.Count : 0;
		}

		public List<long> RetrieveMessageDefIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();

			if (folderId.HasValue)
			{
				if (parms == null)
				{
					parms = new List<Dictionary<string, object>>();
				}
				Dictionary<string, object> p = new Dictionary<string, object>();
				p.Add("Property", "FolderId");
				p.Add("Predicate", LWCriterion.Predicate.Eq);
				p.Add("Value", folderId.Value);
				parms.Add(p);
			}

			List<long> ids = GetContentIds(parms, flags);
			if (!string.IsNullOrEmpty(sortExpression) && ids != null && ids.Count > 0)
			{
				ids = SortContentIds(ids, sortExpression, ascending);
			}
			return ids;
		}

		public List<MessageDef> RetrieveMessageDefs(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			List<long> ids = GetContentIds(parms, flags);
			List<MessageDef> messages = null;
			if (ids != null && ids.Count > 0)
			{
				messages = GetContent(ids, populateAttributes, batchInfo);
			}
			return messages;
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}

        protected override string HandleSqlForProperties(string propertyName, LWCriterion.Predicate op, object propertyValue, List<object> parameters)
        {
            string sql = string.Empty;
            if (propertyName == "Active")
            {
                if ((bool)propertyValue)
                {
                    sql = string.Format("t.StartDate <= @{0} and t.ExpiryDate > @{0}", parameters.Count);
                }
                else
                {
                    sql = string.Format("t.StartDate > @{0} or t.ExpiryDate < @{0}", parameters.Count);
                }
                parameters.Add(DateTime.Now);
            }
            else
            {
                return base.HandleSqlForProperties(propertyName, op, propertyValue, parameters);
            }
            return sql;
        }
    }
}
