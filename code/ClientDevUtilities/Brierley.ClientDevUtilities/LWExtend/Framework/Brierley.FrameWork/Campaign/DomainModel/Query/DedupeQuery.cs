using System;
using System.Collections.Generic;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class DedupeQuery : Query
	{
		public override List<SqlStatement> GetSqlStatement(bool IsValidationTest, Dictionary<string, string> overrideParameters = null)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                if (RootTable == null)
                {
                    throw new Exception("Failed to get sql statement because the query does not have a root input table.");
                }

                base.ApplyTableNames();

                SqlQuery sqlQuery = new SqlQuery();
                sqlQuery.StepType = StepType.DeDupe;
                sqlQuery.Limit = RowLimit != null ? (int)RowLimit : -1;
                sqlQuery.RandomSample = RandomSample;
                sqlQuery.IsLimitPercentage = IsLimitPercentage;
                sqlQuery.DistinctRows = this.DistinctRows;
                sqlQuery.ActionTable = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                sqlQuery.RootTableName = RootTable.Name;
                sqlQuery.InsertFieldList.Add(Key.FieldName.ToLower());
                sqlQuery.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());
                sqlQuery.Columns = Columns;

                MapTableJoins(sqlQuery);

                AddRootTableCarryoverFields(sqlQuery);

                if (IsValidationTest)
                {
                    //override any existing limit and force it to 0, if we're validating.
                    sqlQuery.Limit = 0;
                    sqlQuery.IsLimitPercentage = false;
                }

                List<SqlStatement> statements = new List<SqlStatement>();
                List<string> sqlStatements = manager.BatchProvider.CreateSqlStatement(sqlQuery);
                foreach (string statement in sqlStatements)
                {
                    statements.Add(statement);
                }


                return statements;
            }
		}
	}
}