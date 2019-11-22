using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// A state in the survey's state diagram
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("State_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_State")]
    public class SMState : LWCoreObjectBase
    {
        #region fields
        private SMTransitionCollection _inputs = null;
        private SMTransitionCollection _outputs = null;
        #endregion

		#region properties
		/// <summary>
		/// A unique identifier.
		/// </summary>
        [PetaPoco.Column("State_ID", IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The unique identifier for the related survey.
		/// </summary>
        [PetaPoco.Column("Survey_ID", IsNullable = false)]
        [ForeignKey(typeof(SMSurvey), "ID")]
        [ColumnIndex]
        public long SurveyID { get; set; }

		/// <summary>
		/// The type of state.
		/// </summary>
        [PetaPoco.Column("StateType_ID", IsNullable = false)]
        public StateType StateType { get; set; }

		/// <summary>
		/// The "X" (horizontal) position of the state in the state diagram.  Measured from the top of the canvas, starting at 1.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public int UIPositionX { get; set; }

		/// <summary>
		/// The "Y" (vertical) position of the state in the state diagram.  Measured from the left side of the canvas, starting at 1.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public int UIPositionY { get; set; }

		/// <summary>
		/// The page for this state.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long Page { get; set; }

		/// <summary>
		/// The name of the state as seen in the state diagram.
		/// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
        public String UIName { get; set; }

		/// <summary>
		/// A description of the state as seen in the state diagram.
		/// </summary>
        [PetaPoco.Column(Length = 250, IsNullable = false)]
        public String UIDescription { get; set; }
		#endregion

        #region constructors
        /// <summary>
        /// Constructor
        /// </summary>
        public SMState()
        {
        }

		/// <summary>
		/// Copy constructor
		/// </summary>
		public SMState(SMState existing)
		{
			SurveyID = existing.SurveyID;
			StateType = existing.StateType;
			UIPositionX = existing.UIPositionX;
			UIPositionY = existing.UIPositionY;
			Page = existing.Page;
			UIName = existing.UIName;
			UIDescription = existing.UIDescription;
		}
        #endregion

        #region public methods
        /// <summary>
        /// A collection of transitions that terminate in this state.
        /// </summary>
        public SMTransitionCollection GetInputs(ServiceConfig config)
        {
            if (_inputs == null)
            {
                _inputs = new SMTransitionCollection(config, ID, TransitionCollectionType.Input);
            }
            return _inputs;
        }

        /// <summary>
        /// A collection of transitions that originate in this state.
        /// </summary>
        public SMTransitionCollection GetOutputs(ServiceConfig config)
        {
            if (_outputs == null)
            {
				_outputs = new SMTransitionCollection(config, ID, TransitionCollectionType.Output);
            }
            return _outputs;
        }

        public void ClearInputsAndOutputs()
        {
            if (_inputs != null)
            {
                _inputs.Clear();
                _inputs = null;
            }
            if (_outputs != null)
            {
                _outputs.Clear();
                _outputs = null;
            }
        }
        #endregion
    }
}

