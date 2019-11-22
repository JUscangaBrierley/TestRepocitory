using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGPointsHistory
    {
        #region Properties
        [DataMember]
        public virtual long CurrencyAmount { get; set; }        
        [DataMember]
        public virtual string PointTxnType { get; set; }
        [DataMember]
        public virtual string LoyaltyEvent { get; set; }
        [DataMember]
        public virtual string LoyaltyCurrency { get; set; }
        [DataMember]
        public virtual DateTime TxnDate { get; set; }
        [DataMember]
        public virtual string OwnerType { get; set; }
        [DataMember]
        public virtual long? OwnerId { get; set; }
        [DataMember]
        public virtual long? RowKey { get; set; }
        [DataMember]
        public virtual string PromoCode { get; set; }
        [DataMember]
        public virtual DateTime? ExpirationDate { get; set; }
        [DataMember]
        public virtual string Notes { get; set; }
        #endregion

		public static MGPointsHistory Hydrate(PointTransaction txn)
		{
			using (var loyalty = LWDataServiceUtil.LoyaltyDataServiceInstance())
			{
				MGPointsHistory pointTxn = new MGPointsHistory()
				{
					CurrencyAmount = (long)txn.Points,
					PointTxnType = txn.TransactionType.ToString(),
					LoyaltyEvent = loyalty.GetPointEvent(txn.PointEventId).Name,
					LoyaltyCurrency = loyalty.GetPointType(txn.PointTypeId).Name,
					TxnDate = txn.TransactionDate

				};
				if (txn.OwnerType != PointTransactionOwnerType.Unknown)
				{
					pointTxn.OwnerType = txn.OwnerType.ToString();
					if (txn.OwnerId > 0)
					{
						pointTxn.OwnerId = txn.OwnerId;
					}
					if (txn.RowKey > 0)
					{
						pointTxn.RowKey = txn.RowKey;
					}
				}
				if (!string.IsNullOrEmpty(txn.PromoCode))
				{
					pointTxn.PromoCode = txn.PromoCode;
				}
				if (txn.ExpirationDate != null)
				{
					pointTxn.ExpirationDate = txn.ExpirationDate;
				}
				pointTxn.Notes = txn.Notes;
				return pointTxn;
			}
		}
    }
}