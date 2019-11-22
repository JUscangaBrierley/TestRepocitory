using Brierley.FrameWork.Data.ModelAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLTableFieldValue")]
    [UniqueIndex(ColumnName = "FieldId,Value")]
    public class FieldValue
	{
		[PetaPoco.Column]
		public long Id { get; set; }

		[PetaPoco.Column]
        [ForeignKey(typeof(TableField), "Id")]
		public long FieldId { get; set; }

		[PetaPoco.Column("FieldValue", Length= 255, IsNullable = false)]
		public string Value { get; set; }

		[PetaPoco.Column(Length = 255)]
		public string DisplayValue { get; set; }

		[PetaPoco.Column("ValueCount")]
		public long Count { get; set; }

		public FieldValue()
		{
		}
	}
}
