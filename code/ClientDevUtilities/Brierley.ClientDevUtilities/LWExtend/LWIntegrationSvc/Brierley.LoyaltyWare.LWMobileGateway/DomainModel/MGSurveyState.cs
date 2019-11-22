using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Brierley.FrameWork.Common;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{    
    public class MGSurveyState
    {
        #region Properties
        public long Id { get; set; }
        public StateModelStatus StateModelStatus { get; set; }
        public StateType StateType { get; set; }
        public StateModelTerminationType TerminationType { get; set; }
        public MGSurveyComponent Component { get; set; }
        #endregion
        
        #region Serialization
        public string Serialize()
        {
            string jsonStr = JsonConvert.SerializeObject(this);
            return jsonStr;
        }
        #endregion
    }
}