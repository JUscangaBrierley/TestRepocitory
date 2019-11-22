//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository;

using Brierley.FrameWork.Common.Config;

namespace Brierley.FrameWork.Common.Logging
{
	/// <summary>
	/// This class uses  Microsoft.Enterprise.Library Logging Application Block to log messages to 
    /// a variety of message targets.
	/// </summary>
    /// <remarks>
    /// </remarks>
	[System.Diagnostics.DebuggerStepThrough]
	public class LWLogger
	{
		private ILog _logWriter;
        private string _applicationName = "";
		private static Hashtable _jobidMap = new Hashtable();

	
        // Instances are managed by LWLoggerManager
        internal LWLogger(string applicationName)
		{
            _applicationName = applicationName;
			_logWriter = LogManager.GetLogger(_applicationName);
		}


		public void AssignJobId(string jobId)
		{
			lock (_jobidMap)
			{
				try
				{
					_jobidMap.Add(System.Threading.Thread.CurrentThread.ManagedThreadId, jobId);
				}
				catch (ArgumentException)
				{
					_jobidMap.Remove(System.Threading.Thread.CurrentThread.ManagedThreadId);
					_jobidMap.Add(System.Threading.Thread.CurrentThread.ManagedThreadId, jobId);
				}
			}
		}

		public string GetJobId()
		{
			lock (_jobidMap)
			{
				return _jobidMap.Contains(System.Threading.Thread.CurrentThread.ManagedThreadId) ? (string)_jobidMap[System.Threading.Thread.CurrentThread.ManagedThreadId] : string.Empty;
			}
		}

		public void ClearJobId()
		{
			lock (_jobidMap)
			{
				if (_jobidMap.Contains(System.Threading.Thread.CurrentThread.ManagedThreadId))
				{
					_jobidMap.Remove(System.Threading.Thread.CurrentThread.ManagedThreadId);
				}
			}
		}


		/// <summary>
		/// Is "Critical" log level enabled?
		/// </summary>
		/// <returns></returns>
		public bool IsCriticalEnabled()
		{
			bool result = ShouldLog(TraceEventType.Critical, _applicationName);
			return result;
		}

        /// <summary>
        /// Is "Error" log level enabled?
        /// </summary>
        /// <returns></returns>
        public bool IsErrorEnabled()
        {
			bool result = ShouldLog(TraceEventType.Error, _applicationName);
            return result;
        }

        /// <summary>
        /// Is "Trace" log level enabled?
        /// </summary>
        /// <returns></returns>
        public bool IsTraceEnabled()
        {
			bool result = ShouldLog(TraceEventType.Information, _applicationName);
            return result;
        }
        
        /// <summary>
        /// Is "Debug" log level enabled?
        /// </summary>
        /// <returns></returns>
        public bool IsDebugEnabled()
        {
			bool result = ShouldLog(TraceEventType.Verbose, _applicationName);
            return result;
		}

		/// <summary>
		/// Log a message at "Critical" level
		/// </summary>
		/// <param name="msg">the message</param>
		public void Critical(string msg)
		{
			if (ShouldLog(TraceEventType.Critical, _applicationName))
			{
				string message = FormatMessage(msg);
				Log(TraceEventType.Critical, string.Empty, string.Empty, message);
			}
		}

		/// <summary>
		/// Log a message at "Critical" level
		/// </summary>
		/// <param name="msg">the message</param>
		/// <param name="exception">source exception</param>
		public void Critical(string msg, Exception exception)
		{
			if (ShouldLog(TraceEventType.Critical, _applicationName))
			{
				string message = FormatMessage(msg, exception);
				Log(TraceEventType.Critical, string.Empty, string.Empty, message);
			}
		}

		/// <summary>
		/// Log a message at "Critical" level
		/// </summary>
		/// <param name="className">source class name</param>
		/// <param name="methodName">source method name</param>
		/// <param name="msg">the message</param>
		public void Critical(string className, string methodName, string msg)
		{
			if (ShouldLog(TraceEventType.Critical, _applicationName))
			{
				string message = FormatMessage(className, methodName, msg);
				Log(TraceEventType.Critical, className, methodName, message);
			}
		}

		/// <summary>
		/// Log a message at "Critical" level
		/// </summary>
		/// <param name="className">source class name</param>
		/// <param name="methodName">source method name</param>
		/// <param name="msg">the message</param>
		/// <param name="exception">source exception</param>
		public void Critical(string className, string methodName, string msg, Exception exception)
		{
			FormatAndLogCritical(className, methodName, msg, exception);			
		}

		/// <summary>
		/// Log a message at "Error" level
		/// </summary>
		/// <param name="className">source class name</param>
		/// <param name="methodName">source method name</param>
		/// <param name="msg">the message</param>
		/// <param name="exception">source exception</param>
		/// <param name="logInner">If true then log the inner exceptions</param>
		[Obsolete("Inner stack trace is always logged.", false)]
		public void Critical(string className, string methodName, string msg, Exception exception, bool logInner)
		{
			Critical(className, methodName, msg, exception);
		}

		/// <summary>
		/// Log a message at "Error" level
		/// </summary>
		/// <param name="msg">the message</param>
		public void Error(string msg)
		{
			if (ShouldLog(TraceEventType.Error, _applicationName))
			{
				string message = FormatMessage(msg);
				Log(TraceEventType.Error, string.Empty, string.Empty, message);
			}
		}

		/// <summary>
        /// Log a message at "Error" level
        /// </summary>
        /// <param name="msg">the message</param>
        /// <param name="exception">source exception</param>
        public void Error(string msg, Exception exception)
        {
			if (ShouldLog(TraceEventType.Error, _applicationName))
            {
                string message = FormatMessage(msg, exception);
				Log(TraceEventType.Error, string.Empty, string.Empty, message);
            }
        }

        /// <summary>
        /// Log a message at "Error" level
        /// </summary>
        /// <param name="className">source class name</param>
        /// <param name="methodName">source method name</param>
        /// <param name="msg">the message</param>
        public void Error(string className, string methodName, string msg)
        {
			if (ShouldLog(TraceEventType.Error, _applicationName))
            {
                string message = FormatMessage(className, methodName, msg);
                Log(TraceEventType.Error, className, methodName, message);
            }
        }

        /// <summary>
        /// Log a message at "Error" level
        /// </summary>
        /// <param name="className">source class name</param>
        /// <param name="methodName">source method name</param>
        /// <param name="msg">the message</param>
        /// <param name="exception">source exception</param>
        public void Error(string className, string methodName, string msg, Exception exception)
        {
			FormatAndLogError(className, methodName, msg, exception);			
        }

		/// <summary>
		/// Log a message at "Error" level
		/// </summary>
		/// <param name="className">source class name</param>
		/// <param name="methodName">source method name</param>
		/// <param name="msg">the message</param>
		/// <param name="exception">source exception</param>
		/// <param name="logInner">If true then log the inner exceptions</param>
		[Obsolete("Inner stack trace is always logged.", false)]
		public void Error(string className, string methodName, string msg, Exception exception, bool logInner)
		{
			Error(className, methodName, msg, exception);
		}

		/// <summary>
		/// Log a message at "Warning" level
		/// </summary>
		/// <param name="msg">the message</param>
		public void Warning(string msg)
		{
			if (ShouldLog(TraceEventType.Warning, _applicationName))
			{
				string message = FormatMessage(msg);
				Log(TraceEventType.Warning, string.Empty, string.Empty, message);
			}
		}

		/// <summary>
		/// Log a message at "Warning" level
		/// </summary>
		/// <param name="msg">the message</param>
		/// <param name="exception">source exception</param>
		public void Warning(string msg, Exception exception)
		{
			if (ShouldLog(TraceEventType.Warning, _applicationName))
			{
				string message = FormatMessage(msg, exception);
				Log(TraceEventType.Warning, string.Empty, string.Empty, message);
			}
		}

		/// <summary>
		/// Log a message at "Warning" level
		/// </summary>
		/// <param name="className">source class name</param>
		/// <param name="methodName">source method name</param>
		/// <param name="msg">the message</param>
		public void Warning(string className, string methodName, string msg)
		{
			if (ShouldLog(TraceEventType.Warning, _applicationName))
			{
				string message = FormatMessage(className, methodName, msg);
                Log(TraceEventType.Warning, className, methodName, message);
			}
		}

		/// <summary>
		/// Log a message at "Warning" level
		/// </summary>
		/// <param name="className">source class name</param>
		/// <param name="methodName">source method name</param>
		/// <param name="msg">the message</param>
		/// <param name="exception">source exception</param>
		public void Warning(string className, string methodName, string msg, Exception exception)
		{
			FormatAndLogWarning(className, methodName, msg, exception);
		}

		/// <summary>
        /// Log a message at "Trace" level
        /// </summary>
        /// <param name="msg">the message</param>
		public void Trace(string msg)
		{
			if (ShouldLog(TraceEventType.Information, _applicationName))
            {
                string message = FormatMessage(msg);
				Log(TraceEventType.Information, string.Empty, string.Empty, message);
            }
		}

        /// <summary>
        /// Log a message at "Trace" level
        /// </summary>
        /// <param name="className">source class name</param>
        /// <param name="methodName">source method name</param>
        /// <param name="msg">the message</param>
        public void Trace(string className, string methodName, string msg)
        {
			if (ShouldLog(TraceEventType.Information, _applicationName))
            {
                string message = FormatMessage(className, methodName, msg);
                Log(TraceEventType.Information, className, methodName, message);
            }
		}

		/// <summary>
        /// Log a message at "Debug" level
        /// </summary>
        /// <param name="msg">the message</param>
		public void Debug(string msg)
		{
			if (ShouldLog(TraceEventType.Verbose, _applicationName))
            {
                string message = FormatMessage(msg);
				Log(TraceEventType.Verbose, string.Empty, string.Empty, message);
            }
		}
		
        /// <summary>
        /// Log a message at "Debug" level
        /// </summary>
        /// <param name="className">source class name</param>
        /// <param name="methodName">source method name</param>
        /// <param name="msg">the message</param>
		public void Debug(string className, string methodName, string msg)
		{
			if (ShouldLog(TraceEventType.Verbose, _applicationName))
            {
                string message = FormatMessage(className, methodName, msg);
                Log(TraceEventType.Verbose, className, methodName, message);
            }
		}


		private static SourceLevels GetSourceLevel(string logLevel)
        {
            SourceLevels result = SourceLevels.All;
            try
            {
                result = (SourceLevels)Enum.Parse(typeof(SourceLevels), logLevel, true);
            }
            catch (TypeInitializationException)
            {
                result = SourceLevels.All;
            }
            return result;
        }

        private static string FormatMessage(string msg)
        {
            return msg;
        }

        private static string FormatMessage(string className, string methodName, string msg)
        {
            string result = string.Empty;

            if (className != null)
                result += className + ":";

            if (methodName != null)
                result += methodName + ":";

            if (!string.IsNullOrEmpty(result))
                result += " ";

            result += msg;

            return result;
        }

        private static string FormatMessage(string msg, Exception ex)
        {
            StringBuilder strbuf = new StringBuilder(msg);
            if (ex != null)
            {
                strbuf.Append("\r\n");
                strbuf.Append("Exception Message: ");
                strbuf.Append(ex.Message);
                strbuf.Append("\r\n");
				strbuf.Append("Exception Type: ");
				strbuf.Append(ex.GetType().Name);
				strbuf.Append("\r\n");
				if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    strbuf.Append("Stack Trace: ");
                    strbuf.Append("\r\n");
                    strbuf.Append(ex.StackTrace);
					while (ex != null && ex.InnerException != null)
					{
						ex = ex.InnerException;
						strbuf.Append("\r\n");
						strbuf.Append("Inner Exception Message: ");
						strbuf.Append(ex.Message);
						strbuf.Append("\r\n");
						strbuf.Append("Inner Exception Type: ");
						strbuf.Append(ex.GetType().Name);
						strbuf.Append("\r\n");
						if (!string.IsNullOrEmpty(ex.StackTrace))
						{
							strbuf.Append("Inner Exception Stack Trace: ");
							strbuf.Append("\r\n");
							strbuf.Append(ex.StackTrace);
						}
						else
						{
							strbuf.Append("No inner exception stack trace was available.");
						}
					}
                }
                else
                {
                    strbuf.Append("No stack trace was available.");
                }
            }
            strbuf.Append("\r\n");
            return strbuf.ToString();
        }

        private static string FormatMessage(string className, string methodName, string msg, Exception ex)
        {
            string tmpMsg = FormatMessage(className, methodName, msg);
            string result = FormatMessage(tmpMsg, ex);
            return result;
        }

		private void FormatAndLogCritical(string className, string methodName, string msg, Exception exception)
		{
			if (ShouldLog(TraceEventType.Critical, _applicationName))
			{
				string message = FormatMessage(className, methodName, msg, exception);
                Log(TraceEventType.Critical, className, methodName, message);
			}
		}

		private void FormatAndLogError(string className, string methodName, string msg, Exception exception)
		{
			if (ShouldLog(TraceEventType.Error, _applicationName))
			{
				string message = FormatMessage(className, methodName, msg, exception);
                Log(TraceEventType.Error, className, methodName, message);
			}
		}

		private void FormatAndLogWarning(string className, string methodName, string msg, Exception exception)
		{
			if (ShouldLog(TraceEventType.Warning, _applicationName))
			{
				string message = FormatMessage(className, methodName, msg, exception);
                Log(TraceEventType.Warning, className, methodName, message);
			}
		}

		private bool ShouldLog(TraceEventType level, string category)
        {
            bool result = true;

			switch (level)
			{
				case TraceEventType.Critical:
					result = _logWriter.IsFatalEnabled;
					break;

				case TraceEventType.Error:
					result = _logWriter.IsErrorEnabled;
					break;

				case TraceEventType.Warning:
					result = _logWriter.IsWarnEnabled;
					break;

				case TraceEventType.Information:
					result = _logWriter.IsInfoEnabled;
					break;

				case TraceEventType.Verbose:
				default:
					result = _logWriter.IsDebugEnabled;
					break;
			}
            return result;
        }

        [ThreadStatic]
        private static string ThreadName = null;
		private void Log(TraceEventType level, string className, string methodName, string message)
		{
            ThreadName = ThreadName ?? System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            LoggingEventData logData = new LoggingEventData()
            {
                Message = message,
                LoggerName = _applicationName,
                TimeStamp = DateTime.Now,
                ThreadName = ThreadName
            };

            if (!string.IsNullOrWhiteSpace(className) || !string.IsNullOrWhiteSpace(methodName))
            {
                logData.LocationInfo = new LocationInfo(className, methodName, string.Empty, string.Empty);                
            }

			switch (level)
			{
				case TraceEventType.Critical:
					logData.Level = Level.Critical;
					break;

				case TraceEventType.Error:
					logData.Level = Level.Error;
					break;

				case TraceEventType.Warning:
					logData.Level = Level.Warn;
					break;

				case TraceEventType.Information:
					logData.Level = Level.Info;
					break;

				case TraceEventType.Verbose:
				default:
					logData.Level = Level.Verbose;
					break;
			}

			LoggingEvent logEvent = new LoggingEvent(logData);

			LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
			if (ctx != null)
			{
				logEvent.Properties["Organization"] = ctx.Organization;
				logEvent.Properties["Environment"] = ctx.Environment;
			}
			string jobID = GetJobId();
			if (!string.IsNullOrEmpty(jobID))
			{
				logEvent.Properties["JobId"] = jobID;
			}
            logEvent.GetProperties(); // Hardens threadcontext properties

			_logWriter.Logger.Log(logEvent);

		}

    }
}
