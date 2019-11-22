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

public class MemberReceipts extends LWClientDataObject {			    
	// property: ReceiptType
	
	private Long ReceiptType;
	public 	Long getReceiptType() {
		return this.ReceiptType;
	}
	public 	void setReceiptType(Long receipttype) {
		this.ReceiptType = receipttype;
	}

	// property: TxnStoreID
	@LWStringLength(50) 
	private String TxnStoreID;
	public 	String getTxnStoreID() {
		return this.TxnStoreID;
	}
	public 	void setTxnStoreID(String txnstoreid) {
		this.TxnStoreID = txnstoreid;
	}

	// property: TxnRegisterNumber
	@LWStringLength(10) 
	private String TxnRegisterNumber;
	public 	String getTxnRegisterNumber() {
		return this.TxnRegisterNumber;
	}
	public 	void setTxnRegisterNumber(String txnregisternumber) {
		this.TxnRegisterNumber = txnregisternumber;
	}

	// property: TxnNumber
	@LWStringLength(25) 
	private String TxnNumber;
	public 	String getTxnNumber() {
		return this.TxnNumber;
	}
	public 	void setTxnNumber(String txnnumber) {
		this.TxnNumber = txnnumber;
	}

	// property: TxnDate
	
	private Date TxnDate;
	public 	Date getTxnDate() {
		return this.TxnDate;
	}
	public 	void setTxnDate(Date txndate) {
		this.TxnDate = txndate;
	}

	// property: OrderNumber
	@LWStringLength(15) 
	private String OrderNumber;
	public 	String getOrderNumber() {
		return this.OrderNumber;
	}
	public 	void setOrderNumber(String ordernumber) {
		this.OrderNumber = ordernumber;
	}

	// property: TenderAmount
	
	private java.math.BigDecimal TenderAmount;
	public 	java.math.BigDecimal getTenderAmount() {
		return this.TenderAmount;
	}
	public 	void setTenderAmount(java.math.BigDecimal tenderamount) {
		this.TenderAmount = tenderamount;
	}

	// property: ClosedDate
	
	private Date ClosedDate;
	public 	Date getClosedDate() {
		return this.ClosedDate;
	}
	public 	void setClosedDate(Date closeddate) {
		this.ClosedDate = closeddate;
	}

	// property: ExpirationDate
	
	private Date ExpirationDate;
	public 	Date getExpirationDate() {
		return this.ExpirationDate;
	}
	public 	void setExpirationDate(Date expirationdate) {
		this.ExpirationDate = expirationdate;
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

	// property: CHANGEDBY
	@LWStringLength(255) 
	private String CHANGEDBY;
	public 	String getCHANGEDBY() {
		return this.CHANGEDBY;
	}
	public 	void setCHANGEDBY(String changedby) {
		this.CHANGEDBY = changedby;
	}

}