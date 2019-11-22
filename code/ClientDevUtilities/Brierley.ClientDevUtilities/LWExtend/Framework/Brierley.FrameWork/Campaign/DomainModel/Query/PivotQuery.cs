using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class PivotQuery : Query
	{
		public override void EnsureSchema()
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                foreach (long inputID in this.Step.Inputs)
                {
                    manager.GetStep(inputID).Query.EnsureSchema();
                }

                //check for recent method call that resulted in the schema being up to date
                if (manager.CacheManager.Get(Constants.CacheRegions.StepSchemaById, this.Step.Id) != null)
                {
                    return;
                }

                string tableName = Constants.TempTableNamePrefix + Step.OutputTableId.GetValueOrDefault(0).ToString();
                string keyFieldName = Key.FieldName;
                string keyFieldType = Key.FieldType;

                //list to track all fields identified that are needed in the table. Will later be checked against
                //all fields that currently exist in the table. Existing fields that are not needed will be dropped.
                List<string> requiredFields = new List<string>();


                if (!manager.BatchProvider.TableExists(tableName, false))
                {
                    manager.BatchProvider.CreateTable(tableName, keyFieldName, keyFieldType);
                }

                if (!manager.BatchProvider.FieldExists(tableName, keyFieldName, false))
                {
                    manager.BatchProvider.DropTable(tableName);
                    EnsureSchema();
                    return;
                }

                requiredFields.Add(keyFieldName.ToLower());

                string currentType = manager.BatchProvider.GetFieldType(tableName, keyFieldName, false);
                if (currentType.Contains("("))
                {
                    currentType = currentType.Substring(0, currentType.IndexOf("("));
                }
                if (keyFieldType.Contains("("))
                {
                    keyFieldType = keyFieldType.Substring(0, keyFieldType.IndexOf("("));
                }
                if (!currentType.Trim().Equals(keyFieldType.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    manager.BatchProvider.DropTable(tableName);
                    EnsureSchema();
                    return;
                }

                //Append every field from the input step that *isn't* being pivoted (and doesn't already exist).
                //And append every field that *is* being pivoted, but usng the pivoted field names
                Step step = manager.GetStep(Step.Inputs[0]);
                if (step.OutputTableId == null)
                {
                    throw new Exception("Failed to ensure table schema. The input step " + step.Id.ToString() + " does not have an output table defined.");
                }
                CampaignTable table = manager.GetCampaignTable((long)step.OutputTableId);
                DataTable inputSchema = manager.BatchProvider.GetTableDetails(table.Name, false);
                foreach (DataRow row in inputSchema.Rows)
                {
                    string fieldName = row["FieldName"].ToString();
                    if (fieldName.Equals(Key.FieldName, StringComparison.OrdinalIgnoreCase))
                    {
                        //Key is not needed. continue
                        continue;
                    }

                    string dataType = row["DataType"].ToString();
                    if (Utils.DataTypeRequiresLength(dataType))
                    {
                        dataType += "(" + row["Length"].ToString() + ")";
                    }

                    var pivotColumn = Columns.Where(o => o.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (pivotColumn != null)
                    {
                        foreach (var condition in pivotColumn.Conditions)
                        {
                            if (!manager.BatchProvider.FieldExists(tableName, condition.AssignmentFieldName, false))
                            {
                                manager.BatchProvider.AddFieldToTable(tableName, condition.AssignmentFieldName, dataType);
                            }
                            requiredFields.Add(condition.AssignmentFieldName);
                        }
                    }
                    else
                    {
                        if (!manager.BatchProvider.FieldExists(tableName, fieldName, false))
                        {
                            manager.BatchProvider.AddFieldToTable(tableName, fieldName, dataType);
                        }
                        requiredFields.Add(fieldName);
                    }
                }

                //delete fields that are not required
                if (requiredFields.Count > 0 && !string.IsNullOrEmpty(tableName))
                {
                    DataTable outputSchema = manager.BatchProvider.GetTableDetails(tableName, false);

                    foreach (DataRow row in outputSchema.Rows)
                    {
                        if (!requiredFields.Contains(row["FieldName"].ToString(), StringComparer.OrdinalIgnoreCase))
                        {
                            manager.BatchProvider.RemoveFieldFromTable(tableName, row["FieldName"].ToString());
                        }
                    }
                }
                manager.CacheManager.Update(Constants.CacheRegions.StepSchemaById, Step.Id, true);
            }
		}

		public override List<SqlStatement> GetSqlStatement(bool isValidationTest, Dictionary<string, string> overrideParameters = null)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                var sqlQuery = new SqlQuery();
                sqlQuery.StepType = StepType.Select;

                if (RootTable == null)
                {
                    throw new Exception("Failed to get sql statement because the query does not have a root input table.");
                }

                sqlQuery.Limit = RowLimit != null ? (int)RowLimit : -1;
                sqlQuery.RandomSample = RandomSample;
                sqlQuery.IsLimitPercentage = IsLimitPercentage;
                sqlQuery.DistinctRows = this.DistinctRows;

                sqlQuery.ActionTable = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                sqlQuery.RootTableName = RootTable.Name;
                sqlQuery.InsertFieldList.Add(Key.FieldName.ToLower());
                sqlQuery.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());

                //add fields from root table...
                AddRootTableCarryoverFields(sqlQuery);
                //..and remove the ones that are being pivoted
                sqlQuery.RootTableCarryover = sqlQuery.RootTableCarryover.Where(o => Columns.Where(x => x.FieldName.Equals(o, StringComparison.OrdinalIgnoreCase)).Count() == 0).ToList();

                var inputStep = manager.GetStep(this.Step.Inputs[0]);
                inputStep = manager.GetStep(Step.Inputs[0]);
                string inputStepName = inputStep.UIName;
                string inputTableName = inputStep.OutputTableName;

                int index = 0;
                foreach (var column in Columns)
                {
                    int rowNumber = 1;
                    //add to select and insert list each pivoting column
                    foreach (var condition in column.Conditions)
                    {
                        string alias = GetJoinAlias(index++);
                        sqlQuery.InsertFieldList.Add(condition.AssignmentFieldName);
                        sqlQuery.SelectFieldList.Add(string.Format("{0}.{1} as {2}", alias, column.FieldName, condition.AssignmentFieldName));

                        var join = new Join();
                        join.Query = CreateSubquery(manager, column, condition, inputStepName, inputTableName);
                        join.JoinCondition = string.Format("{0}.rn = {1}", alias, (rowNumber++).ToString());
                        join.Alias = alias;
                        join.FieldName = Key.FieldName;
                        join.JoinToTableName = sqlQuery.RootTableName;
                        join.JoinToFieldName = Key.FieldName;
                        join.JoinType = TableJoinType.Left;

                        sqlQuery.Joins.Add(join);
                    }
                }

                if (isValidationTest)
                {
                    //override any existing limit and force it to 0, if we're validating.
                    sqlQuery.Limit = 0;
                    sqlQuery.IsLimitPercentage = false;
                }

                List<SqlStatement> statements = new List<SqlStatement>();
                foreach (var statement in manager.BatchProvider.CreateSqlStatement(sqlQuery))
                {
                    statements.Add(statement);
                }
                return statements;
            }
		}

		private string GetJoinAlias(int joinNumber)
		{
			int repeat = 1 + joinNumber / 26;
			joinNumber = joinNumber % 26;
			return string.Empty.PadLeft(repeat, (char)(joinNumber + 97));
		}

		private string CreateSubquery(CampaignManager manager, QueryColumn column, ColumnCondition condition, string inputStepName, string inputTableName)
		{
			SqlQuery q = new SqlQuery();
			q.StepType = StepType.Select;

			q.RootTableName = RootTable.Name;
			q.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());
			q.SelectFieldList.Add(column.FieldName);

			FieldEvaluation fields = new FieldEvaluation();
			fields.Fields = new Dictionary<CampaignTable, List<TableField>>();
			fields.Parameters = new List<Attribute>();

			string rankingExpression = base.ConvertFromAliases(column.ColumnExpression, fields, inputStepName, inputTableName);

			q.SelectFieldList.Add(string.Format("row_number() over (partition by {0} order by {1}) as rn", Key.FieldName, rankingExpression));

			return manager.BatchProvider.CreateSqlStatement(q)[0];
		}
	}
}
