using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class BonusDao : ContentDefDaoBase<BonusDef>
	{
		public BonusDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.Bonus)
		{
		}
		
		public BonusDef Retrieve(long id)
		{
			return GetEntity(id);
		}

		public List<BonusDef> Retrieve(long[] idList, bool populateAttributes)
		{
            if (idList == null || idList.Length == 0)
                return new List<BonusDef>();
            var ret = RetrieveByArray<long>("select * from LW_BonusDef where id in(@0)", idList);
            if (populateAttributes && ret != null)
			{
				PopulateContent(ret, populateAttributes);
			}
			return ret;
		}

		public BonusDef Retrieve(string name)
		{
			var ret = Database.FirstOrDefault<BonusDef>("select * from LW_BonusDef where name = @0", name);
			if (ret != null)
			{
				PopulateContent(ret);
			}
			return ret;
		}

		public List<BonusDef> RetrieveByCategory(long categoryId, bool excludeExpired)
		{
			string sql = "select * from LW_BonusDef where CategoryId = @0";
			if (excludeExpired)
			{
				sql += " and StartDate <= @1 and ExpiryDate > @1";
			}
			var ret = Database.Fetch<BonusDef>(sql, categoryId, DateTime.Now);
			if (ret != null)
			{
				PopulateContent(ret, true);
			}
			return ret;
		}

		public List<BonusDef> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<BonusDef>("select * from LW_BonusDef where UpdateDate >= @0", since);
		}

		public List<BonusDef> RetrieveAll()
		{
			var ret = Database.Fetch<BonusDef>("select * from LW_BonusDef");
			if (ret != null)
			{
				PopulateContent(ret, true);
			}
			return ret;
		}

		public int HowManyBonuses(List<Dictionary<string, object>> parms)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			List<long> ids = GetContentIds(parms, flags);
			return ids != null ? ids.Count : 0;
		}

		public List<long> RetrieveBonusDefIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId = null)
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

		public List<BonusDef> RetrieveBonusDefs(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			List<long> ids = GetContentIds(parms, flags);
			List<BonusDef> contents = null;
			if (ids != null && ids.Count > 0)
			{
				contents = GetContent(ids, populateAttributes, batchInfo);
			}
			return contents;
		}

		/// <summary>
		/// This method retrieves the bonus, check its quote count to make sure that it has not been met 
		/// yet and then updates the completed count.
		/// </summary>
		/// <param name="completed"></param>
		/// <returns></returns>
		public long CheckAndUpdateQuotaCount(long bonusId, long completed)
		{
			string sql = string.Format(
				@"select 
					* 
				from 
					LW_BonusDef{0} 
				where 
					id = @0",
				WithUpdateClause(LockingMode.Upgrade));
			sql += ForUpdateClause(LockingMode.Upgrade);

			long quota = 0;
			BonusDef bonus = Database.FirstOrDefault<BonusDef>(sql, bonusId);
			if (bonus == null)
			{
				throw new LWDataServiceException(string.Format("No bonus found with id {0}.", bonusId));
			}
			if (bonus.Quota != null)
			{
				if (bonus.Quota < (bonus.Completed.GetValueOrDefault(0) + completed))
				{
					//quota exceeded
					throw new LWDataServiceException(string.Format("Quota for bonus {0} will be exceeded by this request.", bonus.Name));
				}
				bonus.Completed = bonus.Completed.GetValueOrDefault(0) + completed;
				UpdateEntity(bonus);
				quota = bonus.Completed.GetValueOrDefault();
			}
			return quota;
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
