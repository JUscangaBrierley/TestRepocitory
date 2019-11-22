using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class NotificationDao : ContentDefDaoBase<NotificationDef>
	{
		public NotificationDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.Notification)
		{
		}

		public NotificationDef Retrieve(long id)
		{
			NotificationDef notification = GetEntity(id);
			if (notification != null)
			{
				PopulateContent(notification);
			}
			return notification;
		}

		public NotificationDef Retrieve(string name)
		{
			NotificationDef notification = Database.FirstOrDefault<NotificationDef>("select * from LW_NotificationDef where Name = @0", name);
			if (notification != null)
			{
				PopulateContent(notification);
			}
			return notification;
		}

		public List<NotificationDef> RetrieveAll(LWQueryBatchInfo batchInfo)
		{
			string sql = "select m.* from LW_NotificationDef m";
			var args = new object[0];

			ApplyBatchInfo(batchInfo, ref sql, ref args);
			var ret = Database.Fetch<NotificationDef>(sql, args);
			if (ret != null && ret.Count > 0)
			{
				PopulateContent(ret, true);
			}
			return ret;
		}

		public List<NotificationDef> Retrieve(long[] idList, bool populateAttributes)
		{
            if (idList == null || idList.Length == 0)
                return new List<NotificationDef>();
			var ret = RetrieveByArray<long>("select * from LW_NotificationDef where id in (@0)", idList);
			if (ret != null)
			{
				PopulateContent(ret, populateAttributes);
			}
			return ret;
		}

		public List<NotificationDef> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<NotificationDef>("select * from LW_NotificationDef where UpdateDate >= @0", since);
		}

		public int HowManyNotifications(List<Dictionary<string, object>> parms)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			IList<long> ids = GetContentIds(parms, flags);
			return ids != null ? ids.Count : 0;
		}

		public List<long> RetrieveNotificationDefIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId)
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

		public List<NotificationDef> RetrieveNotificationDefs(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			List<long> ids = GetContentIds(parms, flags);
			List<NotificationDef> notifications = null;
			if (ids != null && ids.Count > 0)
			{
				notifications = GetContent(ids, populateAttributes, batchInfo);
			}
			return notifications;
		}

		public void Delete(long id)
		{
			DeleteEntity(id);
		}
	}
}
