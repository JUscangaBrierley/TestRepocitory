//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.Common.Config;

namespace Brierley.FrameWork.Common.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public class LWFailedMessageLogManager
    {
        private static Hashtable loggerTable = new Hashtable();
        private static bool shuttingDown = false;

        #region Helper Methods
        private static string GetEnvironmentKey(LWConfiguration config)
        {
			return config.Organization + "_" + config.Environment;
        }
        private static LWFailedMessageLogger GetLogger(string key)
        {
            LWFailedMessageLogger logger = null;
            if (!string.IsNullOrEmpty(key))
            {
                lock (loggerTable)
                {
                    return (LWFailedMessageLogger)loggerTable[key];
                }                
            }
            return logger;
        }
        private static LWFailedMessageLogger GetLogger(LWConfiguration config)
        {
            return GetLogger(GetEnvironmentKey(config));            
        }
        #endregion

        public static void Initialize(LWConfiguration config)
        {
            if (!shuttingDown)
            {
                string key = GetEnvironmentKey(config);
                LWFailedMessageLogger logger = null;
                if (!string.IsNullOrEmpty(key))
                {
                    lock (loggerTable)
                    {
                        logger = (LWFailedMessageLogger)loggerTable[key];
                        if (logger == null)
                        {
                            logger = new LWFailedMessageLogger(config);
                            logger.Start();
                            loggerTable.Add(key, logger);
                        }
                    }
                }
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="message"></param>
        public static void LogFailedMessage(LWConfiguration config, LIBMessageLog message)
        {
            if (!shuttingDown)
            {
                LWFailedMessageLogger logger = GetLogger(config);
                if (logger != null)
                {
                    if (string.IsNullOrEmpty(message.EnvKey))
                    {
                        message.EnvKey = GetEnvironmentKey(config);
                    }
                    logger.RequestQueue.Add(message);

                    if(!logger.GetThreadIsAlive)
                    {
                        logger.Start();
                    }
                }
            }
        }        

        /// <summary>
        /// 
        /// </summary>
        public static void Shutdown()
        {           
            shuttingDown = true;
            lock (loggerTable)
            {
                foreach (string key in loggerTable.Keys)
                {
                    LWFailedMessageLogger logger = (LWFailedMessageLogger)loggerTable[key];
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
