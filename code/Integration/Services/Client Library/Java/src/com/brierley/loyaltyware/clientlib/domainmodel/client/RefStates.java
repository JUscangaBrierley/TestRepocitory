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

public class RefStates extends LWClientDataObject {			    
	// property: StateCode
	@LWStringLength(10) 
	private String StateCode;
	public 	String getStateCode() {
		return this.StateCode;
	}
	public 	void setStateCode(String statecode) {
		this.StateCode = statecode;
	}

	// property: FullName
	@LWStringLength(255) 
	private String FullName;
	public 	String getFullName() {
		return this.FullName;
	}
	public 	void setFullName(String fullname) {
		this.FullName = fullname;
	}

	// property: CountryCode
	@LWStringLength(10) 
	private String CountryCode;
	public 	String getCountryCode() {
		return this.CountryCode;
	}
	public 	void setCountryCode(String countrycode) {
		this.CountryCode = countrycode;
	}

}