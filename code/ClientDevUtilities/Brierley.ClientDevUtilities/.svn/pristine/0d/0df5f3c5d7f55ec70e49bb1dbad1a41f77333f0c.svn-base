using System.Collections.Generic;
using System.Data;
using Brierley.FrameWork.CampaignManagement.DomainModel;

namespace Brierley.FrameWork.CampaignManagement.DataProvider
{
	/// <summary>
	/// Contains all information necessary for a provider class to construct a sql query from.
	/// </summary>
	public class SqlQuery
	{
		public StepType StepType { get; set; }

		public string ActionTable { get; set; }

		public string RootTableName { get; set; }

		public List<QueryColumn> Columns { get; set; }

		public List<string> InsertFieldList { get; set; }

		public List<string> SelectFieldList { get; set; }

		public List<string> RootTableCarryover { get; set; }

		public List<Join> Joins { get; set; }

		public List<Join> Exclusions { get; set; }

		public List<Merge> Merges { get; set; }

		/// <summary>
		/// The fields to include as part of the initial merge statement. 
		/// </summary>
		/// <remarks>
		/// Fields not included will be brought in by additional update statements following the merge. For example, the input tables contain
		/// ipcode, firstname and lastname, and the user selects ipcode and first name to merge with
		/// </remarks>
		public List<string> MergeFields { get; set; }

		public bool RandomSample { get; set; }

		public int Limit { get; set; }

		public bool IsLimitPercentage { get; set; }

		public bool CreateTable { get; set; }

		public List<string> GroupBy { get; set; }

		public List<string> OrderBy { get; set; }

		public bool DistinctRows { get; set; }

		public bool RealTimeIPCodeParameter { get; set; }

		public string ProcessName { get; set; }

		public string ProcessValue { get; set; }

		internal SqlQuery()
		{
			ActionTable = string.Empty;
			RootTableName = string.Empty;
			InsertFieldList = new List<string>();
			SelectFieldList = new List<string>();
			RootTableCarryover = new List<string>();
			Joins = new List<Join>();
			Exclusions = new List<Join>();
			Merges = new List<Merge>();
			Limit = -1;
			Columns = new List<QueryColumn>();
		}

	}


	public class Join
	{
		//join a table and field
		public string TableName { get; set; }
		public string FieldName { get; set; }

		//...or a subquery and alias
		public string Query { get; set; }
		public string Alias { get; set; }
		//subquery joins can have an additional set of statements for the join (e.g., setting row number in pivot)
		public string JoinCondition { get; set; }
		
		//to a table and field
		public string JoinToTableName { get; set; }
		public string JoinToFieldName { get; set; }

		public TableJoinType JoinType { get; set; }

		public Join(string tableName, string fieldName, string joinToTableName, string joinToFieldName, TableJoinType joinType)
		{
			TableName = tableName;
			FieldName = fieldName;
			JoinToTableName = joinToTableName;
			JoinToFieldName = joinToFieldName;
			JoinType = joinType;
		}

		public Join()
		{
			JoinType = TableJoinType.Left;
		}
	}
	

	public class Merge
	{
		public string TableName { get; set; }

		public MergeType MergeType { get; set; }

		public DataTable Schema { get; set; }

		public string KeyField { get; set; }

		public Merge(string tableName, MergeType mergeType)
		{
			TableName = tableName;
			MergeType = mergeType;
		}

		public Merge()
		{
		}
	}
	
}
