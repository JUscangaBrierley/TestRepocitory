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
	public class RefLanguagePref : LWClientDataObject
	{							    											    
		#region Properties
		[LWMeta(false)]				
		public virtual long? LanguagePrefCode { get; set; }			
		[LWMeta(false,255)]				
		public virtual string Description { get; set; }			
		#endregion				
	}
}
