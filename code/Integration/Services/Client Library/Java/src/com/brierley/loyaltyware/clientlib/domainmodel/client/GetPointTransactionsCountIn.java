//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by LoyaltyWare.
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

package com.brierley.loyaltyware.clientlib.domainmodel.client;

import java.util.Date;

import com.brierley.loyaltyware.clientlib.annotations.*;
import com.brierley.loyaltyware.clientlib.domainmodel.framework.Member;
import com.brierley.loyaltyware.clientlib.domainmodel.framework.VirtualCard;

public class GetPointTransactionsCountIn {
	// property: MemberIdentity
	@LWIsRequired       								
	private String MemberIdentity;
	public 	String getMemberIdentity() {
		return this.MemberIdentity;
	}
	public 	void setMemberIdentity(String MemberIdentity) {
		this.MemberIdentity = MemberIdentity;
	}	
				
	// property: CardID
	@LWStringLength(255)       								
	private String CardID;
	public 	String getCardID() {
		return this.CardID;
	}
	public 	void setCardID(String CardID) {
		this.CardID = CardID;
	}	
				
	// property: StartDate
	      								
	private Date StartDate;
	public 	Date getStartDate() {
		return this.StartDate;
	}
	public 	void setStartDate(Date StartDate) {
		this.StartDate = StartDate;
	}	
				
	// property: EndDate
	      								
	private Date EndDate;
	public 	Date getEndDate() {
		return this.EndDate;
	}
	public 	void setEndDate(Date EndDate) {
		this.EndDate = EndDate;
	}	
				
	// property: LoyaltyCurrencyNames
	@LWStringLength(150)       								
	private String[] LoyaltyCurrencyNames;
	public 	String[] getLoyaltyCurrencyNames() {
		return this.LoyaltyCurrencyNames;
	}
	public 	void setLoyaltyCurrencyNames(String[] LoyaltyCurrencyNames) {
		this.LoyaltyCurrencyNames = LoyaltyCurrencyNames;
	}	
				
	// property: LoyaltyEventNames
	@LWStringLength(150)       								
	private String[] LoyaltyEventNames;
	public 	String[] getLoyaltyEventNames() {
		return this.LoyaltyEventNames;
	}
	public 	void setLoyaltyEventNames(String[] LoyaltyEventNames) {
		this.LoyaltyEventNames = LoyaltyEventNames;
	}	
				
}
