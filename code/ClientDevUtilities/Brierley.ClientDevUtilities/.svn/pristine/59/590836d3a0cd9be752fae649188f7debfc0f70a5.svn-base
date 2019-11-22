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
	public class GetMemberMessages : OperationProviderBase
	{
		private const string _className = "GetMemberMessages";
		private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);

		public GetMemberMessages()
			: base("GetMemberMessages")
		{
		}

		public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
		{
			const string methodName = "Invoke";

			if (parms == null || parms.Length != 8)
			{
				string errMsg = "Invalid parameters provided for GetMemberMessages.";
				_logger.Error(_className, methodName, errMsg);
				throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
			}

			string language = (string)parms[0];
			if (string.IsNullOrEmpty(language))
			{
				language = LanguageChannelUtil.GetDefaultCulture();
			}
			string channel = (string)parms[1];
			if (string.IsNullOrEmpty(channel))
			{
				channel = LanguageChannelUtil.GetDefaultChannel();
			}

			long pageNumber = (long)parms[2];
			long resultsPerPage = (long)parms[3];

			string statusString = (string)parms[4];
			List<MemberMessageStatus> statuses = new List<MemberMessageStatus>();
			if (!string.IsNullOrEmpty(statusString))
			{
				foreach (var s in statusString.Split(','))
				{
					MemberMessageStatus status = MemberMessageStatus.Unread;
					if (Enum.TryParse<MemberMessageStatus>(s, out status))
					{
						statuses.Add(status);
					}
					else
					{
						throw new Exception(string.Format("Invalid message status {0} provided", s));
					}
				}
			}

			DateTime? startDate = null;
			string startDateString = (string)parms[6];
			if (startDateString != null)
			{
				DateTime temp = DateTime.MinValue;
				if (DateTime.TryParse(startDateString, out temp))
				{
					startDate = temp;
				}
				else
				{
					throw new Exception(string.Format("Invalid start date {0} provided.", startDateString));
				}
			}

			DateTime? endDate = null;
			string endDateString = (string)parms[7];
			if (endDateString != null)
			{
				DateTime temp = DateTime.MinValue;
				if (DateTime.TryParse(endDateString, out temp))
				{
					endDate = temp;
				}
				else
				{
					throw new Exception(string.Format("Invalid end date {0} provided.", endDateString));
				}
			}

			MemberMessageOrder order = MemberMessageOrder.Newest;
			string orderString = (string)parms[5];
			if (!string.IsNullOrEmpty(orderString))
			{
				if (!Enum.TryParse<MemberMessageOrder>(orderString, out order))
				{
					throw new Exception(string.Format("Invalid order {0} provided", orderString));
				}
			}

			Member member = token.CachedMember;

			Page<MemberMessage> messages = LoyaltyService.GetMemberMessages(member.IpCode, statuses, true, pageNumber, resultsPerPage, startDate, endDate, order);

			var ret = new MemberMessagePage()
			{
				TotalPages = messages.TotalPages,
				TotalItems = messages.TotalItems,
				Messages = new List<MGMemberMessage>()
			};
	
			if (messages.Items != null)
			{
				foreach (MemberMessage message in messages.Items)
				{
					ret.Messages.Add(MGMemberMessage.Hydrate(member, message, language, channel));
				}
			}
			return ret;
		}
	}
}