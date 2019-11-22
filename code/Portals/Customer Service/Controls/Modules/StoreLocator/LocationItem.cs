using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brierley.LWModules.StoreLocator
{
	public class LocationItem
	{
		public string ZipOrPostalCode { get; set; }
		public string City { get; set; }
		public string StateOrProvince { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int TimeZoneOffset { get; set; }
		public bool ObservesDST { get; set; }
	}
}