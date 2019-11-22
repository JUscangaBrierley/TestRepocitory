using System;
using System.Data;
using System.Configuration;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// The language-specific text for a question within a particular survey.
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("QuestionContent_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_QuestionContent")]
    public class SMQuestionContent : LWCoreObjectBase
	{
		#region properties
		/// <summary>
		/// A unique identifier.
		/// </summary>
		[PetaPoco.Column("QuestionContent_ID", IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The unique identifier for the related question.
		/// </summary>
        [PetaPoco.Column("Question_ID", IsNullable = false)]
        [ForeignKey(typeof(SMQuestion), "ID")]
		public long QuestionID { get; set; }

		/// <summary>
		/// The unique identifier for the related language.
		/// </summary>
        [PetaPoco.Column("Language_ID", IsNullable = false)]
        [ForeignKey(typeof(SMLanguage), "ID")]
        public long LanguageID { get; set; }

		/// <summary>
		/// For matrix body content, the zero-based index for this item in the matrix.
		/// </summary>
        [PetaPoco.Column("Matrix_Index", IsNullable = false)]
        public long MatrixIndex { get; set; }

		/// <summary>
		/// The type of content represented by this object.  E.g., header text or body text.
		/// </summary>
        [PetaPoco.Column("Content_Type", IsNullable = false)]
        public QuestionContentType ContentType { get; set; }

		/// <summary>
		/// The content of the question
		/// </summary>
        [PetaPoco.Column]
        public string Content { get; set; }

		/// <summary>
		/// For matrix questions, expression used to determine whether this question is visible.
		/// </summary>
        [PetaPoco.Column("VisibilityExpr")]
        public string VisibilityExpression { get; set; }

		/// <summary>
		/// For matrix questions, whether the order that this question is displayed should be random.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public bool Randomize { get; set; }

		/// <summary>
		/// For matrix questions, whether this row should be summarized.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public bool RowSum { get; set; }
		#endregion

		#region constructors
		/// <summary>
		/// Default constructor
		/// </summary>
		public SMQuestionContent()
		{
			ID = -1;
			QuestionID = -1;
			LanguageID = -1;
			MatrixIndex = -1;
			ContentType = QuestionContentType.BODY_TEXT;
			Content = string.Empty;
			VisibilityExpression = string.Empty;
			Randomize = false;
			RowSum = false;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		public SMQuestionContent(SMQuestionContent existing)
		{
			ID = -1;
			QuestionID = existing.QuestionID;
			LanguageID = existing.LanguageID;
			MatrixIndex = existing.MatrixIndex;
			ContentType = existing.ContentType;
			Content = existing.Content;
			VisibilityExpression = existing.VisibilityExpression;
			Randomize = existing.Randomize;
			RowSum = existing.RowSum;
		}
		#endregion
	}
}
