using System;
using System.Collections.Generic;
using System.Linq;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{	
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_Product")]
    public class Product_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
        public long ObjectId { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public long CategoryId { get; set; }
		[PetaPoco.Column]
		public bool? IsVisibleInLn { get; set; }
		[PetaPoco.Column(Length = 255, IsNullable = false)]
		public string Name { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string BrandName { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string PartNumber { get; set; }
        [PetaPoco.Column(IsNullable = false)]
		public decimal BasePrice { get; set; }
		[PetaPoco.Column]
		public long? PointType { get; set; }
		[PetaPoco.Column]
		public long? Quantity { get; set; }
		[PetaPoco.Column]
		public long? QuantityThreshold { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string ClassCode { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string ClassDescription { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string StyleCode { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string StyleDescription { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string DeptCode { get; set; }
		[PetaPoco.Column(Length = 100)]
		public string DeptDescription { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string DivisionCode { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string DivisionDescription { get; set; }
        [PetaPoco.Column(Length = 50)]
		public string CompanyCode { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string CompanyDescription { get; set; }
        [PetaPoco.Column(IsNullable = false)]
		public long CreatedByUser { get; set; }
        [PetaPoco.Column(Length = 100)]
		public string StrUserField { get; set; }
		[PetaPoco.Column]
		public long? LongUserField { get; set; }
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
	}
}