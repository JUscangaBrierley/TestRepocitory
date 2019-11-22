using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons
{
	public class UnredeemMemberCouponByCertNmbr : OperationProviderBase
    {
        private const string _className = "UnredeemMemberCouponByCertNmbr";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public UnredeemMemberCouponByCertNmbr() : base("UnredeemMemberCouponByCertNmbr") { }

		public override string Invoke(string source, string parms)
        {            
            string methodName = "Invoke";

            try
            {
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided to unredeem coupon.") { ErrorCode = 3300 };
                }                
                
                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                string certNmbr = (string)args["CertNmbr"];

                MemberCoupon mc = LoyaltyDataService.GetMemberCouponByCertNmbr(certNmbr);
                if (mc == null)
                {
                    string errMsg = string.Format("No coupon could be located using cert number {0}.", certNmbr);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWOperationInvocationException(errMsg) { ErrorCode = 3370 };
                }

				CouponUtil.UnRedeemCoupon(mc);
				
                Dictionary<string, object> context = new Dictionary<string, object>();
                context.Add("memberId", mc.MemberId);
                context.Add("coupon", mc);
                PostProcessSuccessfullInvocation(context);

                return string.Empty;
            }
            catch (LWException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }
    }
}
