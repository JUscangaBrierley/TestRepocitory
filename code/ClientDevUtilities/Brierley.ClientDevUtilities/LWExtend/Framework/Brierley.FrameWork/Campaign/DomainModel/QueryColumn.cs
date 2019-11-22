using System;
using System.Collections.Generic;
using Brierley.FrameWork.CampaignManagement;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	/// <summary>
	/// POCO for QueryColumn.
	/// </summary>
	[Serializable]
	public class QueryColumn
	{
		private string _nonAliasedColumnExpression = null;

		/// <summary>
		/// Initializes a new instance of the QueryColumn class
		/// </summary>
		public QueryColumn()
		{
			Conditions = new List<ColumnCondition>();
		}

		/// <summary>
		/// Gets or sets the ColumnType for the current QueryColumn
		/// </summary>
		public ColumnType ColumnType { get; set; }

		/// <summary>
		/// Gets or sets the TableID for the current QueryColumn
		/// </summary>
		public long TableId { get; set; }

		/// <summary>
		/// Gets or sets the FieldName for the current QueryColumn
		/// </summary>
		public string FieldName { get; set; }

		/// <summary>
		/// Gets or sets the ColumnExpression for the current QueryColumn
		/// </summary>
		public string ColumnExpression { get; set; }


		/// <summary>
		/// Gets or sets the transient non-aliased column expression
		/// </summary>
		[XmlIgnore]
		public string NonAliasedColumnExpression 
		{
			get { return _nonAliasedColumnExpression ?? ColumnExpression; }
			set { _nonAliasedColumnExpression = value; }
		}

		/// <summary>
		/// Gets or sets the AggregateExpression for the current QueryColumn
		/// </summary>
		public string AggregateExpression { get; set; }

		/// <summary>
		/// Gets or sets the HavingExpression for the current QueryColumn
		/// </summary>
		public string HavingExpression { get; set; }

		/// <summary>
		/// Gets or sets the IncludeInOutput for the current QueryColumn
		/// </summary>
		public bool IncludeInOutput { get; set; }

		/// <summary>
		/// Gets or sets the OutputAs for the current QueryColumn
		/// </summary>
		public string OutputAs { get; set; }

		public string TableName { get; set; }


		public int? RowLimit { get; set; }

		public bool IsLimitPercentage { get; set; }

		public bool RandomSample { get; set; }


		//Construct this column's sql statement as a parameter
		[XmlIgnore]
		public bool IsParameter { get; set; }

		/// <summary>
		/// Gets or sets the with in the query builder
		/// </summary>
		public int Width { get; set; }

		public List<ColumnCondition> Conditions { get; set; }

		public string GetOutputAsToken(List<QueryColumn> siblings)
		{
			string ret = OutputAs;
			if(string.IsNullOrEmpty(ret))
			{
				return ret;
			}

			//Oracle allows you to wrap quotes around your object. If you do that, you can name it pretty much anything you like.
			bool isQuoted = ret.StartsWith("\"") && ret.EndsWith("\"") && ret.Length > 2;
			if (isQuoted)
			{
				ret = ret.Substring(1, ret.Length - 2);
			}
						
			if (!isQuoted)
			{
				//If you haven't wrapped your field in quotes, then invalid characters will not be allowed. Nonquoted 
				//identifiers can contain only alphanumeric characters, underscore, dollar sign and pound sign (e.g., DMC 
				//field names typically contain periods, which are not allowed). Replace invalid characters with underscores:
				ret = Regex.Replace(ret, "[^a-zA-Z0-9#$_]", "_");
			}

			int maxLength = Constants.MaxOracleFieldNameLength - (isQuoted ? 2 : 0);

			if (ret.Length > maxLength)
			{
				//Oracle cannot handle field names longer than 30 characters, and DMC emails wind up with very long field names. In 
				//order to get these into Oracle, we need to shorten the name of any fields over 30 characters long.
				int index = 0;
				for (int i = 0; i < siblings.Count; i++)
				{
					if (siblings[i] == this)
					{
						index = i;
						break;
					}
				}
				string suffix = "_" + index.ToString();
				ret = ret.Substring(0, maxLength - suffix.Length) + suffix;
			}

			if (isQuoted)
			{
				ret = string.Format("\"{0}\"", ret);
			}

			return ret;
		}
	}
}