//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;

using Brierley.FrameWork.Common.Exceptions;

namespace Brierley.FrameWork.Common.Config
{
    public class LWConfiguration
    {
        private const string CONFIGNAME_LW_CONFIG = "LWConfig";
        public const string CONFIGNAME_KEYSTORE_PASS = "KeystorePass";
        public const string CONFIGNAME_KEYSIZE = "KeySize";
        private const string DEFAULT_LW_CONFIG = "Config";
        public const string FWCONFIG_FILE_NAME = "Framework.cfg";
        public const string KEYSTORE_FILE_NAME = "Keystore.dat";
        public const string SYMMETRIC_KEY_FILE_NAME = "SymmetricKeystore.dat";
        public const string DBCONFIG_FILE_NAME = "DBConfig.dat";
        private string _orgName;
        private string _envName;
        private FWConfig _FWConfig = null;
        private LWKeystore _LWKeystore = null;
        private DBConfig _DBConfig = null;

        /// <summary>
        /// Constructs a LWConfiguration based on config files on the disk
        /// </summary>
        /// <param name="orgName">organization name</param>
        /// <param name="envName">environment name</param>
        public LWConfiguration(string orgName, string envName)
        {
            _orgName = orgName;
            _envName = envName;
            LoadConfiguration();
        }

        /// <summary>
        /// Constructs a LWConfiguration dynamically
        /// </summary>
        /// <param name="orgName">organization name</param>
        /// <param name="envName">environment name</param>
        /// <param name="fwconfig">framework configuration</param>
        /// <param name="lwkeystore">keystore</param>
        /// <param name="dbconfig">database configuration</param>
        public LWConfiguration(string orgName, string envName, FWConfig fwconfig, LWKeystore lwkeystore, DBConfig dbconfig)
        {
            _orgName = orgName;
            _envName = envName;
            _FWConfig = fwconfig;
            _LWKeystore = lwkeystore;
            _DBConfig = dbconfig;
        }

        /// <summary>
        /// Gets a unique name for the given entity.  This is used by 
        /// LWConfigurationUtil as a key for its collection of LWConfigurations.
        /// </summary>
        /// <param name="orgName">organization name</param>
        /// <param name="envName">environment name</param>
        /// <returns>the unique key for the given entity</returns>
        public static string GetEntityKey(string orgName, string envName)
        {
            orgName = StringUtils.NoSpaces(orgName);
            envName = StringUtils.NoSpaces(envName);
			string result = string.Format("{0}_{1}", orgName, envName);
            return result;
        }

        /// <summary>
        /// Does the framework configuration contain the specified config name?
        /// </summary>
        /// <param name="configName">config name</param>
        /// <returns>true/false</returns>
        public bool ContainsConfigName(string configName)
        {
            string configValue = _FWConfig.GetFWConfigProperty(configName);
            return (!string.IsNullOrEmpty(configValue) ? true : false);
        }

        /// <summary>
        /// Get the value associated with the specified framework config name.
        /// </summary>
        /// <param name="configName">config name</param>
        /// <returns>config value</returns>
        public string GetConfigValue(string configName)
        {
            string configValue = _FWConfig.GetFWConfigProperty(configName);
            return configValue;            
        }

        /// <summary>
        /// Set the specified value for the specified framework config name.
        /// </summary>
        /// <param name="configName">config name</param>
        /// <param name="configValue">config value</param>
        public void SetConfigValue(string configName, string configValue)
        {
            if (string.IsNullOrEmpty(configName))
                throw new ArgumentNullException(configName);

            _FWConfig.SetFWConfigProperty(configName, configValue);
        }

        /// <summary>
        /// Deletes the property for the specified framework config name.
        /// </summary>
        /// <param name="configName">config name</param>
        public void DeleteConfigValue(string configName)
        {
            if (string.IsNullOrEmpty(configName))
                throw new ArgumentNullException(configName);

            _FWConfig.DeleteFWConfigProperty(configName);
        }

        /// <summary>
        /// Get the framework configuration as XML.
        /// </summary>
        /// <returns>XML</returns>
        public string ToXML()
        {
            string xml = _FWConfig.Serialize();
            return xml;
        }

        /// <summary>
        /// Is there a FWConfig object?
        /// </summary>
        /// <returns>true/false</returns>
        public bool HasFWConfig()
        {
            return (_FWConfig != null);
        }

        /// <summary>
        /// Is there a LWKeystore object?
        /// </summary>
        /// <returns>true/false</returns>
        public bool HasLWKeystore()
        {
            return (_LWKeystore != null);
        }

        /// <summary>
        /// Is there a DBConfig object?
        /// </summary>
        /// <returns>true/false</returns>
        public bool HasDBConfig()
        {
            if (_DBConfig != null)
            {
                return _DBConfig.HasCredentials();
            }
            else
            {
                return false;
            }            
        }

        /// <summary>
        /// Gets a unique name for the entity represented by this LWConfiguration.  
        /// This is used by LWConfigurationUtil as a key for its collection of LWConfigurations.
        /// </summary>
        public string EntityKey
        {
            get { return GetEntityKey(_orgName, _envName); }
        }

        /// <summary>
        /// Get the organization name.
        /// </summary>
        public string Organization
        {
            get { return _orgName; }
        }

        /// <summary>
        /// Get the environment name.
        /// </summary>
        public string Environment
        {
            get { return _envName; }
        }

        /// <summary>
        /// Get the FWConfig object.
        /// </summary>
        public FWConfig FWConfig
        {
            get { return _FWConfig; }
        }

        /// <summary>
        /// Get the LWKeystore object.
        /// </summary>
        public LWKeystore LWKeystore
        {
            get { return _LWKeystore; }
        }

        /// <summary>
        /// Get the DBConfig object.
        /// </summary>
        public DBConfig DBConfig
        {
            get { return _DBConfig; }
        }

        private void LoadConfiguration()
        {
            string normalizedConfigDir = GetNormalizedConfigDir();

            // Load framework configuration
            string configPath = FindFWConfigFile(normalizedConfigDir);
            string fwconfigXml = File.ReadAllText(configPath);
			_FWConfig = new FWConfig(fwconfigXml, _orgName, _envName);

            // Load Keystore
            string keystorePath = FindKeystoreFile(normalizedConfigDir);
            string symmetricKeystorePath = FindSymmetricKeystoreFile(normalizedConfigDir);

            string keystore = StringUtils.FriendlyString(File.ReadAllText(keystorePath), string.Empty).Trim();
            string keystorePass = StringUtils.FriendlyString(_FWConfig.GetFWConfigProperty(CONFIGNAME_KEYSTORE_PASS), string.Empty);

            long keysize = StringUtils.FriendlyInt64(_FWConfig.GetFWConfigProperty(CONFIGNAME_KEYSIZE), 512);

            string[] symmetricKeystoreContents = File.ReadAllLines(symmetricKeystorePath);
            string initializationVector = StringUtils.FriendlyString(symmetricKeystoreContents[0], string.Empty).Trim();
            string symmetricKey = StringUtils.FriendlyString(symmetricKeystoreContents[1], string.Empty).Trim();

            _LWKeystore = new LWKeystore(keystorePass, keystore, initializationVector, symmetricKey, keysize);

            // Load DBConfig
            string dbconfigPath = FindDBConfigFile(normalizedConfigDir);
            string serializedDBConfig = StringUtils.FriendlyString(File.ReadAllText(dbconfigPath), string.Empty).Trim();
            _DBConfig = new DBConfig(_LWKeystore, serializedDBConfig);
        }

        public string GetNormalizedConfigDir()
        {
            // Determine normalized LW_CONFIG based on app settings or default value
            string configRoot = ConfigurationManager.AppSettings[CONFIGNAME_LW_CONFIG];
            if (string.IsNullOrEmpty(configRoot))
            {
                configRoot = DEFAULT_LW_CONFIG;
            }
            configRoot = configRoot.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (configRoot[configRoot.Length - 1] != Path.DirectorySeparatorChar)
            {
                configRoot += Path.DirectorySeparatorChar;
            }

            // Append the entity key
            configRoot += this.EntityKey + Path.DirectorySeparatorChar;

            // Ensure config directory exists
            if (!Directory.Exists(configRoot))
            {
                string tmpDir = AppDomain.CurrentDomain.BaseDirectory;
                tmpDir = tmpDir.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                if (tmpDir[tmpDir.Length - 1] != Path.DirectorySeparatorChar)
                {
                    tmpDir += Path.DirectorySeparatorChar;
                }
                string appDomainConfigRoot = tmpDir + configRoot;
                if (!Directory.Exists(appDomainConfigRoot))
                {
                    throw new LWConfigurationException(string.Format("Configuration directory not found at LWConfig='{0}' nor ~/LWConfig='{1}'.", configRoot, appDomainConfigRoot));
                }
                configRoot = appDomainConfigRoot;
            }
            return configRoot;
        }

        private static string FindFWConfigFile(string normalizedConfigRoot)
        {
            // Determine configuration file path
            string configPath = normalizedConfigRoot + FWCONFIG_FILE_NAME;
            if (!File.Exists(configPath))
            {
                throw new LWConfigurationException("Missing configuration file '" + configPath + "'.");
            }
            return configPath;
        }

        private static string FindKeystoreFile(string normalizedConfigRoot)
        {
            // Determine keystore file path
            string keystorePath = normalizedConfigRoot + KEYSTORE_FILE_NAME;
            if (!File.Exists(keystorePath))
            {
                throw new LWConfigurationException("Missing keystore file '" + keystorePath + "'.");
            }
            return keystorePath;
        }

        private static string FindDBConfigFile(string normalizedConfigRoot)
        {
            // Determine keystore file path
            string dbconfigPath = normalizedConfigRoot + DBCONFIG_FILE_NAME;
            if (!File.Exists(dbconfigPath))
            {
                throw new LWConfigurationException("Missing database config file '" + dbconfigPath + "'.");
            }
            return dbconfigPath;
        }

        private static string FindSymmetricKeystoreFile(string normalizedConfigRoot)
        {
            // Determine keystore file path
            string keystorePath = normalizedConfigRoot + SYMMETRIC_KEY_FILE_NAME;
            if (!File.Exists(keystorePath))
            {
                throw new LWConfigurationException("Missing keystore file '" + keystorePath + "'.");
            }
            return keystorePath;
        }
    }
}
