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

public class RefVendors extends LWClientDataObject {			    
	// property: VendorId
	@LWIsRequired @LWStringLength(255) 
	private String VendorId;
	public 	String getVendorId() {
		return this.VendorId;
	}
	public 	void setVendorId(String vendorid) {
		this.VendorId = vendorid;
	}

	// property: VendorName
	@LWIsRequired @LWStringLength(255) 
	private String VendorName;
	public 	String getVendorName() {
		return this.VendorName;
	}
	public 	void setVendorName(String vendorname) {
		this.VendorName = vendorname;
	}

}