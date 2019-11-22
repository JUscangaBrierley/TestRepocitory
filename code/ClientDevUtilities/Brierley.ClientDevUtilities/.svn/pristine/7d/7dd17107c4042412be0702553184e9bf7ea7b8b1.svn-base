using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

using Brierley.FrameWork;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.ClientDevUtilities.OperationProvider;
using LWGateway = Brierley.ClientDevUtilities.LWGateway;
using Brierley.ClientDevUtilities.LWExtend.LoyaltyWare.LWMobileGateway.DomainModel;

namespace Brierley.ClientDevUtilities.LWExtend.LoyaltyWare.LWMobileGateway.OperationProviders.Rewards
{
    public abstract class GetMemberRewards<R,MR> : MGOperationProviderBase
        where R : GetMemberRewardsRequest, new()
        where MR : MGMemberReward, new()
    {
        #region Fields
        private const string _className = "GetMemberRewards";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        int _parameterCount;
        private LWGateway.ILWDataServiceUtil _lwDataServiceUtil;
        private LWGateway.ILanguageChannelUtil _languageChannelUtil;

        public GetMemberRewards(string name, int parameterCount, LWGateway.ILWDataServiceUtil lwDataServiceUtil, LWGateway.ILanguageChannelUtil languageChannelUtil) : base(name, lwDataServiceUtil)
        {
            _parameterCount = parameterCount;
            _lwDataServiceUtil = lwDataServiceUtil;
            _languageChannelUtil = languageChannelUtil;
        }

        #region Private Helpers        
        #endregion

        #region Invocation
        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";
            var request = new R();

            {
            if (parms == null || parms.Length != _parameterCount)
            {
                string errMsg = "Invalid parameters provided for GetMemberRewards.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWOperationInvocationException(errMsg) { ErrorCode = 1 };
            }

            string catStr = (string)parms[0];
            long? categoryId = !string.IsNullOrEmpty(catStr) ? (long?)int.Parse(catStr) : null;

            DateTime startDate = !string.IsNullOrEmpty((string)parms[1]) ? (DateTime)DateTime.Parse((string)parms[1]) : DateTimeUtil.MinValue;
            DateTime endDate = !string.IsNullOrEmpty((string)parms[2]) ? (DateTime)DateTime.Parse((string)parms[2]) : DateTimeUtil.MaxValue;
            if (endDate < startDate)
            {
                _logger.Error(_className, methodName, "End date cannot be earlier than the start date");
                throw new LWOperationInvocationException("End date cannot be earlier than the start date") { ErrorCode = 3204 };
            }

            bool unRedeemedOnly = !string.IsNullOrEmpty((string)parms[3]) ? bool.Parse((string)parms[3]) : true;
            bool unexpiredOnly = !string.IsNullOrEmpty((string)parms[4]) ? bool.Parse((string)parms[4]) : true;

            string language = (string)parms[5];
            if (string.IsNullOrEmpty(language))
            {
                language = _languageChannelUtil.GetDefaultCulture();
            }
            string channel = (string)parms[6];
            if (string.IsNullOrEmpty(channel))
            {
                channel = _languageChannelUtil.GetDefaultChannel();
            }
            if (!_languageChannelUtil.IsLanguageValid(ContentService, language))
            {
                throw new LWOperationInvocationException("Specified language is not defined.") { ErrorCode = 6002 };
            }
            if (!_languageChannelUtil.IsChannelValid(ContentService, channel))
            {
                throw new LWOperationInvocationException("Specified channel is not defined.") { ErrorCode = 6003 };
            }

            string startIndexStr = (string)parms[7];
            string batchSizeStr = (string)parms[8];
            int? startIndex = !string.IsNullOrEmpty(startIndexStr) ? (int?)int.Parse(startIndexStr) : null;
            int? batchSize = !string.IsNullOrEmpty(batchSizeStr) ? (int?)int.Parse(batchSizeStr) : null;

            LWQueryBatchInfo batchInfo = LWQueryBatchInfo.GetValidBatch(startIndex, batchSize, Config.EnforceValidBatch);

                request.CategoryId = categoryId;
                request.StartDate = startDate;
                request.EndDate = endDate;
                request.UnRedeemedOnly = unRedeemedOnly;
                request.UnexpiredOnly = unexpiredOnly;
                request.Language = language;
                request.Channel = channel;
                request.StartIndex = startIndex;
                request.BatchSize = batchSize;

                ProcessParametersComplete(parms, request);
            }

            Member member = token.CachedMember;

            IList<long> rewardIds = GetMemberRewardIds(member, request);
            if (rewardIds == null || rewardIds.Count == 0)
            {
                throw new LWOperationInvocationException("No reward definitions found.") { ErrorCode = 3362 };
            }
            long[] ids = LWQueryBatchInfo.GetIds(rewardIds.ToArray<long>(), request.StartIndex, request.BatchSize, Config.EnforceValidBatch);
            IList<MemberReward> rewardsList = LoyaltyService.GetMemberRewardByIds(ids);

            GetMemberRewardListComplete(member, request, rewardIds, ids, rewardsList);

            var mcList = new List<MR>();
            long[] vcKeys = member.GetLoyaltyCardIds();
            foreach (MemberReward reward in rewardsList)
            {
                var mgReward = MGMemberRewardUtility.Hydrate<MR>(_lwDataServiceUtil, vcKeys, reward, request.Language, request.Channel);
                HydrateMGMemberRewardComplete(member, request, reward, mgReward);
                mcList.Add(mgReward);
            }            
            return mcList;
        }
        #endregion

        protected virtual void ProcessParametersComplete(object[] parms, R request) { }

        protected virtual IList<long> GetMemberRewardIds(Member member, R request)
        {
            return LoyaltyService.GetMemberRewardIds(member, request.CategoryId, request.StartDate, request.EndDate, null, request.UnRedeemedOnly, request.UnexpiredOnly);
        }

        protected virtual void GetMemberRewardListComplete(Member member, R request, IList<long> allMemberRewardIds, long[] memberRewardIds, IList<MemberReward> memberRewardList) { }

        protected virtual void HydrateMGMemberRewardComplete(Member member, R request, MemberReward memberReward, MR mgMemberReward) { }
    }
}