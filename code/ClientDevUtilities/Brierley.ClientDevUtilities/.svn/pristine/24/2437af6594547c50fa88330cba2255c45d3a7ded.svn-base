using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class NotificationCategoryDao : DaoBase<NotificationCategory>
	{
		public NotificationCategoryDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public NotificationCategory Retrieve(long id)
		{
			return GetEntity(id);
		}

		public NotificationCategory Retrieve(long parentid, string name)
		{
			return Database.FirstOrDefault<NotificationCategory>("select * from LW_NotificationCategory where Name = @0 and ParentCategoryId = @1", name, parentid);
		}

		public List<NotificationCategory> RetrieveNotificationCategoriesByIds(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<NotificationCategory>();
            return RetrieveByArray<long>("select * from LW_NotificationCategory where id in (@0)", ids);
		}

		public List<NotificationCategory> RetrieveChildNotificationCategoriesAll(long notificationId, bool visibleInLN)
		{
			return RetrieveChildNotificationCategoriesAll(notificationId, visibleInLN, null);
		}

		public List<NotificationCategory> RetrieveChildNotificationCategoriesAll(long notificationId, bool visibleInLN, LWQueryBatchInfo batchInfo)
		{
			var args = new object[] { notificationId };
			string sql = "select c.* from LW_NotificationCategory c where ParentCategoryId = @0";

			if (visibleInLN)
			{
				sql += " and (IsVisibleInLn is null or IsVisibleInLn = 1)";
			}
			else
			{
				sql += " and IsVisibleInLn is not null and IsVisibleInLn = 0";
			}
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<NotificationCategory>(sql, args);
		}

		public List<NotificationCategory> RetrieveTopLevelNotificationCategoriesByType(Brierley.FrameWork.Common.CategoryType type, bool visibleInLN)
		{
			string sql = "select * from LW_NotificationCategory where ParentCategoryId = 0 and CategoryType = @0";
			if (visibleInLN)
			{
				sql += " and (IsVisibleInLn is null or IsVisibleInLn = 1)";
			}
			else
			{
				sql += " and IsVisibleInLn is not null and IsVisibleInLn = 0";
			}
			return Database.Fetch<NotificationCategory>(sql, type);
		}

		public List<NotificationCategory> RetrieveByType(Brierley.FrameWork.Common.CategoryType type, bool visibleInLN)
		{
			string sql = "select * from LW_NotificationCategory where CategoryType = @0";
			if (visibleInLN)
			{
				sql += " and (IsVisibleInLn is null or IsVisibleInLn = 1)";
			}
			else
			{
				sql += " and IsVisibleInLn is not null and IsVisibleInLn = 0";
			}
			return Database.Fetch<NotificationCategory>(sql, type);
		}

		public List<NotificationCategory> RetrieveByVisibility(bool visibleInLN)
		{
			string sql = "select * from LW_NotificationCategory where";
			if (visibleInLN)
			{
				sql += " (IsVisibleInLn is null or IsVisibleInLn = 1)";
			}
			else
			{
				sql += " IsVisibleInLn is not null and IsVisibleInLn = 0";
			}
			return Database.Fetch<NotificationCategory>(sql);
		}

		public List<NotificationCategory> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<NotificationCategory>("select * from LW_NotificationCategory where UpdateDate >= @0", since);
		}

		public List<NotificationCategory> RetrieveAll()
		{
			return Database.Fetch<NotificationCategory>("select * from LW_NotificationCategory");
		}

		public void Delete(long notificationId)
		{
			DeleteEntity(notificationId);
		}
	}
}
