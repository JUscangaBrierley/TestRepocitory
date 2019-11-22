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
	public class LWTriggerUserEventLogger : LWThread
	{				
		private ServiceConfig _config = null;
        private LWLogger _triggerUserEventLogger = LWLoggerManager.GetLogger(LWConstants.LW_TRIGGERUSEREVENT_LOG);

        public LWTriggerUserEventLogger()
			: base("LWTriggerUserEventLogger")
		{
			_config = LWDataServiceUtil.GetServiceConfiguration();
		}

		public LWTriggerUserEventLogger(string orgName,string envName)
			: base("LWTriggerUserEventLogger")
		{
			LWConfigurationUtil.SetCurrentEnvironmentContext(orgName, envName);
			_config = LWDataServiceUtil.GetServiceConfiguration(orgName, envName);
		}

        public LWTriggerUserEventLogger(LWConfiguration config)
            : base("LWTriggerUserEventLogger")
		{
			LWConfigurationUtil.SetConfiguration(config);
			_config = LWDataServiceUtil.GetServiceConfiguration(config.Organization, config.Environment);
		}

		public override void ProcessRequest(object jobMessage)
		{
            if (_config.TriggerUserEventLoggingDisabled)
            {
                return;
            }

            TriggerUserEventLog job = jobMessage as TriggerUserEventLog;

			if (job != null)
			{
				try
				{
                    //LW-3792 Update the code to add the object properties to the log4net theread context instead of writing directly to the table. 
                    log4net.ThreadContext.Properties["Id"] = job.Id;
                    log4net.ThreadContext.Properties["MemberId"] = job.MemberId;
                    log4net.ThreadContext.Properties["EventName"] = job.EventName;
                    log4net.ThreadContext.Properties["Channel"] = job.Channel;
                    log4net.ThreadContext.Properties["Context"] = job.Context;
                    log4net.ThreadContext.Properties["RulesExecuted"] = job.RulesExecuted;
                    log4net.ThreadContext.Properties["Result"] = job.Result;
                    log4net.ThreadContext.Properties["CreateDate"] = job.CreateDate;
                    log4net.ThreadContext.Properties["UpdateDate"] = job.UpdateDate;

                    _triggerUserEventLogger.Trace("Id:" + job.Id + " MemberId:" + job.MemberId + " EventName:" + job.EventName + " RulesExecuted:" + job.RulesExecuted + " Result:" + job.Result);
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
