using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Brierley.FrameWork.Common;

namespace Brierley.LWModules.StoreLocator
{
	public class DefaultStoreLocatorInterceptor : IStoreLocatorInterceptor
	{
		private static Object _locationsMutex = new Object();
		private static List<LocationItem> _locations;

		public LocationItem NearestLocationItem(double latitude, double longitude)
		{
			InitializeLocations();
			double degrees = 1;
			var nearestLocation = (from loc in _locations
								   where loc.Latitude <= (latitude + degrees) && loc.Latitude >= (latitude - degrees)
								   && loc.Longitude <= (longitude + degrees) && loc.Longitude >= (longitude - degrees)
								   orderby GeoLocationUtils.GeoDistanceKM(loc.Latitude, loc.Longitude, latitude, longitude) ascending
								   select loc).FirstOrDefault<LocationItem>();
			return nearestLocation;
		}

		public LocationItem NearestLocationItem(string city, string stateOrProvince)
		{
			InitializeLocations();
			city = city.Trim().ToLower();
			stateOrProvince = stateOrProvince.Trim().ToLower();
			var nearestLocation = (from loc in _locations
								   where loc.City.ToLower() == city && loc.StateOrProvince.ToLower() == stateOrProvince
								   select loc).FirstOrDefault<LocationItem>();
			return nearestLocation;
		}

		public LocationItem NearestLocationItem(string zipOrPostalCode)
		{
			InitializeLocations();
			zipOrPostalCode = zipOrPostalCode.Trim().ToLower();
			var nearestLocation = (from loc in _locations
								   where loc.ZipOrPostalCode.ToLower() == zipOrPostalCode
								   select loc).FirstOrDefault<LocationItem>();
			return nearestLocation;
		}

		private void InitializeLocations()
		{
			lock (_locationsMutex)
			{
				if (_locations == null)
				{
					_locations = new List<LocationItem>();
					string data = ResourceUtils.GetManifestResourceString("Brierley.LWModules.StoreLocator.zipcode.csv");
					using (StreamReader reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(data))))
					{
						// consume header
						reader.ReadLine();

						// read rows
						string line = null;
						while (!reader.EndOfStream)
						{
							line = reader.ReadLine();
							if (string.IsNullOrEmpty(line)) continue;

							string[] tokens = line.Split(',');
							LocationItem item = new LocationItem()
							{
								ZipOrPostalCode = TrimToken(tokens, 0),
								City = TrimToken(tokens, 1),
								StateOrProvince = TrimToken(tokens, 2),
								Latitude = double.Parse(TrimToken(tokens, 3)),
								Longitude = double.Parse(TrimToken(tokens, 4)),
								TimeZoneOffset = int.Parse(TrimToken(tokens, 5)),
								ObservesDST = TrimToken(tokens, 6) == "1" ? true : false
							};
							_locations.Add(item);
						}
					}
				}
			}
		}

		private string TrimToken(string[] tokens, int index)
		{
			if (tokens == null || index >= tokens.Length) return string.Empty;

			string token = tokens[index].Trim();
			if (token[0] == '"' || token[0] == '\'')
			{
				token = token.Substring(1);
			}
			int lastCharIndex = token.Length - 1;
			if (token[lastCharIndex] == '"' || token[lastCharIndex] == '\'')
			{
				token = token.Substring(0, lastCharIndex);
			}
			return token;
		}
	}
}