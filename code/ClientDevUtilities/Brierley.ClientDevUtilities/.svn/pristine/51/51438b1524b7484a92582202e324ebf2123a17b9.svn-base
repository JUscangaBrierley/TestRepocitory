using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class NearbyStoreItem
	{
		public double DistanceInMiles { get; set; }
		public StoreDef Store { get; set; }

		public double DistanceInKM
		{
			get
			{
				return GeoLocationUtils.StatuteMiles2Kilometers(DistanceInMiles);
			}
		}
	}

	public class NearbyStoreCollection : List<NearbyStoreItem> { }
}
