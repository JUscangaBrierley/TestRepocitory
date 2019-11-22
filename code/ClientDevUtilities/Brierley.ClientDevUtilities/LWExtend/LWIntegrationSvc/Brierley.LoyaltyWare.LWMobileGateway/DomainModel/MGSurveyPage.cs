using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGSurveyPage : MGSurveyComponent
    {
        #region Properties        
        public List<MGSurveyComponent> Components { get; set; }
        #endregion

        #region Constructor
        public MGSurveyPage()
        {
            TypeIdentifier = MGSurveyComponentType.Page;
        }
        #endregion
    }
}