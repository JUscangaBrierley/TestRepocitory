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
	public class HourlySchedule : LWScheduleBase
	{
		public HourlyScheduleType HourlyType { get; set; }
		public int Frequency { get; set; }


		public static ISchedule Parse(XElement element)
		{
			HourlySchedule schd = new HourlySchedule();
			schd = (HourlySchedule)ParseBaseProperties(element, schd);

			if (bool.Parse(element.AttributeValue("TopOfTheHour", "false")))
			{
				schd.HourlyType = HourlyScheduleType.TopOfTheHour;
			}
			else
			{
				schd.HourlyType = HourlyScheduleType.HourFrequency;
			}

			schd.Frequency = int.Parse(element.AttributeValue("HourFrequency", "0"));

			return schd;
		}
		

		#region Interface Methods



		public override bool NeedToRun(ScheduledJob jobDetail)
		{
			if (base.NeedToRun(jobDetail))
			{
				if (HourlyType == HourlyScheduleType.TopOfTheHour && DateTime.Now.Minute != 0)
				{
					return false;
				}

				using (var svc = LWDataServiceUtil.DataServiceInstance())
				{
					DateTime lastRun = svc.GetScheduledJobLastRunTime(jobDetail.ID).GetValueOrDefault(DateTimeUtil.MinValue);
					if ((DateTime.Now - lastRun).TotalHours >= Math.Max(Frequency, 1))
					{
						return true;
					}
				}
			}
			return false;
		}


		#endregion

	}
}
