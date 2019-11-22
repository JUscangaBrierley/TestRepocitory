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
    /// This class logs failed messages.
    /// </summary>
    public class LWFailedMessageLogger
    {        
        public static string source = "FailedMessages";        
        private bool keepRunning = true;
        private int sleepInterval = 2000;        
        private Thread thrd;

        private ServiceConfig _config = null;
        private LWLogger _failedMessageLogger = LWLoggerManager.GetLogger(LWConstants.LW_FAILEDMESSAGE_LOG);

        private LWSyncQueue requestQ;

        /// <summary>
        /// This is the RequestQ where this logger receives messages to log.
        /// </summary>
        public LWSyncQueue RequestQueue
        {
            get { return requestQ; }
        }

        public LWFailedMessageLogger(LWConfiguration config)
        {
            requestQ = new LWSyncQueue();            
            Initialize(config);
        }

        private void Initialize(LWConfiguration config)
        {
			LWConfigurationUtil.SetCurrentEnvironmentContext(config.Organization, config.Environment);
			_config = LWDataServiceUtil.GetServiceConfiguration(config.Organization, config.Environment);
        }

        /// <summary>
        /// This was logic that was being done prior to insterting to the table so I've moved it here so that we can keep the same fucntionallity as before. 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private LIBMessageLog TransformMessage(LIBMessageLog msg)
        {
            if (string.IsNullOrEmpty(msg.Reason) && msg.Exception != null)
            {
                msg.Reason = msg.Exception.Message;
            }
            if (msg.EnvKey.Length > 150)
            {
                msg.EnvKey = msg.EnvKey.Substring(0, 150);
            }
            msg.LogSource = string.IsNullOrEmpty(msg.LogSource) ? " " : msg.LogSource;
            if (msg.LogSource.Length > 150)
            {
                msg.LogSource = msg.LogSource.Substring(0, 150);
            }
            if (!string.IsNullOrEmpty(msg.Reason) && msg.Reason.Length > 500)
            {
                msg.Reason = msg.Reason.Substring(0, 500);
            }
            msg.FileName = string.IsNullOrEmpty(msg.FileName) ? " " : msg.FileName;
            if (!string.IsNullOrEmpty(msg.FileName) && msg.FileName.Length > 255)
            {
                msg.FileName = msg.FileName.Substring(0, 255);
            }

            msg.Error = msg.Exception != null ? msg.Exception.Message + msg.Exception.StackTrace : "";

            return msg;
        
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

        void Run()
        {
            try
            {
                while (keepRunning || requestQ.Size > 0)
                {
                    Object obj = requestQ.Remove(sleepInterval);

                    if(obj == null)
                    {
                        continue;
                    }

                    LIBMessageLog job = obj as LIBMessageLog;

                    job = TransformMessage(job);

                    //LW-3792 Instead of writing diretly to the table we will just add all of the object properties to the log4net thread context so that we can use an appender. 
                    if (job != null && !_config.LibMessageLoggingDisabled)
                    {
                        log4net.ThreadContext.Properties["MessageID"] = job.MessageId;
                        log4net.ThreadContext.Properties["EnvKey"] = job.EnvKey;
                        log4net.ThreadContext.Properties["LogSource"] = job.LogSource;
                        log4net.ThreadContext.Properties["FileName"] = job.FileName;
                        log4net.ThreadContext.Properties["JobNumber"] = job.JobNumber;
                        log4net.ThreadContext.Properties["Message"] = job.Message;
                        log4net.ThreadContext.Properties["Reason"] = job.Reason;
                        log4net.ThreadContext.Properties["Error"] = job.Error;
                        log4net.ThreadContext.Properties["TryCount"] = job.TryCount;
                        log4net.ThreadContext.Properties["MsgTime"] = job.MsgTime;

                        //We are still going to Utilize the LWLogger since there is no need to duplicate all of that logic. 
                        _failedMessageLogger.Trace("MessageId:" + job.MessageId + " EnvKey:" + job.EnvKey + " LogSource:" + job.LogSource + " Message:" + job.Message + " Error:" + job.Error);

                    }
                }
            }
            catch
            {                
            }            
        }

        public ThreadState GetThreadStatus
        {
           get
            {
                return this.thrd.ThreadState;
            }
        }

        public bool GetThreadIsAlive
        {
            get
            {
                return this.thrd.IsAlive;
            }
        }
    }    
}
