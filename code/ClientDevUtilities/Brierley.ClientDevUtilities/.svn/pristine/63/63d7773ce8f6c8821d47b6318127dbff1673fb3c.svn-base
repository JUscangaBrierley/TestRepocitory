//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Threading;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Common.Logging
{
    /// <summary>
    /// LW-3792 Logs entries to the rule exectuion log table, this doesn't write directly to the table instead uses log4net appenders. 
    /// </summary>
    public class LWRuleExecutionLogger
    {

        private ServiceConfig _config = null;
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_RULEEXECUTION_LOG);

        public LWRuleExecutionLogger()
        {
            _config = LWDataServiceUtil.GetServiceConfiguration();
        }

        public LWRuleExecutionLogger(string orgName, string envName)
        {
            LWConfigurationUtil.SetCurrentEnvironmentContext(orgName, envName);
            _config = LWDataServiceUtil.GetServiceConfiguration(orgName, envName);
        }

        public LWRuleExecutionLogger(LWConfiguration config)
        {
            LWConfigurationUtil.SetConfiguration(config);
            _config = LWDataServiceUtil.GetServiceConfiguration(config.Organization, config.Environment);
        }

        /// <summary>
        /// Process the request by adding to the Log4net global context properties
        /// </summary>
        /// <param name="jobMessage"></param>
        public void ProcessRequest(RuleExecutionLog log)
        {
            //if loggin has been disabled don't do anything
            if (_config.RuleExecutionLoggingDisabled)
            {
                return;
            }

            if (log != null)
            {
                try
                {
                    //This is how log4net will be able to reference the all the properties of the RuleExectionLog object
                    log4net.ThreadContext.Properties["Id"] = log.Id;
                    log4net.ThreadContext.Properties["RuleName"] = log.RuleName;
                    log4net.ThreadContext.Properties["MemberId"] = log.MemberId;
                    log4net.ThreadContext.Properties["ExecutionStatus"] = log.ExecutionStatus;
                    log4net.ThreadContext.Properties["ExecutionMode"] = log.ExecutionMode;
                    log4net.ThreadContext.Properties["OwnerType"] = log.OwnerType;
                    log4net.ThreadContext.Properties["OwnerId"] = log.OwnerId;
                    log4net.ThreadContext.Properties["RowKey"] = log.RowKey;
                    log4net.ThreadContext.Properties["SkipReason"] = log.SkipReason;
                    log4net.ThreadContext.Properties["Detail"] = log.Detail;
                    log4net.ThreadContext.Properties["CreateDate"] = log.CreateDate;
                    log4net.ThreadContext.Properties["UpdateDate"] = log.UpdateDate;


                    //If it was an error we will log it as such
                    if (log.ExecutionStatus == RuleExecutionStatus.Error)
                    {
                        _logger.Error("ID:" + log.Id + " RuleName:" + log.RuleName + " MemberId:" + log.MemberId);
                    }
                    else
                    {
                        _logger.Trace("ID:" + log.Id + " RuleName:" + log.RuleName + " MemberId:" + log.MemberId);
                    }
                }
                finally
                {
                }
            }
        }


    }
}
