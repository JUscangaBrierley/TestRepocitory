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
	public class MinuteSchedule : LWScheduleBase
	{
		public int Frequency { get; set; }


		public MinuteSchedule()
		{
			Type = ScheduleType.Recurring;
		}


		public static ISchedule Parse(XElement node)
		{
			MinuteSchedule schd = new MinuteSchedule();
			schd = (MinuteSchedule)ParseBaseProperties(node, schd);
			schd.Frequency = int.Parse(node.AttributeValue("MinuteFrequency", "0"));
			return schd;
		}


		public override bool NeedToRun(ScheduledJob jobDetail)
		{
			if (base.NeedToRun(jobDetail))
			{
				// has it already been run.
				using (var svc = LWDataServiceUtil.DataServiceInstance())
				{
					DateTime? lastRun = svc.GetScheduledJobLastRunTime(jobDetail.ID).GetValueOrDefault(DateTimeUtil.MinValue);
					if ((DateTime.Now - lastRun.Value).TotalMinutes >= Math.Max(Frequency, 1))
					{
						return true;
					}
				}
			}
			return false;
		}

	}
}
