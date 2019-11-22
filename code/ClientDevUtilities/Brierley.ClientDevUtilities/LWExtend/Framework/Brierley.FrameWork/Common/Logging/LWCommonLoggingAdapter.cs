using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Brierley.FrameWork.Common.Logging
{
	[System.Diagnostics.DebuggerStepThrough]
    public class LWCommonLoggingAdapter : global::Common.Logging.ILoggerFactoryAdapter
    {
        private string _level = "INFO";

        public LWCommonLoggingAdapter()
        {
        }

        public LWCommonLoggingAdapter(NameValueCollection props)
        {
            foreach (string key in props.Keys)
            {
                switch (key.ToLower())
                {
                    case "level":
                        _level = props[key];
                        break;
                }
            }
        }

        #region ILoggerFactoryAdapter Members
        public global::Common.Logging.ILog GetLogger(string name)
        {
            LWLogger logger = LWLoggerManager.GetLogger(name);
            //logger.SetLogLevel(name, GetMappedLogLevel());
            global::Common.Logging.ILog result = new LWCommonLogger(logger);
            return result;
        }

        public global::Common.Logging.ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }
        #endregion

        private SourceLevels GetMappedLogLevel()
        {
            SourceLevels result = SourceLevels.Information;
            try
            {
                result = (SourceLevels)Enum.Parse(typeof(SourceLevels), _level, true);
            }
            catch (Exception)
            {
                result = SourceLevels.Information;
            }
            return result;
        }
    }
}
