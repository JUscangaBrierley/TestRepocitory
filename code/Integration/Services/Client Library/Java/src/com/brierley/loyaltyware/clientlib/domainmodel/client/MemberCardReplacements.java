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
import com.brierley.loyaltyware.clientlib.domainmodel.LWClientDataObject;

public class MemberCardReplacements extends LWClientDataObject {			    
	// property: OldLoyaltyIdNumber
	@LWStringLength(50) 
	private String OldLoyaltyIdNumber;
	public 	String getOldLoyaltyIdNumber() {
		return this.OldLoyaltyIdNumber;
	}
	public 	void setOldLoyaltyIdNumber(String oldloyaltyidnumber) {
		this.OldLoyaltyIdNumber = oldloyaltyidnumber;
	}

	// property: IsTemporary
	
	private Boolean IsTemporary;
	public 	Boolean getIsTemporary() {
		return this.IsTemporary;
	}
	public 	void setIsTemporary(Boolean istemporary) {
		this.IsTemporary = istemporary;
	}

	// property: LoyaltyIDNumber
	@LWStringLength(50) 
	private String LoyaltyIDNumber;
	public 	String getLoyaltyIDNumber() {
		return this.LoyaltyIDNumber;
	}
	public 	void setLoyaltyIDNumber(String loyaltyidnumber) {
		this.LoyaltyIDNumber = loyaltyidnumber;
	}

	// property: DateReceived
	
	private Date DateReceived;
	public 	Date getDateReceived() {
		return this.DateReceived;
	}
	public 	void setDateReceived(Date datereceived) {
		this.DateReceived = datereceived;
	}

	// property: CHANGEDBY
	@LWStringLength(255) 
	private String CHANGEDBY;
	public 	String getCHANGEDBY() {
		return this.CHANGEDBY;
	}
	public 	void setCHANGEDBY(String changedby) {
		this.CHANGEDBY = changedby;
	}

	// property: DATESENT
	
	private Date DATESENT;
	public 	Date getDATESENT() {
		return this.DATESENT;
	}
	public 	void setDATESENT(Date datesent) {
		this.DATESENT = datesent;
	}

}