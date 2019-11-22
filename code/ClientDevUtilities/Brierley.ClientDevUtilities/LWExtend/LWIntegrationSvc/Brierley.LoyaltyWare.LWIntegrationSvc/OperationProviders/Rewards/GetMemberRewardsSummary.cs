using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
	public class GetMemberRewardsSummary : MemberRewardsBase
	{
		private const string _className = "GetMemberRewardsSummary";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetMemberRewardsSummary() : base("GetMemberRewardsSummary") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to retrieve member rewards.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				long? categoryId = args.ContainsKey("CategoryId") ? (long?)args["CategoryId"] : null;
				DateTime startDate = args.ContainsKey("StartDate") ? (DateTime)args["StartDate"] : DateTimeUtil.MinValue;
				DateTime endDate = args.ContainsKey("EndDate") ? (DateTime)args["EndDate"] : DateTimeUtil.MaxValue;
				int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
				int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;
				bool unRedeemedOnly = args.ContainsKey("UnRedeemedOnly") ? (bool)args["UnRedeemedOnly"] : true;
				bool unexpiredOnly = args.ContainsKey("UnexpiredOnly") ? (bool)args["UnexpiredOnly"] : true;
				string lang = args.ContainsKey("Language") ? (string)args["Language"] : LanguageChannelUtil.GetDefaultCulture();
				string channel = args.ContainsKey("Channel") ? (string)args["Channel"] : LanguageChannelUtil.GetDefaultChannel();

				if (endDate < startDate)
				{
					_logger.Error(_className, methodName, "End date cannot be earlier than the start date");
					throw new LWOperationInvocationException("End date cannot be earlier than the start date") { ErrorCode = 3204 };
				}

				Member member = LoadMember(args);

				List<long> rewardIds = LoyaltyDataService.GetMemberRewardIds(
							member, categoryId, startDate, endDate, null, unRedeemedOnly, unexpiredOnly);
				if (rewardIds.Count == 0)
				{
					throw new LWOperationInvocationException("No member rewards found.") { ErrorCode = 3362 };
				}

				long[] ids = LWQueryBatchInfo.GetIds(rewardIds.ToArray<long>(), startIndex, batchSize, Config.EnforceValidBatch);

				IList<MemberReward> rewardsList = LoyaltyDataService.GetMemberRewardByIds(ids);

				if (rewardsList == null || rewardsList.Count == 0)
				{
					throw new LWOperationInvocationException("No member rewards found.") { ErrorCode = 3362 };
				}

				long[] vcKeys = member.GetLoyaltyCardIds();

				APIStruct[] memberRewards = new APIStruct[rewardsList.Count];
				int rewardIdx = 0;
				foreach (MemberReward reward in rewardsList)
				{
					APIStruct rv = MemberRewardSerializationUtility.SerializeMemberRewardSummary(lang, channel, vcKeys, startDate, endDate, reward, false);
					memberRewards[rewardIdx++] = rv;
				}

				APIArguments resultParams = new APIArguments();
				resultParams.Add("MemberRewardSummary", memberRewards);
				response = SerializationUtils.SerializeResult(Name, Config, resultParams);

				return response;
			}
			catch (LWException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
			}
		}
	}
}
