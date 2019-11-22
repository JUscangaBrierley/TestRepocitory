using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brierley.FrameWork.CampaignManagement
{
	public class SqlStatement
	{
		/// <summary>
		/// Gets or sets the SQL text
		/// </summary>
		public string Statement { get; set; }


		/// <summary>
		/// Gets or sets whether or not the results of the statement should count toward the result count of the query
		/// </summary>
		public bool ApplyToResults { get; set; }

		public Dictionary<string, object> Parameters { get; set; }


		public SqlStatement(string sql)
		{
			Statement = sql;
			ApplyToResults = true;
		}

		public SqlStatement(string sql, bool applyToResults)
		{
			Statement = sql;
			ApplyToResults = applyToResults;
		}

		public static implicit operator SqlStatement(string sql)
		{
			return new SqlStatement(sql, true);
		}


		public override string ToString()
		{
			return Statement;
		}

		public void AddParameter(string name, object value)
		{
			if (Parameters == null)
			{
				Parameters = new Dictionary<string, object>();
			}
			if (Parameters.ContainsKey(name))
			{
				if (value == null)
				{
					Parameters.Remove(name);
				}
				else
				{
					Parameters[name] = value;
				}
			}
			else if(value != null)
			{
				Parameters.Add(name, value);
			}
		}

	}
}
