using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
	public class GetMemberWallet : OperationProviderBase
	{
		private const string _className = "GetMemberWallet";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

		public GetMemberWallet() : base("GetMemberWallet") { }

		public override string Invoke(string source, string parms)
		{
			string methodName = "Invoke";
			try
			{
				string response = string.Empty;
				if (string.IsNullOrEmpty(parms))
				{
					throw new LWOperationInvocationException("No parameters provided to retrieve member wallet.") { ErrorCode = 3300 };
				}

				APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
				bool returnRewards = args.ContainsKey("ReturnRewards") ? (bool)args["ReturnRewards"] : false;
				bool returnCoupons = args.ContainsKey("ReturnCoupons") ? (bool)args["ReturnCoupons"] : false;
				bool returnPromotions = args.ContainsKey("ReturnPromotions") ? (bool)args["ReturnPromotions"] : false;
				long? categoryId = args.ContainsKey("CategoryId") ? (long?)args["CategoryId"] : null;
				bool unRedeemedOnly = args.ContainsKey("UnRedeemedOnly") ? (bool)args["UnRedeemedOnly"] : true;
				bool unexpiredOnly = args.ContainsKey("UnexpiredOnly") ? (bool)args["UnexpiredOnly"] : true;
				bool returnAttributes = args.ContainsKey("ReturnContentAttributes") ? (bool)args["ReturnContentAttributes"] : false;
				bool returnPromotionDefinition = args.ContainsKey("ReturnPromotionDefinition") ? (bool)args["ReturnPromotionDefinition"] : false;
				int? startIndex = args.ContainsKey("StartIndex") ? (int?)args["StartIndex"] : null;
				int? batchSize = args.ContainsKey("BatchSize") ? (int?)args["BatchSize"] : null;

				DateTime startDate = args.ContainsKey("StartDate") ? (DateTime)args["StartDate"] : DateTimeUtil.MinValue;
				DateTime endDate = args.ContainsKey("EndDate") ? (DateTime)args["EndDate"] : DateTimeUtil.MaxValue;
				if (endDate < startDate)
				{
					_logger.Error(_className, methodName, "End date cannot be earlier than the start date");
					throw new LWOperationInvocationException("End date cannot be earlier than the start date") { ErrorCode = 3204 };
				}

				string lang = args.ContainsKey("Language") ? (string)args["Language"] : LanguageChannelUtil.GetDefaultCulture();
				string channel = args.ContainsKey("Channel") ? (string)args["Channel"] : LanguageChannelUtil.GetDefaultChannel();
				if (!LanguageChannelUtil.IsLanguageValid(ContentService, lang))
				{
					throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
				}
				if (!LanguageChannelUtil.IsChannelValid(ContentService, channel))
				{
					throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
				}

				Member member = LoadMember(args);

				APIArguments resultParams = new APIArguments();

				if (returnRewards)
				{
					IList<long> rewardIds = LoyaltyDataService.GetMemberRewardIds(
							member, categoryId, startDate, endDate, null, unRedeemedOnly, unexpiredOnly);
					if (rewardIds.Count > 0)
					{
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
							APIStruct rv = Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards.MemberRewardSerializationUtility.SerializeMemberRewardSummary(lang, channel, vcKeys, startDate, endDate, reward, returnAttributes);
							memberRewards[rewardIdx++] = rv;
						}
						resultParams.Add("MemberRewardSummary", memberRewards);
					}
					else
					{
						_logger.Trace(_className, methodName, string.Format("No member rewards found for member with id {0}.", member.IpCode));
					}
				}

				if (returnCoupons)
				{
					LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);
					IList<MemberCoupon> coupons = LoyaltyDataService.GetMemberCouponsByMember(member.IpCode, batchInfo, unRedeemedOnly);

					if (coupons.Count > 0)
					{
						APIStruct[] memberCoupons = new APIStruct[coupons.Count];
						int idx = 0;
						foreach (MemberCoupon coupon in coupons)
						{
							memberCoupons[idx++] = Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Coupons.CouponHelper.SerializeMemberCoupon(member, lang, channel, coupon, returnAttributes);
						}
						resultParams.Add("MemberCoupon", memberCoupons);
					}
					else
					{
						_logger.Trace(_className, methodName, string.Format("No member coupons found for member with id {0}.", member.IpCode));
					}
				}

				if (returnPromotions)
				{
					LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);
					IList<MemberPromotion> promotions = LoyaltyDataService.GetMemberPromotionsByMember(member.IpCode, batchInfo, unexpiredOnly);
					if (promotions.Count > 0)
					{
						APIStruct[] memberPromotions = new APIStruct[promotions.Count];
						int msgIdx = 0;
						foreach (MemberPromotion promo in promotions)
						{
							memberPromotions[msgIdx++] = Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Promotions.PromotionUtil.SerializeMemberPromotion(lang, channel, promo, returnPromotionDefinition, returnAttributes);
						}
						resultParams.Add("MemberPromotion", memberPromotions);
					}
					else
					{
						_logger.Trace(_className, methodName, string.Format("No member promotions found for member with id {0}.", member.IpCode));
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
