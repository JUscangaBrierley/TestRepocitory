using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Messages
{
	public class SetMemberMessageStatus : OperationProviderBase
	{
		private const string _className = "SetMemberMessageStatus";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public SetMemberMessageStatus()
			: base("GetMemberMessages")
		{
		}

		public override string Invoke(string source, string parameters)
		{
			const string methodName = "Invoke";
			try
			{
				if (string.IsNullOrEmpty(parameters))
				{
					throw new LWOperationInvocationException("No parameters provided.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parameters);

				long memberMessageId = (long)args["MemberMessageId"];

				MemberMessageStatus status = MemberMessageStatus.Unread;
				string s = (string)args["Status"];
				if (!Enum.TryParse<MemberMessageStatus>(s, out status))
				{
					string msg = string.Format("Invalid MemberMessageStatus '{0}' provided.", s);
					_logger.Error(_className, methodName, msg);
					throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
				}

				MemberMessage message = LoyaltyDataService.GetMemberMessage(memberMessageId);
				if (message == null)
				{
					throw new LWOperationInvocationException("Member message not found.") { ErrorCode = 3362 };
				}

				if (message.Status != status)
				{
					message.Status = status;
					LoyaltyDataService.UpdateMemberMessage(message);
				}
				return string.Empty;
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, ex.Message, ex);
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
	}
}
