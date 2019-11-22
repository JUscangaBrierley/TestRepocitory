using System;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_Promotion")]
    public class Promotion_AL : LWObjectAuditLogBase
    {
        /// <summary>
        /// Gets or sets the Id for the current Promotion
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the Code for the current Promotion
        /// </summary>
        [PetaPoco.Column(Length = 150, IsNullable = false)]
		public string Code { get; set; }

        /// <summary>
        /// Gets or sets the Name for the current Promotion
        /// </summary>
        [PetaPoco.Column(Length = 100, IsNullable= false)]
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Internal Description for the current Promotion
        /// </summary>
        [PetaPoco.Column(Length = 500)]
        public virtual string PromotionDescription { get; set; }

        /// <summary>
        /// Gets or sets the type of enrollment supported by the promotion.
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public PromotionEnrollmentSupportType EnrollmentSupportType { get; set; }

		/// <summary>
		/// Gets or sets the Folder Id for the current Promotion
		/// </summary>
		[PetaPoco.Column]
		public long? FolderId { get; set; }

        /// <summary>
        /// Gets or sets the StartDate for the current Promotion
        /// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the EndDate for the current Promotion
        /// </summary>
		[PetaPoco.Column]
		public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the DisplayOrder for the current MessageDef
        /// </summary>
		[PetaPoco.Column]
		public int? DisplayOrder { get; set; }

        /// <summary>
        /// Targeted
        /// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public bool Targeted { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
		
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
		
        /// <summary>
        /// Initializes a new instance of the PointEvent class
        /// </summary>
        public Promotion_AL()
        {
            Targeted = true;
        }
    }
}
