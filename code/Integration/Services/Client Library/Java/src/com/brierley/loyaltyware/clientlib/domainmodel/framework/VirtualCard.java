package com.brierley.loyaltyware.clientlib.domainmodel.framework;

import java.util.Date;
import com.brierley.loyaltyware.clientlib.domainmodel.LWAttributeSetContainer;

public class VirtualCard extends LWAttributeSetContainer {
	public enum VirtualCardStatusType {Active, InActive, Hold, Cancelled, Replaced, Expired}

    private long vcKey;
    public long getVcKey() {
		return vcKey;
	}
	public void setVcKey(long vcKey) {
		this.vcKey = vcKey;
	}

	private String loyaltyIdNumber;
	public String getLoyaltyIdNumber() {
		return loyaltyIdNumber;
	}
	public void setLoyaltyIdNumber(String loyaltyIdNumber) {
		this.loyaltyIdNumber = loyaltyIdNumber;
	}

	private long ipCode;
	public long getIpCode() {
		return ipCode;
	}
	public void setIpCode(long ipCode) {
		this.ipCode = ipCode;
	}

	private Date dateIssued;
	public Date getDateIssued() {
		return dateIssued;
	}
	public void setDateIssued(Date dateIssued) {
		this.dateIssued = dateIssued;
	}

	private Date dateRegistered;
	public Date getDateRegistered() {
		return dateRegistered;
	}
	public void setDateRegistered(Date dateRegistered) {
		this.dateRegistered = dateRegistered;
	}

	private VirtualCardStatusType status;
	public VirtualCardStatusType getStatus() {
		return status;
	}
	public void setStatus(VirtualCardStatusType status) {
		this.status = status;
	}

	private Boolean isPrimary;
	public Boolean getIsPrimary() {
		return isPrimary;
	}
	public void setIsPrimary(Boolean isPrimary) {
		this.isPrimary = isPrimary;
	}

	private long cardType;
	public long getCardType() {
		return cardType;
	}
	public void setCardType(long cardType) {
		this.cardType = cardType;
	}
}
