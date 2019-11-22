using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.LWExtend.FrameWork.Rules
{
    public interface ILWExtendTierUtil
    {
        decimal GetCumulativePoints(Member member, IList<VirtualCard> cards, string[] pointTypes, string[] pointEvents, DateTime? from, DateTime? to, DateTime? awardDateFrom, DateTime? awardDateTo, bool includeExpiredPoints);
    }
}