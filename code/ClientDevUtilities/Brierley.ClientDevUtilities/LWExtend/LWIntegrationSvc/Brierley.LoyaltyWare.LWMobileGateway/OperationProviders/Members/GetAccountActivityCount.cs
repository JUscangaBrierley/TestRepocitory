using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Common.Exceptions;

using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

using Brierley.FrameWork.LWIntegration.Util;

using Brierley.LoyaltyWare.LWMobileGateway.DomainModel;
using Brierley.LoyaltyWare.LWMobileGateway.Authentication;

namespace Brierley.LoyaltyWare.LWMobileGateway.OperationProviders.Members
{
    public class GetAccountActivityCount : OperationProviderBase
    {
        #region Fields
        private const string _className = "GetAccountActivityCount";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_MOBILEGATEWAY_SERVICE);
        #endregion

        public GetAccountActivityCount() : base("GetAccountActivityCount") { }


        public override object Invoke(string source, WcfAuthenticationToken token, object[] parms)
        {
            string methodName = "Invoke";
            
            Member member = token.CachedMember;

            string startDateStr = parms[0] as string;
            DateTime? startDate = null;
            if (!string.IsNullOrEmpty(startDateStr))
            {
                startDate = DateTime.Parse(startDateStr);
            }

            string endDateStr = parms[1] as string;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(endDateStr))
            {
                endDate = DateTime.Parse(endDateStr);
            }

            if (endDate < startDate)
            {
                _logger.Error(_className, methodName, "End date cannot be earlier than the start date");
                throw new LWOperationInvocationException("End date cannot be earlier than the start date") { ErrorCode = 3204 };
            }
            if (startDate != null)
            {
                startDate = DateTimeUtil.GetBeginningOfDay((DateTime)startDate);
            }
            if (endDate != null)
            {
                endDate = DateTimeUtil.GetEndOfDay((DateTime)endDate);
            }

            AttributeSetMetaData meta = LoyaltyService.GetAttributeSetMetaData("TxnHeader");
            if (meta == null)
            {
                throw new LWOperationInvocationException("Standard implementation requires TxnHeader attribute set to be defined.") { ErrorCode = 3357 };
            }

            LWCriterion crit = new LWCriterion(meta.Name);
            if (startDate != null)
            {
                crit.Add(LWCriterion.OperatorType.AND, "TxnDate", startDate, LWCriterion.Predicate.Gt);
            }
            if (endDate != null)
            {
                crit.Add(LWCriterion.OperatorType.AND, "TxnDate", endDate, LWCriterion.Predicate.Le);
            }

            long count = 0;
            foreach (VirtualCard vc in member.LoyaltyCards)
            {
                count += LoyaltyService.CountAttributeSetObjects(vc, meta, crit);
            }

            return (int)count;
        }
    }
}