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
	public class GetMemberRewardsByOrderNumberStruct
	{							    											    
		#region Properties
		[LWMeta(false,0)]        								
		public virtual string OrderNumber { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string Source { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string Channel { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string ChangedBy { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string FirstName { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string LastName { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string EmailAddress { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string AddressLineOne { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string AddressLineTwo { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string AddressLineThree { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string AddressLineFour { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string City { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string StateOrProvince { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string ZipOrPostalCode { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string County { get; set; }			
		[LWMeta(false,0)]        								
		public virtual string Country { get; set; }			
		[LWMeta(true)]        								
		public virtual MemberRewardInfoStruct[] MemberRewardInfo { get; set; }			
		#endregion				
	}
}
