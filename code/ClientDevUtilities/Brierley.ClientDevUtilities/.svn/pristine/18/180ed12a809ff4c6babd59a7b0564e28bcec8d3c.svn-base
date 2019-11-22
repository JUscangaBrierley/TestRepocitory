//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.Mappers;
using PetaPoco;
using Brierley.FrameWork.Push;

namespace Brierley.FrameWork.Data
{
	public static class LWDataServiceUtil
	{
		private const string _className = "LWDataServiceUtil";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private static Dictionary<string, ServiceConfig> _serviceConfigurationMap = new Dictionary<string, ServiceConfig>();

		static LWDataServiceUtil()
		{
			PetaPoco.Mappers.Register(typeof(Member), new MemberMapper());

			PetaPoco.Mappers.Register(typeof(X509Cert), new X509CertMapper());
			PetaPoco.Mappers.Register(typeof(SkinItem), new SkinItemMapper());

			var vcMapper = new VirtualCardMapper();
			PetaPoco.Mappers.Register(typeof(VirtualCard), vcMapper);
			PetaPoco.Mappers.Register(typeof(VirtualCard_AL), vcMapper);

			var rewardMapper = new MemberRewardMapper();
			PetaPoco.Mappers.Register(typeof(MemberReward), rewardMapper);
			PetaPoco.Mappers.Register(typeof(MemberReward_AL), rewardMapper);

			var templateMapper = new TemplateMapper();
			PetaPoco.Mappers.Register(typeof(Template), templateMapper);
			PetaPoco.Mappers.Register(typeof(Template_AL), templateMapper);

			PetaPoco.Mappers.Register(typeof(Campaign), new CampaignMapper());
			PetaPoco.Mappers.Register(typeof(Step), new StepMapper());

			var attributeMapper = new AttributeMapper();
			PetaPoco.Mappers.Register(typeof(AttributeMetaData), attributeMapper);
			PetaPoco.Mappers.Register(typeof(AttributeMetaData_AL), attributeMapper);

			var contentAttributeMapper = new ContentAttributeMapper();
			PetaPoco.Mappers.Register(typeof(ContentAttribute), contentAttributeMapper);
			PetaPoco.Mappers.Register(typeof(ContentAttribute_AL), contentAttributeMapper);
			//LangChanContent has LangchanType, which is ContentObjType, so it can share (for now, at least) the ContentAttributeMapper
			PetaPoco.Mappers.Register(typeof(LangChanContent), contentAttributeMapper);

			PetaPoco.Mappers.Register(typeof(ContentAttributeDef), new ContentAttributeDefMapper());

			PetaPoco.Mappers.Register(typeof(SocialCampaignData), new SocialCampaignMapper());

			PetaPoco.Mappers.Register(typeof(MemberMobileEvent), new MemberMobileEventMapper());
			PetaPoco.Mappers.Register(typeof(ScheduledJobRun), new ScheduledJobRunMapper());

			PetaPoco.Mappers.Register(typeof(SMRespondentList), new SMRespondentListMapper());

			PetaPoco.Mappers.Register(typeof(RuleExecutionLog), new RuleExecutionLogMapper());
			PetaPoco.Mappers.Register(typeof(PromotionCertificate), new PromotionCertificateMapper());

			PetaPoco.Mappers.Register(typeof(MemberBonus), new MemberBonusMapper());
			PetaPoco.Mappers.Register(typeof(MemberCoupon), new MemberCouponMapper());

            PetaPoco.Mappers.Register(typeof(ArchiveObject), new ArchiveObjectMapper());
		}

		public static void Reinitialize()
		{
			var ctx = GetCurrentContext();
			Reinitialize(ctx.Organization, ctx.Environment);
		}

		public static void Reinitialize(string orgName, string envName)
		{
			string key = DataServiceUtil.GetKey(orgName, envName);
			lock (_serviceConfigurationMap)
			{
				if (_serviceConfigurationMap.ContainsKey(key))
				{
					_serviceConfigurationMap.Remove(key);
				}
			}
		}


		public static ServiceConfig GetServiceConfiguration()
		{
			var ctx = GetCurrentContext();
			return GetServiceConfiguration(ctx.Organization, ctx.Environment);
		}

		public static ServiceConfig GetServiceConfiguration(LWConfiguration lwConfig)
		{
			return GetServiceConfiguration(lwConfig.Organization, lwConfig.Environment);
		}

		public static ServiceConfig GetServiceConfiguration(string orgName, string envName)
		{
			const string methodName = "GetDataServiceConfiguraton";

			lock (_serviceConfigurationMap)
			{
				string key = DataServiceUtil.GetKey(orgName, envName);
				if (_serviceConfigurationMap.ContainsKey(key))
				{
					return _serviceConfigurationMap[key];
				}

				LWConfiguration config = LWConfigurationUtil.GetConfiguration(orgName, envName);
				if (config == null)
				{
					throw new Exception(string.Format("Could not load configuration for {0}", key));
				}

				string connectString = config.DBConfig.GetEncodedConnectionString();
				if (string.IsNullOrEmpty(connectString))
				{
					throw new LWException("Configured database has no connection string");
				}
				connectString = CryptoUtil.DecodeUTF8(connectString);

				var serviceConfig = new ServiceConfig(
					orgName,
					envName,
					connectString,
					config.DBConfig.DBType,
					ResolveProvider(config.DBConfig.DBType),
					new Brierley.FrameWork.Data.Cache.MSCaching.MSCacheProvider());

                LoadRemoteAssemblies(serviceConfig);

				string cfgValue = LWConfigurationUtil.GetConfigurationValue("LW_CacheExpirationInterval");
				if (!string.IsNullOrEmpty(cfgValue))
				{
					_logger.Trace(_className, methodName, "Setting cache expiration interval to " + cfgValue);
					serviceConfig.CacheManager.ExpirationInterval = int.Parse(cfgValue);
				}
				string debitPayOffMethodOn = LWConfigurationUtil.GetConfigurationValue("LW_PointAccrualDebitPayoff");
				if (!string.IsNullOrEmpty(debitPayOffMethodOn))
				{
					_logger.Trace(_className, methodName, "Setting method for point accruals " + debitPayOffMethodOn);
					serviceConfig.DebitPayOffMethodOn = bool.Parse(debitPayOffMethodOn);
				}
				string debitPayOffPointTypeRestrictionOn = LWConfigurationUtil.GetConfigurationValue("LW_DebitPayoffPointTypeRestriction"); // LW-1301
				if (!string.IsNullOrEmpty(debitPayOffPointTypeRestrictionOn))
				{
					_logger.Trace(_className, methodName, "Setting method for point accruals " + debitPayOffPointTypeRestrictionOn);
					serviceConfig.DebitPayOffPointTypeRestrictionOn = bool.Parse(debitPayOffPointTypeRestrictionOn);
				}
				string debitPayOffPointEventRestrictionOn = LWConfigurationUtil.GetConfigurationValue("LW_DebitPayoffPointEventRestriction"); // LW-1301
				if (!string.IsNullOrEmpty(debitPayOffPointEventRestrictionOn))
				{
					_logger.Trace(_className, methodName, "Setting method for point accruals " + debitPayOffPointEventRestrictionOn);
					serviceConfig.DebitPayOffPointEventRestrictionOn = bool.Parse(debitPayOffPointEventRestrictionOn);
				}
				string bulkBatchSize = LWConfigurationUtil.GetConfigurationValue("BulkLoadingBatchSize");
				if (!string.IsNullOrEmpty(bulkBatchSize))
				{
					_logger.Trace(_className, methodName, "Setting bulk loading batch size " + bulkBatchSize);
					serviceConfig.BulkLoadingBatchSize = int.Parse(bulkBatchSize);
				}
				serviceConfig.BucketSize = StringUtils.FriendlyInt32(LWConfigurationUtil.GetConfigurationValue("IDGenBucketSize"), 50);

                string libMessageLoggingDisabled = LWConfigurationUtil.GetConfigurationValue("LibMessageLoggingDisabled");
                if (!string.IsNullOrEmpty(libMessageLoggingDisabled))
                {
                    serviceConfig.LibMessageLoggingDisabled = bool.Parse(libMessageLoggingDisabled);
                    _logger.Trace(_className, methodName, "Setting LibMessageLoggingDisabled " + libMessageLoggingDisabled);
                }

                string syncJobLoggingDisabled = LWConfigurationUtil.GetConfigurationValue("SyncJobLoggingDisabled");
                if (!string.IsNullOrEmpty(syncJobLoggingDisabled))
                {
                    serviceConfig.SyncJobLoggingDisabled = bool.Parse(syncJobLoggingDisabled);
                    _logger.Trace(_className, methodName, "Setting SyncJobLoggingDisabled " + syncJobLoggingDisabled);
                }

                string ruleExecutionLoggingDisabled = LWConfigurationUtil.GetConfigurationValue("RuleExecutionLoggingDisabled");
                if (!string.IsNullOrEmpty(ruleExecutionLoggingDisabled))
                {
                    serviceConfig.RuleExecutionLoggingDisabled = bool.Parse(ruleExecutionLoggingDisabled);
                    _logger.Trace(_className, methodName, "Setting RuleExecutionLoggingDisabled " + ruleExecutionLoggingDisabled);
                }

                string triggerUserEventLoggingDisabled = LWConfigurationUtil.GetConfigurationValue("TriggerUserEventLoggingDisabled");
                if (!string.IsNullOrEmpty(triggerUserEventLoggingDisabled))
                {
                    serviceConfig.TriggerUserEventLoggingDisabled = bool.Parse(triggerUserEventLoggingDisabled);
                    _logger.Trace(_className, methodName, "Setting TriggerUserEventLoggingDisabled " + triggerUserEventLoggingDisabled);
                }
				
				//email suppression settings
				serviceConfig.EmailSuppressionSettings = new Email.SuppressionSettings();

				Email.SuppressionSettings.SuppressionRule permanentRule = new Email.SuppressionSettings.SuppressionRule();
				string permanentBounceRule = LWConfigurationUtil.GetConfigurationValue(Email.Constants.PermanentRule);
				if(!string.IsNullOrEmpty(permanentBounceRule))
				{
					EmailBounceRuleType permanentRuleType = EmailBounceRuleType.Strict;
					Enum.TryParse<EmailBounceRuleType>(permanentBounceRule, out permanentRuleType);
					permanentRule.Type = permanentRuleType;

					string limit = LWConfigurationUtil.GetConfigurationValue(Email.Constants.PermanentLimit);
					if (!string.IsNullOrEmpty(limit))
					{
						permanentRule.Limit = int.Parse(limit);
					}

					string interval = LWConfigurationUtil.GetConfigurationValue(Email.Constants.PermanentInterval);
					if (!string.IsNullOrEmpty(interval))
					{
						permanentRule.Interval = int.Parse(interval);
					}
				}
				serviceConfig.EmailSuppressionSettings.PermanentBounceRule = permanentRule;


				Email.SuppressionSettings.SuppressionRule transientRule = new Email.SuppressionSettings.SuppressionRule();
				string transientBounceRule = LWConfigurationUtil.GetConfigurationValue(Email.Constants.TransientRule);
				if (!string.IsNullOrEmpty(transientBounceRule))
				{
					EmailBounceRuleType transientRuleType = EmailBounceRuleType.Sliding;
					Enum.TryParse<EmailBounceRuleType>(transientBounceRule, out transientRuleType);
					transientRule.Type = transientRuleType;

					string limit = LWConfigurationUtil.GetConfigurationValue(Email.Constants.TransientLimit);
					if (!string.IsNullOrEmpty(limit))
					{
						transientRule.Limit = int.Parse(limit);
					}

					string interval = LWConfigurationUtil.GetConfigurationValue(Email.Constants.TransientInterval);
					if (!string.IsNullOrEmpty(interval))
					{
						transientRule.Interval = int.Parse(interval);
					}					
				}
				serviceConfig.EmailSuppressionSettings.TransientBounceRule = transientRule;


				Email.SuppressionSettings.SuppressionRule complaintRule = new Email.SuppressionSettings.SuppressionRule();
				string complaintBounceRule = LWConfigurationUtil.GetConfigurationValue(Email.Constants.ComplaintRule);
				if (!string.IsNullOrEmpty(complaintBounceRule))
				{
					EmailBounceRuleType complaintRuleType = EmailBounceRuleType.Strict;
					Enum.TryParse<EmailBounceRuleType>(complaintBounceRule, out complaintRuleType);
					complaintRule.Type = complaintRuleType;

					string limit = LWConfigurationUtil.GetConfigurationValue(Email.Constants.ComplaintLimit);
					if (!string.IsNullOrEmpty(limit))
					{
						complaintRule.Limit = int.Parse(limit);
					}

					string interval = LWConfigurationUtil.GetConfigurationValue(Email.Constants.ComplaintInterval);
					if (!string.IsNullOrEmpty(interval))
					{
						complaintRule.Interval = int.Parse(interval);
					}
				}
				serviceConfig.EmailSuppressionSettings.ComplaintBounceRule = complaintRule;


				Email.SuppressionSettings.SuppressionRule undeterminedRule = new Email.SuppressionSettings.SuppressionRule();
				string undeterminedBounceRule = LWConfigurationUtil.GetConfigurationValue(Email.Constants.UndeterminedRule);
				if (!string.IsNullOrEmpty(undeterminedBounceRule))
				{
					EmailBounceRuleType undeterminedRuleType = EmailBounceRuleType.Sliding;
					Enum.TryParse<EmailBounceRuleType>(undeterminedBounceRule, out undeterminedRuleType);
					undeterminedRule.Type = undeterminedRuleType;

					string limit = LWConfigurationUtil.GetConfigurationValue(Email.Constants.UndeterminedLimit);
					if (!string.IsNullOrEmpty(limit))
					{
						undeterminedRule.Limit = int.Parse(limit);
					}

					string interval = LWConfigurationUtil.GetConfigurationValue(Email.Constants.UndeterminedInterval);
					if (!string.IsNullOrEmpty(interval))
					{
						undeterminedRule.Interval = int.Parse(interval);
					}
				}
				serviceConfig.EmailSuppressionSettings.UndeterminedBounceRule = undeterminedRule;

				_serviceConfigurationMap.Add(key, serviceConfig);

				_logger.Trace(_className, methodName, string.Format("ServiceConfig created and added to the cache for {0}/{1}", orgName, envName));

				return serviceConfig;
			}
		}

        private static void LoadRemoteAssemblies(ServiceConfig serviceConfig)
        {
            const string methodName = "LoadRemoteAssemblies";
            try
            {
                using (DataService svc = new DataService(serviceConfig))
                {
                    // I'm genuinely not sure if purging is necessary since a new assembly will have new types with the exact same name but won't match the existing keys
                    // I think it's better to be safe than sorry. We aren't immediately removing any mappings, just updating them if they match and adding new ones.
                    DataServiceUtil.PurgeLoadedPetaPocoTypes();
                    svc.GetAllRemoteAssemblies();
                }
            }
            catch(Exception ex)
            {
                _logger.Error(_className, methodName, string.Format("Error loading remote assemblies while initializing service config for {0} : {1}"
                    , serviceConfig.Organization, serviceConfig.Environment), ex);
            }
        }


		public static DataService DataServiceInstance()
		{
			var ctx = GetCurrentContext();
			return DataServiceInstance(ctx.Organization, ctx.Environment);
		}

		public static DataService DataServiceInstance(LWConfiguration config)
		{
			return DataServiceInstance(config.Organization, config.Environment);
		}

		public static DataService DataServiceInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new DataService(config);
		}


		public static LoyaltyDataService LoyaltyDataServiceInstance()
		{
			var ctx = GetCurrentContext();
			return LoyaltyDataServiceInstance(ctx.Organization, ctx.Environment);
		}

		public static LoyaltyDataService LoyaltyDataServiceInstance(LWConfiguration config)
		{
			return LoyaltyDataServiceInstance(config.Organization, config.Environment);
		}

		public static LoyaltyDataService LoyaltyDataServiceInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new LoyaltyDataService(config);
		}


		public static ContentService ContentServiceInstance()
		{
			var ctx = GetCurrentContext();
			return ContentServiceInstance(ctx.Organization, ctx.Environment);
		}

		public static ContentService ContentServiceInstance(LWConfiguration config)
		{
			return ContentServiceInstance(config.Organization, config.Environment);
		}

		public static ContentService ContentServiceInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new ContentService(config);
		}


		public static TestingDataService TestingDataServiceInstance()
		{
			var ctx = GetCurrentContext();
			return TestingDataServiceInstance(ctx.Organization, ctx.Environment);
		}

		public static TestingDataService TestingDataServiceInstance(LWConfiguration config)
		{
			return TestingDataServiceInstance(config.Organization, config.Environment);
		}

		public static TestingDataService TestingDataServiceInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new TestingDataService(config);
		}
		
		public static EmailService EmailServiceInstance()
		{
			var ctx = GetCurrentContext();
			return EmailServiceInstance(ctx.Organization, ctx.Environment);
		}

		public static EmailService EmailServiceInstance(LWConfiguration config)
		{
			return EmailServiceInstance(config.Organization, config.Environment);
		}

		public static EmailService EmailServiceInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new EmailService(config);
		}

		public static SmsService SmsServiceInstance()
		{
			var ctx = GetCurrentContext();
			return SmsServiceInstance(ctx.Organization, ctx.Environment);
		}

		public static SmsService SmsServiceInstance(LWConfiguration config)
		{
			return SmsServiceInstance(config.Organization, config.Environment);
		}

		public static SmsService SmsServiceInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new SmsService(config);
		}

        public static PushService PushServiceInstance()
        {
            var ctx = GetCurrentContext();
            return PushServiceInstance(ctx.Organization, ctx.Environment);
        }

        public static PushService PushServiceInstance(LWConfiguration config)
        {
            return PushServiceInstance(config.Organization, config.Environment);
        }

        public static PushService PushServiceInstance(string orgName, string envName)
        {
            var config = GetServiceConfiguration(orgName, envName);
            return new PushService(config);
        }

        public static CSService CSServiceInstance()
		{
			var ctx = GetCurrentContext();
			return CSServiceInstance(ctx.Organization, ctx.Environment);
		}

		public static CSService CSServiceInstance(LWConfiguration config)
		{
			return CSServiceInstance(config.Organization, config.Environment);
		}

		public static CSService CSServiceInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new CSService(config);
		}


		public static MobileDataService MobileServiceInstance()
		{
			var ctx = GetCurrentContext();
			return MobileServiceInstance(ctx.Organization, ctx.Environment);
		}

		public static MobileDataService MobileServiceInstance(LWConfiguration config)
		{
			return MobileServiceInstance(config.Organization, config.Environment);
		}

		public static MobileDataService MobileServiceInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new MobileDataService(config);
		}


		public static SurveyManager SurveyManagerInstance()
		{
			var ctx = GetCurrentContext();
			return SurveyManagerInstance(ctx.Organization, ctx.Environment);
		}

		public static SurveyManager SurveyManagerInstance(LWConfiguration config)
		{
			return SurveyManagerInstance(config.Organization, config.Environment);
		}

		public static SurveyManager SurveyManagerInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new SurveyManager(config);
		}

		public static bool HasValidCampaignConfiguration()
		{
			lock (_serviceConfigurationMap)
			{
				var config = GetServiceConfiguration();
				if (config.CampaignConfig != null)
				{
					return config.CampaignConfig.BatchProvider != null;
				}

				var campaignConfig = CampaignConfiguration.GetConfiguration();
				return campaignConfig == null || campaignConfig.ConfigExists == false;
			}
		}

		public static CampaignManager CampaignManagerInstance()
		{
			var ctx = GetCurrentContext();
			return CampaignManagerInstance(ctx.Organization, ctx.Environment);
		}

		//public static CampaignManager CampaignManagerInstance(CampaignConfiguration campaignConfig)
		//{
		//	var config = GetServiceConfiguration();
		//	if()
		//	var ctx = GetCurrentContext();
		//	return CampaignManagerInstance(ctx.Organization, ctx.Environment);
		//}

		public static CampaignManager CampaignManagerInstance(LWConfiguration config)
		{
			return CampaignManagerInstance(config.Organization, config.Environment);
		}

		public static CampaignManager CampaignManagerInstance(string orgName, string envName)
		{
			const string methodName = "CampaignManagerInstance";
			lock (_serviceConfigurationMap)
			{
				var config = GetServiceConfiguration(orgName, envName);
				if (config.CampaignConfig != null)
				{
					return new CampaignManager(config);
				}

				var campaignConfig = CampaignConfiguration.GetConfiguration();
				if (campaignConfig == null || campaignConfig.ConfigExists == false)
				{
					throw new Exception("CampaignWare has not been configured. Please have a DB Manager complete configuration before using.");
				}

                //initialize data providers...
                SupportedDataSourceType? databaseType = null;
				string connectionString = null;

				if (campaignConfig.UseFramework)
				{
					LWConfiguration lwConfig = LWConfigurationUtil.GetCurrentConfiguration();
					switch (lwConfig.DBConfig.DBType)
					{
						case Brierley.FrameWork.Common.SupportedDataSourceType.MsSQL2005:
                            databaseType = SupportedDataSourceType.MsSQL2005;
							break;
						case Brierley.FrameWork.Common.SupportedDataSourceType.Oracle10g:
                            databaseType = SupportedDataSourceType.Oracle10g;
							break;
					}
					connectionString = CryptoUtil.DecodeUTF8(lwConfig.DBConfig.GetEncodedConnectionString());
				}
				else
				{
                    if(campaignConfig.DatabaseType.Equals("oracle", StringComparison.OrdinalIgnoreCase))
                    {
                        databaseType = SupportedDataSourceType.Oracle10g;
                    }
                    else if (campaignConfig.DatabaseType.Equals("mssql", StringComparison.OrdinalIgnoreCase))
                    {
                        databaseType = SupportedDataSourceType.MsSQL2005;
                    }
					connectionString = campaignConfig.ConnectionString;
				}

				if (databaseType == null)
				{
					_logger.Error(_className, methodName, "Database configuration does not exist. Module must be configured before the server may be used");
					throw new Exception("Configuration does not exist. The campaign module must be configured before the web service may be used.");
				}
				CampaignDataProvider provider = CampaignDataProviderUtil.GetInstance(databaseType.Value, connectionString, campaignConfig.DataSchema, campaignConfig.IndexTempTables, campaignConfig.UseArrayBinding);
				provider.UsesFramework = campaignConfig.UseFramework;

				config.CampaignConfig = new CampaignConfig()
					{
						BatchProvider = provider,
						ExecutionType = campaignConfig.ExecutionType,
						SendExecutionEmail = campaignConfig.SendExecutionEmail
					};

				if (config.CampaignConfig.BatchProvider != null && config.CampaignConfig.BatchProvider.UsesFramework)
				{
					config.CampaignConfig.RealTimeProvider = config.CampaignConfig.BatchProvider;
				}
				else
				{
					LWConfiguration lwConfig = LWConfigurationUtil.GetCurrentConfiguration();
					string connection = CryptoUtil.DecodeUTF8(lwConfig.DBConfig.GetEncodedConnectionString());
					config.CampaignConfig.RealTimeProvider = CampaignDataProviderUtil.GetInstance(databaseType.Value, connection, null, false, false);
				}
				return new CampaignManager(config);
			}
		}

		public static SocialDataService SocialServiceInstance()
		{
			var ctx = GetCurrentContext();
			return SocialServiceInstance(ctx.Organization, ctx.Environment);
		}

		public static SocialDataService SocialServiceInstance(LWConfiguration config)
		{
			return SocialServiceInstance(config.Organization, config.Environment);
		}

		public static SocialDataService SocialServiceInstance(string orgName, string envName)
		{
			var config = GetServiceConfiguration(orgName, envName);
			return new SocialDataService(config);
		}

	    public static RestAclDataService RestAclDataServiceInstance()
	    {
	        var ctx = GetCurrentContext();
	        return RestAclDataServiceInstance(ctx.Organization, ctx.Environment);
	    }

	    public static RestAclDataService RestAclDataServiceInstance(LWConfiguration config)
	    {
	        return RestAclDataServiceInstance(config.Organization, config.Environment);
	    }

	    public static RestAclDataService RestAclDataServiceInstance(string orgName, string envName)
	    {
	        var config = GetServiceConfiguration(orgName, envName);
            return new RestAclDataService(config);
	    }

		public static DbProviderFactory ResolveProvider(SupportedDataSourceType provider)
		{
			if (provider == SupportedDataSourceType.Oracle10g)
			{
				return new Oracle.ManagedDataAccess.Client.OracleClientFactory();
			}
			if (provider == SupportedDataSourceType.MsSQL2005)
			{
				DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
				return factory;
			}
			return null;
		}

		/// <summary>
		/// Retrieves a list of all current service configurations.
		/// </summary>
		/// <remarks>
		/// This method will not create a new instance. It only returns existing ones. Its intended use is for
		/// maintenance tasks in applications that may have a single data service (e.g., LW Portal) instance 
		/// or multiple instances (Loyalty Navigator).
		/// </remarks>
		/// <returns>List of ILWService</returns>
		public static List<ServiceConfig> ServiceInstances()
		{
			var ret = new List<ServiceConfig>();
			foreach (var config in _serviceConfigurationMap.Values)
			{
				ret.Add(config);
			}
			return ret;
		}


		private static LWConfigurationContext GetCurrentContext()
		{
			LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
			if (ctx == null)
			{
				string msg = string.Format("Unable to determine the current environment context. Please ensure that the environment context is set.");
				_logger.Critical(_className, "GetCurrentContext", msg);
				throw new LWConfigurationException(msg);
			}
			return ctx;
		}
	}
}
