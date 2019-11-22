using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGSurveyMessage : MGSurveyComponent
    {
        #region Properties
        public string Content { get; set; }
        #endregion

        #region Constructor
        public MGSurveyMessage()
        {
            TypeIdentifier = MGSurveyComponentType.Message;
        }
        #endregion
    }
}