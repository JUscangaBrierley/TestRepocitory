//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using System.Xml.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Extensions;

namespace Brierley.FrameWork.JobScheduler
{
    public abstract class LWScheduleBase : ISchedule
    {
		private ScheduleType type = ScheduleType.OneTime;

        #region Parse		
        protected static LWScheduleBase ParseBaseProperties(XElement element, LWScheduleBase instance)
        {
			switch (element.AttributeValue("Type"))
			{
				case "OneTime":
					instance.Type = ScheduleType.OneTime;
					break;
				case "Recurring":
					instance.Type = ScheduleType.Recurring;
					break;
				default:
					break;
			}
            return instance;
        }
        #endregion

        #region Properties
        //public DateTime StartTime
        //{
        //    get { return startTime; }
        //}

        //public DateTime EndTime
        //{
        //    get { return endTime; }
        //}
		public ScheduleType Type
        {
			get { return type; }
            set { type = value; }
        }        
        #endregion

        #region Public Methods
        /// <summary>
        /// This method ensures that the timer is active now, based on the start time that
        /// was specified.
        /// </summary>
        /// <returns></returns>
        public bool IsActive(ScheduledJob jobDetail)
        {
            DateTime dt = DateTime.Now;
			if (jobDetail.StartDate != null && dt.CompareTo(jobDetail.StartDate) < 0)
            {
                // it is not time to start yet.
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// This method checks the end date and ensures that the timer is not expired
        /// yet.
        /// </summary>
        /// <returns></returns>
        public bool IsExpired(ScheduledJob jobDetail)
        {
            //return expired;
            DateTime dt = DateTime.Now;
			if (jobDetail.EndDate != null && dt.CompareTo(jobDetail.EndDate) > 0)
            {
                // the end has already passed.
                return true;
            }
            else
            {
                return false;
            }
        }
               
        //public virtual void ResetSchedule(LWScheduler scheduler)
        //{
        //}
		/*
        public void Reset(LWScheduler scheduler)
        {
			if (type == ScheduleType.Recurring)
			{
				// reset the schedule.
				ResetSchedule(scheduler);
			}
			else
			{
				// stop the timer.
				scheduler.ShutDown = true;
			}
        }
		*/
		public virtual bool NeedToRun(ScheduledJob jobDetail)
        {
			bool runjob = IsActive(jobDetail) && !IsExpired(jobDetail) ? true : false;
            return runjob;
        }
        #endregion
    }
}
