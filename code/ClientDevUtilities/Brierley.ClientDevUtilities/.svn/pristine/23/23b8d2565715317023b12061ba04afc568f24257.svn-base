using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;
using PetaPoco.Internal;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class ClientDataObjectDao : DaoBase<ClientDataObject>
	{
		//these map our client data object types back to generic petapoco methods for retrieval. 
		private static Dictionary<Type, MethodInfo> _pagedMethodMap = new Dictionary<Type, MethodInfo>();
		private static Dictionary<Type, MethodInfo> _fetchMethodMap = new Dictionary<Type, MethodInfo>();
		private static Dictionary<Type, MethodInfo> _firstOrDefaultMethodMap = new Dictionary<Type, MethodInfo>();
		private static Dictionary<Type, string> _tableNameMap = new Dictionary<Type, string>();

		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private const string _className = "ClientDataObjectDao";

		public AtsLockDao AtsLockDao { get; set; }

		public ClientDataObjectDao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public void Create(IClientDataObject cobj)
		{
			cobj.CreateDate = DateTime.Now;
			cobj.UpdateDate = DateTime.Now;
			SaveEntity(cobj);
		}

		public void Create(List<IClientDataObject> ts)
		{
			Create(ts.ConvertAll(o => (ClientDataObject)o));
		}

		public void Update(IClientDataObject cobj)
		{
			cobj.UpdateDate = DateTime.Now;
			UpdateEntity(cobj);
		}

		public void Update(List<IClientDataObject> ts)
		{
			Update(ts.ConvertAll(o => (ClientDataObject)o));
		}


		public bool TableExists(string attSetName)
		{
			//todo: implementation is assuming an exception means the table does not exist. But what if the table does exist and
			//the exception is for another reason? If possible, change this to something more reliable.
			bool exists = true;
			string queryStr = string.Format("select count(*) from ATS_{0}", attSetName);
			try
			{
				Database.ExecuteScalar<long>(queryStr);
			}
			catch
			{
				// the table *probably* does not exist.
				exists = false;
			}
			return exists;
		}

		public bool IsEmpty(string attSetName)
		{
			//todo: same as above. Exception is eaten and we inform the caller that the table is empty.
			bool isEmpty = true;
			string sql = string.Format("select count(*) from ATS_{0}", attSetName);
			try
			{
				long count = Database.ExecuteScalar<long>(sql);
				isEmpty = count == 0;
			}
			catch
			{
			}
			return isEmpty;
		}

		
		#region New Generic Methods

		public T Retrieve<T>(long rowkey, LockingMode lockMode) where T : IClientDataObject
		{
            return Database.FirstOrDefault<T>(string.Format("{0} where A_RowKey = @0{1}", WithUpdateClause(lockMode), ForUpdateClause(lockMode)), rowkey);
		}

		public List<T> Retrieve<T>(long[] rowkeys, LockingMode lockMode) where T : IClientDataObject
		{
            if (rowkeys == null || rowkeys.Length == 0)
                return new List<T>();
			string sql = string.Format("{0} where A_RowKey in (@keys){1}", WithUpdateClause(lockMode), ForUpdateClause(lockMode));
			return Database.Fetch<T>(sql, new { keys = rowkeys });
		}

		public List<T> Retrieve<T>(IAttributeSetContainer owner, LockingMode lockMode) where T : IClientDataObject
		{
			return Retrieve<T>(owner, string.Empty, new EvaluatedCriterion(), null, null, lockMode);
		}

		public List<long> RetrieveChangedObjects<T>(DateTime since) where T : IClientDataObject
		{
			string sql = string.Format("select A_RowKey from {0} where UpdateDate >= @0", GetTableName(typeof(T)));
			return Database.Fetch<long>(sql, since);
		}

		public List<long> RetrieveIds<T>(AttributeSetContainer[] owners, string alias, EvaluatedCriterion whereClause, string orderByClause) where T : IClientDataObject
		{
			return RetrieveIds(typeof(T), owners, alias, whereClause, orderByClause);
		}

		public List<T> Retrieve<T>(IAttributeSetContainer owner, string alias, string whereClause, string orderByClause, LWQueryBatchInfo batchInfo, LockingMode lockMode) where T : IClientDataObject
		{
			ATSLock lockObj = ObtainLock(typeof(T).Name, batchInfo, lockMode);
			string sql = BuildSelectStatement(GetTableName(typeof(T)), owner, alias, whereClause, orderByClause, batchInfo == null ? lockMode : LockingMode.None);
			List<T> list = null;
			object[] args = owner != null ? new object[]{ owner.MyKey } : new object[0];
			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}
			list = Database.Fetch<T>(sql, args);
			if (lockObj != null)
			{
				AtsLockDao.UpdateLock(lockObj);
			}
			ClearDirtyFlags(list);
			return list;
		}

		public List<T> Retrieve<T>(AttributeSetContainer[] owners, string alias, EvaluatedCriterion whereClause, string orderByClause, LWQueryBatchInfo batchInfo, LockingMode lockMode) where T : IClientDataObject
		{
			ATSLock lockObj = ObtainLock(typeof(T).Name, batchInfo, lockMode);
			string where = whereClause != null ? whereClause.Where : null;
			string sql = BuildSelectStatement(GetTableName(typeof(T)), owners, alias, where, orderByClause, batchInfo == null ? lockMode : LockingMode.None);

			List<T> list = null;

			List<object> parameters = new List<object>();

			int parameterIndex = 0;
			if (owners != null && owners.Length > 0)
			{
				var keys = new List<long>();
				for (int i = 0; i < owners.Length; i++)
				{
					keys.Add(owners[i].MyKey);
				}
				parameters.Add(new { keys = keys });
				parameterIndex++;
			}

			if (whereClause != null)
			{
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					sql = sql.Replace(":" + parm.Key, "@" + parameterIndex.ToString());
					parameters.Add(parm.Value);
				}
			}

			var args = parameters.ToArray();

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}
			list = Database.Fetch<T>(sql, args);

			if (lockObj != null)
			{
				AtsLockDao.UpdateLock(lockObj);
			}

			ClearDirtyFlags(list);

			return list;
		}

		public List<T> Retrieve<T>(IAttributeSetContainer owner, string alias, EvaluatedCriterion whereClause, string orderByClause, LWQueryBatchInfo batchInfo, LockingMode lockMode) where T : IClientDataObject
		{
			ATSLock lockObj = ObtainLock(typeof(T).Name, batchInfo, lockMode);
			string where = whereClause != null ? whereClause.Where : null;
			string sql = BuildSelectStatement(GetTableName(typeof(T)), owner, alias, where, orderByClause, batchInfo == null ? lockMode : LockingMode.None);

			int parameterIndex = 0;
			var parameters = new List<object>();
			if(owner != null)
			{
				parameters.Add(owner.MyKey);
				parameterIndex++;
			}
			
			if (whereClause != null)
			{
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					sql = sql.Replace(":" + parm.Key, "@" + parameterIndex.ToString());
					parameters.Add(parm.Value);
					parameterIndex++;
				}
			}

			var args = parameters.ToArray();

			List<T> list = null;

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}
			list = Database.Fetch<T>(sql, args);

			if (lockObj != null)
			{
				AtsLockDao.UpdateLock(lockObj);
			}

			ClearDirtyFlags(list);

			return list;
		}

		public List<IClientDataObject> Retrieve<T>(AttributeSetContainer[] owners, string alias, EvaluatedCriterion whereClause, IList<string> distinct, string orderByClause, LWQueryBatchInfo batchInfo, LockingMode lockMode)
		{
			string tableName = GetTableName(typeof(T));
			ATSLock lockObj = ObtainLock(typeof(T).Name, batchInfo, lockMode);

			List<T> list = null;
            string sql = string.Format("select {1}.* from {0} {1} where A_RowKey in (select min(A_RowKey) from {0} where {{0}})", tableName, string.IsNullOrEmpty(alias) ? "a" : alias);

			string where = whereClause != null && !string.IsNullOrEmpty(whereClause.Where) ? whereClause.Where : string.Empty;
			var parameters = new List<object>();
			int parameterIndex = 0;
			if (whereClause != null)
			{
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					where = where.Replace(":" + parm.Key, "@" + parameterIndex.ToString());
					parameters.Add(parm.Value);
					parameterIndex++;
				}
			}
						
			if (owners != null && owners.Length > 0)
			{
				if (!string.IsNullOrEmpty(where))
				{
					where += " and ";
				}
				where += ResolveKeyfieldName(owners[0]) + " in (@keys)";

				long[] keys = new long[owners.Length];
				for (int i = 0; i < owners.Length; i++)
				{
					keys[i] = owners[i].MyKey;
				}
				parameters.Add(new { keys = keys });
				parameterIndex++;
			}

			var args = parameters.ToArray();

			string groupBy = distinct != null && distinct.Count > 0 ? " group by " + string.Join(", ", distinct.ToArray()) : string.Empty;
            string orderBy = " " + orderByClause;
			sql = string.Format(sql, where + groupBy + orderBy);

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}

			list = Database.Fetch<T>(sql, args);

			if (lockObj != null)
			{
				AtsLockDao.UpdateLock(lockObj);
			}

			ClearDirtyFlags(list);

			return list.OfType<IClientDataObject>().ToList();
		}

		#endregion




		public IClientDataObject Retrieve(Type type, long rowkey, LockingMode lockMode)
		{
			string sql = string.Format("select * from {0}{1} where A_RowKey = @0{2}", GetTableName(type), WithUpdateClause(lockMode), ForUpdateClause(lockMode));
			MethodInfo genericMethod = GetGenericFirstOrDefaultMethod(type);
			var ret = genericMethod.Invoke(Database, new object[] { sql, new object[] { rowkey } });
			return ret as IClientDataObject;
		}

		
		public List<IClientDataObject> Retrieve(Type type, long[] rowKeys, LockingMode lockMode)
		{
			string sql = string.Format("select * from {0}{1} where A_RowKey in (@keys){2}", GetTableName(type), WithUpdateClause(lockMode), ForUpdateClause(lockMode));
			MethodInfo genericMethod = GetGenericFetchMethod(type);
			var list = genericMethod.Invoke(Database, new object[] { sql, new object[] { new { keys = rowKeys } } }) as IEnumerable<object> ?? new List<IClientDataObject>();
			ClearDirtyFlags(list);
			return list.OfType<IClientDataObject>().ToList();
		}


		public List<IClientDataObject> Retrieve(Type type, IAttributeSetContainer owner, LockingMode lockMode)
		{
			return Retrieve(type, owner, string.Empty, new EvaluatedCriterion(), string.Empty, null, lockMode);
		}


		public List<long> RetrieveChangedObjects(Type type, DateTime since)
		{
			string sql = string.Format("select A_RowKey from {0} where UpdateDate >= @0", GetTableName(type));
			return Database.Fetch<long>(sql, since);
		}

		
		public List<long> RetrieveIds(Type type, AttributeSetContainer[] owners, string alias, EvaluatedCriterion whereClause, string orderByClause)
		{
			string method = "RetrieveIds";

			string sql = string.Format("select A_RowKey from {0} {1}", GetTableName(type), alias ?? string.Empty);
			
			if (whereClause != null && !string.IsNullOrEmpty(whereClause.Where))
			{
				sql += " where " + whereClause.Where;
			}

			if (!string.IsNullOrEmpty(orderByClause))
			{
				sql += " order by " + orderByClause;
			}

			List<object> parameters = new List<object>();
			int parameterIndex = 0;

			if (owners != null && owners.Length > 0)
			{
				sql += whereClause != null && !string.IsNullOrEmpty(whereClause.Where) ? " and " : " where ";

				sql += ResolveKeyfieldName(owners[0]) + " in (@keys)";

				long[] keys = new long[owners.Length];
				for (int i = 0; i < owners.Length; i++)
				{
					keys[i] = owners[i].MyKey;
				}
				parameters.Add(new { keys = keys });
				parameterIndex++;
			}

			_logger.Debug(_className, method, "Executing query: " + sql);

			if (whereClause != null)
			{
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					sql = sql.Replace(":" + parm.Key, "@" + parameterIndex.ToString());
					parameters.Add(parm.Value);
				}
			}

			var args = parameters.ToArray();
			return Database.Fetch<long>(sql, args);
		}


		public List<IClientDataObject> Retrieve(Type type, IAttributeSetContainer owner, string alias, string whereClause, string orderByClause, LWQueryBatchInfo batchInfo, LockingMode lockMode)
		{
			ATSLock lockObj = ObtainLock(type.Name, batchInfo, lockMode);
			if (batchInfo == null)
			{
				lockMode = LockingMode.None;
			}
			string sql = BuildSelectStatement(type, owner, alias, whereClause, orderByClause, batchInfo == null ? lockMode : LockingMode.None);

			IEnumerable<object> list = null;

			var args = owner == null ? new object[0] : new object[] { owner.MyKey };

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}
			MethodInfo genericMethod = GetGenericFetchMethod(type);
			list = genericMethod.Invoke(Database, new object[] { sql, args }) as IEnumerable<object> ?? new List<IClientDataObject>();

			if (lockObj != null)
			{
				AtsLockDao.UpdateLock(lockObj);
			}

			ClearDirtyFlags(list);

			return list.OfType<IClientDataObject>().ToList();
		}
		
		
		public List<IClientDataObject> Retrieve(Type type, AttributeSetContainer[] owners, string alias, EvaluatedCriterion whereClause, string orderByClause, LWQueryBatchInfo batchInfo, LockingMode lockMode)
		{
			ATSLock lockObj = ObtainLock(type.Name, batchInfo, lockMode);
			string where = whereClause != null ? whereClause.Where : null;
			string sql = BuildSelectStatement(GetTableName(type), owners, alias, where, orderByClause, batchInfo == null ? lockMode : LockingMode.None);

			IEnumerable<object> list = null;

			List<object> parameters = new List<object>();

			int parameterIndex = 0;
			if (owners != null && owners.Length > 0)
			{
				var keys = new List<long>();
				for (int i = 0; i < owners.Length; i++)
				{
					keys.Add(owners[i].MyKey);
				}
				parameters.Add(new { keys = keys });
				parameterIndex++;
			}

			if (whereClause != null)
			{
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					sql = sql.Replace(":" + parm.Key, "@" + parameterIndex++.ToString());
					parameters.Add(parm.Value);
				}
			}

			var args = parameters.ToArray();

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}
			MethodInfo genericMethod = GetGenericFetchMethod(type);
			list = genericMethod.Invoke(Database, new object[] { sql, args }) as IEnumerable<object> ?? new List<IClientDataObject>();

			if (lockObj != null)
			{
				AtsLockDao.UpdateLock(lockObj);
			}

			ClearDirtyFlags(list);

			return list.OfType<IClientDataObject>().ToList();
		}


		public List<IClientDataObject> Retrieve(Type type, IAttributeSetContainer owner, string alias, EvaluatedCriterion whereClause, string orderByClause, LWQueryBatchInfo batchInfo, LockingMode lockMode)
		{
			ATSLock lockObj = ObtainLock(type.Name, batchInfo, lockMode);
			string where = whereClause != null ? whereClause.Where : null;
			string sql = BuildSelectStatement(type, owner, alias, where, orderByClause, batchInfo == null ? lockMode : LockingMode.None);

			IEnumerable<object> list = null;

			int parameterIndex = 0;
			var parameters = new List<object> ();
			if (owner != null)
			{
				parameters.Add(owner.MyKey);
				parameterIndex++;
			}
			
			if (whereClause != null)
			{
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					sql = sql.Replace(":" + parm.Key, "@" + parameterIndex.ToString());
					parameters.Add(parm.Value);
					parameterIndex++;
				}
			}

			var args = parameters.ToArray();

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
					throw new Exception("Unable to parse SQL statement for paged query");

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
				MethodInfo genericMethod = GetGenericFetchMethod(type);
				list = genericMethod.Invoke(Database, new object[] { sql, args }) as IEnumerable<object> ?? new List<IClientDataObject>(); ;
			}
			else
			{
				MethodInfo genericMethod = GetGenericFetchMethod(type);
				list = genericMethod.Invoke(Database, new object[] { sql, args }) as IEnumerable<object> ?? new List<IClientDataObject>(); ;
			}

			if (lockObj != null)
			{
				AtsLockDao.UpdateLock(lockObj);
			}

			ClearDirtyFlags(list);

			return list.OfType<IClientDataObject>().ToList();
		}


		public List<IClientDataObject> Retrieve(Type type, IAttributeSetContainer owner, string alias, EvaluatedCriterion whereClause, IList<string> distinct, string orderByClause, LWQueryBatchInfo batchInfo, LockingMode lockMode)
		{
			string tableName = GetTableName(type);
			ATSLock lockObj = ObtainLock(type.Name, batchInfo, lockMode);

			IEnumerable<object> list = null;
            string sql = string.Format("select {1}.* from {0} {1} where A_RowKey in (select min(A_RowKey) from {0} where {{0}})", tableName, string.IsNullOrEmpty(alias) ? "a" : alias);
            
			var parameters = new List<object>();
			int parameterIndex = 0;

			string where = whereClause != null && !string.IsNullOrEmpty(whereClause.Where) ? whereClause.Where : string.Empty;

			if (!string.IsNullOrEmpty(where))
			{
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					where = where.Replace(":" + parm.Key, "@" + parameterIndex.ToString());
					parameters.Add(parm.Value);
					parameterIndex++;
				}
			}
						
			if (owner != null)
			{
				if (!string.IsNullOrEmpty(where))
				{
					where += " and ";
				}
				where += ResolveKeyfieldName(owner) + " = @" + parameterIndex.ToString();
				parameters.Add(owner.MyKey);
                parameterIndex++;
            }

            var args = parameters.ToArray();

            string groupBy = distinct != null && distinct.Count > 0 ? " group by " + string.Join(", ", distinct.ToArray()) : string.Empty;
            string orderBy = " " + orderByClause;

            sql = string.Format(sql, where + groupBy + orderBy);


			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
					throw new Exception("Unable to parse SQL statement for paged query");

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}

			MethodInfo genericMethod = GetGenericFetchMethod(type);
			list = genericMethod.Invoke(Database, new object[] { sql, args }) as IEnumerable<object> ?? new List<IClientDataObject>();

			if (lockObj != null)
			{
				AtsLockDao.UpdateLock(lockObj);
			}

			ClearDirtyFlags(list);

			return list.OfType<IClientDataObject>().ToList();
		}


		public List<IClientDataObject> Retrieve(Type type, AttributeSetContainer[] owners, string alias, EvaluatedCriterion whereClause, IList<string> distinct, string orderByClause, LWQueryBatchInfo batchInfo, LockingMode lockMode)
		{
			string tableName = GetTableName(type);
			ATSLock lockObj = ObtainLock(type.Name, batchInfo, lockMode);

			IEnumerable<object> list = null;
            string sql = string.Format("select {1}.* from {0} {1} where A_RowKey in (select min(A_RowKey) from {0} where {{0}})", tableName, string.IsNullOrEmpty(alias) ? "a" : alias);

			string where = whereClause != null && !string.IsNullOrEmpty(whereClause.Where) ? whereClause.Where : string.Empty;
			var parameters = new List<object>();
			int parameterIndex = 1;
			if (whereClause != null)
			{
				foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
				{
					where = where.Replace(":" + parm.Key, "@" + parameterIndex.ToString());
					parameters.Add(parm.Value);
					parameterIndex++;
				}
			}

			if (owners != null && owners.Length > 0)
			{
				if (!string.IsNullOrEmpty(where))
				{
					where += " and ";
				}
				where += ResolveKeyfieldName(owners[0]) + " in (@keys)";

				long[] keys = new long[owners.Length];
				for (int i = 0; i < owners.Length; i++)
				{
					keys[i] = owners[i].MyKey;
				}
				parameters.Add(new { keys = keys });
				parameterIndex++;
			}

			var args = parameters.ToArray();

			string groupBy = distinct != null && distinct.Count > 0 ? " group by " + string.Join(", ", distinct.ToArray()) : string.Empty;
            string orderBy = " " + orderByClause;

            sql = string.Format(sql, where + groupBy + orderBy);

			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}

				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}

			MethodInfo genericMethod = GetGenericFetchMethod(type);
			list = genericMethod.Invoke(Database, new object[] { sql, args }) as IEnumerable<object> ?? new List<IClientDataObject>();

			if (lockObj != null)
			{
				AtsLockDao.UpdateLock(lockObj);
			}

			ClearDirtyFlags(list);

			return list.OfType<IClientDataObject>().ToList();
		}


        public List<IClientDataObject> Retrieve(Type type, IAttributeSetContainer owner, string alias, EvaluatedCriterion whereClause, string orderByClause, int limitNumber, LockingMode lockMode)
        {
            ATSLock lockObj = ObtainLock(type.Name, null, lockMode);
            string where = whereClause != null ? whereClause.Where : null;
            string sql = BuildLimitedSelectStatement(type, owner, alias, where, orderByClause, limitNumber, lockMode);

            IEnumerable<object> list = null;

            int parameterIndex = 0;
            var parameters = new List<object>();
            if (owner != null)
            {
                parameters.Add(owner.MyKey);
                parameterIndex++;
            }

            if (whereClause != null)
            {
                foreach (KeyValuePair<string, object> parm in whereClause.Parameters)
                {
                    sql = sql.Replace(":" + parm.Key, "@" + parameterIndex.ToString());
                    parameters.Add(parm.Value);
                    parameterIndex++;
                }
            }

            var args = parameters.ToArray();

            MethodInfo genericMethod = GetGenericFetchMethod(type);
            list = genericMethod.Invoke(Database, new object[] { sql, args }) as IEnumerable<object> ?? new List<IClientDataObject>();

            if (lockObj != null)
            {
                AtsLockDao.UpdateLock(lockObj);
            }

            ClearDirtyFlags(list);

            return list.OfType<IClientDataObject>().ToList();
        }


		

		public long HowMany<T>(IAttributeSetContainer owner, string alias, string whereClause) where T : IClientDataObject
		{
			return HowMany(GetTableName(typeof(T)), owner, alias, whereClause);
		}

		public long HowMany(Type type, IAttributeSetContainer owner, string alias, string whereClause)
		{
			return HowMany(GetTableName(type), owner, alias, whereClause);
		}

		public long HowMany<T>(IAttributeSetContainer owner, string alias, EvaluatedCriterion where)
		{
			return HowMany(typeof(T), owner, alias, where);
		}

		public long HowMany(Type type, IAttributeSetContainer owner, string alias, EvaluatedCriterion whereClause)
		{
			return HowMany(GetTableName(type), owner, alias, whereClause);
		}

		public long HowMany(Type type, IAttributeSetContainer owner, long[] ownerKeys, DateTime from, DateTime to)
		{
			var where = new EvaluatedCriterion() { Where = "CreateDate >= :fromDate and CreateDate <= :toDate" };
			where.AddParameter("fromDate", from);
			where.AddParameter("toDate", to);
			return HowMany(GetTableName(type), owner, null, where);
		}
		




		public void Delete(IClientDataObject row)
		{
			Database.Delete(row);
		}

		public void Delete<T>(object rowOrKey)
		{
			Database.Delete<T>(rowOrKey);
		}

		public void Delete(Type type, long rowKey)
		{
			IClientDataObject cobj = Retrieve(type, rowKey, LockingMode.None);
			if (cobj != null)
			{
				Database.Delete(cobj);
			}
		}

		public int Delete(Type type, long[] keys)
		{
			string methodName = "Delete";
			try
			{
				string sql = string.Format("delete from {0} where A_RowKey in (@keys)", GetTableName(type));
				int nRows = Database.Execute(sql, new { keys = keys });
				_logger.Trace(_className, methodName, nRows + " rows deleted from " + type.Name);
				return nRows;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error deleting objects.", ex);
				throw;
			}
		}

		public void DeleteByLink(IAttributeSetContainer link, string attSetName)
		{
			const string methodName = "DeleteByLink";
			string sql = string.Format("delete from ats_{0} where {1} = @0", attSetName, link is VirtualCard ? "A_VcKey" : "A_IPCode");
			int nRows = Database.Execute(sql, link.MyKey);
			_logger.Trace(_className, methodName, nRows + " rows deleted from " + attSetName);
		}

		public void DeleteByLinks(IList<IAttributeSetContainer> links, string attSetName)
		{
			const string methodName = "DeleteByLink";
			if (links == null || links.Count == 0)
			{
				return;
			}

			IAttributeSetContainer first = links[0];
			string sql = string.Format("delete from ATS_{0} where {1} in (@keys)", attSetName, first is VirtualCard ? "A_VcKey" : "A_IPCode");

			var keysList = from x in links select x.MyKey;
			long[] keys = keysList.ToArray<long>();
			int keysRemaining = keys.Length;
			int startIdx = 0;
			int nRows = 0;
			while (keysRemaining > 0)
			{
				long[] ids = LimitInClauseList<long>(keys, ref startIdx, ref keysRemaining);
				nRows += Database.Execute(sql, new { keys = ids });
			}
			_logger.Trace(_className, methodName, nRows + " rows deleted from " + attSetName);
		}



		private void ClearDirtyFlags(IEnumerable<object> list)
		{
			foreach (IClientDataObject o in list)
			{
				o.IsDirty = false;
			}
		}

		private void ClearDirtyFlags<T>(List<T> list)
		{
			foreach (IClientDataObject o in list)
			{
				o.IsDirty = false;
			}
		}

		private string GetTableName(Type type)
		{
			lock (_tableNameMap)
			{
				if (_tableNameMap.ContainsKey(type))
				{
					return _tableNameMap[type];
				}
				object[] attributes = type.GetCustomAttributes(typeof(PetaPoco.TableNameAttribute), true);
				string ret = ((PetaPoco.TableNameAttribute)attributes[0]).Value;
				_tableNameMap.Add(type, ret);
				return ret;
			}
		}

		private string BuildSelectStatement(Type t, IAttributeSetContainer owner)
		{
			return BuildSelectStatement(GetTableName(t), owner);
		}

		private string BuildSelectStatement(Type t, IAttributeSetContainer owner, string alias = null, string where = null, string orderBy = null, LockingMode lockMode = LockingMode.None)
		{
			return BuildSelectStatement(GetTableName(t), owner, alias, where, orderBy, lockMode);
		}

		private string BuildSelectStatement(Type t, IAttributeSetContainer owner, EvaluatedCriterion where, string alias = null, string orderBy = null, LockingMode lockMode = LockingMode.None)
		{
			return BuildSelectStatement(GetTableName(t), owner, alias, where != null ? where.Where : null, orderBy, lockMode);
		}

		private string BuildSelectStatement(string tableName, IAttributeSetContainer owner, string alias = null, string where = null, string orderBy = null, LockingMode lockMode = LockingMode.None)
		{
            const string sql = "select {2}.* from {0} {2}{1}";
			string ret = string.Format(sql, tableName, WithUpdateClause(lockMode), string.IsNullOrEmpty(alias) ? "a" : alias);
			if (owner != null)
			{
				if (string.IsNullOrEmpty(where))
				{
					where = ResolveKeyfieldName(owner) + " = @0";
				}
				else
				{
					where = ResolveKeyfieldName(owner) + " = @0 and " + where;
				}
			}
			if (!string.IsNullOrEmpty(where))
			{
				ret += " where " + where;
			}
            if (!string.IsNullOrEmpty(orderBy))
            {
                //order by, for some reason, was expected to have the "order by" clause in it, so we're excluding it.
                ret += " " + orderBy;
            }
			ret += ForUpdateClause(lockMode);
			return ret;
		}

		private string BuildSelectStatement(string tableName, IAttributeSetContainer[] owners, string alias = null, string where = null, string orderBy = null, LockingMode lockMode = LockingMode.None)
		{
			const string sql = "select {1}.* from {0} {1}{2}";
            string ret = string.Format(sql, tableName, string.IsNullOrEmpty(alias) ? "a" : alias, WithUpdateClause(lockMode));
			if (owners != null && owners.Length > 0)
			{
				if (string.IsNullOrEmpty(where))
				{
					where = ResolveKeyfieldName(owners[0]) + " in (@keys)";
				}
				else
				{
					where = ResolveKeyfieldName(owners[0]) + " in (@keys) and " + where;
				}
			}
			if (!string.IsNullOrEmpty(where))
			{
				ret += " where " + where;
			}
            if (!string.IsNullOrEmpty(orderBy))
            {
                //order by, for some reason, was expected to have the "order by" clause in it, so we're excluding it.
                ret += " " + orderBy;
            }
            ret += ForUpdateClause(lockMode);
			return ret;
		}

		private string BuildSelectCountStatement(Type t, IAttributeSetContainer owner)
		{
			return BuildSelectCountStatement(GetTableName(t), owner);
		}

		private string BuildSelectCountStatement(Type t, IAttributeSetContainer owner, string alias = null, string where = null, string orderBy = null, LockingMode lockMode = LockingMode.None)
		{
			return BuildSelectCountStatement(GetTableName(t), owner, alias, where, orderBy);
		}

		private string BuildSelectCountStatement(Type t, IAttributeSetContainer owner, EvaluatedCriterion where, string alias = null, string orderBy = null, LockingMode lockMode = LockingMode.None)
		{
			return BuildSelectCountStatement(GetTableName(t), owner, alias, where != null ? where.Where : null, orderBy);
		}

		private string BuildSelectCountStatement(string tableName, IAttributeSetContainer owner, string alias = null, string where = null, string orderBy = null, LockingMode lockMode = LockingMode.None)
		{
			const string sql = "select count(*) from {0} {2}{1}";
			string ret = string.Format(sql, tableName, WithUpdateClause(lockMode), alias ?? string.Empty);
			if (owner != null)
			{
                if (string.IsNullOrEmpty(where))
				{
					where = ResolveKeyfieldName(owner) + " = @0";
				}
				else
				{
					where = ResolveKeyfieldName(owner) + " = @0 and " + where;
				}
			}
			if (!string.IsNullOrEmpty(where))
			{
				ret += " where " + where;
			}
			if (!string.IsNullOrEmpty(orderBy))
			{
				//order by, for some reason, was expected to have the "order by" clause in it, so we're excluding it.
				ret += " " + orderBy;
			}
			ret += ForUpdateClause(lockMode);
			return ret;
		}

		private string BuildSelectCountStatement(string tableName, IAttributeSetContainer[] owners, string alias = null, string where = null, string orderBy = null, LockingMode lockMode = LockingMode.None)
		{
			const string sql = "select count(*) from {0} {1}";
			string ret = string.Format(sql, tableName, alias ?? string.Empty);
			if (owners != null && owners.Length > 0)
			{
				if (where == null)
				{
					where = ResolveKeyfieldName(owners[0]) + " in (@keys)";
				}
				else
				{
					where = ResolveKeyfieldName(owners[0]) + " in (@keys) and " + where;
				}
			}
			if (!string.IsNullOrEmpty(where))
			{
				ret += " where " + where;
			}
			if (!string.IsNullOrEmpty(orderBy))
			{
				//order by, for some reason, was expected to have the "order by" clause in it, so we're excluding it.
				ret += " " + orderBy;
			}
			return ret;
		}



        private string BuildLimitedSelectStatement(Type type, IAttributeSetContainer owner, string alias, string where, string orderBy, int limitNumber, LockingMode lockMode)
        {
            const string sql = "select {3}{2}.* from {0} {2}{1}";
            
            string tableName = GetTableName(type);

            string limit = string.Empty;
            string top = string.Empty;
            string rownum = string.Empty;
            if (Database._dbType is PetaPoco.DatabaseTypes.OracleDatabaseType)
            {
                rownum = string.Format(" ROWNUM <= {0} ", limitNumber);
            }
            if (Database._dbType is PetaPoco.DatabaseTypes.SqlServerDatabaseType)
            {
                top = string.Format(" TOP {0} ", limitNumber);
            }
            if (Database._dbType is PetaPoco.DatabaseTypes.MySqlDatabaseType)
            {
                limit = string.Format(" LIMIT {0}", limitNumber);
            }

            string ret = string.Format(sql, tableName, WithUpdateClause(lockMode), string.IsNullOrEmpty(alias) ? "a" : alias, top);
            if (owner != null)
            {
                where = ResolveKeyfieldName(owner) + " = @0" + (string.IsNullOrEmpty(where) ? string.Empty : " and " + where);
            }
            if (!string.IsNullOrEmpty(rownum))
            {
                where = rownum + (string.IsNullOrEmpty(where) ? string.Empty : " and " + where);
            }
            if (!string.IsNullOrEmpty(where))
            {
                ret += " where " + where;
            }
            if (!string.IsNullOrEmpty(orderBy))
            {
                //order by, for some reason, was expected to have the "order by" clause in it, so we're excluding it.
                ret += " " + orderBy;
            }
            ret += ForUpdateClause(lockMode);
            ret += limit;
            return ret;
        }



		private ATSLock ObtainLock(string name, LWQueryBatchInfo batchInfo, LockingMode lockMode)
		{
			if (batchInfo == null && lockMode != LockingMode.None)
			{
				return AtsLockDao.ObtainLock(name);
			}
			return null;
		}

		private MethodInfo GetGenericFetchMethod(Type type)
		{
			//Type[] types = { typeof(string), typeof(object[]) };
			lock (_fetchMethodMap)
			{
				if (_fetchMethodMap.ContainsKey(type))
				{
					return _fetchMethodMap[type];
				}
				MethodInfo method = typeof(PetaPoco.Database)
					.GetMethods()
					.Where(m => m.Name == "Fetch")
					.Select(m => new
					{
						Method = m,
						Params = m.GetParameters(),
						Args = m.GetGenericArguments()
					})
					.Where(x => x.Params.Length == 2
								&& x.Args.Length == 1
								&& x.Params[0].ParameterType == typeof(string)
								&& x.Params[1].ParameterType == typeof(object[])
								)
					.Select(x => x.Method)
					.First();

				MethodInfo genericMethod = method.MakeGenericMethod(type);
				_fetchMethodMap.Add(type, genericMethod);
				return genericMethod;
			}
		}

		private MethodInfo GetGenericPagedMethod(Type type)
		{
			//Type[] types = { typeof(string), typeof(object[]) };
			lock (_pagedMethodMap)
			{
				if (_pagedMethodMap.ContainsKey(type))
				{
					return _pagedMethodMap[type];
				}
				MethodInfo method = typeof(PetaPoco.Database)
					.GetMethods()
					.Where(m => m.Name == "Page")
					.Select(m => new
					{
						Method = m,
						Params = m.GetParameters(),
						Args = m.GetGenericArguments()
					})
					.Where(x => x.Params.Length == 4
								&& x.Args.Length == 1
								&& x.Params[0].ParameterType == typeof(long)
								&& x.Params[1].ParameterType == typeof(long)
								&& x.Params[2].ParameterType == typeof(string)
								&& x.Params[3].ParameterType == typeof(object[])
								)
					.Select(x => x.Method)
					.First();

				MethodInfo genericMethod = method.MakeGenericMethod(type);
				_pagedMethodMap.Add(type, genericMethod);
				return genericMethod;
			}
		}

		private MethodInfo GetGenericFirstOrDefaultMethod(Type type)
		{
			lock (_firstOrDefaultMethodMap)
			{
				if (_firstOrDefaultMethodMap.ContainsKey(type))
				{
					return _firstOrDefaultMethodMap[type];
				}
				MethodInfo method = typeof(PetaPoco.Database)
					.GetMethods()
					.Where(m => m.Name == "FirstOrDefault")
					.Select(m => new
					{
						Method = m,
						Params = m.GetParameters(),
						Args = m.GetGenericArguments()
					})
					.Where(x => x.Params.Length == 2
								&& x.Args.Length == 1
								&& x.Params[0].ParameterType == typeof(string)
								&& x.Params[1].ParameterType == typeof(object[])
								)
					.Select(x => x.Method)
					.First();

				MethodInfo genericMethod = method.MakeGenericMethod(type);
				_firstOrDefaultMethodMap.Add(type, genericMethod);
				return genericMethod;
			}
		}



		private string ResolveKeyfieldName(IAttributeSetContainer owner)
		{
			const string parentRowKey = "A_ParentRowKey";
			const string ipCode = "A_IPCode";
			const string vcKey = "A_VCKey";
			if (owner is Member)
			{
				return ipCode;
			}
			if (owner is VirtualCard)
			{
				return vcKey;
			}
			return parentRowKey;
		}


		private long HowMany(string tableName, IAttributeSetContainer owner, string alias, string whereClause)
		{
			return HowMany(tableName, owner, alias, new EvaluatedCriterion() { Where = whereClause });
		}

		private long HowMany(string tableName, IAttributeSetContainer owner, string alias, EvaluatedCriterion where)
		{
			string sql = BuildSelectCountStatement(tableName, owner, alias, where != null ? where.Where : null);
			var args = new List<object>();
            int parameterIndex = 0;
            if (owner != null)
			{
				args.Add(owner.MyKey);
                //BuildSelectCountStatement has reserved 0 for owner
                parameterIndex++;
			}
            if (where.Parameters.Count > 0)
            {

                foreach (KeyValuePair<string, object> param in where.Parameters)
                {
                    sql = sql.Replace(":" + param.Key, "@" + parameterIndex++.ToString());
                    args.Add(param.Value);
                }
            }
			return Database.ExecuteScalar<long>(sql, args.ToArray());
		}



	}
}