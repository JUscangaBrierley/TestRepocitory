using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_PointType")]
	public class PointType_AL : LWObjectAuditLogBase
	{
		[PetaPoco.Column(IsNullable = false)]
		public Int64 ObjectId { get; set; }

		[PetaPoco.Column(Length = 150, IsNullable = false)]
		public String Name { get; set; }

		[PetaPoco.Column(Length = 150)]
		public String Description { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public Boolean MoneyBacked { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Int64 ConsumptionPriority { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }

		public PointType_AL()
		{
		}
	}
}
