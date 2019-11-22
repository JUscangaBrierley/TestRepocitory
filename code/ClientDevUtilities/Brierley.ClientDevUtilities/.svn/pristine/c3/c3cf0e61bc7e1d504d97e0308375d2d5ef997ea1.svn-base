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

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Rewards
{
    public class GetRewardCatalogCount : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetRewardCatalogCount";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetRewardCatalogCount() : base("GetRewardCatalogCount") { }

        #region Private Helpers
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 4)
            {
                string errMsg = "Invalid parameters provided for GetRewardCatalogCount.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string catStr = (string)parms[0];
            long? categoryId = !string.IsNullOrEmpty(catStr) ? (long?)int.Parse(catStr) : null;

            List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();
            if (!string.IsNullOrEmpty((string)parms[1]))
            {
                bool activeOnly = bool.Parse((string)parms[1]);
                Dictionary<string, object> entry = new Dictionary<string, object>();
                entry.Add("Property", "Active");
                entry.Add("Predicate", LWCriterion.Predicate.Eq);
                entry.Add("Value", activeOnly);
                parmsList.Add(entry);
            }

            if (!string.IsNullOrEmpty((string)parms[2]))
            {
                TierDef tier = LoyaltyService.GetTierDef((string)parms[2]);
                if (tier != null)
                {
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    entry.Add("Property", "TierId");
                    entry.Add("Predicate", LWCriterion.Predicate.Eq);
                    entry.Add("Value", tier.Id);
                    parmsList.Add(entry);
                }
            }

            string contentAttributesStr = (string)parms[3];            
            if (!string.IsNullOrEmpty(contentAttributesStr))
            {
                MGContentAttribute[] contentAttributes = MGContentAttribute.ConvertFromJson(contentAttributesStr);
                foreach (MGContentAttribute ca in contentAttributes)
                {
                    Dictionary<string, object> entry = new Dictionary<string, object>();
                    entry.Add("Property", ca.AttributeName);
                    entry.Add("Predicate", LWCriterion.Predicate.Eq);
                    entry.Add("Value", ca.AttributeValue);
                    entry.Add("IsAttribute", true);
                    parmsList.Add(entry);
                }                
            }

            int count = ContentService.HowManyRewardDefs(parmsList, categoryId);

            return count;
        }
        #endregion
    }
}