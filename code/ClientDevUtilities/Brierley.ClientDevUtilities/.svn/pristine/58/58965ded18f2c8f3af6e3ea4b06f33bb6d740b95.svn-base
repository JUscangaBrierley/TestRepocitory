////-----------------------------------------------------------------------
////(C) 2008 Brierley & Partners.  All Rights Reserved
////THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
////-----------------------------------------------------------------------


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Brierley.FrameWork.Common;
//using Brierley.FrameWork.Common.Config;
//using Brierley.FrameWork.Common.Logging;
//using Brierley.FrameWork.Common.Threading;
//using Brierley.FrameWork.Data;
//using Brierley.FrameWork.Data.DomainModel;

//namespace Brierley.FrameWork.Common.Logging
//{
//    public class LWBillingMetricLogger : LWThread
//    {				
//        private ILWDataService service = null;

//        public LWBillingMetricLogger()
//            : base("BillingMetricLogger")
//        {
//            service = LWDataServiceUtil.DataServiceInstance(false);
//        }

//        public LWBillingMetricLogger(string orgName,string envName)
//            : base("BillingMetricLogger")
//        {
//            LWConfigurationUtil.SetCurrentEnvironmentContext(orgName, envName);
//            service = LWDataServiceUtil.DataServiceInstance(false);
//        }

//        public LWBillingMetricLogger(LWConfiguration config)
//            : base("BillingMetricLogger")
//        {
//            LWConfigurationUtil.SetConfiguration(config);
//            service = LWDataServiceUtil.DataServiceInstance(false);
//        }

//        public override void ProcessRequest(object jobMessage)
//        {
//            BillingMetric metric = jobMessage as BillingMetric;

//            if (metric != null)
//            {
//                try
//                {
//                    service.CreateBillingMetric(metric);
//                }
//                finally
//                {
//                }
//            }
//        }

//        public override void Cleanup()
//        {
//        }

//        protected override void ChildDispose()
//        {
//        }
//    }
//}
