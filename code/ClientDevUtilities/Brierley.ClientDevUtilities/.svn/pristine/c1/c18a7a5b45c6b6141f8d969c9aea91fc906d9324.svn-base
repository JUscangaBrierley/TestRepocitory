using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;
using Brierley.LoyaltyWare.LWIntegrationSvc.Exceptions;
using Brierley.LoyaltyWare.LWIntegrationSvc.Marshalling;

namespace Brierley.LoyaltyWare.LWIntegrationSvc.OperationProviders
{
    internal static class PointTransactionHelper
    {
        public static APIStruct SerializePointTransaction(PointTransaction txn, string name)
        {
            APIArguments tparms = new APIArguments();
            tparms.Add("CurrencyAmount", txn.Points);
            tparms.Add("PointTxnType", txn.TransactionType.ToString());
            using (LoyaltyDataService service = LWDataServiceUtil.LoyaltyDataServiceInstance())
            {
                tparms.Add("LoyaltyEvent", service.GetPointEvent(txn.PointEventId).Name);
                tparms.Add("LoyaltyCurrency", service.GetPointType(txn.PointTypeId).Name);
            }
            tparms.Add("TxnDate", txn.TransactionDate);

            //Jira:LW-3388 Adding point award date
            tparms.Add("PointAwardDate", txn.PointAwardDate);

            if (txn.OwnerType != PointTransactionOwnerType.Unknown)
            {
                tparms.Add("OwnerType", txn.OwnerType.ToString());
                if (txn.OwnerId > 0)
                {
                    tparms.Add("OwnerId", txn.OwnerId);
                }
                if (txn.RowKey > 0)
                {
                    tparms.Add("RowKey", txn.RowKey);
                }
            }
            if (!string.IsNullOrEmpty(txn.PromoCode))
            {
                tparms.Add("PromoCode", txn.PromoCode);
            }
            if (txn.ExpirationDate != null)
            {
                tparms.Add("ExpirationDate", txn.ExpirationDate);
            }
            tparms.Add("Notes", txn.Notes);
            APIStruct pointHistory = new APIStruct() { Name = name, IsRequired = false, Parms = tparms };
            return pointHistory;
        }        
    }
}
