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

public class MemberBraPromoCertHistory extends LWClientDataObject {			    
	// property: Threshold
	
	private Long Threshold;
	public 	Long getThreshold() {
		return this.Threshold;
	}
	public 	void setThreshold(Long threshold) {
		this.Threshold = threshold;
	}

	// property: TxnHeaderID
	@LWStringLength(50) 
	private String TxnHeaderID;
	public 	String getTxnHeaderID() {
		return this.TxnHeaderID;
	}
	public 	void setTxnHeaderID(String txnheaderid) {
		this.TxnHeaderID = txnheaderid;
	}

	// property: ReturnTxnHeaderID
	@LWStringLength(50) 
	private String ReturnTxnHeaderID;
	public 	String getReturnTxnHeaderID() {
		return this.ReturnTxnHeaderID;
	}
	public 	void setReturnTxnHeaderID(String returntxnheaderid) {
		this.ReturnTxnHeaderID = returntxnheaderid;
	}

	// property: FulfillmentDate
	
	private Date FulfillmentDate;
	public 	Date getFulfillmentDate() {
		return this.FulfillmentDate;
	}
	public 	void setFulfillmentDate(Date fulfillmentdate) {
		this.FulfillmentDate = fulfillmentdate;
	}

	// property: ChangedBy
	@LWStringLength(255) 
	private String ChangedBy;
	public 	String getChangedBy() {
		return this.ChangedBy;
	}
	public 	void setChangedBy(String changedby) {
		this.ChangedBy = changedby;
	}

	// property: IsFulfilled
	
	private Boolean IsFulfilled;
	public 	Boolean getIsFulfilled() {
		return this.IsFulfilled;
	}
	public 	void setIsFulfilled(Boolean isfulfilled) {
		this.IsFulfilled = isfulfilled;
	}

	// property: IsDeleted
	
	private Boolean IsDeleted;
	public 	Boolean getIsDeleted() {
		return this.IsDeleted;
	}
	public 	void setIsDeleted(Boolean isdeleted) {
		this.IsDeleted = isdeleted;
	}

}