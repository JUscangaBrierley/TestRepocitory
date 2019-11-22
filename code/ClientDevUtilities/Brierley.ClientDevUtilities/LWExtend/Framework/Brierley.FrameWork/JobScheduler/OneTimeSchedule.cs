//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Extensions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.JobScheduler
{
	public class OneTimeSchedule : LWScheduleBase
	{
		public DateTime RunTime { get; set; }

		public OneTimeSchedule()
		{
            RunTime = DateTimeUtil.MaxValue;
		}

		#region Parse
		
		public static ISchedule Parse(XElement element)
		{
			OneTimeSchedule schd = new OneTimeSchedule();
			schd = (OneTimeSchedule)ParseBaseProperties(element, schd);

            schd.RunTime = DateTime.Parse(element.AttributeValue("RunAt", DateTimeUtil.MinValue.ToString()));

			return schd;
		}
		#endregion

		#region Interface Methods
		public override bool NeedToRun(ScheduledJob jobDetail)
		{
			// has it already been run.
			using (var svc = LWDataServiceUtil.DataServiceInstance())
			{
				if (svc.HasScheduledJobBeenRun(jobDetail.ID))
				{
					return false;
				}
				else
				{
					return Math.Abs((RunTime - DateTime.Now).TotalSeconds) < 60;
				}
			}
		}

		#endregion
	}
}