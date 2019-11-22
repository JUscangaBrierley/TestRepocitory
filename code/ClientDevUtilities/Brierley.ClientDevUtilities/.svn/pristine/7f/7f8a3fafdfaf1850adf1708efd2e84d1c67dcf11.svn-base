using System;
using System.Data;
using System.Configuration;
using System.Web;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// A question within a particular survey.
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Question_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_Question")]
    public class SMQuestion : LWCoreObjectBase
    {
		#region properties
		/// <summary>
		/// A unique identifier.
		/// </summary>
        [PetaPoco.Column("Question_ID", IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// The unique identifier for the related survey.
		/// </summary>
        [PetaPoco.Column("State_ID", IsNullable = false)]
        [ForeignKey(typeof(SMState), "ID")]
        [ColumnIndex]
        public long StateID { get; set; }

		/// <summary>
		/// The effective date.
		/// </summary>
        [PetaPoco.Column("Effective_Date", IsNullable = false)]
        public DateTime EffectiveDate { get; set; }

		/// <summary>
		/// The expiration date.
		/// </summary>
        [PetaPoco.Column("Expiration_Date", IsNullable = false)]
        public DateTime ExpirationDate { get; set; }

		/// <summary>
		/// Is this a question matrix?
		/// </summary>
        [PetaPoco.Column("Is_Matrix", IsNullable = false)]
        public bool IsMatrix { get; set; }

		/// <summary>
		/// For radio, checkbox, and multiselect text types, should an "Other - Specify" textbox be added?
		/// </summary>
        [PetaPoco.Column("Has_OtherSpecify", IsNullable = false)]
        public bool HasOtherSpecify { get; set; }

		/// <summary>
		/// For question matrix, the size of the point scale (e.g., 7 point scale).
		/// </summary>
        [PetaPoco.Column("Matrix_Point_Scale", IsNullable = false)]
        public QA_PointScale MatrixPointScale { get; set; }

		/// <summary>
		/// Type of control used to render the answer.
		/// </summary>
        [PetaPoco.Column("Answer_Control_Type", IsNullable = false)]
        public QA_AnswerControlType AnswerControlType { get; set; }

		/// <summary>
		/// For radio, checkbox, and multiselect text types, the orientation of the answer list.
		/// </summary>
        [PetaPoco.Column("Answer_Orientation", IsNullable = false)]
        public QA_OrientationType AnswerOrientation { get; set; }

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
		/// Whether the response to this question is optional.
		/// </summary>
        [PetaPoco.Column("Response_Optional", IsNullable = false)]
        public bool ResponseOptional { get; set; }

		/// <summary>
		/// Whether the answers for this question are piped from the responses for another question.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public bool IsPiped { get; set; }

		/// <summary>
		/// The ID of the (question) state from which responses this question's anwers are piped.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long PipedStateID { get; set; }

		/// <summary>
		/// For matrix question, limits the number of rows displayed.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public int RowLimit { get; set; }

        // CS-73
        /// <summary>
        /// For matrix question, sets the total that free form text boxes must equal to continue.
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string ValidationTotal { get; set; }
		#endregion

        #region constructor
		/// <summary>
		/// Default constructor
		/// </summary>
        public SMQuestion()
        {
            ID = -1;
            StateID = -1;
            EffectiveDate = DateTimeUtil.MinValue;
            ExpirationDate = DateTimeUtil.MaxValue;
			IsMatrix = false;
			HasOtherSpecify = false;
			MatrixPointScale = QA_PointScale.Seven;
			AnswerControlType = QA_AnswerControlType.RADIO;
			AnswerOrientation = QA_OrientationType.VERTICAL;
			ResponseMinVal = string.Empty;
			ResponseMaxVal = string.Empty;
			ResponseOptional = false;
			IsPiped = false;
			PipedStateID = 0;
			RowLimit = 0;
            ValidationTotal = string.Empty; // CS-73
        }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public SMQuestion(SMQuestion existing)
		{
			ID = -1;
			StateID = existing.StateID;
			EffectiveDate = existing.EffectiveDate;
			ExpirationDate = existing.ExpirationDate;
			IsMatrix = existing.IsMatrix;
			HasOtherSpecify = existing.HasOtherSpecify;
			MatrixPointScale = existing.MatrixPointScale;
			AnswerControlType = existing.AnswerControlType;
			AnswerOrientation = existing.AnswerOrientation;
			ResponseMinVal = existing.ResponseMinVal;
			ResponseMaxVal = existing.ResponseMaxVal;
			ResponseOptional = existing.ResponseOptional;
			IsPiped = existing.IsPiped;
			PipedStateID = existing.PipedStateID;
			RowLimit = existing.RowLimit;
            ValidationTotal = existing.ValidationTotal; // CS-73
		}
        #endregion

        #region public methods
        public bool IsSameAs(SMQuestion other)
        {
            if (EffectiveDate != other.EffectiveDate) return false;
            if (ExpirationDate != other.ExpirationDate) return false;
			if (IsMatrix != other.IsMatrix) return false;
			if (HasOtherSpecify != other.HasOtherSpecify) return false;
			if (MatrixPointScale != other.MatrixPointScale) return false;
			if (AnswerControlType != other.AnswerControlType) return false;
			if (AnswerOrientation != other.AnswerOrientation) return false;
			if (ResponseMinVal != other.ResponseMinVal) return false;
			if (ResponseMaxVal != other.ResponseMaxVal) return false;
			if (ResponseOptional != other.ResponseOptional) return false;
			if (IsPiped != other.IsPiped) return false;
			if (PipedStateID != other.PipedStateID) return false;
			if (RowLimit != other.RowLimit) return false;
            if (ValidationTotal != other.ValidationTotal) return false; // CS-73
            return true;
        }

		public bool IsListAnswerType()
		{
			switch (AnswerControlType)
			{
				case QA_AnswerControlType.RADIO:
				case QA_AnswerControlType.CHECK:
				case QA_AnswerControlType.LIST:
				case QA_AnswerControlType.DROPDOWN:
					return true;
			}
			return false;
		}

		public bool IsMultiselectSimpleQuestion()
		{
			bool result = false;
			if (!IsMatrix)
			{
				switch (AnswerControlType)
				{
					case QA_AnswerControlType.RADIO:
					case QA_AnswerControlType.DROPDOWN:
					case QA_AnswerControlType.SHORT_TEXT:
					case QA_AnswerControlType.LONG_TEXT:
					case QA_AnswerControlType.DATETIME:
					case QA_AnswerControlType.DATE:
					case QA_AnswerControlType.TIME:
					case QA_AnswerControlType.INTEGER:
					case QA_AnswerControlType.REAL:
					case QA_AnswerControlType.DOLLAR:
					case QA_AnswerControlType.PERCENT:
						result = false;
						break;

					case QA_AnswerControlType.CHECK:
					case QA_AnswerControlType.LIST:
						result = true;
						break;
				}
			}
			return result;
		}

		public bool IsMultiAnswer()
		{
			if (IsMatrix) return true;
			if (IsMultiselectSimpleQuestion()) return true;
			return false;
		}

		public bool CanBePiped()
		{
			bool result = IsPiped && PipedStateID > 0;
			return result;
		}
        #endregion
    }
}
