using System;
using System.Collections.Generic;
using System.Data;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Data.Sql
{
    public class LWQueryUtil
    {
        private const string _className = "LWQueryUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private enum ExecuteSQLReturnType { Reader, Scalar, NonQuery };

        private PetaPoco.Database _database;

        internal PetaPoco.Database Database
        {
            get
            {
                if (_database == null)
                {
                    ServiceConfig serviceConfiguration = LWDataServiceUtil.GetServiceConfiguration();
                    _database = serviceConfiguration.CreateDatabase();
                }
                if (_database.Connection == null || _database.Connection.State == ConnectionState.Closed || _database.Connection.State == ConnectionState.Broken)
                {
                    _database.OpenSharedConnection();
                }
                return _database;
            }
            set
            {
                _database = value;
            }
        }

        public LWQueryUtil()
        {
        }

        public LWQueryUtil(PetaPoco.Database database)
        {
            _database = database;
        }

        public string GetField(LWDatabaseFieldType field, object fieldValue)
        {
            string method = "GetField";

            string value = string.Empty;
            try
            {
                if (fieldValue != null && fieldValue.GetType() != typeof(System.DBNull))
                {
                    value = fieldValue.ToString();                    
                }
                else if (field.Required)
                {
                    throw new LWValidationException(string.Format("Null value of required field {0} encountered",
                        field.Name));
                }
                if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(field.DefaultValue))
                {
                    value = field.DefaultValue;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, method, "Error getting value for field " + field.Name, ex);
            }
            return value;
        }

        private string GetField(LWDatabaseFieldType field, IDataReader reader)
        {
            return GetField(field, reader[field.Name]);            
        }

        private DbType GetParameterType(string pname)
        {
            DbType dbtype = DbType.Int64;
            switch (pname.ToLower())
            {
                case "int32":
                    dbtype = DbType.Int32;
                    break;
                case "int64":
                    dbtype = DbType.Int64;
                    break;
                case "string":
                    dbtype = DbType.String;
                    break;
                case "datetime":
                    dbtype = DbType.Date;
                    break;
                case "double":
                    dbtype = DbType.Double;
                    break;
                case "decimal":
                    dbtype = DbType.Decimal;
                    break;
            }
            return dbtype;
        }

        private DbType GetParameterType(Type t)
        {
            return GetParameterType(t.Name);
        }

        private SupportedDataSourceType GetDataSourceType()
        {
            LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
            return config.DBConfig.DBType;
        }

        private IDbDataParameter GetOracleProcParameter(IDbCommand cmd)
        {
            string enumType = "Oracle.ManagedDataAccess.Client.OracleDbType";
            string parmType = "Oracle.ManagedDataAccess.Client.OracleParameter";
            string oraAssembly = "Oracle.ManagedDataAccess";
            
            Type[] argTypes = new Type[3];
            argTypes[0] = typeof(string);
            argTypes[1] = Type.GetType(string.Format("{0}, {1}", enumType, oraAssembly));
            argTypes[2] = typeof(ParameterDirection);

            object[] args = new object[3];
            args[0] = "retval";
            args[1] = Enum.Parse(argTypes[1], "RefCursor");
            args[2] = ParameterDirection.Output;

            IDbDataParameter parm = (IDbDataParameter)Brierley.FrameWork.Common.ClassLoaderUtil.CreateInstance(oraAssembly, parmType, argTypes, args);
            return parm;
        }

        private static object GetParmValue(DbType type, object value, string regex)
        {
            object ovalue = System.DBNull.Value;
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                string parmValue = value.ToString();
                if (!string.IsNullOrEmpty(regex))
                {
                    parmValue = System.Text.RegularExpressions.Regex.Replace(value.ToString(), regex, "");
                }
                switch (type)
                {
                    case DbType.Int32:
                        ovalue = System.Convert.ToInt64(parmValue);
                        break;
                    case DbType.Date:
                    case DbType.DateTime:
                        ovalue = DateTime.Parse(parmValue);
                        break;
                    case DbType.Decimal:
                    case DbType.Int64:
                        ovalue = System.Convert.ToDouble(parmValue);
                        break;
                    case DbType.String:
                        ovalue = parmValue;
                        break;
                    case DbType.Boolean:
                        if (value.GetType() == typeof(Boolean))
                        {
                            ovalue = (bool)value ? System.Convert.ToInt64("1") : System.Convert.ToInt64("0");
                        }
                        else
                        {
                            ovalue = int.Parse(parmValue);
                        }
                        break;
                }
            }
            return ovalue;
        }
        

        public LWDataReader ExecuteSQL(string sqlCmd)
        {
            string methodName = "ExecuteSQL";

            try
            {
                IDataReader reader = null;
                _logger.Debug(_className, methodName, "Executing SQL: " + sqlCmd);

                using (IDbCommand cmd = Database.Connection.CreateCommand())
                {
                    cmd.CommandText = sqlCmd;
                    cmd.CommandType = CommandType.Text;
                    reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                return new LWDataReader(reader);
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error processing SQL: " + ex.Message, ex);
                throw;
            }
        }

        public LWDataReader ExecuteSQLReader(string sql, Dictionary<string, object> parameters)
        {
            return (LWDataReader)ExecuteSQLByReturnType(sql, parameters, ExecuteSQLReturnType.Reader);
        }

        public LWDataReader ExecuteSQLReader(string sql, List<IDbDataParameter> parameters)
        {
            return (LWDataReader)ExecuteSQLByReturnType(sql, parameters, ExecuteSQLReturnType.Reader);
        }

        public object ExecuteSQLScalar(string sql, Dictionary<string, object> parameters)
        {
            return ExecuteSQLByReturnType(sql, parameters, ExecuteSQLReturnType.Scalar);
        }

        public object ExecuteSQLScalar(string sql, List<IDbDataParameter> parameters)
        {
            return ExecuteSQLByReturnType(sql, parameters, ExecuteSQLReturnType.Scalar);
        }

        public int ExecuteSQLNonQuery(string sql, Dictionary<string, object> parameters)
        {
            return (int)ExecuteSQLByReturnType(sql, parameters, ExecuteSQLReturnType.NonQuery);
        }

        public int ExecuteSQLNonQuery(string sql, List<IDbDataParameter> parameters)
        {
            return (int)ExecuteSQLByReturnType(sql, parameters, ExecuteSQLReturnType.NonQuery);
        }

        private object ExecuteSQLByReturnType(string sql, object parameters, ExecuteSQLReturnType returnType)
        {
            string methodName = "ExecuteSQL";

            try
            {
                using (IDbCommand cmd = Database.Connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null)
                    {
                        if (parameters is Dictionary<string, object>)
                        {
                            foreach (KeyValuePair<string, object> kvp in (Dictionary<string, object>)parameters)
                            {
                                IDbDataParameter parameter = cmd.CreateParameter();
                                parameter.DbType = GetParameterType(kvp.Value.GetType());
                                parameter.Direction = ParameterDirection.Input;
                                parameter.ParameterName = kvp.Key;
                                parameter.Value = kvp.Value;
                                cmd.Parameters.Add(parameter);
                            }
                        }
                        else if (parameters is List<IDbDataParameter>)
                        {
                            foreach (IDbDataParameter parameter in (List<IDbDataParameter>)parameters)
                            {
                                cmd.Parameters.Add(parameter);
                            }
                        }
                    }

                    switch (returnType)
                    {
                        case ExecuteSQLReturnType.Reader:
                            return new LWDataReader(cmd.ExecuteReader(CommandBehavior.CloseConnection));
                        case ExecuteSQLReturnType.Scalar:
                            return cmd.ExecuteScalar();
                        case ExecuteSQLReturnType.NonQuery:
                            return cmd.ExecuteNonQuery();
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error processing SQL: " + sql, ex);
                throw ex;
            }
            finally
            {
                if (_database != null && returnType != ExecuteSQLReturnType.Reader)
                    _database.Dispose();
            }
        }

        public List<Dictionary<string, string>> ExecuteSQL<T>
            (
            string sqlCmd, 
            string queryParmName, 
            List<T> set, 
            string[] orderBy, 
            Dictionary<string, 
            LWDatabaseFieldType> fields, 
            string whereAppender
            )
        {
            string methodName = "ExecuteSQL";
            
            _logger.Debug(_className, methodName, "Executing SQL: " + sqlCmd);

            LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();

            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            int maxBatchSize = 1000;
            for (int batch = 0; batch <= (set.Count - 1) / maxBatchSize; batch++)
            {
                string sqlQuery = string.Format("{0} where {1} in (", sqlCmd, queryParmName);

                try
                {
                    using (Database)
                    {
                        IDbConnection conn = Database.Connection;
                        string prefix = string.Empty;
                        if (config.DBConfig.DBType == SupportedDataSourceType.MsSQL2005)
                        {
                            prefix = "@";
                        }
                        else
                        {
                            prefix = ":";
                        }

                        T[] setsBatch = set.Count <= maxBatchSize ? set.ToArray() : LWQueryBatchInfo.GetBatchValues(set.ToArray(), batch * maxBatchSize, maxBatchSize, true);
                        using (IDbCommand cmd = conn.CreateCommand())
                        {
                            int idx = 0;
                            bool first = true;
                            foreach (var ipcode in setsBatch)
                            {
                                IDbDataParameter parm = cmd.CreateParameter();
                                parm.ParameterName = prefix + "p" + idx.ToString();
                                if (first)
                                {
                                    sqlQuery += parm.ParameterName;
                                    first = false;
                                }
                                else
                                {
                                    sqlQuery = string.Format("{0},{1}", sqlQuery, parm.ParameterName);
                                }
                                parm.DbType = GetParameterType(typeof(T));
                                parm.Value = ipcode;
                                cmd.Parameters.Add(parm);
                                idx++;
                            }
                            sqlQuery += ")";
                            if (!string.IsNullOrEmpty(whereAppender))
                            {
                                sqlQuery = string.Format("{0} and {1}", sqlQuery, whereAppender);
                            }
                            if (orderBy != null && orderBy.Length > 0)
                            {
                                bool firstob = true;
                                sqlQuery = string.Format("{0} order by ", sqlQuery);
                                foreach (string ob in orderBy)
                                {
                                    if (!firstob)
                                    {
                                        sqlQuery += ",";
                                    }
                                    else
                                    {
                                        firstob = false;
                                    }
                                    sqlQuery += ob;
                                }
                            }
                            cmd.CommandText = sqlQuery;
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            _logger.Debug(_className, methodName, "Executing query: " + sqlQuery);

                            using (LWDataReader reader = new LWDataReader(cmd.ExecuteReader()))
                            {
                                while (reader.Next())
                                {
                                    Dictionary<string, string> row = new Dictionary<string, string>();
                                    foreach (LWDatabaseFieldType field in fields.Values)
                                    {
                                        try
                                        {
                                            string value = GetField(field, reader.GetData(field.Name));
                                            row.Add(field.Name, value);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Error(_className, methodName, "Error extracting field " + field.Name, ex);
                                            throw;
                                        }
                                    }
                                    result.Add(row);
                                }
                            }
                        }
                    }
                }
                finally
                {
                }
            }
            return result;
        }

        public LWDataReader ExecuteStoredProc(string procName, List<LWDatabaseFieldType> parameters, bool hasReturnParm)
        {
            string methodName = "ExecuteStoredProc";

            using (IDbCommand cmd = Database.Connection.CreateCommand())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = procName;
                IDbDataParameter parm = null;
				if (parameters != null)
				{
					foreach (LWDatabaseFieldType field in parameters)
					{
						DbType actualType = System.Data.DbType.String;
						parm = cmd.CreateParameter();
						if (field.Type == DataType.Boolean && GetDataSourceType() == SupportedDataSourceType.Oracle10g)
						{
							parm.DbType = DbType.Int32;
							actualType = DbType.Boolean;
						}
						else if (field.Type == DataType.Integer)
						{
							parm.DbType = DbType.Int32;
							actualType = DbType.Int32;
						}
						else
						{
							parm.DbType = (DbType)Enum.Parse(typeof(DbType), field.Type.ToString());
							actualType = parm.DbType;
						}
						parm.ParameterName = field.Name;
						parm.Direction = ParameterDirection.Input;
						parm.Value = GetParmValue(actualType, field.Value, field.Regex);
						_logger.Debug(_className, methodName, string.Format("Parameter Name = {0} - Value = {1}", parm.ParameterName, parm.Value));
						cmd.Parameters.Add(parm);
					}
				}

                if (GetDataSourceType() == SupportedDataSourceType.Oracle10g && hasReturnParm)
                {
                    // add out parameter                        
                    IDbDataParameter oparm = GetOracleProcParameter(cmd);
                    cmd.Parameters.Add(oparm);
                }
                else if (GetDataSourceType() == SupportedDataSourceType.MsSQL2005)
                {
                }
                return new LWDataReader(cmd.ExecuteReader(CommandBehavior.CloseConnection));
            }
        }
        
        public void DataTableFill(string sql, List<IDbDataParameter> parameters, ref DataTable table)
        {
            const string methodName = "DataTableFill";

            try
            {
                using (IDbCommand cmd = Database.Connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    table.Load(cmd.ExecuteReader());                    
                }
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error processing SQL: " + sql, ex);
                throw ex;
            }
            finally
            {
                if (_database != null)
                {
                    _database.Dispose();
                }
            }
        }

        public void TruncateTable(string tableName)
        {
            string methodName = "TruncateTable";

            using(Database)
            using (IDbCommand cmd = Database.Connection.CreateCommand())
            {
                string sqlCmd = string.Format("truncate table {0}", tableName);
                _logger.Debug(_className, methodName, sqlCmd);
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;
                int nRows = cmd.ExecuteNonQuery();
            }
        }

        public void InsertRecord(string tableName, string primaryKey, List<LWDatabaseFieldType> fields)
        {
            string methodName = "InsertRecord";

            string prefix = string.Empty;
            long id = -1;
            LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
            if (config.DBConfig.DBType == SupportedDataSourceType.MsSQL2005)
            {
                prefix = "@";
            }
            else
            {
                prefix = ":";
            }

            if (!string.IsNullOrEmpty(primaryKey) && config.DBConfig.DBType == SupportedDataSourceType.Oracle10g)
            {
                using (DataService dataService = LWDataServiceUtil.DataServiceInstance())
                    id = dataService.GetNextSequence();
            }

            string columns = string.Empty;
            string values = string.Empty;

            using(Database)
            using (IDbCommand cmd = Database.Connection.CreateCommand())
            {
                int idx = 0;
                if (id != -1)
                {
                    IDbDataParameter parm = cmd.CreateParameter();
                    parm.ParameterName = prefix + "p" + idx.ToString();
                    columns = string.Format("{0}{1}", columns, primaryKey);
                    values = string.Format("{0}{1}", values, parm.ParameterName);
                    parm.DbType = GetParameterType(typeof(Int64));
                    parm.Value = id;
                    cmd.Parameters.Add(parm);
                    idx++;
                }
                foreach (LWDatabaseFieldType field in fields)
                {
                    if (!string.IsNullOrEmpty(columns))
                    {
                        columns += ",";
                        values += ",";
                    }

                    object value = field.Value;

                    IDbDataParameter parm = cmd.CreateParameter();
                    parm.ParameterName = prefix + "p" + idx.ToString();
                    columns = string.Format("{0}{1}", columns, field.Name);
                    values = string.Format("{0}{1}", values, parm.ParameterName);
                    parm.DbType = GetParameterType(value.GetType());
                    parm.Value = value;
                    cmd.Parameters.Add(parm);
                    idx++;
                }
                string sqlCmd = string.Format("insert into {0} ({1}) values({2})", tableName, columns, values);
                _logger.Debug(_className, methodName, sqlCmd);
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;
                int nRows = cmd.ExecuteNonQuery();
            }
        }

        public void UpdateRecord(string tableName, string primaryKey, List<LWDatabaseFieldType> fields)
        {
            string methodName = "UpdateRecord";

            string prefix = string.Empty;
            LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
            if (config.DBConfig.DBType == SupportedDataSourceType.MsSQL2005)
            {
                prefix = "@";
            }
            else
            {
                prefix = ":";
            }

            using(Database)
            using (IDbCommand cmd = Database.Connection.CreateCommand())
            {
                int idx = 0;
                string updateCmd = string.Empty;
                string whereCmd = string.Empty;
                foreach (LWDatabaseFieldType field in fields)
                {
                    object value = field.Value;
                    if (field.Name == primaryKey)
                    {                      
                        whereCmd = string.Format("where {0}={1}", field.Name, value.ToString());
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(updateCmd))
                        {
                            updateCmd += ",";
                        }
                        else
                        {
                            updateCmd += "set ";
                        }

                        string parmName = prefix + "p" + idx.ToString();
                        IDbDataParameter parm = cmd.CreateParameter();
                        parm.ParameterName = parmName;
                        parm.DbType = GetParameterType(value.GetType());
                        parm.Value = value;
                        cmd.Parameters.Add(parm);

                        updateCmd = string.Format("{0} {1}={2}", updateCmd, field.Name, parmName);
                    }
                    idx++;
                }

                string sqlCmd = string.Format("update {0} {1} {2}", tableName, updateCmd, whereCmd);

                _logger.Debug(_className, methodName, sqlCmd);
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;
                int nRows = cmd.ExecuteNonQuery();
                _logger.Trace(_className, methodName, nRows + " have been updated.");
            }
        }

        public void UpdateRecords(string tableName, string primaryKey, List<LWDatabaseFieldType> fields)
        {
            string methodName = "UpdateRecords";

            string prefix = string.Empty;
            LWConfiguration config = LWConfigurationUtil.GetCurrentConfiguration();
            if (config.DBConfig.DBType == SupportedDataSourceType.MsSQL2005)
            {
                prefix = "@";
            }
            else
            {
                prefix = ":";
            }
            
            using(Database)
            using (IDbCommand cmd = Database.Connection.CreateCommand())
            {
                int idx = 0;
                string updateCmd = string.Empty;
                string keyset = string.Empty;
                foreach (LWDatabaseFieldType field in fields)
                {
                    object value = field.Value;
                    if (field.Name == primaryKey)
                    {
                        if (!string.IsNullOrEmpty(keyset))
                        {
                            keyset += ",";
                        }
                        keyset = string.Format("{0}{1}", keyset, value.ToString());
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(updateCmd))
                        {
                            updateCmd += ",";
                        }
                        else
                        {
                            updateCmd += "set ";
                        }

                        string parmName = prefix + "p" + idx.ToString();
                        IDbDataParameter parm = cmd.CreateParameter();
                        parm.ParameterName = parmName;
                        parm.DbType = GetParameterType(value.GetType());
                        parm.Value = value;
                        cmd.Parameters.Add(parm);

                        updateCmd = string.Format("{0} {1}={2}", updateCmd, field.Name, parmName);
                    }
                    idx++;
                }

                string sqlCmd = string.Format("update {0} {1} where {2} in ({3})", tableName, updateCmd, primaryKey, keyset);

                _logger.Debug(_className, methodName, sqlCmd);
                cmd.CommandText = sqlCmd;
                cmd.CommandType = CommandType.Text;
                int nRows = cmd.ExecuteNonQuery();
                _logger.Trace(_className, methodName, nRows + " have been updated.");
            }
        }

        public void Dispose()
        {
            string methodName = "Dispose";

            _logger.Trace(_className, methodName, "Disposing simple database provider.");
            
            try
            {                
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error disposing session.", ex);
            }
        }
    }
}
