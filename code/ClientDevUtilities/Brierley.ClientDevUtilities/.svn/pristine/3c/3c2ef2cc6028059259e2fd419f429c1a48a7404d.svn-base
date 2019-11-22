using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.Cache;
using Brierley.FrameWork.Data.DataAccess;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	public class DataService : ServiceBase
	{
		private const string _className = "DataService";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		private LWSchemaVersionDao _schemaVersionDao;
		private X509CertDao _x509CertDao;
		private ScheduledJobDao _scheduledJobDao;
		private ScheduledJobRunDao _scheduledJobRunDao;
		private SkinDao _skinDao;
		private SkinItemDao _skinItemDao;
		private AsyncJobDao _asyncJobDao;
		private SyncJobDao _syncJobDao;
		private AsyncJobProcessedObjectDao _asyncJobProcessedObjectDao;
		private IdGeneratorDao _idGeneratorDao;
		private AuditLogConfigDao _auditLogConfigDao;
		private ClientConfigurationDao _clientConfigurationDao;
		private BScriptDao _bscriptDao;
		private LibMessageLogDao _libMessageLogDao;
		private ArchiveObjectDao _archiveObjectDao;
		private CacheRefreshDao _cacheRefreshDao;
        private RemoteAssemblyDao _remoteAssemblyDao;

        public Dictionary<string, AuditLogConfig> AuditableObjects
        {
            get
            {
					return ((ServiceConfig)Config).AuditableObjects;
            }
            private set
            {
                ((ServiceConfig)Config).AuditableObjects = value;
            }
        }

		public bool DebitPayOffMethodOn
		{
			get
			{
				return Config.DebitPayOffMethodOn;
			}
		}

		public bool DebitPayOffPointTypeRestrictionOn
		{
			get
			{
				return Config.DebitPayOffPointTypeRestrictionOn;
			}
		}

		public bool DebitPayOffPointEventRestrictionOn
		{
			get
			{
				return Config.DebitPayOffPointEventRestrictionOn;
			}
		}

		public X509CertDao X509CertDao
		{
			get
			{
				if (_x509CertDao == null)
				{
					_x509CertDao = new X509CertDao(Database, Config);
				}
				return _x509CertDao;
			}
		}

		public LWSchemaVersionDao SchemaVersionDao
		{
			get
			{
				if (_schemaVersionDao == null)
				{
					_schemaVersionDao = new LWSchemaVersionDao(Database, Config);
				}
				return _schemaVersionDao;
			}
		}

		public ScheduledJobDao ScheduledJobDao
		{
			get
			{
				if (_scheduledJobDao == null)
				{
					_scheduledJobDao = new ScheduledJobDao(Database, Config);
				}
				return _scheduledJobDao;
			}
		}

		public ScheduledJobRunDao ScheduledJobRunDao
		{
			get
			{
				if (_scheduledJobRunDao == null)
				{
					_scheduledJobRunDao = new ScheduledJobRunDao(Database, Config);
				}
				return _scheduledJobRunDao;
			}
		}

		public SkinItemDao SkinItemDao
		{
			get
			{
				if (_skinItemDao == null)
				{
					_skinItemDao = new SkinItemDao(Database, Config);
				}
				return _skinItemDao;
			}
		}

		public SkinDao SkinDao
		{
			get
			{
				if (_skinDao == null)
				{
					_skinDao = new SkinDao(Database, Config);
				}
				return _skinDao;
			}
		}

		public AsyncJobDao AsyncJobDao
		{
			get
			{
				if (_asyncJobDao == null)
				{
					_asyncJobDao = new AsyncJobDao(Database, Config);
				}
				return _asyncJobDao;
			}
		}

		public SyncJobDao SyncJobDao
		{
			get
			{
				if (_syncJobDao == null)
				{
					_syncJobDao = new SyncJobDao(Database, Config);
				}
				return _syncJobDao;
			}
		}

		public AsyncJobProcessedObjectDao AsyncJobProcessedObjectDao
		{
			get
			{
				if (_asyncJobProcessedObjectDao == null)
				{
					_asyncJobProcessedObjectDao = new AsyncJobProcessedObjectDao(Database, Config);
				}
				return _asyncJobProcessedObjectDao;
			}
		}

		public AuditLogConfigDao AuditLogConfigDao
		{
			get
			{
				if (_auditLogConfigDao == null)
				{
					_auditLogConfigDao = new AuditLogConfigDao(Database, Config);
				}
				return _auditLogConfigDao;
			}
		}

		public ClientConfigurationDao ClientConfigurationDao
		{
			get
			{
				if (_clientConfigurationDao == null)
				{
					_clientConfigurationDao = new ClientConfigurationDao(Database, Config);
				}
				return _clientConfigurationDao;
			}
		}

		public IdGeneratorDao IdGeneratorDao
		{
			get
			{
				if (_idGeneratorDao == null)
				{
					_idGeneratorDao = new IdGeneratorDao(Database, Config);
				}
				return _idGeneratorDao;
			}
		}

		public BScriptDao BScriptDao
		{
			get
			{
				if (_bscriptDao == null)
				{
					_bscriptDao = new BScriptDao(Database, Config);
				}
				return _bscriptDao;
			}
		}

		public LibMessageLogDao LibMessageLogDao
		{
			get
			{
				if (_libMessageLogDao == null)
				{
					_libMessageLogDao = new LibMessageLogDao(Database, Config);
				}
				return _libMessageLogDao;
			}
		}

		public ArchiveObjectDao ArchiveObjectDao
		{
			get
			{
				if (_archiveObjectDao == null)
				{
					_archiveObjectDao = new ArchiveObjectDao(Database, Config);
				}
				return _archiveObjectDao;
			}
		}

		public CacheRefreshDao CacheRefreshDao
		{
			get
			{
				if (_cacheRefreshDao == null)
				{
					_cacheRefreshDao = new CacheRefreshDao(Database, Config);
				}
				return _cacheRefreshDao;
			}
		}

        public RemoteAssemblyDao RemoteAssemblyDao
        {
            get
            {
                if (_remoteAssemblyDao == null)
                {
                    _remoteAssemblyDao = new RemoteAssemblyDao(Database, Config);
                }
                return _remoteAssemblyDao;
            }
        }

		public DataService(ServiceConfig config)
			: base(config)
		{
		}

		public void CreateX509Cert(X509Cert entity)
		{
			X509CertDao.Create(entity);
		}

		public void UpdateX509Cert(X509Cert entity)
		{
			X509CertDao.Update(entity);
		}

		public X509Cert GetX509Cert(long id)
		{
			return X509CertDao.Retrieve(id);
		}

		public X509Cert GetX509Cert(string certName)
		{
			return X509CertDao.Retrieve(certName);
		}

		public List<X509Cert> GetAllX509Certs()
		{
			return X509CertDao.RetrieveAll();
		}

		public List<X509Cert> GetAllX509Certs(DateTime changedSince)
		{
			return X509CertDao.RetrieveAll(changedSince);
		}

		public List<X509Cert> GetAllX509Certs(string sortExpression, bool ascending)
		{
			return X509CertDao.RetrieveAll(sortExpression, ascending);
		}

		public List<X509Cert> GetAllX509CertByCertType(X509CertType certType)
		{
			return X509CertDao.RetrieveAllByCertType(certType);
		}

		public List<X509Cert> GetAllX509CertByPassType(string passType)
		{
			List<X509Cert> certs = X509CertDao.RetrieveByPassType(passType);
			if (certs == null)
			{
				certs = new List<X509Cert>();
			}
			return certs;
		}

		public void DeleteX509Cert(long id)
		{
			X509CertDao.Delete(id);
		}


		public override IList<LWSchemaVersion> GetSchemaVersions()
		{
			return SchemaVersionDao.RetrieveAll() ?? new List<LWSchemaVersion>();
		}

		public override IList<LWSchemaVersion> GetSchemaVersion(string targetType)
		{
			return SchemaVersionDao.RetrieveByTargetType(targetType) ?? new List<LWSchemaVersion>();
		}

		public override LWSchemaVersion GetLatestSchemaVersion(string targetType)
		{
			return SchemaVersionDao.RetrieveLatestVersionByTargetType(targetType);
		}


		public void CreateScheduledJob(ScheduledJob job)
		{
			ScheduledJobDao.Create(job);
		}

		public void UpdateScheduledJob(ScheduledJob job)
		{
			ScheduledJobDao.Update(job);
		}

		public ScheduledJob GetScheduledJob(long jobId)
		{
			return ScheduledJobDao.Retrieve(jobId);
		}

		public ScheduledJob GetScheduledJob(string jobName)
		{
			return ScheduledJobDao.Retrieve(jobName);
		}

		public IList<ScheduledJob> GetScheduledJob(string assemblyName, string factoryName)
		{
			return ScheduledJobDao.Retrieve(assemblyName, factoryName);
		}

		public IList<ScheduledJob> GetAllScheduledJobs()
		{
			return ScheduledJobDao.RetrieveAll() ?? new List<ScheduledJob>();
		}

		public IList<ScheduledJob> GetScheduledJobs(List<Dictionary<string, object>> parms)
		{
			return ScheduledJobDao.RetrieveAll(parms);
		}

		public void DeleteScheduledJob(long jobId)
		{
			using (var txn = Database.GetTransaction())
			{
				IList<ScheduledJobRun> runs = ScheduledJobRunDao.RetrieveByJobId(jobId);
				foreach (ScheduledJobRun run in runs)
				{
					ScheduledJobRunDao.Delete(run.ID);
				}
				ScheduledJobDao.Delete(jobId);
				txn.Complete();
			}
		}


		public void CreateScheduledJobRun(ScheduledJobRun run)
		{
			ScheduledJobRunDao.Create(run);
		}

		public void UpdateScheduledJobRun(ScheduledJobRun run)
		{
			ScheduledJobRunDao.Update(run);
		}

		public ScheduledJobRun GetScheduledJobRun(long runId)
		{
			return ScheduledJobRunDao.Retrieve(runId);
		}

		public bool HasScheduledJobBeenRun(long jobId)
		{
			return ScheduledJobRunDao.HasJobBeenRun(jobId);
		}

		public DateTime? GetScheduledJobLastRunTime(long jobId)
		{
			return ScheduledJobRunDao.GetLastJobRun(jobId);
		}

		public IList<ScheduledJobRun> GetAllScheduledJobRuns(long jobId)
		{
			return ScheduledJobRunDao.RetrieveByJobId(jobId);
		}

		public IEnumerable<ScheduledJobRun> GetIncompleteJobRuns(DateTime afterStartDate)
		{
			return ScheduledJobRunDao.RetrieveIncompleteJobRuns(afterStartDate);
		}

		public void DeleteScheduledJobRun(long runId)
		{
			ScheduledJobRunDao.Delete(runId);
		}

		public void CreateSkin(Skin entity)
		{
			SkinDao.Create(entity);
		}

		public void UpdateSkin(Skin entity)
		{
			SkinDao.Update(entity);
		}

		public Skin GetSkin(long ID)
		{
			return SkinDao.Retrieve(ID);
		}

		public Skin GetSkin(string skinName)
		{
			return SkinDao.Retrieve(skinName);
		}

		public IList<Skin> GetAllSkins()
		{
			return SkinDao.RetrieveAll();
		}

		public IList<Skin> GetAllSkins(DateTime changedSince)
		{
			return SkinDao.RetrieveAll(changedSince);
		}

		public void DeleteSkin(long ID)
		{
			SkinDao.Delete(ID);
		}

		public void CreateSkinItem(SkinItem entity)
		{
			SkinItemDao.Create(entity);
		}

		public void UpdateSkinItem(SkinItem entity)
		{
			SkinItemDao.Update(entity);
		}

		public SkinItem GetSkinItem(long id)
		{
			return SkinItemDao.Retrieve(id);
		}

		public SkinItem GetSkinItem(long skinId, SkinItemTypeEnum skinItemType, string fileName)
		{
			return SkinItemDao.Retrieve(skinId, skinItemType, fileName);
		}

		public IList<SkinItem> GetAllSkinItems(long skinId)
		{
			return SkinItemDao.RetrieveAll(skinId);
		}

		public IList<SkinItem> GetAllSkinItems(long skinId, SkinItemTypeEnum skinItemType)
		{
			return SkinItemDao.RetrieveAll(skinId, skinItemType);
		}

		public IList<SkinItem> GetAllSkinItems(long skinId, DateTime changedSince)
		{
			return SkinItemDao.RetrieveAll(skinId, changedSince);
		}

		public IList<SkinItem> GetAllSkinItems(long skinId, SkinItemTypeEnum skinItemType, DateTime changedSince)
		{
			return SkinItemDao.RetrieveAll(skinId, skinItemType, changedSince);
		}

		public void DeleteSkinItem(long id)
		{
			SkinItemDao.Delete(id);
		}

		public void CreateSyncJob(SyncJob job)
		{
            if (!Config.SyncJobLoggingDisabled)
            {
                SyncJobDao.Create(job);
            }
		}

        public void CreateAsyncJob(AsyncJob job)
        {
            AsyncJobDao.Create(job);
		}

		public void UpdateAsyncJob(AsyncJob job)
		{
            AsyncJobDao.Update(job);
		}

		public AsyncJob GetAsyncJobById(long jobId)
		{
			return AsyncJobDao.Retrieve(jobId);
		}

		public AsyncJob GetAsyncJobByJobNumber(long jobNumber)
		{
			return AsyncJobDao.RetrieveByJobNumber(jobNumber);
		}

		public IList<AsyncJob> GetAsyncJobs(List<Dictionary<string, object>> parms)
		{
			return AsyncJobDao.RetrieveAll(parms);
		}

		public IList<AsyncJob> GetAllAsyncJobs()
		{
			return AsyncJobDao.RetrieveAll();
		}

		public void CreateAsyncJobProcessedObjects(List<AsyncJobProcessedObjects> objects)
		{
            AsyncJobProcessedObjectDao.Create(objects);
		}

		public void Update(List<AsyncJobProcessedObjects> objects)
		{
            AsyncJobProcessedObjectDao.Update(objects);
		}

		public List<long> GetAsyncJobProcessedObjectIdsByJobName(string jobName, string objectName)
		{
			return AsyncJobProcessedObjectDao.RetrieveObjectIds(jobName, objectName) ?? new List<long>();
		}

		public List<long> GetAsyncJobProcessedObjectIdsByJobNumber(long jobNumber, string objectName)
		{
			return AsyncJobProcessedObjectDao.RetrieveObjectIds(jobNumber, objectName) ?? new List<long>();
		}

		public long GetNextID(string objectName, int howMany)
		{
			var dsc = Config;
			lock (dsc.IdGenMutex)
			{
				// Create bucket if needed for this objectName
				if (!dsc.IdGenBuckets.ContainsKey(objectName))
				{
					dsc.IdGenBuckets.Add(objectName, new IDGenStats());
				}

				// Allocate more IDs in bucket if needed
				if (!dsc.IdGenBuckets[objectName].hasEnoughIDs(howMany))
				{
					dsc.IdGenBuckets[objectName].CurrentId = -1;
					dsc.IdGenBuckets[objectName].LastId = -1;
					dsc.IdGenBuckets = IdGeneratorDao.ReplenishIDs(objectName, howMany, dsc.BucketSize, dsc.IdGenBuckets);
				}

				// Allocate the IDs
				long id = dsc.IdGenBuckets[objectName].CurrentId;
				dsc.IdGenBuckets[objectName].CurrentId += howMany;
				return id;
			}
		}

		public void CreateAuditLogConfig(AuditLogConfig cfg)
		{
			AuditLogConfigDao.Create(cfg);
            if (AuditableObjects.ContainsKey(cfg.TypeName))
            {
                AuditableObjects.Remove(cfg.TypeName);
            }
            AuditableObjects[cfg.TypeName] = cfg;
		}

		public AuditLogConfig UpdateAuditLogConfig(AuditLogConfig cfg)
		{
			AuditLogConfigDao.Update(cfg);
            if (AuditableObjects.ContainsKey(cfg.TypeName))
			{
                AuditableObjects.Remove(cfg.TypeName);
			}
            AuditableObjects.Add(cfg.TypeName, cfg);
			return cfg;
		}

		public AuditLogConfig GetAuditLogConfig(string typeName)
		{
            return AuditableObjects.ContainsKey(typeName) ? AuditableObjects[typeName] : null;
		}

		public List<AuditLogConfig> GetAuditLogConfigs()
		{
            return AuditLogConfigDao.RetrieveAll();
		}

		public void DeleteAuditLogConfig(long id)
		{
			AuditLogConfigDao.Delete(id);
		}

		public void CreateAuditObject(LWObjectAuditLogBase arc)
		{
            Database.Insert(arc);
		}

		public string GetClientConfigProp(string key)
		{
			string result = string.Empty;
			ClientConfiguration config = GetClientConfiguration(key);
			if (config != null && !string.IsNullOrEmpty(config.Value))
			{
				result = config.Value;
			}
			return result;
		}

		public void SetClientConfigProp(string key, string value)
		{
			ClientConfiguration config = GetClientConfiguration(key);
			if (config != null)
			{
				config.Value = StringUtils.FriendlyString(value);
				config.ExternalValue = false;
				UpdateClientConfiguration(config);
			}
			else
			{
				config = new ClientConfiguration();
				config.ExternalValue = false;
				config.Key = key;
				config.Value = value;
				CreateClientConfiguration(config);
			}
		}

		public ClientConfiguration GetClientConfiguration(string key)
		{
			ClientConfiguration config = null;
			try
			{
				config = (ClientConfiguration)CacheManager.Get(CacheRegions.ClientConfigurationByKey, key);
				if (config == null)
				{
					config = ClientConfigurationDao.Retrieve(key);
					if (config != null)
					{
						CacheManager.Update(CacheRegions.ClientConfigurationByKey, config.Key, config);
					}
				}
			}
			catch
			{
				config = null;
			}
			return config;
		}

		public IList<ClientConfiguration> GetAllClientConfigurations()
		{
			return ClientConfigurationDao.RetrieveAll() ?? new List<ClientConfiguration>();
		}

		public IList<ClientConfiguration> GetAllClientConfigurationsLike(string keyPattern)
		{
			return ClientConfigurationDao.RetrieveAllLike(keyPattern) ?? new List<ClientConfiguration>();
		}

		public IList<string> GetAllClientConfigurationKeys()
		{
			return ClientConfigurationDao.RetrieveAllKeys() ?? new List<string>();
		}

		public IList<ClientConfiguration> GetAllExternalClientConfigurations()
		{
			return ClientConfigurationDao.RetrieveAllExternal() ?? new List<ClientConfiguration>();
		}

		public IList<ClientConfiguration> GetAllExternalClientConfigurationsByFolder(long folderId)
		{
			return ClientConfigurationDao.RetrieveAllExternalByFolder(folderId);
		}

		public IList<ClientConfiguration> GetAllChangedClientConfigurations(DateTime since, bool onlyExternal)
		{
			return ClientConfigurationDao.RetrieveChangedObjects(since, onlyExternal) ?? new List<ClientConfiguration>();
		}

		public void CreateClientConfiguration(ClientConfiguration config)
		{
			_logger.Trace(_className, "CreateClientConfiguration", "Creating new client configuration " + config.Key);
			ClientConfigurationDao.Create(config);
			CacheManager.Update(CacheRegions.ClientConfigurationByKey, config.Key, config);
		}

		public void UpdateClientConfiguration(ClientConfiguration config)
		{
			ClientConfigurationDao.Update(config);
			CacheManager.Update(CacheRegions.ClientConfigurationByKey, config.Key, config);
		}

		public void DeleteClientConfiguration(string key)
		{
			_logger.Trace(_className, "DeleteClientConfiguration", "Deleting configuration with key = " + key);
			ClientConfigurationDao.Delete(key);
			CacheManager.Remove(CacheRegions.ClientConfigurationByKey, key);
		}

        public void SaveRemoteAssembly(RemoteAssembly assembly)
        {
            List<RemoteAssembly> assemblies = (List<RemoteAssembly>)CacheManager.Get(CacheRegions.RemoteAssemblies, "all");
            if (assemblies == null)
                assemblies = new List<RemoteAssembly>();

            RemoteAssembly existing = GetRemoteAssembly(assembly.AssemblyFileName);
            if (existing != null)
            {
                assemblies.Remove(assembly);
                existing.Assembly = assembly.Assembly;
                existing.AssemblyFileName = assembly.AssemblyFileName;
                existing.AssemblyName = assembly.AssemblyName;
                RemoteAssemblyDao.Update(existing);
            }
            else
                RemoteAssemblyDao.Create(assembly);

            
            assemblies.Add(assembly);
            CacheManager.Update(CacheRegions.RemoteAssemblies, "all", assemblies);
        }

        public void DeleteRemoteAssembly(RemoteAssembly assembly)
        {
            RemoteAssemblyDao.Delete(assembly.Id);

            List<RemoteAssembly> assemblies = (List<RemoteAssembly>)CacheManager.Get(CacheRegions.RemoteAssemblies, "all");
            if (assemblies == null)
                assemblies = new List<RemoteAssembly>();
            assemblies.Remove(assembly);
            CacheManager.Update(CacheRegions.RemoteAssemblies, "all", assemblies);
        }

        public RemoteAssembly GetRemoteAssembly(long id)
        {
            List<RemoteAssembly> assemblies = GetAllRemoteAssemblies();
            if (assemblies == null || assemblies.Count == 0)
                return null;
            return assemblies.Where(x => x.Id == id).FirstOrDefault();
        }

        public RemoteAssembly GetRemoteAssembly(string filename)
        {
            List<RemoteAssembly> assemblies = GetAllRemoteAssemblies();
            if (assemblies == null || assemblies.Count == 0)
                return null;
            return assemblies.Where(x => 
                     x.AssemblyFileName.ToLower() == filename.ToLower() ||
                     x.AssemblyFileName.ToLower().Replace(".dll", string.Empty) == filename.ToLower().Replace(".dll", string.Empty)
                   ).FirstOrDefault();
        }

        public List<RemoteAssembly> GetAllRemoteAssemblies()
        {
            List<RemoteAssembly> assemblies = (List<RemoteAssembly>)CacheManager.Get(CacheRegions.RemoteAssemblies, "all");
            if (assemblies == null)
            {
                assemblies = RemoteAssemblyDao.RetrieveAll();
                CacheManager.Update(CacheRegions.RemoteAssemblies, "all", assemblies);
            }
            return assemblies;
        }

        public RemoteAssembly.ComponentReference GetComponentReference(CustomComponentTypeEnum componentType, string name)
        {
            List<RemoteAssembly> assemblies = GetAllRemoteAssemblies();
            if (assemblies == null || assemblies.Count == 0)
                return null;

            foreach(RemoteAssembly assembly in assemblies)
            {
                var reference = assembly.GetReferenceByName(componentType, name);
                if (reference != null)
                    return reference;
            }

            return null;
        }

        public void CreateBscriptExpression(Bscript bscript)
		{
			BScriptDao.Create(bscript);
			CacheManager.Update(CacheRegions.BScriptExpressionByName, bscript.Name, bscript);
		}

		public void UpdateBscriptExpression(Bscript bscript)
		{
			BScriptDao.Update(bscript);
			CacheManager.Update(CacheRegions.BScriptExpressionByName, bscript.Name, bscript);
		}

		public IList<Bscript> GetAllBscriptExpressions()
		{
			IList<Bscript> exprList = BScriptDao.RetrieveAll() ?? new List<Bscript>();
			foreach (Bscript expr in exprList)
			{
				CacheManager.Update(CacheRegions.BScriptExpressionByName, expr.Name, expr);
			}
			return exprList;
		}

		public IList<Bscript> GetAllBscriptExpressions(long[] ids)
		{
			return BScriptDao.RetrieveAll(ids) ?? new List<Bscript>();
		}

		public IList<Bscript> GetAllBscriptExpressions(string[] names)
		{
			return BScriptDao.RetrieveAll(names) ?? new List<Bscript>();
		}

		public Bscript GetBscriptExpression(long bsId)
		{
			Bscript expr = BScriptDao.Retrieve(bsId);
			if (expr != null)
			{
				CacheManager.Update(CacheRegions.BScriptExpressionByName, expr.Name, expr);
			}
			return expr;
		}

		public Bscript GetBscriptExpression(string bsName)
		{
			Bscript expr = (Bscript)CacheManager.Get(CacheRegions.BScriptExpressionByName, bsName);
			if (expr == null)
			{
				expr = BScriptDao.Retrieve(bsName);
				if (expr != null)
				{
					CacheManager.Update(CacheRegions.BScriptExpressionByName, bsName, expr);
				}
			}
			return expr;
		}

		public IEnumerable<Bscript> SearchBScriptExpressions(string search, ExpressionContexts context, string currentConditionAttributeSet, int maxResults)
		{
			return BScriptDao.Search(search, context, currentConditionAttributeSet, maxResults);
		}

		public IList<Bscript> GetAllChangedBscriptExpressions(DateTime since)
		{
			return BScriptDao.RetrieveChangedObjects(since) ?? new List<Bscript>();
		}

		public void DeleteBscriptExpression(long bsId)
		{
			Bscript bscript = GetBscriptExpression(bsId);
			if (bscript != null)
			{
				BScriptDao.Delete(bsId);
				CacheManager.Remove(CacheRegions.BScriptExpressionByName, bscript.Name);
			}
		}

		public void DeleteBscriptExpression(string bsName)
		{
			BScriptDao.Delete(bsName);
			CacheManager.Remove(CacheRegions.BScriptExpressionByName, bsName);
		}

		public void LogLIBMessage(LIBMessageLog msg)
		{
			const string methodName = "LogLIBMessage";

            if (Config.LibMessageLoggingDisabled)
            {
                return;
            }

			if (string.IsNullOrEmpty(msg.Reason) && msg.Exception != null)
			{
				msg.Reason = msg.Exception.Message;
			}
			if (msg.EnvKey.Length > 150)
			{
				msg.EnvKey = msg.EnvKey.Substring(0, 150);
			}
			msg.LogSource = string.IsNullOrEmpty(msg.LogSource) ? " " : msg.LogSource;
			if (msg.LogSource.Length > 150)
			{
				msg.LogSource = msg.LogSource.Substring(0, 150);
			}
			if (!string.IsNullOrEmpty(msg.Reason) && msg.Reason.Length > 500)
			{
				msg.Reason = msg.Reason.Substring(0, 500);
			}
			msg.FileName = string.IsNullOrEmpty(msg.FileName) ? " " : msg.FileName;
			if (!string.IsNullOrEmpty(msg.FileName) && msg.FileName.Length > 255)
			{
				msg.FileName = msg.FileName.Substring(0, 255);
			}

			msg.Error = msg.Exception != null ? msg.Exception.Message + msg.Exception.StackTrace : "";

			try
			{
				LIBMessageLog existing = null;
				if (msg.MessageId > 0)
				{
					LibMessageLogDao.Retrieve(msg.MessageId);
				}
				if (existing == null)
				{
					LibMessageLogDao.Create(msg);
				}
				else
				{
					msg.TryCount = existing.TryCount + 1;
					LibMessageLogDao.Update(msg);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error writing LIB log message", ex);
			}
		}

		public LIBMessageLog RetrieveLIBMessage(long messageId)
		{
			return LibMessageLogDao.Retrieve(messageId);
		}

		public IList<LIBMessageLog> RetrieveLIBMessagesByJobNumber(long jobNumber)
		{
			IList<LIBMessageLog> msgList = LibMessageLogDao.RetrieveByJobNumber(jobNumber);
			if (msgList == null)
			{
				msgList = new List<LIBMessageLog>();
			}
			return msgList;
		}

		public int HowManyLIBMessages(long jobNumber)
		{
			return LibMessageLogDao.HowMany(jobNumber);
		}

		public long GetNextID(string objectName)
		{
			return GetNextID(objectName, 1);
		}

		public void CreateIDGenerator(string objectName)
		{
			IdGeneratorDao.CreateIDGenerator(objectName);
		}

		public long GetNextSequence()
		{
			return IdGeneratorDao.GetNextSequence();
		}

		public void CreateArchiveObject(ArchiveObject archive)
		{
			ArchiveObjectDao.Create(archive);
		}

		public void UpdateMessageDef(ArchiveObject archive)
		{
			ArchiveObjectDao.Update(archive);
		}

		public IList<ArchiveObject> GetArchiveObjectByGroup(long groupId, long runNumber)
		{
			return ArchiveObjectDao.RetrieveByGroup(groupId, runNumber) ?? new List<ArchiveObject>();
		}

		public void DeleteArchiveObject(long id)
		{
			ArchiveObjectDao.Delete(id);
		}

		public void TestConnection()
		{
			Database.OpenSharedConnection();
			Database.CloseSharedConnection();
		}

		public void CreateCacheRefresh(string username)
		{
			CacheRefreshDao.Create(new CacheRefresh() { InitiatedBy = username });
		}

		public CacheRefresh GetLatestCacheRefresh()
		{
			return CacheRefreshDao.RetrieveMostRecent();
		}

	}
}
