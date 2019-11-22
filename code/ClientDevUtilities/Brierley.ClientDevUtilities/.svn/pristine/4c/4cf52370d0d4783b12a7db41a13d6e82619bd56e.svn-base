using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGLocation
    {
        #region properties
        /// <summary>
        /// The latitude of the location measured in +/- degrees
        /// </summary>
        public virtual double Latitude { set; get; }        

        /// <summary>
        /// The longitude of the location measured in +/- degrees
        /// </summary>
        public virtual double Longitude { set; get; }

        /// <summary>
        /// The altitude of the location measured in +/- meters
        /// </summary>
        public virtual double Altitude { set; get; }

        /// <summary>
        /// The non-negative speed of travel measured in meters/second
        /// </summary>
        public virtual double Speed { set; get; }

        /// <summary>
        /// The non-negative compass heading measured in degrees east of due north
        /// </summary>
        public virtual double Heading { set; get; }

        /// <summary>
        /// The non-negative radius of lattitude/longitude accuracy measured in meters
        /// </summary>
        public virtual double PositionAccuracy { set; get; }

        /// <summary>
        /// The non-negative altitude accuracy measured in meters
        /// </summary>
        public virtual double AltitudeAccuracy { set; get; }

        /// <summary>
        /// The date/time when the location was determined
        /// </summary>
        public virtual DateTime WhenDetermined { set; get; }
        
        #endregion

        #region Data Transfer Methods
        public static MGLocation ConvertFromJson(string locationString)
        {
            return JsonConvert.DeserializeObject<MGLocation>(locationString);            
        }
        #endregion
    }
}