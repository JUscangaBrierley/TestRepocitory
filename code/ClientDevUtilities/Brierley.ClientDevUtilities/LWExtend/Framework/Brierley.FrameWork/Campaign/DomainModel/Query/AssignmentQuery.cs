using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class AssignmentQuery : Query
	{
		public override List<SqlStatement> GetSqlStatement(bool IsValidationTest, Dictionary<string, string> overrideParameters = null)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {

                base.ApplyTableNames();

                SqlQuery sqlQuery = new SqlQuery();
                sqlQuery.StepType = StepType.Assignment;
                sqlQuery.Limit = RowLimit != null ? (int)RowLimit : -1;
                sqlQuery.RandomSample = RandomSample;
                sqlQuery.IsLimitPercentage = IsLimitPercentage;
                sqlQuery.DistinctRows = this.DistinctRows;

                //begin assignment specific logic
                sqlQuery.ActionTable = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                sqlQuery.RootTableName = RootTable.Name;
                sqlQuery.InsertFieldList.Add(Key.FieldName.ToLower());
                sqlQuery.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());

                foreach (QueryColumn column in Columns.Where(o => o.ColumnType == ColumnType.Append))
                {
                    sqlQuery.InsertFieldList.Add(column.FieldName.ToLower());
                }

                sqlQuery.Columns = Columns;

                //end assignment specific logic

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