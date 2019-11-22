using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Brierley.FrameWork.CampaignManagement
{
	public enum TableJoinType
	{
		Inner,
		Left,
		Right
	}

	public enum TableInclusionType
	{
		IncludeOnlyFramework,
		IncludeOnlyWarehouse,
		IncludeAll
	}

	public enum CampaignType
	{
		Batch = 1,
		RealTime = 2,
		Plan = 3
	}

	public enum VerificationState
	{
		None = 0,
		Pending = 1,
		Verified = 2
	}

	public enum OutputType
	{
		FlatFile = 2,
		Table = 3,
		Promotion = 4,
		Offer = 5,
		Survey = 6,
		Coupon = 7,
		Message = 8,
		Email = 9, 
		SocialMedia = 10, 
		NextBestAction = 11,
        Sms = 12,
        Notification = 13
	}

	public enum ColumnType
	{
		Condition = 1,
		Append = 2,
		Segment = 3
	}

	public enum TableType
	{
		Framework = 1,
		Step = 2,
		Output = 3
	}

	[Serializable]
	public enum StepType
	{
		/// <summary>
		/// Selects rows with either a framework/data warehouse table or a table created as the ouput of a step and inserts 
		/// the results into its output table.
		/// </summary>
		Select = 1,

		/// <summary>
		/// Step which splits the campaign processing between two or more steps based on the selection criteria of each step.
		/// </summary>
		SplitProcess = 2,

		/// <summary>
		/// Appends new fields with assigned values to the step's output table. Will pull all records from the input table and 
		/// output to the output table (i.e., either a catch-all value exists or null is assigned to the new column).
		/// </summary>
		Assignment = 4,

		/// <summary>
		/// Merges rows from 2 or more input steps.
		/// </summary>
		Merge = 5,

		/// <summary>
		/// Selects rows with either a framework/data warehouse table
		/// or a table created as the ouput of a step. This type only applies to output queries, because
		/// all other queries feed results into an output table.
		/// </summary>
		Output = 7,

		ChangeAudience = 8,

		/// <summary>
		/// A query that uses SQL input by the user. No columns, no conditions, just straight SQL text.
		/// </summary>
		Sql = 9,
		DeDupe = 10,

		/// <summary>
		/// One to N queries that perform selections with either a row or percentage limit, meant to separate records into "test" and "control" groups.
		/// </summary>
		ControlGroup = 11,

		//real-time versions of the StepTypes listed above:
		RealTimeSelect = 12,
		RealTimeAssignment = 13,
		RealTimeOutput = 15,
		RealTimeInputProcessing = 16,
		RealTimebScript = 17,
		RealTimeSurvey = 18,

		//Steps for Campaign Plan:
		Campaign = 19,
		Wait = 20,

		//bScript, batch version, new in LoyaltyWare 4.5
		bScript = 21, 

		//pivot, batch only, new in 4.5.6
		Pivot = 22
	}

	public enum ExecutionTypes
	{
		/// <summary>
		/// camapign or step is being executed by a user in the foreground
		/// </summary>
		Foreground,

		/// <summary>
		/// campaign or step is being executed by a user in the background
		/// </summary>
		Background,

		/// <summary>
		/// campaign is being executed by the job scheduler
		/// </summary>
		Schedule
	}


	[Serializable]
	public enum DateParts
	{
		Complete,
		Month,
		Day,
		Year,
		Hour,
		Minute,
		Second
	}

	[Serializable]
	[Flags]
	public enum StringFunctions
	{
		Upper = 1,
		Lower = 2,
		Trim = 4,
		Length = 8
	}

	[Serializable]
	public enum KeyConversionFunctions
	{
		TakeAllDuplicates = 1,
		Min = 2,
		Max = 3
	}


	/// <summary>
	/// Enum to determine how unique values of a column are gathered and stored.
	/// </summary>
	public enum ValueGenerationType
	{
		/// <summary>
		/// The column does not contain a computable list of unique values, therefore no list will be generated.
		/// </summary>
		NotGenerated = 1,

		/// <summary>
		/// The column belongs to a lookup table small enough to compute unique values on the fly, as needed.
		/// </summary>
		OnTheFly = 2,

		/// <summary>
		/// The column belongs to a table too large to calculate unique values on the fly, therefore the values are 
		/// calculated when the campaign manager chooses to refreshe the list, and the unique values are stored in a 
		/// table for retrieval.
		/// </summary>
		Cached = 3,

		/// <summary>
		/// A list of unique values are provided by the campaign manager
		/// </summary>
		Manual = 4,

		/// <summary>
		/// Sql statement is provided to obtain a value list from either a separate table or from the same table using a 
		/// display field as a helper (e.g., supply tier code in LW_MemberTiers with a list of tier id and name from LW_Tiers)
		/// </summary>
		SqlCached = 5,

		/// <summary>
		/// 
		/// </summary>
		SqlOnTheFly = 6
	}


	public enum MergeType
	{
		/// <summary>
		/// Treat the table as the top level table of the union statement
		/// </summary>
		[System.Xml.Serialization.XmlEnum]
		Top = 1,

		/// <summary>
		/// Union select, exclude duplicates
		/// </summary>
		Union = 2,

		/// <summary>
		/// Include duplicates
		/// </summary>
		UnionAll = 3,

		/// <summary>
		/// Returns any distinct values that are returned by both the query on the left and right sides of the INTERSECT operand.
		/// </summary>
		Intersect = 4,

		/// <summary>
		/// All rows from top table, minus those rows that exist in the bottom table
		/// </summary>
		ExceptOrMinus = 5
	}


	public enum AttributeDataType
	{
		Integer,
		String,
		Float,
		DateTime
	}

	public enum AttributeTypes
	{
		Campaign = 1, 
		Segment = 2, 
		Offer = 3
	}

	public class Constants
	{
		public const string LW_CM = "CampaignWare";
		public const string TempTableNamePrefix = "LW_CL_";
		public const short MaxOracleFieldNameLength = 30;

		//todo: this constant has not been implemented by any rule as of yet:
		/// <summary>
		/// The maximum allowable length for a field where distinct values may be computed
		/// </summary>
		/// <remarks>
		///	Unless provided by the campaign manager in a manual list, no string database field may have its distinct values
		///	computed by campaign management if the maximum length of the field exceeds this constant. Values that are cached
		///	in the database are stored in a table with the same maximum length as this constant, therefore any fields that
		///	exceed this length would be truncated and the values could not be guaranteed accurate.
		/// </remarks>
		public const int DistinctValueMaxLength = 255;

		public const string CampaignHistoryTableName = "LW_CLHistory";

		public class CacheRegions
		{
			public const string SqlStatementsByStepId = "SqlStatementsByStepId";
			public const string CampaignById = "CampaignById";
			public const string MappableTablesByAudienceId = "MappableTablesByAudienceId";
			public const string TableSchemaById = "TableSchemaById";
			public const string StepSchemaById = "StepSchemaById";
		}
	}
}
