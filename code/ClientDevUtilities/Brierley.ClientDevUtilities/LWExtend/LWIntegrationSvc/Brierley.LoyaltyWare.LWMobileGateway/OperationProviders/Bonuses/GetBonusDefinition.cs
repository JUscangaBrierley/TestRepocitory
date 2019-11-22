using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Bonuses
{
	public class GetBonusDefinition : OperationProviderBase
	{
		private const string _className = "GetBonusDefinition";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public GetBonusDefinition()
			: base("GetBonusDefinition")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parameters)
		{
			const string methodName = "Invoke";

			if (parameters == null || parameters.Length != 4)
			{
				string errMsg = "Invalid parameters provided for GetBonusDefinition.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			long id = (long)parameters[0];
			string language = (string)parameters[1];
			if (string.IsNullOrEmpty(language))
			{
				language = LanguageChannelUtil.GetDefaultCulture();
			}

			string channel = (string)parameters[2];
			if (string.IsNullOrEmpty(channel))
			{
				channel = LanguageChannelUtil.GetDefaultChannel();
			}

			bool returnAttributes = (bool)parameters[3];

			MGBonusDef ret = null;
			var bonus = ContentService.GetBonusDef(id);
			if (bonus != null)
			{
				ret = MGBonusDef.Hydrate(bonus, language, channel, returnAttributes);
			}
			return ret;
		}
	}
}