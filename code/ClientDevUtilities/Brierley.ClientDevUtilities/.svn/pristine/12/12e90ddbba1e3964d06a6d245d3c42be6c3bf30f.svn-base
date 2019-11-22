using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class ChangeAudienceQuery : SelectQuery
	{
		[System.Xml.Serialization.XmlElement(/*Order = 12*/)]
		public virtual Int64? ConvertToKeyId { get; set; }

		[System.Xml.Serialization.XmlElement(/*Order = 13*/)]
		public virtual KeyConversionFunctions? KeyConversionFunction { get; set; }


		public bool ShouldSerializeConvertToKey()
		{
			return !SerializingToDatabase;
		}

		[System.Xml.Serialization.XmlElement(/*Order = 17*/)]
		public virtual TableKey ConvertToKey
		{
			get
			{
				if (!ConvertToKeyId.HasValue)
				{
					return null;
				}
				else
				{
                    using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                        return manager.GetTableKey(ConvertToKeyId.Value);
				}
			}
		}


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
                sqlQuery.StepType = StepType.ChangeAudience;

                if (RootTable == null)
                {
                    throw new Exception("Failed to get sql statement because the query does not have a root input table.");
                }

                sqlQuery.Limit = RowLimit != null ? (int)RowLimit : -1;
                sqlQuery.RandomSample = RandomSample;
                sqlQuery.IsLimitPercentage = IsLimitPercentage;
                sqlQuery.DistinctRows = this.DistinctRows;

                //begin change audience specific logic

                sqlQuery.ActionTable = Constants.TempTableNamePrefix + Step.OutputTableId.ToString();
                sqlQuery.RootTableName = RootTable.Name;

                string keyConversion = string.Empty;
                switch (KeyConversionFunction.GetValueOrDefault(KeyConversionFunctions.Max))
                {
                    case KeyConversionFunctions.TakeAllDuplicates:
                        break;
                    case KeyConversionFunctions.Max:
                        keyConversion = "max";
                        break;
                    case KeyConversionFunctions.Min:
                        keyConversion = "min";
                        break;
                }

                TableKey convertToKey = ConvertToKey;
                TableKey convertFromKey = Step.Key;

                sqlQuery.InsertFieldList.Add(convertToKey.FieldName.ToLower());
                sqlQuery.SelectFieldList.Add(manager.GetCampaignTable(convertToKey.TableId).Name.ToLower() + "." + convertToKey.FieldName.ToLower());

                sqlQuery.InsertFieldList.Add(convertFromKey.FieldName.ToLower());
                sqlQuery.SelectFieldList.Add(keyConversion + "(" + manager.GetCampaignTable(convertFromKey.TableId).Name.ToLower() + "." + convertFromKey.FieldName.ToLower() + ")");

                //Need to be able to map the join of new audience key, so it will be added as a column
                var columns = new List<QueryColumn>();
                var col = new QueryColumn();
                col.TableId = convertToKey.TableId;
                col.TableName = manager.GetCampaignTable(convertToKey.TableId).Name;
                col.FieldName = convertToKey.FieldName;
                col.ColumnType = ColumnType.Condition;
                columns.Add(col);

                col.Conditions.Add(new ColumnCondition() { ConditionExpression = "is not null" });

                sqlQuery.Columns = columns;

                //end change audience specific logic


                MapTableJoins(sqlQuery, columns);
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