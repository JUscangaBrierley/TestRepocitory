using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PilotMicroSite.Models
{
    public class Profile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }
        public string Gender { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CityStateZip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string MobilePhone { get; set; }
        public string EmailAddress { get; set; }
        public Dictionary<string, string> Countries { get; set; }
        public string SelectedCountry { get; set; }
        public Dictionary<string, string> States { get; set; }
        public string SelectedState { get; set; }
    }
}