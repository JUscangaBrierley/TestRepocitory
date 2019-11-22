using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Common.Config
{
    /// <summary>
    /// Database configuration.
    /// </summary>
    public class DBConfig
	{
		public class DBConnectionConfig
		{
			public string Name;
			public SupportedDataSourceType DBType = SupportedDataSourceType.Oracle10g;
			public string ConnectionProperties;
			public string UserID;
			public string EncodedPassword;
			public string DefaultSchema = null;
			public string Server;
			public string Database;

			public string GetConnectionString()
			{
				StringBuilder sb = new StringBuilder();

				string userID = StringUtils.FriendlyString(UserID);
				string password = (string.IsNullOrEmpty(EncodedPassword) ? "" : CryptoUtil.DecodeUTF8(EncodedPassword));
				string server = StringUtils.FriendlyString(Server);
				string database = StringUtils.FriendlyString(Database);
				string credentials;
				switch (DBType)
				{
					case SupportedDataSourceType.Oracle10g:
					default:
						credentials = "User Id=" + userID + ";Password=" + password + ";Data Source=" + database + ";";
						break;
					case SupportedDataSourceType.MsSQL2005:
						credentials = "User Id=" + userID + ";Password=" + password + ";Data Source=" + server + ";Database=" + database + ";";
						break;
					case SupportedDataSourceType.MySQL55:
						credentials = "User Id=" + userID + ";Password=" + password + ";Server=" + server + ";Database=" + database + ";";
						break;
				}
				sb.Append(credentials);

				if (!string.IsNullOrWhiteSpace(ConnectionProperties))
				{
					sb.Append(ConnectionProperties);
				}

				string result = sb.ToString();
				return result;
			}

            /// <summary>
            /// Returns true if UserId, EncodedPassword, and Database are provided
            /// </summary>
            public bool IsValid
            {
                get
                {
                    return !(string.IsNullOrEmpty(UserID) || string.IsNullOrEmpty(EncodedPassword) || string.IsNullOrEmpty(Database));
                }
            }
		}

		public const string FRAMEWORK_DB_CONNECTION_NAME = "FrameworkDB";
        public const string MMS_DB_CONNECTION_NAME = "MMS Database";

        private Dictionary<string, DBConnectionConfig> _connectionConfigs = new Dictionary<string, DBConnectionConfig>();

		/// <summary>
		/// The type of database.
		/// </summary>
		public SupportedDataSourceType DBType
		{
			get { return _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].DBType; }
			set { _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].DBType = value; }
		}

		/// <summary>
		/// The user ID for logging into the database.
		/// </summary>
		public string UserID
		{
			get { return _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].UserID; }
			set { _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].UserID = value; }
		}

		/// <summary>
		/// The default schema to use when connecting to the database (if different than the UserID).
		/// </summary>
		public string DefaultSchema
		{
			get { return _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].DefaultSchema; }
			set { _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].DefaultSchema = value; }
		}

		/// <summary>
		/// The server name on which the database resides.
		/// </summary>
		public string Server
		{
			get { return _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].Server; }
			set { _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].Server = value; }
		}

		/// <summary>
		/// The database or schema name.
		/// </summary>
		public string Database
		{
			get { return _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].Database; }
			set { _connectionConfigs[FRAMEWORK_DB_CONNECTION_NAME].Database = value; }
		}

		public Dictionary<string, DBConnectionConfig> ConnectionConfigs
		{
			get
			{
				return _connectionConfigs;
			}
		}

		/// <summary>
        /// Construct a database configuration.
        /// </summary>
        public DBConfig()
        {
            AddStandardEntries();
        }

        /// <summary>
        /// Construct a database configuration based on the encrypted serialized data.
        /// The data must have been created using the Serialize() method with the same
        /// LWKeystore as specified here.
        /// </summary>
        /// <param name="lwkeystore">a LWKeystore to be used for decryption</param>
        /// <param name="serializedDBConfig">encrypted serialized data</param>
        public DBConfig(LWKeystore lwkeystore, string serializedDBConfig)
        {
            Deserialize(lwkeystore, serializedDBConfig);

            AddStandardEntries();
        }

        /// <summary>
        /// Serialize a database configuration.  This is used to store the configuration
        /// in a database.  The resulting data is encrypted using the specified LWKeystore.
        /// </summary>
        /// <param name="keystore">a LWKeystore to be used for encryption</param>
        /// <returns>encrypted serialized data</returns>
        public string Serialize(LWKeystore keystore)
        {
            if (keystore == null
                || string.IsNullOrEmpty(keystore.EncodedKeystorePass)
                || string.IsNullOrEmpty(keystore.Keystore))
                throw new ArgumentNullException("keystore");

			string json = JsonConvert.SerializeObject(_connectionConfigs);

			string result = CryptoUtil.EncryptAsymmetricUTF8(keystore, json);
            return result;
        }

        /// <summary>
        /// Gets the encoded database password.
        /// </summary>
        /// <returns>encoded database password</returns>
        public string GetEncodedPassword()
        {
			return GetEncodedPassword(FRAMEWORK_DB_CONNECTION_NAME);
        }

		/// <summary>
		/// Gets the encoded database password
		/// </summary>
		/// <param name="connectionName">name of the connection</param>
		/// <returns>encoded database password for the named connection</returns>
		public string GetEncodedPassword(string connectionName)
		{
			if (string.IsNullOrEmpty(connectionName))
				throw new ArgumentNullException("connectionName");
			if (!_connectionConfigs.ContainsKey(connectionName))
				throw new ArgumentException("Invalid connection name: " + connectionName);
	
			string result = _connectionConfigs[connectionName].EncodedPassword;
			return result;
		}

        /// <summary>
        /// Sets the database password using the specified encoded value
        /// </summary>
        /// <param name="encodedPassword">encoded password</param>
        public void SetPassword(string encodedPassword)
        {
			SetPassword(FRAMEWORK_DB_CONNECTION_NAME, encodedPassword);
        }

		/// <summary>
		/// Sets the database password for the named connection using the specified encoded value
		/// </summary>
		/// <param name="connectionName">name of the connection</param>
		/// <param name="encodedPassword">encoded password</param>
		public void SetPassword(string connectionName, string encodedPassword)
		{
			if (string.IsNullOrEmpty(connectionName))
				throw new ArgumentNullException("connectionName");
			if (!_connectionConfigs.ContainsKey(connectionName))
				throw new ArgumentException("Invalid connection name: " + connectionName);
			if (string.IsNullOrEmpty(encodedPassword))
				throw new ArgumentNullException("encodedPassword");
			try
			{
				CryptoUtil.Decode(encodedPassword);
			}
			catch (Exception)
			{
				throw new ArithmeticException("Password argument must be encoded");
			}

			_connectionConfigs[connectionName].EncodedPassword = encodedPassword;
		}

        /// <summary>
        /// Get the encoded connection string.
        /// </summary>
        /// <returns>encoded connection string</returns>
        public string GetEncodedConnectionString()
        {
			return GetEncodedConnectionString(FRAMEWORK_DB_CONNECTION_NAME);
        }

		/// <summary>
		/// Get the encoded connection string for the named connection
		/// </summary>
		/// <returns></returns>
		public string GetEncodedConnectionString(string connectionName)
		{
			if (string.IsNullOrEmpty(connectionName))
				throw new ArgumentNullException("connectionName");
			if (!_connectionConfigs.ContainsKey(connectionName))
				throw new ArgumentException("Invalid connection name: " + connectionName);

			string connectionString = _connectionConfigs[connectionName].GetConnectionString();
			string encodedConnectionString = CryptoUtil.EncodeUTF8(connectionString);
			return encodedConnectionString;
		}

        /// <summary>
        /// Get a dictionary containing the collection of database connection properties.
        /// </summary>
        /// <returns>dictionary of connection properties</returns>
		public string GetConnectionProperties(string connectionName)
        {
            return _connectionConfigs[connectionName].ConnectionProperties;
        }

		public Dictionary<string, DBConnectionConfig> GetConnectionConfigs()
		{
			return _connectionConfigs;
		}

        /// <summary>
        /// Get the database connection property value associated with the specified property name.
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns>property value</returns>
		//public string GetConnectionProperty(string propertyName)
		//{
		//	if (string.IsNullOrEmpty(propertyName))
		//		throw new ArgumentNullException("propertyName");

		//	string result = null;
		//	if (_connectionProperties.ContainsKey(propertyName))
		//	{
		//		result = _connectionProperties[propertyName];
		//	}
		//	return result;
		//}

        /// <summary>
        /// Set the specified database connection property name and value
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <param name="propertyValue">property value</param>
		//public void SetConnectionProperty(string connectionName, string propertyName, string propertyValue)
		//{
		//	if (string.IsNullOrEmpty(propertyName))
		//		throw new ArgumentNullException("propertyName");
		//	if (propertyValue == null)  // "" is OK
		//		throw new ArgumentNullException("propertyValue");

		//	propertyName = propertyName.Trim();
		//	propertyValue = propertyValue.Trim();
		//	if (_connectionConfigs[connectionName].ConnectionProperties.ContainsKey(propertyName))
		//	{
		//		_connectionConfigs[connectionName].ConnectionProperties[propertyName] = propertyValue;
		//	}
		//	else
		//	{
		//		_connectionConfigs[connectionName].ConnectionProperties.Add(propertyName, propertyValue);
		//	}
		//}

        /// <summary>
        /// Delete the specified database connection property.
        /// </summary>
        /// <param name="propertyName">property name</param>
		//public void DeleteConnectionProperty(string connectionName, string propertyName)
		//{
		//	if (string.IsNullOrEmpty(propertyName))
		//		throw new ArgumentNullException("propertyName");

		//	propertyName = propertyName.Trim();
		//	if (_connectionConfigs[connectionName].ConnectionProperties.ContainsKey(propertyName))
		//	{
		//		_connectionConfigs[connectionName].ConnectionProperties.Remove(propertyName);
		//	}
		//	else
		//	{
		//		throw new LWException("Property '" + propertyName + "' not found");
		//	}
		//}

		public bool HasCredentials()
		{
			return HasCredentials(FRAMEWORK_DB_CONNECTION_NAME);
		}

		public bool HasCredentials(string connectionName)
        {            
            string userID = StringUtils.FriendlyString(_connectionConfigs[connectionName].UserID);
            string password = (string.IsNullOrEmpty(_connectionConfigs[connectionName].EncodedPassword) ? "" : CryptoUtil.DecodeUTF8(_connectionConfigs[connectionName].EncodedPassword));
            string server = StringUtils.FriendlyString(_connectionConfigs[connectionName].Server);
            string database = StringUtils.FriendlyString(_connectionConfigs[connectionName].Database);
            return string.IsNullOrWhiteSpace(userID) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(database) ? false : true;                       
        }

        private void Deserialize(LWKeystore keystore, string serializedDBConfig)
        {
            if (keystore == null
                || string.IsNullOrEmpty(keystore.EncodedKeystorePass)
                || string.IsNullOrEmpty(keystore.Keystore))
                throw new ArgumentNullException("keystore");
            if (string.IsNullOrEmpty(serializedDBConfig))
                throw new ArgumentNullException("serializedDBConfig");

			string json = CryptoUtil.DecryptAsymmetricUTF8(keystore, serializedDBConfig);
			if (json.StartsWith("<?xml"))
			{
				LoadOldDBConfig(json);
			}
			else
			{
				_connectionConfigs = JsonConvert.DeserializeObject<Dictionary<string, DBConnectionConfig>>(json);
			}
        }

		private void LoadOldDBConfig(string xml)
		{
			_connectionConfigs = new Dictionary<string, DBConnectionConfig>();
			DBConnectionConfig frameworkDBConnection = new DBConnectionConfig() { Name = FRAMEWORK_DB_CONNECTION_NAME };
			_connectionConfigs.Add(FRAMEWORK_DB_CONNECTION_NAME, frameworkDBConnection);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);
			XmlNode rootNode = doc.DocumentElement;
			if (rootNode.LocalName == "hibernate-configuration" && rootNode.HasChildNodes)
			{
				foreach (XmlNode entryNode in rootNode)
				{
					if (entryNode.NodeType == XmlNodeType.Element && entryNode.LocalName == "session-factory")
					{
						foreach (XmlNode propNode in entryNode)
						{
							if (propNode.NodeType == XmlNodeType.Element && propNode.LocalName == "property")
							{
								string propName = propNode.Attributes["name"].Value;
								string propValue = propNode.InnerText;
								switch (propName)
								{
									case "dialect":
										frameworkDBConnection.DBType = Dialect2DBType(propValue);
										break;
									case "connection.connection_string":
										LoadConnectionProperties(frameworkDBConnection, propValue);
										break;
									case "default":
										break;
								}
							}
						}
					}
				}
			}
		}

		private static SupportedDataSourceType Dialect2DBType(string dialect)
		{
			SupportedDataSourceType result = SupportedDataSourceType.Oracle10g;
			switch (dialect)
			{
				case "NHibernate.Dialect.Oracle10gDialect":
				default:
					result = SupportedDataSourceType.Oracle10g;
					break;
				case "NHibernate.Dialect.MsSql2005Dialect":
					result = SupportedDataSourceType.MsSQL2005;
					break;
				case "NHibernate.Dialect.MySQLDialect":
					result = SupportedDataSourceType.MySQL55;
					break;
			}
			return result;
		}

		private void LoadConnectionProperties(DBConnectionConfig frameworkDBConnection, string DBConnectionString)
		{
			string[] items = DBConnectionString.Split(';');
			if (items != null && items.Length > 0)
			{
				foreach (string item in items)
				{
					string[] nvpair = item.Split('=');
					if (nvpair != null && nvpair.Length >= 2)
					{
						string key = nvpair[0].Trim();
						string value = nvpair[1].Trim();
						if (nvpair.Length > 2)
						{
							for (int i = 2; i < nvpair.Length; i++)
							{
								value += "=" + nvpair[i];
							}
						}
						if (!LoadCredential(frameworkDBConnection, key, value))
						{
							frameworkDBConnection.ConnectionProperties += key + "=" + value + ";";
						}
					}
				}
			}
		}

		private bool LoadCredential(DBConnectionConfig frameworkDBConnection, string key, string value)
		{
			bool credentialWasLoaded = false;
			switch (frameworkDBConnection.DBType)
			{
				case SupportedDataSourceType.Oracle10g:
				default:
					switch (key)
					{
						case "User Id":
							frameworkDBConnection.UserID = value;
							credentialWasLoaded = true;
							break;
						case "Password":
							if (string.IsNullOrEmpty(value))
								frameworkDBConnection.EncodedPassword = null;
							else
								frameworkDBConnection.EncodedPassword = CryptoUtil.EncodeUTF8(value);
							credentialWasLoaded = true;
							break;
						case "Server":
							frameworkDBConnection.Server = value;
							credentialWasLoaded = true;
							break;
						case "Data Source":
							frameworkDBConnection.Database = value;
							credentialWasLoaded = true;
							break;
					}
					break;
				case SupportedDataSourceType.MsSQL2005:
					switch (key)
					{
						case "User Id":
							frameworkDBConnection.UserID = value;
							credentialWasLoaded = true;
							break;
						case "Password":
							if (string.IsNullOrEmpty(value))
							{
								throw new DataServiceConfigurationException("No password defined for framework database.");
							}
							if (string.IsNullOrEmpty(value))
								frameworkDBConnection.EncodedPassword = null;
							else
								frameworkDBConnection.EncodedPassword = CryptoUtil.EncodeUTF8(value);
							credentialWasLoaded = true;
							break;
						case "Data Source":
							frameworkDBConnection.Server = value;
							credentialWasLoaded = true;
							break;
						case "Database":
							frameworkDBConnection.Database = value;
							credentialWasLoaded = true;
							break;
					}
					break;
				case SupportedDataSourceType.MySQL55:
					switch (key)
					{
						case "User Id":
							frameworkDBConnection.UserID = value;
							credentialWasLoaded = true;
							break;
						case "Password":
							if (string.IsNullOrEmpty(value))
								frameworkDBConnection.EncodedPassword = null;
							else
								frameworkDBConnection.EncodedPassword = CryptoUtil.EncodeUTF8(value);
							credentialWasLoaded = true;
							break;
						case "Server":
							frameworkDBConnection.Server = value;
							credentialWasLoaded = true;
							break;
						case "Database":
							frameworkDBConnection.Database = value;
							credentialWasLoaded = true;
							break;
					}
					break;
			}
			return credentialWasLoaded;
		}

        private void AddStandardEntries()
        {
            if (!_connectionConfigs.ContainsKey(MMS_DB_CONNECTION_NAME))
            {
                _connectionConfigs.Add(MMS_DB_CONNECTION_NAME, new DBConnectionConfig() { Name = MMS_DB_CONNECTION_NAME });
            }

            if (!_connectionConfigs.ContainsKey(FRAMEWORK_DB_CONNECTION_NAME))
            {
                _connectionConfigs.Add(FRAMEWORK_DB_CONNECTION_NAME, new DBConnectionConfig() { Name = FRAMEWORK_DB_CONNECTION_NAME });
            }
            }
            }
        }
