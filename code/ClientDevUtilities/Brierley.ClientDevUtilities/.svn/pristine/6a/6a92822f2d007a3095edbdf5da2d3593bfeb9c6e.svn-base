using System;
using System.Collections.Generic;
using System.Data.Common;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.Cache;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Dmc;
using Brierley.FrameWork.Email;

namespace Brierley.FrameWork.Data
{
	/// <summary>
	/// Contains configuration data needed to instantiate a data service object.
	/// </summary>
	public sealed class ServiceConfig
	{
		private const int _maxFailedPasswordAttempts = 3;
		private ICommunicationLogger _smsLogger = null;
		private ICommunicationLogger _emailLogger = null;
		public EmailProviderType? _emailProviderType = null;

		public string Organization { get; private set; }
		public string Environment { get; private set; }
		public string ConnectString { get; private set; }
		public SupportedDataSourceType DatabaseType { get; private set; }
		public DbProviderFactory Factory { get; private set; }
		public IDataCacheProvider CacheManager { get; private set; }
		public int? BulkLoadingBatchSize { get; set; }
		public bool DebitPayOffMethodOn { get; set; }
		public bool DebitPayOffPointTypeRestrictionOn { get; set; }
		public bool DebitPayOffPointEventRestrictionOn { get; set; }

        //logging tables
        public bool LibMessageLoggingDisabled { get; set; }
        public bool SyncJobLoggingDisabled { get; set; }
        public bool RuleExecutionLoggingDisabled { get; set; }
        public bool TriggerUserEventLoggingDisabled { get; set; }


        public CampaignConfig CampaignConfig { get; internal set; }

		public int MaxFailedPasswordAttempts
		{
			get
			{
				return _maxFailedPasswordAttempts;
			}
		}

		public EmailProviderType? EmailProviderType
		{
			get
			{
				if (!_emailProviderType.HasValue)
				{
					var provider = LWConfigurationUtil.GetConfigurationValue("LWEmailProvider");
					switch (provider)
					{
						case "aws":
						case "AWS":
							_emailProviderType = Brierley.FrameWork.Common.EmailProviderType.Aws;
							break;
						case "dmc":
						case "DMC":
							_emailProviderType = Brierley.FrameWork.Common.EmailProviderType.Dmc;
							break;
						case "custom":
							_emailProviderType = Brierley.FrameWork.Common.EmailProviderType.Custom;
							break;
					}
				}
				return _emailProviderType;
			}
		}
        
		public ICommunicationLogger EmailLogger
		{
			get
			{
				if (_emailLogger == null)
				{
					string assemblyName = LWConfigurationUtil.GetConfigurationValue("LW_EmailLoggerAssembly");
					if (!string.IsNullOrEmpty(assemblyName))
					{
						string className = LWConfigurationUtil.GetConfigurationValue("LW_EmailLoggerClass");
						if (string.IsNullOrEmpty(className))
						{
							throw new Exception("Failed to load Email Logger assembly. LW_EmailLoggerClass has not been configured.");
						}

						_emailLogger = ClassLoaderUtil.CreateInstance(assemblyName, className) as ICommunicationLogger;
						if (_emailLogger == null)
						{
							throw new Exception(string.Format("Failed to resolve Email Logger {0}, {1}", assemblyName, className));
						}
						return _emailLogger;
					}

					if(!EmailProviderType.HasValue)
					{
						throw new Exception("Could not determine the email communication logger to use because no email provider has been configured. Please ensure configuration value \"LWEmailProvider\" is set to a valid value (aws, dmc or custom).");
					}

					switch (EmailProviderType.Value)
					{
						case Brierley.FrameWork.Common.EmailProviderType.Aws:
							_emailLogger = new AwsEmailQueueLogger();
							break;
						case Common.EmailProviderType.Dmc:
							_emailLogger = new DmcEmailQueueLogger();
							break;
						case Common.EmailProviderType.Custom:
							//email provider is custom and no logger has been configured. use null logger
							_emailLogger = new NullCommunicationLogger();
								break;
					}
				}
				return _emailLogger;
			}
		}

		public ICommunicationLogger SmsLogger
		{
			get
			{
				if (_smsLogger == null)
				{
					string assemblyName = LWConfigurationUtil.GetConfigurationValue("LW_SmsLoggerAssembly");

					if (string.IsNullOrEmpty(assemblyName))
					{
						_smsLogger = new DmcSmsQueueLogger();
					}
					else
					{
						string className = LWConfigurationUtil.GetConfigurationValue("LW_SmsLoggerClass");
						if (string.IsNullOrEmpty(className))
						{
							throw new Exception("Failed to load SMS Logger assembly. LW_SmsLoggerClass has not been configured.");
						}

						_smsLogger = ClassLoaderUtil.CreateInstance(assemblyName, className) as ICommunicationLogger;
						if (_smsLogger == null)
						{
							throw new Exception(string.Format("Failed to resolve SMS Logger {0}, {1}", assemblyName, className));
						}
					}
				}
				return _smsLogger;
			}
		}

		public SuppressionSettings EmailSuppressionSettings { get; set; }

        private Dictionary<string, AuditLogConfig> _auditableObjects;
        public Dictionary<string, AuditLogConfig> AuditableObjects
        {
            get
            {
                if (_auditableObjects == null)
                {
                    // Get defaults
                    _auditableObjects = AuditLogConfig.GetDefaultLogging();
                    // Add settings from DB
                    using (var service = new DataService(this))
                    {
                        try
                        {
                            var configs = service.GetAuditLogConfigs();
                            foreach (AuditLogConfig config in configs)
                            {
                                if (_auditableObjects.ContainsKey(config.TypeName))
                                {
                                    _auditableObjects[config.TypeName].LoggingEnabled = config.LoggingEnabled;
                                }
                                else
                                {
                                    _auditableObjects.Add(config.TypeName, config);
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            if (!ex.Message.Contains("ORA-00942"))
                                throw;
                        }
                    }
                }
                return _auditableObjects;
            }
            set
            {
                _auditableObjects = value;
            }
        }

		public ServiceConfig(string organization, string environment, string connectString, SupportedDataSourceType databaseType, DbProviderFactory factory, IDataCacheProvider cacheManager)
		{
			Organization = organization;
			Environment = environment;
			ConnectString = connectString;
			DatabaseType = databaseType;
			Factory = factory;
			CacheManager = cacheManager;
			IdGenBuckets = new Dictionary<string, IDGenStats>();
			IdGenMutex = new object();
		}

		internal int BucketSize { get; set; }
		internal Dictionary<string, IDGenStats> IdGenBuckets { get; set; }
		internal object IdGenMutex { get; set; }

		/// <summary>
		/// Opens a new database connection every time this property is accessed.
		/// </summary>
		public PetaPoco.Database CreateDatabase()
		{
			PetaPoco.Database database;
			if (this.Factory != null)
			{
				database = new PetaPoco.Database(this.ConnectString, this.Factory);
			}
			else
			{
				database = new PetaPoco.Database(this.ConnectString, "System.Data.SqlClient");
			}
			database.OpenSharedConnection();
			return database;
		}
	}
}
