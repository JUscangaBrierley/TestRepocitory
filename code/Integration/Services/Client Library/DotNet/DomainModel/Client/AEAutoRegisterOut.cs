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
	public class AEAutoRegisterOut
	{							    											    
		#region Properties
		[LWMeta(true,0)]        								
		public virtual string LoyaltyNumber { get; set; }			
		[LWMeta(true,0)]        								
		public virtual string ResponseMessage { get; set; }			
		[LWMeta(false)]        								
		public virtual int? ResponseCode { get; set; }			
		#endregion				
	}
}
