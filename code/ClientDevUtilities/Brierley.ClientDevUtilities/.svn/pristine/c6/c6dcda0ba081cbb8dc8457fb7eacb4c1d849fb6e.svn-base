using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Brierley.FrameWork.Common;

namespace Brierley.LoyaltyWare.LWMobileGateway.DomainModel
{
    public class MGSurveyMatrixQuestionColumn
    {
        #region Properties
        public string AnchorText { get; set; }
        public bool HasColumnSummary { get; set; }        
        public long ColumnIndex { get; set; }
        #endregion
    }

    public class MGSurveyMatrixQuestionCell
    {
        #region Properties
        public long Id { get; set; }
        public int AnswerType { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string Answer { get; set; }             
        #endregion
    }

    public class MGSurveyMatrixQuestionRow
    {
        #region Properties
        
        public string Question { get; set; }
        public long Id { get; set; }
        public bool HasRowSummary { get; set; }
        public bool IsResponseOption { get; set; }
        public long RowIndex { get; set; }
        public List<MGSurveyMatrixQuestionCell> RowCells { get; set; }
        #endregion
    }

    public class MGSurveyMatrixQuestion : MGSurveyComponent
    {
        #region Properties
        public string Header { get; set; }
        public QA_PointScale PointScale { get; set; }
        public List<MGSurveyMatrixQuestionColumn> Columns { get; set; }
        public List<MGSurveyMatrixQuestionRow> Rows { get; set; }
        #endregion

        #region Constructor
        public MGSurveyMatrixQuestion()
        {
            TypeIdentifier = MGSurveyComponentType.MatrixQuestion;
        }
        #endregion
    }
}