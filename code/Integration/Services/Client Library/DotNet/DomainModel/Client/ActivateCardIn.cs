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
	public class ActivateCardIn
	{							    											    
		#region Properties
		[LWMeta(true,255)]        								
		public virtual string CardID { get; set; }			
		[LWMeta(false)]        								
		public virtual DateTime? EffectiveDate { get; set; }			
		[LWMeta(false,255)]        								
		public virtual string UpdateCardStatusReason { get; set; }			
		#endregion				
	}
}
