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

public class GetRewardCatalogCountIn {
	// property: ActiveOnly
	      								
	private Boolean ActiveOnly;
	public 	Boolean getActiveOnly() {
		return this.ActiveOnly;
	}
	public 	void setActiveOnly(Boolean ActiveOnly) {
		this.ActiveOnly = ActiveOnly;
	}	
				
	// property: Tier
	@LWStringLength(50)       								
	private String Tier;
	public 	String getTier() {
		return this.Tier;
	}
	public 	void setTier(String Tier) {
		this.Tier = Tier;
	}	
				
	// property: CategoryId
	      								
	private Long CategoryId;
	public 	Long getCategoryId() {
		return this.CategoryId;
	}
	public 	void setCategoryId(Long CategoryId) {
		this.CategoryId = CategoryId;
	}	
				
	// property: ContentSearchAttributes
	      								
	private ContentSearchAttributesStruct[] ContentSearchAttributes;
	public 	ContentSearchAttributesStruct[] getContentSearchAttributes() {
		return this.ContentSearchAttributes;
	}
	public 	void setContentSearchAttributes(ContentSearchAttributesStruct[] ContentSearchAttributes) {
		this.ContentSearchAttributes = ContentSearchAttributes;
	}	
				
}
