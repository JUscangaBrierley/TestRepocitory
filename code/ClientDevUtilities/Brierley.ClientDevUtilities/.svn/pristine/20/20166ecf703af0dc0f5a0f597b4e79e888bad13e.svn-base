using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Bonuses
{
	public class RemoveBonus : OperationProviderBase
	{
		private const string _className = "RemoveBonus";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public RemoveBonus()
			: base("RemoveBonus")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parameters)
		{
			const string methodName = "Invoke";

			if (parameters == null || parameters.Length != 1)
			{
				string errMsg = "Invalid parameters provided for RemoveBonus.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			long bonusId = 0;
			if (parameters[0] != null)
			{
				bonusId = Convert.ToInt64(parameters[0]);
			}
			else
			{
				string errMsg = string.Format("No bonus id provided.");
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 3218 };
			}

			var memberBonus = LoyaltyService.GetMemberOffer(bonusId);
			if (memberBonus != null && memberBonus.Status != MemberBonusStatus.Saved)
			{
				memberBonus.Status = MemberBonusStatus.Saved;
				LoyaltyService.UpdateMemberOffer(memberBonus);
			}
			return null;
		}
	}
}