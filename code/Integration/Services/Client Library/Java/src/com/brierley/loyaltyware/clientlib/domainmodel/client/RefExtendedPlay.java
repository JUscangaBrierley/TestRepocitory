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

public class RefExtendedPlay extends LWClientDataObject {			    
	// property: ExtendedPlayCode
	
	private Long ExtendedPlayCode;
	public 	Long getExtendedPlayCode() {
		return this.ExtendedPlayCode;
	}
	public 	void setExtendedPlayCode(Long extendedplaycode) {
		this.ExtendedPlayCode = extendedplaycode;
	}

	// property: Description
	@LWStringLength(255) 
	private String Description;
	public 	String getDescription() {
		return this.Description;
	}
	public 	void setDescription(String description) {
		this.Description = description;
	}

}