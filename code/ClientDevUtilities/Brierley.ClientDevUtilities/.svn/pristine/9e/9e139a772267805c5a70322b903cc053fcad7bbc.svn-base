using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.CampaignManagement.DataProvider
{
    public interface ICMQueryManager
    {
		IBulkOutputProvider BulkOutputProvider { get; }

		string GetTableNameQuery(string SchemaName);

		string GetTableDetailQuery(string SchemaName, string tableName);        

		string GetTableExistsQuery(string SchemaName, string TableName);

		SqlStatement GetValidateSqlQuery(SqlStatement Sql);

		string GetDropTableQuery(string TableName);

		string GetRowCountEstimateQuery(string SchemaName, string TableName);

		string GetFieldTypeQuery(string SchemaName, string TableName, string FieldName);

		string AppendFieldQuery(string TableName, string FieldName, string DataType);

		string DropFieldQuery(string TableName, string FieldName);

		string GetMergeKeyword(MergeType MergeType);

		string GetLimitQueryStart(SqlQuery Query);

		string GetLimitQueryEnd(SqlQuery Query);

		string GetOrderByRandom();

		string GetCreateTableClause(string TableName);

		string GetInsertIntoClause(string TableName);

		string ParseSqlStatement(SqlQuery Query);

		string GetDistinctValuesQuery(string schemaName, string tableName, string fieldName);

		string GetAssignmentCharacterConversion(string assignmentField);

		string GetDateFormula(string fieldName, DateTime startDate, DateTime? endDate, DateParts datePart, string dateOperator);

		string GetFieldFunction(string fieldName, StringFunctions functions);

		string GetDataSchemaPrefix(string schemaName);

		string GetCreateHistoryTableStatement();

		string GetParameterPrefix();
    }
}
