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

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Coupons
{
    public class GetCouponDefinitions : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetCouponDefinitions";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetCouponDefinitions() : base("GetCouponDefinitions") { }

        #region Private Helpers
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 7)
            {
                string errMsg = "Invalid parameters provided for GetCouponDefinitions.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string language = (string)parms[0];
            if (string.IsNullOrEmpty(language))
            {
                language = LanguageChannelUtil.GetDefaultCulture();
            }
            string channel = (string)parms[1];
            if (string.IsNullOrEmpty(channel))
            {
                channel = LanguageChannelUtil.GetDefaultChannel();
            }
            
            List<Dictionary<string, object>> parmsList = new List<Dictionary<string, object>>();

            if (!string.IsNullOrEmpty((string)parms[2]))
            {
                bool activeOnly = bool.Parse((string)parms[2]);
                Dictionary<string, object> entry = new Dictionary<string, object>();
                entry.Add("Property", "Active");
                entry.Add("Predicate", LWCriterion.Predicate.Eq);
                entry.Add("Value", activeOnly);
                parmsList.Add(entry);
            }


            string contentAttributesStr = (string)parms[3];
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
            
            string startIndexStr = (string)parms[4];
            string batchSizeStr = (string)parms[5];
            int? startIndex = !string.IsNullOrEmpty(startIndexStr) ? (int?)int.Parse(startIndexStr) : null;
            int? batchSize = !string.IsNullOrEmpty(batchSizeStr) ? (int?)int.Parse(batchSizeStr) : null;

            LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

            string returnAttrubutesStr = (string)parms[6];
            bool returnAttributes = false;
            if (!string.IsNullOrEmpty(returnAttrubutesStr))
            {
                returnAttributes = bool.Parse(returnAttrubutesStr);
            }

            IList<CouponDef> coupons = ContentService.GetCouponDefs(parmsList, returnAttributes, batchInfo);
            if (coupons.Count == 0)
            {
                throw new LWOperationInvocationException("No coupon definitions found.") { ErrorCode = 3362 };
            }

            List<MGCouponDef> msgList = null;

            if (coupons.Count > 0)
            {
                msgList = new List<MGCouponDef>();
                foreach (CouponDef coupon in coupons)
                {
                    msgList.Add(MGCouponDef.Hydrate(coupon, language, channel, returnAttributes));
                }
            }

            return msgList;
        }
        #endregion
    }
}