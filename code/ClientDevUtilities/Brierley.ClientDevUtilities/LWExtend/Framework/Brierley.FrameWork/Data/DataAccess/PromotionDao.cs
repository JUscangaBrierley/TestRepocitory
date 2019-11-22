using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class PromotionDao : ContentDefDaoBase<Promotion>
	{
		public PromotionDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.Promotion)
		{
		}

		public Promotion Retrieve(long id)
		{
			Promotion promotion = GetEntity(id);
			if (promotion != null)
			{
				PopulateContent(promotion);
			}
			return promotion;
		}

		public Promotion RetrieveByCode(string code)
		{
			var ret = Database.FirstOrDefault<Promotion>("select * from LW_Promotion where code = @0", code);
			if (ret != null)
			{
				PopulateContent(ret);
			}
			return ret;
		}

		public Promotion RetrieveByName(string name)
		{
			var ret = Database.FirstOrDefault<Promotion>("select * from LW_Promotion where name = @0", name);
			if (ret != null)
			{
				PopulateContent(ret);
			}
			return ret;
		}

		public List<Promotion> RetrieveAll(LWQueryBatchInfo batchInfo)
		{
			string sql = "select p.* from LW_Promotion p";
			var args = new object[0];
			ApplyBatchInfo(batchInfo, ref sql, ref args);
			var ret = Database.Fetch<Promotion>(sql, args);
			if (ret != null && ret.Count > 0)
			{
				PopulateContent(ret, true);
			}
			return ret;
		}

		public List<Promotion> Retrieve(long[] ids)
		{
            if (ids == null || ids.Length == 0)
                return new List<Promotion>();
            return RetrieveByArray<long>("select * from LW_Promotion where id in (@0)", ids);
		}

		public List<Promotion> RetrieveChangedObjects(DateTime since)
		{
			return Database.Fetch<Promotion>("select * from LW_Promotion where UpdateDate >= @0", since);
		}

		public int HowManyPromotions(List<Dictionary<string, object>> parms)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			List<long> ids = GetContentIds(parms, flags);
			return ids != null ? ids.Count : 0;
		}

		public List<long> RetrievePromotionIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId = null)
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

		public List<Promotion> RetrievePromotions(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			List<long> ids = GetContentIds(parms, flags);
			List<Promotion> promotions = null;
			if (ids != null && ids.Count > 0)
			{
				promotions = GetContent(ids, populateAttributes, batchInfo);
			}
			return promotions;
		}

		public void Delete(long promotionId)
		{
			DeleteEntity(promotionId);
		}

		protected override string HandleSqlForProperties(string propertyName, LWCriterion.Predicate op, object propertyValue, List<object> parameters)
		{
			string sql = string.Empty;
			if (propertyName == "Active")
			{
				if ((bool)propertyValue)
				{
					sql = string.Format("t.StartDate <= @{0} and t.EndDate > @{0}", parameters.Count);
				}
				else
				{
					sql = string.Format("t.StartDate > @{0} or t.EndDate < @{0}", parameters.Count);
				}
				parameters.Add(DateTime.Now);
			}
			else if (propertyName == "Unexpired")
			{
				if ((bool)propertyValue)
				{
					sql = string.Format("t.EndDate is null or t.EndDate > @{0}", parameters.Count);
				}
				else
				{
					sql = string.Format("not t.EndDate is null and t.EndDate < 2{0}", parameters.Count);
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
