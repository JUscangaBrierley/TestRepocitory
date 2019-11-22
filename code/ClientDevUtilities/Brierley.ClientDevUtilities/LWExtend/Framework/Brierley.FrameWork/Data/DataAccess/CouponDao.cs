using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class CouponDao : ContentDefDaoBase<CouponDef>
	{
		public CouponDao(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao)
			: base(database, config, langChanContentDao, contentAttributeDao, Common.ContentObjType.Coupon)
		{
		}

		public CouponDef Retrieve(long id)
		{
			CouponDef coupon = GetEntity(id);
			if (coupon != null)
			{
				PopulateContent(coupon);
			}
			return coupon;
		}

		public List<CouponDef> Retrieve(long[] idList, bool populateAttributes)
		{
			List<long> ids = new List<long>(idList);
			List<CouponDef> contents = null;
			if (ids != null && ids.Count > 0)
			{
				contents = GetContent(ids, populateAttributes, null);
			}
			return contents;
		}

		public CouponDef Retrieve(string name)
		{
			var ret = Database.FirstOrDefault<CouponDef>("select * from LW_CouponDef where Name = @0", name);
			if (ret != null)
			{
				PopulateContent(ret);
			}
			return ret;
		}

		public CouponDef RetrieveByCode(string code)
		{
			var ret = Database.FirstOrDefault<CouponDef>("select * from LW_CouponDef where CouponTypeCode = @0", code);
			if (ret != null)
			{
				PopulateContent(ret);
			}
			return ret;
		}

		public List<CouponDef> RetrieveByCategory(long categoryId)
		{
			var ret = Database.Fetch<CouponDef>("select * from LW_CouponDef where CategoryId = @0 and StartDate <= @1 and ExpiryDate > @1", categoryId, DateTime.Now);
			Populate(ret);
			return ret;
		}

		public List<CouponDef> RetrieveChangedObjects(DateTime since)
		{
			var ret = Database.Fetch<CouponDef>("select * from LW_CouponDef where UpdateDate >= @0", since);
			Populate(ret);
			return ret;
		}

		public List<CouponDef> RetrieveAll()
		{
			var ret = Database.Fetch<CouponDef>("select * from LW_CouponDef");
			Populate(ret);
			return ret;
		}

		[Obsolete("This method is deprecated. Use: Count(List<Dictionary<string, object>> parms, ActiveCouponOptions options = null) instead")]
		public int HowManyCoupons(List<Dictionary<string, object>> parms)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			IList<long> ids = GetContentIds(parms, flags);
			return ids != null ? ids.Count : 0;
		}

		public int Count(List<Dictionary<string, object>> parms, ActiveCouponOptions options = null)
		{
			//todo: not the most efficient way to count - we're executing the count and then getting the first page when we only need the count...
			//This method may be of no use, though, because the new methods for retrieving definitions will return the count with each page. This method
			//is likely used along with the old "LWQueryBatchInfo" technique because it did not provide a total count, so you need to get a count up front in order
			//to know the number of pages to display
			Dictionary<string, object> flags = new Dictionary<string, object>();
			var page = GetContent(parms, flags, options, false, 1, 1);
			return Convert.ToInt32(page.TotalItems);
		}

		public List<long> RetrieveCouponDefIds(List<Dictionary<string, object>> parms, string sortExpression, bool ascending, long? folderId = null)
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

		[Obsolete("This method is deprecated. Use Retrieve(List<Dictionary<string, object>> parms, ActiveCouponOptions options, bool populateAttributes, long? memberId) instead")]
		public List<CouponDef> RetrieveCouponDefs(List<Dictionary<string, object>> parms, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			List<long> ids = GetContentIds(parms, flags);
			List<CouponDef> contents = null;
			if (ids != null && ids.Count > 0)
			{
				contents = GetContent(ids, populateAttributes, batchInfo);
			}
			return contents;
		}

		//new in 4.7.1 - coupons with ActiveCouponOption, no paging
		public List<CouponDef> Retrieve(List<Dictionary<string, object>> parms, ActiveCouponOptions options, bool populateAttributes, long? memberId)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			var page = GetContent(parms, flags, options, populateAttributes, null, null);
			return page.Items;
		}

		//another overload with page parameters, returning PetaPoco.Page
		public PetaPoco.Page<CouponDef> Retrieve(List<Dictionary<string, object>> parms, ActiveCouponOptions options, bool populateAttributes, long? memberId, int? pageNumber, int? resultsPerPage)
		{
			Dictionary<string, object> flags = new Dictionary<string, object>();
			var page = GetContent(parms, flags, options, populateAttributes, pageNumber, resultsPerPage);
			return page;
		}

		public void Delete(long couponId)
		{
			DeleteEntity(couponId);
		}

		protected override void SaveEntity(object entity)
		{
			var coupon = (CouponDef)entity;
			using (var txn = Database.GetTransaction())
			{
				base.SaveEntity(coupon);
				txn.Complete();
			}
		}

		protected override void UpdateEntity(object entity)
		{
			var coupon = (CouponDef)entity;
			using (var txn = Database.GetTransaction())
			{
				base.UpdateEntity(entity);
				txn.Complete();
			}
		}

		private void Populate(List<CouponDef> coupons)
		{
			if (coupons != null)
			{
				PopulateContent(coupons, true);
			}
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

		/// <summary>
		/// fetches the specified page or all if no page is specified
		/// </summary>
		/// <param name="parms"></param>
		/// <param name="flags"></param>
		/// <param name="options"></param>
		/// <param name="populateAttributes"></param>
		/// <param name="pageNumber"></param>
		/// <param name="resultsPerPage"></param>
		/// <returns></returns>
		private PetaPoco.Page<CouponDef> GetContent(
			List<Dictionary<string, object>> parms,
			Dictionary<string, object> flags, ActiveCouponOptions options, bool populateAttributes, int? pageNumber, int? resultsPerPage)
		{
			if (pageNumber == null ^ resultsPerPage == null)
			{
				throw new ArgumentException("You must supply a value for both pageNumber and resultsPerPage or null for both");
			}

			//todo: much of this can probably be combined with the base DAO's GetContentIds
			var attributeConditions = from x in parms where x.ContainsKey("IsAttribute") && (bool)x["IsAttribute"] select x;
			var otherConditions = from x in parms where !x.ContainsKey("IsAttribute") || !(bool)x["IsAttribute"] select x;
			bool hasAttributes = attributeConditions.Count() > 0;

			Dictionary<string, object> queryParms = new Dictionary<string, object>();

			var parameters = new List<object>();

			// change - id to *
			//    string sql = "select t.Id from " + TableName + " t";
			string sql = "select t.* from " + TableName + " t";
			bool hasWhere = false;

			Dictionary<string, string> joinMap = null;
			Func<string, string> getJoinAlias = (string attributeName) =>
			{
				if (joinMap.ContainsKey(attributeName))
				{
					return joinMap[attributeName];
				}
				string ret = Regex.Replace(attributeName, "[^a-zA-Z]", string.Empty);
				if (ret.Length == 0)
				{
					ret = "a";
				}
				if (ret.Length > 2)
				{
					ret = ret.Substring(0, 2);
				}
				ret += "_";
				if (joinMap.ContainsValue(ret))
				{
					int i = 0;
					while (joinMap.ContainsValue(ret + (++i).ToString()))
					{
					}
					ret += i.ToString();
				}
				return ret;
			};

			if (hasAttributes)
			{
				parameters.Add(ContentObjType.Coupon.ToString());

				joinMap = new Dictionary<string, string>();
				foreach (var condition in attributeConditions)
				{
					string attribute = (string)condition["Property"];
					if (!joinMap.ContainsKey(attribute))
					{
						joinMap.Add(attribute, getJoinAlias(attribute));
						sql += string.Format(
@"
left join (
	select a.RefId, a.AttributeValue, ad.Name 
	from LW_ContentAttribute a 
	inner join LW_ContentAttributeDef ad on a.ContentAttributeDefId = ad.ID and a.ContentType = @0 
	where ad.Name = @{0} 
) {1} on {1}.RefId = t.Id",
						  parameters.Count,
						  joinMap[attribute]);

						parameters.Add(attribute);
					}
				}
			}

			bool first = true;
			foreach (var cond in otherConditions)
			{
				if (first)
				{
					if (!hasWhere)
					{
						sql += " where ";
						hasWhere = true;
					}
					else
					{
						sql += " and ";
					}
					first = false;
				}
				else
				{
					string op = cond.ContainsKey("Operator") ? cond["Operator"].ToString() : " and ";
					sql += string.Format(" {0} ", op);
				}
				object propName = cond["Property"];
				object propVal = cond["Value"];
				LWCriterion.Predicate predicate = (LWCriterion.Predicate)cond["Predicate"];

				sql += "(";
				sql += HandleSqlForProperties((string)cond["Property"], (LWCriterion.Predicate)cond["Predicate"], cond["Value"], parameters);
				sql += ")";
			}

			if (flags.ContainsKey("categoryId"))
			{
				string categoryID = flags["categoryId"].ToString();
				if (!hasWhere)
				{
					sql += " where ";
				}
				else
				{
					sql += " and ";
				}
				sql += "t.productid in (select p.id from LW_PRODUCT p where p.categoryid = " + categoryID + ")";
			}

			if (hasAttributes)
			{
				first = true;
				sql += hasWhere ? " and (" : " where (";

				foreach (var cond in attributeConditions)
				{
					if (!first)
					{
						sql += string.Format(" {0} ", cond.ContainsKey("Operator") ? cond["Operator"].ToString() : "and");
					}
					first = false;

					sql += string.Format(
						"{0}.AttributeValue {1} @{2}",
						getJoinAlias((string)cond["Property"]),
						LWCriterion.GetSqlPredicate((LWCriterion.Predicate)cond["Predicate"]),
						parameters.Count);
					parameters.Add(cond["Value"]);
				}
				sql += ")";
			}

			if (options != null)
			{
				// afraid it won't be this easy
				switch (options.RestrictGlobalCoupons)
				{
					case GlobalCouponRestriction.None:
						break;
					case GlobalCouponRestriction.RestrictGlobal:
						sql += hasWhere ? " and (" : " where (";
						sql += "IsGlobal = 0";
						sql += ")";
						break;
					case GlobalCouponRestriction.RestrictNonGlobal:
						sql += hasWhere ? " and (" : " where (";
						sql += "IsGlobal = 1";
						sql += ")";
						break;
				}

				if (options.RestrictDate.HasValue)
				{
					sql += hasWhere ? " and (" : " where (";
					sql += string.Format("StartDate <= @{0} and ExpiryDate > @{0}", parameters.Count);
					sql += ")";
					parameters.Add(options.RestrictDate.Value);
				}

			}

			PetaPoco.Page<CouponDef> result = Database.Page<CouponDef>(pageNumber.Value, resultsPerPage.Value, sql, parameters.ToArray());
			PopulateContent(result.Items, populateAttributes);
			return result;
		}
	}
}
