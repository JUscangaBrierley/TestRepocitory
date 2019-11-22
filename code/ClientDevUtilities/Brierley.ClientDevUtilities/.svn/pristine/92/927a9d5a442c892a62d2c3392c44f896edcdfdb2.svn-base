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
    public class DailySchedule : LWScheduleBase
    {
		public DailyScheduleType DailyType { get; set; }

		public int Frequency { get; set; }

		public int RunAtHour { get; set; }

		public int RunAtMinute { get; set; }

		public List<System.DayOfWeek> RunOnDays { get; set; }


        public static ISchedule Parse(XElement element)
        {
            DailySchedule schd = new DailySchedule();
            schd = (DailySchedule)ParseBaseProperties(element, schd);

			schd.RunAtHour = int.Parse(element.AttributeValue("RunAtHour"));
			schd.RunAtMinute = int.Parse(element.AttributeValue("RunAtMinute"));

			schd.Frequency = int.Parse(element.AttributeValue("DailyFrequency", "0"));
			if (schd.Frequency > 0)
			{
				schd.DailyType = DailyScheduleType.DailyFrequency;
			}

			XElement runEvery = element.Element("RunEvery");
			if (runEvery != null)
			{
				schd.DailyType = DailyScheduleType.RunEvery;
				schd.RunOnDays = new List<DayOfWeek>();
				if (runEvery.Element(DayOfWeek.Sunday.ToString()) != null)
				{
					schd.RunOnDays.Add(DayOfWeek.Sunday);
				}
				if (runEvery.Element(DayOfWeek.Monday.ToString()) != null)
				{
					schd.RunOnDays.Add(DayOfWeek.Monday);
				}
				if (runEvery.Element(DayOfWeek.Tuesday.ToString()) != null)
				{
					schd.RunOnDays.Add(DayOfWeek.Tuesday);
				}
				if (runEvery.Element(DayOfWeek.Wednesday.ToString()) != null)
				{
					schd.RunOnDays.Add(DayOfWeek.Wednesday);
				}
				if (runEvery.Element(DayOfWeek.Thursday.ToString()) != null)
				{
					schd.RunOnDays.Add(DayOfWeek.Thursday);
				}
				if (runEvery.Element(DayOfWeek.Friday.ToString()) != null)
				{
					schd.RunOnDays.Add(DayOfWeek.Friday);
				}
				if (runEvery.Element(DayOfWeek.Saturday.ToString()) != null)
				{
					schd.RunOnDays.Add(DayOfWeek.Saturday);
				}
			}
            return schd;
        }
		

		public override bool NeedToRun(Brierley.FrameWork.Data.DomainModel.ScheduledJob jobDetail)
		{
			if (base.NeedToRun(jobDetail))
			{
				using (var svc = LWDataServiceUtil.DataServiceInstance())
				{

					//are we running on specific days of the week? If so, is today one of those days?
					if (DailyType == DailyScheduleType.RunEvery && Frequency < 1 && RunOnDays != null && RunOnDays.Count > 0)
					{
						bool canRunToday = false;
						foreach (DayOfWeek runOnDay in RunOnDays)
						{
							if (DateTime.Today.DayOfWeek == runOnDay)
							{
								canRunToday = true;
								break;
							}
						}
						if (!canRunToday)
						{
							return false;
						}
					}


					DateTime lastRun = svc.GetScheduledJobLastRunTime(jobDetail.ID).GetValueOrDefault(DateTimeUtil.MinValue);
					if (lastRun >= DateTime.Today)
					{
						//job has already been run once today, cannot run again.
						return false;
					}


					//are we at or past (within reason - no more than 5 minutes past) the time to run?
					DateTime runTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, RunAtHour, RunAtMinute, 0);
					if (DateTime.Now >= runTime && (DateTime.Now - runTime).TotalMinutes < 5)
					{
						if (Frequency > 0)
						{
							//schedule has intervals. need to check last run time to see if it can be run
							if (Math.Round((DateTime.Today - lastRun).TotalDays) >= Frequency)
							{
								//daily interval has elapsed, job needs to run
								return true;
							}
						}
						else
						{
							//job runs every day
							return true;
						}
					}
				}
			}
			return false;
		}
    }
}
