using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class CategoryDao : DaoBase<Category>
	{
		public CategoryDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public Category Retrieve(long id)
		{
			return GetEntity(id);
		}

		public Category Retrieve(long parentid, string name)
		{
			return Database.FirstOrDefault<Category>("select * from LW_Category where Name = @0 and ParentCategoryId = @1", name, parentid);
		}

		public List<Category> RetrieveCategoriesByIds(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<Category>();
            return RetrieveByArray<long>("select * from LW_Category where id in (@0)", ids);
		}

		public List<Category> RetrieveChildCategoriesAll(long categoryId, bool visibleInLN)
		{
			return RetrieveChildCategoriesAll(categoryId, visibleInLN, null);
		}

		public List<Category> RetrieveChildCategoriesAll(long categoryId, bool visibleInLN, LWQueryBatchInfo batchInfo)
		{
			var args = new object[] {categoryId};
			string sql = "select c.* from LW_Category c where ParentCategoryId = @0";

			if (visibleInLN)
			{
				sql += " and (IsVisibleInLn is null or IsVisibleInLn = 1)";
			}
			else
			{
				sql += " and IsVisibleInLn is not null and IsVisibleInLn = 0";
			}
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			return Database.Fetch<Category>(sql, args);
		}

		public List<Category> RetrieveTopLevelCategoriesByType(Brierley.FrameWork.Common.CategoryType type, bool visibleInLN)
		{
			string sql = "select * from LW_Category where ParentCategoryId = 0 and CategoryType = @0";
			if (visibleInLN)
			{
				sql += " and (IsVisibleInLn is null or IsVisibleInLn = 1)";
			}
			else
			{
				sql += " and IsVisibleInLn is not null and IsVisibleInLn = 0";
			}
			return Database.Fetch<Category>(sql, type);
		}

		public List<Category> RetrieveByType(Brierley.FrameWork.Common.CategoryType type, bool visibleInLN)
		{
			string sql = "select * from LW_Category where CategoryType = @0";
			if (visibleInLN)
			{
				sql += " and (IsVisibleInLn is null or IsVisibleInLn = 1)";
			}
			else
			{
				sql += " and IsVisibleInLn is not null and IsVisibleInLn = 0";
			}
			return Database.Fetch<Category>(sql, type);
		}

		public List<Category> RetrieveByVisibility(bool visibleInLN)
		{
			string sql = "select * from LW_Category where";
			if (visibleInLN)
			{
				sql += " (IsVisibleInLn is null or IsVisibleInLn = 1)";
			}
			else
			{
				sql += " IsVisibleInLn is not null and IsVisibleInLn = 0";
			}
			return Database.Fetch<Category>(sql);
		}

		public List<Category> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<Category>("select * from LW_Category where UpdateDate >= @0", since);
		}

		public List<Category> RetrieveAll()
		{
			return Database.Fetch<Category>("select * from LW_Category");
		}

		public void Delete(long categoryId)
		{
			DeleteEntity(categoryId);
		}
	}
}
