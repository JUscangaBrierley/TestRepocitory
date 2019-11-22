using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class RealTimeAssignmentQuery : Query
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

                //give the data provider the name of each table, rather than the internal id (which is useless to the data provider)
                foreach (QueryColumn column in Columns)
                {
                    if (column.TableId > 0)
                    {
                        CampaignTable table = manager.GetCampaignTable(column.TableId);
                        column.TableName = table.Name; //manager.GetCampaignTable(column.TableID).Name;
                    }
                    if (column.ColumnType == ColumnType.Append && column.Conditions.Count > 0)
                    {
                        column.IncludeInOutput = true;
                        foreach (ColumnCondition c in column.Conditions)
                        {
                            if (c.AssignmentTableId.GetValueOrDefault(0) > 0 && !string.IsNullOrEmpty(c.AssignmentFieldName))
                            {
                                CampaignTable table = manager.GetCampaignTable(c.AssignmentTableId.Value);
                                c.AssignmentTableName = table.Name;
                            }
                        }
                    }
                }

                SqlQuery sqlQuery = new SqlQuery();
                sqlQuery.StepType = StepType.Output;

                if (RootTable == null)
                {
                    throw new Exception("Failed to get sql statement because the query does not have a root input table.");
                }

                //begin updateall specific logic

                sqlQuery.ActionTable = null;
                sqlQuery.RootTableName = RootTable.Name;
                sqlQuery.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());

                sqlQuery.Columns = Columns;
                sqlQuery.RealTimeIPCodeParameter = true;

                foreach (QueryColumn column in Columns.Where(o => o.ColumnType == ColumnType.Segment || o.ColumnType == ColumnType.Append))
                {
                    sqlQuery.InsertFieldList.Add(column.FieldName.ToLower());
                }

                //end updateall specific logic

                MapTableJoins(sqlQuery);

                List<string> sqlStatements = manager.RealTimeProvider.CreateSqlStatement(sqlQuery);
                foreach (SqlStatement statement in sqlStatements)
                {
                    //if (IsValidationTest)
                    //{
                    //	var parms = new Dictionary<string, object>();

                    //	parms.Add("ipcode", -1);

                    //	foreach (var qc in Columns.Where(o => o.TableId == 0))
                    //	{
                    //		parms.Add(qc.FieldName, "?");
                    //	}
                    //}

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
                int rowCount = 0;
                var env = co.Environment;

				long ipCode = -1;
				Member m = co.ResolveMember();
				if (m != null)
				{
					ipCode = m.IpCode;
				}

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
                        parms.Add(parm, val);
                    }

                    var table = manager.RealTimeProvider.ExecuteDataTable(sql.ToString(), parms);
                    if (sql.ApplyToResults)
                    {
                        rowCount += table.Rows.Count;
                        if (table.Rows.Count > 0 && co != null)
                        {
                            if (co.Environment == null)
                            {
                                co.Environment = new Dictionary<string, object>();
                            }
                            var dictionary = co.Environment;
                            foreach (var col in Columns.Where(o => o.ColumnType == ColumnType.Append))
                            {
                                var name = col.FieldName;
                                var val = table.Rows[0][name].ToString();
                                if (dictionary.ContainsKey(name))
                                {
                                    dictionary[name] = val;
                                }
                                else
                                {
                                    dictionary.Add(name, val);
                                }
                            }
                        }
                    }
                }
                return new List<CampaignResult>() { new CampaignResult(rowCount) };
            }
		}

	}
}