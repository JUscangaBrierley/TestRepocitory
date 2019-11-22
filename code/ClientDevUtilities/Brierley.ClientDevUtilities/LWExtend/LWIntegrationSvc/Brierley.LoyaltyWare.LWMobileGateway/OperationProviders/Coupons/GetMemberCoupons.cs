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
    public class GetMemberCoupons : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetMemberCoupons";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetMemberCoupons() : base("GetMemberCoupons") { }

        #region Private Helpers        
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 6)
            {
                string errMsg = "Invalid parameters provided for GetMemberCoupons.";
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
            string typeCode = (string)parms[2];

            string startIndexStr = (string)parms[3];
            string batchSizeStr = (string)parms[4];
            int? startIndex = !string.IsNullOrEmpty(startIndexStr) ? (int?)int.Parse(startIndexStr) : null;
            int? batchSize = !string.IsNullOrEmpty(batchSizeStr) ? (int?)int.Parse(batchSizeStr) : null;

            LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

            string returnAttrubutesStr = (string)parms[5];
            bool returnAttributes = false;
            if (string.IsNullOrEmpty(channel))
            {
                returnAttributes = bool.Parse(returnAttrubutesStr);
            }

            Member member = token.CachedMember;

            IList<MemberCoupon> coupons = null;
            if (string.IsNullOrEmpty(typeCode))
            {
                coupons = LoyaltyService.GetMemberCouponsByMember(member.IpCode, batchInfo);
            }
            else
            {
                coupons = LoyaltyService.GetMemberCouponsByMemberByTypeCode(member.IpCode, typeCode, batchInfo);
            }

            List<MGMemberCoupon> mcList = null;

            if (coupons.Count > 0)
            {
                mcList = new List<MGMemberCoupon>();
                foreach (MemberCoupon coupon in coupons)
                {
                    mcList.Add(MGMemberCoupon.Hydrate(member, coupon, language, channel, returnAttributes));
                }
            }
            else
            {
                throw new LWOperationInvocationException("No member coupons found.") { ErrorCode = 3362 };
            }

            return mcList;
        }
        #endregion
    }
}