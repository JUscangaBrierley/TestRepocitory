using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brierley.FrameWork;

namespace Brierley.FrameWork.CampaignManagement
{
	public class CampaignResult : ContextObject.RuleResult
	{
		public OutputType? OutputType { get; set; }
		public int RowCount { get; set; }
		public long ReferenceId { get; set; }
		public long MemberReferenceId { get; set; }
		

		public CampaignResult(OutputType outputType, long referenceId, long memberReferenceId)
		{
			OutputType = outputType;
			ReferenceId = referenceId;
			MemberReferenceId = memberReferenceId;
			RowCount = 1;
		}

		public CampaignResult(OutputType outputType, int rowCount)
		{
			OutputType = outputType;
			RowCount = rowCount;
		}

		public CampaignResult(int rowCount)
		{
			RowCount = rowCount;
		}

		public CampaignResult()
		{
		}
	}
}