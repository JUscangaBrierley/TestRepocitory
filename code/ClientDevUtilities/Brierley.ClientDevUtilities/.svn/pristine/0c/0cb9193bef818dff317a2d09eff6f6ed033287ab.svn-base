using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.FrameWork;
using Brierley.ClientDevUtilities.LWGateway;
using Brierley.ClientDevUtilities.LWExtend.FrameWork.Data;

namespace Brierley.ClientDevUtilities.LWExtend.FrameWork.Rules
{
    public class LWExtendTierUtil : ILWExtendTierUtil
    {
        private ILWDataServiceUtil _lwDataServiceUtil;
        private ILWExtendLoyaltyDataService _lwExtendLoyaltyDataService;

        public LWExtendTierUtil(ILWDataServiceUtil lwDataServiceUtil, ILWExtendLoyaltyDataService lwExtendLoyaltyDataService)
        {
            _lwDataServiceUtil = lwDataServiceUtil;
            _lwExtendLoyaltyDataService = lwExtendLoyaltyDataService;
        }

        public static LWExtendTierUtil Instance { get; private set; }

        static LWExtendTierUtil()
        {
            Instance = new LWExtendTierUtil(LWDataServiceUtil.Instance, LWExtendLoyaltyDataService.Instance);
        }

        public static void CalculateTierActivityDates(ExpressionFactory exprF, ContextObject context, ref DateTime start, ref DateTime end, TierDef tier)
        {
            if (exprF == null)
            {
                exprF = new ExpressionFactory();
            }
            if (context == null)
            {
                context = new ContextObject();
            }

            if (!string.IsNullOrEmpty(tier.ActivityPeriodStartExpression))
            {
                start = (DateTime)exprF.Create(tier.ActivityPeriodStartExpression).evaluate(context);
            }
            else
            {
                start = (DateTime)exprF.Create("GetBeginningOfDay(GetFirstDateOfYear(Date()))").evaluate(context);
            }
            if (!string.IsNullOrEmpty(tier.ActivityPeriodEndExpression))
            {
                end = (DateTime)exprF.Create(tier.ActivityPeriodEndExpression).evaluate(context);
            }
            else
            {
                end = (DateTime)exprF.Create("GetEndOfDay(GetLastDateOfYear(Date()))").evaluate(context);
            }
        }

		public decimal GetCumulativePoints(
			Member member,
			IList<VirtualCard> cards,
			string[] pointTypes,
			string[] pointEvents,
			DateTime? from,
			DateTime? to,
            DateTime? awardDateFrom,
            DateTime? awardDateTo,
            bool includeExpiredPoints)
		{
			using (var service = _lwDataServiceUtil.LoyaltyDataServiceInstance())
			{
				IList<PointType> ptList = new List<PointType>();

                if (pointTypes != null)
                {
                    foreach (string pointTypeName in pointTypes)
                    {
                        PointType pt = service.GetPointType(pointTypeName);
                        if (pt == null)
                        {
                            throw new LWException("No point type could be found with name " + pointTypeName + ".") { ErrorCode = 1 };
                        }
                        ptList.Add(pt);
                    }
                }

				IList<PointEvent> peList = new List<PointEvent>();

                if (pointEvents != null)
                {
                    foreach (string pointEventName in pointEvents)
                    {
                        PointEvent pe = service.GetPointEvent(pointEventName);
                        if (pe == null)
                        {
                            throw new LWException("No point event could be found with name " + pointEventName + ".") { ErrorCode = 1 };
                        }
                        peList.Add(pe);
                    }
                }

				return _lwExtendLoyaltyDataService.GetEarnedPointBalance(cards, ptList, peList, from, to, awardDateFrom, awardDateTo, includeExpiredPoints);
			}
		}
    }
}
