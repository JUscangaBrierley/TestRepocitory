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

public class MemberMergeHistory extends LWClientDataObject {			    
	// property: ChangedBy
	@LWStringLength(50) 
	private String ChangedBy;
	public 	String getChangedBy() {
		return this.ChangedBy;
	}
	public 	void setChangedBy(String changedby) {
		this.ChangedBy = changedby;
	}

	// property: FromLoyaltyID
	@LWStringLength(50) 
	private String FromLoyaltyID;
	public 	String getFromLoyaltyID() {
		return this.FromLoyaltyID;
	}
	public 	void setFromLoyaltyID(String fromloyaltyid) {
		this.FromLoyaltyID = fromloyaltyid;
	}

	// property: FromIPCode
	
	private Long FromIPCode;
	public 	Long getFromIPCode() {
		return this.FromIPCode;
	}
	public 	void setFromIPCode(Long fromipcode) {
		this.FromIPCode = fromipcode;
	}

}