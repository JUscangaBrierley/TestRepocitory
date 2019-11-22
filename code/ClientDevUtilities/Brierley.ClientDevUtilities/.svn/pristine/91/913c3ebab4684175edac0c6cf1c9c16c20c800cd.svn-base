using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGSurveySimpleQuestionValidAnswerChoice
    {
        #region Properties
        public long Id { get; set; }
        public string Answer { get; set; }        
        #endregion
    }

    public class MGSurveySimpleQuestion : MGSurveyComponent
    {
        #region Properties
        public string Question { get; set; }
        public bool IsResponseOptional { get; set; }
        public QA_AnswerControlType AnswerType { get; set; }
        public QA_OrientationType AnswerOrientation { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string OtherSpecifiedPrompt { get; set; }
        public List<MGSurveySimpleQuestionValidAnswerChoice> AnswerChoices { get; set; }
        #endregion

        #region Constructor
        public MGSurveySimpleQuestion()
        {
            TypeIdentifier = MGSurveyComponentType.SimpleQuestion;
        }
        #endregion
    }
}