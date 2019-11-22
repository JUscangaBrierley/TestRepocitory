using System;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_AL_MemberTiers")]
    public class MemberTier_AL : LWObjectAuditLogBase
    {
        #region Fields
		#endregion

        #region Constructors		
		/// <summary>
        /// Initializes a new instance of the MemberTier_AR class
		/// </summary>
		public MemberTier_AL()
		{
		}				
		#endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Id for the current MemberTier
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public Int64 ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the TierDefId for the current MemberTier
        /// </summary>
        [PetaPoco.Column("TierId", IsNullable = false)]
        public Int64 TierDefId { get; set; }

        /// <summary>
        /// Gets or sets the MemberId for the current MemberTier
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public Int64 MemberId { get; set; }

        /// <summary>
        /// Gets or sets the FormDate for the current MemberTier
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// Gets or sets the ToDate for the current MemberTier
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// Gets or sets the Description 
        /// </summary>
        [PetaPoco.Column(Length = 150)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the CreateDate for the current LIBJob
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the UpdateDate for the current LIBJob
        /// </summary>
        [PetaPoco.Column]
        public DateTime? UpdateDate { get; set; }
        #endregion
    }
}