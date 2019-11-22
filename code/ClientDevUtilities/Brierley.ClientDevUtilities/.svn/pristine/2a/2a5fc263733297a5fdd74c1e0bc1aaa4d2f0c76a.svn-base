using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGAccountActivitySummary
    {
        #region Properties
        [DataMember]
        public virtual string TxnHeaderId { get; set; }        
        [DataMember]
        public virtual DateTime? TxnDate { get; set; }
        [DataMember]
        public virtual string TxnNumber { get; set; }
        [DataMember]
        public virtual string TxnRegisterNumber { get; set; }
        [DataMember]
        public virtual MGStoreInfo TxnStoreInfo { get; set; }
        [DataMember]
        public virtual decimal TxnAmount { get; set; }
        [DataMember]
        public virtual string TxnChannel { get; set; }
        [DataMember]
        public virtual List<MGPointsHistory> PointsHistory { get; set; }
        [DataMember]
        public virtual List<MGClientEntityAttribute> AdditionalAttributes { get; set; }        
        #endregion
    }
}