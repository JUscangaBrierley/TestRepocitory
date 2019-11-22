//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.JobScheduler;

namespace Brierley.FrameWork.CampaignManagement.Jobs
{
	public class CampaignJobFactory : IJobFactory
	{
		public IJob GetJob()
		{
			return new CampaignJob();
		}
	}
}
