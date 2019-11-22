using Brierley.FrameWork.Data.ModelAttributes;
using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for PointEvent.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("PointEventId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_PointEvent")]
    [AuditLog(true)]
	public class PointEvent : LWCoreObjectBase
	{
		/// <summary>
		/// Initializes a new instance of the PointEvent class
		/// </summary>
		public PointEvent()
		{
		}

		/// <summary>
		/// Gets or sets the ID for the current PointEvent
		/// </summary>
        [PetaPoco.Column("PointEventId", IsNullable = false)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the Name for the current PointEvent
		/// </summary>
        [PetaPoco.Column(Length = 150, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Description for the current PointEvent
		/// </summary>
        [PetaPoco.Column(Length = 500)]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the DefaultPoints for the current PointEvent
		/// </summary>
        [PetaPoco.Column]
		public decimal? DefaultPoints { get; set; }

		public PointEvent Clone()
		{
			return Clone(new PointEvent());
		}

		public PointEvent Clone(PointEvent other)
		{
			other.Name = Name;
			other.Description = Description;
			other.DefaultPoints = DefaultPoints;
			return (PointEvent)base.Clone(other);
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			PointEvent_AL ar = new PointEvent_AL()
			{
				ObjectId = this.ID,
				Name = this.Name,
				Description = this.Description,
				DefaultPoints = this.DefaultPoints,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}