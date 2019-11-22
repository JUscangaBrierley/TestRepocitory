using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGStoreInfo
    {
        #region Properties
        [DataMember]
        public virtual string StoreName { get; set; }        
        [DataMember]
        public virtual string StoreNumber { get; set; }
        [DataMember]
        public virtual string City { get; set; }
        [DataMember]
        public virtual string State { get; set; }        
        #endregion
    }
}