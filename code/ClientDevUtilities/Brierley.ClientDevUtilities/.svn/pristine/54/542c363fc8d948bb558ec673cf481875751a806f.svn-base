//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace Brierley.FrameWork.CampaignManagement.DataProvider.Oracle
{
	public class OracleQueryManager : ICMQueryManager
    {
		private IBulkOutputProvider _bulkOutputProvider = null;

		public IBulkOutputProvider BulkOutputProvider
		{
			get
			{
				if (_bulkOutputProvider == null)
				{
					_bulkOutputProvider = new OracleBulkOutput();
				}
				return _bulkOutputProvider;
			}
		}

        #region Dictionary Methods

		public string GetParameterPrefix()
		{
			return ":";
		}

        public string GetTableNameQuery(string SchemaName)
        {
			if (string.IsNullOrEmpty(SchemaName))
			{
				return "select * from (select table_name as TableName, 0 as IsView from user_tables union select view_name as TableName, 1 as IsView from user_views) order by tablename";
			}
			else
			{
				return string.Format("select object_name as TableName, case when object_type = 'TABLE' then 0 else 1 end as IsView from all_objects where owner = '{0}' and object_type in('TABLE', 'VIEW') order by object_name", EnsureSchemaName(SchemaName));
			}
        }

		public string GetTableExistsQuery(string SchemaName, string TableName)
		{
			if (string.IsNullOrEmpty(SchemaName))
			{
				return string.Format("select * from (select table_name as TableName from user_tables union select view_name as TableName from user_views) where TableName = '{0}'", TableName.ToUpper());
			}
			else
			{
				return string.Format("select object_name as TableName from all_objects where owner = '{0}' and object_type in('TABLE', 'VIEW') and object_name = '{1}'", EnsureSchemaName(SchemaName), TableName.ToUpper());
			}
		}

        public string GetTableDetailQuery(string SchemaName, string TableName)
        {
			if (string.IsNullOrEmpty(SchemaName))
			{
				return string.Format("SELECT column_name as FieldName, data_type as DataType, char_length as Length, data_precision as Precision, data_scale as Scale FROM user_tab_cols WHERE table_name = '{0}' and column_id is not null order by internal_column_id", TableName.ToUpper());
			}
			else
			{
				return string.Format("SELECT column_name as FieldName, data_type as DataType, char_length as Length, data_precision as Precision, data_scale as Scale FROM all_tab_cols WHERE owner = '{0}' and table_name = '{1}' and column_id is not null order by internal_column_id", EnsureSchemaName(SchemaName), TableName.ToUpper());
			}
        }

		public SqlStatement GetValidateSqlQuery(SqlStatement Sql)
		{
			//explain plan does not work when run against views where the user does not have select rights to the underlying tables. 
			//Instead, a test parameter has been added to Query to indicate the query should just be validated, and a "where 1=2" will
			//be appended to the queries.
			//return "EXPLAIN PLAN FOR " + Sql;
			return Sql;
		}

		public string GetDropTableQuery(string TableName)
		{
			return "DROP TABLE " + TableName;
		}

		public string GetRowCountEstimateQuery(string SchemaName, string TableName)
		{
			if(string.IsNullOrEmpty(SchemaName))
			{
				return "SELECT NUM_ROWS FROM user_tables WHERE TABLE_NAME = '" + TableName.ToUpper() + "'";
			}
			else
			{
				return string.Format("SELECT NUM_ROWS FROM all_tables WHERE owner = '{0}' and TABLE_NAME = {1}", EnsureSchemaName(SchemaName), TableName.ToUpper());
			}
		}

		public string GetFieldTypeQuery(string SchemaName, string TableName, string FieldName)
		{
			if (string.IsNullOrEmpty(SchemaName))
			{
				return string.Format("SELECT data_type AS DataType, char_length AS Length, data_precision as Precision, data_scale as Scale FROM user_tab_columns WHERE table_name = '{0}' AND column_name = '{1}'", TableName.ToUpper(), FieldName.ToUpper());
			}
			else
			{
				return string.Format("SELECT data_type AS DataType, char_length AS Length, data_precision as Precision, data_scale as Scale FROM all_tab_columns WHERE owner = '{0}' and table_name = '{1}' AND column_name = '{2}'", EnsureSchemaName(SchemaName), TableName.ToUpper(), FieldName.ToUpper());
			}
		}

		public string AppendFieldQuery(string TableName, string FieldName, string DataType)
		{
			return string.Format("ALTER TABLE {0} ADD {1} {2}", TableName, FieldName, (string.IsNullOrEmpty(DataType) ? "varchar2(250)" : DataType));
		}


		public string DropFieldQuery(string TableName, string FieldName)
		{
			return string.Format("ALTER TABLE {0} DROP COLUMN {1}", TableName, FieldName);
		}


		public string GetDistinctValuesQuery(string schemaName, string tableName, string fieldName)
		{
			return string.Format("SELECT {0} AS \"Value\", count(*) AS \"ValueCount\" FROM {1} WHERE {0} IS NOT NULL GROUP BY {0}", fieldName, EnsureSchemaName(schemaName, true) + tableName);
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
					sql = "MINUS";
					break;
				case MergeType.Intersect:
					sql = "INTERSECT";
					break;
			}
			return sql;
		}

		public string GetLimitQueryStart(SqlQuery Query)
		{
			StringBuilder sql = new StringBuilder();
			if (Query.IsLimitPercentage)
			{
				sql.Append("SELECT");
				if (Query.DistinctRows)
				{
					sql.Append(" DISTINCT");
				}
				sql.Append("\r\n    ");

				switch (Query.StepType)
				{
					case StepType.Output:
						int lastOutputIndex = -1;
						for (int i = Query.Columns.Count - 1; i > 0; i--)
						{
							if (Query.Columns[i].IncludeInOutput)
							{
								lastOutputIndex = i;
								break;
							}
						}

						for (int i = 0; i < Query.Columns.Count; i++)
						{
							if (Query.Columns[i].IncludeInOutput)
							{
								if (!string.IsNullOrEmpty(Query.Columns[i].OutputAs))
								{
									sql.Append(Query.Columns[i].OutputAs);
								}
								else
								{
									sql.Append(ExtractFieldName(Query.Columns[i].FieldName));
								}

								if (i < lastOutputIndex)
								{
									sql.Append(",\r\n    ");
								}
								else
								{
									sql.Append("\r\n ");
								}
							}
						}
						break;

					case StepType.Select:



						List<string> fieldList = new List<string>();

						for (int i = 0; i < Query.SelectFieldList.Count; i++)
						{
							fieldList.Add(ExtractFieldName(Query.SelectFieldList[i]));
						}
						for (int i = 0; i < Query.RootTableCarryover.Count; i++)
						{
							fieldList.Add(ExtractFieldName(Query.RootTableCarryover[i]));
						}
						for (int i = 0; i < fieldList.Count; i++)
						{
							sql.Append(fieldList[i]);
							if (i < fieldList.Count - 1)
							{
								sql.Append(",\r\n    ");
							}
						}


						break;

					case StepType.SplitProcess:
					case StepType.Assignment:
						for (int i = 0; i < Query.InsertFieldList.Count; i++)
						{
							sql.Append(ExtractFieldName(Query.InsertFieldList[i]));
							if (i < Query.InsertFieldList.Count - 1 || Query.RootTableCarryover.Count > 0)
							{
								sql.Append(",\r\n    ");
							}
						}

						for (int i = 0; i < Query.RootTableCarryover.Count; i++)
						{
							sql.Append(ExtractFieldName(Query.RootTableCarryover[i]));
							if (i < Query.RootTableCarryover.Count - 1)
							{
								sql.Append(",\r\n    ");
							}
							else
							{
								sql.Append("\r\n ");
							}
						}
						break;
				}

				sql.Append("\rFROM\r\n    (SELECT percent_rank() OVER (ORDER BY " + (Query.RandomSample ? "dbms_random.random" : "rownum") + ") as PercentRank, ");
			}
			else
			{
				sql.Append("SELECT\r\n    *\r\nFROM\r\n    (SELECT");
				if (Query.DistinctRows)
				{
					sql.Append(" DISTINCT");
				}
			}
			return sql.ToString();
		}

		public string GetLimitQueryEnd(SqlQuery Query)
		{
			if (Query.IsLimitPercentage)
			{
				return string.Format(") \r\nWHERE\r\n    PercentRank <= {0}", ((double)Query.Limit / 100).ToString());
			}
			else
			{
				return string.Format(") \r\nWHERE\r\n    rownum <= {0}", Query.Limit.ToString());
			}
		}

		public string GetOrderByRandom()
		{
			return "\r\nORDER BY\r\n    dbms_random.random";
		}


		public string GetCreateTableClause(string TableName)
		{
			return string.Format("CREATE TABLE {0} AS", TableName);
		}

		public string GetInsertIntoClause(string TableName)
		{
			return string.Empty;
		}


		public string ParseSqlStatement(SqlQuery Query)
		{
			return "";
		}

		private string ExtractFieldName(string Field)
		{
			if (string.IsNullOrEmpty(Field))
			{
				return "";
			}
			return Field.Substring(Field.LastIndexOf(".") + 1);
		}

		private string EnsureSchemaName(string SchemaName)
		{
			return EnsureSchemaName(SchemaName, false);
		}

		private string EnsureSchemaName(string SchemaName, bool FormatAsPrefix)
		{
			if (string.IsNullOrEmpty(SchemaName))
			{
				return SchemaName;
			}
			SchemaName = SchemaName.Trim().ToUpper();
			if (FormatAsPrefix && !SchemaName.EndsWith("."))
			{
				SchemaName += ".";
			}
			return SchemaName;
		}

		public string GetAssignmentCharacterConversion(string field)
		{
			return string.Format("to_char({0})", field);
		}

		public string GetCreateHistoryTableStatement()
		{
			return "CREATE TABLE " + Constants.CampaignHistoryTableName + " (AudienceKey nvarchar2(500), CampaignId number, StepId number, OutputType varchar2(50), OutputFile nvarchar2(500), OutputId number, OutputDate date default sysdate)";
		}


		public string GetDateFormula(string fieldName, DateTime startDate, DateTime? endDate, DateParts datePart, string dateOperator)
		{
			string start = string.Empty;
			string end = string.Empty;
			string field = string.Empty;

			switch (datePart)
			{
				case DateParts.Complete:
                    System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                    start = "to_date('" + startDate.ToString("dd-MMM-yyyy", culture) + "','dd-MON-yyyy','NLS_DATE_LANGUAGE = ''AMERICAN'' NLS_CALENDAR = ''GREGORIAN''')";
					if (endDate.HasValue)
					{
                        end = "to_date('" + endDate.Value.ToString("dd-MMM-yyyy", culture) + "','dd-MON-yyyy','NLS_DATE_LANGUAGE = ''AMERICAN'' NLS_CALENDAR = ''GREGORIAN''')";
					}
					field = fieldName;
					break;
				case DateParts.Day:
					start = "'" + startDate.Day.ToString().PadLeft(2, '0') + "'";
					if (endDate.HasValue)
					{
						end = "'" + endDate.Value.Day.ToString().PadLeft(2, '0') + "'";
					}
					field = "to_char(" + fieldName + ", 'DD')";
					break;
				case DateParts.Month:
					start = "'" + startDate.Month.ToString().PadLeft(2, '0') + "'";
					if (endDate.HasValue)
					{
						end = "'" + endDate.Value.Month.ToString().PadLeft(2, '0') + "'";
					}
					field = "to_char(" + fieldName + ", 'MM')";
					break;
				case DateParts.Year:
					start = "'" + startDate.Year.ToString().PadLeft(4, '0') + "'";
					if (endDate.HasValue)
					{
						end = "'" + endDate.Value.Year.ToString().PadLeft(4, '0') + "'";
					}
					field = "to_char(" + fieldName + ", 'YYYY')";
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
				ret = "length(" + ret + ")";
			}
			return ret;
		}

		public string GetDataSchemaPrefix(string schemaName)
		{
			return EnsureSchemaName(schemaName, true);
		}

        #endregion
    }
}
