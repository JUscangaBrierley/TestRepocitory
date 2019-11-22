using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
using PetaPoco;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Messages
{
	public class GetMemberMessages : OperationProviderBase
	{
		private const string _className = "GetMemberMessages";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetMemberMessages()
			: base("GetMemberMessages")
		{
		}

		public override string Invoke(string source, string parms)
		{
			const string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				string language = LanguageChannelUtil.GetDefaultCulture();
				string channel = LanguageChannelUtil.GetDefaultChannel();

				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to retrieve member messages.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				if (args.ContainsKey("Language"))
				{
					language = (string)args["Language"];
				}
				if (args.ContainsKey("Channel"))
				{
					channel = (string)args["Channel"];
				}
				DateTime? startDate = null;
				if (args.ContainsKey("StartDate"))
				{
					startDate = (DateTime)args["StartDate"];
				}
				DateTime? endDate = null;
				if (args.ContainsKey("EndDate"))
				{
					endDate = (DateTime)args["EndDate"];
				}

				List<MemberMessageStatus> statuses = new List<MemberMessageStatus>();
				if (args.ContainsKey("Status"))
				{
					string[] statusArray = (string[])args["Status"];
					if (statusArray != null && statusArray.Length > 0)
					{
						foreach (string s in statusArray)
						{
							MemberMessageStatus parsed = MemberMessageStatus.Unread;
							if (!Enum.TryParse<MemberMessageStatus>(s, out parsed))
							{
								string msg = string.Format("Invalid MemberMessageStatus '{0}' provided for GetMemberMessages.", s);
								_logger.Error(_className, methodName, msg);
								throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
							}
							statuses.Add(parsed);
						}
					}
				}

				if (statuses.Count == 0)
				{
					statuses.Add(MemberMessageStatus.Unread);
					statuses.Add(MemberMessageStatus.Read);
				}

				MemberMessageOrder order = MemberMessageOrder.Newest;
				if (args.ContainsKey("Order"))
				{
					var o = (string)args["Order"];
					if (!Enum.TryParse<MemberMessageOrder>(o, out order))
					{
						string msg = string.Format("Invalid Order '{0}' provided for GetMemberMessages.", o);
						_logger.Error(_className, methodName, msg);
						throw new LWOperationInvocationException(msg) { ErrorCode = 3304 };
					}
				}

				Member member = LoadMember(args);

				// validate language and channel
				if (!LanguageChannelUtil.IsLanguageValid(ContentService, language))
				{
					throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
				}
				if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
				{
					throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
				}

                bool activeOnly = args.ContainsKey("ActiveOnly") ? (bool)args["ActiveOnly"] : false;

                APIArguments resultParams = new APIArguments();

				//If the caller passed StartIndex and BatchSize, we'll activeOnly those for backward compatibility, for now. 
				//The preferred "parms" are PageNumber and ResultsPerPage, but the original method accepted batch info.
				bool usesPaging = !args.ContainsKey("StartIndex") && !args.ContainsKey("BatchSize");
				if (usesPaging)
				{
					long pageNumber = args.ContainsKey("PageNumber") ? (long)args["PageNumber"] : -1;
					long resultsPerPage = args.ContainsKey("ResultsPerPage") ? (long)args["ResultsPerPage"] : -1;

					if (pageNumber < 1)
					{
						throw new LWOperationInvocationException("Invalid PageNumber provided. Must be greater than zero.") { ErrorCode = 3304 };
					}
					if (resultsPerPage < 1)
					{
						throw new LWOperationInvocationException("Invalid ResultsPerPage provided. Must be greater than zero.") { ErrorCode = 3305 };
					}

					Page<MemberMessage> messages = LoyaltyDataService.GetMemberMessages(member.IpCode, statuses, activeOnly, pageNumber, resultsPerPage, startDate, endDate, order);

					resultParams.Add("TotalPages", messages.TotalPages);
					resultParams.Add("TotalItems", messages.TotalItems);

					if (messages.Items.Count > 0)
					{
						APIStruct[] memberMessages = new APIStruct[messages.Items.Count];
						int msgIdx = 0;
						foreach (MemberMessage message in messages.Items)
						{
							memberMessages[msgIdx++] = MessageUtil.SerializeMemberMessage(member, language, channel, message);
						}
						resultParams.Add("MemberMessage", memberMessages);
					}
				}
				else
				{
					//this is the obsolete way. This will be removed in a future version, but here today for backward compatibility.
					int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
					int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;
					LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

					List<MemberMessage> messages = LoyaltyDataService.GetMemberMessages(member.IpCode, statuses, activeOnly, batchInfo, startDate, endDate, order);
					if (messages.Count > 0)
					{
						APIStruct[] memberMessages = new APIStruct[messages.Count];
						int msgIdx = 0;
						foreach (MemberMessage message in messages)
						{
							memberMessages[msgIdx++] = MessageUtil.SerializeMemberMessage(member, language, channel, message);
						}
						resultParams.Add("MemberMessage", memberMessages);
					}
					else
					{
						throw new LWOperationInvocationException("No member messages found.") { ErrorCode = 3362 };
					}
				}
				response = SerializationUtils.SerializeResult(Name, Config, resultParams);
				return response;
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
