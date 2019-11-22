using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;
using PetaPoco.Internal;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class DaoBase<T>
	{
		private const string _className = "DaoBase";
		private const int _inClauseLimit = 1000;
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		protected PetaPoco.Database Database { get; set; }
		protected ServiceConfig Config {get;set;}

		protected static LWLogger logger { get { return _logger; } }

		protected DaoBase(Database database, ServiceConfig config)
		{
			if (database == null)
			{
				throw new ArgumentNullException("database");
			}
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			Database = database;
			Config = config;
		}

		public virtual void Create(T t)
		{
			SaveEntity(t);
		}

		public virtual void Create(IEnumerable<T> ts)
		{
			ITransaction txn = Database.GetTransaction();
			try
			{
				int i = 0;
				int bulkLoadingBatchSize = Config.BulkLoadingBatchSize.GetValueOrDefault(100);
				foreach (T t in ts)
				{
					SaveEntity(t);
					if (i != 0 && bulkLoadingBatchSize > 0 && i % bulkLoadingBatchSize == 0)
					{
						txn.Complete();
						txn.Dispose();
						txn = Database.GetTransaction();
					}
					i++;
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

		public virtual void Update(T t)
		{
			UpdateEntity(t);
		}

		public virtual void Update(IEnumerable<T> ts)
		{
			ITransaction txn = Database.GetTransaction();
			try
			{
				int i = 0;
				int bulkLoadingBatchSize = Config.BulkLoadingBatchSize.GetValueOrDefault(100);
				foreach (T t in ts)
				{
					UpdateEntity(t);
					if (i != 0 && bulkLoadingBatchSize > 0 && i % bulkLoadingBatchSize == 0)
					{
						txn.Complete();
						txn.Dispose();
						txn = Database.GetTransaction();
					}
					i++;
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
        /// Batches long id arrays into chunks of 1000
        /// </summary>
        /// <typeparam name="U">The type of the id array</typeparam>
        /// <param name="sql">The query to execute with the id array as "@0"</param>
        /// <param name="refIds">The array of identifiers to batch</param>
        /// <returns>A list of 0+ items of the caller's expected type</returns>
        protected List<T> RetrieveByArray<U>(string sql, U[] refIds)
        {
            int idsRemaining = refIds.Length;
            int startIndex = 0;
            List<T> ret = new List<T>();
            while (idsRemaining > 0)
            {
                U[] ids = LimitInClauseList<U>(refIds, ref startIndex, ref idsRemaining);
                var set = Database.Fetch<T>(sql, ids);
                if (set != null && set.Count > 0)
                {
                    ret.AddRange(set);
                }
            }
            return ret;
        }

		protected U[] LimitInClauseList<U>(U[] origArray, ref int startingIndex, ref int remaining)
		{
			int batchSize = origArray.Length - startingIndex;
			if (batchSize > _inClauseLimit)
			{
				batchSize = _inClauseLimit;
			}
			U[] ids = new U[batchSize];
			Array.Copy(origArray, startingIndex, ids, 0, ids.Length);
			startingIndex += batchSize;
			remaining -= batchSize;
			return ids;
		}

        protected string AddBatchedInList(string fieldName, long[] items, List<object> parameters)
        {
            string sql = string.Empty;
            int itemsRemaining = items.Length;
            int startIdx = 0;
            while (itemsRemaining > 0)
            {
                if (!string.IsNullOrEmpty(sql))
                    sql += " OR ";
                long[] itemsBatch = LimitInClauseList<long>(items, ref startIdx, ref itemsRemaining);
                sql += string.Format("{0} in (@{1})", fieldName, parameters.Count.ToString());
                parameters.Add(itemsBatch);
            }
            return string.Format("({0})", sql);
        }

		protected string WithUpdateClause(LockingMode lockMode)
		{
			if (lockMode >= LockingMode.Upgrade && !(Database._dbType is PetaPoco.DatabaseTypes.OracleDatabaseType))
			{
				return " with (updlock)";
			}
			return string.Empty;
		}

		protected string ForUpdateClause(LockingMode lockMode)
		{
			if (lockMode >= LockingMode.Upgrade && Database._dbType is PetaPoco.DatabaseTypes.OracleDatabaseType)
			{
				return " for update";
			}
			return string.Empty;
		}

		///// <summary>
		///// Retrieves first or default by key
		///// </summary>
		///// <param name="id"></param>
		///// <returns></returns>
		//protected T FirstOrDefault(long id)
		//{
		//	return Database.FirstOrDefault<T>("where id = @0", id);
		//}

		//protected T FirstOrDefault(string name)
		//{
		//	return Database.FirstOrDefault<T>("where name = @0", name);
		//}

		protected virtual T GetEntity(object key)
		{
			return Database.SingleOrDefault<T>(key);
		}

		protected virtual void SaveEntity(object entity)
		{
			try
			{
				LWCoreObjectBase obj = entity as LWCoreObjectBase;
				if (obj != null)
				{
					obj.CreateDate = DateTime.Now;
					obj.UpdateDate = DateTime.Now;
				}

				if (ShouldLog(entity))
				{					
					using (var txn = Database.GetTransaction())
					{
						Database.Insert(entity);
						ArchiveEntity(entity, DataChangeType.I);
						txn.Complete();
					}
				}
				else
				{
					Database.Insert(entity);
				}
				if (entity is AttributeSetContainer)
				{
					((AttributeSetContainer)entity).IsDirty = false;
				}
			}
			catch (Exception ex)
			{
				string msg = string.Format("Error saving object of type {0}", entity.GetType().FullName);
				logger.Error(_className, "SaveEntity", msg, ex);
				throw;
			}
		}

		protected virtual void UpdateEntity(object entity)
		{
			try
			{
				LWCoreObjectBase obj = entity as LWCoreObjectBase;
				if (obj != null)
				{
					obj.UpdateDate = DateTime.Now;
				}
				if (ShouldLog(entity))
				{
					using (var txn = Database.GetTransaction())
					{
						Database.Update(entity);
						ArchiveEntity(entity, DataChangeType.U);
						txn.Complete();
					}
				}
				else
				{
					Database.Update(entity);
				}
				if (entity is AttributeSetContainer)
				{
					((AttributeSetContainer)entity).IsDirty = false;
				}
			}
			catch (Exception ex)
			{
				string msg = string.Format("Error updating object of type {0}", entity.GetType().FullName);
				logger.Error(_className, "UpdateEntity", msg, ex);
				throw;
			}
		}

        protected virtual void DeleteEntity(long id)
        {
            if (ShouldLog(typeof(T)))
            {
                object entity = GetEntity(id);
                if (entity != null)
                    DeleteEntity(entity);
            }
            else
                Database.Delete<T>(id);
        }

		protected void DeleteEntity(object entity)
		{
			if (ShouldLog(entity))
			{
				using (var txn = Database.GetTransaction())
				{
					Database.Delete<T>(entity);
					ArchiveEntity(entity, DataChangeType.D);
					txn.Complete();
				}
			}
			else
			{
				Database.Delete<T>(entity);
			}
		}

		protected void ApplyBatchInfo(LWQueryBatchInfo batchInfo, ref string sql, ref object[] args)
		{
			if (batchInfo != null)
			{
				PagingHelper.SQLParts parts;
				if (!PagingHelper.SplitSQL(sql, out parts))
				{
					throw new Exception("Unable to parse SQL statement for paged query");
				}
				sql = Database._dbType.BuildPageQuery(batchInfo.StartIndex, batchInfo.BatchSize, parts, ref args);
			}
		}

        protected string CreateLimitQuery(string tableName, string alias, string where, string orderBy, int limitNumber, LockingMode lockMode)
        {
            const string sql = "select {3}{2}.* from {0} {2}{1}";

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

		/// <summary>
		/// Executes a query and return the first column of the first row in the result set.
		/// </summary>
		/// <remarks>
		/// Slightly safer than the underlying PetaPoco method, this method will convert DBNull (or a lack or result) into the default value for 
		/// the generic type. Use this veriosn of ExecuteScalar when the query might return null or no results and the generic type cannot be 
		/// cast from DBNull.Value (int, long, double, etc.).
		/// </remarks>
		/// <typeparam name="X"></typeparam>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		protected X ExecuteScalar<X>(string sql, params object[] parameters) 
		{
			object scalar = Database.ExecuteScalar<object>(sql, parameters);
			X ret = scalar == DBNull.Value ? default(X) : (X)scalar;
			return ret;
		}

		private bool ShouldLog(object affectedEntity)
		{
			if (affectedEntity is LWObjectAuditLogBase || affectedEntity is AuditLogConfig)
			{
				return false;
			}
			
			if (affectedEntity is LangChanContent)
			{
				if (!IsContentObjTypeAuditEnabled(((LangChanContent)affectedEntity).LangChanType))
				{
					return false;
				}
			}
			else if (affectedEntity is ContentAttribute)
			{
				if (!this.IsContentObjTypeAuditEnabled(((ContentAttribute)affectedEntity).ContentType))
				{
					return false;
				}
			}

			string typeName = affectedEntity.GetType().Name;
			if (typeName == "EmailDocument")
			{
				// a hack because email class is called EmailDocument instead of Email.
				typeName = "Email";
			}
            AuditLogConfig cfg = Config.AuditableObjects.ContainsKey(typeName) ? Config.AuditableObjects[typeName] : null;
			return cfg != null && cfg.LoggingEnabled;
		}

        // This overload is only used when deleting by ID
        private bool ShouldLog(Type type)
        {
            if (type == typeof(LWObjectAuditLogBase) || type == typeof(AuditLogConfig))
            {
                return false;
            }

            string typeName = type.Name;
            if (typeName == "EmailDocument")
            {
                typeName = "Email";
            }
            AuditLogConfig cfg = Config.AuditableObjects.ContainsKey(typeName) ? Config.AuditableObjects[typeName] : null;
            return cfg != null && cfg.LoggingEnabled;
        }
				
		private ApplicationType _applicationType;
		private string _hostName;

		/// <summary>
		/// CS: CS Portal, CF: CF web site, LN: Loyalty Navigator, AP: API,
		/// DA: DAP, RU: Rule Processor, OT: Others
		/// </summary>
		public ApplicationType ApplicationType
		{
			get
			{
				if (this._applicationType == 0)
				{
					try
					{
						this._applicationType = (ApplicationType)Enum.Parse(typeof(ApplicationType), LWConfigurationUtil.GetConfigurationValue("ApplicationType"), true);
					}
					catch
					{
						this._applicationType = ApplicationType.OT;
					}
				}
				return this._applicationType;
			}
			set
			{
				this._applicationType = value;
			}
		}

		/// <summary>
		/// Record owner's unique identifier.
		///   OwnerType = "C": CS portal user, the id is CS agent's ID (not username);
		///   OwnerType = "F": CF web site, the id is loyalty member ID (ipcode);
		///   OwnerType = "L": Loyalty Navigator, the id is LN user ID;
		///   OwnerType = "A": Standard API, the id is "API";
		///   OwnerType = "D": DAP processor, the id is "DAP";
		///   OwnerType = "R": Rule Processor, the id is "RuleProcessor";
		///   OwnerType = "O" or other unknown type: the id is "(Unknown)";        
		/// </summary>
		public string OwnerID
		{
			get
			{
				string ownerID = "";
				switch (this.ApplicationType)
				{
					case ApplicationType.CS:   // CS agent
						ownerID = this.GetCSPortalAgentUserName();
						break;
					case ApplicationType.CF:   // loyalty member
						ownerID = this.GetLoyaltyMemberIpcode();
						break;
					case ApplicationType.LN:   // LN user
						ownerID = this.GetLoyaltyNavigatorUserName();
						break;
					case ApplicationType.AP:
						ownerID = "API";
						break;
					case ApplicationType.DA:
						ownerID = "DAP";
						break;
					case ApplicationType.RU:
						ownerID = "RuleProcessor";
						break;
					default:
						ownerID = "(Unknown)";
						break;
				}
				return ownerID;
			}
		}

		/// <summary>
		/// Host Name can be set in web.config or app.config file. If not set, local machine name will 
		/// be used.
		/// </summary>
		public string HostName
		{
			get
			{
				if (string.IsNullOrEmpty(this._hostName))
				{
					try
					{
						this._hostName = LWConfigurationUtil.GetConfigurationValue("HostName");
					}
					catch
					{
					}
				}
				if (string.IsNullOrEmpty(this._hostName))
				{
					try
					{
						this._hostName = System.Environment.GetEnvironmentVariable("COMPUTERNAME");
					}
					catch
					{
						this._hostName = "(Unknown)";
					}
				}
				return this._hostName;
			}
		}

		/// <summary>
		/// The value type stored in authenticated user's Identity.Name field. Used only by 
		/// web applications (CS portal, CF site and Loyalty Navigator).
		/// </summary>
		public UserIdentityType AuthIdentityType
		{
			get
			{
				if (this.ApplicationType == ApplicationType.CF)        // CF site
					return UserIdentityType.Ipcode;
				else if (this.ApplicationType == ApplicationType.CS)   // CS portal
					return UserIdentityType.AgentUserName;
				else if (this.ApplicationType == ApplicationType.LN)   // Loyalty Navigator
					return UserIdentityType.UserName;
				else
					return UserIdentityType.Unknown;
			}
		}

		

		/// <summary>
		/// Get CS Portal's current logged in user's username.
		/// </summary>
		/// <returns></returns>
		private string GetCSPortalAgentUserName()
		{
			string methodName = "GetCSPortalAgentUserName";
			if (HttpContext.Current.Request.IsAuthenticated)
				return HttpContext.Current.User.Identity.Name;
			else
			{
				_logger.Error(_className, methodName, "GetCSPortalAgentUserName() is called when CS agent is not authenticated");
				return "(Unknown)";
			}
		}

		/// <summary>
		/// Get loyalty member's ipcode. Called when application type is CF web site.
		/// </summary>
		/// <returns></returns>
		private string GetLoyaltyMemberIpcode()
		{
			string methodName = "GetLoyaltyMemberIpcode";
			if (HttpContext.Current.Request.IsAuthenticated)
				return HttpContext.Current.User.Identity.Name;
			else
			{
				_logger.Error(_className, methodName, "GetLoyaltyMemberIpcode() is called when loyalty member is not authenticated");
				return "(Unknown)";
			}
		}

		/// <summary>
		/// Get Loyalty Navigator's user name. Called when application type is L (LN). For
		/// Loyalty Navigator, user name is stored in Identity.Name field if the user has been
		/// authenticated.
		/// </summary>
		/// <returns></returns>
		private string GetLoyaltyNavigatorUserName()
		{
			string result = null;
			if (HttpContext.Current.Request.IsAuthenticated)
			{
				result = HttpContext.Current.User.Identity.Name;
			}
			return result == null ? "(Unknown)" : result;
		}

		/// <summary>
		/// Archive affected entity if it is configured for audit logging.
		/// </summary>
		/// <param name="affectedEntity"></param>
		/// <param name="changeType"></param>
		private void ArchiveEntity(object affectedEntity, DataChangeType changeType)
		{
			string methodName = "ArchiveEntity";
			string entityClassName = affectedEntity.GetType().Name;

			try
			{
				LWCoreObjectBase auditableObject = affectedEntity as LWCoreObjectBase;
				if (auditableObject != null)
				{
					LWObjectAuditLogBase arc = auditableObject.GetArchiveObject();
					if (arc != null)
					{
						arc.Ar_CreatedOn = DateTime.Now;
						arc.Ar_AppType = this.ApplicationType;
						arc.Ar_HostName = this.HostName;
						arc.Ar_OwnerID = this.OwnerID;
						arc.Ar_ChangeType = changeType;
						Database.Insert(arc);
					}
				}
			}
			catch (Exception e)
			{
				string msg = string.Format("Error archiving loyaltyware record. Affected entity: {0}.  Error message: {1}", entityClassName, e.Message);
				_logger.Error(_className, methodName, msg, e);
				throw new Exception(msg, e);
			}
		}

		private bool IsContentObjTypeAuditEnabled(ContentObjType objType)
		{
			string typeName = null;
			switch (objType)
			{
				case ContentObjType.Bonus:
					typeName = "BonusDef";
					break;
				case ContentObjType.Coupon:
					typeName = "CouponDef";
					break;
				case ContentObjType.Message:
					typeName = "MessageDef";
					break;
				case ContentObjType.Product:
					typeName = "Product";
					break;
				case ContentObjType.Promotion:
					typeName = "Promotion";
					break;
				case ContentObjType.Reward:
					typeName = "RewardDef";
					break;
			}
			if (typeName == null)
			{
				return false;
			}
            AuditLogConfig cfg = Config.AuditableObjects.ContainsKey(typeName) ? Config.AuditableObjects[typeName] : null;
			return cfg != null && cfg.LoggingEnabled;
		}

	}
}
