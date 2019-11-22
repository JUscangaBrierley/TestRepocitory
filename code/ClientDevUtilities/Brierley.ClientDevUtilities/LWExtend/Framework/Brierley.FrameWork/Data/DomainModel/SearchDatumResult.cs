using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	public class SearchDatumResultItem
	{
		public string SearchExpression;
		public bool CaseSensitiveSearch;
		public StructuredContentData MatchingDatum;
		public StructuredDataRow DataRow;

		public string GetFlattenedFilters()
		{
			string result = string.Empty;
			if (DataRow != null)
			{
				var FilterValues = from cell in DataRow 
								  where cell.Attribute.IsFilter 
								  orderby cell.Attribute.FilterOrder 
								  select cell.Value;
				foreach (string filterValue in FilterValues)
				{
					if (!string.IsNullOrEmpty(result)) result += ", ";
					result += filterValue;
				}
			}
            if (string.IsNullOrEmpty(result)) result = "unfiltered";
			return result;
		}

		public string GetMatchInContext()
		{
			const int contextSize = 20;  // # chars on each side
			string result = string.Empty;
			int idx;
			if (CaseSensitiveSearch)
			{
				idx = MatchingDatum.Data.IndexOf(SearchExpression);
			}
			else
			{
				idx = MatchingDatum.Data.ToLower().IndexOf(SearchExpression.ToLower());
			}
			if (idx > -1)
			{
				int endIdx = Math.Min(idx + SearchExpression.Length + contextSize, MatchingDatum.Data.Length);
				result = 
					"..."
					+ MatchingDatum.Data.Substring(Math.Max(idx - contextSize, 0), Math.Min(idx, contextSize)) 
					+ "<b>"
					+ MatchingDatum.Data.Substring(idx, SearchExpression.Length)
					+ "</b>"
					+ MatchingDatum.Data.Substring(idx + SearchExpression.Length,
						Math.Min(contextSize, MatchingDatum.Data.Length - (idx + SearchExpression.Length))
					  ) 
					+ "...";
			}
			return result;
		}
	}

	[Serializable]
	public class SearchDatumResultBatch : List<SearchDatumResultItem>
	{
		private long _batchID = -1;
		private string _batchName = string.Empty;

		public long BatchID
		{
			get { return _batchID; }
			set { _batchID = value; }
		}

		public string BatchName
		{
			get { return _batchName; }
			set { _batchName = value; }
		}

		public void AddMatch(SearchDatumResultItem match)
		{
			this.Add(match);
		}
	}

	[Serializable]
	public class SearchDatumResult : List<SearchDatumResultBatch>
	{
		public SearchDatumResultBatch FindBatch(long batchID)
		{
			SearchDatumResultBatch result = null;
			foreach (SearchDatumResultBatch batch in this)
			{
				if (batch.BatchID == batchID)
				{
					result = batch;
				}
			}
			return result;
		}

		public SearchDatumResultBatch FindBatch(string batchName)
		{
			SearchDatumResultBatch result = null;
			foreach (SearchDatumResultBatch batch in this)
			{
				if (batch.BatchName == batchName)
				{
					result = batch;
				}
			}
			return result;
		}

		public SearchDatumResultBatch CreateBatch(Batch batch)
		{
			SearchDatumResultBatch result = null;
			if (batch != null)
			{
				result = new SearchDatumResultBatch();
				result.BatchID = batch.ID;
				result.BatchName = batch.Name;
				this.Add(result);
			}
			return result;
		}
	}
}
