using System;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_ProductVariant")]
    public class ProductVariant_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public long ProductId { get; set; }
        [PetaPoco.Column(Length = 255, IsNullable = false)]
		public string VariantDescription { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string PartNumber { get; set; }
		[PetaPoco.Column]
		public long? Quantity { get; set; }
		[PetaPoco.Column]
		public long? QuantityThreshold { get; set; }
        [PetaPoco.Column(IsNullable = false)]
		public long VariantOrder { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
    }
}