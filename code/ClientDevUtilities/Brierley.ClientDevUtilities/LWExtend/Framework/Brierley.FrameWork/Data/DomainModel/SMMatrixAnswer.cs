using System;
using System.Data;
using System.Configuration;
using System.Web;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// The language-specific answer for a matrix question within a particular survey.
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("MatrixAnswer_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_MatrixAnswer")]
    public class SMMatrixAnswer : LWCoreObjectBase
	{
		#region attributes
		/// <summary>
		/// A unique identifier.
		/// </summary>
        [PetaPoco.Column("MatrixAnswer_ID", IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The unique identifier for the related question content.
		/// </summary>
        [PetaPoco.Column("QuestionContent_ID", IsNullable = false)]
        public long QuestionContentID { get; set; }

		/// <summary>
		/// The unique identifier for the related language.
		/// </summary>
        [PetaPoco.Column("Language_ID", IsNullable = false)]
        public long LanguageID { get; set; }

		/// <summary>
		/// The zero-based display index for this answer in the list of answers for the related question.
		/// </summary>
        [PetaPoco.Column("Column_Index", IsNullable = false)]
        public long ColumnIndex { get; set; }

		/// <summary>
		/// Type of control used to render the answer.
		/// </summary>
        [PetaPoco.Column("Answer_Control_Type", IsNullable = false)]
        public QA_AnswerControlType AnswerControlType { get; set; }

		/// <summary>
		/// For validated types, the minimum valid value for the response as a string.
		/// </summary>
        [PetaPoco.Column("Response_MinVal", Length = 255)]
        public string ResponseMinVal { get; set; }

		/// <summary>
		/// For validated types, the maximum valid value for the response as a string.
		/// </summary>
        [PetaPoco.Column("Response_MaxVal", Length = 255)]
        public string ResponseMaxVal { get; set; }

		/// <summary>
		/// Whether this column should be summarized.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public bool ColSum { get; set; }

		#endregion

		#region constructor
		public SMMatrixAnswer()
		{
			ID = -1;
			QuestionContentID = -1;
			LanguageID = -1;
			ColumnIndex = -1;
			AnswerControlType = QA_AnswerControlType.RADIO;
			ResponseMinVal = string.Empty;
			ResponseMaxVal = string.Empty;
			ColSum = false;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		public SMMatrixAnswer(SMMatrixAnswer existing)
		{
			ID = -1;
			QuestionContentID = existing.QuestionContentID;
			LanguageID = existing.LanguageID;
			ColumnIndex = existing.ColumnIndex;
			AnswerControlType = existing.AnswerControlType;
			ResponseMinVal = existing.ResponseMinVal;
			ResponseMaxVal = existing.ResponseMaxVal;
			ColSum = existing.ColSum;
		}
		#endregion


	}
}
