//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Brierley.FrameWork.CampaignManagement.DataProvider.Oracle;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DataProvider
{
    public class CampaignDataProvider
    {
        private const int _maxIdentifierLength = 30;

        private static string[] _operators = new string[] { "<=", ">=", "<>", "<", ">", "=", "like" };
        private static string[] _noOperator = new string[] { "is null", "is not null", "in" };
        private static string[] _qualifiers = new string[] { "(", ")", "and", "or" };
        private static string[] _functions = new string[] { "lower", "upper", "length" };
        private static string[] _aggregates = new string[] { "sum", "avg", "count", "min", "max" };

        private Dictionary<SupportedDataSourceType, ICMQueryManager> _queryMgrMap;

        public string ConnectionString { get; set; }

        public DbProviderFactory Factory { get; set; }

        public string DataSchema { get; set; }

        public string DataSchemaPrefix
        {
            get
            {
                if (string.IsNullOrEmpty(DataSchema))
                {
                    return string.Empty;
                }
                else
                {
                    return _queryMgrMap[DatabaseType].GetDataSchemaPrefix(DataSchema);
                }
            }
        }

        public SupportedDataSourceType DatabaseType { get; set; }

        public Dictionary<SupportedDataSourceType, ICMQueryManager> QueryManagerMap
        {
            get { return _queryMgrMap; }
            set { _queryMgrMap = value; }
        }

        public ICMQueryManager QueryManager
        {
            get
            {
                return _queryMgrMap[DatabaseType];
            }
        }

        public IBulkOutputProvider BulkOutputProvider
        {
            get { return _queryMgrMap[DatabaseType].BulkOutputProvider; }
        }

        public bool UsesFramework { get; set; }

        public bool IndexTempTables { get; set; }

        public bool UseArrayBinding { get; set; }

        public string ParameterPrefix
        {
            get
            {
                return _queryMgrMap[DatabaseType].GetParameterPrefix();
            }
        }

        public CampaignDataProvider(SupportedDataSourceType databaseType, string connectionString)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            Factory = LWDataServiceUtil.ResolveProvider(databaseType);

            _queryMgrMap = new Dictionary<SupportedDataSourceType, ICMQueryManager>();

            ICMQueryManager mgr = null;

            if (databaseType == SupportedDataSourceType.Oracle10g)
            {
                mgr = new OracleQueryManager();
            }
            else if (databaseType == SupportedDataSourceType.MsSQL2005)
            {
                mgr = new MsSQL.MsSQLQueryManager();
            }
            else
            {
                throw new ArgumentException(string.Format("Unknown or unsupported database type {0} provided.", databaseType));
            }

            _queryMgrMap.Add(databaseType, mgr);
        }

        public bool TableExists(string tableName, bool useAlternateSchema)
        {
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            DataTable table = new DataTable();
            DataTableFill(table, queryMgr.GetTableExistsQuery(useAlternateSchema ? DataSchema : null, tableName));
            return table.Rows.Count > 0;
        }

        public bool FieldExists(string tableName, string fieldName, bool useAlternateSchema)
        {
            return !string.IsNullOrEmpty(GetFieldType(tableName, fieldName, useAlternateSchema));
        }

        public DataSet GetAvailableTables()
        {
            DataSet tableSet = new DataSet();
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            DataTable table = new DataTable();
            DataTableFill(table, queryMgr.GetTableNameQuery(null));
            tableSet.Tables.Add(table);
            tableSet.Tables[0].TableName = "CampaignSchema";
            if (!string.IsNullOrEmpty(DataSchema))
            {
                table = new DataTable();
                DataTableFill(table, queryMgr.GetTableNameQuery(DataSchema));
                tableSet.Tables.Add(table);
                tableSet.Tables[1].TableName = "DataSchema";
            }
            return tableSet;
        }

        public DataTable GetTableDetails(string tableName, bool useAlternateSchema)
        {
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            DataTable table = new DataTable();
            DataTableFill(table, queryMgr.GetTableDetailQuery(useAlternateSchema ? DataSchema : null, tableName));
            return table;
        }

        public void DropTable(string tableName)
        {
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            if (!TableExists(tableName, false))
            {
                return;
            }
            ExecuteNonQuery(queryMgr.GetDropTableQuery(tableName));
        }

        public void CreateCampaignHistoryTable()
        {
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            ExecuteNonQuery(queryMgr.GetCreateHistoryTableStatement());
        }

        public void CreateTable(string tableName, string fieldName, string fieldType)
        {
            const string createString = "CREATE TABLE {0} ({1} {2})";
            if (TableExists(tableName, false))
            {
                throw new Exception("Cannot create table " + tableName + " because it already exists.");
            }

            ExecuteNonQuery(string.Format(createString, tableName, fieldName, fieldType));

            if (IndexTempTables)
            {
                string indexName = "IDX_" + tableName + fieldName;
                if (indexName.Length > 30)
                {
                    indexName = indexName.Substring(0, 30);
                }

                ExecuteNonQuery("CREATE INDEX " + indexName + " ON " + tableName + "(" + fieldName + ")");
            }
        }

        public void AddFieldToTable(string tableName, string fieldName, string dataType)
        {
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            ExecuteNonQuery(queryMgr.AppendFieldQuery(tableName, fieldName, dataType));
        }

        public void RemoveFieldFromTable(string tableName, string fieldName)
        {
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            ExecuteNonQuery(queryMgr.DropFieldQuery(tableName, fieldName));
        }

        public void TruncateTable(string tableName)
        {
            if (!TableExists(tableName, false))
            {
                return;
            }
            ExecuteNonQuery("TRUNCATE TABLE " + tableName);
        }

        public int RowCountEstimate(string tableName, bool useAlternateSchema)
        {
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            return int.Parse(ExecuteScalar(queryMgr.GetRowCountEstimateQuery(useAlternateSchema ? DataSchema : null, tableName)).ToString());
        }

        public int RowCountExact(string tableName)
        {
            return int.Parse(ExecuteScalar("SELECT count(*) as NumRows FROM " + tableName).ToString());
        }

        public string GetFieldType(string tableName, string fieldName, bool useAlternateSchema)
        {
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            DataTable table = new DataTable();
            DataTableFill(table, queryMgr.GetFieldTypeQuery(useAlternateSchema ? DataSchema : null, tableName, fieldName));
            string fieldType = null;
            if (table.Rows.Count > 0)
            {
                fieldType = table.Rows[0]["DataType"].ToString();
                switch (fieldType.ToUpper())
                {
                    case "CHAR":
                    case "NCHAR":
                    case "VARCHAR":
                    case "NVARCHAR":
                    case "VARCHAR2":
                    case "NVARCHAR2":
                        fieldType += "(" + table.Rows[0]["Length"].ToString() + ")";
                        break;
                    default:
                        break;
                }
            }
            return fieldType;
        }

        public bool IsValidSqlStatement(List<SqlStatement> sql, Dictionary<string, object> overrideParameters, ref string errors)
        {
            bool isValid = true;
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];

            try
            {
                foreach (SqlStatement s in sql)
                {
                    CheckForSqlInjection(s.ToString());
                    if (s.Parameters != null)
                    {
                        ExecuteNonQuery(queryMgr.GetValidateSqlQuery(s).ToString(), GetDBParameters(s.Parameters, overrideParameters));
                    }
                    else
                    {
                        ExecuteNonQuery(queryMgr.GetValidateSqlQuery(s).ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // explain plan has been removed in favor of either (1) setting row limit to 0, or (2) appending "where 1=2"
                //Oracle's explain plan statement may throw an error that there are insufficient privileges on a view used across schemas. If
                //this is the case, we can only warn the user we're unable to validate the query.
                if (ex.Message.Contains("ORA-01039"))
                {
                    errors += "Campagin Management could not validate the query due to insufficient privileges. The query may still be valid, but it cannot be determined at this point. " + ex.Message;
                }
                else
                {
                    isValid = false;
                    errors = ex.Message;
                }
            }
            return isValid;
        }

        public IDataReader ExecuteReader(string sql)
        {
            using (var command = Factory.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                var connection = Factory.CreateConnection();
                connection.ConnectionString = ConnectionString;
                connection.Open();
                command.Connection = connection;
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public int Execute(SqlStatement statement, Dictionary<string, object> overrideParameters)
        {
            CheckForSqlInjection(statement.ToString());
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            int rowCount = ExecuteNonQuery(statement.ToString(), GetDBParameters(statement.Parameters, overrideParameters));
            return rowCount;
        }

        public DataTable ExecuteDataTable(SqlStatement statement, Dictionary<string, object> overrideParameters)
        {
            var parameters = GetDBParameters(statement.Parameters, overrideParameters);
            return ExecuteDataTable(statement.ToString(), parameters);
        }

        public DataTable ExecuteDataTable(string sql, List<IDbDataParameter> parameters = null)
        {
            DataTable table = new DataTable();
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            int rowCount = 0; // AdoTemplate.DataTableFill(table, System.Data.CommandType.Text, Sql);

            if (parameters != null && parameters.Count > 0)
            {
                rowCount = DataTableFill(table, sql, parameters);
            }
            else
            {
                rowCount = DataTableFill(table, sql);
            }

            return table;
        }

        public DataTable GetDistinctValues(CampaignTable campaignTable, string fieldName)
        {
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            string schemaName = string.Empty;
            if (!string.IsNullOrEmpty(DataSchema))
            {
                if (!campaignTable.IsCampaignSchema)
                {
                    schemaName = DataSchema;
                }
            }
            DataTable table = new DataTable();
            DataTableFill(table, queryMgr.GetDistinctValuesQuery(schemaName, campaignTable.Name, fieldName));
            return table;
        }

        public List<string> CreateSqlStatement(SqlQuery query)
        {
            List<string> ret = new List<string>();
            ICMQueryManager queryMgr = _queryMgrMap[DatabaseType];
            List<string> fieldList = new List<string>();

            if (query.StepType == StepType.Merge)
            {
                throw new Exception("Cannot create union queries through CampaignDataProvider.");
            }

            StringBuilder sql = new StringBuilder();
            List<string> includedTables = new List<string>();

            if (query.CreateTable && !string.IsNullOrEmpty(query.ActionTable))
            {
                sql.Append(queryMgr.GetCreateTableClause(query.ActionTable));
                sql.Append(" ");
            }


            if (!string.IsNullOrEmpty(query.ActionTable) && !query.CreateTable)
            {
                sql.Append("INSERT INTO ");
                sql.Append(query.ActionTable);
                sql.Append("(");

                fieldList = new List<string>();
                fieldList.AddRange(query.InsertFieldList);
                fieldList.AddRange(query.RootTableCarryover);
                sql.Append(string.Join(", ", fieldList.ToArray()));
                sql.Append(")\r\n");
            }

            if (query.Limit > -1)
            {
                //this call will correctly place the DISTINCT clause, if required...
                sql.Append(queryMgr.GetLimitQueryStart(query));
            }
            else
            {
                sql.Append("SELECT");
                ///...otherwise, we'll place it here:
                if (query.DistinctRows)
                {
                    sql.Append(" DISTINCT");
                }
            }


            sql.Append("\r\n");


            List<string> select = new List<string>();

            foreach (var col in query.Columns.Where(o => o.ColumnType == ColumnType.Condition && o.IncludeInOutput))
            {
                string column = string.Empty;

                if (!string.IsNullOrEmpty(col.AggregateExpression))
                {
                    column = col.AggregateExpression + "({0})";
                }
                else
                {
                    column = "{0}";
                }
                if (!string.IsNullOrWhiteSpace(col.NonAliasedColumnExpression))
                {
                    column = string.Format(column, col.NonAliasedColumnExpression);
                }
                else
                {
                    column = string.Format(column, string.Format("{0}.{1}", col.TableName, col.FieldName));
                }

                if (!string.IsNullOrEmpty(col.OutputAs))
                {
                    column += string.Format(" AS {0}", col.GetOutputAsToken(query.Columns));
                }
                select.Add(column.ToString());
            }

            select.AddRange(query.SelectFieldList);





            //apply segment/assignment columns
            if (query.StepType != StepType.DeDupe)
            {
                foreach (QueryColumn column in query.Columns)
                {
                    if (column.ColumnType == ColumnType.Segment)
                    {
                        select.Add(column.NonAliasedColumnExpression + " as " + column.FieldName);
                    }
                    else if (column.ColumnType == ColumnType.Append)
                    {
                        if (NeedsCaseStatements(query))
                        {
                            BuildCaseStatement(query, column, select);
                        }
                        else
                        {
                            //case statements are not needed. Just use the expression to insert:
                            bool foundExpression = false;
                            foreach (ColumnCondition condition in column.Conditions)
                            {
                                if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression))
                                {
                                    foundExpression = true;
                                    select.Add(GetAssignmentConditionExpression(query, column.Conditions[0]) + " AS " + column.FieldName);
                                    break;
                                }
                            }
                            //todo: Added "&& Query.StepType != StepType.DeDupe" to prevent the "null as X" from being inserted into the query.
                            //This, to me, is a sign of things beginning to break down - the dedupe query is getting special treatment and before
                            //you know it, all query/step types are getting special treatment. This entire method should probably be re-written
                            //along with SqlQuery so that it only supports basic structure and construction; anything else can construct its
                            //own query using the appropriate QueryManager functions. 
                            if (!foundExpression && query.StepType != StepType.DeDupe)
                            {
                                select.Add("null AS " + column.FieldName);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < query.RootTableCarryover.Count; i++)
            {
                select.Add(query.RootTableName + "." + query.RootTableCarryover[i]);
            }


            //append select list to query
            for (int i = 0; i < select.Count; i++)
            {
                sql.Append("    ");
                sql.Append(select[i]);
                if (i < select.Count - 1)
                {
                    sql.AppendLine(", ");
                }
            }


            if (query.CreateTable)
            {
                string into = queryMgr.GetInsertIntoClause(query.ActionTable);
                if (!string.IsNullOrEmpty(into))
                {
                    sql.AppendLine();
                    sql.Append(into);
                }
            }



            sql.Append("\r\nFROM\r\n    ");
            sql.Append(query.RootTableName);
            sql.Append(" ");


            foreach (Join join in query.Joins)
            {
                if (!string.IsNullOrEmpty(join.Query))
                {
                    //new type of join - joining subquery for use with the pivot step
                    sql.Append(string.Format("\r\n    {0} JOIN ", join.JoinType.ToString().ToUpper()));
                    sql.AppendFormat("(\r\n        {0}\r\n    ) {1}", join.Query.Replace("    ", "        ").Replace("\r\n", "\r\n        "), join.Alias);
                    sql.Append(" ON ");
                    sql.Append(join.Alias);
                    sql.Append(".");
                    sql.Append(join.FieldName);
                    sql.Append(" = ");
                    sql.Append(join.JoinToTableName);
                    sql.Append(".");
                    sql.Append(join.JoinToFieldName);
                    sql.Append(" ");
                    if (!string.IsNullOrEmpty(join.JoinCondition))
                    {
                        sql.AppendFormat("AND {0} ", join.JoinCondition);
                    }
                }
                else if (!includedTables.Contains(join.TableName))
                {
                    includedTables.Add(join.TableName);
                    sql.Append(string.Format("\r\n    {0} JOIN ", join.JoinType.ToString().ToUpper()));
                    sql.Append(join.TableName);
                    sql.Append(" ON ");
                    sql.Append(join.TableName);
                    sql.Append(".");
                    sql.Append(join.FieldName);
                    sql.Append(" = ");
                    sql.Append(join.JoinToTableName);
                    sql.Append(".");
                    sql.Append(join.JoinToFieldName);
                    sql.Append(" ");
                }
            }

            if (query.StepType == StepType.DeDupe)
            {
                //append derived table
                QueryColumn groupField = null;
                QueryColumn totalField = null;
                string aggregate = string.Empty;

                if (query.Columns.Count >= 2)
                {
                    foreach (QueryColumn column in query.Columns)
                    {
                        if (string.IsNullOrEmpty(column.AggregateExpression))
                        {
                            groupField = column;
                        }
                        else
                        {
                            totalField = column;
                            aggregate = column.AggregateExpression;
                        }
                    }

                    sql.Append("\r\n    INNER JOIN \r\n    (\r\n        SELECT ");

                    sql.Append(groupField.TableName + "." + groupField.FieldName + " AS \"" + groupField.FieldName + "\"");
                    sql.Append(", ");

                    sql.Append(aggregate);
                    sql.Append("(");
                    sql.Append(totalField.TableName + "." + totalField.FieldName);
                    sql.Append(") AS ");
                    sql.Append(totalField.FieldName);

                    sql.Append("\r\n        FROM ");
                    sql.Append(query.RootTableName);

                    var dedupeTables = new List<string>();
                    foreach (Join join in query.Joins)
                    {
                        if (!dedupeTables.Contains(join.TableName))
                        {
                            dedupeTables.Add(join.TableName);
                            sql.Append(string.Format("\r\n        {0} JOIN ", join.JoinType.ToString().ToUpper()));
                            sql.Append(join.TableName);
                            sql.Append(" ON ");
                            sql.Append(join.TableName);
                            sql.Append(".");
                            sql.Append(join.FieldName);
                            sql.Append(" = ");
                            sql.Append(join.JoinToTableName);
                            sql.Append(".");
                            sql.Append(join.JoinToFieldName);
                            sql.Append(" ");
                        }
                    }


                    sql.Append("\r\n        GROUP BY ");

                    sql.Append(groupField.TableName + "." + groupField.FieldName);

                    sql.Append("\r\n    )\r\n    t2 ON t2." + groupField.FieldName + " = " + groupField.TableName + "." + groupField.FieldName + " AND ");
                    sql.Append("t2." + totalField.FieldName + " = " + totalField.TableName + "." + totalField.FieldName);

                    if (query.GroupBy != null && query.GroupBy.Count > 0)
                    {
                        sql.Append("\r\nGROUP BY");
                        for (int i = 0; i < query.GroupBy.Count; i++)
                        {
                            if (i > 0)
                            {
                                sql.Append(", ");
                            }
                            sql.Append("\r\n    " + query.GroupBy[i]);
                        }
                    }
                }
            }

            if (query.StepType != StepType.DeDupe)
            {
                bool hasConditions = HasWhereConditions(query.Columns);
                if (hasConditions || query.RealTimeIPCodeParameter)
                {
                    sql.Append("\r\nWHERE\r\n    ");
                    if (query.RealTimeIPCodeParameter)
                    {
                        sql.Append("LW_LoyaltyMember.IPCode = " + queryMgr.GetParameterPrefix() + "ipcode");
                        if (hasConditions)
                        {
                            sql.Append(" AND\r\n    (\r\n");
                        }
                    }
                    if (hasConditions)
                    {
                        AppendConditions(query.Columns, sql);
                    }
                    if (hasConditions && query.RealTimeIPCodeParameter)
                    {
                        sql.Append(")");
                    }
                    sql.Append(" ");
                }

                if (query.Exclusions != null && query.Exclusions.Count > 0)
                {
                    if (!hasConditions)
                    {
                        sql.Append("\r\nWHERE NOT\r\n    ");
                    }
                    bool isFirst = true;
                    foreach (Join join in query.Exclusions)
                    {
                        if (!isFirst || hasConditions)
                        {
                            sql.Append("\r\nAND NOT ");
                        }

                        sql.Append(query.RootTableName);
                        sql.Append(".");
                        sql.Append(join.FieldName);
                        sql.Append(" IN (SELECT ");
                        sql.Append(join.FieldName);
                        sql.Append(" FROM ");
                        sql.Append(join.TableName);
                        sql.Append(") ");

                        isFirst = false;
                    }
                }

                List<string> groupByList = new List<string>();
                bool hasAggregatesInSelectList = false;
                bool hasAggregateColumns = HasAggregateColumns(query.Columns);

                for (int i = 0; i < query.SelectFieldList.Count; i++)
                {
                    bool isAggregate = false;
                    foreach (string aggregate in _aggregates)
                    {
                        if (query.SelectFieldList[i].ToLower().StartsWith(aggregate + "("))
                        {
                            isAggregate = true;
                            hasAggregatesInSelectList = true;
                            break;
                        }
                    }
                    if (!isAggregate)
                    {
                        //expression needs to be a field in order to go into the group by. String literals, numbers and 
                        //other functions that don't touch a row of data do not get grouped
                        if (!IsLiteralExpression(query.SelectFieldList[i]) && query.SelectFieldList[i].Contains("."))
                        {
                            groupByList.Add(query.SelectFieldList[i].ToLower());
                        }
                    }
                }


                if (hasAggregatesInSelectList || hasAggregateColumns)
                {
                    for (int i = 0; i < query.RootTableCarryover.Count; i++)
                    {
                        string groupField = (query.RootTableName + "." + query.RootTableCarryover[i]).ToLower();
                        if (!groupByList.Contains(groupField))
                        {
                            groupByList.Add(groupField);
                        }
                    }

                    if (query.StepType != StepType.Assignment)
                    {
                        foreach (var col in query.Columns.Where(o => o.ColumnType == ColumnType.Condition && string.IsNullOrEmpty(o.AggregateExpression)))
                        {
                            if (!string.IsNullOrEmpty(col.TableName) && !string.IsNullOrEmpty(col.FieldName))
                            {
                                //4.3 backward compatability
                                string groupField = col.TableName + "." + col.FieldName;
                                if (!groupByList.Contains(groupField, StringComparer.OrdinalIgnoreCase))
                                {
                                    groupByList.Add(groupField);
                                }
                            }
                            else if (
                                !string.IsNullOrEmpty(col.NonAliasedColumnExpression) &&
                                col.NonAliasedColumnExpression.Contains(".") &&
                                !groupByList.Contains(col.NonAliasedColumnExpression, StringComparer.OrdinalIgnoreCase)
                                )
                            {
                                //new 4.4 way
                                groupByList.Add(col.NonAliasedColumnExpression);
                            }
                        }
                    }

                    if (groupByList.Count > 0)
                    {
                        sql.Append("\r\nGROUP BY \r\n    ");
                        for (int i = 0; i < groupByList.Count; i++)
                        {
                            sql.Append(groupByList[i]);
                            if (i < groupByList.Count - 1)
                            {
                                sql.Append(", ");
                            }
                        }
                    }

                    if (HasNonCaseAggregateConditions(query.Columns))
                    {
                        sql.Append("\r\nHAVING\r\n    ");
                        ParseAggregates(query.Columns, sql);
                        sql.Append(" ");
                    }

                    if (query.OrderBy != null)
                    {
                        sql.Append("\r\nORDER BY \r\n    ");
                        sql.Append(string.Join(", ", query.OrderBy));
                    }
                }
            }


            if (query.Limit > -1)
            {
                if (query.RandomSample)
                {
                    sql.Append(queryMgr.GetOrderByRandom());
                }
                sql.Append(queryMgr.GetLimitQueryEnd(query));
            }

            ret.Add(sql.ToString());

            if (query.StepType == StepType.Assignment && !NeedsCaseStatements(query))
            {
                //assignment queries without a case statement need to also carry over any records that were excluded by the 
                //where clause of the query.
                SqlQuery q = new SqlQuery();
                q.Limit = query.Limit;
                q.ActionTable = query.ActionTable;
                //q.Columns = Query.Columns;
                q.Exclusions.Add(
                    new Join(
                        query.ActionTable,
                        query.SelectFieldList[0].Substring(query.SelectFieldList[0].LastIndexOf(".") + 1),
                        query.RootTableName,
                        query.SelectFieldList[0].Substring(query.SelectFieldList[0].LastIndexOf(".") + 1),
                        TableJoinType.Left));
                q.InsertFieldList = query.InsertFieldList;
                q.StepType = StepType.Select;
                q.RootTableCarryover = query.RootTableCarryover;
                q.RootTableName = query.RootTableName;
                q.SelectFieldList = query.SelectFieldList;
                //add null for the assignment column value
                q.SelectFieldList.Add("null");
                List<string> parsedQuery = CreateSqlStatement(q);
                ret.Add(parsedQuery[0]);
            }

            return ret;
        }

        public bool IsLiteralExpression(string expression)
        {
            if (expression.Contains("'"))
            {
                //expression has quoted text ("'abc'" or "'abc' or 'xyz'"). if the entire expression is quoted, return true, else false
                return Regex.IsMatch(expression.Replace("''", string.Empty), "^'{1}[^']{0,}'{1}$");
            }
            if (!expression.Contains(" "))
            {
                //expression does not contain quoted text, but has spaces - could be "1 or 2 or 3" or "in(1, 2, 3)"
                return false;
            }
            else
            {
                //no quoted text and no spaces ("1", "5" etc.)
                return true;
            }
        }

        public string EnsureQuotes(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "''";
            }
            if (value.StartsWith("'"))
            {
                value = value.Substring(1);
            }
            if (value.EndsWith("'"))
            {
                value = value.Substring(0, value.Length - 1);
            }
            return "'" + value.Replace("'", "''") + "'";
        }

        private List<IDbDataParameter> GetDBParameters(Dictionary<string, object> parameters, Dictionary<string, object> overrideParameters)
        {
            var combined = new Dictionary<string, object>();
            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    combined.Add(key, parameters[key]);
                }
            }
            if (overrideParameters != null)
            {
                foreach (var key in overrideParameters.Keys)
                {
                    if (combined.ContainsKey(key))
                    {
                        combined[key] = overrideParameters[key];
                    }
                    else
                    {
                        combined.Add(key, overrideParameters[key]);
                    }
                }
            }

            var ret = new List<IDbDataParameter>();

            if (combined.Count > 0)
            {
                //ret = AdoTemplate.CreateDbParameters();
                foreach (var key in combined.Keys)
                {
                    var parameter = Factory.CreateParameter(); // AdoTemplate.DbProvider.CreateParameter();
                    parameter.ParameterName = key;
                    parameter.Value = combined[key];
                    ret.Add(parameter);
                }
            }
            return ret;
        }

        private void CheckForSqlInjection(string sql)
        {
            const string errorMessage = "The SQL statement cannot be executed because it has failed a check for malicious code:\r\n\r\n{0}\r\n\r\nPlease remove any potentially dangerous SQL statements from the query and try again.";
            string regex = "(alter|drop{1,})([ \\r\\n\\t]{1,})(queue|remote|role|route|schema|service|table|trigger|user|view|xml|assembly|certificate|database|endpoint|function|index|login|message|procedure{1})";
            Match match = Regex.Match(sql, regex, RegexOptions.IgnoreCase);
            if (match.Captures.Count > 0)
            {
                throw new Exception(string.Format(errorMessage, GetOffendingText(sql, match.Index)));
            }
        }

        private string GetOffendingText(string sql, int index)
        {
            int start = 0;
            int length = 50;
            if (index > 25)
            {
                start = index - 15;
            }

            if (start + 50 > sql.Length)
            {
                length = sql.Length - start;
            }
            return sql.Substring(start, length);
        }

        private bool HasWhereConditions(List<QueryColumn> columns)
        {
            //get all rows that are part of a case statement and exclude them (is any) 
            List<int> caseRows = GetCaseRows(columns);

            if (columns != null && columns.Count > 0)
            {
                foreach (QueryColumn column in columns)
                {
                    if (string.IsNullOrEmpty(column.AggregateExpression) && column.Conditions != null && column.Conditions.Count > 0)
                    {
                        foreach (ColumnCondition condition in column.Conditions)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression) && !caseRows.Contains(condition.RowOrder))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool HasAggregateColumns(List<QueryColumn> columns)
        {
            if (columns != null && columns.Count > 0)
            {
                foreach (QueryColumn column in columns)
                {
                    if (!string.IsNullOrEmpty(column.AggregateExpression))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HasAggregateConditions(List<QueryColumn> columns)
        {
            if (columns != null && columns.Count > 0)
            {
                foreach (QueryColumn column in columns)
                {
                    if (!string.IsNullOrEmpty(column.AggregateExpression) && column.Conditions != null && column.Conditions.Count > 0)
                    {
                        foreach (ColumnCondition condition in column.Conditions)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool HasNonCaseAggregateConditions(List<QueryColumn> columns)
        {
            List<int> caseRows = GetCaseRows(columns);
            if (columns != null && columns.Count > 0)
            {
                foreach (QueryColumn column in columns)
                {
                    if (!string.IsNullOrEmpty(column.AggregateExpression) && column.Conditions != null && column.Conditions.Count > 0)
                    {
                        foreach (ColumnCondition condition in column.Conditions)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression) && !caseRows.Contains(condition.RowOrder))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// returns a list of all rows to be used in building case statements
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        private List<int> GetCaseRows(List<QueryColumn> columns)
        {
            //new: only rows where a "set to" is used will be part of the case statement
            List<int> caseList = new List<int>();
            if (columns != null)
            {
                foreach (QueryColumn column in columns)
                {
                    if (column.ColumnType == ColumnType.Append && column.Conditions != null && column.Conditions.Count > 0)
                    {
                        foreach (ColumnCondition condition in column.Conditions)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression) && !caseList.Contains(condition.RowOrder))
                            {
                                caseList.Add(condition.RowOrder);
                            }
                        }
                    }
                }
            }
            caseList.Sort();
            return caseList;
        }

        private List<int> GetConditionRows(List<QueryColumn> columns)
        {
            List<int> conditionList = new List<int>();
            List<int> caseList = GetCaseRows(columns);

            if (columns != null)
            {
                foreach (QueryColumn column in columns)
                {
                    if (column.ColumnType == ColumnType.Condition && column.Conditions != null && column.Conditions.Count > 0)
                    {
                        foreach (ColumnCondition condition in column.Conditions)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression) && !conditionList.Contains(condition.RowOrder) && !caseList.Contains(condition.RowOrder))
                            {
                                conditionList.Add(condition.RowOrder);
                            }
                        }
                    }
                }
            }
            conditionList.Sort();
            return conditionList;
        }

        private void AppendConditions(List<QueryColumn> columns, StringBuilder sql)
        {
            List<int> rowList = GetConditionRows(columns);

            //build where clause row by row
            int rowCount = 0;

            foreach (int rowNumber in rowList)
            {
                int conditionCount = 0;
                string conditionRow = string.Empty;

                foreach (QueryColumn column in columns)
                {
                    if (column.ColumnType == ColumnType.Condition && column.Conditions != null && column.Conditions.Count > 0 && string.IsNullOrEmpty(column.AggregateExpression))
                    {
                        foreach (ColumnCondition condition in column.Conditions)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression) && condition.RowOrder == rowNumber)
                            {
                                //add the condition
                                if (conditionCount > 0)
                                {
                                    conditionRow += "AND\r\n    ";
                                }
                                //conditionRow += GetConditionExpression(column.TableName, column.FieldName, condition, column.IsParameter);
                                conditionRow += GetConditionExpression(column, condition);
                                conditionRow += " ";
                                conditionCount++;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(conditionRow))
                {
                    if (rowCount > 0)
                    {
                        sql.Append(" OR\r\n    ");
                    }
                    sql.Append("(");
                    sql.Append(conditionRow);
                    sql.Append(")");
                }
                rowCount++;
            }
        }

        private void ParseAggregates(List<QueryColumn> columns, StringBuilder sql)
        {
            List<int> rowList = GetConditionRows(columns);

            //build having clause row by row
            int rowCount = 0;

            foreach (int rowNumber in rowList)
            {
                int conditionCount = 0;
                string conditionRow = string.Empty;

                foreach (QueryColumn column in columns)
                {
                    if (column.ColumnType == ColumnType.Condition && !string.IsNullOrEmpty(column.AggregateExpression) && column.Conditions != null && column.Conditions.Count > 0)
                    {
                        foreach (ColumnCondition condition in column.Conditions)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression) && condition.RowOrder == rowNumber)
                            {
                                //add the condition
                                if (conditionCount > 0)
                                {
                                    conditionRow += "AND\r\n    ";
                                }
                                //conditionRow += GetConditionExpression(column.TableName, column.FieldName, condition, column.AggregateExpression, column.IsParameter);
                                conditionRow += GetConditionExpression(column, condition);
                                conditionRow += " ";
                                conditionCount++;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(conditionRow))
                {
                    if (rowCount > 0)
                    {
                        sql.Append(" OR\r\n    ");
                    }
                    sql.Append("(");
                    sql.Append(conditionRow);
                    sql.Append(")");
                    rowCount++;
                }
            }
        }

        private string GetConditionExpression(QueryColumn column, ColumnCondition condition)
        {
            string expression = string.Empty;
            if (string.IsNullOrEmpty(column.NonAliasedColumnExpression))
            {
                expression = (string.IsNullOrEmpty(column.TableName) ? string.Empty : column.TableName + ".") + column.FieldName;
            }
            else
            {
                expression = column.NonAliasedColumnExpression;
            }

            if (column.IsParameter)
            {
                expression = _queryMgrMap[DatabaseType].GetParameterPrefix() + expression;
            }
            if (!string.IsNullOrEmpty(column.AggregateExpression))
            {
                expression = column.AggregateExpression + "(" + expression + ")";
            }

            if (condition.DatePart.HasValue)
            {
                //condition is a date field. Attempt to parse date values

                string[] dates = condition.NonAliasedConditionExpression.Split('|');
                if (dates.Length > 0)
                {
                    DateTime startDate = DateTime.MinValue;
                    DateTime endDate = DateTime.MinValue;
                    if (DateTime.TryParse(dates[0], out startDate))
                    {
                        if (dates.Length > 1 && DateTime.TryParse(dates[1], out endDate))
                        {
                            return _queryMgrMap[DatabaseType].GetDateFormula(expression, startDate, endDate, condition.DatePart.Value, condition.Operator);
                        }
                        return _queryMgrMap[DatabaseType].GetDateFormula(expression, startDate, null, condition.DatePart.Value, condition.Operator);
                    }
                }
            }

            if (condition.FieldFunction != 0)
            {
                expression = _queryMgrMap[DatabaseType].GetFieldFunction(expression, condition.FieldFunction);
            }

            if (condition.Operator != null && condition.Operator == "in")
            {
                condition.Operator = null;
                if (condition.NonAliasedConditionExpression.Trim().StartsWith("(") && condition.NonAliasedConditionExpression.Trim().EndsWith(")"))
                {
                    condition.NonAliasedConditionExpression = "in " + condition.NonAliasedConditionExpression + string.Empty;
                }
                else
                {
                    condition.NonAliasedConditionExpression = "in (" + condition.NonAliasedConditionExpression + ")";
                }
            }

            string ret = string.Empty;
            bool holdingOp = false;
            List<string> splitExpressions = new List<string>();
            if (!string.IsNullOrEmpty(condition.Operator))
            {
                splitExpressions.Add(condition.Operator);
            }
            splitExpressions.AddRange(GetExpressionList(condition.NonAliasedConditionExpression));

            foreach (string exp in splitExpressions)
            {
                if (exp.ToLower() == "not")
                {
                    ret += "not ";
                    continue;
                }

                bool added = false;
                foreach (string q in _qualifiers)
                {
                    if (exp == q)
                    {
                        ret += " " + q + " ";
                        added = true;
                    }
                }
                if (added)
                {
                    continue;
                }

                foreach (string noOperator in _noOperator)
                {
                    if (exp.ToLower().StartsWith(noOperator))
                    {
                        ret += expression + " " + exp + " ";
                        added = true;
                    }
                }
                if (added)
                {
                    continue;
                }

                foreach (string o in _operators)
                {
                    if (exp == o)
                    {
                        ret += expression + " " + o + " ";
                        added = true;
                        holdingOp = true;
                    }
                }
                if (added)
                {
                    continue;
                }

                if (!holdingOp)
                {
                    ret += expression + " ";
                    if (string.IsNullOrEmpty(condition.Operator))
                    {
                        ret += "= ";
                    }
                    else
                    {
                        ret += condition.Operator + " ";
                    }
                }

                ret += exp;
                holdingOp = false;
            }
            return ret;
        }

        private List<string> GetExpressionList(string expression)
        {
            List<string> expressions = new List<string>();

            while (!string.IsNullOrEmpty(expression))
            {
                expression = expression.Trim();
                string lower = expression.ToLower();

                if (lower.StartsWith("'"))
                {
                    int literalEnd = -1;
                    for (int i = 1; i < expression.Length; i++)
                    {
                        if (expression.Substring(i, 1) == "'")
                        {
                            if (i < expression.Length - 1)
                            {
                                if (expression.Substring(i + 1, 1) == "'")
                                {
                                    i++;
                                    continue;
                                }
                                else
                                {
                                    literalEnd = i;
                                    break;
                                }
                            }
                            else
                            {
                                literalEnd = i;
                                break;
                            }
                        }
                    }
                    if (literalEnd < 0)
                    {
                        throw new Exception("could not parse the end of the literal string starting at: " + expression);
                    }
                    else
                    {
                        expressions.Add(expression.Substring(0, literalEnd + 1));
                        expression = expression.Substring(literalEnd + 1);
                    }
                }
                else if (lower.StartsWith("in(") || lower.StartsWith("in ("))
                {
                    int inEnd = -1;
                    for (int i = 1; i < expression.Length; i++)
                    {
                        if (expression.Substring(i, 1) == ")")
                        {
                            inEnd = i;
                            break;
                        }
                    }
                    if (inEnd < 0)
                    {
                        throw new Exception("could not parse the end of the \"in()\" statement at: " + expression);
                    }
                    else
                    {
                        expressions.Add(expression.Substring(0, inEnd + 1));
                        expression = expression.Substring(inEnd + 1);
                    }
                }
                else
                {
                    bool added = false;
                    foreach (string s in _operators)
                    {
                        if (lower.StartsWith(s))
                        {
                            expressions.Add(s);
                            expression = expression.Substring(s.Length);
                            lower = expression.ToLower();
                            added = true;
                        }
                    }
                    if (added)
                    {
                        continue;
                    }

                    foreach (string s in _qualifiers)
                    {
                        if (lower.StartsWith(s))
                        {
                            expressions.Add(s);
                            expression = expression.Substring(s.Length);
                            added = true;
                        }
                    }
                    if (added)
                    {
                        continue;
                    }

                    foreach (string s in _noOperator)
                    {
                        if (lower.StartsWith(s))
                        {
                            expressions.Add(s);
                            expression = expression.Substring(s.Length);
                            added = true;
                        }
                    }
                    if (added)
                    {
                        continue;
                    }

                    if (lower.Contains(" "))
                    {
                        string unknownValue = expression.Substring(0, expression.IndexOf(" "));
                        expressions.Add(unknownValue);
                        expression = expression.Substring(unknownValue.Length);
                        continue;
                    }
                    else
                    {
                        //that's about all we can do
                        expressions.Add(expression);
                        expression = string.Empty;
                        break;
                    }
                }
            }
            return expressions;
        }

        private void BuildCaseStatement(SqlQuery query, QueryColumn column, List<string> fields)
        {
            List<int> rowList = GetCaseRows(query.Columns);
            List<string> setList = new List<string>();
            int catchAllRowNumber = -1;
            string setValue = string.Empty;
            //string setField = "";
            StringBuilder sql = new StringBuilder();

            if (rowList.Count < 1)
            {
                //this should be a single catch-all value
                fields.Add(/*EnsureQuotes(*/column.NonAliasedColumnExpression/*)*/ + " AS " + column.FieldName);
                return;
            }

            if (rowList.Count == 1 && IsCatchAllRow(rowList[0], query))
            {
                var condition = column.Conditions.Where(o => o.RowOrder == rowList[0]).FirstOrDefault();
                fields.Add(/*EnsureQuotes(*/GetAssignmentConditionExpression(query, condition)/*)*/ + " AS " + column.FieldName);
                return;
            }

            string catchAllRow = string.Empty;
            int rowCount = 0;

            sql.Append("CASE WHEN ");
            int caseCount = 0;

            foreach (int rowNumber in rowList)
            {
                if (IsCatchAllRow(rowNumber, query))
                {
                    catchAllRowNumber = rowNumber;
                    foreach (QueryColumn c in query.Columns)
                    {
                        if (c.ColumnType == ColumnType.Append)
                        {
                            foreach (ColumnCondition condition in column.Conditions)
                            {
                                if (condition.RowOrder == rowNumber)
                                {
                                    catchAllRow = /*EnsureQuotes(*/GetAssignmentConditionExpression(query, condition)/*)*/;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    continue;
                }

                string rowExpression = string.Empty;

                foreach (QueryColumn c in query.Columns)
                {
                    if (c.ColumnType == ColumnType.Condition && c.Conditions != null && c.Conditions.Count > 0)
                    {
                        foreach (ColumnCondition condition in c.Conditions)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression) && condition.RowOrder == rowNumber)
                            {
                                //add the condition
                                if (!string.IsNullOrEmpty(rowExpression))
                                {
                                    rowExpression += " AND ";
                                }
                                //rowExpression += GetConditionExpression(column.TableName, column.FieldName, condition, column.AggregateExpression, column.IsParameter);
                                rowExpression += GetConditionExpression(c, condition);
                            }
                        }
                    }
                    else if ((c.ColumnType == ColumnType.Append || c.ColumnType == ColumnType.Segment) && c.FieldName == column.FieldName)
                    {
                        foreach (ColumnCondition condition in column.Conditions)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression) && condition.RowOrder == rowNumber)
                            {
                                setValue = /*EnsureQuotes(*/GetAssignmentConditionExpression(query, condition)/*)*/;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(rowExpression))
                {
                    if (caseCount > 0)
                    {
                        sql.Append("\r\n\twhen ");
                    }
                    sql.Append(rowExpression);
                    sql.Append(" THEN ");
                    if (!string.IsNullOrEmpty(setValue))
                    {
                        sql.Append(setValue);
                    }
                    else
                    {
                        sql.Append(/*EnsureQuotes(*/column.NonAliasedColumnExpression/*)*/);
                    }
                    caseCount++;
                }
                rowCount++;
            }

            if (catchAllRowNumber > -1)
            {
                sql.Append("\r\n\tELSE ");
                sql.Append(catchAllRow);
            }
            else if (query.StepType == StepType.Assignment)
            {
                //this is not necessary for either Oracle or SQL server.
                //Sql.Append(" ELSE '' ");
            }
            sql.Append(" END AS \"");
            sql.Append(column.FieldName);
            sql.Append("\" ");
            fields.Add(sql.ToString());
        }

        private bool IsCatchAllRow(int rowId, SqlQuery query)
        {
            foreach (QueryColumn column in query.Columns)
            {
                if (column.ColumnType == ColumnType.Condition)
                {
                    foreach (ColumnCondition condition in column.Conditions)
                    {
                        if (condition.RowOrder == rowId)
                        {
                            if (!string.IsNullOrEmpty(condition.NonAliasedConditionExpression))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private string AddWhereClause(string query, string clause)
        {
            int whereIndex = query.ToLower().LastIndexOf("where");
            if (whereIndex < 1)
            {
                query += " WHERE " + clause;
            }
            else
            {
                query += " AND " + clause;
            }
            return query;
        }

        private string GetAssignmentConditionExpression(SqlQuery query, ColumnCondition condition)
        {
            if (!string.IsNullOrWhiteSpace(condition.ConditionExpression))
            {
                //this should only happen in 4.4 or later. In 4.3, field assignments were dont using 
                string aggregate = string.Empty;
                if (HasAggregateColumns(query.Columns))
                {
                    foreach (QueryColumn column in query.Columns)
                    {
                        //4.3 compatability
                        if (!string.IsNullOrEmpty(column.AggregateExpression) && column.ColumnExpression.Equals(condition.ConditionExpression, StringComparison.OrdinalIgnoreCase))
                        {
                            aggregate = column.AggregateExpression;
                            break;
                        }
                    }
                    string expression = condition.ConditionExpression;
                    if (!string.IsNullOrEmpty(aggregate))
                    {
                        expression = string.Format("{0}({1})", aggregate, expression);
                    }
                    return _queryMgrMap[DatabaseType].GetAssignmentCharacterConversion(expression);
                }
            }

            //begin 4.3 backward compatability:
            if (condition.AssignmentTableId.GetValueOrDefault(0) > 0 && !string.IsNullOrEmpty(condition.AssignmentFieldName) && !string.IsNullOrEmpty(condition.AssignmentTableName))
            {
                string fullExpression = condition.AssignmentTableName + "." + condition.AssignmentFieldName;
                string aggregate = string.Empty;
                if (HasAggregateColumns(query.Columns))
                {
                    foreach (QueryColumn column in query.Columns)
                    {
                        if (column.TableId == condition.AssignmentTableId && column.FieldName == condition.AssignmentFieldName && !string.IsNullOrEmpty(column.AggregateExpression))
                        {
                            aggregate = column.AggregateExpression;
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(aggregate))
                {
                    fullExpression = aggregate + "(" + fullExpression + ")";
                }
                return _queryMgrMap[DatabaseType].GetAssignmentCharacterConversion(fullExpression);
            }
            //end 4.3 backward compatability:

            return condition.NonAliasedConditionExpression;
        }

        private bool NeedsCaseStatements(SqlQuery query)
        {
            //only assignment queries build case statements
            if (query.StepType != StepType.Assignment && query.StepType != StepType.Output)
            {
                return false;
            }

            if (query.StepType == StepType.Assignment || query.StepType == StepType.Output /*real-time assignment*/)
            {
                if (!HasAggregateColumns(query.Columns) && !HasAggregateConditions(query.Columns))
                {
                    return true;
                }

                int assignmentCount = 0;
                foreach (QueryColumn col in query.Columns)
                {
                    if (col.ColumnType == ColumnType.Append)
                    {
                        assignmentCount++;
                        if (assignmentCount > 1)
                        {
                            return true;
                        }
                        string expression = string.Empty;
                        foreach (ColumnCondition condition in col.Conditions)
                        {
                            if (string.IsNullOrEmpty(expression))
                            {
                                expression = condition.NonAliasedConditionExpression;
                            }

                            if (condition.NonAliasedConditionExpression != expression)
                            {
                                //column has non-matching conditions. Case statement is needed
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private int ExecuteNonQuery(string cmdText, List<IDbDataParameter> parameters = null)
        {
            var database = new PetaPoco.Database(ConnectionString, Factory);
            database.OpenSharedConnection();
            var util = new Brierley.FrameWork.Data.Sql.LWQueryUtil(database);
            return util.ExecuteSQLNonQuery(cmdText, parameters ?? new List<IDbDataParameter>());
        }

        private object ExecuteScalar(string cmdText, List<IDbDataParameter> parameters = null)
        {
            var database = new PetaPoco.Database(ConnectionString, Factory);
            database.OpenSharedConnection();
            var util = new Brierley.FrameWork.Data.Sql.LWQueryUtil(database);
            return util.ExecuteSQLScalar(cmdText, parameters ?? new List<IDbDataParameter>());
        }

        private int DataTableFill(DataTable table, string cmdText, List<IDbDataParameter> parameters = null)
        {
            var database = new PetaPoco.Database(ConnectionString, Factory);
            database.OpenSharedConnection();
            var util = new Brierley.FrameWork.Data.Sql.LWQueryUtil(database);
            util.DataTableFill(cmdText, parameters ?? new List<IDbDataParameter>(), ref table);
            return table.Rows.Count;
        }
    }
}
