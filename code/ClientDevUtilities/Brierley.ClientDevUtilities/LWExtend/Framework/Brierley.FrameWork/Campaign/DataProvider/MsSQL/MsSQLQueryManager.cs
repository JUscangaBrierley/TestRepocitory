//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using System.Data;

namespace Brierley.FrameWork.CampaignManagement.DataProvider.MsSQL
{
    public class MsSQLQueryManager : ICMQueryManager
    {
		private IBulkOutputProvider _bulkOutputProvider = null;

		public IBulkOutputProvider BulkOutputProvider
		{
			get
			{
				if (_bulkOutputProvider == null)
				{
					_bulkOutputProvider = new MSSqlBulkOutput();
				}
				return _bulkOutputProvider;
			}
		}

        #region Dictionary Methods

		public string GetParameterPrefix()
		{
			return "@";
		}

		public string GetTableExistsQuery(string SchemaName, string TableName)
		{
			return string.Format("SELECT name AS TableName FROM {0}sysobjects WHERE xtype in('u', 'v') AND name = '{1}'", EnsureSchemaName(SchemaName, true, true), TableName);
		}


        public string GetTableNameQuery(string SchemaName)
        {
			return string.Format("select name as TableName, case when xtype = 'u' then 0 else 1 end as IsView from {0}sysobjects where xtype in('u', 'v') ORDER BY name", EnsureSchemaName(SchemaName, true, true));
        }


        public string GetTableDetailQuery(string SchemaName, string TableName)
        {
			SchemaName = EnsureSchemaName(SchemaName, true, true);
			return string.Format(@"SELECT syscolumns.name as FieldName, systypes.name as DataType, syscolumns.length as Length, syscolumns.xprec as Precision, syscolumns.xscale as Scale
									FROM {0}syscolumns
									inner join {1}sysobjects on sysobjects.id = syscolumns.id
									inner join {2}systypes on systypes.xusertype = syscolumns.xtype
									WHERE sysobjects.xtype in('u','v') and sysobjects.name='{3}'", SchemaName, SchemaName, SchemaName, TableName);
        }


		public SqlStatement GetValidateSqlQuery(SqlStatement Sql)
		{
			//don't exec
			Sql.Statement = string.Format("SET NOEXEC ON\r\n{0}\r\nSET NOEXEC OFF", Sql.Statement);
			return Sql;

			//don't exec, but return metadata
			//return "SET FMTONLY ON\r\n" + Sql + "\r\nSET FMTONLY OFF";
		}


		public string GetDropTableQuery(string TableName)
		{
			return "DROP TABLE " + TableName;
		}


		public string GetRowCountEstimateQuery(string SchemaName, string TableName)
		{
			return "SEELCT COUNT(*) AS RowCount FROM " + EnsureSchemaName(SchemaName, true, true) + TableName;
		}


		public string GetFieldTypeQuery(string SchemaName, string TableName, string FieldName)
		{
			SchemaName = EnsureSchemaName(SchemaName, true, true);
			return string.Format(@"SELECT systypes.name as DataType, syscolumns.length as Length, syscolumns.xprec as Precision, syscolumns.xscale as Scale
									FROM {0}syscolumns
									inner join {1}sysobjects on sysobjects.id = syscolumns.id
									inner join {2}systypes on systypes.xusertype = syscolumns.xtype
									WHERE sysobjects.xtype in('u','v') and sysobjects.name='{3}'
										AND syscolumns.name = '{4}'", SchemaName, SchemaName, SchemaName, TableName, FieldName);
		}


		public string AppendFieldQuery(string TableName, string FieldName, string DataType)
		{
			return string.Format("ALTER TABLE {0} ADD {1} {2}", TableName, FieldName, (string.IsNullOrEmpty(DataType) ? "varchar(250)" : DataType));
		}


		public string DropFieldQuery(string TableName, string FieldName)
		{
			return string.Format("ALTER TABLE {0} DROP COLUMN {1}", TableName, FieldName);
		}


		public string GetDistinctValuesQuery(string schemaName, string tableName, string fieldName)
		{
			return string.Format("SELECT {0} AS \"Value\", count(*) AS \"ValueCount\" FROM {1} WHERE {0} IS NOT NULL GROUP BY {0}", fieldName, EnsureSchemaName(schemaName, true, true) + tableName);
		}


		public string ParseSqlStatement(SqlQuery Query)
		{
			throw new Exception("not implemented");
		}


		public string GetMergeKeyword(MergeType MergeType)
		{
			string sql = null;
			switch (MergeType)
			{
				case MergeType.Top:
					sql = "UNION ALL";
					break;
				case MergeType.Union:
					sql = "UNION";
					break;
				case MergeType.UnionAll:
					sql = "UNION ALL";
					break;
				case MergeType.ExceptOrMinus:
					sql = "EXCEPT";
					break;
				case MergeType.Intersect:
					sql = "INTERSECT";
					break;
			}
			return sql;			
		}


		public string GetLimitQueryStart(SqlQuery Query)
		{
			return string.Format("SELECT {0}TOP {1}{2}", (Query.DistinctRows ? "DISTINCT " : string.Empty), Query.Limit.ToString(), Query.IsLimitPercentage ? " PERCENT" : "");
		}

		public string GetLimitQueryEnd(SqlQuery Query)
		{
			return string.Empty;
		}

		public string GetOrderByRandom()
		{
			return "ORDER BY newid()";
		}



		public string GetCreateTableClause(string TableName)
		{
			return string.Empty;
		}

		public string GetInsertIntoClause(string TableName)
		{
			return "INTO " + TableName;
		}

		public string GetAssignmentCharacterConversion(string field)
		{
			return field;
		}

		public string GetCreateHistoryTableStatement()
		{
			return "CREATE TABLE " + Constants.CampaignHistoryTableName + " (AudienceKey nvarchar(500), CampaignId bigint, StepId bigint, OutputType varchar(50), OutputFile nvarchar(500), OutputId bigint, OutputDate datetime default getdate())";
		}

        #endregion


		private string EnsureSchemaName(string SchemaName, bool FormatAsPrefix, bool AppendDbo)
		{
			if (string.IsNullOrEmpty(SchemaName))
			{
				return SchemaName;
			}
			SchemaName = SchemaName.Trim();
			if (FormatAsPrefix && !SchemaName.EndsWith("."))
			{
				SchemaName += ".";
			}
			if (AppendDbo)
			{
				SchemaName += "dbo.";
			}
			return SchemaName;
		}


		public string GetDateFormula(string fieldName, DateTime startDate, DateTime? endDate, DateParts datePart, string dateOperator)
		{
			string start = string.Empty;
			string end = string.Empty;
			string field = string.Empty;

			switch (datePart)
			{
				case DateParts.Complete:
					start = "'" + startDate.ToString("MM/dd/yyyy") + "'";
					if (endDate.HasValue)
					{
						end = "'" + endDate.Value.ToString("MM/dd/yyyy") + "'";
					}
					field = fieldName;
					break;
				case DateParts.Day:
					start = startDate.Day.ToString();
					if (endDate.HasValue)
					{
						end = endDate.Value.Day.ToString();
					}
					field = "DatePart(d, " + fieldName + ")";
					break;
				case DateParts.Month:
					start = startDate.Month.ToString();
					if (endDate.HasValue)
					{
						end = endDate.Value.Month.ToString();
					}
					field = "DatePart(m, " + fieldName + ")";
					break;
				case DateParts.Year:
					start = startDate.Year.ToString();
					if (endDate.HasValue)
					{
						end = endDate.Value.Year.ToString();
					}
					field = "DatePart(yyyy, " + fieldName + ")";
					break;
			}


			if (dateOperator.ToLower() == "bet")
			{
				if (!endDate.HasValue)
				{
					throw new ArgumentNullException("endDate");
				}
				return string.Format("{0} between {1} and {2}", field, start, end);
			}
			else
			{
				return string.Format("{0} {1} {2}", field, dateOperator, start);
			}

		}

		public string GetFieldFunction(string fieldName, StringFunctions functions)
		{
			string ret = fieldName;
			if ((functions & StringFunctions.Trim) == StringFunctions.Trim)
			{
				ret = "ltrim(rtrim(" + ret + "))";
			}
			if ((functions & StringFunctions.Lower) == StringFunctions.Lower)
			{
				ret = "lower(" + ret + ")";
			}
			if ((functions & StringFunctions.Upper) == StringFunctions.Upper)
			{
				ret = "upper(" + ret + ")";
			}
			if ((functions & StringFunctions.Length) == StringFunctions.Length)
			{
				ret = "len(" + ret + ")";
			}
			return ret;
		}

		public string GetDataSchemaPrefix(string schemaName)
		{
			return EnsureSchemaName(schemaName, true, true);
		}

    }
}
