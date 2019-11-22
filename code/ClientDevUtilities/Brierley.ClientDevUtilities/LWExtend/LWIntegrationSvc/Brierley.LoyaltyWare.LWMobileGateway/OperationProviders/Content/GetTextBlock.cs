using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.FrameWork.Data;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Content
{
	public class GetTextBlock : OperationProviderBase
	{
		private const string _className = "GetTextBlock";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public GetTextBlock() : base("GetTextBlock") { }

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			string methodName = "Invoke";

			if (parms == null || parms.Length != 3)
			{
				string errMsg = "Invalid parameters provided for GetTextBlock.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			string name = parms[0] as string;
			string language = parms[1] as string;
			string channel = parms[2] as string;
			string ret = string.Empty;

			TextBlock tb = ContentService.GetTextBlock(name);
			if (tb != null)
			{
				ret = tb.GetContent(language, channel);
			}
			return ret;
		}
	}
}