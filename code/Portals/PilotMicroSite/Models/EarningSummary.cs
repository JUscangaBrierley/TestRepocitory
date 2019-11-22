using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PilotMicroSite.Models
{
    public class EarningSummary
    {
        public string Tier { get; set; }
        public string AccountStatus { get; set; }
        public string LoyaltyNumber { get; set; }
        public string BasePoints { get; set; }
        public string BonusPoints { get; set; }
        public string BrasPurchased { get; set; }
        public string BrasToNextReward { get; set; }
        public string JeansPurchase { get; set; }
        public string JeansToNextReward { get; set; }
        public string PointsToNextReward { get; set; }
        public string Reward { get; set; } 
        public string TotalPoints { get; set; } 
    }
}