using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;
using System.Runtime.Serialization;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    #region Enumerations
    public enum MGSurveyComponentType { Message, Page, SimpleQuestion, MatrixQuestion };    
    #endregion

	[KnownType(typeof(MGSurveySimpleQuestion))]
	[KnownType(typeof(MGSurveyMatrixQuestion))]
	[KnownType(typeof(MGSurveyMessage))]
	[KnownType(typeof(MGSurveyPage))]
    public class MGSurveyComponent
    {
        #region Properties        
        public long Id { get; set; }        
        public MGSurveyComponentType TypeIdentifier { get; set; }
        public string Description { get; set; }        
        public long StateId { get; set; }
        #endregion
        
        #region Serialization
        public virtual string Serialize()
        {
            return string.Empty;
        }
        #endregion
    }
}