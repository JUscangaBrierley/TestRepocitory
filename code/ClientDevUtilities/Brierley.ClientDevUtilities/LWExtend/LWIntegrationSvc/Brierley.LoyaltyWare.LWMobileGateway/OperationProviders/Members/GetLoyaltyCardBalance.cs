using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
    public class GetLoyaltyCardBalance : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetLoyaltyCard";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetLoyaltyCardBalance() : base("GetLoyaltyCardBalance") { }

        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";

            if (parms == null || parms.Length == 0)
            {
                string errMsg = "No loyalty id provided to lookup a loyalty card.";
                _logger.Error(_className, methodName, errMsg);
                throw new LWException(errMsg) { ErrorCode = 1 };
            }

            string loyaltyId = parms[0] as string;
            Member m = token.CachedMember;
            VirtualCard vc = m.GetLoyaltyCard(loyaltyId);
            long[] vcKeys = new long[1] { vc.VcKey};

            string ptNameList = GetFunctionParameter("LoyaltyCurrencyNames");
            long[] pointTypeIds = null;
            if (!string.IsNullOrEmpty(ptNameList))
            {
                string[] pointTypeNames = ptNameList.Split(',');
                IList<PointType> ptList = LoyaltyService.GetPointTypes(pointTypeNames);
                if (ptList.Count < pointTypeNames.Length)
                {
                    throw new LWOperationInvocationException("Unable to find loyalty currencies.") { ErrorCode = 3311 };
                }
                if (ptList.Count > 0)
                {
                    pointTypeIds = new long[ptList.Count];
                    int idx = 0;
                    foreach (PointType pt in ptList)
                    {
                        pointTypeIds[idx++] = pt.ID;
                    }
                }
            }

            string peNameList = GetFunctionParameter("LoyaltyEventNames");
            long[] pointEventIds = null;
            if (!string.IsNullOrEmpty(peNameList))
            {
                string[] pointEventNames = peNameList.Split(',');
                IList<PointEvent> peList = LoyaltyService.GetPointEvents(pointEventNames);
                if (peList.Count < pointEventNames.Length)
                {
                    throw new LWOperationInvocationException("Unable to find loyalty events.") { ErrorCode = 3310 };
                }
                if (peList.Count > 0)
                {
                    pointEventIds = new long[peList.Count];
                    int idx = 0;
                    foreach (PointEvent pe in peList)
                    {
                        pointEventIds[idx++] = pe.ID;
                    }
                }
            }

            PointBankTransactionType[] tt = { PointBankTransactionType.Credit, PointBankTransactionType.Debit };

            return LoyaltyService.GetPointBalance(vcKeys, pointTypeIds, pointEventIds, tt, null, null, null, null, null, null, null, null, null);
        }
    }
}