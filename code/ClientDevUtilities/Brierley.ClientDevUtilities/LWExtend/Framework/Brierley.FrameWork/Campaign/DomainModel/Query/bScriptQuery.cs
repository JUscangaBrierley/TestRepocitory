using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class bScriptQuery : Query
	{
		public string Expression { get; set; }


		public override List<SqlStatement> GetSqlStatement(bool IsValidationTest, Dictionary<string, string> overrideParameters = null)
		{
			if (Step.Outputs != null && Step.Outputs.Count > 0)
			{
				//we only need a SQL statement if the step is fed into other steps. Otherwise, there's no point in
				//taking up the tablespace, since the data doesn't change with this step.

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
                    sqlQuery.StepType = StepType.Select;

                    sqlQuery.Limit = RowLimit != null ? (int)RowLimit : -1;
                    sqlQuery.RandomSample = RandomSample;
                    sqlQuery.IsLimitPercentage = IsLimitPercentage;
                    sqlQuery.DistinctRows = this.DistinctRows;


                    //begin select specific logic
                    sqlQuery.ActionTable = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                    sqlQuery.RootTableName = RootTable.Name;
                    sqlQuery.InsertFieldList.Add(Key.FieldName.ToLower());
                    sqlQuery.SelectFieldList.Add(RootTable.Name.ToLower() + "." + Key.FieldName.ToLower());
                    //sqlQuery.Columns = Columns; <-- no columns, we're simply taking what's in the input step's table
                    //end select specific logic

                    //MapTableJoins(sqlQuery);
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
                        //ApplyCampaignParameters(statement, overrideParameters);
                        statements.Add(statement);
                    }

                    return statements;
                }
			}

			return new List<SqlStatement>();
		}


		public override bool Validate(List<ValidationMessage> Warnings, bool ValidateSql)
		{
			if (Expression == null)
			{
				Warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Expression is required for step {0}", Step.UIName)));
				return false;
			}
			try
			{
				new Brierley.FrameWork.bScript.ExpressionFactory().Create(Expression);
			}
			catch (Exception ex)
			{
				Warnings.Add(new ValidationMessage(ValidationLevel.Exception, string.Format("Error parsing bScript expression for step {0}: {1}", Step.UIName, ex.Message)));
				return false;
			}
			return base.Validate(Warnings, ValidateSql);
		}


		internal override List<CampaignResult> Execute(FrameWork.ContextObject co = null, Dictionary<string, string> overrideParameters = null, bool resume = false)
		{
			int hash = Expression.GetHashCode();
			Expression exp = new ExpressionFactory().Create(Expression);

			bool success = true;
			object result = exp.evaluate(co);
			if (result != null)
			{
				if (!bool.TryParse(result.ToString(), out success))
				{
					//for this step, we assume success unless the expression returns false. 
					//We'll consider most anything to be "truthy" other than false (e.g., 0, 1, 'yes', 'no', null, "abcdefg").
					success = true; 
				}
			}

			//if success and step has outputs, we need to execute query to carry data over
			if (success && Step.Outputs != null && Step.Outputs.Count > 0)
			{
					return base.Execute(co, overrideParameters, resume);
			}

			//otherwise, the data does not need to carry, so we'll just truncate the table
			using(CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
			if (Step != null)
			{
				manager.BatchProvider.TruncateTable(manager.GetCampaignTable((long)Step.OutputTableId).Name);
			}

			//...and then return 1 or 0, depending on success
			if (success)
			{
				return new List<CampaignResult>() { new CampaignResult(1) };
			}
			else
			{
				return new List<CampaignResult>() { new CampaignResult(0) };
			}
		}
	}
}