//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.JobScheduler
{
    public class ImmediateSchedule : LWScheduleBase
    {
		public ImmediateSchedule()
        {            
        }

        #region Parse
        public static ISchedule Parse(XElement node)
        {
			ImmediateSchedule schd = new ImmediateSchedule();
			schd = (ImmediateSchedule)ParseBaseProperties(node, schd);                       
            return schd;
        }
        #endregion               

        #region Interface Methods 
		public override bool NeedToRun(ScheduledJob jobDetail)
		{
			bool runIt = base.NeedToRun(jobDetail);
			if (runIt)
			{
				// has it already been run.
				using (var svc = LWDataServiceUtil.DataServiceInstance())
				{
					if (svc.HasScheduledJobBeenRun(jobDetail.ID))
					{
						runIt = false;
					}
				}
			}
			return runIt;
		}

		//public override void ResetSchedule(LWScheduler scheduler)
		//{
		//	//scheduler.ResetTimer(0);
		//}
        #endregion        
    }
}
