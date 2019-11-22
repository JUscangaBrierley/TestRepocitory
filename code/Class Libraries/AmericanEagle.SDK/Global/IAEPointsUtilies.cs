using System;
using System.Collections.Generic;
using Brierley.FrameWork.Data.DomainModel;

namespace AmericanEagle.SDK.Global
{
    public interface IAEPointsUtilies
    {
        decimal GetDollarPointsEllegibleForRewards(Member member, DateTime startDate, DateTime endDate);
        decimal GetDollarPointsOnHold(Member member, DateTime startDate, DateTime endDate);
        decimal GetPointsOnHold(Member member, DateTime startDate, DateTime endDate, IList<long> pointTypeIDs);
        IList<long> GetPointTypeIdsFilteredByType(TypeOfPoint type);
        IList<PointType> GetPointTypesFromIdsList(IList<long> pointTypeIDs);
        decimal GetTotalDollarPoints(Member member, DateTime startDate, DateTime endDate);
        decimal GetTotalBasicDollarPoints(Member member, DateTime startDate, DateTime endDate);
        decimal GetTotalBonusDollarPoints(Member member, DateTime startDate, DateTime endDate);
        decimal GetTotalPoints(Member member, DateTime startDate, DateTime endDate, IList<long> pointTypeIDs);
        List<string> GetValidPointTypeNamesByType(TypeOfPoint type);
    }
}