using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;
using Brierley.FrameWork.Rules;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders.Members
{
    public class GetAccountSummary : OperationProviderBase
    {
        private RewardRuleUtil _util = new RewardRuleUtil();

        public GetAccountSummary() : base("GetAccountSummary") { }

        public override string Invoke(string source, string parms)
        {
            try
            {
                string response = string.Empty;

                APIArguments args = SerializationUtils.DeserializeRequest(Name, Config, parms);

                Member member = LoadMember(args);

                string language = args.ContainsKey("Language") ? (string)args["Language"] : LanguageChannelUtil.GetDefaultCulture();
                string channel = args.ContainsKey("Channel") ? (string)args["Channel"] : LanguageChannelUtil.GetDefaultChannel();

                decimal balance = member.GetPoints(null, null, null, null);

                APIArguments resultParams = new APIArguments();
                resultParams.Add("CurrencyBalance", balance);
                resultParams.Add("MemberStatus", Enum.GetName(typeof(MemberStatusEnum), member.MemberStatus));
                resultParams.Add("MemberAddDate", member.MemberCreateDate);
                MemberTier tier = member.GetTier(DateTime.Now);
                if (tier != null)
                {
                    TierDef def = LoyaltyDataService.GetTierDef(tier.TierDefId);
                    resultParams.Add("CurrentTierName", def.Name);
                    resultParams.Add("CurrentTierExpirationDate", tier.ToDate);
                }
                resultParams.Add("CurrencyToNextTier", member.GetPointsToNextTier());
                if (member.LastActivityDate != null)
                {
                    resultParams.Add("LastActivityDate", member.LastActivityDate);
                }
                resultParams.Add("CurrencyToNextReward", member.GetPointsToNextReward());
                resultParams.Add("PointsToRewardChoice", member.GetPointsToRewardChoice());

                RewardDef choice = LoyaltyDataService.GetCurrentRewardChoiceOrDefault(member);
                if (choice != null)
                {
                    var choiceParams = new APIArguments();

                    choiceParams.Add("RewardId", choice.Id);
                    choiceParams.Add("TypeCode", choice.CertificateTypeCode);

                    var p = choice.Product != null ? choice.Product : ContentService.GetProduct(choice.ProductId);
                    choiceParams.Add("CategoryId", p.CategoryId);

                    choiceParams.Add("CatalogStartDate", choice.CatalogStartDate);
                    choiceParams.Add("CatalogEndDate", choice.CatalogEndDate);
                    choiceParams.Add("CurrencyToEarn", choice.HowManyPointsToEarn);
                    choiceParams.Add("CurrencyType", choice.PointType);
                    choiceParams.Add("DisplayName", choice.GetDisplayName(language, channel));
                    choiceParams.Add("ShortDescription", choice.GetShortDescription(language, channel));
                    choiceParams.Add("LongDescription", choice.GetLongDescription(language, channel));
                    choiceParams.Add("LegalText", choice.GetLegalText(language, channel));
                    choiceParams.Add("SmallImageFile", choice.SmallImageFile);
                    choiceParams.Add("LargeImageFile", choice.LargeImageFile);

                    resultParams.Add("RewardChoice", new APIStruct() { Name = "RewardChoice", IsRequired = false, Parms = choiceParams });
                }
                RewardDef lcapReward = ContentService.GetRewardDefForExchange(member);
                if (lcapReward != null)
                {
                    decimal pointsBalance = _util.GetPoints(lcapReward, lcapReward.GetPointTypes(), lcapReward.GetPointEvents(), member, null);
                    decimal pointsOnHold = _util.GetOnHoldPoints(lcapReward, lcapReward.GetPointTypes(), lcapReward.GetPointEvents(), member, null);
                    resultParams.Add("LoyaltyCurrencyAsPaymentBalance", pointsBalance - pointsOnHold);
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
                throw new LWOperationInvocationException(ex.Message) { ErrorCode = 1 };
            }
        }
    }
}
