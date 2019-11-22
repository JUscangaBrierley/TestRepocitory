using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LWModules.VisitMap.Components
{
	public class VisitMapTransactionHeader
	{
		public long RowKey { get; set; }
		public string HeaderId { get; set; }
		public DateTime TransactionDate { get; set; }
		public string ActivityName { get; set; }
		public bool IsQualifyingSpend { get; set; }
		public double Spend { get; set; }
		public StoreDef Store { get; set; }
	}
}