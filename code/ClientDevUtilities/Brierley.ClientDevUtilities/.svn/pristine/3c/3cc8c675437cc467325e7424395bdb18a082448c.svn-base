using System;
using System.Collections.Generic;
using System.Text;

using Common.Logging;

namespace Brierley.FrameWork.Common.Logging
{
	[System.Diagnostics.DebuggerStepThrough]
    public class LWCommonLogger : ILog
    {
        private LWLogger logger;

        public LWCommonLogger(LWLogger logger)
        {
            this.logger = logger;
        }

        #region ILog Members
        public void Fatal(object message, Exception exception)
        {
            logger.Error(message.ToString(), exception);
        }

        public void Fatal(object message)
        {
            logger.Error(message.ToString());
        }

        public void Error(object message, Exception exception)
        {
            logger.Error(message.ToString(), exception);
        }

        public void Error(object message)
        {
            logger.Error(message.ToString());
        }

        public void Warn(object message, Exception exception)
        {
            logger.Error(message.ToString(), exception);
        }

        public void Warn(object message)
        {
            logger.Error(message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            logger.Trace(message.ToString() + ", Exception: " + exception.Message);
        }

        public void Info(object message)
        {
            logger.Trace(message.ToString());
        }

        public void Trace(object message, Exception exception)
        {
            logger.Trace(message.ToString() + ", Exception: " + exception.Message);
        }

        public void Trace(object message)
        {
            logger.Trace(message.ToString());
        }

        public void Debug(object message, Exception exception)
        {
            logger.Debug(message.ToString() + ", Exception: " + exception.Message);
        }

        public void Debug(object message)
        {
            logger.Debug(message.ToString());
        }

        public bool IsFatalEnabled
        {
            get { return logger.IsErrorEnabled(); }
        }

        public bool IsErrorEnabled
        {
            get { return logger.IsErrorEnabled(); }
        }

        public bool IsWarnEnabled
        {
            get { return logger.IsErrorEnabled(); }
        }

        public bool IsInfoEnabled
        {
            get { return logger.IsTraceEnabled(); }
        }

        public bool IsTraceEnabled
        {
            get { return logger.IsTraceEnabled(); }
        }

        public bool IsDebugEnabled
        {
            get { return logger.IsDebugEnabled(); }
        }
        #endregion

        
        #region Unsupported Methods from ILog
        public void Debug(Action<FormatMessageHandler> formatMessageCallback) { }
        public void Debug(Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback) { }
        public void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }        
        public void DebugFormat(string format, params object[] args) { }
        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args) { }
        public void DebugFormat(string format, Exception exception, params object[] args) { }
        public void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args) { }

        public void Error(Action<FormatMessageHandler> formatMessageCallback) { }
        public void Error(Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback) { }
        public void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void ErrorFormat(string format, params object[] args) { }
        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args) { }
        public void ErrorFormat(string format, Exception exception, params object[] args) { }
        public void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args) { }

        public void Fatal(Action<FormatMessageHandler> formatMessageCallback) { }
        public void Fatal(Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback) { }
        public void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void FatalFormat(string format, params object[] args) { }
        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args) { }
        public void FatalFormat(string format, Exception exception, params object[] args) { }
        public void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args) { }

        public void Info(Action<FormatMessageHandler> formatMessageCallback) { }
        public void Info(Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback) { }
        public void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void InfoFormat(string format, params object[] args) { }
        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args) { }
        public void InfoFormat(string format, Exception exception, params object[] args) { }
        public void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args) { }

        public void Trace(Action<FormatMessageHandler> formatMessageCallback) { }
        public void Trace(Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback) { }
        public void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void TraceFormat(string format, params object[] args) { }
        public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args) { }
        public void TraceFormat(string format, Exception exception, params object[] args) { }
        public void TraceFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args) { }

        public void Warn(Action<FormatMessageHandler> formatMessageCallback) { }
        public void Warn(Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback) { }
        public void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception) { }
        public void WarnFormat(string format, params object[] args) { }
        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args) { }
        public void WarnFormat(string format, Exception exception, params object[] args) { }
        public void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args) { } 
        #endregion         
    }
}
