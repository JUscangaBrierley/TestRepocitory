using System;
using System.Collections.Generic;
using System.Text;

using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace Brierley.FrameWork.Common.Logging
{
    public class LWLog4NetAppender : IAppender
    {
        private const string _loggerName = "LWLog4NetAppender";
        private string _name = "";
        private ILayout _layout = null;

        #region IAppender Members
        public void Close()
        {
        }

        public void DoAppend(LoggingEvent loggingEvent)
        {
            LWLogger logger = LWLoggerManager.GetLogger(_loggerName); // loggingEvent.LoggerName

            string message = loggingEvent.MessageObject.ToString();

            switch (loggingEvent.Level.Name.ToLower())
            {
                case "alert":
                case "critical":
                case "emergency":
                case "error":
                case "fatal":
                case "severe":
                case "warn":
                    if (loggingEvent.ExceptionObject != null)
                    {
                        logger.Error(loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName, message, loggingEvent.ExceptionObject);
                    }
                    else
                    {
                        logger.Error(loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName, message);
                    }
                    break;

                case "all":
                case "info":
                case "notice":
                case "trace":
                    if (loggingEvent.ExceptionObject != null)
                    {
                        logger.Trace(loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName, message + ", Exception: " + loggingEvent.ExceptionObject.Message);
                    }
                    else
                    {
                        logger.Trace(loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName, message);
                    }
                    break;

                case "debug":
                case "fine":
                case "finer":
                case "finest":
                case "verbose":
                    if (loggingEvent.ExceptionObject != null)
                    {
                        logger.Debug(loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName, message + ", Exception: " + loggingEvent.ExceptionObject.Message);
                    }
                    else
                    {
                        logger.Debug(loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName, message);
                    }
                    break;

                case "off":
                    break;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ILayout Layout
        {
            get { return _layout; }
            set { _layout = value; }
        }
        #endregion
    }
}
