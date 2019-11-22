using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;
using Brierley.ClientDevUtilities.OperationProvider;
using Brierley.ClientDevUtilities.LWGateway;

namespace Brierley.ClientDevUtilities.LWExtend.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
    public abstract class GetAccountSummary<T> : MGOperationProviderBase where T : MGAccountSummary, new()
    {
        public GetAccountSummary(string name, ILWDataServiceUtil lwDataServiceUtil) : base(name, lwDataServiceUtil) { }

        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            Member member = token.CachedMember;

            long[] pointTypeIds = null;
            long[] pointEventIds = null;

            string pointTypesStr = GetFunctionParameter("LoyaltyCurrencyNames");
            if (!string.IsNullOrEmpty(pointTypesStr))
            {
                string[] pointTypeNames = pointTypesStr.Split(';');
                pointTypeIds = new long[pointTypeNames.Length];
                for(int i = 0; i < pointTypeNames.Length; i++)
                {
                    pointTypeIds[i] = LoyaltyService.GetPointType(pointTypeNames[i]).ID;
                }
            }

            string pointEventsStr = GetFunctionParameter("LoyaltyEventNames");
            if (!string.IsNullOrEmpty(pointEventsStr))
            {
                string[] pointEventNames = pointEventsStr.Split(';');
                pointEventIds = new long[pointEventNames.Length];
                for (int i = 0; i < pointEventNames.Length; i++)
                {
                    pointEventIds[i] = LoyaltyService.GetPointEvent(pointEventNames[i]).ID;
                }
            }

            T summary = new T();

            summary.CurrencyBalance = member.GetPoints(pointTypeIds, pointEventIds, null, null);

            summary.PointsToRewardChoice = member.GetPointsToRewardChoice();

            //summary.MemberStatus = Enum.GetName(typeof(MemberStatusEnum), member.MemberStatus);
            summary.MemberAddDate = member.MemberCreateDate;
            MemberTier tier = member.GetTier(DateTime.Now);
            if (tier != null)
            {
                TierDef def = LoyaltyService.GetTierDef(tier.TierDefId);
                summary.CurrentTierName = def.Name;
                summary.CurrentTierExpirationDate = tier.ToDate;                
            }
            summary.CurrencyToNextTier = member.GetPointsToNextTier();
            if (member.LastActivityDate != null)
            {
                summary.LastActivityDate = member.LastActivityDate.Value;                
            }
            summary.CurrencyToNextReward = GetPointsToNextReward(member, parms, pointEventIds, pointEventIds);

            InvokeComplete(member, parms, pointEventIds, pointEventIds, summary);

            return summary;
        }

        protected virtual decimal GetPointsToNextReward(Member member, object[] parms, long[] pointTypeIds, long[] pointEventIds)
        {
            return member.GetPointsToNextReward();
        }

        protected virtual void InvokeComplete(Member member, object[] parms, long[] pointTypeIds, long[] pointEventIds, T summary) { }
    }
}