//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Threading;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Common.Logging
{
    /// <summary>
    /// LW-3792 New logger class to add the asynjob properties to the log4net thread context, this will allow us to use a log4net appender instead of writing directly to the table. 
    /// </summary>
    public class LWAsyncJobLogger : LWThread
    {
       				
		private ServiceConfig _config = null;
        private LWLogger _asyncJobLogger = LWLoggerManager.GetLogger(LWConstants.LW_ASYNCJOB_LOG);


        public LWAsyncJobLogger() 
            : base("LWAsyncJobLogger")
        {
            _config = LWDataServiceUtil.GetServiceConfiguration();
        }

        public LWAsyncJobLogger(string orgName,string envName)
			: base("LWAsyncJobLogger")
		{
			LWConfigurationUtil.SetCurrentEnvironmentContext(orgName, envName);
			_config = LWDataServiceUtil.GetServiceConfiguration();
		}

		public LWAsyncJobLogger(LWConfiguration config)
			: base("LWAsyncJobLogger")
		{
			LWConfigurationUtil.SetConfiguration(config);
			_config = LWDataServiceUtil.GetServiceConfiguration();
		}

        /// <summary>
        /// This method will add all of the asynjob properties to a log4net thread context.
        /// </summary>
        /// <param name="jobMessage">This will be an AsyncJob object</param>
        public override void ProcessRequest(object jobMessage)
		{
            AsyncJob job = jobMessage as AsyncJob;

            if (job != null)
            {
                try
                {
                    log4net.ThreadContext.Properties["JobId"] = job.JobId;
                    log4net.ThreadContext.Properties["JobNumber"] = job.JobNumber;
                    log4net.ThreadContext.Properties["JobType"] = job.JobType;
                    log4net.ThreadContext.Properties["JobName"] = job.JobName;
                    log4net.ThreadContext.Properties["JobDirection"] = job.JobDirection;
                    log4net.ThreadContext.Properties["FileName"] = job.FileName;
                    log4net.ThreadContext.Properties["StartTime"] = job.StartTime;
                    log4net.ThreadContext.Properties["EndTime"] = job.EndTime;
                    log4net.ThreadContext.Properties["MessagesReceived"] = job.MessagesReceived;
                    log4net.ThreadContext.Properties["MessagesFailed"] = job.MessagesFailed;
                    log4net.ThreadContext.Properties["JobStatus"] = job.JobStatus;
                    log4net.ThreadContext.Properties["CreateDate"] = job.CreateDate;
                    log4net.ThreadContext.Properties["UpdateDate"] = job.UpdateDate;


                    //We will still use the LWLogger to actually log things to log4net, I don't see a reason to duplicate the logic
                    _asyncJobLogger.Trace("JobId:" + job.JobId + " JobNumber:" + job.JobNumber + " JobName:" + job.JobName);


                }
                finally
                {
                }
            }
		}

		public override void Cleanup()
		{
		}

		protected override void ChildDispose()
		{
		}
    }
}
