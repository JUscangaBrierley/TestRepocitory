//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.CodeGen.Schema
{
	public class Generator
	{
		[Flags]
		public enum ScriptType { Init = 1, Backout = 2, All = 3 }

		private class Definitions
		{
			public static readonly Dictionary<string, Type> TypeMapperAttrSet = new Dictionary<string, Type>()
			{
				{"integer", typeof(int)}, 
				{"number", typeof(long)}, 
				{"string", typeof(string)}, 
				{"date", typeof(DateTime)}, 
				{"money", typeof(decimal)}, 
				{"boolean", typeof(bool)}
			};

			public static readonly Dictionary<Type, string> ColumnMapperMsSQL = new Dictionary<Type, string>()
			{
				{typeof(int), "INT"},		
				{typeof(long), "BIGINT"},
				{typeof(string), "NVARCHAR(255)"},
				{typeof(bool), "BIT"},
				{typeof(DateTime), "DATETIME"},
                {typeof(DateTimeOffset), "DATETIME" },  //ToDo: Update this to DATETIMEOFFSET if we ever support SQL SERVER 2008 onwards
				{typeof(float), "FLOAT"},
				{typeof(double), "FLOAT"},
				{typeof(decimal), "DECIMAL(19,5)"},
				{typeof(Guid), "UNIQUEIDENTIFIER"},
				{typeof(Enum), "INT"},
				{typeof(byte[]), "IMAGE"}
			};

			public static readonly Dictionary<Type, string> ColumnMapperOracle = new Dictionary<Type, string>()
			{
				{typeof(int), "NUMBER(10)"}, 
				{typeof(long), "NUMBER(18)"}, 
				{typeof(string), "NVARCHAR2(255)"}, 
				{typeof(bool), "NUMBER(1)"}, 
				{typeof(DateTime), "TIMESTAMP(4)"},
                {typeof(DateTimeOffset),"TIMESTAMP(4) WITH TIME ZONE" },
				{typeof(float), "FLOAT"}, 
				{typeof(double), "FLOAT"}, 
				{typeof(decimal), "NUMBER(19,5)"}, 
				{typeof(Guid), "RAW(16)"}, 
				{typeof(Enum), "NUMBER(10)"}, 
				{typeof(byte[]), "BLOB"}
			};
		}

		private Assembly _assembly = null;
		private SupportedDataSourceType _databaseType;
		private List<string> _classes = new List<string>();
        private Dictionary<string, ColumnProperties> _columnProperties = new Dictionary<string, ColumnProperties>();
		private TablePropeties _tableProperties = new TablePropeties();
        private Dictionary<string, bool> _sequenceNames = new Dictionary<string, bool>();
        private List<ForeignKeyAttribute> _foreignKeys = new List<ForeignKeyAttribute>();
        private List<IndexingProperties> _frameworkIndexes = new List<IndexingProperties>();

        #region Helpers
        private string GetColumnType(SupportedDataSourceType dtType, Type type, ColumnProperties columnProperties, out bool isNullable)
        {
            string returnValue = string.Empty;
            bool persistEnumAsString = columnProperties != null ? columnProperties.PersistEnumAsString : false;
            Type propertyType = Nullable.GetUnderlyingType(type) ?? type;
            propertyType = !propertyType.FullName.ToLower().Contains("system")
                           ? persistEnumAsString
                             ? typeof(string)
                             : GetEnumType(type).BaseType
                           : propertyType;

            isNullable = !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

            switch (dtType)
            {
                case SupportedDataSourceType.MsSQL2005:
                case SupportedDataSourceType.MySQL55:
                    if (Definitions.ColumnMapperMsSQL.ContainsKey(propertyType))
                    {
                        returnValue = Definitions.ColumnMapperMsSQL[propertyType];
                    }
                    break;
                case SupportedDataSourceType.Oracle10g:
                    if (Definitions.ColumnMapperOracle.ContainsKey(propertyType))
                    {
                        returnValue = Definitions.ColumnMapperOracle[propertyType];
                    }
                    break;
            }

            return returnValue;
        }

        private Type GetEnumType(Type t)
        {
            if (t.IsEnum)
            {
                return t;
            }
            if (IsNullableEnum(t))
            {
                return t.GetGenericArguments()[0];
            }
            return null;
        }

        private bool IsNullableEnum(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && t.GetGenericArguments()[0].IsEnum;
        }

        private string GetClassName(string className)
        {
            string returnValue = string.Empty;

            if (!string.IsNullOrEmpty(className))
            {
                string[] tokens = className.Split('.');
                returnValue = tokens[(tokens.Length - 1)];
            }

            return returnValue;
        }
        #endregion


        #region Attribute Set Generation
        public Generator(SupportedDataSourceType dtType)
        {
            _databaseType = dtType;
        }

        public List<string> GenerateModelScripts(AttributeSetMetaData meta, ScriptType scriptType)
        {
            return GenerateModelScript(meta, scriptType);
        }

        public List<string> GenerateModelScript(AttributeSetMetaData meta, ScriptType scriptType)
        {
            List<string> sqlStrings = new List<string>();
            bool isNullable = false;
            string scriptCloser = string.Empty;
            string tableName = ("ats_" + meta.Name).ToUpper();
            string identity = _databaseType == SupportedDataSourceType.MsSQL2005 ? " identity(1,1)" : string.Empty;
            ColumnProperties cProperties = null;

            List<string> tableIndexes = new List<string>();

            switch (_databaseType)
            {
                case SupportedDataSourceType.MsSQL2005:
                case SupportedDataSourceType.MySQL55:
                    tableName = "[" + tableName + "]";
                    break;
                case SupportedDataSourceType.Oracle10g:
                    scriptCloser = ";";
                    break;
            }

            if ((scriptType & ScriptType.Backout) != 0)
            {
                sqlStrings.Add("drop table " + tableName + scriptCloser);
                sqlStrings.AddRange(GenerateAuditLogAttributeSet(meta, ScriptType.Backout));
            }
            if ((scriptType & ScriptType.Init) != 0)
            {
                string sqlString = "create table " + tableName + "(";
                AttributeMetaData attributeStandard = new AttributeMetaData() { ID = -1, AttributeSetCode = meta.ID };
                if (meta.Attributes != null && meta.Attributes.Count > 0)
                {
                    sqlString += " A_RowKey " + GetColumnType(_databaseType, typeof(long), cProperties, out isNullable) + " not null" + identity + ",";
                    switch (meta.Type)
                    {
                        case AttributeSetType.Member:
                            sqlString += " A_IpCode " + GetColumnType(_databaseType, typeof(long), cProperties, out isNullable) + " not null,";
                            attributeStandard.Name = "A_IpCode";
                            tableIndexes.Add(CreateAttributeSetIndex(tableName, attributeStandard, false));
                            attributeStandard.Name = "A_ParentRowKey";
                            tableIndexes.Add(CreateAttributeSetIndex(tableName, attributeStandard, false));
                            break;
                        case AttributeSetType.VirtualCard:
                            sqlString += " A_VcKey " + GetColumnType(_databaseType, typeof(long), cProperties, out isNullable) + " not null,";
                            attributeStandard.Name = "A_VcKey";
                            tableIndexes.Add(CreateAttributeSetIndex(tableName, attributeStandard, false));
                            attributeStandard.Name = "A_ParentRowKey";
                            tableIndexes.Add(CreateAttributeSetIndex(tableName, attributeStandard, false));
                            break;
                        case AttributeSetType.Global:
                            sqlString += " LWIdentifier " + GetColumnType(_databaseType, typeof(long), cProperties, out isNullable) + " not null,";
                            attributeStandard.Name = "LWIdentifier";
                            tableIndexes.Add(CreateAttributeSetIndex(tableName, attributeStandard, true));
                            sqlString += " A_IpCode " + GetColumnType(_databaseType, typeof(long), cProperties, out isNullable) + " not null,";
                            break;
                    }
                    sqlString += " A_ParentRowKey " + GetColumnType(_databaseType, typeof(long), cProperties, out isNullable) + " null,";

                    foreach (AttributeMetaData attribute in meta.Attributes)
                    {
                        cProperties = new ColumnProperties();
                        Type type = typeof(string);
                        if (Definitions.TypeMapperAttrSet.ContainsKey(attribute.DataType.ToLower()))
                        {
                            type = Definitions.TypeMapperAttrSet[attribute.DataType.ToLower()];
                        }
                        string columnType = GetColumnType(_databaseType, type, cProperties, out isNullable);
                        string setNull = attribute.IsRequired ? " not null" : " null";
                        columnType = columnType.ToLower().Contains("nvarchar")
                                     ? attribute.MaxLength > 0
                                       ? columnType.Replace("(255)", "(" + attribute.MaxLength + ")")
                                       : _databaseType == SupportedDataSourceType.MsSQL2005 || _databaseType == SupportedDataSourceType.MySQL55
                                         ? "NTEXT"
                                         : "NCLOB"
                                     : columnType;
                        sqlString += " A_" + attribute.Name + " " + columnType + setNull + ",";
                        if (attribute.IsUnique)
                        {
                            tableIndexes.Add(CreateAttributeSetIndex(tableName, attribute, true));
                        }
                    }

                    cProperties = null;
                    sqlString += " StatusCode " + GetColumnType(_databaseType, typeof(long), cProperties, out isNullable) + " null,";
                    sqlString += " CreateDate " + GetColumnType(_databaseType, typeof(DateTime), cProperties, out isNullable) + " not null,";
                    sqlString += " UpdateDate " + GetColumnType(_databaseType, typeof(DateTime), cProperties, out isNullable) + " null,";
                    sqlString += " LastDmlId " + GetColumnType(_databaseType, typeof(long), cProperties, out isNullable) + " null,";
                    sqlString += " primary key (A_RowKey)";
                    sqlString = sqlString.TrimEnd(',') + ")" + scriptCloser;
                    sqlStrings.Add(sqlString);

                    sqlStrings.AddRange(tableIndexes);
                    sqlStrings.AddRange(GenerateAuditLogAttributeSet(meta, ScriptType.Init));
                }
                else
                {
                    throw new Exception(string.Format("Attribute set {0} has no attributes", meta.Name));
                }
            }

            return sqlStrings;
        }

        private string CreateAttributeSetIndex(string tableName, AttributeMetaData attribute, bool isUnique)
        {
            string indexPrefix = isUnique ? "UDX_" : "IDX_";
            string indexName = indexPrefix + tableName.Replace("[", string.Empty).Replace("]", string.Empty).Replace("ats_", string.Empty) + "_" + attribute.Name;
            if (indexName.Length > 30)
            {
                indexName = indexPrefix + "_ATS" + attribute.AttributeSetCode.ToString() + "_" + attribute.Name.Replace("A_", string.Empty).Replace("a_", string.Empty);
                if (indexName.Length > 30)
                {
                    indexName = indexPrefix + "_ATS" + attribute.AttributeSetCode.ToString() + "_" + attribute.ID;
                }
            }

            if (indexName.Length > 30)
            {
                throw new Exception("Index name " + indexName + " is too long, please adjust accordingly");
            }

            return
                "CREATE " +
                (isUnique ? "UNIQUE " : string.Empty) +
                "INDEX " +
                indexName +
                " ON " +
                tableName.Replace("[", string.Empty).Replace("]", string.Empty) +
                " (" +
                (attribute.ID != -1 ? "A_" : string.Empty) +
                attribute.Name + ");";
        }

        public List<string> GenerateAuditLogAttributeSet(AttributeSetMetaData meta, ScriptType scriptType)
        {
            List<string> sqlStrings = new List<string>();
            string scriptCloser = _databaseType == SupportedDataSourceType.Oracle10g ? ";" : string.Empty;
            string alTableName = ("ats_al_" + meta.Name).ToUpper();
            if(_databaseType == SupportedDataSourceType.MsSQL2005 || _databaseType == SupportedDataSourceType.MySQL55)
                alTableName = string.Format("[{0}]", alTableName);

            if((scriptType & ScriptType.Backout) != 0)
            {
                sqlStrings.Add("drop table " + alTableName + scriptCloser);
            }
            if ((scriptType & ScriptType.Init) != 0)
            {
                if (meta.Attributes == null || meta.Attributes.Count == 0)
                    throw new Exception(string.Format("Attribute set {0} has no attributes", meta.Name));

                string identity = _databaseType == SupportedDataSourceType.MsSQL2005 ? " identity(1,1)" : string.Empty;
                string script = string.Format("create table {0} (", alTableName);
                bool isNullable = false;
                script += string.Format(" AR_ID {0} not null {1},", GetColumnType(_databaseType, typeof(long), null, out isNullable), identity);
                script += string.Format(" AR_APPTYPE {0} null,", GetColumnType(_databaseType, typeof(string), null, out isNullable).Replace("255", "4"));
                script += string.Format(" AR_HOSTNAME {0} null,", GetColumnType(_databaseType, typeof(string), null, out isNullable).Replace("255", "100"));
                script += string.Format(" AR_OWNERID {0} not null,", GetColumnType(_databaseType, typeof(string), null, out isNullable).Replace("255", "50"));
                script += string.Format(" AR_CHANGETYPE {0} not null,", GetColumnType(_databaseType, typeof(string), null, out isNullable).Replace("255", "2")); // TODO: mapper
                script += string.Format(" AR_CREATEDON {0} not null,", GetColumnType(_databaseType, typeof(DateTime), null, out isNullable));
                script += string.Format(" OBJECTID {0} not null,", GetColumnType(_databaseType, typeof(long), null, out isNullable));
                if (meta.Type == AttributeSetType.Global)
                {
                    script += string.Format(" LWIDENTIFIER {0} not null,", GetColumnType(_databaseType, typeof(long), null, out isNullable));
                }
                if(meta.Type == AttributeSetType.VirtualCard)
                    script += string.Format(" VCKEY {0} not null,", GetColumnType(_databaseType, typeof(long), null, out isNullable));
                else
                    script += string.Format(" IPCODE {0} not null,", GetColumnType(_databaseType, typeof(long), null, out isNullable));
                script += string.Format(" A_PARENTROWKEY {0} not null,", GetColumnType(_databaseType, typeof(long), null, out isNullable));

                foreach (AttributeMetaData attribute in meta.Attributes)
                {
                    Type type = typeof(string);
                    if (Definitions.TypeMapperAttrSet.ContainsKey(attribute.DataType.ToLower()))
                    {
                        type = Definitions.TypeMapperAttrSet[attribute.DataType.ToLower()];
                    }
                    string columnType = GetColumnType(_databaseType, type, null, out isNullable);
                    string setNull = attribute.IsRequired ? " not null" : " null";
                    columnType = columnType.ToLower().Contains("nvarchar")
                                 ? attribute.MaxLength > 0
                                   ? columnType.Replace("(255)", "(" + attribute.MaxLength + ")")
                                   : _databaseType == SupportedDataSourceType.MsSQL2005 || _databaseType == SupportedDataSourceType.MySQL55
                                     ? "NTEXT"
                                     : "NCLOB"
                                 : columnType;
                    script += string.Format(" A_{0} {1}{2},", attribute.Name, columnType, setNull);
                }

                script += string.Format(" StatusCode {0} null,", GetColumnType(_databaseType, typeof(long), null, out isNullable));
                script += string.Format(" CreateDate {0} not null,", GetColumnType(_databaseType, typeof(DateTime), null, out isNullable));
                script += string.Format(" UpdateDate {0} null,", GetColumnType(_databaseType, typeof(DateTime), null, out isNullable));
                script += " primary key (AR_ID)";
                script = script.TrimEnd(',') + ")" + scriptCloser;
                sqlStrings.Add(script);
            }
            return sqlStrings;
        }
        #endregion

        public Generator(Assembly assembly, SupportedDataSourceType dtType)
		{
			_assembly = assembly;
			_databaseType = dtType;
		}
        

		public List<string> GenerateModelScripts(IEnumerable<string> namespaces, ScriptType scriptType)
		{
			var ret = new List<string>();
			foreach (string ns in namespaces)
			{
				ret.AddRange(GenerateModelScripts(ns, scriptType));
			}
			return ret;
		}

		public List<string> GenerateModelScripts(string nameSpace, ScriptType scriptType)
		{
			List<string> returnValue = new List<string>();

            if (string.IsNullOrEmpty(nameSpace))
            {
                throw new Exception("The namespace is missing.");
            }
			if (_assembly == null)
			{
				throw new Exception("The assembly is missing.");
			}
			else
			{
                if (PopulateClasses(nameSpace))
                {
                    returnValue = GenerateScripts(scriptType);
                    if (returnValue.Count == 0)
                    {
                        throw new Exception("There was a problem creating the schema scripts");
                    }
                }
                else
                {
                    throw new Exception("The namespace was not found in the assembly");
                }
			}

			return returnValue;
		}

        private List<string> GenerateScripts(ScriptType scriptType)
        {
            List<string> sqlSchema = new List<string>();
            List<string> dropForeignKeyScripts = new List<string>();
            List<string> backoutScripts = new List<string>();
            List<string> initScripts = new List<string>();

            _frameworkIndexes = new List<IndexingProperties>();
            _foreignKeys = new List<ForeignKeyAttribute>();

            if (_assembly != null)
            {
                foreach (var item in _classes)
                {
                    GetTableAttributes(item.ToString());
                    if (!string.IsNullOrEmpty(_tableProperties.TableName))
                    {
                        if ((scriptType & ScriptType.Backout) != 0)
                        {
                            backoutScripts.Add(GenerateModelScript(ScriptType.Backout, item.ToString()));
                        }
                        if ((scriptType & ScriptType.Init) != 0)
                        {
                            initScripts.Add(GenerateModelScript(ScriptType.Init, item.ToString()));
                        }
                    }
                }
                
                if (_databaseType == SupportedDataSourceType.Oracle10g)
                {
                    // Create sequences
                    foreach (KeyValuePair<string, bool> sequence in _sequenceNames.ToList())
                    {
                        if ((scriptType & ScriptType.Backout) != 0)
                        {
                            backoutScripts.Add(CreateSequence(sequence.Key, ScriptType.Backout));
                        }
                        if ((scriptType & ScriptType.Init) != 0 && !sequence.Value)
                        {
                            initScripts.Add(CreateSequence(sequence.Key, ScriptType.Init));
                            _sequenceNames[sequence.Key] = true;
                        }
                    }
                }

                if ((scriptType & ScriptType.Backout) != 0)
                {
                    // Drop foreign keys
                    foreach (ForeignKeyAttribute fkey in _foreignKeys)
                    {
                        dropForeignKeyScripts.Add(CreateForeignKey(fkey, ScriptType.Backout));
                    }
                }

                if ((scriptType & ScriptType.Init) != 0)
                {
                    // Create indexes
                    foreach (IndexingProperties index in _frameworkIndexes)
                    {
                        // Create unique constraints
                        if (index.IndexType == IndexType.UniqueConstraint)
                            initScripts.Add(CreateUniqueConstraint(index));
                        else
                            initScripts.Add(CreateFrameworkIndex(index));
                    }
                    // Create foreign keys
                    foreach (ForeignKeyAttribute fkey in _foreignKeys)
                    {
                        initScripts.Add(CreateForeignKey(fkey, ScriptType.Init));
                    }
                }

                sqlSchema.AddRange(dropForeignKeyScripts);
                sqlSchema.AddRange(backoutScripts);
                sqlSchema.AddRange(initScripts);
            }
            return sqlSchema;
        }

		private bool PopulateClasses(string nameSpace)
		{
			_classes.Clear();

			foreach (Type type in _assembly.GetTypes())
			{
                if (type.Namespace == nameSpace)
				{
					_classes.Add(type.FullName);
				}
			}

			return _classes.Count > 0;
		}

		private void PopulateProperties(string className)
		{
			if (_assembly != null && !string.IsNullOrEmpty(className))
			{
				Type type = _assembly.GetType(className);

				foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
				{
                    GetPropertyAttributes(prop, className);
				}
			}
		}

		private string GenerateModelScript(ScriptType scriptType, string className)
		{
			string sqlString = string.Empty;
			bool isNullable = false;
			string tableName = _tableProperties.TableName.ToUpper();
			string scriptCloser = string.Empty;
			string primaryKey = string.Empty;
			string sqlUnique = string.Empty;
            string cascade = string.Empty;

			switch (_databaseType)
			{
				case SupportedDataSourceType.MsSQL2005:
				case SupportedDataSourceType.MySQL55:
					tableName = "[" + tableName + "]";
					primaryKey = "primary key (" + _tableProperties.PrimaryKey.ColumnName + ")";
					break;
				case SupportedDataSourceType.Oracle10g:
					scriptCloser = ";";
					string primaryKeyTableName = _tableProperties.TableName.Length > 27 ? _tableProperties.TableName.Substring(0, 27) : _tableProperties.TableName;
					primaryKey = string.Format("CONSTRAINT {0}_pk PRIMARY KEY({1})", primaryKeyTableName, _tableProperties.PrimaryKey.ColumnName);
                    cascade = "CASCADE CONSTRAINTS";
                    break;
			}

			if ((scriptType & ScriptType.Backout) != 0)
			{
                sqlString = string.Format("drop table {0} {1} {2}", tableName, cascade, scriptCloser);
			}

			if ((scriptType & ScriptType.Init) != 0)
			{
				sqlString = "create table " + tableName + "(";
                if (_columnProperties != null && _columnProperties.Count > 0)
				{
                    foreach (KeyValuePair<string, ColumnProperties> property in _columnProperties)
					{
						string identity = _tableProperties.PrimaryKey.ColumnName.ToLower() == property.Key.ToLower()
										  && _databaseType == SupportedDataSourceType.MsSQL2005
										  && _tableProperties.PrimaryKey.AutoIncrement ? " identity(1,1)" : string.Empty;

                        string columnType = GetColumnType(_databaseType, property.Value.PropertyType, property.Value, out isNullable);
						string setNull = columnType.ToLower().Contains("nvarchar")
                                  ? !property.Value.isNullable ? " not null" : " null"
								  : isNullable ? " null" : " not null";
						columnType = columnType.ToLower().Contains("nvarchar")
                                     ? property.Value.ColumnLength > 0
                                       ? columnType.Replace("(255)", "(" + property.Value.ColumnLength + ")")
									   : _databaseType == SupportedDataSourceType.MsSQL2005 || _databaseType == SupportedDataSourceType.MySQL55
										 ? "NTEXT"
										 : "NCLOB"
									 : columnType;
						sqlString += " " + property.Key + " " + columnType + setNull + identity + sqlUnique + ",";
					}
					sqlString += !string.IsNullOrEmpty(primaryKey) ? " " + primaryKey : string.Empty;
					sqlString = sqlString.Substring(0, sqlString.Length).TrimEnd(',') + ")" + scriptCloser;
				}
				else
				{
					throw new Exception("The selected class has no properties");
				}
			}
			_tableProperties.TableName = string.Empty; // Why?
			return sqlString;
		}

		private void GetTableAttributes(string className)
		{
            _tableProperties = new TablePropeties();
            _columnProperties = new Dictionary<string, ColumnProperties>();
			if (!string.IsNullOrEmpty(className))
			{
				Type thisClass = _assembly.GetType(className);
                var customAttributes = thisClass.GetCustomAttributes();
                if (customAttributes != null && customAttributes.Count() > 0)
                {
                    foreach (Attribute attribute in customAttributes)
                    {
                        if (attribute is PetaPoco.ExplicitColumnsAttribute)
                        {
                            _tableProperties.ExplicitColumns = true;
                        }

                        if (attribute is PetaPoco.PrimaryKeyAttribute)
                        {
                            PetaPoco.PrimaryKeyAttribute primaryKeyAttribute = (PetaPoco.PrimaryKeyAttribute)attribute;
                            _tableProperties.PrimaryKey.ColumnName = primaryKeyAttribute.Value;
                            _tableProperties.PrimaryKey.SequenceName = (primaryKeyAttribute.sequenceName ?? string.Empty).ToUpper();
                            _tableProperties.PrimaryKey.AutoIncrement = primaryKeyAttribute.autoIncrement;

                            if (!string.IsNullOrEmpty(_tableProperties.PrimaryKey.SequenceName)
                                && !_sequenceNames.ContainsKey(_tableProperties.PrimaryKey.SequenceName))
                            {
                                _sequenceNames.Add(_tableProperties.PrimaryKey.SequenceName, false);
                            }
                        }

                        if (attribute is PetaPoco.TableNameAttribute)
                        {
                            PetaPoco.TableNameAttribute tableName = (PetaPoco.TableNameAttribute)attribute;
                            _tableProperties.TableName = tableName.Value;
                        }

                        if (attribute is ColumnIndexAttribute || attribute is UniqueIndexAttribute)
                        {
                            ColumnIndexAttribute oldAttribute = (ColumnIndexAttribute)attribute;

                            IndexingProperties index = new IndexingProperties();
                            index.IndexType = attribute is UniqueIndexAttribute ? IndexType.Unique : IndexType.Column;
                            index.RequiresLowerFunction = false;
                            index.RequiresSubstrFunction = false;
                            index.TableName = ((PetaPoco.TableNameAttribute)(from a in customAttributes where a is PetaPoco.TableNameAttribute select a).FirstOrDefault()).Value;
                            index.Columns = new List<string>();

                            foreach (string propertyName in oldAttribute.ColumnName.Split(','))
                            {
                                PropertyInfo pi = thisClass.GetProperty(propertyName.Trim(), BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                                if (pi == null)
                                    throw new Exception(string.Format("Invalid property name '{0}' for class '{1}'", propertyName, className));
                                var attr = pi.GetCustomAttribute(typeof(PetaPoco.ColumnAttribute));
                                if (attr == null)
                                    throw new Exception(string.Format("Invalid property '{0}' for class '{1}', missing Column attribute", propertyName, className));
                                var columnAttribute = (PetaPoco.ColumnAttribute)attr;
                                index.Columns.Add(string.IsNullOrEmpty(columnAttribute.Name) ? pi.Name : columnAttribute.Name);
                            }

                            _frameworkIndexes.Add(index);
                        }
                    }
                    PopulateProperties(className);
                }
			}
		}

        private void GetPropertyAttributes(PropertyInfo property, string className)
        {
            var customAttributes = property.GetCustomAttributes();
            if (customAttributes != null && customAttributes.Count() > 0)
            {
                // Get the column info first and then process the rest
                string columnName = string.Empty;
                var columnAtt = (from a in customAttributes where a is PetaPoco.ColumnAttribute && !(a is PetaPoco.ResultColumnAttribute) select a).FirstOrDefault();
                if (columnAtt != null)
                {
                    PetaPoco.ColumnAttribute columnAttribute = (PetaPoco.ColumnAttribute)columnAtt;
                    ColumnProperties columnProperties = new ColumnProperties();
                    columnProperties.ColumnName = !string.IsNullOrEmpty(columnAttribute.Name) ? columnAttribute.Name : property.Name;
                    columnName = columnProperties.ColumnName;
                    columnProperties.ColumnLength = columnAttribute.Length;
                    columnProperties.isNullable = columnAttribute.IsNullable;
                    columnProperties.isEncrypted = columnAttribute.IsEncrypted;
                    columnProperties.PersistEnumAsString = columnAttribute.PersistEnumAsString;
                    columnProperties.PropertyType = property.PropertyType;

                    if (!_columnProperties.ContainsKey(columnProperties.ColumnName))
                    {
                        _columnProperties.Add(columnProperties.ColumnName, columnProperties);
                    }
                    else
                    {
                        throw new Exception("Table Name: " + _tableProperties.TableName + " has duplicate column named: " + columnProperties.ColumnName);
                    }

                    foreach (Attribute attribute in customAttributes)
                    {
                        if (attribute is ForeignKeyAttribute)
                        {
                            ForeignKeyAttribute oldAttribute = (ForeignKeyAttribute)attribute;
                            ForeignKeyAttribute newFKAttribute = new ForeignKeyAttribute();
                            newFKAttribute.ForeignKeyColumn = oldAttribute.ForeignKeyColumn;
                            newFKAttribute.ForeignKeyTable = oldAttribute.ForeignKeyTable;
                            newFKAttribute.PrimaryKeyColumn = string.IsNullOrEmpty(oldAttribute.PrimaryKeyColumn) ? columnName : oldAttribute.PrimaryKeyColumn;
                            newFKAttribute.PrimaryKeyTable = _tableProperties.TableName;
                            newFKAttribute.ForeignKeyName = oldAttribute.ForeignKeyName;

                            _foreignKeys.Add(newFKAttribute);
                        }

                        if (attribute is ColumnIndexAttribute && !(attribute is UniqueIndexAttribute))
                        {
                            ColumnIndexAttribute oldAttribute = (ColumnIndexAttribute)attribute;

                            IndexingProperties index = new IndexingProperties();
                            index.IndexType = IndexType.Column;
                            index.TableName = _tableProperties.TableName;
                            // Adds a lower() to the index
                            index.RequiresLowerFunction = _databaseType == SupportedDataSourceType.Oracle10g && oldAttribute.RequiresLowerFunction;
                            // Adds a to_char(substr(data, 0, 2000)) for a clob index. Only 'used' on StructuredContentData.Data
                            index.RequiresSubstrFunction = _databaseType == SupportedDataSourceType.Oracle10g && property.PropertyType == typeof(string) && columnAttribute.Length == 0;
                            index.Columns = new List<string>() { columnName };
                            _frameworkIndexes.Add(index);
                        }

                        if (attribute is UniqueIndexAttribute)
                        {
                            UniqueIndexAttribute oldAttribute = (UniqueIndexAttribute)attribute;

                            IndexingProperties index = new IndexingProperties();
                            index.TableName = _tableProperties.TableName;
                            index.RequiresLowerFunction = _databaseType == SupportedDataSourceType.Oracle10g && oldAttribute.RequiresLowerFunction;
                            index.RequiresSubstrFunction = false;
                            index.Columns = new List<string>() { columnName };
                            // This is a hack until we get unique constraints separate from unique indexes
                            if (_databaseType == SupportedDataSourceType.Oracle10g && 
                                columnProperties.ColumnName != _tableProperties.PrimaryKey.ColumnName &&
                                !index.RequiresLowerFunction)
                            {
                                index.IndexType = IndexType.UniqueConstraint;
                            }
                            else
                            {
                                index.IndexType = IndexType.Unique;
                            }
                            _frameworkIndexes.Add(index);
                        }
                    }
                }
            }
        }

        private string CreateSequence(string name, ScriptType scriptType)
        {
            if ((scriptType & ScriptType.Backout) != 0)
                return string.Format("drop sequence {0}", name.ToUpper());
            if ((scriptType & ScriptType.Init) != 0)
                return string.Format("create sequence {0} minvalue 1 maxvalue 9223372036854775807 start with 1 increment by 1 cache 20", name.ToUpper());
            return string.Empty;
        }

        private string CreateForeignKey(ForeignKeyAttribute attribute, ScriptType scriptType)
        {
            if ((scriptType & ScriptType.Backout) != 0)
                return string.Format("alter table {0} drop constraint {1}", 
                    attribute.PrimaryKeyTable, attribute.ForeignKeyName);
            if ((scriptType & ScriptType.Init) != 0)
                return string.Format("alter table {0} add constraint {1} foreign key ({2}) references {3} ({4})", 
                    attribute.PrimaryKeyTable, attribute.ForeignKeyName, attribute.PrimaryKeyColumn, attribute.ForeignKeyTable, attribute.ForeignKeyColumn);
            return string.Empty;
        }

        private string CreateFrameworkIndex(IndexingProperties attribute)
        {
            string columnsList = string.Join(",", attribute.Columns);

            if (attribute.RequiresLowerFunction)
                columnsList = string.Format("lower({0})", attribute.Columns[0]);
            if (attribute.RequiresSubstrFunction)
                columnsList = string.Format("to_char(substr({0}, 0, 2000))", attribute.Columns[0]);

            return string.Format("create {0} index {1} on {2} ({3})",
                attribute.IndexType == IndexType.Unique ? "unique" : string.Empty,
                attribute.IndexName,
                attribute.TableName,
                columnsList);
        }

        private string CreateUniqueConstraint(IndexingProperties attribute)
        {
            string columnsList = string.Join(",", attribute.Columns);

            return string.Format("alter table {0} add constraint {1} UNIQUE({2})",
                attribute.TableName,
                attribute.IndexName,
                columnsList);
        }
	}
}
