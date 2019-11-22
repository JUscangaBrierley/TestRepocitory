using System;
using System.Data;
using System.Configuration;
using System.Web;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Records that a concept was shown to a particular respondent.
	/// </summary>
	[Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("ConceptView_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_SM_ConceptView")]
    public class SMConceptView : LWCoreObjectBase
	{
		#region properties
		/// <summary>
		/// The unique identifier for this concept view.
		/// </summary>
        [PetaPoco.Column("ConceptView_ID", IsNullable = false)]
        public long ID { get; set; }

		/// <summary>
		/// The unique identifier for the associated concept.
		/// </summary>
        [PetaPoco.Column("Concept_ID", IsNullable = false)]
        [ForeignKey(typeof(SMConcept), "ID")]
        public long ConceptID { get; set; }

		/// <summary>
		/// The unique identifier for the associated respondent.
		/// </summary>
        [PetaPoco.Column("Respondent_ID", IsNullable = false)]
        [ForeignKey(typeof(SMRespondent), "ID")]
        public long RespondentID { get; set; }

		/// <summary>
		/// The unique identifier for the associated state.
		/// </summary>
        [PetaPoco.Column("State_ID", IsNullable = false)]
        public long StateID { get; set; }
		#endregion

		#region constructors
		public SMConceptView()
		{
			ID = -1;
			ConceptID = -1;
			RespondentID = -1;
			StateID = -1;
		}
		#endregion
	}
}
