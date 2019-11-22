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
using Brierley.FrameWork.WalletPay;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Passbook
{
	internal class MGPassbookUtil
	{
        private const string _className = "GetPassFromMTouch";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        private const string RFC1123_DATE_FORMAT = "ddd, dd MMM yyyy hh:mm:ss GMT";

        internal static byte[] CreateLoyaltyCardPass(VirtualCard vc, ServiceConfig config)
        {
            string methodName = "CreateLoyaltyCardPass";
			using (var mobile = new MobileDataService(config))
			{
				var memberPass = mobile.GetMemberAppleWalletLoyaltyCardByMemberId(vc.IpCode);

                // Make sure LWContentRootPath has been defined
                string imageRoot = LWConfigurationUtil.GetConfigurationValue("LWContentRootPath");
                if (string.IsNullOrEmpty(imageRoot))
                {
                    _logger.Error(_className, methodName, "LWContentRootPath must be defined in order to generate passbook passes.");
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                    return new byte[0];
                }

                var response = AppleWalletUtil.GenerateLoyaltyCard(vc.Member, null, memberPass);
                WebOperationContext.Current.OutgoingResponse.ContentType = "application/vnd.apple.pkpass";
                return response;
            }
        }

        internal static byte[] CreateCouponPass(MemberCoupon mc, ServiceConfig config)
        {
            string methodName = "CreateCouponPass";
            return new byte[0]; // Currently unsupported
        }
	}
}