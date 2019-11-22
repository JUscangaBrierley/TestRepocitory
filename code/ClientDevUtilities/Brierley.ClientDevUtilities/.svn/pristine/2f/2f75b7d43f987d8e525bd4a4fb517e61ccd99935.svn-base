using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class RewardDao : ContentDefDaoBase<RewardDef>
	{
		private const string _className = "MemberRewardDao";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		
		public RewardDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.Reward)
		{
		}

		public override void Create(RewardDef reward)
		{
			reward = StripSecondsFromDates(reward);
			base.Create(reward);
		}

		public override void Update(RewardDef reward)
		{
			reward = StripSecondsFromDates(reward);
			base.Update(reward);
		}

		public Category RetrieveCategoryForRewardDef(long rewardId)
		{
			return Database.FirstOrDefault<Category>("select c.* from LW_Category c, LW_Product p, LW_RewardsDef r where r.ProductId = p.Id and p.CategoryId = c.ID and r.Id = @0", rewardId);
		}

		public RewardDef Retrieve(long rewardId)
		{
			RewardDef ret = GetEntity(rewardId);
			if (ret != null)
			{
				PopulateContent(ret);
			}
			return ret;
		}

		public RewardDef Retrieve(string rewardName)
		{
			RewardDef ret = Database.FirstOrDefault<RewardDef>("select * from LW_RewardsDef where name = @0", rewardName);
			if (ret != null)
			{
				PopulateContent(ret);
			}
			return ret;
		}

		public List<RewardDef> RetrieveByCertificateTypeCode(string certTypeCode)
		{
			var rewards = Database.Fetch<RewardDef>("select * from LW_RewardsDef where CertificateTypeCode = @0", certTypeCode);
			if (rewards != null)
			{
				PopulateContent(rewards, true);
			}
			return rewards;
		}

		public List<RewardDef> RetrieveByTier(long tierId)
		{
			var rewards = Database.Fetch<RewardDef>("select * from LW_RewardsDef where TierId = @0", tierId);
			if (rewards != null)
			{
				PopulateContent(rewards, true);
			}
			return rewards;
		}

        public List<RewardDef> RetrieveByRewardType(RewardType rewardType)
        {
            var rewards = Database.Fetch<RewardDef>("select * from LW_RewardsDef where RewardType = @0", rewardType);
            if(rewards != null)
            {
                PopulateContent(rewards, true);
            }
            return rewards;
        }

        public int HowManyRewards(List<Dictionary<string, object>> parms, long? categoryId = null)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			bool hasCategory = categoryId.GetValueOrDefault() > 0;
			if (hasCategory)
			{
				flags.Add("hasCategory", hasCategory);
				flags.Add("categoryId", categoryId.Value);
			}
			List<long> ids = GetContentIds(parms, flags);
			return ids != null ? ids.Count : 0;
		}

		public List<RewardDef> RetrieveRewardDefsByProperty(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, LWQueryBatchInfo batchInfo, long? categoryId = null)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			bool hasCategory = categoryId.GetValueOrDefault() > 0;
			if (hasCategory)
			{
				flags.Add("hasCategory", hasCategory);
				flags.Add("categoryId", categoryId.Value);
			}
			List<long> ids = GetContentIds(parms, flags);
			List<RewardDef> contents = null;
			if (ids != null && ids.Count > 0)
			{
				contents = GetContent(ids, true, batchInfo);
			}
			return contents;
		}

		public List<long> RetrieveRewardDefIdsByProperty(List<Dictionary<string, object>> parms, string sortExpression, bool ascending)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			List<long> ids = GetContentIds(parms, flags);
			if (!string.IsNullOrEmpty(sortExpression) && ids != null && ids.Count > 0)
			{
				ids = SortContentIds(ids, sortExpression, ascending);
			}
			return ids;
		}

		public List<RewardDef> RetrieveRewardDefsByAttribute(string attName, LWCriterion.Predicate predicate, string value)
		{
			string sql = string.Format(
				"select r.* from LW_RewardsDef r, LW_ContentAttribute ad, LW_ContentAttributeDef a where ad.ContentAttributeDefId = a.ID and ad.RefId = r.Id and ad.ContentType = 'Reward' and a.Name = @0 and ad.Value {0} @1",
				LWCriterion.GetSqlPredicate(predicate));

			var rewards = Database.Fetch<RewardDef>(sql, attName, value);
			if (rewards != null)
			{
				PopulateContent(rewards, true);
			}
			return rewards;
		}

		public List<RewardDef> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<RewardDef>("select * from LW_RewardsDef where UpdateDate >= @0", since);
		}

		public List<RewardDef> RetrieveAll()
		{
			var rewards = Database.Fetch<RewardDef>("select * from LW_RewardsDef");
			if (rewards != null)
			{
				PopulateContent(rewards, true);
			}
			return rewards;
		}

		public List<RewardDef> Retrieve(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<RewardDef>();
            var rewards = RetrieveByArray<long>("select * from LW_RewardsDef where id in (@0)", ids);
			if (rewards != null)
			{
				PopulateContent(rewards, true);
			}
			return rewards;
		}

		public List<long> RetrieveAllIds(string sortExpression, bool ascending)
		{
			string sql = "select id from LW_RewardsDef";
			if (!string.IsNullOrEmpty(sortExpression))
			{
				sql += string.Format(" order by {0} {1}", sortExpression, ascending ? "asc" : "desc");
			}
			return Database.Fetch<long>(sql);
		}

		public List<RewardDef> RetrieveRewardDefsByPointType(string pointTypeName)
		{
			var rewards = Database.Fetch<RewardDef>("select * from LW_RewardsDef where PointType = @0", pointTypeName);
			if (rewards != null)
			{
				PopulateContent(rewards, true);
			}
			return rewards;
		}

		public List<RewardDef> RetrieveRewardDefsByPointEvent(string pointEventName)
		{
			var rewards = Database.Fetch<RewardDef>("select * from LW_RewardsDef where PointEvent = @0", pointEventName);
			if (rewards != null)
			{
				PopulateContent(rewards, true);
			}
			return rewards;
		}

		public List<RewardDef> RetrieveRewardDefsByCategory(long categoryId)
		{
			var rewards = Database.Fetch<RewardDef>("select r.* from LW_RewardsDef r, LW_Product p where p.Id = r.ProductId and p.CategoryId = @0", categoryId);
			if (rewards != null)
			{
				PopulateContent(rewards, true);
			}
			return rewards;
		}

		public void Delete(long rewardId)
		{
			DeleteEntity(rewardId);
		}

		private RewardDef StripSecondsFromDates(RewardDef reward)
		{
			if (reward.CatalogStartDate != null)
			{
				DateTime t = (DateTime)reward.CatalogStartDate;
				reward.CatalogStartDate = DateTimeUtil.StripSecondsFromDates(t);
			}
			if (reward.CatalogEndDate != null)
			{
				DateTime t = (DateTime)reward.CatalogEndDate;
				reward.CatalogEndDate = DateTimeUtil.StripSecondsFromDates(t);
			}
			return reward;
		}

        protected override string HandleSqlForProperties(string propertyName, LWCriterion.Predicate op, object propertyValue, List<object> parameters)
        {
            string sql = string.Empty;

            //some common properties that require special handling
            if (propertyName == "Active")
            {
                if ((bool)propertyValue)
                {
                    sql = string.Format("t.CATALOGSTARTDATE <= @{0} and t.CATALOGENDDATE > @{0}", parameters.Count);
                    parameters.Add(DateTime.Now);
                }
                else
                {
                    //sql = string.Format("t.CATALOGSTARTDATE > @{0} or t.CATALOGENDDATE < @{0}", parameters.Count);
                    sql = "1=1";
                }
                
            }
            else if (propertyName == "Unexpired")
            {
                if ((bool)propertyValue)
                {
                    sql = string.Format("t.CATALOGENDDATE is null or t.CATALOGENDDATE > @{0}", parameters.Count);
                }
                else
                {
                    sql = string.Format("t.CATALOGENDDATE is not null and t.CATALOGENDDATE < @{0}", parameters.Count);
                }
                parameters.Add(DateTime.Now);
            }
            else
            {
                bool propValNotNull = propertyValue != null && !string.IsNullOrEmpty(propertyValue.ToString());
                if (op == LWCriterion.Predicate.Ne)
                {
                    if (propValNotNull)
                    {
                        sql += string.Format("(t.{0} {1} @" + parameters.Count.ToString() + " or t.{0} is null)", propertyName, LWCriterion.GetSqlPredicate(op));
                        parameters.Add(propertyValue);
                    }
                    else
                    {
                        sql += string.Format("t.{0} is not null", propertyName);
                    }
                }
                else if (op == LWCriterion.Predicate.Eq && !propValNotNull)
                {
                    sql += string.Format("(t.{0} is null or t.{0}='')", propertyName);
                }
                else
                {
                    sql += string.Format("t.{0} {1} @{2}", propertyName, LWCriterion.GetSqlPredicate(op), parameters.Count.ToString());
                    parameters.Add(propertyValue);
                }
            }
            return sql;
        }
	}
}
