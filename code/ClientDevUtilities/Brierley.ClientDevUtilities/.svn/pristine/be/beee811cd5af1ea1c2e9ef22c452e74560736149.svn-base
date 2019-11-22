using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using System.Net;
using System.Web;
using System.ServiceModel.Web;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.OperationProviders;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Passbook
{
    public class GetPassFromMTouch : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetPassFromMTouch";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        private const string RFC1123_DATE_FORMAT = "ddd, dd MMM yyyy hh:mm:ss GMT";
        #endregion

        #region Constructor
        public GetPassFromMTouch() : base("GetPassFromMTouch") { }
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length != 2)
            {
                string errMsg = "Invalid parameters provided for GetPassFromMTouch.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string passType = (string)parms[0];
            string mtouch = (string)parms[1];

            if (string.IsNullOrEmpty(mtouch))
            {
                string errMsg = "No MTouch provided.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            MTouch mt = LoyaltyService.GetMTouch(mtouch);
            if (mt == null)
            {
                string errMsg = "Unable to find mtouch given the mtouch id " + mtouch + ".";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            if (mt.MTouchType != MTouchType.Coupon && mt.MTouchType != MTouchType.LoyaltyCard)
            {
                string errMsg = string.Format("Invalid type of MTouch {0} provided.", mt.MTouchType.ToString());
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            _logger.Debug(_className, methodName,
                string.Format("Processing MTouch id {0} of Type {1}.", mt.ID, mt.MTouchType.ToString()));

            byte[] response = null;

            if (mt.MTouchType == MTouchType.LoyaltyCard)
            {
                // now retrieve the member based on the VcKey in the MTouch
                // entity id = IpCode
                // secondary id = loyalty id number

                Member m = LoyaltyService.LoadMemberFromIPCode(long.Parse(mt.EntityId));
                VirtualCard vc = m.GetLoyaltyCard(mt.SecondaryId);

                response = MGPassbookUtil.CreateLoyaltyCardPass(vc, LWDataServiceUtil.GetServiceConfiguration());
            }
            else if (mt.MTouchType == MTouchType.Coupon)
            {
                // now retrieve the member based on the VcKey in the MTouch
                // entity id = MemberCouponId

                MemberCoupon mc = LoyaltyService.GetMemberCoupon(long.Parse(mt.EntityId));

            }

            return new MemoryStream(response);
        }
        #endregion
    }
}