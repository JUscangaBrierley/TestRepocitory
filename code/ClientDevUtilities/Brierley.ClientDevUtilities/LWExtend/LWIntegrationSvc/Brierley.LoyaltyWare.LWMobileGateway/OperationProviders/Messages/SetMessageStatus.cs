using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using PetaPoco;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Messages
{
	public class SetMessageStatus : OperationProviderBase
	{
		private const string _className = "SetMessageStatus";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public SetMessageStatus()
			: base("SetMessageStatus")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			const string methodName = "Invoke";

			if (parms == null || parms.Length != 2)
			{
				string errMsg = "Invalid parameters provided for SetMessageStatus.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			Member member = token.CachedMember;

			long id = (long)parms[0];
			MemberMessageStatus status = (MemberMessageStatus)parms[1];

			MemberMessage message = LoyaltyService.GetMemberMessage(id);
			if (message == null)
			{
				SetResponseCode(System.Net.HttpStatusCode.NotFound);
				return null;
			}

			if (member.IpCode != message.MemberId)
			{
				SetResponseCode(System.Net.HttpStatusCode.BadRequest);
				return null;
			}

			if (message.Status != status)
			{
				message.Status = status;
				LoyaltyService.UpdateMemberMessage(message);
			}
			return null;
		}
	}
}