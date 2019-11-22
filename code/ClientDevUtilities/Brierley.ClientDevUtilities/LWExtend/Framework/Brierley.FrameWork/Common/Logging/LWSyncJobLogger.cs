//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Threading;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Common.Logging
{
	public class LWSyncJobLogger : LWThread
	{				
		private ServiceConfig _config = null;
        private LWLogger _syncJobLogger = LWLoggerManager.GetLogger(LWConstants.LW_SYNCJOB_LOG);

		public LWSyncJobLogger()
			: base("SyncJobLogger")
		{
			_config = LWDataServiceUtil.GetServiceConfiguration();
		}

		public LWSyncJobLogger(string orgName,string envName)
			: base("SyncJobLogger")
		{
			LWConfigurationUtil.SetCurrentEnvironmentContext(orgName, envName);
			_config = LWDataServiceUtil.GetServiceConfiguration();
		}

		public LWSyncJobLogger(LWConfiguration config)
			: base("SyncJobLogger")
		{
			LWConfigurationUtil.SetConfiguration(config);
			_config = LWDataServiceUtil.GetServiceConfiguration();
		}

		public override void ProcessRequest(object jobMessage)
		{
            SyncJob job = jobMessage as SyncJob;

            if (job != null && !_config.SyncJobLoggingDisabled)
            {
                try
                {
                    //LW-3792 Update the code to add the object properties to the log4net theread context instead of writing directly to the table. 
                    log4net.ThreadContext.Properties["MessageId"] = job.MessageId;
                    log4net.ThreadContext.Properties["Source"] = job.Source;
                    log4net.ThreadContext.Properties["SourceEnv"] = job.SourceEnv;
                    log4net.ThreadContext.Properties["StartTime"] = job.StartTime;
                    log4net.ThreadContext.Properties["EndTime"] = job.EndTime;
                    log4net.ThreadContext.Properties["ThreadId"] = job.ThreadId;
                    log4net.ThreadContext.Properties["ExternalId"] = job.ExternalId;
                    log4net.ThreadContext.Properties["OperationName"] = job.OperationName;
                    log4net.ThreadContext.Properties["OperationParm"] = job.OperationParm;
                    log4net.ThreadContext.Properties["Status"] = job.Status;
                    log4net.ThreadContext.Properties["Response"] = job.Response;
                    log4net.ThreadContext.Properties["ElapsedTime"] = job.ElapsedTime;

                    if (job.Status == 0)
                    {
                        _syncJobLogger.Trace("Message Id:" + job.MessageId + " Status:" + job.Status + " Source:" + job.Source + " OperationName:" + job.OperationName);
                    }
                    else
                    {
                        _syncJobLogger.Error("Message Id:" + job.MessageId + " Status:" + job.Status + " Source:" + job.Source + " OperationName:" + job.OperationName);
                    }

                }
                finally
                {
                }
            }
		}

		public override void Cleanup()
		{
		}

		protected override void ChildDispose()
		{
		}
	}
}
