using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PilotMicroSite.Models
{
    public class ReturnLoyaltyNumber
    {
        [JsonProperty("rewardsNumber")]
        public string LoyaltyNumber { get; set; }
        [JsonProperty("error")]
        public string ErrorMessage { get; set; }
    }
}