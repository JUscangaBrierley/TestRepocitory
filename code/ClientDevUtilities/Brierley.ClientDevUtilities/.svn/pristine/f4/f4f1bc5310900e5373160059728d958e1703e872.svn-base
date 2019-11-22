using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using System.Data;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	public class SqlTextQuery:Query
	{
		/// <summary>
		/// Gets or sets the SqlText for the current Query
		/// </summary>
		[System.Xml.Serialization.XmlElement(/*Order = 6*/)]
		public virtual string SqlText { get; set; }

		//SqlBeginText and SqlSuggestedText worked great before we started serializing the queries for storage. Now the serializer will call
		//these and store them. Aside from the extra space they take up, they also cause stack overflow exceptions when hit from the serializer
		//because they - for reasons currently unknown - end up in an infinite loop reloading the step.
		public bool ShouldSerializeSqlBeginText()
		{
			return !SerializingToDatabase;
		}

		public bool ShouldSerializeSqlSuggestedText()
		{
			return !SerializingToDatabase;
		}


		[System.Xml.Serialization.XmlElement(/*Order = 7*/)]
		public virtual string SqlBeginText
		{
			get
			{
				string ret = "INSERT INTO \r\n\t";
				if (this.Step != null && this.Step.Inputs.Count > 0 && this.Key != null)
				{
                    using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                    {
                        string keyField = this.Key.FieldName;
                        List<string> additionalFields = new List<string>();

                        ret += this.Step.OutputTableName + " (";

                        DataTable inputSchema = manager.BatchProvider.GetTableDetails(manager.GetCampaignTable(manager.GetStep(this.Step.Inputs[0]).OutputTableId.GetValueOrDefault(-1)).Name, false);

                        for (int i = 0; i < inputSchema.Rows.Count; i++)
                        {
                            if (inputSchema.Rows[i]["FieldName"].ToString().ToLower() != keyField.ToLower())
                            {
                                additionalFields.Add(inputSchema.Rows[i]["FieldName"].ToString());
                            }
                        }
                        ret += keyField;
                        if (additionalFields.Count > 0)
                        {
                            ret += ", ";
                        }
                        additionalFields.Sort();
                        for (int ii = 0; ii < additionalFields.Count; ii++)
                        {
                            ret += additionalFields[ii];
                            if (ii < additionalFields.Count - 1)
                            {
                                ret += ", ";
                            }
                        }
                        ret += ")";
                    }
				}
				else
				{
					ret += "<Output Table> (Connect this step to an input source to view the table and field list)";
				}
				return ret + "\r\nSELECT";
			}
			set { }
		}



		[System.Xml.Serialization.XmlElement(/*Order = 8*/)]
		public virtual string SqlSuggestedText
		{
			get
			{
				if (string.IsNullOrEmpty(SqlText))
				{
                    using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                    {
                        string ret = string.Empty;
                        string keyField = string.Empty;
                        List<string> additionalFields = new List<string>();

                        if (this.Key != null)
                        {
                            keyField = this.Key.FieldName;
                        }

                        if (this.Step != null && this.Step.Inputs.Count > 0)
                        {
                            CampaignTable inputTable = manager.GetCampaignTable(manager.GetStep(this.Step.Inputs[0]).OutputTableId.GetValueOrDefault(-1));
                            if (inputTable != null)
                            {
                                DataTable inputSchema = manager.BatchProvider.GetTableDetails(inputTable.Name, false);
                                for (int i = 0; i < inputSchema.Rows.Count; i++)
                                {
                                    if (inputSchema.Rows[i]["FieldName"].ToString().ToLower() != keyField.ToLower())
                                    {
                                        additionalFields.Add(inputSchema.Rows[i]["FieldName"].ToString());
                                    }
                                }
                                ret += "\t" + keyField;
                                if (additionalFields.Count > 0)
                                {
                                    ret += ", \r\n";
                                }
                                else
                                {
                                    ret += "\r\n";
                                }
                                additionalFields.Sort();
                                for (int ii = 0; ii < additionalFields.Count; ii++)
                                {
                                    ret += "\t" + additionalFields[ii];
                                    if (ii < additionalFields.Count - 1)
                                    {
                                        ret += ", ";
                                    }
                                    ret += "\r\n";
                                }

                                ret += "FROM\r\n\t" + "##input##"; //inputTable.Name;
                            }
                            return ret;
                        }
                    }
				}
				return null;
			}
			set { }
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

                //give the data provider the name of each table, rather than the internal id (which is useless to the data provider)
                foreach (QueryColumn column in Columns)
                {
                    if (column.TableId > 0)
                    {
                        CampaignTable table = manager.GetCampaignTable(column.TableId);
                        if (table.ResidesInAlternateSchema)
                        {
                            column.TableName = altSchemaName + table.Name;
                        }
                        else
                        {
                            column.TableName = table.Name; //manager.GetCampaignTable(column.TableID).Name;
                        }
                    }
                    if (column.ColumnType == ColumnType.Append && column.Conditions.Count > 0)
                    {
                        foreach (ColumnCondition c in column.Conditions)
                        {
                            if (c.AssignmentTableId.GetValueOrDefault(0) > 0 && !string.IsNullOrEmpty(c.AssignmentFieldName))
                            {
                                CampaignTable table = manager.GetCampaignTable(c.AssignmentTableId.Value);

                                if (table.ResidesInAlternateSchema)
                                {
                                    c.AssignmentTableName = altSchemaName + table.Name;
                                }
                                else
                                {
                                    c.AssignmentTableName = table.Name;
                                }
                            }
                        }
                    }
                }

                string sql = string.Empty;
                if (IsValidationTest)
                {
                    SqlQuery query = new SqlQuery() { Limit = 0 };

                    DataTable inputSchema = manager.BatchProvider.GetTableDetails(manager.GetCampaignTable(manager.GetStep(this.Step.Inputs[0]).OutputTableId.GetValueOrDefault(-1)).Name, false);

                    for (int i = 0; i < inputSchema.Rows.Count; i++)
                    {
                        query.InsertFieldList.Add(inputSchema.Rows[i]["FieldName"].ToString());
                    }

                    sql += manager.BatchProvider.QueryManagerMap[manager.BatchProvider.DatabaseType].GetLimitQueryStart(query);
                    sql += (string.IsNullOrEmpty(SqlText) ? SqlSuggestedText : SqlText);
                    sql += manager.BatchProvider.QueryManagerMap[manager.BatchProvider.DatabaseType].GetLimitQueryEnd(query);
                }
                else
                {
                    sql = SqlBeginText + "\r\n" + (string.IsNullOrEmpty(SqlText) ? SqlSuggestedText : SqlText);
                }

                if (this.Step != null && this.Step.Inputs.Count > 0)
                {
                    string inputTableName = manager.GetStep(this.Step.Inputs[0]).OutputTableName;

                    if (sql.Contains("##input##") && !string.IsNullOrEmpty(inputTableName))
                    {
                        sql = sql.Replace("##input##", inputTableName);
                    }
                }
                return new List<SqlStatement> { new SqlStatement(sql) };
            }
		}



	}
}