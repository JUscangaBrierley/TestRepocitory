using System;
using System.Collections.Specialized;
using System.Web.UI;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
//using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;

using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Common.Exceptions
{
    /// <summary>
    /// Provides exception handling for the application as configured by the 
    /// policies defined in the Web.Config or App.Config.  This class should 
    /// not be called directly by an application, but rather the 
    /// <see cref="T:Brierley.Common.Exceptions.ExceptionManager"/>
    /// should be used.
    /// </summary>
    [ConfigurationElementType(typeof(CustomHandlerData))]
    public class DefaultExceptionHandler : IExceptionHandler
    {
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ignore"></param>
        public DefaultExceptionHandler(NameValueCollection ignore)
        {
        }

        /// <summary>
        /// Handle an exception based on the defined policy
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="correlationID"></param>
        /// <returns></returns>
        public Exception HandleException(Exception ex, Guid correlationID)
        {
            _logger.Error("DefaultExceptionHandler", "HandleException", ex.Message, ex);
            return ex;
        }
    }
}
