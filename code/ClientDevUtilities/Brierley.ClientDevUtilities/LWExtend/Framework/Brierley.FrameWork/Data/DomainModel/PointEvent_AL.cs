using System;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_PointEvent")]
    public class PointEvent_AL : LWObjectAuditLogBase
    {
		[PetaPoco.Column(IsNullable = false)]
        public Int64 ObjectId{get;set;}
		[PetaPoco.Column(Length = 150, IsNullable = false)]
		public string Name { get; set; }
		[PetaPoco.Column(Length = 500)]
		public string Description { get; set; }
		[PetaPoco.Column]
		public decimal? DefaultPoints { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the PointEvent class
        /// </summary>
        public PointEvent_AL()
        {
        }
    }
}