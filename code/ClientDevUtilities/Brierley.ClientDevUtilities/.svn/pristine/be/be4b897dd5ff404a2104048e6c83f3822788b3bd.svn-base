//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class Address
	{
		#region Properties
		public virtual string AddressLineOne { get; set; }
		public virtual string AddressLineTwo { get; set; }
		public virtual string AddressLineThree { get; set; }
		public virtual string AddressLineFour { get; set; }
		public virtual string City { get; set; }
		public virtual string StateOrProvince { get; set; }
		public virtual string ZipOrPostalCode { get; set; }
		public virtual string County { get; set; }
		public virtual string Country { get; set; }
		public virtual double Longitude { get; set; }
		public virtual double Latitude { get; set; }
		#endregion

		public override string ToString()
		{
			var cityState = !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(StateOrProvince) ? string.Format("{0}, {1}", City, StateOrProvince) : City + StateOrProvince;
			return string.Join(" \r\n", 
				(new List<string>() {
				AddressLineOne, AddressLineTwo, AddressLineThree, AddressLineFour, 
				string.Join(" ", (new List<string> {cityState, ZipOrPostalCode, Country}).Where(o => !string.IsNullOrEmpty(o))), 
				string.Join(" ", (new List<string>(){ Longitude != 0 ? Longitude.ToString() : string.Empty, Latitude != 0 ? Latitude.ToString() : string.Empty}).Where(o => !string.IsNullOrEmpty(o)))
				}).Where(o => !string.IsNullOrEmpty(o))
				);
		}
	}
}
