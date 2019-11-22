using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class SearchDatumArgs
	{
		public long BatchID { get; set;}
		public string SearchExpression { get; set; }
		public bool IsCaseSensitiveSearch { get; set; }
		public long ElementID { get; set; }
	}
}
