using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGAccountActivityDetails
    {
        #region Properties
        [DataMember]
        public virtual string ItemDescription { get; set; }
        [DataMember]
        public virtual int? Quantity { get; set; }
        [DataMember]
        public virtual decimal? SaleAmount { get; set; }
        [DataMember]
        public virtual string ClassDescription { get; set; }
        [DataMember]
        public virtual string ClassCode { get; set; }
        [DataMember]
        public virtual List<MGPointsHistory> PointsHistory { get; set; }
        [DataMember]
        public virtual List<MGClientEntityAttribute> AdditionalAttributes { get; set; }        
        #endregion
    }
}