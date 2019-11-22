using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork.LWIntegration.Util;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Bonuses
{
	public class BonusAction : OperationProviderBase
	{
		private const string _className = "BonusAction";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public BonusAction()
			: base("BonusAction")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parameters)
		{
			const string methodName = "Invoke";

			if (parameters == null || parameters.Length != 4)
			{
				string errMsg = "Invalid parameters provided for BonusAction.";
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

			MemberBonusStatus newStatus = (MemberBonusStatus)parameters[1];

			string language = (string)parameters[2];
			if (string.IsNullOrEmpty(language))
			{
				language = LanguageChannelUtil.GetDefaultCulture();
			}

			string channel = (string)parameters[3];
			if (string.IsNullOrEmpty(channel))
			{
				channel = LanguageChannelUtil.GetDefaultChannel();
			}


			//string awardPointRule = GetFunctionParameter("AwardPointRule");
			string completedEvent = GetFunctionParameter("BonusCompletedEvent");

			MemberBonus bonus = LoyaltyService.GetMemberOffer(bonusId);
			return RfeUtil.ProcessBonusAction(bonusId, newStatus, language, channel, completedEvent, -1, false);
		}
	}
}