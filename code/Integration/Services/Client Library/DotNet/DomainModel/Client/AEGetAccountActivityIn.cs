//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by LoyaltyWare.
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;

using Brierley.LoyaltyWare.ClientLib;
//using Brierley.LoyaltyWare.ClientLib.DomainModel;
using Brierley.LoyaltyWare.ClientLib.DomainModel.Framework;

namespace Brierley.LoyaltyWare.ClientLib.DomainModel.Client
{
	public class AEGetAccountActivityIn
	{							    											    
		#region Properties
		[LWMeta(false,0)]        								
		public virtual string LoyaltyNumber { get; set; }			
		[LWMeta(false)]        								
		public virtual DateTime? StartDate { get; set; }			
		[LWMeta(false)]        								
		public virtual DateTime? EndDate { get; set; }			
		[LWMeta(false)]        								
		public virtual int? PageNumber { get; set; }			
		#endregion				
	}
}
