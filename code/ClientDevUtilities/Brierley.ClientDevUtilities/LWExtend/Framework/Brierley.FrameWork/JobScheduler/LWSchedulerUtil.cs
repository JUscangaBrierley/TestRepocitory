//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.JobScheduler;

namespace Brierley.FrameWork.JobScheduler
{
	public class LWSchedulerUtil
	{
		#region Fields

		private const string _className = "LWSchedulerUtil";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_JOBSCHEDULER);

		#endregion


		#region Private Helpers


		private static XElement CreateJobDetailXml(
			string jobName,
			string assemblyName,
			string factoryType,
			bool executeInSeparateThread,
			bool pauseTimerWhileRunning,
			DateTime startDate,
			DateTime endDate,
			string parms
			)
		{
			// create job details
			XElement job = new XElement("JobDetail",
				new XAttribute("ExecuteInSeparateThread", executeInSeparateThread),
				new XAttribute("PauseTimerWhileRunningJob", pauseTimerWhileRunning),
				new XElement("JobName", jobName),
				new XElement("StartDate", startDate),
				new XElement("EndDate", endDate),
				new XElement("JobFactoryType", factoryType),
				new XElement("AssemblyName", assemblyName)
			);

			if (!string.IsNullOrEmpty(parms))
			{
				job.Add(new XElement("JobParm", parms));
			}
			return job;
		}


		private static XElement CreateJobDetailXml(ScheduledJob job)
		{
			return CreateJobDetailXml(job.Name, job.AssemblyName, job.FactoryName, job.ExecuteInSeparateThread, job.PauseTimerWhileRunningJob, job.StartDate, job.EndDate, job.Parms);
		}


		#endregion


		#region Xml Helpers


		public static string CreateImmediateScheduleXml(
			string jobName,
			string assemblyName,
			string factoryType,
			bool executeInSeparateThread,
			bool pauseTimerWhileRunning,
			DateTime startDate,
			DateTime endDate,
			string parms)
		{
			XElement element = new XElement("JobSchedule",
				new XElement("ImmediateSchedule"));

			XNamespace ns = XNamespace.Get("http://www.brierley.com/JobSchedule");

			element.Name = ns + element.Name.LocalName;

			// create job details
			element.Add(CreateJobDetailXml(jobName, assemblyName, factoryType, executeInSeparateThread, pauseTimerWhileRunning, startDate, endDate, parms));
			return element.ToString();
		}


		public static string CreateImmediateScheduleXml(ScheduledJob job)
		{
			XElement element = new XElement("JobSchedule",
			new XElement("ImmediateSchedule"));

			XNamespace ns = XNamespace.Get("http://www.brierley.com/JobSchedule");

			element.Name = ns + element.Name.LocalName;

			// create job details
			element.Add(CreateJobDetailXml(job));
			return element.ToString();
		}


		public static string CreateDailyScheduleXml(ScheduledJob job, DailySchedule schedule)
		{
			return CreateDailyScheduleXml(job.Name,
				job.AssemblyName,
				job.FactoryName,
				job.ExecuteInSeparateThread,
				job.PauseTimerWhileRunningJob,
				schedule.RunAtHour,
				schedule.RunAtMinute,
				schedule.Type,
				job.StartDate,
				job.EndDate,
				schedule.DailyType,
				schedule.Frequency,
				schedule.RunOnDays,
				job.Parms);
		}


		public static string CreateDailyScheduleXml(
			string jobName,
			string assemblyName,
			string factoryType,
			bool executeInSeparateThread,
			bool pauseTimerWhileRunning,
			int runAtHour,
			int runAtMinute,
			ScheduleType recurringType,
			DateTime startDate,
			DateTime endDate,
			DailyScheduleType type,
			int dailyFrequency,
			List<DayOfWeek> runEvery,
			string parms)
		{
			XElement element = new XElement("JobSchedule",
				new XElement("DailySchedule",
					new XAttribute("RunAtHour", runAtHour),
					new XAttribute("RunAtMinute", runAtMinute),
					new XAttribute("Type", Enum.GetName(typeof(ScheduleType), recurringType)), 
					new XAttribute("DailyFrequency", dailyFrequency)
					));

			if (type == DailyScheduleType.RunEvery)
			{
				XElement dayElement = new XElement("RunEvery");
				foreach (DayOfWeek day in runEvery)
				{
					dayElement.Add(new XElement(day.ToString()));
				}
				element.Element("DailySchedule").Add(dayElement);
			}

			XNamespace ns = XNamespace.Get("http://www.brierley.com/JobSchedule");
			element.Name = ns + element.Name.LocalName;

			// create job details
			element.Add(CreateJobDetailXml(jobName, assemblyName, factoryType, executeInSeparateThread, pauseTimerWhileRunning, startDate, endDate, parms));
			return element.ToString();
		}


		public static string CreateHourlyScheduleXml(ScheduledJob job, HourlySchedule schedule)
		{
			return CreateHourlyScheduleXml(job.Name,
				job.AssemblyName,
				job.FactoryName,
				job.ExecuteInSeparateThread,
				job.PauseTimerWhileRunningJob,
				schedule.Type,
				job.StartDate,
				job.EndDate,
				schedule.HourlyType,
				schedule.Frequency,
				job.Parms);
		}


		public static string CreateHourlyScheduleXml(
			string jobName,
			string assemblyName,
			string factoryType,
			bool executeInSeparateThread,
			bool pauseTimerWhileRunning,
			ScheduleType recurringType,
			DateTime startDate,
			DateTime endDate,
			HourlyScheduleType type,
			int hourFrequency,
			string parms)
		{

			XElement element = new XElement("JobSchedule",
				new XElement("HourlySchedule",
					new XAttribute("Type", recurringType),
					new XAttribute("TopOfTheHour", type == HourlyScheduleType.TopOfTheHour),
					new XAttribute("HourFrequency", hourFrequency)
					));

			XNamespace ns = XNamespace.Get("http://www.brierley.com/JobSchedule");
			element.Name = ns + element.Name.LocalName;

			// create job details
			element.Add(CreateJobDetailXml(jobName, assemblyName, factoryType, executeInSeparateThread, pauseTimerWhileRunning, startDate, endDate, parms));
			return element.ToString();
		}


		public static string CreateMinutelyScheduleXml(ScheduledJob job, MinuteSchedule schedule)
		{
			return CreateMinutelyScheduleXml(job.Name, 
				job.AssemblyName, 
				job.FactoryName, 
				job.ExecuteInSeparateThread, 
				job.PauseTimerWhileRunningJob, 
				schedule.Type, 
				job.StartDate, 
				job.EndDate, 
				schedule.Frequency, 
				job.Parms);
		}


		public static string CreateMinutelyScheduleXml(
			string jobName,
			string assemblyName,
			string factoryType,
			bool executeInSeparateThread,
			bool pauseTimerWhileRunning,
			ScheduleType recurringType,
			DateTime startDate,
			DateTime endDate,
			int minuteFrequency,
			string parms)
		{

			XElement element = new XElement("JobSchedule",
				new XElement("MinutelySchedule",
					new XAttribute("MinuteFrequency", minuteFrequency)
					));


			XNamespace ns = XNamespace.Get("http://www.brierley.com/JobSchedule");
			element.Name = ns + element.Name.LocalName;

			// create job details
			element.Add(CreateJobDetailXml(jobName, assemblyName, factoryType, executeInSeparateThread, pauseTimerWhileRunning, startDate, endDate, parms));
			return element.ToString();
		}


		public static string CreateOneTimeScheduleXml(ScheduledJob job, OneTimeSchedule schedule)
		{
			return CreateOneTimeScheduleXml(job.Name,
				job.AssemblyName,
				job.FactoryName,
				job.ExecuteInSeparateThread,
				job.PauseTimerWhileRunningJob,
				schedule.RunTime,
				job.Parms);
		}


		public static string CreateOneTimeScheduleXml(
			string jobName,
			string assemblyName,
			string factoryType,
			bool executeInSeparateThread,
			bool pauseTimerWhileRunning,
			DateTime runTime,
			string parms)
		{
			XElement element = new XElement("JobSchedule",
				new XElement("OneTimeSchedule",
					new XAttribute("RunAt", runTime)
					)
				);

			XNamespace ns = XNamespace.Get("http://www.brierley.com/JobSchedule");
			element.Name = ns + element.Name.LocalName;

			// create job details
			element.Add(CreateJobDetailXml(jobName, assemblyName, factoryType, executeInSeparateThread, pauseTimerWhileRunning, runTime, runTime, parms));
			return element.ToString();
		}




		#endregion



	}
}
