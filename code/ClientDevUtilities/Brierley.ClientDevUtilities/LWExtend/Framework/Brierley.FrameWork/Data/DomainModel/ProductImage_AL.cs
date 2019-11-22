using System;

namespace Brierley.FrameWork.Data.DomainModel
{	
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_ProductImage")]
    public class ProductImage_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public long ProductId { get; set; }
        [PetaPoco.Column("ProductImage", Length = 255, IsNullable = false)]
		public string Image { get; set; }
        [PetaPoco.Column(IsNullable = false)]
		public long ImageOrder { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
	}
}