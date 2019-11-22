//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Config;
using System.Collections.Generic;

namespace Brierley.FrameWork.Common.Logging
{

    /// <summary>
    /// LW-3792 This class will manage all log entries that use the AsyncJobProcessedObjectsLogger, this was class was devloped by using the LWFailedMessageLogManager as a template
    /// </summary>
    public class LWProcessedObjectsLogManager
    {
        private static Hashtable loggerTable = new Hashtable();
        private static bool shuttingDown = false;


        private static string GetEnvironmentKey(LWConfiguration config)
        {
            return config.Organization + "_" + config.Environment;
        }


        private static LWAsyncJobProcessedObjectsLogger GetLogger(string key)
        {
            LWAsyncJobProcessedObjectsLogger logger = null;
            if (!string.IsNullOrEmpty(key))
            {
                lock (loggerTable)
                {
                    return (LWAsyncJobProcessedObjectsLogger)loggerTable[key];
                }
            }
            return logger;
        }

        private static LWAsyncJobProcessedObjectsLogger GetLogger(LWConfiguration config)
        {
            return GetLogger(GetEnvironmentKey(config));
        }
 
        /// <summary>
        /// Initializes the logger
        /// </summary>
        /// <param name="config"></param>
        public static void Initialize(LWConfiguration config)
        {
            if (!shuttingDown)
            {
                string key = GetEnvironmentKey(config);
                LWAsyncJobProcessedObjectsLogger logger = null;
                if (!string.IsNullOrEmpty(key))
                {
                    lock (loggerTable)
                    {
                        logger = (LWAsyncJobProcessedObjectsLogger)loggerTable[key];
                        if (logger == null)
                        {
                            logger = new LWAsyncJobProcessedObjectsLogger(config);
                            logger.Start();
                            loggerTable.Add(key, logger);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Process a list of objects that need to be logged to the table
        /// </summary>
        /// <param name="config"></param>
        /// <param name="message"></param>
        public static void LogMessages(LWConfiguration config, List<AsyncJobProcessedObjects> messages)
        {
            if (!shuttingDown)
            {
                LWAsyncJobProcessedObjectsLogger logger = GetLogger(config);
                if (logger != null)
                {
                    foreach (AsyncJobProcessedObjects message in messages)
                    {
                        logger.RequestQueue.Add(message);
                    }
                }
            }
        }

        /// <summary>
        /// Shutdowns the logger
        /// </summary>
        public static void Shutdown()
        {
            shuttingDown = true;
            lock (loggerTable)
            {
                foreach (string key in loggerTable.Keys)
                {
                    LWAsyncJobProcessedObjectsLogger logger = (LWAsyncJobProcessedObjectsLogger)loggerTable[key];
                    if (logger != null)
                    {
                        logger.Shutdown();
                        logger.WaitToFinish();
                    }
                }
                loggerTable.Clear();
            }
        }
    }
}
