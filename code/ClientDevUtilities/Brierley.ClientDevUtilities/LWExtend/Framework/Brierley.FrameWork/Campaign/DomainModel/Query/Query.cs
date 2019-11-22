using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Brierley.FrameWork.CampaignManagement.DataProvider;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	/// <summary>
	/// </summary>
	[Serializable]

	[XmlInclude(typeof(AssignmentQuery))]
	[KnownType(typeof(AssignmentQuery))]

	[XmlInclude(typeof(ChangeAudienceQuery))]
	[KnownType(typeof(ChangeAudienceQuery))]

	[XmlInclude(typeof(ControlGroupQuery))]
	[KnownType(typeof(ControlGroupQuery))]

	[XmlInclude(typeof(DedupeQuery))]
	[KnownType(typeof(DedupeQuery))]

	[XmlInclude(typeof(OutputQuery))]
	[KnownType(typeof(OutputQuery))]

	[XmlInclude(typeof(SelectQuery))]
	[KnownType(typeof(SelectQuery))]

	[XmlInclude(typeof(RealTimeSelectQuery))]
	[KnownType(typeof(RealTimeSelectQuery))]

	[XmlInclude(typeof(SplitProcessQuery))]
	[KnownType(typeof(SplitProcessQuery))]

	[XmlInclude(typeof(SqlTextQuery))]
	[KnownType(typeof(SqlTextQuery))]

	[XmlInclude(typeof(MergeQuery))]
	[KnownType(typeof(MergeQuery))]

	[XmlInclude(typeof(bScriptQuery))]
	[KnownType(typeof(bScriptQuery))]

	[XmlInclude(typeof(RealTimeInputQuery))]
	[KnownType(typeof(RealTimeInputQuery))]

	[XmlInclude(typeof(RealTimeAssignmentQuery))]
	[KnownType(typeof(RealTimeAssignmentQuery))]

	[XmlInclude(typeof(RealTimeOutputQuery))]
	[KnownType(typeof(RealTimeOutputQuery))]

	[XmlInclude(typeof(SurveyQuery))]
	[KnownType(typeof(SurveyQuery))]

	[XmlInclude(typeof(CampaignQuery))]
	[KnownType(typeof(CampaignQuery))]

	[XmlInclude(typeof(WaitQuery))]
	[KnownType(typeof(WaitQuery))]

	[XmlInclude(typeof(RealTimebScriptQuery))]
	[KnownType(typeof(RealTimebScriptQuery))]

	[XmlInclude(typeof(PivotQuery))]
	[KnownType(typeof(PivotQuery))]


	//[DataContract(Namespace = "http://www.brierley.com/CampaignWare/CampaignService")]
	public class Query
	{
		protected Tree<TableKey> _joinRoutes = new Tree<TableKey>();
		protected OutputOption _outputOption = null;
		protected const string _oracleProvider = "OracleODP-2.0";
		private CampaignTable _rootTable = null;
		private string _altSchemaName = null;

		[XmlIgnore]
		public bool SerializingToDatabase = false;

		public const string RealTimeAssignedValuesAlias = "Assigned Values";

		/// <summary>
		/// Gets or sets the IsGlobal for the current Query
		/// </summary>
		//[System.Xml.Serialization.XmlElement(/*Order = 5*/)]
		[DataMember]
		public virtual bool IsGlobal { get; set; }

		//[System.Xml.Serialization.XmlElement(/*Order = 9*/)]
		[DataMember]
		public virtual int? RowLimit { get; set; }

		//[System.Xml.Serialization.XmlElement(/*Order = 10*/)]
		[DataMember]
		public virtual bool IsLimitPercentage { get; set; }

		//[System.Xml.Serialization.XmlElement(/*Order = 11*/)]
		[DataMember]
		public virtual bool RandomSample { get; set; }

		[System.Xml.Serialization.XmlIgnore]
		[IgnoreDataMember]
		public virtual Step Step { get; set; }

		//[System.Xml.Serialization.XmlElement(/*Order = 14*/)]
		[DataMember]
		public virtual DateTime CreateDate { get; set; }


		protected TableKey Key
		{
			get
			{
				if (Step != null)
				{
					return Step.Key;
				}
				return null;
			}
		}


		//[System.Xml.Serialization.XmlElement(/*Order = 18,*/ Type = typeof(List<QueryColumn>))]
		[DataMember]
		public virtual List<QueryColumn> Columns { get; set; }


		//[System.Xml.Serialization.XmlElement(/*Order=19*/)]
		[DataMember]
		public virtual bool DistinctRows { get; set; }

		[XmlIgnore]
		[IgnoreDataMember]
		protected string AlternateSchemaName
		{
			get
			{
				if (_altSchemaName == null)
				{
                    using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                    {
                        string altSchemaName = manager.BatchProvider.DataSchemaPrefix;
                        if (!string.IsNullOrEmpty(altSchemaName))
                        {
                            if (!altSchemaName.EndsWith("."))
                            {
                                altSchemaName += ".";
                            }
                        }
                        _altSchemaName = altSchemaName;
                    }
				}
				return _altSchemaName;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		protected CampaignTable RootTable
		{
			get
			{
				if (_rootTable == null && Key != null)
				{
                    using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                        _rootTable = manager.GetCampaignTable(Key.TableId);

					if (_rootTable.ResidesInAlternateSchema)
					{
						_rootTable.Name = AlternateSchemaName + _rootTable.Name;
					}
				}
				return _rootTable;
			}
		}

		// IgnoreDataMember is not working, presumably because the service is configured with the XmlSerializerFormat attribute instead of using DataContractSerializer.
		// Changing it will lead to exceptions about our elements appearing out of order, which appears to be related somehow to Step inheriting from LWCoreObjectBase.
		// Leaving it like this is not a major problem: we're unable to exclude this property, so GetStepDetails is returning it. It's just extra data that we don't need.
		[IgnoreDataMember]
		public virtual List<Join> JoinHints { get; set; }


		/// <summary>
		/// Initializes a new instance of the Query class
		/// </summary>
		public Query()
		{
			Columns = new List<QueryColumn>();
		}


		/// <summary>
		/// Assembles the SQL statement based on the parameters of the query.
		/// </summary>
		/// <returns></returns>
		public virtual List<SqlStatement> GetSqlStatement(Dictionary<string, string> overrideParameters = null)
		{
			return GetSqlStatement(false, overrideParameters);
		}

		public /*abstract*/ virtual List<SqlStatement> GetSqlStatement(bool IsValidationTest, Dictionary<string, string> overrideParameters = null)
		{
			return null;
		}

		/// <summary>
		/// Assembles the SQL statement for the verification (commit) process of the step
		/// </summary>
		/// <returns></returns>
		public virtual List<SqlStatement> GetVerifySqlStatement()
		{
			return null;
		}



		/// <summary>
		/// Maps necessary table joins to the query's root table.
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		protected internal virtual void MapTableJoins(SqlQuery SqlQuery, List<QueryColumn> transientColumns = null)
		{
			SqlQuery.Joins = GetJoins(transientColumns);
		}

		public virtual List<Join> GetJoins(List<QueryColumn> transientColumns = null)
		{
			var joins = new List<Join>();

            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                string altSchemaName = null;
                if (!Step.IsRealTimeStep())
                {
                    altSchemaName = manager.BatchProvider.DataSchemaPrefix;
                }
                if (!string.IsNullOrEmpty(altSchemaName))
                {
                    if (!altSchemaName.EndsWith("."))
                    {
                        altSchemaName += ".";
                    }
                }

                List<long> joinedTables = new List<long>();
                IList<TableKey> rootKeys = manager.GetTableKeyByTable(Key.TableId);

                List<long> tableIds = new List<long>();

                IList<CampaignTable> tables = manager.GetAllCampaignTables(new TableType[] { TableType.Framework, TableType.Output });
                IList<Attribute> parameters = manager.GetAllAttributes();

                var columns = new List<QueryColumn>();
                if (Columns != null && Columns.Count > 0)
                {
                    columns.AddRange(Columns);
                }
                if (transientColumns != null && transientColumns.Count > 0)
                {
                    columns.AddRange(transientColumns);
                }

                foreach (QueryColumn column in columns)
                {
                    if (string.IsNullOrEmpty(column.ColumnExpression) && column.TableId > 0 && !tableIds.Contains(column.TableId))
                    {
                        tableIds.Add(column.TableId);
                    }
                    if (!string.IsNullOrWhiteSpace(column.ColumnExpression) || column.ColumnType == ColumnType.Append)
                    {
                        //get tables, fields and parameters from the expression
                        var fields = EvaluateFields(tables, parameters, column);
                        foreach (var t in fields.Fields.Keys)
                        {
                            if (!tableIds.Contains(t.Id))
                            {
                                tableIds.Add(t.Id);
                            }
                        }

                        //convert aliases to actual table and field names
                        string inputStepName = null;
                        string inputTableName = null;
                        Step inputStep = null;
                        if (Step.InputCount == 1)
                        {
                            inputStep = manager.GetStep(Step.Inputs[0]);
                            inputStepName = inputStep.UIName;
                            inputTableName = inputStep.OutputTableName;
                        }
                        ConvertFromAliases(fields, column, inputStepName, inputTableName);
                    }
                    if (column.ColumnType == ColumnType.Append)
                    {
                        foreach (ColumnCondition condition in column.Conditions)
                        {
                            long assignmentId = condition.AssignmentTableId.GetValueOrDefault(0);
                            if (assignmentId > 0 && !string.IsNullOrEmpty(condition.AssignmentFieldName) && !tableIds.Contains(assignmentId))
                            {
                                tableIds.Add(assignmentId);
                            }
                        }
                    }
                }

                const int deferLimit = 5;
                for (int deferred = 0; deferred <= deferLimit; deferred++)
                {
                    foreach (long tableId in tableIds)
                    {
                        if (tableId > 0 && tableId != Key.TableId && !joinedTables.Contains(tableId))
                        {
                            bool joinFound = false;
                            IList<TableKey> keys = manager.GetTableKeyByTable(tableId);
                            //check the keys of the table to be joined. If a key with an audience level matching the root table is found, then a 
                            //join can be created between the table and the root table.
                            foreach (TableKey key in keys)
                            {
                                foreach (TableKey rootKey in rootKeys)
                                {
                                    if (key.AudienceId == rootKey.AudienceId)
                                    {
                                        //easy join, both tables match on audience
                                        CampaignTable table = manager.GetCampaignTable(tableId);
                                        joinedTables.Add(tableId);
                                        Join join = new Join();
                                        join.TableName = (table.ResidesInAlternateSchema ? altSchemaName : string.Empty) + table.Name;
                                        join.FieldName = key.FieldName;
                                        join.JoinToTableName = RootTable.Name;
                                        join.JoinToFieldName = rootKey.FieldName;
                                        join.JoinType = GetJoinHintOrDefault(join.TableName, join.FieldName, join.JoinToTableName, join.JoinToFieldName);
                                        joins.Add(join);
                                        joinFound = true;
                                        break;
                                    }
                                }

                                if (joinFound) continue;

                                //check joinedTables (tables already joined) for keys that may match.
                                foreach (long joinId in joinedTables.ToList())
                                {
                                    IList<TableKey> joinedTableKeys = manager.GetTableKeyByTable(joinId);
                                    foreach (TableKey joinedKey in joinedTableKeys)
                                    {
                                        if (key.AudienceId == joinedKey.AudienceId)
                                        {
                                            //easy join, both tables match on audience
                                            CampaignTable joinedTable = manager.GetCampaignTable(joinId);
                                            CampaignTable table = manager.GetCampaignTable(tableId);
                                            joinedTables.Add(tableId);
                                            Join join = new Join();
                                            join.TableName = (table.ResidesInAlternateSchema ? altSchemaName : string.Empty) + table.Name;
                                            join.FieldName = key.FieldName;
                                            join.JoinToTableName = (joinedTable.ResidesInAlternateSchema ? altSchemaName : string.Empty) + joinedTable.Name;
                                            join.JoinToFieldName = joinedKey.FieldName;
                                            join.JoinType = GetJoinHintOrDefault(join.TableName, join.FieldName, join.JoinToTableName, join.JoinToFieldName);
                                            joins.Add(join);
                                            joinFound = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!joinFound && deferred == deferLimit)
                            {
                                //The root table and the table to be joined do not match on audience level. We need to find the route back 
                                //to the root table. Scan all other framework tables to see if any one table shares keys between the two.
                                List<TableKey> routeList = GetTableJoinRouteFast(tableId);
                                if (routeList == null)
                                {
                                    //no table exists sharing keys between the two. Build a tree of all table keys and find the join route.
                                    routeList = GetTableJoinRouteTree(tableId);
                                }

                                if (routeList != null)
                                {
                                    for (int i = 0; i < routeList.Count; i++)
                                    {
                                        TableKey routeKey = routeList[i];
                                        if (!joinedTables.Contains(routeKey.TableId) && routeKey.TableId != Key.TableId)
                                        {
                                            joinedTables.Add(routeKey.TableId);
                                            CampaignTable table = manager.GetCampaignTable(routeKey.TableId);
                                            Join join = new Join();
                                            join.TableName = (table.ResidesInAlternateSchema ? altSchemaName : string.Empty) + table.Name;
                                            join.FieldName = routeKey.FieldName;

                                            if (i > 0 && routeList[i - 1].TableId != routeKey.TableId)
                                            {
                                                join.JoinToTableName = manager.GetCampaignTable(routeList[i - 1].TableId).Name;
                                            }
                                            else
                                            {
                                                join.JoinToTableName = RootTable.Name;
                                            }

                                            if (i > 0)
                                            {
                                                join.JoinToFieldName = routeList[i - 1].FieldName;
                                            }
                                            else
                                            {
                                                join.JoinToFieldName = Key.FieldName;
                                            }
                                            join.JoinType = GetJoinHintOrDefault(join.TableName, join.FieldName, join.JoinToTableName, join.JoinToFieldName);
                                            joins.Add(join);
                                        }
                                    }
                                }
                                else
                                {
                                    string tableName = string.Empty;
                                    foreach (QueryColumn column in Columns)
                                    {
                                        if (column.TableId == tableId)
                                        {
                                            tableName = column.TableName;
                                            break;
                                        }
                                        foreach (ColumnCondition condition in column.Conditions)
                                        {
                                            if (condition.AssignmentTableId.GetValueOrDefault(0) == tableId)
                                            {
                                                tableName = condition.AssignmentTableName;
                                                break;
                                            }
                                        }
                                        if (tableName != string.Empty)
                                        {
                                            break;
                                        }
                                    }
                                    throw new Exception(String.Format("Failed to map table joins. Table {0} cannot be mapped to {1}", tableName, RootTable.Name));
                                }
                            }
                        }
                    }
                }
            }
			return joins;
		}

		/// <summary>
		/// Attempts to find a join route between two tables that do not share a common key by locating a third
		/// table containing two keys that can used to join as the middle table between the two. For example, if
		/// a query is attempting to use LW_LoyaltyMember as the root table and the query lists criteria on LW_Tiers, 
		/// this method would locate LW_MemberTiers and join the tables as:
		/// SELECT ...
		/// FROM
		///		LW_LoyaltyMember 
		///		LEFT JOIN LW_MemberTiers ON LW_MemberTiers.MemberId = LW_LoyaltyMember.IPCODE 
		///		LEFT JOIN LW_Tiers ON LW_Tiers.TierId = LW_MemberTiers.TierId
		/// 
		/// This is the faster of the two table mapping methods and is therefore always called first.
		/// </summary>
		/// <param name="tableId"></param>
		/// <returns></returns>
		private List<TableKey> GetTableJoinRouteFast(long tableId)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                List<TableKey> route = new List<TableKey>();

                IList<TableKey> rootKeys = manager.GetTableKeyByTable(Key.TableId);
                IList<TableKey> joinKeys = manager.GetTableKeyByTable(tableId);
                IList<TableKey> middleKeys = null;

                IList<CampaignTable> tables = manager.GetAllCampaignTables(new TableType[] { TableType.Framework, TableType.Output });
                foreach (CampaignTable table in tables)
                {
                    if (table.Id == Key.TableId || table.Id == tableId)
                    {
                        continue;
                    }

                    bool matchesRoot = false;
                    bool matchesJoin = false;

                    middleKeys = manager.GetTableKeyByTable(table.Id);
                    if (middleKeys.Count < 2)
                    {
                        //it is already known that the root table and middle table do not share a common key. Therefore, the
                        //third table in the join must contain at least 2 keys in order to be a candidate to join between.
                        continue;
                    }

                    foreach (TableKey rootKey in rootKeys)
                    {
                        foreach (TableKey middleKey in middleKeys)
                        {
                            if (middleKey.AudienceId == rootKey.AudienceId)
                            {
                                route.Add(rootKey);
                                route.Add(middleKey);
                                matchesRoot = true;
                                break;
                            }
                        }
                        if (matchesRoot)
                        {
                            break;
                        }
                    }

                    if (matchesRoot)
                    {
                        foreach (TableKey joinKey in joinKeys)
                        {
                            foreach (TableKey middleKey in middleKeys)
                            {
                                if (joinKey.AudienceId == middleKey.AudienceId)
                                {
                                    route.Add(middleKey);
                                    route.Add(joinKey);
                                    matchesJoin = true;
                                    break;
                                }
                            }
                            if (matchesJoin)
                            {
                                break;
                            }
                        }
                    }

                    if (matchesRoot && matchesJoin)
                    {
                        break;
                    }
                    else
                    {
                        route.Clear();
                    }
                }

                if (route.Count > 0)
                {
                    return route;
                }
                else
                {
                    return null;
                }
            }
		}


		/// <summary>
		/// Builds a tree of all framework tables from the root table in an attempt to map the provided table id back 
		/// to the query's root table.
		/// </summary>
		/// <remarks>
		/// This method should only be called in cases where a table is linked more than 2 joins away from the root 
		/// table. In most cases, the join will be picked up either directly (the root table and the joined table each
		/// have a key that matches the other's on audience id) or with a single table between them (handled by 
		/// GetTableJoinRouteFast).
		/// </remarks>
		/// <param name="tableID"></param>
		/// <returns></returns>
		private List<TableKey> GetTableJoinRouteTree(long tableID)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                if (_joinRoutes == null || _joinRoutes.Value == null || _joinRoutes.Children.Count == 0)
                {
                    _joinRoutes = new Tree<TableKey>(Key);
                    List<long> includedTables = new List<long>();
                    includedTables.Add(Key.TableId);

                    IList<TableKey> rootKeys = manager.GetTableKeyByTable(Key.TableId);
                    foreach (TableKey rootKey in rootKeys)
                    {
                        _joinRoutes.Children.Add(rootKey);
                    }

                    foreach (TreeNode<TableKey> node in _joinRoutes.Children)
                    {
                        MapJoins(node, includedTables, tableID);
                    }
                }

                TreeNode<TableKey> endNode = SearchTree(tableID, _joinRoutes.Root);
                if (endNode != null)
                {
                    List<TableKey> routeList = new List<TableKey>();
                    routeList.Add(endNode.Value);
                    TreeNode<TableKey> parentNode = endNode.Parent;
                    while (parentNode != null)
                    {
                        routeList.Add(parentNode.Value);
                        parentNode = parentNode.Parent;
                    }
                    routeList.Reverse();
                    return routeList;
                }
                else
                {
                    return null;
                }
            }
		}


		private TreeNode<TableKey> SearchTree(long tableId, TreeNode<TableKey> node)
		{
			foreach (TreeNode<TableKey> child in node.Children)
			{
				if (child.Value.TableId == tableId)
				{
					return child;
				}
				else
				{
					TreeNode<TableKey> node2 = SearchTree(tableId, child);
					if (node2 != null)
					{
						return node2;
					}
				}
			}
			return null;
		}



		protected internal virtual void MapJoins(TreeNode<TableKey> node, List<long> includedTables, long tableId)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                List<TableKey> keys = manager.GetTableKeyByAudience(node.Value.AudienceId);
                foreach (TableKey key in keys)
                {
                    if (!includedTables.Contains(key.TableId))
                    {
                        if (manager.GetCampaignTable(key.TableId).TableType != TableType.Step)
                        {
                            includedTables.Add(key.TableId);
                            TreeNode<TableKey> childNode = new TreeNode<TableKey>(key);
                            node.Children.Add(childNode);
                        }
                    }
                }

                foreach (TreeNode<TableKey> childNode in node.Children)
                {
                    keys = manager.GetTableKeyByTable(childNode.Value.TableId);
                    foreach (TableKey key in keys)
                    {
                        TreeNode<TableKey> childNode2 = new TreeNode<TableKey>(key);
                        childNode.Children.Add(childNode2);
                        MapJoins(childNode2, includedTables, tableId);
                    }
                }
            }
		}


		protected TableJoinType GetJoinHintOrDefault(string tableName, string fieldName, string joinToTableName, string joinToFieldName, TableJoinType defaultJoinType = TableJoinType.Left)
		{
			TableJoinType ret = defaultJoinType;
			if (JoinHints != null)
			{
				var thisHint = JoinHints.Where(
					o =>
						o.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase) &&
						o.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase) &&
						o.JoinToTableName.Equals(joinToTableName, StringComparison.OrdinalIgnoreCase) &&
						o.JoinToFieldName.Equals(joinToFieldName, StringComparison.OrdinalIgnoreCase)
				).FirstOrDefault();
				if (thisHint != null)
				{
					ret = thisHint.JoinType;
				}
			}
			return ret;
		}


		public virtual bool Validate(List<ValidationMessage> warnings, bool validateSql)
		{
			bool fatal = false;

			if (Step == null)
			{
				warnings.Add(new ValidationMessage(ValidationLevel.Exception, "No step exists for query " + Step.UIName + ". An input step must be assigned"));
				fatal = true;
			}

			if (validateSql && !fatal)
			{
				//ensure schema exists and is valid.
				try
				{
					EnsureSchema();
				}
				catch (Exception ex)
				{
					warnings.Add(new ValidationMessage(ValidationLevel.Exception, "Failed to ensure schema of output table for query " + Step.UIName + ": " + ex.Message));
					fatal = true;
				}

				if (!fatal)
				{
					string sqlErrors = string.Empty;
                    using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                    {
                        var provider = Step.IsRealTimeStep() ? manager.RealTimeProvider : manager.BatchProvider;
                        if (!provider.IsValidSqlStatement(GetSqlStatement(true), null, ref sqlErrors))
                        {
                            warnings.Add(new ValidationMessage(ValidationLevel.Exception, "SQL statement validation failed for " + Step.UIName + ": " + sqlErrors));
                            fatal = true;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(sqlErrors))
                            {
                                warnings.Add(new ValidationMessage(ValidationLevel.Warning, "SQL statement validation failed for " + Step.UIName + ": " + sqlErrors));
                            }
                        }
                    }
				}
			}
			return !fatal;
		}


		public virtual void EnsureSchema()
		{
			if (Step.StepType == StepType.Output)
			{
				//output steps do not create their table here.
				return;
			}

			if (Step.IsRealTimeStep())
			{
				//no schema for real time steps.
				return;
			}

            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                //check for recent method call that resulted in the schema being up to date
                bool ensured = manager.CacheManager.Get(Constants.CacheRegions.StepSchemaById, this.Step.Id) != null;

                string tableName = null;
                string fieldName = null;
                string fieldType = null;

                if (ensured)
                {
                    foreach (long inputID in this.Step.Inputs)
                    {
                        Step step = manager.GetStep(inputID);
                        step.Query.EnsureSchema();
                    }
                }
                else
                {
                    //list to track all fields identified that are needed in the table. Will later be checked against
                    //all fields that currently exist in the table. Existing fields that are not needed will be dropped.
                    List<string> requiredFields = new List<string>();

                    if (Step.NeedsOutputTable())
                    {
                        //tableName = manager.GetCampaignTable((long)Step.OutputTableID).Name; <-- save the database call by using line below instead:
                        tableName = Constants.TempTableNamePrefix + Step.OutputTableId.GetValueOrDefault(0).ToString();
                        if (Step.StepType == StepType.ChangeAudience)
                        {
                            var q = Step.Query as ChangeAudienceQuery;
                            TableKey convertToKey = q.ConvertToKey;
                            if (convertToKey == null)
                            {
                                throw new Exception("Failed to ensure table schema. The change audience step " + Step.UIName + " has not been provided with a new audience level.");
                            }
                            fieldName = convertToKey.FieldName;
                            fieldType = convertToKey.FieldType;
                        }
                        else
                        {
                            fieldName = Key.FieldName;
                            fieldType = Key.FieldType;
                        }

                        if (!manager.BatchProvider.TableExists(tableName, false))
                        {
                            manager.BatchProvider.CreateTable(tableName, fieldName, fieldType);
                        }

                        if (!manager.BatchProvider.FieldExists(tableName, fieldName, false))
                        {
                            manager.BatchProvider.DropTable(tableName);
                            EnsureSchema();
                            return;
                        }

                        requiredFields.Add(fieldName.ToLower());

                        //todo: this is a slightly lame implementation. We need to discover all required keys
                        //for the output table, i.e., if a user switches through 5 different audience levels, we
                        //need to track back up the chain of steps and get each key, then append the key to the table.
                        if (Step.StepType == StepType.ChangeAudience)
                        {
                            if (!manager.BatchProvider.FieldExists(tableName, Key.FieldName, false))
                            {
                                manager.BatchProvider.AddFieldToTable(tableName, Key.FieldName, Key.FieldType);
                            }
                            requiredFields.Add(Key.FieldName.ToLower());
                        }

                        string currentType = manager.BatchProvider.GetFieldType(tableName, fieldName, false);
                        if (currentType.Contains("("))
                        {
                            currentType = currentType.Substring(0, currentType.IndexOf("("));
                        }
                        if (fieldType.Contains("("))
                        {
                            fieldType = fieldType.Substring(0, fieldType.IndexOf("("));
                        }
                        if (currentType.Trim().ToLower() != fieldType.Trim().ToLower())
                        {
                            manager.BatchProvider.DropTable(tableName);
                            EnsureSchema();
                            return;
                        }

                        //additional fields from segments / updates (full output table needs full field list to exist in the destination table)
                        foreach (QueryColumn column in Columns)
                        {
                            if ((column.ColumnType == ColumnType.Segment || column.ColumnType == ColumnType.Append) && !string.IsNullOrEmpty(column.FieldName))
                            {
                                if (!manager.BatchProvider.FieldExists(tableName, column.FieldName, false))
                                {
                                    string dataType = string.Empty;
                                    if (column.ColumnType == ColumnType.Append && column.Conditions != null & column.Conditions.Count > 0)
                                    {
                                        foreach (ColumnCondition condition in column.Conditions)
                                        {
                                            string conditionType = string.Empty;



                                            IList<CampaignTable> tables = manager.GetAllCampaignTables(new TableType[] { TableType.Framework, TableType.Output });
                                            IList<Attribute> parameters = manager.GetAllAttributes();
                                            var fields = EvaluateFields(tables, parameters, column);
                                            if (fields != null && fields.Fields.Keys.Count == 1)
                                            {

                                                //if (condition.AssignmentTableId.HasValue && !string.IsNullOrEmpty(condition.AssignmentFieldName))
                                                //{
                                                //CampaignTable conditionTable = manager.GetCampaignTable(condition.AssignmentTableId.Value);
                                                var conditionTable = fields.Fields.Keys.First();
                                                var conditionFields = fields.Fields[conditionTable];
                                                if (conditionFields != null && conditionFields.Count() == 1)
                                                {
                                                    conditionType = manager.BatchProvider.GetFieldType(conditionTable.Name, conditionFields.First().Name, conditionTable.ResidesInAlternateSchema);
                                                }
                                            }
                                            if (!string.IsNullOrEmpty(dataType))
                                            {
                                                if (dataType != conditionType)
                                                {
                                                    //multiple conditions exist that do not match on data type. Type will default to varchar
                                                    dataType = string.Empty;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                dataType = conditionType;
                                            }
                                        }

                                    }
                                    manager.BatchProvider.AddFieldToTable(tableName, column.FieldName, dataType);
                                }
                                requiredFields.Add(column.FieldName.ToLower());
                            }
                        }
                    }


                    //todo: remove this section and override in SplitProcessQuery with a call to base() to get all other fields.
                    //We're putting it here for now because this method automatically removes any extra fields, which would remove 
                    //the ProcessName field. We would need to change the method to either take a list of the derived class' additional
                    //fields or just stop removing them from here... or just override the entire method and not call base()
                    if (this is SplitProcessQuery)
                    {
                        var split = (SplitProcessQuery)this;
                        if (!string.IsNullOrEmpty(split.ProcessName) && !string.IsNullOrEmpty(split.ProcessValue))
                        {
                            if (!manager.BatchProvider.FieldExists(tableName, split.ProcessName, false))
                            {
                                manager.BatchProvider.AddFieldToTable(tableName, split.ProcessName, string.Empty);
                            }
                            requiredFields.Add(split.ProcessName.ToLower());
                        }
                    }



                    //append any field from input steps that don't already exist. Every field that isn't a key has its type defaulted to 
                    //varchar2(250) for Oracle or varchar(250) for sql server, so we shouldn't need to worry about type matching at this 
                    //point; just that the field exists.
                    foreach (long inputID in this.Step.Inputs)
                    {
                        Step step = manager.GetStep(inputID);
                        if (step.OutputTableId == null)
                        {
                            throw new Exception("Failed to ensure merge table schema. The input step " + inputID.ToString() + " does not have an output table defined.");
                        }
                        else
                        {
                            try
                            {
                                step.Query.EnsureSchema();
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Failed to ensure table schema of input step " + inputID.ToString(), ex);
                            }

                            if (Step.NeedsOutputTable())
                            {

                                CampaignTable table = manager.GetCampaignTable((long)step.OutputTableId);
                                DataTable inputSchema = manager.BatchProvider.GetTableDetails(table.Name, false);

                                foreach (DataRow row in inputSchema.Rows)
                                {
                                    if (Step.StepType == StepType.ChangeAudience && row["FieldName"].ToString().ToLower() == Key.FieldName.ToLower())
                                    {
                                        //field is same as old key which is being converted to a new key. The old key is not needed in the table.
                                        continue;
                                    }

                                    if (!manager.BatchProvider.FieldExists(tableName, row["FieldName"].ToString(), false))
                                    {
                                        string dataType = row["DataType"].ToString();
                                        if (Utils.DataTypeRequiresLength(dataType))
                                        {
                                            dataType += "(" + row["Length"].ToString() + ")";
                                        }

                                        manager.BatchProvider.AddFieldToTable(tableName, row["FieldName"].ToString(), dataType);
                                    }
                                    requiredFields.Add(row["FieldName"].ToString().ToLower());
                                }
                            }
                        }
                    }

                    //delete fields that are not required
                    if (requiredFields.Count > 0 && !string.IsNullOrEmpty(tableName))
                    {
                        DataTable outputSchema = manager.BatchProvider.GetTableDetails(tableName, false);

                        foreach (DataRow row in outputSchema.Rows)
                        {
                            bool required = false;
                            foreach (string requiredField in requiredFields)
                            {
                                if (requiredField == row["FieldName"].ToString().ToLower())
                                {
                                    required = true;
                                    break;
                                }
                            }
                            if (!required)
                            {
                                manager.BatchProvider.RemoveFieldFromTable(tableName, row["FieldName"].ToString());
                            }
                        }
                    }
                    manager.CacheManager.Update(Constants.CacheRegions.StepSchemaById, Step.Id, true);
                }
            }
		}


		internal virtual List<CampaignResult> Execute(ContextObject co = null, Dictionary<string, string> overrideParameters = null, bool resume = false)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                List<ValidationMessage> warnings = new List<ValidationMessage>();
                if (!Validate(warnings, true))
                {
                    string exception = "Failed to execute " + Step.UIName + " because the step is invalid.";
                    foreach (ValidationMessage message in warnings)
                    {
                        exception += message.Message;
                    }
                    throw new Exception(exception);
                }

                if (Step != null && Step.NeedsOutputTable())
                {
                    manager.BatchProvider.TruncateTable(manager.GetCampaignTable((long)Step.OutputTableId).Name);
                }

                int rowCount = 0;
                foreach (SqlStatement sql in GetSqlStatement(overrideParameters))
                {
                    int statementCount = manager.BatchProvider.Execute(sql, GetRealTimeParameters());
                    if (sql.ApplyToResults)
                    {
                        rowCount += statementCount;
                    }
                }
                return new List<CampaignResult> { new CampaignResult(rowCount) };
            }
		}



		public virtual int Verify(ContextObject co = null, Dictionary<string, string> overrideParameters = null)
		{
			return 0;
		}


		public virtual DataTable GetDataSample(string[] groupBy)
		{
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                SqlQuery query = new SqlQuery();
                query.Columns = new List<QueryColumn>();
                query.StepType = StepType.Output;
                query.Limit = 200;

                DataTable outputSchema = manager.BatchProvider.GetTableDetails(this.Step.OutputTableName, false);

                if (groupBy == null || groupBy.Length == 0)
                {
                    for (int i = 0; i < outputSchema.Rows.Count; i++)
                    {
                        query.Columns.Add(new QueryColumn()
                        {
                            ColumnType = ColumnType.Condition,
                            FieldName = outputSchema.Rows[i]["FieldName"].ToString(),
                            TableName = Step.OutputTableName,
                            IncludeInOutput = true
                        });
                    }
                }
                else
                {
                    query.Columns.Add(new QueryColumn()
                    {
                        ColumnType = ColumnType.Condition,
                        TableName = this.Step.OutputTableName,
                        FieldName = this.Step.Key.FieldName,
                        AggregateExpression = "count",
                        IncludeInOutput = true
                    });

                    foreach (string group in groupBy)
                    {
                        for (int i = 0; i < outputSchema.Rows.Count; i++)
                        {
                            if (outputSchema.Rows[i]["FieldName"].ToString() == group)
                            {
                                query.Columns.Add(new QueryColumn()
                                {
                                    ColumnType = ColumnType.Condition,
                                    FieldName = outputSchema.Rows[i]["FieldName"].ToString(),
                                    TableName = Step.OutputTableName,
                                    IncludeInOutput = true
                                });
                                break;
                            }
                        }
                    }
                }
                query.RootTableName = this.Step.OutputTableName;

                if (groupBy != null)
                {
                    query.OrderBy = new List<string>();
                    query.GroupBy = new List<string>();
                    foreach (string group in groupBy)
                    {
                        query.GroupBy.Add(group);
                        query.OrderBy.Add(group);
                    }
                }

                return manager.BatchProvider.ExecuteDataTable(manager.BatchProvider.CreateSqlStatement(query)[0]);
            }
		}


		protected internal virtual string ApplyEscapeSequences(string Value)
		{
			if (string.IsNullOrEmpty(Value))
			{
				return string.Empty;
			}

			Dictionary<string, string> sequences = new Dictionary<string, string>();
			sequences.Add("\\r", "\r");
			sequences.Add("\\n", "\n");
			sequences.Add("\\'", "\'");
			sequences.Add("\\\"", "\"");
			sequences.Add("\\0", "\0");
			sequences.Add("\\a", "\a");
			sequences.Add("\\b", "\b");
			sequences.Add("\\f", "\f");
			sequences.Add("\\t", "\t");
			sequences.Add("\\v", "\v");

			foreach (string sequence in sequences.Keys)
			{
				Value = Value.Replace(sequence, sequences[sequence]);
			}
			return Value;
		}


		protected internal virtual string GetTempFileName()
		{
			string fileName = "CampaignOutput_" + DateTime.Now.ToString("ddmmyyyyfff");
			string currentDir = Brierley.FrameWork.Common.IO.IOUtils.GetTempDirectory();
			if (!currentDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				currentDir += Path.DirectorySeparatorChar;
			}

			while (File.Exists(currentDir + Path.DirectorySeparatorChar + fileName))
			{
				fileName = "CampaignOutput_" + DateTime.Now.ToString("ddmmyyyyfff");
			}
			return currentDir + Path.DirectorySeparatorChar + fileName;
		}

		protected virtual void ApplyTableNames()
		{
            using (var manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                string altSchemaName = manager.BatchProvider.DataSchemaPrefix;
                if (!string.IsNullOrEmpty(altSchemaName) && !altSchemaName.EndsWith("."))
                {
                    altSchemaName += ".";
                }

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
                            column.TableName = table.Name;
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
            }
		}

		protected virtual void AddRootTableCarryoverFields(SqlQuery query)
		{
            using (var manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                //all other fields in the source table must carry over to the destination
                if (RootTable.TableType == TableType.Step &&
                    (
                    Step.StepType == StepType.Select ||
                    Step.StepType == StepType.SplitProcess ||
                    Step.StepType == StepType.Assignment ||
                    Step.StepType == StepType.DeDupe ||
                    Step.StepType == StepType.Pivot
                    ))
                {
                    DataTable additionalFields = manager.BatchProvider.GetTableDetails(RootTable.Name, false);
                    foreach (DataRow row in additionalFields.Rows)
                    {
                        if (
                            !query.InsertFieldList.Contains(row["FieldName"].ToString().ToLower()) &&
                            !query.SelectFieldList.Contains(RootTable.Name.ToLower() + "." + row["FieldName"].ToString().ToLower()) &&
                            row["FieldName"].ToString().ToLower() != Key.FieldName.ToLower()
                            )
                        {
                            query.RootTableCarryover.Add(row["FieldName"].ToString().ToLower());
                        }
                    }
                }
            }
		}

		protected virtual void ApplyCampaignParameters(SqlStatement statement, Dictionary<string, string> overrideParameters)
		{
			if (statement == null)
			{
				throw new ArgumentNullException("statement");
			}
			if (Step.CampaignId.HasValue)
			{
                using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                {
                    var attributes = manager.GetAllAttributes();
                    if (attributes != null && attributes.Count > 0)
                    {
                        var campaignAttributes = manager.GetAllCampaignAttributes(Step.CampaignId.Value);
                        foreach (var attribute in attributes)
                        {
                            if (statement.Statement.IndexOf(attribute.ToDatabaseName(), StringComparison.OrdinalIgnoreCase) > -1)
                            {
                                string value = null;
                                if (overrideParameters != null && overrideParameters.ContainsKey(attribute.Name))
                                {
                                    value = overrideParameters[attribute.Name];
                                }
                                else
                                {
                                    value = campaignAttributes.Where(o => o.AttributeId == attribute.Id).Select(o => o.AttributeValue).FirstOrDefault();
                                }
                                statement.AddParameter(attribute.ToDatabaseName(), value);
                            }
                        }
                    }
                }
			}
		}

		public class FieldEvaluation
		{
			public Dictionary<CampaignTable, List<TableField>> Fields { get; set; }
			public List<Attribute> Parameters { get; set; }
		}

		/// <summary>
		/// Returns a list showing which fields and parameters are in use by the QueryColumn.
		/// </summary>
		/// <param name="tables"></param>
		/// <param name="parameters"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		protected internal FieldEvaluation EvaluateFields(IList<CampaignTable> tables, IList<Attribute> parameters, QueryColumn column)
		{
			FieldEvaluation ret = new FieldEvaluation();
			ret.Parameters = new List<Attribute>();
			ret.Fields = new Dictionary<CampaignTable, List<TableField>>();

			if (column.ColumnType != ColumnType.Append)
			{
				EvaluateExpression(tables, parameters, column.ColumnExpression, ret);
			}
			foreach (var condition in column.Conditions)
			{
				EvaluateExpression(tables, parameters, condition.ConditionExpression, ret);
			}

			return ret;
		}

		protected void EvaluateExpression(IList<CampaignTable> tables, IList<Attribute> parameters, string expression, FieldEvaluation fields)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				return;
			}
			string text = expression.ToLower();

			//remove literal strings
			if (text.Contains("'"))
			{
				text = System.Text.RegularExpressions.Regex.Replace(text, @"\'(?<StringLiteral>.*?)\'", string.Empty);
			}

			foreach (var p in parameters)
			{
				string param = string.Format("@\"{0}\"", p.Name);
				if (text.IndexOf(param, StringComparison.OrdinalIgnoreCase) > -1)
				{
					fields.Parameters.Add(p);
				}
			}

			if (text.Contains("."))
			{
				string[] split = text.Split('.');
				if (split.Length >= 2)
				{
					for (int i = 0; i < split.Length - 1; i++)
					{
						var table = GetTableName(split[i], tables);
						if (table != null)
						{
							/*
							 * We have a match on a table name. We now have 2 options here:
							 * 1. We can trust that the text in the array following the table name/alias entry is actually a field. We'd need to scan the text for the 
							 *    end of the field in order to get the name right (last alphanumeric character).
							 * 2. We can load the table's schema and do a double check to see if the next item is a field, trying to match it against a known field.
							 * 
							 * We have the ability to pull and cache the list fairly easily
							*/

							if (table.Fields == null)
							{
                                using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                                    table.Fields = manager.TableFieldDao.RetrieveByTable(table.Id);
							}

							//look for any field in the next position in the split
							var field = GetColumnName(split[i + 1], table.Fields);

							if (field != null)
							{
								//good, we found a  table.field combination that matches our schema, add them to the list
								if (fields.Fields.ContainsKey(table))
								{
									if (!fields.Fields[table].Contains(field))
									{
										fields.Fields[table].Add(field);
									}
								}
								else
								{
									fields.Fields.Add(table, new List<TableField>() { field });
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Converts any aliased table or field names in the expression to their actual name, then stores in 
		/// </summary>
		/// <param name="tables"></param>
		/// <param name="parameters"></param>
		/// <param name="column"></param>
		protected internal virtual void ConvertFromAliases(FieldEvaluation fieldsInUse, QueryColumn column, string inputStepName, string inputTableName)
		{
			if (!string.IsNullOrWhiteSpace(column.ColumnExpression))
			{
				column.NonAliasedColumnExpression = ConvertFromAliases(column.ColumnExpression, fieldsInUse, inputStepName, inputTableName);
			}
			foreach (var condition in column.Conditions.Where(o => !string.IsNullOrWhiteSpace(o.ConditionExpression)))
			{
				condition.NonAliasedConditionExpression = ConvertFromAliases(condition.ConditionExpression, fieldsInUse, inputStepName, inputTableName);
			}
		}

		protected string ConvertFromAliases(string expression, FieldEvaluation fieldsInUse, string inputStepName, string inputTableName)
		{
			if (string.IsNullOrWhiteSpace(expression))
			{
				return expression;
			}

			const string literalPlaceholder = "!!!CampaignLiteralPlaceholder!!!";
			Queue<string> literals = new Queue<string>();

			//remove literal strings from the expression, just in case they contain a <table alias>.<field alias> or a parameter name
			while (true)
			{
				if (!expression.Contains("'"))
				{
					break;
				}
				var match = System.Text.RegularExpressions.Regex.Match(expression, @"\'(?<StringLiteral>.*?)\'");
				if (match == null || match.Captures.Count == 0)
				{
					break;
				}
				literals.Enqueue(match.Value);
				expression = string.Format("{0}{1}{2}", expression.Substring(0, match.Index), literalPlaceholder, expression.Substring(match.Index + match.Length));
			}

			CampaignDataProvider provider;
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                provider = Step.IsRealTimeStep() ? manager.RealTimeProvider : manager.BatchProvider;

			if (expression.Contains("."))
			{
				var split = expression.Split('.');
				if (split.Length >= 2)
				{
					for (int i = 0; i < split.Length - 1; i++)
					{
						var table = GetTableName(split[i], fieldsInUse.Fields.Keys);
						if (table != null)
						{
							//first look for exact match on table alias or name
							//replace table alias with name, if needed:

							var field = GetColumnName(split[i + 1], table.Fields);

							if (field != null)
							{
								split[i] = RemoveAlias(table, split[i]);
								split[i + 1] = RemoveAlias(field, split[i + 1]);
							}
						}
						else if (!string.IsNullOrEmpty(inputStepName) && !string.IsNullOrEmpty(inputTableName) && split[i].EndsWith(inputStepName, StringComparison.OrdinalIgnoreCase))
						{
							split[i] = split[i].Substring(0, split[i].Length - inputStepName.Length) + inputTableName;
						}
					}
				}
				expression = string.Join(".", split);

				//if we're lucky, we can get away with this. All values created from the assignment step should appear as "Assigned Values.<assignment name>". Since we only allow alphanumeric as
				//an assignment name, replacing the table alias with "@" should produce a valid parameter name
				expression = expression.Replace(Query.RealTimeAssignedValuesAlias + ".", provider.ParameterPrefix);

			}

			

			if (fieldsInUse.Parameters != null)
			{
				foreach (var p in fieldsInUse.Parameters)
				{
					expression = Regex.Replace(expression, string.Format("@\"{0}\"", p.Name), string.Format("{0}{1}", provider.ParameterPrefix, p.ToDatabaseName()), RegexOptions.IgnoreCase);
				}
			}

			while (literals.Count > 0)
			{
				int index = expression.IndexOf(literalPlaceholder);
				expression = expression.Substring(0, index) + literals.Dequeue() + expression.Substring(index + literalPlaceholder.Length);
			}

			return expression;
		}


		protected CampaignTable GetTableName(string val, IEnumerable<CampaignTable> tables)
		{
			if (string.IsNullOrWhiteSpace(val))
			{
				return null;
			}
			//first try for exact match, then begin removing characters to the left of and 
			//including any spaces in the expression (e.g.: search for "not null or member", 
			//"null or member", "or member", "member" (hit)
			char[] characters = { ' ', '|', '+', '(', ')', ',' };
			string tableName = val;
			while (!string.IsNullOrWhiteSpace(tableName))
			{
				CampaignTable ret = tables.Where(o => (!string.IsNullOrEmpty(o.Alias) && o.Alias.Equals(tableName, StringComparison.OrdinalIgnoreCase)) || o.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
				if (ret != null)
				{
					return ret;
				}

				bool changed = false;
				for (int i = 0; i < tableName.Length; i++)
				{
					if (characters.Contains(tableName[i]))
					{
						tableName = tableName.Substring(i + 1);
						changed = true;
						break;
					}
				}

				if (!changed)
				{
					break;
				}
			}
			return null;
		}

		protected TableField GetColumnName(string val, IEnumerable<TableField> fields, bool allowSpaces = false)
		{
			if (string.IsNullOrWhiteSpace(val))
			{
				return null;
			}

			char[] characters = { ' ', '|', '+', '(', ')', ',' };
			string fieldName = val;
			while (!string.IsNullOrWhiteSpace(fieldName))
			{
				TableField ret = fields.Where(o => (!string.IsNullOrEmpty(o.Alias) && o.Alias.Equals(fieldName, StringComparison.OrdinalIgnoreCase)) || o.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
				if (ret != null)
				{
					return ret;
				}

				bool changed = false;
				for (int i = fieldName.Length - 1; i > 0; i--)
				{
					if (characters.Contains(fieldName[i]))
					{
						fieldName = fieldName.Substring(0, i);
						changed = true;
						break;
					}
				}

				if (!changed)
				{
					break;
				}
			}
			return null;
		}

		protected string RemoveAlias(CampaignTable t, string exp)
		{
			if (string.IsNullOrEmpty(exp))
			{
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(t.Alias) && exp.EndsWith(t.Alias) && !exp.EndsWith(t.Name))
			{
				return exp.Substring(0, exp.LastIndexOf(t.Alias, StringComparison.OrdinalIgnoreCase)) + t.Name;
			}
			return exp;
		}

		protected string RemoveAlias(TableField f, string exp)
		{
			if (string.IsNullOrEmpty(exp))
			{
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(f.Alias) && exp.StartsWith(f.Alias) && !exp.StartsWith(f.Name))
			{
				return f.Name + exp.Substring(f.Alias.Length);
			}
			return exp;
		}

		protected IEnumerable<string> GetRealTimeAssignmentParameterNames()
		{
			var parms = new List<string>();
			foreach (var c in Columns)
			{
				if (!string.IsNullOrEmpty(c.ColumnExpression))
				{
					var split = c.ColumnExpression.Split('.');
					if (split.Length > 1)
					{
						for (int i = 0; i < split.Length - 1; i++)
						{
							if (split[i].EndsWith(Query.RealTimeAssignedValuesAlias, StringComparison.OrdinalIgnoreCase))
							{
								string parm = split[i + 1];
								var match = Regex.Match(parm, "[^0-9a-zA-Z_@#$]");
								if (match != null && match.Index > 0)
								{
									parm = parm.Substring(0, match.Index);
								}
								if (!parms.Contains(parm, StringComparer.OrdinalIgnoreCase))
								{
									parms.Add(parm);
								}
							}
						}

					}
				}
			}
			return parms;
		}

		protected Dictionary<string, object> GetRealTimeParameters()
		{
			var parms = new Dictionary<string, object>();
			foreach (var c in Columns)
			{
				if (!string.IsNullOrEmpty(c.ColumnExpression))
				{
					var split = c.ColumnExpression.Split('.');
					if (split.Length > 1)
					{
						for (int i = 0; i < split.Length - 1; i++)
						{
							if (split[i].EndsWith(Query.RealTimeAssignedValuesAlias, StringComparison.OrdinalIgnoreCase))
							{
								string parm = split[i + 1];
								var match = Regex.Match(parm, "[^0-9a-zA-Z_@#$]");
								if (match != null && match.Index > 0)
								{
									parm = parm.Substring(0, match.Index);
								}
								if (!parms.Keys.Contains(parm, StringComparer.OrdinalIgnoreCase))
								{
									parms.Add(parm, null);
								}
							}
						}
					}
				}
			}
			return parms;
		}

	}
}
