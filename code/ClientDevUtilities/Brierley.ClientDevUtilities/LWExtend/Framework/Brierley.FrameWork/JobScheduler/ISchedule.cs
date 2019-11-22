//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.JobScheduler
{    
    public interface ISchedule
    {
		bool IsExpired(ScheduledJob jobDetail);        
        bool NeedToRun(ScheduledJob jobDetail);
        //void Reset(LWScheduler scheduler);
    }
}
