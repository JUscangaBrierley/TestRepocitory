using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.ClientDevUtilities.LWExtend.FrameWork.Data
{
    public interface ILWExtendLoyaltyDataService
    {
        decimal GetEarnedPointBalance(
            IList<VirtualCard> vcList,
            long[] pointTypes,
            long[] pointEvents,
            DateTime? from,
            DateTime? to,
            DateTime? awardDateFrom,
            DateTime? awardDateTo,
            bool includeExpiredPoints);

        decimal GetEarnedPointBalance(
            IList<VirtualCard> vcList,
            IList<PointType> pointTypes,
            IList<PointEvent> pointEvents,
            DateTime? from,
            DateTime? to,
            DateTime? awardDateFrom,
            DateTime? awardDateTo,
            bool includeExpiredPoints);
    }
}
