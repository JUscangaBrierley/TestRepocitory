using Brierley.FrameWork.Data.ModelAttributes;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.CampaignManagement.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("FieldId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_CLTableField")]
	public class TableField
	{
		[PetaPoco.Column("FieldId")]
		public long Id { get; set; }

		[PetaPoco.Column]
        [ForeignKey(typeof(CampaignTable), "Id")]
        [ColumnIndex]
		public long TableId { get; set; }

		[PetaPoco.Column("FieldName", Length = 150, IsNullable = false)]
		public string Name { get; set; }

		[PetaPoco.Column("FieldType", Length = 150)]
		public string DataType { get; set; }

		[PetaPoco.Column(Length = 150)]
		public string Alias { get; set; }

		[PetaPoco.Column]
		public bool? Visible { get; set; }

		[PetaPoco.Column]
		public ValueGenerationType ValueGenerationType { get; set; }

		[PetaPoco.Column]
		public string ValueList { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public AttributeEncryptionType EncryptionType { get; set; }

		public TableField()
		{
			//set defaults:
			Visible = true;
			ValueGenerationType = ValueGenerationType.NotGenerated;
            EncryptionType = AttributeEncryptionType.None;
		}
	}
}
