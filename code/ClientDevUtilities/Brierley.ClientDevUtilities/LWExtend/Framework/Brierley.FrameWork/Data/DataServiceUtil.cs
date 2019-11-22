//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Brierley.FrameWork.CodeGen;
using Brierley.FrameWork.CodeGen.ClientDataModel;
using Brierley.FrameWork.CodeGen.Schema;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.IO;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Data
{
	public static class DataServiceUtil
	{
		public const string EntLibCacheProviderType = "Brierley.FrameWork.Data.Cache.MSCaching.MSCacheProvider";
		public const string AppFabricCacheProviderType = "Brierley.FrameWork.Data.Cache.AppFabric.AppFabricCacheProvider"; 
		
		private const string _className = "DataServiceUtil";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private static Dictionary<string, Type> _clientAssemblyTypeMap = new Dictionary<string, Type>();
        private static HashSet<Type> _petaPocoTypes = new HashSet<Type>();

		public class ResourceInfo
		{
			public string Path { get; set; }
			public string Uri { get; set; }

			public ResourceInfo()
			{
				Path = string.Empty;
				Uri = string.Empty;
			}
		}
		
		static DataServiceUtil()
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ClassLoaderUtil.MyAssemblyResolver);
		}

        public static string FrameworkAssembly
        {
            get
            {
                return Assembly.GetExecutingAssembly().FullName;
            }
        }

        private static string GetNextSqlStatement(StreamReader reader, bool skipSemiColon)
		{
			StringBuilder sqlBuff = new StringBuilder();
			string line = string.Empty;
			bool inComment = false;
			while ((line = reader.ReadLine()) != null)
			{
				line = line.Trim();
				if (string.IsNullOrEmpty(line))
				{
					continue;
				}
				if (line.StartsWith("#") || line.StartsWith("--"))
				{
					continue;
				}
				if (line.StartsWith("/*"))
				{
					inComment = true;
					continue;
				}
				if (inComment)
				{
					if (line.EndsWith("*/"))
					{
						inComment = false;
					}
					continue;
				}
				// strip off the line feed and carriage return.
				if (sqlBuff.Length != 0)
				{
					sqlBuff.Append(' ');
				}
				for (int i = 0; i < line.Length; i++)
				{
					if (skipSemiColon)
					{
						if (line[i] != '\r' && line[i] != '\n' && line[i] != ';')
						{
							if (line[i] == '-' && line[i + 1] == '-')
							{
								// there is an inline comment.
								break;
							}
							sqlBuff.Append(line[i]);
						}
					}
					else
					{
						if (line[i] != '\r' && line[i] != '\n')
						{
							if (line[i] == '-' && line[i + 1] == '-')
							{
								// there is an inline comment.
								break;
							}
							sqlBuff.Append(line[i]);
						}
					}
				}
				if (line.EndsWith(";"))
				{
					// this is the end of statement
					break;
				}
			}
			return sqlBuff.ToString();
		}
		
		public static string ClientDataModelAssemblyName
		{
			get
			{
				string clientAssemblyName = ClientsDataBaseName;
				System.Reflection.Assembly clientAssembly = ClassLoaderUtil.LoadAssembly(clientAssemblyName);
				if (clientAssembly == null)
				{
					string msg = string.Format("Unable to load assembly [" + clientAssemblyName + "].  Have you generated the assembly yet?");
					throw new FileNotFoundException(msg);
				}
				return clientAssemblyName;
			}
		}

		public static string ClientDataModelNamespace
		{
			get
			{
				string assName = string.Empty;
				LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
				if (ctx != null)
				{
					assName = string.Format("Brierley.Clients.{0}.DataModel", ctx.Organization.Trim());
				}
				return assName;
			}
		}

		public static string ClientsDataBaseName
		{
			get
			{
				string assName = string.Empty;
				LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
				if (ctx != null)
				{
					//assName = string.Format("Brierley.Clients.{0}.{1}.DataModel", ctx.Organization.Trim(), ctx.Environment.Trim());
					assName = string.Format("Brierley.Clients.{0}.DataModel", ctx.Organization.Trim());
				}
				return assName;
			}
		}

		public static string GetKey(string org, /*string app,*/ string env)
		{
			return string.Format("{0}:{1}", org, env);
		}

		public static Type GetClientDataObjectType(string attributeSetName)
		{
			string key = ClientDataModelNamespace + attributeSetName;

			if (!_clientAssemblyTypeMap.ContainsKey(attributeSetName))
			{
				lock (_clientAssemblyTypeMap)
				{
					if (!_clientAssemblyTypeMap.ContainsKey(attributeSetName))
					{
						IClientDataObject instance = GetNewClientDataObject(attributeSetName);
						_clientAssemblyTypeMap.Add(attributeSetName, instance.GetType());
					}
				}
			}
			return _clientAssemblyTypeMap[attributeSetName];
		}

		public static IClientDataObject GetNewClientDataObject(string attSetName)
		{
			string methodName = "GetClientDataObject";

			string objType = string.Empty;
			string assemblyName = string.Empty;
			IClientDataObject clntObject = null;
			try
			{
				objType = string.Format("{0}.{1}", ClientDataModelNamespace, attSetName);
				assemblyName = ClientsDataBaseName;
				clntObject = (ClientDataObject)ClassLoaderUtil.CreateInstance(assemblyName, objType);
				if (clntObject == null)
				{
					throw new LWException(string.Format("Unable to instantiate type {0} from {1}", objType, assemblyName));
				}
                lock (_petaPocoTypes)
                {
                    // Ensure type is loaded in PetaPoco
                    Type t = clntObject.GetType();
                    if (!_petaPocoTypes.Contains(t))
                    {
                        Assembly a = t.Assembly;
                        var auditType = a.ExportedTypes.Where(o => o.Name == t.Name + "_AL").FirstOrDefault();
                        var iMapperType = a.ExportedTypes.Where(o => o.Name == t.Name + "Mapper" && o.GetInterface("IMapper", true) != null).FirstOrDefault();

                        PetaPoco.IMapper iMapper = null;
                        if (iMapperType != null)
                            iMapper = Activator.CreateInstance(iMapperType) as PetaPoco.IMapper;

                        _petaPocoTypes.Add(t);
                        if (iMapper != null)
                        {
                            try
                            {
                                PetaPoco.Mappers.Revoke(t);
                            }
                            catch { }
                            PetaPoco.Mappers.Register(t, iMapper);
                        }
                        if (auditType != null)
                        {
                            _petaPocoTypes.Add(auditType);
                            if (iMapper != null)
                            {
                                try
                                {
                                    PetaPoco.Mappers.Revoke(auditType);
                                }
                                catch { }
                                PetaPoco.Mappers.Register(auditType, iMapper);
                            }
                        }
                    }
                }
            }
			catch (Exception ex)
			{
				string msg = string.Format("Unable to create object of type {0} from assembly {1}", objType, assemblyName);
				_logger.Error(_className, methodName, msg, ex);
				throw;
			}
			return clntObject;
		}

        public static void PurgeLoadedPetaPocoTypes()
        {
            lock (_petaPocoTypes)
                _petaPocoTypes.Clear();
        }

		public static void ExecuteRawSqlCommand(ServiceConfig config, string cmdline)
		{
			string methodName = "ExecuteRawSqlCommand";
			_logger.Debug(_className, methodName, "Running command: " + cmdline);
			using (var db = config.CreateDatabase())
			{
				IDbConnection conn = db.Connection;
				using (IDbCommand cmd = conn.CreateCommand())
				{
					cmd.CommandText = cmdline;
					cmd.CommandType = CommandType.Text;
					int nrows = cmd.ExecuteNonQuery();
					_logger.Debug(_className, methodName, "Number of rows affected: " + nrows);
				}
			}
		}

		public static void ExecuteRawSqlCommand(PetaPoco.Database database, string cmdline)
		{
			string methodName = "ExecuteRawSqlCommand";
			_logger.Debug(_className, methodName, "Running command: " + cmdline);

			IDbConnection conn = database.Connection;
			using (IDbCommand cmd = conn.CreateCommand())
			{
				cmd.CommandText = cmdline;
				cmd.CommandType = CommandType.Text;
				int nrows = cmd.ExecuteNonQuery();
				_logger.Debug(_className, methodName, "Number of rows affected: " + nrows);
			}
		}

        public static void SetupStandardAttributeSets(string orgName, string envName)
        {
            string virtualPath = "~/database.aspx";
            string templateFolder = System.Configuration.ConfigurationManager.AppSettings["LWTemplatePath"];
            if (string.IsNullOrEmpty(templateFolder))
                throw new LWConfigurationException(string.Format(ResourceUtils.GetLocalWebResource(virtualPath, "DefineLWTemplatePath")));

            if (!System.IO.Directory.Exists(templateFolder))
                throw new LWConfigurationException(string.Format(ResourceUtils.GetLocalWebResource(virtualPath, "TemplateFolderDoesntExist"), templateFolder));

            string result = string.Empty;
            string fpath = IOUtils.AppendSeparatorToFolderPath(templateFolder) + "LWRetailTestOrgDA.xsd";
            DataModelGenerator dmGen = new DataModelGenerator(fpath, true);
            IList<string> msgList = dmGen.GenerateDataModel();

            using (LoyaltyDataService LoyaltyService = LWDataServiceUtil.LoyaltyDataServiceInstance(orgName, envName))
            {
                List<AttributeSetMetaData> attSets = LoyaltyService.GetAllAttributeSets();
                foreach (AttributeSetMetaData attSet in attSets)
                {
                    try
                    {
                        LoyaltyService.CreateAttributeSetTable(attSet.ID);
                    }
                    catch (Exception ex)
                    {
                        msgList.Add(string.Format(ResourceUtils.GetLocalWebResource(virtualPath, "AttributeTableCreateError"), attSet.Name, ex.Message));
                    }
                }
            }

            if(msgList != null && msgList.Count > 0)
            {
                foreach (string m in msgList)
                {
                    result += m + "\n";
                }
                throw new LWConfigurationException(string.Format(ResourceUtils.GetLocalWebResource(virtualPath, "StdAttributeSetCreateError"), result));
            }

            ClientDataModelSolutionGenerator cgen = new ClientDataModelSolutionGenerator(true, true);
            GeneratedSolutionInfo info = cgen.Generate();

            AssemblyName name = AssemblyName.GetAssemblyName(info.AssemblyFileName);
            RemoteAssembly remoteAssembly = new RemoteAssembly();
            remoteAssembly.AssemblyName = name.FullName;
            remoteAssembly.AssemblyFileName = Path.GetFileName(info.AssemblyFileName);
            remoteAssembly.Assembly = IOUtils.BytesFromFile(info.AssemblyFileName);
            using (var svc = LWDataServiceUtil.DataServiceInstance(orgName, envName))
            {
                svc.SaveRemoteAssembly(remoteAssembly);
                svc.CreateCacheRefresh("SetupStandardAttributeSets");
            }

            LWDataServiceUtil.Reinitialize(orgName, envName);
        }

		private static void PostProcessTargetDatabase(SupportedDataSourceType dtType, Assembly fwkAssembly, PetaPoco.Database database)
		{
			string methodName = "PostProcessTargetDatabase";
			string dtTypeName = null;
			switch (dtType)
			{
				case SupportedDataSourceType.MsSQL2005:
					dtTypeName = "MsSql";
					break;
				case SupportedDataSourceType.Oracle10g:
					dtTypeName = "Oracle";
					break;
				default:
					throw new Exception(string.Format("The chosen database type {0} is not supported", dtType.ToString()));
			}

			StringBuilder errors = new StringBuilder();
			_logger.Trace(_className, methodName, "Post processing " + dtTypeName);
			try
			{
				string ns = "Brierley.FrameWork.Data.Scripts";

				List<string> resourceNames = new List<string>() { string.Format("{0}.{1}Init.sql", ns, dtTypeName) };
				if (dtType == SupportedDataSourceType.Oracle10g)
				{
					resourceNames.Add(string.Format("{0}.OracleInitLod.sql", ns));
				}

                foreach (string resourceName in resourceNames)
                {
                    bool addedCognosError = false;
                    _logger.Trace(_className, methodName, "Running resource script " + resourceName);
                    Stream stream = fwkAssembly.GetManifestResourceStream(resourceName);
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string batch = string.Empty;
                        string line = string.Empty;

                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (!string.IsNullOrEmpty(line) && !line.StartsWith("--"))
                            {
                                if (line == "go" || line == "/")
                                {
                                    try
                                    {
                                        ExecuteRawSqlCommand(database, batch.TrimEnd(';'));
                                    }
                                    catch (Exception e)
                                    {
                                        if (e.Message.Contains("ORA-01917") && e.Message.Contains("COGNOS_R"))
                                        {
                                            if (!addedCognosError)
                                            {
                                                _logger.Error(_className, methodName, string.Format(
@"An attempt was made to grant reporting view permission to user cognos_r, but the user does not exist. After creating the user, select permission should be granted to the following views: 
v_lw_rpt_top_visits
v_lw_rpt_enrollment_summary
v_lw_rpt_txn_summary
v_lw_rpt_mobile_event_summary
v_lw_rpt_points_summary
v_lw_rpt_rewards_summary
v_lw_rpt_top_points_earners
v_lw_rpt_top_pts_txn_summary
v_lw_rpt_top_reward_redeemers
v_lw_rpt_top_rwd_pts_summary
v_lw_rpt_top_rwd_txn_summary
lw_storedef
lw_rewardsdef
lw_csagent
lw_pointtype
SQL statement: {0}
Error: {1}", batch, e.Message));
                                                addedCognosError = true;
                                            }
                                        }
                                        else
                                        {
                                            errors.Append(string.Format("Error executing SQL: {0}.\n  Error Message: {1}\n\n", batch, e.Message));
                                        }
                                    }
                                    batch = string.Empty;
                                }
                                else
                                {
                                    batch += "\r\n" + line;
                                }
                            }
                        }
                        if (errors.Length > 0)
                        {
                            throw new LWException(errors.ToString());
                        }
                    }
                }
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error post processing " + dtTypeName, ex);
				throw;
			}
		}

		private static void PreProcessTargetDatabase(SupportedDataSourceType dtType, Assembly fwkAssembly, PetaPoco.Database database)
		{
			string methodName = "PreProcessTargetDatabase";
			string dtTypeName = null;
			switch (dtType)
			{
				case SupportedDataSourceType.MsSQL2005:
					dtTypeName = "MsSql";
					break;
				case SupportedDataSourceType.Oracle10g:
					dtTypeName = "Oracle";
					break;
				default:
					throw new Exception(string.Format("The chosen database type {0} is not supported", dtType.ToString()));
			}

			StringBuilder errors = new StringBuilder();
			_logger.Trace(_className, methodName, "Pre processing " + dtTypeName);
			try
			{
				string ns = "Brierley.FrameWork.Data.Scripts";
				List<string> resourceNames = new List<string>() { string.Format("{0}.{1}Nuke.sql", ns, dtTypeName) };
				if (dtType == SupportedDataSourceType.Oracle10g)
				{
					resourceNames.Add(string.Format("{0}.OracleNukeLod.sql", ns));
				}

                foreach (string resourceName in resourceNames)
                {
                    _logger.Trace(_className, methodName, "Running resource script " + resourceName);
                    Stream stream = fwkAssembly.GetManifestResourceStream(resourceName);
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string batch = string.Empty;
                        string line = string.Empty;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (!string.IsNullOrEmpty(line) && !line.StartsWith("--"))
                            {
                                if (line == "go" || line == "/")
                                {
                                    //execute batch
                                    try
                                    {
                                        ExecuteRawSqlCommand(database, batch.TrimEnd(';'));
                                    }
                                    catch (Exception e)
                                    {
                                        if (!e.Message.Contains("ORA-02443") && !e.Message.Contains("ORA-02289") && !e.Message.Contains("ORA-00942") && !e.Message.Contains("ORA-04043"))
                                        {
                                            errors.Append(string.Format("Error executing SQL: {0}.\n  Error Message: {1}\n\n", batch, e.Message));
                                        }
                                        else
                                        {
                                            _logger.Debug(_className, methodName, string.Format("Suppressed error message: {0}.\n", e.Message));
                                        }
                                    }
                                    batch = string.Empty;
                                }
                                else
                                {
                                    batch += "\r\n" + line;
                                }
                            }
                        }
                        if (errors.Length > 0)
                        {
                            throw new LWException(errors.ToString());
                        }
                    }
                }
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, "Error pre processing " + dtTypeName, ex);
				throw;
			}
		}

        private static void SetupDefaultValues(string orgName, string envName)
        {
            using (DataService ds = LWDataServiceUtil.DataServiceInstance(orgName, envName))
            {
                ds.CreateIDGenerator("MetaId");
                ds.CreateIDGenerator("LIBJob");
                ds.CreateIDGenerator("BatchID");
                ds.CreateIDGenerator("LWOrderNumber");
                ds.CreateIDGenerator("MMSOrderNumber");
                ds.CreateIDGenerator("PBSerialNumber");
                ds.CreateIDGenerator("LoyaltyId");
                ds.CreateIDGenerator("LWIdentifier");

                ds.CreateX509Cert(new X509Cert()
                {
                    CertName = "Apple WWDR CA Certificate",
                    CertType = X509CertType.AppleWWDRCACert,
                    CertPassword = null,
                    Value = "MIIEIjCCAwqgAwIBAgIIAd68xDltoBAwDQYJKoZIhvcNAQEFBQAwYjELMAkGA1UEBhMCVVMxEzARBgNVBAoTCkFwcGxlIEluYy4xJjAkBgNVBAsTHUFwcGxlIENlcnRpZmljYXRpb24gQXV0aG9yaXR5MRYwFAYDVQQDEw1BcHBsZSBSb290IENBMB4XDTEzMDIwNzIxNDg0N1oXDTIzMDIwNzIxNDg0N1owgZYxCzAJBgNVBAYTAlVTMRMwEQYDVQQKDApBcHBsZSBJbmMuMSwwKgYDVQQLDCNBcHBsZSBXb3JsZHdpZGUgRGV2ZWxvcGVyIFJlbGF0aW9uczFEMEIGA1UEAww7QXBwbGUgV29ybGR3aWRlIERldmVsb3BlciBSZWxhdGlvbnMgQ2VydGlmaWNhdGlvbiBBdXRob3JpdHkwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDKOFSmy1aqyCQ5SOmM7uxfuH8mkbw0U3rOfGOAYXdkXqUHI7Y5/lAtFVZYcC1+xG7BSoU+L/DehBqhV8mvexj/avoVEkkVCBmsqtsqMu2WY2hSFT2Miuy/axiV4AOsAX2XBWfODoWVN2rtCbauZ81RZJ/GXNG8V25nNYB2NqSHgW44j9grFU57Jdhav06DwY3Sk9UacbVgnJ0zTlX5ElgMhrgWDcHld0WNUEi6Ky3klIXh6MSdxmilsKP8Z35wugJZS3dCkTm59c3hTO/AO0iMpuUhXf1qarunFjVg0uat80YpyejDi+l5wGphZxWy8P3laLxiX27Pmd3vG2P+kmWrAgMBAAGjgaYwgaMwHQYDVR0OBBYEFIgnFwmpthhgi+zruvZHWcVSVKO3MA8GA1UdEwEB/wQFMAMBAf8wHwYDVR0jBBgwFoAUK9BpR5R2Cf70a40uQKb3R01/CF4wLgYDVR0fBCcwJTAjoCGgH4YdaHR0cDovL2NybC5hcHBsZS5jb20vcm9vdC5jcmwwDgYDVR0PAQH/BAQDAgGGMBAGCiqGSIb3Y2QGAgEEAgUAMA0GCSqGSIb3DQEBBQUAA4IBAQBPz+9Zviz1smwvj+4ThzLoBTWobot9yWkMudkXvHcs1Gfi/ZptOllc34MBvbKuKmFysa/Nw0Uwj6ODDc4dR7Txk4qjdJukw5hyhzs+r0ULklS5MruQGFNrCk4QttkdUGwhgAqJTleMa1s8Pab93vcNIx0LSiaHP7qRkkykGRIZbVf1eliHe2iK5IaMSuviSRSqpd1VAKmuu0swruGgsbwpgOYJd+W+NKIByn/c4grmO7i77LpilfMFY0GCzQ87HUyVpNur+cmV6U/kTecmmYHpvPm0KdIBembhLoz2IYrF+Hjhga6/05Cdqa3zr/04GpZnMBxRpVzscYqCtGwPDBUf"
                });

                foreach (AuditLogConfig config in AuditLogConfig.GetDefaultLogging().Values)
                    ds.CreateAuditLogConfig(config);
            }

            using (ContentService contentService = LWDataServiceUtil.ContentServiceInstance(orgName, envName))
            {
                contentService.LanguageDao.Create(new LanguageDef() { Culture = "en", Language = "English" });
                contentService.LanguageDao.Create(new LanguageDef() { Culture = "fr", Language = "French" });
                contentService.LanguageDao.Create(new LanguageDef() { Culture = "es", Language = "Spanish" });
                contentService.LanguageDao.Create(new LanguageDef() { Culture = "ru", Language = "Russian" });
                contentService.LanguageDao.Create(new LanguageDef() { Culture = "de", Language = "German" });
                contentService.LanguageDao.Create(new LanguageDef() { Culture = "ja", Language = "Japanese" });

                contentService.CreateChannelDef(new ChannelDef() { Name = "Web", Description = "A regular website", MimeType = "text/html" });
                contentService.CreateChannelDef(new ChannelDef() { Name = "Mobile", Description = "A mobile application or website", MimeType = "text/html" });
                contentService.CreateChannelDef(new ChannelDef() { Name = "SMS", Description = "Text messaging", MimeType = "text/plain" });
                contentService.CreateChannelDef(new ChannelDef() { Name = "Email", Description = "Emails", MimeType = "text/html" });
                contentService.CreateChannelDef(new ChannelDef() { Name = "Print", Description = "Print media", MimeType = "text/plain" });
                contentService.CreateChannelDef(new ChannelDef() { Name = "Push", Description = "Push Notifications", MimeType = "text/plain" });

                contentService.AddNotificationCategory(new NotificationCategory() { Name = "click_action", IsActive = true, SupportedVersion = 1 });
                contentService.AddNotificationCategory(new NotificationCategory() { Name = "category", IsActive = true, SupportedVersion = 1 });
            }

            using (LoyaltyDataService ls = LWDataServiceUtil.LoyaltyDataServiceInstance(orgName, envName))
            {
                ls.FulfillmentProviderDao.Create(new FulfillmentProvider() { Name = "MMS", ProviderAssemblyName = "Brierley.LoyaltyWare.LWIntegrationSvc.MMSFulfillmentProvider.FulfillmentProvider", ProviderTypeName = "MMSFulfillmentProvider.dll" });

                ls.CreateLWEvent(new LWEvent() { Name = "MemberAuthenticate", DisplayText = "MemberAuthenticate", Description = "MemberAuthenticate", UserDefined = false });
                ls.CreateLWEvent(new LWEvent() { Name = "MemberLoad", DisplayText = "MemberLoad", Description = "MemberLoad", UserDefined = false });
                ls.CreateLWEvent(new LWEvent() { Name = "MemberSave", DisplayText = "MemberSave", Description = "MemberSave", UserDefined = false });
                ls.CreateLWEvent(new LWEvent() { Name = "MemberSmsOptIn", DisplayText = "MemberSmsOptIn", Description = "MemberSmsOptIn", UserDefined = false });
                ls.CreateLWEvent(new LWEvent() { Name = "PromotionEnrollment", DisplayText = "Promotion Enrollment", Description = "Occurs whenever a member enrolls into a promotion", UserDefined = false });
                ls.CreateLWEvent(new LWEvent() { Name = "RewardChoiceSelection", DisplayText = "RewardChoiceSelection", Description = "Occurs whever a member changes their reward choice", UserDefined = false });
                
            }

            using (SurveyManager sm = LWDataServiceUtil.SurveyManagerInstance(orgName, envName))
            {
                SMLanguage englishLanguage = new SMLanguage() { Description = "English" };
                sm.CreateLanguage(englishLanguage);
                SMLanguage spanishLanguage = new SMLanguage() { Description = "Spanish" };
                sm.CreateLanguage(spanishLanguage);

                sm.CreateCultureMap(new SMCultureMap() { Culture = "en", Description = "English", LanguageID = englishLanguage.ID });
                sm.CreateCultureMap(new SMCultureMap() { Culture = "en-US", Description = "English (United States)", LanguageID = englishLanguage.ID });
                sm.CreateCultureMap(new SMCultureMap() { Culture = "en-GB", Description = "English (Great Brittain)", LanguageID = englishLanguage.ID });
                sm.CreateCultureMap(new SMCultureMap() { Culture = "es", Description = "Spanish", LanguageID = spanishLanguage.ID });
                sm.CreateCultureMap(new SMCultureMap() { Culture = "es-ES", Description = "Spanish (Spain)", LanguageID = spanishLanguage.ID });
            }

            using (CSService cs = LWDataServiceUtil.CSServiceInstance(orgName, envName))
            {
                CSFunction userAdminFunction = new CSFunction() { Name = "UserAdministration", Description = "Function to allow user administration" };
                cs.CreateFunction(userAdminFunction);
                cs.CreateFunction(new CSFunction() { Name = "ChangeAccountStatus", Description = "Function to allow the user to change the member account status" });
                cs.CreateFunction(new CSFunction() { Name = "UpdateAccount", Description = "Function to allow the user to update member profile" });
                cs.CreateFunction(new CSFunction() { Name = "AllowPointAward", Description = "Function to allow user to give points" });
                CSFunction searchMemberFunction = new CSFunction() { Name = "SearchMember", Description = "Function to allow user to search for a member" };
                cs.CreateFunction(searchMemberFunction);
                CSFunction memberProfileFunction = new CSFunction() { Name = "ViewMemberProfile", Description = "Function to allow user to view member profile" };
                cs.CreateFunction(memberProfileFunction);
                CSFunction transactionHistoryFunction = new CSFunction() { Name = "ViewTransactionHistory", Description = "Function to allow user to view transaction history" };
                cs.CreateFunction(transactionHistoryFunction);
                CSFunction rewardHistoryFunction = new CSFunction() { Name = "ViewRewardHistory", Description = "Function to allow user to view reward history" };
                cs.CreateFunction(rewardHistoryFunction);

                CSRole userAdminRole = new CSRole() { Name = "UserAdministration", Description = "Function to allow user administration" };
                cs.CreateRole(userAdminRole);

                cs.AddFunctionToRole(userAdminRole, userAdminFunction);
                cs.AddFunctionToRole(userAdminRole, searchMemberFunction);
                cs.AddFunctionToRole(userAdminRole, memberProfileFunction);
                cs.AddFunctionToRole(userAdminRole, transactionHistoryFunction);
                cs.AddFunctionToRole(userAdminRole, rewardHistoryFunction);

                cs.CreateCSAgent(new CSAgent() { Username = "csadmin", Password = "csadmin", FirstName = "csadmin", LastName = "csadmin", EmailAddress = "csadmin", RoleId = userAdminRole.Id });
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            string version = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            using (DataService ds = LWDataServiceUtil.DataServiceInstance(orgName, envName))
                ds.SchemaVersionDao.Create(new LWSchemaVersion() { AppliedBy = "LoyaltyNavigator", DateApplied = DateTime.Now, TargetType = "FrameworkObjects", VersionNumber = version });
        }

        public static void SetupStandardProgram(string orgName, string envName, bool hasAttributeSets)
        {
            using (LoyaltyDataService ls = LWDataServiceUtil.LoyaltyDataServiceInstance(orgName, envName))
            {
                ls.CreatePointEvent(new PointEvent() { Name = "PurchaseActivity", DefaultPoints = 10 });
                ls.CreatePointEvent(new PointEvent() { Name = "ReturnActivity", DefaultPoints = 10 });
                ls.CreatePointEvent(new PointEvent() { Name = "AppeasementActivity", DefaultPoints = 10 });
                ls.CreatePointEvent(new PointEvent() { Name = "EngagementActivity", DefaultPoints = 10 });
                ls.CreatePointEvent(new PointEvent() { Name = "TransferActivity", DefaultPoints = 10 });
                ls.CreatePointEvent(new PointEvent() { Name = "MergeActivity", DefaultPoints = 10 });

                ls.CreatePointType(new PointType() { Name = "BasePoints", ConsumptionPriority = 1, MoneyBacked = false });
                ls.CreatePointType(new PointType() { Name = "BonusPoints", ConsumptionPriority = 2, MoneyBacked = false });

                string pointTypeNames = "BasePoints";
                string pointEventNames = "PurchaseActivity;ReturnActivity;AppeasementActivity;EngagementActivity;TransferActivity;MergeActivity";
                string expirationDate = "GetLastDateOfYear(Date())";
                string activityStart = "GetBeginningOfDay(GetFirstDateOfYear(Date()))";
                string activityEnd = "GetEndOfDay(GetLastDateOfYear(Date()))";
                ls.CreateTierDef(new TierDef() { Name = "Standard", DisplayText = "Standard", EntryPoints = 0, ExitPoints = 99, PointTypeNames = pointTypeNames, PointEventNames = pointEventNames, ExpirationDateExpression = "90", ActivityPeriodStartExpression = activityStart, ActivityPeriodEndExpression = activityEnd, AddToEnrollmentDate = true });
                ls.CreateTierDef(new TierDef() { Name = "Silver", DisplayText = "Silver", EntryPoints = 100, ExitPoints = 999, PointTypeNames = pointTypeNames, PointEventNames = pointEventNames, ExpirationDateExpression = expirationDate, ActivityPeriodStartExpression = activityStart, ActivityPeriodEndExpression = activityEnd, AddToEnrollmentDate = false });
                ls.CreateTierDef(new TierDef() { Name = "Gold", DisplayText = "Gold", EntryPoints = 1000, ExitPoints = 5999, PointTypeNames = pointTypeNames, PointEventNames = pointEventNames, ExpirationDateExpression = expirationDate, ActivityPeriodStartExpression = activityStart, ActivityPeriodEndExpression = activityEnd, AddToEnrollmentDate = false });
                ls.CreateTierDef(new TierDef() { Name = "Platinum", DisplayText = "Platinum", EntryPoints = 6000, ExitPoints = 9999999, PointTypeNames = pointTypeNames, PointEventNames = pointEventNames, ExpirationDateExpression = expirationDate, ActivityPeriodStartExpression = activityStart, ActivityPeriodEndExpression = activityEnd, AddToEnrollmentDate = false });

                #region Ref Attribute Set Data
                if (hasAttributeSets)
                {
                    IClientDataObject cdoUSRef = ls.SaveClientDataObject(CreateRefCountry("USA", "us", 1, true, "USA (1)", "USA"), null, RuleExecutionMode.Real);
                    IClientDataObject cdoCanRef = ls.SaveClientDataObject(CreateRefCountry("Canada", "ca", 1, false, "Canada (1)", "CAN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Afghanistan", "AF", 93, false, "Afghanistan (93)", "AFG"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Albania", "AL", 355, false, "Albania (355)", "ALB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Algeria", "DZ", 213, false, "Algeria (213)", "DZA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("American Samoa", "AS", 1, false, "American Samoa (1)", "ASM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Andorra", "AD", 376, false, "Andorra (376)", "AND"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Angola", "AO", 244, false, "Angola (244)", "AGO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Anguilla", "AI", 1, false, "Anguilla (1)", "AIA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Antigua and Barbuda", "AG", 1, false, "Antigua and Barbuda (1)", "ATG"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Argentina", "AR", 54, false, "Argentina (54)", "ARG"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Armenia", "AM", 374, false, "Armenia (374)", "ARM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Aruba", "AW", 297, false, "Aruba (297)", "ABW"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Ascension", "AC", 247, false, "Ascension (247)", "ASC"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Australia", "AU", 61, false, "Australia (61)", "AUS"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Austria", "AT", 43, false, "Austria (43)", "AUT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Azerbaijan", "AZ", 994, false, "Azerbaijan (994)", "AZE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Bahamas, The", "BS", 1, false, "Bahamas, The (1)", "BHS"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Bahrain", "BH", 973, false, "Bahrain (973)", "BHR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Bangladesh", "BD", 880, false, "Bangladesh (880)", "BGD"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Barbados", "BB", 1, false, "Barbados (1)", "BRB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Belarus", "BY", 375, false, "Belarus (375)", "BLR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Belgium", "BE", 32, false, "Belgium (32)", "BEL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Belize", "BZ", 501, false, "Belize (501)", "BLZ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Benin", "BJ", 229, false, "Benin (229)", "BEN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Bermuda", "BM", 1, false, "Bermuda (1)", "BMU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Bhutan", "BT", 975, false, "Bhutan (975)", "BTN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Bolivia", "BO", 591, false, "Bolivia (591)", "BOL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Bosnia and Herzegovina", "BA", 387, false, "Bosnia and Herzegovina (387)", "BIH"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Botswana", "BW", 267, false, "Botswana (267)", "BWA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Brazil", "BR", 55, false, "Brazil (55)", "BRA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("British Indian Ocean Territory", "IO", 246, false, "British Indian Ocean Territory (246)", "IOT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("British Virgin Islands", "VG", 1, false, "British Virgin Islands (1)", "VGB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Brunei", "BN", 673, false, "Brunei (673)", "BRN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Bulgaria", "BG", 359, false, "Bulgaria (359)", "BGR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Burkina Faso", "BF", 226, false, "Burkina Faso (226)", "BFA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Burundi", "BI", 257, false, "Burundi (257)", "BDI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Cambodia", "KH", 855, false, "Cambodia (855)", "KHM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Cameroon", "CM", 237, false, "Cameroon (237)", "CMR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Cape Verde", "CV", 238, false, "Cape Verde (238)", "CPV"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Cayman Islands", "KY", 1, false, "Cayman Islands (1)", "CYM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Central African Republic", "CF", 236, false, "Central African Republic (236)", "CAF"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Chad", "TD", 235, false, "Chad (235)", "TCD"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Chile", "CL", 56, false, "Chile (56)", "CHL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("China, People\"s Republic of", "CN", 86, false, "China, People\"s Republic of (86)", "CHN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Christmas Island", "CX", 61, false, "Christmas Island (61)", "CXR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Cocos (Keeling) Islands", "CC", 61, false, "Cocos (Keeling) Islands (61)", "CCK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Colombia", "CO", 57, false, "Colombia (57)", "COL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Comoros", "KM", 269, false, "Comoros (269)", "COM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Congo", "CG", 242, false, "Congo (242)", "COG"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Congo, Democratic Republic of the (Zaire)", "CD", 243, false, "Congo, Democratic Republic of the (Zaire) (243)", "COD"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Cook Islands", "CK", 682, false, "Cook Islands (682)", "COK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Costa Rica", "CR", 506, false, "Costa Rica (506)", "CRI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Cote d\"Ivoire (Ivory Coast)", "CI", 225, false, "Cote d\"Ivoire (Ivory Coast) (225)", "CIV"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Croatia", "HR", 385, false, "Croatia (385)", "HRV"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Cuba", "CU", 53, false, "Cuba (53)", "CUB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Curacao", "CW", 599, false, "Curacao (599)", "CUW"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Cyprus", "CY", 357, false, "Cyprus (357)", "CYP"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Czech Republic", "CZ", 420, false, "Czech Republic (420)", "CZE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Denmark", "DK", 45, false, "Denmark (45)", "DNK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Djibouti", "DJ", 253, false, "Djibouti (253)", "DJI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Dominica", "DM", 1, false, "Dominica (1)", "DMA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Dominican Republic", "DO", 1, false, "Dominican Republic (1)", "DOM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Ecuador", "EC", 593, false, "Ecuador (593)", "ECU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Egypt", "EG", 20, false, "Egypt (20)", "EGY"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("El Salvador", "SV", 503, false, "El Salvador (503)", "SLV"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Equatorial Guinea", "GQ", 240, false, "Equatorial Guinea (240)", "GNQ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Eritrea", "ER", 291, false, "Eritrea (291)", "ERI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Estonia", "EE", 372, false, "Estonia (372)", "EST"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Ethiopia", "ET", 251, false, "Ethiopia (251)", "ETH"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Falkland Islands (Islas Malvinas)", "FK", 500, false, "Falkland Islands (Islas Malvinas) (500)", "FLK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Faroe Islands", "FO", 298, false, "Faroe Islands (298)", "FRO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Fiji", "FJ", 679, false, "Fiji (679)", "FJI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Finland", "FI", 358, false, "Finland (358)", "FIN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("France", "FR", 33, false, "France (33)", "FRA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("French Guiana", "GF", 594, false, "French Guiana (594)", "GUF"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("French Polynesia", "PF", 689, false, "French Polynesia (689)", "PYF"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Gabon", "GA", 241, false, "Gabon (241)", "GAB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Gambia, The", "GM", 220, false, "Gambia, The (220)", "GMB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Georgia", "GE", 995, false, "Georgia (995)", "GEO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Germany", "DE", 49, false, "Germany (49)", "DEU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Ghana", "GH", 233, false, "Ghana (233)", "GHA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Gibraltar", "GI", 350, false, "Gibraltar (350)", "GIB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Greece", "GR", 30, false, "Greece (30)", "GRC"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Greenland", "GL", 299, false, "Greenland (299)", "GRL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Grenada", "GD", 1, false, "Grenada (1)", "GRD"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Guadeloupe", "GP", 590, false, "Guadeloupe (590)", "GLP"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Guam", "GU", 1, false, "Guam (1)", "GUM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Guatemala", "GT", 502, false, "Guatemala (502)", "GTM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Guernsey", "GG", 44, false, "Guernsey (44)", "GGY"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Guinea", "GN", 224, false, "Guinea (224)", "GIN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Guinea-Bissau", "GW", 245, false, "Guinea-Bissau (245)", "GNB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Guyana", "GY", 592, false, "Guyana (592)", "GUY"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Haiti", "HT", 509, false, "Haiti (509)", "HTI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Honduras", "HN", 504, false, "Honduras (504)", "HND"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Hong Kong", "HK", 852, false, "Hong Kong (852)", "HKG"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Hungary", "HU", 36, false, "Hungary (36)", "HUN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Iceland", "IS", 354, false, "Iceland (354)", "ISL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("India", "IN", 91, false, "India (91)", "IND"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Indonesia", "ID", 62, false, "Indonesia (62)", "IDN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Iran", "IR", 98, false, "Iran (98)", "IRN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Iraq", "IQ", 964, false, "Iraq (964)", "IRQ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Ireland", "IE", 353, false, "Ireland (353)", "IRL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Isle of Man", "IM", 44, false, "Isle of Man (44)", "IMN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Israel", "IL", 972, false, "Israel (972)", "ISR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Italy", "IT", 39, false, "Italy (39)", "ITA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Jamaica", "JM", 1, false, "Jamaica (1)", "JAM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Japan", "JP", 81, false, "Japan (81)", "JPN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Jersey", "JE", 44, false, "Jersey (44)", "JEY"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Jordan", "JO", 962, false, "Jordan (962)", "JOR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Kazakhstan", "KZ", 7, false, "Kazakhstan (7)", "KAZ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Kenya", "KE", 254, false, "Kenya (254)", "KEN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Kiribati", "KI", 686, false, "Kiribati (686)", "KIR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Korea, South", "KR", 82, false, "Korea, South (82)", "KOR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Kosovo", "XK", 383, false, "Kosovo (383)", "XKX"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Kuwait", "KW", 965, false, "Kuwait (965)", "KWT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Kyrgyzstan", "KG", 996, false, "Kyrgyzstan (996)", "KGZ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Laos", "LA", 856, false, "Laos (856)", "LAO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Latvia", "LV", 371, false, "Latvia (371)", "LVA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Lebanon", "LB", 961, false, "Lebanon (961)", "LBN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Lesotho", "LS", 266, false, "Lesotho (266)", "LSO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Liberia", "LR", 231, false, "Liberia (231)", "LBR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Libya", "LY", 218, false, "Libya (218)", "LBY"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Liechtenstein", "LI", 423, false, "Liechtenstein (423)", "LIE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Lithuania", "LT", 370, false, "Lithuania (370)", "LTU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Luxembourg", "LU", 352, false, "Luxembourg (352)", "LUX"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Macao (China)", "MO", 853, false, "Macao (China) (853)", "MAC"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Macedonia", "MK", 389, false, "Macedonia (389)", "MKD"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Madagascar", "MG", 261, false, "Madagascar (261)", "MDG"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Malawi", "MW", 265, false, "Malawi (265)", "MWI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Malaysia", "MY", 60, false, "Malaysia (60)", "MYS"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Maldives", "MV", 960, false, "Maldives (960)", "MDV"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Mali", "ML", 223, false, "Mali (223)", "MLI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Malta", "MT", 356, false, "Malta (356)", "MLT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Marshall Islands", "MH", 692, false, "Marshall Islands (692)", "MHL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Martinique", "MQ", 596, false, "Martinique (596)", "MTQ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Mauritania", "MR", 222, false, "Mauritania (222)", "MRT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Mauritius", "MU", 230, false, "Mauritius (230)", "MUS"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Mayotte", "YT", 262, false, "Mayotte (262)", "MYT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Mexico", "MX", 52, false, "Mexico (52)", "MEX"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Micronesia", "FM", 691, false, "Micronesia (691)", "FSM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Moldova", "MD", 373, false, "Moldova (373)", "MDA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Monaco", "MC", 377, false, "Monaco (377)", "MCO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Mongolia", "MN", 976, false, "Mongolia (976)", "MNG"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Montenegro", "ME", 382, false, "Montenegro (382)", "MNE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Montserrat", "MS", 1, false, "Montserrat (1)", "MSR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Morocco", "MA", 212, false, "Morocco (212)", "MAR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Mozambique", "MZ", 258, false, "Mozambique (258)", "MOZ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Myanmar (Burma)", "MM", 95, false, "Myanmar (Burma) (95)", "MMR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Namibia", "NA", 264, false, "Namibia (264)", "NAM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Nauru", "NR", 674, false, "Nauru (674)", "NRU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Nepal", "NP", 977, false, "Nepal (977)", "NPL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Netherlands", "NL", 31, false, "Netherlands (31)", "NLD"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Netherlands Antilles", "AN", 599, false, "Netherlands Antilles (599)", "ANT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("New Caledonia", "NC", 687, false, "New Caledonia (687)", "NCL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("New Zealand", "NZ", 64, false, "New Zealand (64)", "NZL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Nicaragua", "NI", 505, false, "Nicaragua (505)", "NIC"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Niger", "NE", 227, false, "Niger (227)", "NER"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Nigeria", "NG", 234, false, "Nigeria (234)", "NGA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Niue", "NU", 683, false, "Niue (683)", "NIU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Northern Mariana Islands", "MP", 1, false, "Northern Mariana Islands (1)", "MNP"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Norway", "NO", 47, false, "Norway (47)", "NOR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Oman", "OM", 968, false, "Oman (968)", "OMN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Pakistan", "PK", 92, false, "Pakistan (92)", "PAK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Palau", "PW", 680, false, "Palau (680)", "PLW"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Palestine", "PS", 970, false, "Palestine (970)", "PSE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Panama", "PA", 507, false, "Panama (507)", "PAN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Papua New Guinea", "PG", 675, false, "Papua New Guinea (675)", "PNG"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Paraguay", "PY", 595, false, "Paraguay (595)", "PRY"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Peru", "PE", 51, false, "Peru (51)", "PER"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Philippines", "PH", 63, false, "Philippines (63)", "PHL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Poland", "PL", 48, false, "Poland (48)", "POL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Portugal", "PT", 351, false, "Portugal (351)", "PRT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Puerto Rico", "PR", 1, false, "Puerto Rico (1)", "PRI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Qatar", "QA", 974, false, "Qatar (974)", "QAT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Reunion", "RE", 262, false, "Reunion (262)", "REU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Romania", "RO", 40, false, "Romania (40)", "ROU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Russia", "RU", 7, false, "Russia (7)", "RUS"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Rwanda", "RW", 250, false, "Rwanda (250)", "RWA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Saint Barthelemy", "BL", 590, false, "Saint Barthelemy (590)", "BLM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Saint Helena", "SH", 290, false, "Saint Helena (290)", "SHN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Saint Kitts and Nevis", "KN", 1, false, "Saint Kitts and Nevis (1)", "KNA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Saint Lucia", "LC", 1, false, "Saint Lucia (1)", "LCA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Saint Martin", "MF", 590, false, "Saint Martin (590)", "MAF"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Saint Pierre and Miquelon", "PM", 508, false, "Saint Pierre and Miquelon (508)", "SPM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Saint Vincent and the Grenadines", "VC", 1, false, "Saint Vincent and the Grenadines (1)", "VCT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Samoa", "WS", 685, false, "Samoa (685)", "WSM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("San Marino", "SM", 378, false, "San Marino (378)", "SMR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Sao Tome and Principe", "ST", 239, false, "Sao Tome and Principe (239)", "STP"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Saudi Arabia", "SA", 966, false, "Saudi Arabia (966)", "SAU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Senegal", "SN", 221, false, "Senegal (221)", "SEN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Serbia", "RS", 381, false, "Serbia (381)", "SRB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Seychelles", "SC", 248, false, "Seychelles (248)", "SYC"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Sierra Leone", "SL", 232, false, "Sierra Leone (232)", "SLE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Singapore", "SG", 65, false, "Singapore (65)", "SGP"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Sint Maarten", "SX", 1, false, "Sint Maarten (1)", "SXM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Slovakia", "SK", 421, false, "Slovakia (421)", "SVK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Slovenia", "SI", 386, false, "Slovenia (386)", "SVN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Solomon Islands", "SB", 677, false, "Solomon Islands (677)", "SLB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Somalia", "SO", 252, false, "Somalia (252)", "SOM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("South Africa", "ZA", 27, false, "South Africa (27)", "ZAF"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("South Sudan", "SS", 211, false, "South Sudan (211)", "SSD"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Spain", "ES", 34, false, "Spain (34)", "ESP"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Sri Lanka", "LK", 94, false, "Sri Lanka (94)", "LKA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Sudan", "SD", 249, false, "Sudan (249)", "SDN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Suriname", "SR", 597, false, "Suriname (597)", "SUR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Swaziland", "SZ", 268, false, "Swaziland (268)", "SWZ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Sweden", "SE", 46, false, "Sweden (46)", "SWE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Switzerland", "CH", 41, false, "Switzerland (41)", "CHE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Syria", "SY", 963, false, "Syria (963)", "SYR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Taiwan (China)", "TW", 886, false, "Taiwan (China) (886)", "TWN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Tajikistan", "TJ", 992, false, "Tajikistan (992)", "TJK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Tanzania", "TZ", 255, false, "Tanzania (255)", "TZA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Thailand", "TH", 66, false, "Thailand (66)", "THA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Timor-Leste (East Timor)", "TL", 670, false, "Timor-Leste (East Timor) (670)", "TLS"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Togo", "TG", 228, false, "Togo (228)", "TGO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Tokelau", "TK", 690, false, "Tokelau (690)", "TKL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Tonga", "TO", 676, false, "Tonga (676)", "TON"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Trinidad and Tobago", "TT", 1, false, "Trinidad and Tobago (1)", "TTO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Tunisia", "TN", 216, false, "Tunisia (216)", "TUN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Turkey", "TR", 90, false, "Turkey (90)", "TUR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Turkmenistan", "TM", 993, false, "Turkmenistan (993)", "TKM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Turks and Caicos Islands", "TC", 1, false, "Turks and Caicos Islands (1)", "TCA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Tuvalu", "TV", 688, false, "Tuvalu (688)", "TUV"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("U.S. Virgin Islands", "VI", 1, false, "U.S. Virgin Islands (1)", "VIR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Uganda", "UG", 256, false, "Uganda (256)", "UGA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Ukraine", "UA", 380, false, "Ukraine (380)", "UKR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("United Arab Emirates", "AE", 971, false, "United Arab Emirates (971)", "ARE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("United Kingdom", "GB", 44, false, "United Kingdom (44)", "GBR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Uruguay", "UY", 598, false, "Uruguay (598)", "URY"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Uzbekistan", "UZ", 998, false, "Uzbekistan (998)", "UZB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Vanuatu", "VU", 678, false, "Vanuatu (678)", "VUT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Vatican City", "VA", 379, false, "Vatican City (379)", "VAT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Venezuela", "VE", 58, false, "Venezuela (58)", "VEN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Vietnam", "VN", 84, false, "Vietnam (84)", "VNM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Wallis and Futuna", "WF", 681, false, "Wallis and Futuna (681)", "WLF"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Western Sahara", "EH", 212, false, "Western Sahara (212)", "ESH"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Yemen", "YE", 967, false, "Yemen (967)", "YEM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Zambia", "ZM", 260, false, "Zambia (260)", "ZMB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefCountry("Zimbabwe", "ZW", 263, false, "Zimbabwe (263)", "ZWE"), null, RuleExecutionMode.Real);

                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Alabama", "AL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Alaska", "AK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Arizona", "AZ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Arkansas", "AR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "California", "CA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Colorado", "CO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Connecticut", "CT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Delaware", "DE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Florida", "FL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Georgia", "GA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Hawaii", "HI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Idaho", "ID"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Illinois", "IL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Indiana", "IN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Iowa", "IA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Kansas", "KS"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Kentucky", "KY"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Lousiana", "LA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Maine", "ME"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Maryland", "MD"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Massachusetts", "MA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Michigan", "MI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Minnesota", "MN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Mississippi", "MS"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Missouri", "MO"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Montana", "MT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Nebraska", "NE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Nevada", "NV"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "New Hampshire", "NH"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "New Jersey", "NJ"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "New Mexico", "NM"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "New York", "NY"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "North Carolina", "NC"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "North Dakota", "ND"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Ohio", "OH"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Oklahoma", "OK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Oregon", "OR"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Pennsylvania", "PA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Rhode Island", "RI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "South Carolina", "SC"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "South Dakota", "SD"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Tennessee", "TN"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Texas", "TX"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Utah", "UT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Vermont", "VT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Virginia", "VA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Washington", "WA"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "West Virginia", "WV"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Wisconsin", "WI"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoUSRef.RowKey, "Wyoming", "WY"), null, RuleExecutionMode.Real);

                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Alberta", "AB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "British Columbia", "BC"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Manitoba", "MB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "New Brunswick", "NB"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Newfoundland and Labrador", "NL"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Northwest Territories", "NT"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Nova Scotia", "NS"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Nunavut", "NU"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Ontario", "ON"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Prince Edward Island", "PE"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Quebec", "QC"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Saskatchewan", "SK"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateRefState(cdoCanRef.RowKey, "Yukon", "YT"), null, RuleExecutionMode.Real);

                    ls.SaveClientDataObject(CreateCardLabel(0, "Loyalty Card"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateCardLabel(1, "MasterCard"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateCardLabel(2, "Visa"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateCardLabel(3, "Discover"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateCardLabel(4, "American Express"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateCardLabel(5, "Diner"), null, RuleExecutionMode.Real);
                    ls.SaveClientDataObject(CreateCardLabel(6, "Private Label"), null, RuleExecutionMode.Real);
                }
                #endregion

                ls.CreateLWEvent(new LWEvent() { Name = "MemberOthers", DisplayText = "MemberOthers", Description = "MemberOthers", UserDefined = true });
                ls.CreateRuleTrigger(new RuleTrigger()
                {
                    AttributeSetCode = 0,
                    Sequence = 1,
                    InvocationType = "Manual",
                    ConditionalExpression = "1==1",
                    IsConfigured = true,
                    RuleName = "DefaultRewardAppeasementRule",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    Targeted = false,
                    LogExecution = false,
                    AlwaysRun = true,
                    OwningObject = "MemberOthers",
                    ContinueOnError = false,
                    CanQueue = false,
                    Rule = new Brierley.FrameWork.Rules.RewardCatalogIssueReward()
                    {
                        FulfillmentOption = RewardFulfillmentOption.Electronic,
                        PointsConsumption = PointsConsumptionOnIssueReward.NoAction,
                        ExpiryDateExpression = "AddYear(Date(), 5)",
                        AssignLWCertificate = true
                    }
                });
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            string version = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            using (DataService ds = LWDataServiceUtil.DataServiceInstance(orgName, envName))
                ds.SchemaVersionDao.Create(new LWSchemaVersion() { AppliedBy = "LoyaltyNavigator", DateApplied = DateTime.Now, TargetType = "StandardProgram", VersionNumber = version });
        }

        private static IClientDataObject CreateRefCountry(string countryName, string tld, int phoneCode, bool smsEnabled, string displayName, string countryCode)
        {
            IClientDataObject cdobject = GetNewClientDataObject("RefCountry");
            cdobject.SetAttributeValue("CountryName", countryName);
            cdobject.SetAttributeValue("TLD", tld);
            cdobject.SetAttributeValue("PhoneCode", phoneCode);
            cdobject.SetAttributeValue("SmsEnabled", smsEnabled);
            cdobject.SetAttributeValue("DisplayName", displayName);
            cdobject.SetAttributeValue("CountryCode", countryCode);
            return cdobject;
        }

        private static IClientDataObject CreateRefState(long parentId, string stateOrProvinceName, string abbr)
        {
            IClientDataObject cdobject = GetNewClientDataObject("RefStateOrProvince");
            cdobject.ParentRowKey = parentId;
            cdobject.SetAttributeValue("StateOrProvinceName", stateOrProvinceName);
            cdobject.SetAttributeValue("Abbreviation", abbr);
            return cdobject;
        }

        private static IClientDataObject CreateCardLabel(int cardType, string cardLabel)
        {
            IClientDataObject cdobject = GetNewClientDataObject("RefCardLabel");
            cdobject.SetAttributeValue("CardType", cardType);
            cdobject.SetAttributeValue("CardLabel", cardLabel);
            return cdobject;
        }

        public static void InitializeTargetDatabase(LWConfiguration lwConfig)
        {
            string methodName = "InitializeTargetDatabase";
            _logger.Trace(_className, methodName, string.Format("Initializing database for {0}/{1}", lwConfig.Organization, lwConfig.Environment));

            SupportedDataSourceType dtType = lwConfig.DBConfig.DBType;
            Assembly fwkAssembly = Assembly.GetExecutingAssembly();

            using (PetaPoco.Database database = new PetaPoco.Database(
                lwConfig.DBConfig.ConnectionConfigs[DBConfig.FRAMEWORK_DB_CONNECTION_NAME].GetConnectionString(),
                LWDataServiceUtil.ResolveProvider(lwConfig.DBConfig.DBType)))
            {
                database.OpenSharedConnection();
                // execute a constraint nuke script if one exists.
                PreProcessTargetDatabase(dtType, fwkAssembly, database);

                LWDataServiceUtil.GetServiceConfiguration().AuditableObjects = null;

                Generator schemaGenerator = new Generator(fwkAssembly, dtType);

                //generate both a backout and an init, combining them:
                List<string> sqlScript = schemaGenerator.GenerateModelScripts(GetKnownModelNamespaces(), Generator.ScriptType.Backout);
                sqlScript.AddRange(schemaGenerator.GenerateModelScripts(GetKnownModelNamespaces(), Generator.ScriptType.Init));

                StringBuilder errors = new StringBuilder();
                foreach (string script in sqlScript)
                {
                    try
                    {
                        ExecuteRawSqlCommand(database, script.TrimEnd(';'));
                    }
                    catch (Exception e)
                    {
                        // TODO: We need a better way to ignore expected errors
                        if ((script.Contains("drop sequence") && e.Message.Contains("ORA-02289")) ||
                           e.Message.Contains("ORA-00942") ||
                           e.Message.Contains("ORA-02443") ||
                           e.Message.Contains("it does not exist or you do not have permission") ||
                           (e.Message.Contains("is not a constraint") && e.Message.Contains("Could not drop constraint")))
                        {
                            _logger.Debug(_className, methodName, string.Format("Suppressed error message: {0}.\n", e.Message));
                        }
                        else
                        {
                            errors.Append(string.Format("Error executing SQL: {0}.\n  Error Message: {1}\n\n", script, e.Message));
                        }
                    }
                }

                if (errors.Length > 0)
                {
                    throw new LWException(errors.ToString());
                }

                // now execute an initialization script if one exists.
                PostProcessTargetDatabase(dtType, fwkAssembly, database);
            }
            // Add default items
            SetupDefaultValues(lwConfig.Organization, lwConfig.Environment);
        }

		public static string RunDBScript(string scriptName, bool skipSemiColon = true)
		{
			StringBuilder strBuf = new StringBuilder();
			string method = "RunDBScript";
			string fileName = scriptName;
			string sqlStatement = string.Empty;
			_logger.Trace(_className, method, "Executing statements from script " + scriptName);
			if (!File.Exists(scriptName))
			{
				return string.Format("{0} does not exist.", scriptName);
			}
			using (StreamReader reader = new StreamReader(fileName))
			{
				var config = LWDataServiceUtil.GetServiceConfiguration();
				while (true)
				{
					sqlStatement = GetNextSqlStatement(reader, skipSemiColon);
					if (!string.IsNullOrEmpty(sqlStatement))
					{
						if (sqlStatement.EndsWith(";"))
						{
							// strip off the ending ;
							sqlStatement = sqlStatement.Substring(0, sqlStatement.Length - 1);
						}
						_logger.Debug(_className, method, "Executing SQL: " + sqlStatement);
						try
						{
							ExecuteRawSqlCommand(config, sqlStatement);
						}
						catch (Exception)
						{
							string err = "Unable to execute statement: " + sqlStatement;
							if (strBuf.Length != 0)
							{
								strBuf.Append("\n");
							}
							strBuf.Append(err);
							_logger.Error(_className, method, err);
						}
					}
					else
					{
						break;
					}
				}
			}
			return strBuf.ToString();
		}

		public static List<string> GetKnownModelNamespaces()
		{
			//we could eventually reflect over the framework assembly and find all namespaces that have PetaPoco models. Today, though, it is only these two:
			return new List<string>() { "Brierley.FrameWork.Data.DomainModel", "Brierley.FrameWork.CampaignManagement.DomainModel" };
		}
    }
}
