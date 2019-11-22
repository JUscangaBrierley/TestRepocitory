using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Rewards
{
    public class GetMemberRewardSummaryById : MemberRewardsBase
    {
        public GetMemberRewardSummaryById() : base("GetMemberRewardSummaryById") { }

        public override string Invoke(string source, string parms)
        {
            try
            {
                string response = string.Empty;
                if (string.IsNullOrEmpty(parms))
                {
                    throw new LWOperationInvocationException("No parameters provided to retrieve member rewards.") { ErrorCode = 3300 };
                }

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);
                long rewardId = (long)args["MemberRewardId"];
                string lang = args.ContainsKey("Language") ? (string)args["Language"] : LanguageChannelUtil.GetDefaultCulture();
                string channel = args.ContainsKey("Channel") ? (string)args["Channel"] : LanguageChannelUtil.GetDefaultChannel();

                MemberReward reward = LoyaltyDataService.GetMemberReward(rewardId);
                if (reward == null)
                {
                    throw new LWOperationInvocationException("No reward found with id " + rewardId + ".") { ErrorCode = 3347 };
                }

                Member member = LoyaltyDataService.LoadMemberFromIPCode(reward.MemberId);
                long[] vcKeys = member.GetLoyaltyCardIds();

                APIStruct rewardSummary = MemberRewardSerializationUtility.SerializeMemberRewardSummary(lang, channel, vcKeys, null, null, reward, false);

                string memberIdentity = ReturnMemberIdentity(member);

                APIArguments resultParams = new APIArguments();
                resultParams.Add("MemberRewardSummary", rewardSummary);
                resultParams.Add("MemberIdentity", memberIdentity);

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
