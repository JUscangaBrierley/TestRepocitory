using System;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Bonuses
{
	public class GetMemberBonusCount : OperationProviderBase
	{
		private const string _className = "GetMemberBonusCount";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public GetMemberBonusCount()
			: base("GetMemberBonusCount")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			const string methodName = "Invoke";

			if (parms == null || parms.Length != 1)
			{
				string errMsg = "Invalid parameters provided for GetMemberBonusCount.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			bool activeOnly = false;
			if (parms[0] != null)
			{
				activeOnly = Convert.ToBoolean(parms[0]);
			}

			Member member = token.CachedMember;
			var bonuses = LoyaltyService.GetMemberBonusesByMember(member.IpCode, null, activeOnly, null);

			return bonuses.Count();
		}
	}
}