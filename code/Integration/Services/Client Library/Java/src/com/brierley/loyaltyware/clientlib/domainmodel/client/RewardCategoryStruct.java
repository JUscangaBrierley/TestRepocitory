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

public class RewardCategoryStruct {
	// property: CategoryId
	@LWIsRequired       								
	private Long CategoryId;
	public 	Long getCategoryId() {
		return this.CategoryId;
	}
	public 	void setCategoryId(Long CategoryId) {
		this.CategoryId = CategoryId;
	}	
				
	// property: CategoryName
	@LWIsRequired       								
	private String CategoryName;
	public 	String getCategoryName() {
		return this.CategoryName;
	}
	public 	void setCategoryName(String CategoryName) {
		this.CategoryName = CategoryName;
	}	
				
}
