using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Passbook
{
	public class GetLoyaltyCardPass : OperationProviderBase
	{
		private const string _className = "GetLoyaltyCardPass";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public GetLoyaltyCardPass() : base("GetLoyaltyCardPass") { }

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			string methodName = "Invoke";

			if (parms == null || parms.Length == 0)
			{
				string errMsg = "No loyalty id provided to lookup a loyalty card.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			string loyaltyId = parms[0] as string;
			Member m = token.CachedMember;
			VirtualCard vc = m.GetLoyaltyCard(loyaltyId);
			if (vc != null)
			{
				return MGPassbookUtil.CreateLoyaltyCardPass(vc, LWDataServiceUtil.GetServiceConfiguration());
			}
			else
			{
				string errMsg = string.Format("Member with ipcode {0} does not have a loyalty card with loyalty id {1}.", m.IpCode, loyaltyId);
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}
		}
	}
}