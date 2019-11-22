//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Brierley.FrameWork.Common
{
    public static class LWConstants
    {
        public const string LW_LOYALTYNAVIGATOR_DATAMODEL_SERVICE = "LNDataModelService";
        public const string LW_LOYALTYNAVIGATOR_MANAGEEMAIL_SERVICE = "ManageEmails";
        public const string LW_LOYALTYNAVIGATOR_MANAGEMENT = "LNManagement";
        public const string LW_CONTENT_MANAGEMENT = "LWContentManagement";
        public const string LW_EMAIL = "LWEmail";
        public const string LW_SMS = "LWSms";
        public const string LW_PUSH = "LWPush";
        public const string LW_LOYALTYNAVIGATOR = "LoyaltyNavigator";
        public const string LW_FRAMEWORK = "LoyaltyWareFramework";
        public const string LW_WEBFRAMEWORK = "LoyaltyWareWebFramework";
        public const string LW_PORTALMODULESDK = "PortalModuleSDK";
        public const string LW_JOBSCHEDULER = "LoyaltyWareJobScheduler";
        public const string LW_CUSTOMERSERVICE = "CustomerService";
        //public const string LW_LWINTEGRATION_COMMON = "LWIntegrationCommon";
        public const string LW_DAP_SERVICE = "LWDataAcquisitionService";
        public const string LW_LWINTEGRATION_SERVICE = "LWIntegrationService";
        public const string LW_RULESPROCESSING_SERVICE = "LWRulesProcessor";
        public const string LW_DATAMIGRATION = "DataMigration";
        public const string LW_MOBILEGATEWAY_SERVICE = "LWMobileGatewayService";
        public const string LW_PASSBOOK_SERVICE = "PassbookService";
        public const string LW_NOTIFICATION_SERVICE = "NotificationService";
        public const string LW_QueueProcessor = "LoyaltyWareQueueProcessor";

        public static string LW_PORTALMODULES = "LWPortalModules";

        public static string LW_AUCTIONS = "Auction";

        public const string LW_TRACE = "Trace";

        public const string LW_LWINTGRSRVC_PAYLOAD = "APIPayload";

        //LW-3792 Adding this constants as they will be used as the names that we will use for the log4net appenders
        public const string LW_RULEEXECUTION_LOG = "RuleExecutionLogger";
        public const string LW_SYNCJOB_LOG = "SyncJobLogger";
        public const string LW_ASYNCJOB_LOG = "AsyncJobLogger";
        public const string LW_ASYNCPROCESSEDOBJECTS_LOG = "AsyncProcessedObjectsLogger";
        public const string LW_TRIGGERUSEREVENT_LOG = "TriggerUserEventLogger";
        public const string LW_FAILEDMESSAGE_LOG = "FailedMessageLogger";
    }
}
