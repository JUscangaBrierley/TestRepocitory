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

public class GetMembersIn {
	// property: MemberSearchType
	@LWIsRequired       								
	private String[] MemberSearchType;
	public 	String[] getMemberSearchType() {
		return this.MemberSearchType;
	}
	public 	void setMemberSearchType(String[] MemberSearchType) {
		this.MemberSearchType = MemberSearchType;
	}	
				
	// property: SearchValue
	@LWIsRequired       								
	private String[] SearchValue;
	public 	String[] getSearchValue() {
		return this.SearchValue;
	}
	public 	void setSearchValue(String[] SearchValue) {
		this.SearchValue = SearchValue;
	}	
				
	// property: StartIndex
	      								
	private Integer StartIndex;
	public 	Integer getStartIndex() {
		return this.StartIndex;
	}
	public 	void setStartIndex(Integer StartIndex) {
		this.StartIndex = StartIndex;
	}	
				
	// property: BatchSize
	      								
	private Integer BatchSize;
	public 	Integer getBatchSize() {
		return this.BatchSize;
	}
	public 	void setBatchSize(Integer BatchSize) {
		this.BatchSize = BatchSize;
	}	
				
}
