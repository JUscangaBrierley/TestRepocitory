//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common.Exceptions;
using System.IO;

namespace Brierley.FrameWork.Common.Logging
{
	/// <summary>
	/// This class manages LWLogger instances.  Use this pattern to log:
    /// <code>
    /// private const string _className = "MyClass";
    /// private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
    /// _logger.Error(_className, ...);
    /// </code>
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough]
	public static class LWLoggerManager
	{        
        // Be sure access to this is thread safe
        private static Hashtable _loggers = new Hashtable();

        /// <summary>
        /// Get the logger for the named application.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <returns>LWLogger</returns>
        public static LWLogger GetLogger(string applicationName)
        {
            if (string.IsNullOrEmpty(applicationName)) 
                throw new ArgumentException("Null or empty applicationName parameter");

            LWLogger logger = null;
            lock (typeof(LWLoggerManager))
            {
                if (_loggers.ContainsKey(applicationName))
                {
                    logger = (LWLogger)_loggers[applicationName];
                }
                else
                {
                    logger = new LWLogger(applicationName);
                    //logger.Trace("LWLoggerManager", "GetLogger", "Created logger for application " + applicationName);
                    _loggers.Add(applicationName, logger);
                }
            }
            return logger;
        }

        public static void InitLogger(string filePath)
        {
            if (File.Exists(filePath))
            {
                log4net.Config.XmlConfigurator.Configure(new FileInfo(filePath));
                GetLogger(LWConstants.LW_FRAMEWORK).Trace("LWLoggerManager", "InitLogger", string.Format("Initialized log4net using config file '{0}'", filePath));
            }
        }
    }
}
