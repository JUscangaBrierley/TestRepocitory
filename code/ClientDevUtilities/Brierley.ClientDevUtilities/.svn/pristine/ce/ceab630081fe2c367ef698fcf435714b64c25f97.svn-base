using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Data;
using System.Collections;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class MergeQuery : Query
	{
		public override List<SqlStatement> GetSqlStatement(bool IsValidationTest, Dictionary<string, string> overrideParameters = null)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                base.ApplyTableNames();

                SqlQuery sqlQuery = new SqlQuery();
                sqlQuery.StepType = StepType.Merge;

                if (RootTable == null)
                {
                    throw new Exception("Failed to get sql statement because the query does not have a root input table.");
                }

                sqlQuery.Limit = RowLimit != null ? (int)RowLimit : -1;
                sqlQuery.RandomSample = RandomSample;
                sqlQuery.IsLimitPercentage = IsLimitPercentage;
                sqlQuery.DistinctRows = this.DistinctRows;


                sqlQuery.MergeFields = this.Columns.Select(o => o.FieldName).ToList();


                //begin union specific section
                sqlQuery.ActionTable = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                sqlQuery.InsertFieldList.Add(Key.FieldName);
                //sqlQuery.InsertFieldList.AddRange(this.Columns.Select(o => o.FieldName));
                List<StepIO> inputs = (List<StepIO>)manager.StepIODao.RetrieveInputs(Step.Id);
                foreach (StepIO io in inputs.OrderBy(o => o.MergeOrder))
                {
                    if (io.MergeType != null)
                    {
                        Step mergeStep = manager.GetStep(io.InputStepId);
                        Merge merge = new Merge();
                        merge.TableName = manager.GetCampaignTable(mergeStep.OutputTableId.GetValueOrDefault(-1)).Name;
                        merge.Schema = manager.BatchProvider.GetTableDetails(merge.TableName, false);
                        merge.MergeType = (MergeType)io.MergeType;
                        //merge.KeyField = mergeStep.Query.Key.FieldName;

                        IList<TableKey> mergeKeys = manager.GetTableKeyByTable(mergeStep.OutputTableId.GetValueOrDefault(-1));
                        if (mergeKeys.Count > 0)
                        {
                            merge.KeyField = mergeKeys[0].FieldName;
                        }


                        sqlQuery.Merges.Add(merge);
                    }
                }
                //end union specific section

                AddRootTableCarryoverFields(sqlQuery);

                if (IsValidationTest)
                {
                    //override any existing limit and force it to 0, if we're validating.
                    sqlQuery.Limit = 0;
                    sqlQuery.IsLimitPercentage = false;
                }

                List<SqlStatement> statements = new List<SqlStatement>();
                statements.Add(CreateUnionQuery(sqlQuery));
                return statements;
            }
		}


		public string CreateUnionQuery(SqlQuery query)
		{
			//TODO: the schema for each of the tables listed in Query.Merges comes from the physical table. It should come from a method GetSchema()
			//on the Query class. Using the physical table will require that all input step tables exist prior to building the sql query. It IS ok to
			//run the step, because running will require an EnsureSchema() call, which will build all tables, but make sure there is no condition where
			//we need to build the sql statement before the tables exist.

			ICMQueryManager queryManager;
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance()) 
				queryManager = manager.BatchProvider.QueryManager;

			List<string> fieldList = new List<string>();
			StringBuilder sql = new StringBuilder();

			//build field list of all "union" and "union all" merges. These carry over. <-- In addition to this, we're now only including 
			//fields that the user has selected as merge fields. 

			if (query.MergeFields == null)
			{
				query.MergeFields = new List<string>();
			}

			if (query.MergeFields.Count == 0 && query.Merges.Count > 0)
			{
				query.MergeFields.Add(query.Merges[0].KeyField);
			}

			foreach (var merge in (from x in query.Merges /*where x.MergeType != MergeType.ExceptOrMinus && x.MergeType != MergeType.Intersect*/ select x))
			{
				foreach (DataRow row in merge.Schema.Rows)
				{
					string fieldName = row["FieldName"].ToString();
					if (!fieldList.Contains(fieldName, StringComparer.OrdinalIgnoreCase) /*&& (query.MergeFields.Contains(fieldName, StringComparer.OrdinalIgnoreCase) || fieldName.Equals(merge.KeyField, StringComparison.OrdinalIgnoreCase))*/)
					{
						fieldList.Add(fieldName);
					}
				}
			}

			sql.AppendFormat("INSERT INTO {0} (", query.ActionTable);
			sql.Append(string.Join(", ", fieldList.ToArray()));
			sql.Append(")");

			for (int i = 0; i < query.Merges.Count; i++)
			{
				StringBuilder where = new StringBuilder();
				Merge merge = query.Merges[i];
				sql.AppendLine();
				if (merge.MergeType == MergeType.Union || merge.MergeType == MergeType.UnionAll)
				{
					//top, intersect and minus do not use sql merge keyword in CM.
					sql.Append(queryManager.GetMergeKeyword(query.Merges[i].MergeType));
					sql.Append(" ");
				}
				sql.Append("SELECT");
				sql.AppendLine();
				sql.Append("    ");

				foreach (string field in fieldList)
				{
					bool hasField = false;
					foreach (DataRow row in query.Merges[i].Schema.Rows)
					{
						if (row["FieldName"].ToString().Equals(field, StringComparison.OrdinalIgnoreCase))
						{
							hasField = true;
							break;
						}
					}

					if (hasField)
					{
						if (query.Merges.Where(o => o.MergeType == MergeType.Union || o.MergeType == MergeType.UnionAll).Count() > 0)
						{
							hasField = query.MergeFields.Contains(field, StringComparer.OrdinalIgnoreCase);
						}
					}

					if (hasField)
					{
						sql.AppendFormat("{0}.{1}", query.Merges[i].TableName, field);
					}
					else
					{
						sql.AppendFormat("null as \"{0}\"", field);
					}

					if (field != fieldList[fieldList.Count - 1])
					{
						sql.Append(", ");
					}
				}
				sql.AppendLine();
				sql.Append("FROM");
				sql.AppendLine();
				sql.Append("    ");
				sql.Append(query.Merges[i].TableName);

				//advance through merge index, appening join conditions for Intersect and Minus, until end or next union is reached
				for (int nextIndex = i + 1; nextIndex < query.Merges.Count; nextIndex++)
				{

					var nextMerge = query.Merges[nextIndex];
					if (nextMerge.MergeType == MergeType.ExceptOrMinus || nextMerge.MergeType == MergeType.Intersect)
					{
						sql.AppendLine();

						//this can throw duplicates into our result, if the right-side merge table has them:
						//sql.AppendFormat("    {0} JOIN {1} ON ", nextMerge.MergeType == MergeType.Intersect ? "INNER" : "LEFT", nextMerge.TableName);

						//this works, but I think we need the list of fields instead of "*" in order for it to work correctly (moving the distinct keyword up
						//a level causes any duplicates in the left-side table to be removed, which we want to avoid doing):

						List<string> subqueryFields = new List<string>();

						foreach (string field in query.MergeFields)
						{
							if (nextMerge.Schema.AsEnumerable().Where(o => o.Field<string>("FieldName").Equals(field, StringComparison.OrdinalIgnoreCase)).Count() > 0)
							{
								subqueryFields.Add(field);
							}
						}
						

						sql.AppendFormat("    {0} JOIN (SELECT DISTINCT {1} FROM {2}) {2} ON ",
							nextMerge.MergeType == MergeType.Intersect ? "INNER" : "LEFT",
							string.Join(", ", subqueryFields.ToArray()),
							nextMerge.TableName);


						sql.AppendFormat("{0}.{1} = {2}.{3}",
							nextMerge.TableName,
							nextMerge.KeyField,
							merge.TableName,
							merge.KeyField
							);

						foreach (string mergeField in query.MergeFields.Where(o => !o.Equals(query.Merges[i].KeyField, StringComparison.OrdinalIgnoreCase)))
						{
							bool hasField =
							query.Merges[i].Schema.AsEnumerable().Where(o => o.Field<string>("FieldName").Equals(mergeField, StringComparison.OrdinalIgnoreCase)).Count() > 0 &&
							nextMerge.Schema.AsEnumerable().Where(o => o.Field<string>("FieldName").Equals(mergeField, StringComparison.OrdinalIgnoreCase)).Count() > 0;

							if (hasField)
							{
								sql.AppendLine();
								sql.AppendFormat("    AND {0}.{2} = {1}.{2}",
								nextMerge.TableName,
								query.Merges[i].TableName,
								mergeField
								);
							}
						}

						if (nextMerge.MergeType == MergeType.ExceptOrMinus)
						{
							if (where.Length > 0)
							{
								where.Append(" AND");
								where.AppendLine();
							}
							where.AppendFormat("    {0}.{1} IS NULL", nextMerge.TableName, nextMerge.KeyField);
						}

						//advance to next index
						i = nextIndex;
					}
					else
					{
						i = nextIndex - 1;
						break;
					}
				}

				if (query.Limit == 0)
				{
					if (where.Length > 0)
					{
						where.Append(" AND");
						where.AppendLine();
					}
					//this is a validation test, add "where 1=2"
					where.Append("    1 = 2");
				}

				if (where.Length > 0)
				{
					sql.AppendLine();
					sql.Append("WHERE");
					sql.AppendLine();
					sql.Append(where.ToString());
				}

				//end
			}

			return sql.ToString();
		}

	}
}