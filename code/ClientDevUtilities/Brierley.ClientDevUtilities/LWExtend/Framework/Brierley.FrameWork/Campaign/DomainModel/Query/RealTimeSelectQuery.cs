using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class RealTimeSelectQuery : Query
	{
		private bool _isExecuting = false;

		public override List<SqlStatement> GetSqlStatement(bool IsValidationTest, Dictionary<string, string> overrideParameters = null)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                List<SqlStatement> statements = null;

                if (_isExecuting && !IsValidationTest)
                {
                    statements = (List<SqlStatement>)manager.CacheManager.Get(Constants.CacheRegions.SqlStatementsByStepId, this.Step.Id);

                    if (statements != null)
                    {
                        return statements;
                    }
                }

                statements = new List<SqlStatement>();

                SqlQuery sqlQuery = new SqlQuery();
                sqlQuery.StepType = StepType.Output;

                if (RootTable == null)
                {
                    throw new Exception("Failed to get sql statement because the query does not have a root input table.");
                }

                //begin select specific logic
                sqlQuery.ActionTable = null;
                sqlQuery.RootTableName = RootTable.Name;
                sqlQuery.InsertFieldList.Add(Key.FieldName.ToLower());
                sqlQuery.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());
                sqlQuery.Columns = Columns;

                sqlQuery.RealTimeIPCodeParameter = true;

                //end select specific logic

                MapTableJoins(sqlQuery);

                //hack: step validation did not provide parameters for the query, so queries that reference parameters that are passed around and the all important ipcode
                //parameter are missing when validating, and the validate function fails due to the invalid SQL. SqlStatement has been modified so that it holds parameters
                //now, and those may be used when validating. The Execute method handles this in a slightly different way, so we'll need to eventually create a single way to
                //do this, but as of this writing there was a time constraint as the issue was found in QA.
                List<string> sqlStatements = manager.RealTimeProvider.CreateSqlStatement(sqlQuery);
                foreach (SqlStatement statement in sqlStatements)
                {
                    ApplyCampaignParameters(statement, overrideParameters);

                    if (IsValidationTest)
                    {
                        statement.AddParameter("ipcode", -1);
                        foreach (string parm in GetRealTimeAssignmentParameterNames())
                        {
                            statement.AddParameter(parm, "?");
                        }
                    }

                    statements.Add(statement);
                }

                if (!IsValidationTest)
                {
                    manager.CacheManager.Update(Constants.CacheRegions.SqlStatementsByStepId, Step.Id, statements);
                }
                return statements;
            }
		}


		internal override List<CampaignResult> Execute(ContextObject co = null, Dictionary<string, string> overrideParameters = null, bool resume = false)
		{
			_isExecuting = true;

            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
				long ipCode = -1;
				Member m = co.ResolveMember();
				if (m != null)
				{
					ipCode = m.IpCode;
				}

                int rowCount = 0;
                var env = co.Environment;

                foreach (SqlStatement sql in GetSqlStatement(overrideParameters))
                {
                    var parms = new Dictionary<string, object>();
                    parms.Add("ipcode", ipCode);

                    foreach (string parm in GetRealTimeAssignmentParameterNames())
                    {
                        string val = string.Empty;
                        if (env != null && env.ContainsKey(parm))
                        {
                            val = (string)env[parm];
                        }
                        //can't do this - not thread safe (sql is cached)
                        //sql.AddParameter(parm, val);
                        parms.Add(parm, val);
                    }

                    var table = manager.RealTimeProvider.ExecuteDataTable(sql, parms);
                    if (sql.ApplyToResults)
                    {
                        rowCount += table.Rows.Count;
                    }
                }
                return new List<CampaignResult>() { new CampaignResult(rowCount) };
            }
		}


		protected internal override void ConvertFromAliases(Query.FieldEvaluation fieldsInUse, QueryColumn column, string inputStepName, string inputTableName)
		{
			base.ConvertFromAliases(fieldsInUse, column, inputStepName, inputTableName);
		}
	}
}