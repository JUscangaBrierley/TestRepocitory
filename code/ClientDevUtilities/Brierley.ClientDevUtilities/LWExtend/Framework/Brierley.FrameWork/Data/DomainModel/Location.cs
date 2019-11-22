using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Indicates the location of a mobile device (iPhone or Android) at a point in time.
	/// </summary>
    [DataContract]
	public class Location
	{
		#region fields
		private double _latitude;
		private double _longitude;
		private double _altitude;
		private double _speed;
		private double _heading;
		private double _positionAccuracy;
		private double _altitudeAccuracy;
		private DateTime _whenDetermined;
        #endregion
		
		#region properties
		/// <summary>
		/// The latitude of the location measured in +/- degrees
		/// </summary>
		[DataMember]
		public virtual double Latitude
		{
			get { return this._latitude; }
			set { this._latitude = value; }
		}

		/// <summary>
		/// The longitude of the location measured in +/- degrees
		/// </summary>
		[DataMember]
		public virtual double Longitude
		{
			get { return this._longitude; }
			set { this._longitude = value; }
		}

		/// <summary>
		/// The altitude of the location measured in +/- meters
		/// </summary>
		[DataMember]
		public virtual double Altitude
		{
			get { return this._altitude; }
			set { this._altitude = value; }
		}

		/// <summary>
		/// The non-negative speed of travel measured in meters/second
		/// </summary>
		[DataMember]
		public virtual double Speed
		{
			get { return this._speed; }
			set { this._speed = value; }
		}

		/// <summary>
		/// The non-negative compass heading measured in degrees east of due north
		/// </summary>
		[DataMember]
		public virtual double Heading
		{
			get { return this._heading; }
			set { this._heading = value; }
		}

		/// <summary>
		/// The non-negative radius of lattitude/longitude accuracy measured in meters
		/// </summary>
		[DataMember]
		public virtual double PositionAccuracy
		{
			get { return this._positionAccuracy; }
			set { this._positionAccuracy = value; }
		}

		/// <summary>
		/// The non-negative altitude accuracy measured in meters
		/// </summary>
		[DataMember]
		public virtual double AltitudeAccuracy
		{
			get { return this._altitudeAccuracy; }
			set { this._altitudeAccuracy = value; }
		}

		/// <summary>
		/// The date/time when the location was determined
		/// </summary>
		[DataMember]
		public virtual DateTime WhenDetermined
		{
			get { return this._whenDetermined; }
			set { this._whenDetermined = value; }
		}
		#endregion
	}
}
