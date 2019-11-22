using System;
using System.Data;
using System.Configuration;
using System.Web;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// The language-specific text for an answer within a particular survey.
	/// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("AnswerContent_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_AnswerContent")]
	public class SMAnswerContent : LWCoreObjectBase
	{
		#region properties
		/// <summary>
		/// A unique identifier.
		/// </summary>
        [PetaPoco.Column("AnswerContent_ID", IsNullable = false)]
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
		/// The zero-based display index for this answer in the list of answers for the related question.
		/// </summary>
        [PetaPoco.Column("Display_Index", IsNullable = false)]
        public long DisplayIndex { get; set; }

		/// <summary>
		/// The content of the answer.
		/// </summary>
		[PetaPoco.Column]
        public string Content { get; set; }

		/// <summary>
		/// Whether the order that this answer is displayed should be random.
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
        public bool Randomize { get; set; }

		/// <summary>
		/// The visibility expression for this answer.
		/// </summary>
        [PetaPoco.Column("VisibilityExpr")]
        public string VisibilityExpression { get; set; }
		#endregion

		#region constructors
		/// <summary>
		/// Default constructor
		/// </summary>
		public SMAnswerContent()
		{
			ID = -1;
			QuestionID = -1;
			LanguageID = -1;
			DisplayIndex = -1;
			Content = string.Empty;
			VisibilityExpression = string.Empty;
			Randomize = false;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		public SMAnswerContent(SMAnswerContent existing)
		{
			ID = -1;
			QuestionID = existing.QuestionID;
			LanguageID = existing.LanguageID;
			DisplayIndex = existing.DisplayIndex;
			Content = existing.Content;
			VisibilityExpression = existing.VisibilityExpression;
			Randomize = existing.Randomize;
		}
		#endregion
	}
}
