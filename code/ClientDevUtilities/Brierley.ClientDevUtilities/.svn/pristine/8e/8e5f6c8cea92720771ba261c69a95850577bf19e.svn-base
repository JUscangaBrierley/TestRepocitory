using System;
using System.Data;
using System.Configuration;
using System.Web;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// An answer to a question on a survey by a respondent.
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Response_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_Response")]
    [ColumnIndex(ColumnName = "RespondentID,QuestionContentID")]
    public class SMResponse : LWCoreObjectBase
	{
		#region properties
		/// <summary>
		/// A unique identifier.
		/// </summary>
        [PetaPoco.Column("Response_ID", IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The unique identifier of the associated Respondent.
		/// </summary>
        [PetaPoco.Column("Respondent_ID", IsNullable = false)]
        [ForeignKey(typeof(SMRespondent), "ID")]
        public long RespondentID { get; set; }

		/// <summary>
		/// The related State.
		/// </summary>
        [PetaPoco.Column("State_ID", IsNullable = false)]
        public long StateID { get; set; }

		/// <summary>
		/// The related QuestionContent
		/// </summary>
        [PetaPoco.Column("QuestionContent_ID", IsNullable = false)]
        public long QuestionContentID { get; set; }

		/// <summary>
		/// For non free-form text answers, this is the related AnswerContent.
		/// </summary>
        [PetaPoco.Column("AnswerContent_ID", IsNullable = false)]
        public long AnswerContentID { get; set; }

		/// <summary>
		/// For piped matrix questions, this is the response from which this response has been piped.
		/// </summary>
        [PetaPoco.Column("PipedResponse_ID", IsNullable = false)]
        public long PipedResponseID { get; set; }

		/// <summary>
		/// For matrix answers, this is the related MatrixAnswer.
		/// </summary>
        [PetaPoco.Column("MatrixAnswer_ID", IsNullable = false)]
        public long MatrixAnswerID { get; set; }

		/// <summary>
		/// For matrix answers, this is the column index.
		/// </summary>
        [PetaPoco.Column("Column_Index", IsNullable = false)]
        public long ColumnIndex { get; set; }

		/// <summary>
		/// For free-form text answers, this is the actual content.
		/// </summary>
        [PetaPoco.Column(Length = 2000)]
        public string Content { get; set; }

		/// <summary>
		/// For piped questions, this is the piped content.
		/// </summary>
        [PetaPoco.Column(Length = 2000)]
        public string PipedContent { get; set; }

		/// <summary>
		/// The date/time this the related question was started.
		/// </summary>
        [PetaPoco.Column("Start_Date", IsNullable = false)]
        public DateTime StartDate { get; set; }

		/// <summary>
		/// The date/time the response was completed for the related question.
		/// </summary>
        [PetaPoco.Column("Complete_Date", IsNullable = false)]
        public DateTime CompleteDate { get; set; }
		#endregion

		#region constructors
		public SMResponse()
		{
			ID = -1;
			RespondentID = -1;
			StateID = -1;
			QuestionContentID = -1;
			AnswerContentID = -1;
			PipedResponseID = -1;
			MatrixAnswerID = -1;
			ColumnIndex = -1;
			Content = string.Empty;
			PipedContent = string.Empty;
		}
		#endregion
	}
}
