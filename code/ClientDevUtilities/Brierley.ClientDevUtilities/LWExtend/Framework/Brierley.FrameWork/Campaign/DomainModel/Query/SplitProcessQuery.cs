using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using System.Data;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class SplitProcessQuery : Query
	{
		public string ProcessName { get; set; }

		public string ProcessValue { get; set; }

		public bool CatchOrphans { get; set; }

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

                base.ApplyTableNames();

                SqlQuery sqlQuery = new SqlQuery();
                sqlQuery.StepType = StepType.SplitProcess;

                if (RootTable == null)
                {
                    throw new Exception("Failed to get sql statement because the query does not have a root input table.");
                }

                sqlQuery.Limit = RowLimit != null ? (int)RowLimit : -1;
                sqlQuery.RandomSample = RandomSample;
                sqlQuery.IsLimitPercentage = IsLimitPercentage;
                sqlQuery.DistinctRows = this.DistinctRows;

                //begin split process specific logic

                sqlQuery.ActionTable = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                sqlQuery.RootTableName = RootTable.Name;
                sqlQuery.InsertFieldList.Add(Key.FieldName.ToLower());
                sqlQuery.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());

                if (!string.IsNullOrEmpty(ProcessName) && !string.IsNullOrEmpty(ProcessValue))
                {
                    sqlQuery.InsertFieldList.Add(ProcessName);
                    sqlQuery.SelectFieldList.Add(ProcessValue + " as " + ProcessName);
                }

                //create new list of columns instead of passing the reference. This query needs to create additional columns
                //that cannot be persisted to the database:
                //sqlQuery.Columns = Columns;
                sqlQuery.Columns = new List<QueryColumn>();
                foreach (var c in Columns)
                {
                    sqlQuery.Columns.Add(c);
                }


                //end update specific logic

                MapTableJoins(sqlQuery);

                if (Step.StepType == StepType.SplitProcess && Step.Inputs.Count == 1 && ((SplitProcessQuery)Step.Query).CatchOrphans)
                {
                    sqlQuery.Exclusions = new List<Join>();
                    IList<StepIO> steps = manager.StepIODao.RetrieveOutputs(Step.Inputs[0]);
                    foreach (StepIO io in steps)
                    {
                        if (io.OutputStepId != Step.Id)
                        {
                            Step step = manager.GetStep(io.OutputStepId);
                            if (
                                step != null &&
                                step.StepType == StepType.SplitProcess &&
                                step.OutputTableId != null &&
                                (step.UIPositionX < Step.UIPositionX || (step.UIPositionX == Step.UIPositionX && step.UIPositionY < Step.UIPositionY)
                                ))
                            {
                                Join join = new Join();
                                join.TableName = manager.GetCampaignTable((long)step.OutputTableId).Name;
                                join.JoinToTableName = manager.GetCampaignTable(this.Key.TableId).Name;
                                join.FieldName = Key.FieldName;
                                join.JoinToFieldName = Key.FieldName;
                                join.JoinType = GetJoinHintOrDefault(join.TableName, join.FieldName, join.JoinToTableName, join.JoinToFieldName);
                                //sqlQuery.Exclusions.Add(join);
                                sqlQuery.Joins.Add(join);
                                QueryColumn exclusionColumn = new QueryColumn() { ColumnType = ColumnType.Condition, FieldName = join.FieldName, TableName = join.TableName };

                                List<int> rows = new List<int>();
                                foreach (QueryColumn column in this.Columns)
                                {
                                    foreach (ColumnCondition condition in column.Conditions)
                                    {
                                        if (!rows.Contains(condition.RowOrder))
                                        {
                                            rows.Add(condition.RowOrder);
                                        }
                                    }
                                }
                                if (rows.Count == 0)
                                {
                                    rows.Add(1);
                                }

                                foreach (int row in rows)
                                {
                                    exclusionColumn.Conditions.Add(new ColumnCondition() { ConditionExpression = "IS NULL", RowOrder = row });
                                }
                                sqlQuery.Columns.Add(exclusionColumn);
                            }
                        }
                    }
                }



                AddRootTableCarryoverFields(sqlQuery);

                if (IsValidationTest)
                {
                    //override any existing limit and force it to 0, if we're validating.
                    sqlQuery.Limit = 0;
                    sqlQuery.IsLimitPercentage = false;
                }

                List<SqlStatement> statements = new List<SqlStatement>();
                List<string> sqlStatements = manager.BatchProvider.CreateSqlStatement(sqlQuery);
                foreach (SqlStatement statement in sqlStatements)
                {
                    ApplyCampaignParameters(statement, overrideParameters);
                    statements.Add(statement);
                }
                return statements;
            }
		}


	}
}