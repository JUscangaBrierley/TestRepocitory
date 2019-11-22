using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class ControlGroupQuery:Query
	{
		public override List<SqlStatement> GetSqlStatement(bool IsValidationTest, Dictionary<string, string> overrideParameters = null)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                string altSchemaName = manager.BatchProvider.DataSchemaPrefix;
                if (!string.IsNullOrEmpty(altSchemaName))
                {
                    if (!altSchemaName.EndsWith("."))
                        altSchemaName += ".";
                }
            }
			base.ApplyTableNames();

			return GetControlGroupSqlStatements(IsValidationTest, overrideParameters);
		}


		private List<SqlStatement> GetControlGroupSqlStatements(bool IsValidationTest, Dictionary<string, string> overrideParameters = null)
		{
			List<SqlStatement> statements = new List<SqlStatement>();
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                if (RootTable == null)
                {
                    throw new Exception("Failed to get sql statement because the query does not have a root input table.");
                }

                string processName = string.Empty;
                string segmentName = string.Empty;

                //find segment column
                foreach (QueryColumn column in Columns)
                {
                    if (column.ColumnType == ColumnType.Segment)
                    {
                        processName = column.FieldName;
                        segmentName = column.ColumnExpression;
                    }
                }

                foreach (QueryColumn column in this.Columns)
                {
                    if (column.ColumnType != ColumnType.Condition || column.Conditions.Count < 2)
                    {
                        continue;
                    }

                    ColumnCondition testCondition = null;
                    ColumnCondition controlCondition = null;

                    foreach (ColumnCondition condition in column.Conditions)
                    {
                        if (condition.RowOrder == 1)
                        {
                            testCondition = condition;
                        }
                        else if (condition.RowOrder == 2)
                        {
                            controlCondition = condition;
                        }
                    }

                    //test
                    SqlQuery sqlQuery = new SqlQuery();
                    sqlQuery.Columns = new List<QueryColumn>();
                    sqlQuery.StepType = StepType.Select;

                    if (IsValidationTest)
                    {
                        sqlQuery.Limit = 0;
                        sqlQuery.IsLimitPercentage = false;
                    }
                    else
                    {
                        sqlQuery.Limit = column.RowLimit.GetValueOrDefault(-1);
                        sqlQuery.IsLimitPercentage = column.IsLimitPercentage;
                    }
                    sqlQuery.RandomSample = column.RandomSample;

                    sqlQuery.ActionTable = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                    sqlQuery.RootTableName = RootTable.Name;

                    sqlQuery.InsertFieldList.Add(Key.FieldName.ToLower());
                    sqlQuery.InsertFieldList.Add(processName);

                    sqlQuery.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());
                    sqlQuery.SelectFieldList.Add(manager.BatchProvider.EnsureQuotes(testCondition.AssignmentFieldName) + " AS " + processName);

                    QueryColumn segmentColumn = new QueryColumn();
                    segmentColumn.TableId = RootTable.Id;
                    segmentColumn.TableName = RootTable.Name;
                    segmentColumn.FieldName = segmentName;
                    segmentColumn.ColumnType = ColumnType.Condition;
                    segmentColumn.Conditions = new List<ColumnCondition>();
                    segmentColumn.Conditions.Add(new ColumnCondition() { ConditionExpression = testCondition.ConditionExpression });
                    sqlQuery.Columns.Add(segmentColumn);

                    //all other fields in the source table must carry over to the destination
                    DataTable additionalFields = manager.BatchProvider.GetTableDetails(RootTable.Name, false);
                    foreach (DataRow row in additionalFields.Rows)
                    {
                        if (
                            !sqlQuery.InsertFieldList.Contains(row["FieldName"].ToString().ToLower()) &&
                            !sqlQuery.SelectFieldList.Contains(RootTable.Name.ToLower() + "." + row["FieldName"].ToString().ToLower()) &&
                            row["FieldName"].ToString().ToLower() != Key.FieldName.ToLower()
                            )
                        {
                            sqlQuery.RootTableCarryover.Add(row["FieldName"].ToString().ToLower());
                        }
                    }

                    List<string> sqlStatements = manager.BatchProvider.CreateSqlStatement(sqlQuery);
                    foreach (SqlStatement statement in sqlStatements)
                    {
                        ApplyCampaignParameters(statement, overrideParameters);
                        statements.Add(statement);
                    }


                    //control
                    sqlQuery = new SqlQuery();
                    sqlQuery.Columns = new List<QueryColumn>();
                    sqlQuery.StepType = StepType.Select;

                    if (IsValidationTest)
                    {
                        sqlQuery.Limit = 0;
                    }
                    else
                    {
                        sqlQuery.Limit = -1;
                    }

                    sqlQuery.ActionTable = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                    sqlQuery.RootTableName = RootTable.Name;

                    sqlQuery.InsertFieldList.Add(Key.FieldName.ToLower());
                    sqlQuery.InsertFieldList.Add(processName);

                    sqlQuery.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());
                    sqlQuery.SelectFieldList.Add(manager.BatchProvider.EnsureQuotes(controlCondition.AssignmentFieldName) + " AS " + processName);


                    //exclude any rows already in the output table
                    sqlQuery.Exclusions = new List<Join>();
                    Join join = new Join();
                    join.TableName = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                    join.JoinToTableName = RootTable.Name;
                    join.FieldName = Key.FieldName;
                    join.JoinToFieldName = Key.FieldName;
                    //join.JoinType = Join.TableJoinType.Left;
                    join.JoinType = GetJoinHintOrDefault(join.TableName, join.FieldName, join.JoinToTableName, join.JoinToFieldName);
                    sqlQuery.Joins.Add(join);
                    QueryColumn exclusionColumn = new QueryColumn() { ColumnType = ColumnType.Condition, FieldName = join.FieldName, TableName = join.TableName };
                    exclusionColumn.Conditions.Add(new ColumnCondition() { ConditionExpression = "IS NULL", RowOrder = 1 });
                    sqlQuery.Columns.Add(exclusionColumn);

                    segmentColumn = new QueryColumn();
                    segmentColumn.TableId = RootTable.Id;
                    segmentColumn.TableName = RootTable.Name;
                    segmentColumn.FieldName = segmentName;
                    segmentColumn.ColumnType = ColumnType.Condition;
                    segmentColumn.Conditions = new List<ColumnCondition>();
                    segmentColumn.Conditions.Add(new ColumnCondition() { ConditionExpression = testCondition.ConditionExpression, RowOrder = 1 });
                    sqlQuery.Columns.Add(segmentColumn);


                    //AddRootTableCarryoverFields(sqlQuery);
                    //all other fields in the source table must carry over to the destination
                    foreach (DataRow row in additionalFields.Rows)
                    {
                        if (
                            !sqlQuery.InsertFieldList.Contains(row["FieldName"].ToString().ToLower()) &&
                            !sqlQuery.SelectFieldList.Contains(RootTable.Name.ToLower() + "." + row["FieldName"].ToString().ToLower()) &&
                            row["FieldName"].ToString().ToLower() != Key.FieldName.ToLower()
                            )
                        {
                            sqlQuery.RootTableCarryover.Add(row["FieldName"].ToString().ToLower());
                        }
                    }


                    if (IsValidationTest)
                    {
                        //override any existing limit and force it to 0, if we're validating.
                        sqlQuery.Limit = 0;
                        sqlQuery.IsLimitPercentage = false;
                    }

                    sqlStatements = manager.BatchProvider.CreateSqlStatement(sqlQuery);
                    foreach (SqlStatement statement in sqlStatements)
                    {
                        ApplyCampaignParameters(statement, overrideParameters);
                        statements.Add(statement);
                    }
                }
                return statements;
            }
		}


	}
}