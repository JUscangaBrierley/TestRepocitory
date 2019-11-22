using System;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for Attribute.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_Attribute")]
    public class AttributeMetaData_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public Int64 ObjectId { get; set; }

		[PetaPoco.Column("StatusCode", IsNullable=false)]
		public Int64 Status { get; set; }

        [PetaPoco.Column("AttributeName", Length = 50, IsNullable = false)]
		public String Name { get; set; }

        [PetaPoco.Column("DataTypeCode", Length= 20, IsNullable = false)]
		public String DataType { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Int64 AttributeSetCode { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean IsRequired { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Int64 MinLength { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Int64 MaxLength { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean IsUnique { get; set; }

        [PetaPoco.Column(Length = 255, PersistEnumAsString = true, IsNullable = false)]
		public AttributeEncryptionType EncryptionType { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean VisibleInGrid { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean CanBeUpdated { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean IsSortable { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean IsMigrationText { get; set; }

        [PetaPoco.Column(Length = 1024)]
		public string DefaultValues { get; set; }

        [PetaPoco.Column(Length = 50)]
		public String DisplayText { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string Description { get; set; }

        [PetaPoco.Column(Length = 50)]
        public string Alias { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

        [PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
	}
}