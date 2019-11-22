using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common
{
	public class GeoLocationUtils
	{
		//public const int minute_of_gc_arc = 6076; // one minute of great circle arc in feet
		//public const int feet_in_mile     = 5280;
		//public const int earth_diameter   = 7926;
		//public const int earth_radius     = earth_diameter / 2;

		// Angular constants
		public const double RADIANS_PER_DEGREE = Math.PI / 180d;
		public const double DEGREES_PER_RADIAN = 180d / Math.PI;
		public const double KM_PER_MINUTE = 1.852d;
		public const double MINUTES_PER_DEGREE = 60d;

		// Unit-of-measure constants
		public const double KILOMETERS_PER_STATUTE_MILE = 1.609344d;
		public const double STATUTE_MILES_PER_KILOMETER = 0.621371192d;

		public static double Degrees2Radians(double degrees)
		{
			double radians = degrees * RADIANS_PER_DEGREE;
			return radians;
		}

		public static double Radians2Degrees(double radians)
		{
			double degrees = radians * DEGREES_PER_RADIAN;
			return degrees;
		}

		public static double Kilometers2StatuteMiles(double kilometers)
		{
			double statuteMiles = kilometers / KILOMETERS_PER_STATUTE_MILE;
			return statuteMiles;
		}

		public static double StatuteMiles2Kilometers(double statuteMiles)
		{
			double kilometers = statuteMiles / STATUTE_MILES_PER_KILOMETER;
			return kilometers;
		}

		/// <summary>
		/// Provide the distance in kilometers between locations A and B.  
		/// Locations are specified using degrees latitude and logitude.
		/// </summary>
		/// <param name="latitudeA">latitude of location A in degrees</param>
		/// <param name="longitudeA">longitude of location A in degrees</param>
		/// <param name="latitudeB">latitude of location B in degrees</param>
		/// <param name="longitudeB">longitude of location B in degrees</param>
		/// <returns>distance in kilometers</returns>
		public static double GeoDistanceKM(double latitudeA, double longitudeA, double latitudeB, double longitudeB)
		{
			double latA = Degrees2Radians(latitudeA);
			double lonA = Degrees2Radians(longitudeA);
			double latB = Degrees2Radians(latitudeB);
			double lonB = Degrees2Radians(longitudeB);

			// "Great Circle" distance based on spherical trigonometry
			// Assumes: 1 minute of arc = 1 nautical mile, 1 nautical mile = 1.852km
			// This has an accuracy of about 200m over 50km, and may deteriorate over longer distances

			// x = cos(central angle in radians)
			double x = Math.Sin(latA) * Math.Sin(latB) + Math.Cos(latA) * Math.Cos(latB) * Math.Cos(Math.Abs(lonB - lonA));

			// y = arccos(x) = arctan(sqrt(1-x*x)/x) = central angle in radians
			// using arctan variant possibly because of the sign (per Craig Nelson)
			double y = Math.Atan(Math.Sqrt(1 - x * x) / x);

			// y2 = arccos(x)
			double y2 = Math.Acos(x);

			// distance in km
			double distance = KM_PER_MINUTE * MINUTES_PER_DEGREE * Radians2Degrees(y);
            if (distance < 0) distance = Math.Abs(distance);

			// convert km to statute miles
			//distance = distance / KM_PER_MILE; 

			return distance;
		}
	}
}
