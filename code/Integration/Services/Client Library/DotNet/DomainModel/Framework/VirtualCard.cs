//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by LoyaltyWare.
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

using Brierley.LoyaltyWare.ClientLib.DomainModel;

namespace Brierley.LoyaltyWare.ClientLib.DomainModel.Framework
{
    public class VirtualCard : LWAttributeSetContainer
    {
        public enum VirtualCardStatusType { Active = 1, InActive = 2, Hold = 3, Cancelled = 4, Replaced = 5, Expired = 6 }
        
        public virtual Int64 VcKey { get; set; }
        [LWMeta(true, 255)]
        public virtual String LoyaltyIdNumber { get; set; }        
        public virtual Int64 IpCode { get; set; }
        public virtual DateTime? DateIssued { get; set; }
        public virtual DateTime? DateRegistered { get; set; }
        public virtual VirtualCardStatusType Status { get; set; }
        public virtual Boolean IsPrimary { get; set; }
        public virtual Int64 CardType { get; set; }       
    }
}
