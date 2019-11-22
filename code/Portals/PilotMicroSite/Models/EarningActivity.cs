using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PilotMicroSite.Models
{
    public class EarningActivity
    {
        public string TxnDate { get; set; }
        public string Activity { get; set; }
        public string Order { get; set; }
        public string Store { get; set; }
        public string StoreNumber { get; set; }
        public string Points { get; set; }
    }
}