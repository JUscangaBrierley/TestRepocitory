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
using Brierley.LoyaltyWare.ClientLib.DomainModel;

namespace Brierley.LoyaltyWare.ClientLib.DomainModel.Client
{
	public class RefBrand : LWClientDataObject
	{							    											    
		#region Properties
		[LWMeta(false,50)]				
		public virtual string ShortBrandName { get; set; }			
		[LWMeta(false)]				
		public virtual long? BrandID { get; set; }			
		[LWMeta(true)]				
		public virtual long BrandNumber { get; set; }			
		[LWMeta(false,255)]				
		public virtual string BrandName { get; set; }			
		[LWMeta(false,10)]				
		public virtual string LoyaltyNumberPrefix { get; set; }			
		#endregion				
	}
}
