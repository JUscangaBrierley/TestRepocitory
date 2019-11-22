using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ContentDefDaoBase<T> : DaoBase<T> where T : ContentDefBase
	{
		private const string _className = "ContentDefDaoBase";

		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private string _tableName;
		private string _primaryKey;
		private LangChanContentDao _langChanContentDao;
		private ContentAttributeDao _contentAttributeDao;
		private ContentObjType _contentType;

		public string TableName
		{
			get
			{
				if (_tableName == null)
				{
					Type t = typeof(T);
					var attribute = t.GetCustomAttributes(typeof(TableNameAttribute), true);
					if (attribute == null || attribute.Length == 0)
					{
						throw new Exception("Could not determine table name for type " + t.Name + ". Please ensure the type contains a valid TableName attribute.");
					}
					_tableName = (attribute[0] as TableNameAttribute).Value;
				}
				return _tableName;
			}
		}

		public string PrimaryKey
		{
			get
			{
				if (_primaryKey == null)
				{
					Type t = typeof(T);
					var attribute = t.GetCustomAttributes(typeof(PrimaryKeyAttribute), true);
					if (attribute == null || attribute.Length == 0)
					{
						throw new Exception("Could not determine the primary key for type " + t.Name + ". Please ensure the type contains a valid PrimaryKey attribute.");
					}
					_primaryKey = (attribute[0] as PrimaryKeyAttribute).Value;
				}
				return _primaryKey;
			}
		}

		//todo: would be nice if we could get the content type from the generic T, but for now we'll take it as a parameter
		protected ContentDefDaoBase(Database database, ServiceConfig config, LangChanContentDao langChanContentDao, ContentAttributeDao contentAttributeDao, ContentObjType contentType)
			: base(database, config)
		{
			_langChanContentDao = langChanContentDao;
			_contentAttributeDao = contentAttributeDao;
			_contentType = contentType;
		}

		public override void Create(T t)
		{
			using (var txn = Database.GetTransaction())
			{
				SaveEntity(t);
				ContentDefBase content = t as ContentDefBase;
				foreach (string name in content.Contents.Keys)
				{
					foreach (var lc in content.Contents[name])
					{
						lc.RefId = content.Id;
						lc.CreateDate = DateTime.Now;
						lc.UpdateDate = DateTime.Now;
						_langChanContentDao.Create(lc);
					}
				}
				foreach (ContentAttribute att in content.Attributes)
				{
					att.ContentType = content.ContentType;
					att.CreateDate = DateTime.Now;
					att.UpdateDate = DateTime.Now;
					att.RefId = content.Id;
					_contentAttributeDao.Create(att);
				}
				txn.Complete();
			}
		}

		public virtual void Update(T t, bool deep = true)
		{
			using (var txn = Database.GetTransaction())
			{
				UpdateEntity(t);
				if (deep)
				{
					ContentDefBase content = t as ContentDefBase;
					_langChanContentDao.Delete(content.ContentType, content.Id);
					foreach (string name in content.Contents.Keys)
					{
						foreach (var lc in content.Contents[name])
						{
							lc.RefId = content.Id;
							lc.CreateDate = DateTime.Now;
							lc.UpdateDate = DateTime.Now;
							_langChanContentDao.Create(lc);
						}
					}
					_contentAttributeDao.DeleteByRefId(content.ContentType, content.Id);
					foreach (ContentAttribute att in content.Attributes)
					{
						att.ContentType = content.ContentType;
						att.CreateDate = DateTime.Now;
						att.UpdateDate = DateTime.Now;
						att.RefId = content.Id;
						_contentAttributeDao.Create(att);
					}
				}
				txn.Complete();
			}
		}


		public virtual void Update(List<T> ts, bool deep = true)
		{
			ITransaction txn = Database.GetTransaction();
			try
			{
				int bulkLoadingBatchSize = Config.BulkLoadingBatchSize.GetValueOrDefault(100);
				for (int i = 0; i < ts.Count; i++)
				{
					T t = ts[i];
					Update(t, deep);
					if (i != 0 && bulkLoadingBatchSize > 0 && i % bulkLoadingBatchSize == 0)
					{
						txn.Complete();
						txn.Dispose();
						txn = Database.GetTransaction();
					}
				}
				if (txn != null)
				{
					txn.Complete();
					txn.Dispose();
					txn = null;
				}
			}
			catch
			{
				if (txn != null)
				{
					txn.Dispose();
				}
				throw;
			}
		}

		/// <summary>
		/// TODO: This method might give a set that is larger than the desired result.  If you have attributes that 
		/// have same values and are included in the comparison, the result might require further reduction.
		/// Also, this would not allow OR condition between the attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parms"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public List<long> GetContentIds(
			List<Dictionary<string, object>> parms,
			Dictionary<string, object> flags)
		{
			const string methodName = "GetContentIds";

			var attributeConditions = from x in parms where x.ContainsKey("IsAttribute") && (bool)x["IsAttribute"] select x;
			var otherConditions = from x in parms where !x.ContainsKey("IsAttribute") || !(bool)x["IsAttribute"] select x;
			bool hasAttributes = attributeConditions.Count() > 0;

			Dictionary<string, object> queryParms = new Dictionary<string, object>();


			var parameters = new List<object>();
			string sql = "select t.Id from " + TableName + " t";
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
				parameters.Add(_contentType.ToString());

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

			Stopwatch timer = new Stopwatch();
			timer.Start();
			List<long> result = Database.Fetch<long>(sql, parameters.ToArray());
			timer.Stop();
			long count = result != null ? result.Count : 0;
			_logger.Debug(_className, methodName, string.Format("Retrieved {0} ids in {1} ms.", count, timer.ElapsedMilliseconds));
			return result;
		}

		public List<long> SortContentIds(IList<long> ids, string sortExpression, bool ascending)
		{
			Dictionary<string, object> queryParms = new Dictionary<string, object>();
			string sql = string.Format("select m.Id from {0} m where m.Id in (@ids) order by m.{1} {2}", TableName, sortExpression, ascending ? "asc" : "desc");
			return Database.Fetch<long>(sql, new { ids = ids.ToArray() });
		}

		public List<T> GetContent(IList<long> ids, bool populateAttributes, LWQueryBatchInfo batchInfo)
		{
			if (ids == null || ids.Count == 0)
				return new List<T>();

			long[] origIds = ids.ToArray();

			int idsRemaining = ids.Count;
			int startIndex = 0;
			List<T> contents = new List<T>();
			while (idsRemaining > 0)
			{
				long[] refIds = LimitInClauseList<long>(origIds, ref startIndex, ref idsRemaining);
				string sql = string.Format("select a.* from {0} a where {1} in (@refIds) order by {1}", TableName, PrimaryKey);
				var args = new object[] { new { refIds = refIds } };
				ApplyBatchInfo(batchInfo, ref sql, ref args);

				var set = Database.Fetch<T>(sql, args);
				if (set != null && set.Count > 0)
				{
					PopulateContent(set, populateAttributes);
					contents.AddRange(set);
				}
			}
			return contents;
		}
		
		public Dictionary<long, string> RetrieveByProperty(string propertyName, string whereClause)
		{
			string sql = string.Format("select Id as TheKey, {0} as TheValue from {1}", propertyName, TableName);
			if (!string.IsNullOrEmpty(whereClause))
			{
				sql += " where " + whereClause;
			}
			return MakeDictionary(Database.Fetch<dynamic>(sql.ToUpper()));
		}

		protected virtual string HandleSqlForProperties(string propertyName, LWCriterion.Predicate op, object propertyValue, List<object> parameters)
		{
			string sql = string.Empty;

			//some common properties that require special handling
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
			else if (propertyName == "Unexpired")
			{
				if ((bool)propertyValue)
				{
					sql = string.Format("t.ExpiryDate is null or t.ExpiryDate > @{0}", parameters.Count);
				}
				else
				{
					sql = string.Format("t.ExpiryDate is not null and t.ExpiryDate < @{0}", parameters.Count);
				}
				parameters.Add(DateTime.Now);
			}
			else if (propertyName == "FolderId")
			{
				sql = string.Format("t.FolderId {0} @{1}", LWCriterion.GetSqlPredicate(op), parameters.Count);
				parameters.Add(propertyValue);
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

		protected override void DeleteEntity(long id)
		{
			using (var txn = Database.GetTransaction())
			{
				_langChanContentDao.Delete(_contentType, id);
				_contentAttributeDao.DeleteByRefId(_contentType, id);
				base.DeleteEntity(id);
				txn.Complete();
			}
		}

		protected void DeleteContent(T t)
		{
			ContentDefBase content = t as ContentDefBase;
			if (content != null)
			{
				using (var txn = Database.GetTransaction())
				{
					_langChanContentDao.Delete(content.ContentType, content.Id);
					_contentAttributeDao.DeleteByRefId(content.ContentType, content.Id);
					DeleteEntity(t);
					txn.Complete();
				}
			}
		}

		protected void PopulateContent(T t)
		{
			ContentDefBase content = (ContentDefBase)t;
			List<LangChanContent> contentList = _langChanContentDao.Retrieve(content.ContentType, content.Id);
			content.Contents.Clear();
			foreach (LangChanContent lc in contentList)
			{
				List<LangChanContent> list = null;
				if (content.Contents.ContainsKey(lc.Name))
				{
					list = content.Contents[lc.Name];
				}
				else
				{
					list = new List<LangChanContent>();
					content.Contents.Add(lc.Name, list);
				}
				list.Add(lc);
			}
			List<ContentAttribute> atts = _contentAttributeDao.RetrieveByRefId(content.ContentType, content.Id);
			if (atts != null)
			{
				content.Attributes = atts;
			}
		}

		protected void PopulateContent(IEnumerable<T> contentObjs, bool populateAttributes)
		{
			IDictionary<long, T> contentMap = new Dictionary<long, T>();
			var contentIdList = new List<long>();
			foreach (T t in contentObjs)
			{
				ContentDefBase content = t as ContentDefBase;
				contentMap.Add(content.Id, t);
				contentIdList.Add(content.Id);
			}
			if (contentIdList.Count == 0)
			{
				return;
			}
			long[] contentIds = contentIdList.ToArray();
			List<LangChanContent> contents = _langChanContentDao.Retrieve(_contentType, contentIds);
			if (contents != null && contents.Count > 0)
			{
				foreach (var content in contents)
				{
					ContentDefBase r = contentMap[content.RefId] as ContentDefBase;
					List<LangChanContent> list = null;
					if (r.Contents.ContainsKey(content.Name))
					{
						list = r.Contents[content.Name];
					}
					else
					{
						list = new List<LangChanContent>();
						r.Contents.Add(content.Name, list);
					}
					list.Add(content);
				}
			}
			if (populateAttributes)
			{
				List<ContentAttribute> atts = _contentAttributeDao.RetrieveByRefIds(_contentType, contentIds);
				if (atts != null)
				{
					foreach (ContentAttribute ra in atts)
					{
						ContentDefBase r = contentMap[ra.RefId] as ContentDefBase;
						r.Attributes.Add(ra);
					}
				}
			}
		}

		private Dictionary<long, string> MakeDictionary(List<dynamic> list)
		{
			Dictionary<long, string> ret = new Dictionary<long, string>();
			foreach (dynamic d in list)
			{
				ret.Add((long)d.THEKEY, d.THEVALUE.ToString());
			}
			return ret;
		}
	}
}
