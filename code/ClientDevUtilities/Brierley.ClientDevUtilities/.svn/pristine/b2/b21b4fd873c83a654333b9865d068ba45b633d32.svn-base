using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Messages
{
    public class GetMessageDefinitionCount : OperationProviderBase
    {
        private const string _className = "GetMessageDefinitionCount";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

        public GetMessageDefinitionCount() : base("GetMessageDefinitionCount") { }

        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 2)
            {
                string errMsg = "Invalid parameters provided for GetMessageDefinitionCount.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
            if (!string.IsNullOrEmpty((string)parms[0]))
            {
                bool activeOnly = bool.Parse((string)parms[0]);
                Dictionary<string, object> entry = new Dictionary<string, object>();
                entry.Add("Property", "Active");
                entry.Add("Predicate", LWCriterion.Predicate.Eq);
                entry.Add("Value", activeOnly);
                parmsList.Add(entry);
            }
                        
            string contentAttributesStr = (string)parms[1];            
            if (!string.IsNullOrEmpty(contentAttributesStr))
            {
                MGContentAttribute[] contentAttributes = MGContentAttribute.ConvertFromJson(contentAttributesStr);
                foreach (MGContentAttribute ca in contentAttributes)
                {
                    Dictionary<string, object> e = new Dictionary<string, object>();
                    e.Add("Property", ca.AttributeName);
                    e.Add("Predicate", LWCriterion.Predicate.Eq);
                    e.Add("Value", ca.AttributeValue);
                    if (ca.AttributeName != "Name")
                    {
                        e.Add("IsAttribute", true);
                    }
                    parmsList.Add(e);
                }                
            }

            int count = ContentService.HowManyMessageDefs(parmsList);

            return count;
        }
    }
}