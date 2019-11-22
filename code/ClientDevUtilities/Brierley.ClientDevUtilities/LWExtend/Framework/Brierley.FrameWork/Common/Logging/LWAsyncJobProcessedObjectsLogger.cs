//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Threading;
using System.Xml;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Threading;

namespace Brierley.FrameWork.Common.Logging
{
    /// <summary>
    /// LW-3792 New logger that will be used to log table entries by using a log4net appender
    /// </summary>
    public class LWAsyncJobProcessedObjectsLogger
    {
        public static string source = "FailedMessages";
        private bool keepRunning = true;
        private int sleepInterval = 2000;
        private Thread thrd;

        private ServiceConfig _config = null;
        private LWLogger _asyncProcessedOjbectsLogger = LWLoggerManager.GetLogger(LWConstants.LW_ASYNCPROCESSEDOBJECTS_LOG);
        private LWSyncQueue requestQ;

        /// <summary>
        /// This is the RequestQ where this logger receives messages to log.
        /// </summary>
        public LWSyncQueue RequestQueue
        {
            get { return requestQ; }
        }

        public LWAsyncJobProcessedObjectsLogger(LWConfiguration config)
        {
            requestQ = new LWSyncQueue();
            Initialize(config);
        }

        private void Initialize(LWConfiguration config)
        {
            LWConfigurationUtil.SetCurrentEnvironmentContext(config.Organization, config.Environment);
            _config = LWDataServiceUtil.GetServiceConfiguration(config.Organization, config.Environment);
        }

        public void Start()
        {
            thrd = new Thread(this.Run); // use method group conversion            
            thrd.Start(); // start the thread
        }

        public void Shutdown()
        {
            keepRunning = false;
        }

        public void WaitToFinish()
        {
            if (thrd != null && thrd.IsAlive)
            {
                thrd.Join();
            }
        }

        /// <summary>
        /// Main method, this is adding all of the object properties to the the log4net thread context 
        /// </summary>
        void Run()
        {

            while (keepRunning || requestQ.Size > 0)
            {
                Object obj = requestQ.Remove(sleepInterval);

                AsyncJobProcessedObjects job = obj as AsyncJobProcessedObjects;

                if (job != null)
                {
                    try
                    {
                        log4net.ThreadContext.Properties["Id"] = job.Id;
                        log4net.ThreadContext.Properties["JobName"] = job.JobName;
                        log4net.ThreadContext.Properties["JobNumber"] = job.JobNumber;
                        log4net.ThreadContext.Properties["ObjectName"] = job.ObjectName;
                        log4net.ThreadContext.Properties["RowKey"] = job.RowKey;
                        log4net.ThreadContext.Properties["CreateDate"] = job.CreateDate;
                        log4net.ThreadContext.Properties["UpdateDate"] = job.UpdateDate;

                        //We are still going to Utilize the LWLogger since there is no need to duplicate all of that logic. 
                        _asyncProcessedOjbectsLogger.Trace("Id:" + job.Id + " JobName:" + job.JobName + " JobNumber:" + job.JobNumber);

                    }
                    finally
                    {
                    }
                }
            }
        }

    }
}
