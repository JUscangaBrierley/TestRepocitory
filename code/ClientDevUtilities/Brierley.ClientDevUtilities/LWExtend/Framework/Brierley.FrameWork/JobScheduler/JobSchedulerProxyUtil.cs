using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.JobSchedulerService;

namespace Brierley.FrameWork.JobScheduler
{
    public class JobSchedulerProxyUtil
    {
        #region Fields
        private static string _className = "JobSchedulerProxyUtil";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);        
        #endregion

        public static LWJobSchedulerClient GetSchedulerClient()
        {
            string methodName = "GetSchedulerClient";

            string endPoint = System.Configuration.ConfigurationManager.AppSettings["LWSchedulerEndPoint"];
            if (string.IsNullOrEmpty(endPoint))
            {
                endPoint = "NamedPipeEndPoint";
            }
            _logger.Debug(_className,methodName,string.Format("Using endpoint {0} to create scheduler client.",endPoint));
            Brierley.FrameWork.JobSchedulerService.LWJobSchedulerClient scheduleClient = new FrameWork.JobSchedulerService.LWJobSchedulerClient(endPoint);
            return scheduleClient;
        }
    }
}
