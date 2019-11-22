using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.ClientDevUtilities.LWGateway
{
    public class LWDataServiceUtil : ILWDataServiceUtil
    {
        private const string CLASS_NAME = "LWDataServiceUtil";

        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        public static LWDataServiceUtil Instance { get; private set; }

        static LWDataServiceUtil()
        {
            Instance = new LWDataServiceUtil();
        }

        public IDataService DataServiceInstance()
        {
            var ctx = GetCurrentContext();
            return DataServiceInstance(ctx.Organization, ctx.Environment);
        }

        public IDataService DataServiceInstance(LWConfiguration config)
        {
            return DataServiceInstance(config.Organization, config.Environment);
        }

        public IDataService DataServiceInstance(string orgName, string envName)
        {
            var config = Brierley.FrameWork.Data.LWDataServiceUtil.GetServiceConfiguration(orgName, envName);
            return new DataService(config);
        }

        public IContentService ContentServiceInstance()
        {
            var ctx = GetCurrentContext();
            return ContentServiceInstance(ctx.Organization, ctx.Environment);
        }

        public IContentService ContentServiceInstance(LWConfiguration config)
        {
            return ContentServiceInstance(config.Organization, config.Environment);
        }

        public IContentService ContentServiceInstance(string orgName, string envName)
        {
            var config = Brierley.FrameWork.Data.LWDataServiceUtil.GetServiceConfiguration(orgName, envName);
            return new ContentService(config);
        }

        public ILoyaltyDataService LoyaltyDataServiceInstance()
        {
            var ctx = GetCurrentContext();
            return LoyaltyDataServiceInstance(ctx.Organization, ctx.Environment);
        }

        public ILoyaltyDataService LoyaltyDataServiceInstance(LWConfiguration config)
        {
            return LoyaltyDataServiceInstance(config.Organization, config.Environment);
        }

        public ILoyaltyDataService LoyaltyDataServiceInstance(string orgName, string envName)
        {
            var config = Brierley.FrameWork.Data.LWDataServiceUtil.GetServiceConfiguration(orgName, envName);
            return new LoyaltyDataService(config);
        }

        public IEmailService EmailServiceInstance()
        {
            var ctx = GetCurrentContext();
            return EmailServiceInstance(ctx.Organization, ctx.Environment);
        }

        public IEmailService EmailServiceInstance(LWConfiguration config)
        {
            return EmailServiceInstance(config.Organization, config.Environment);
        }

        public IEmailService EmailServiceInstance(string orgName, string envName)
        {
            var config = Brierley.FrameWork.Data.LWDataServiceUtil.GetServiceConfiguration(orgName, envName);
            return new EmailService(config);
        }

        public ISurveyManager SurveyManagerInstance()
        {
            var ctx = GetCurrentContext();
            return SurveyManagerInstance(ctx.Organization, ctx.Environment);
        }

        public ISurveyManager SurveyManagerInstance(LWConfiguration config)
        {
            return SurveyManagerInstance(config.Organization, config.Environment);
        }

        public ISurveyManager SurveyManagerInstance(string orgName, string envName)
        {
            var config = Brierley.FrameWork.Data.LWDataServiceUtil.GetServiceConfiguration(orgName, envName);
            return new SurveyManager(config);
        }

        public LWConfigurationContext GetCurrentContext()
        {
            LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
            if (ctx == null)
            {
                string msg = string.Format("Unable to determine the current environment context. Please ensure that the environment context is set.");
                _logger.Critical(CLASS_NAME, "GetCurrentContext", msg);
                throw new LWConfigurationException(msg);
            }
            return ctx;
        }
    }
}
