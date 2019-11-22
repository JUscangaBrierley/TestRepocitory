using System;
using System.Collections.Generic;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for AttributeSet.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_AttributeSet")]
    public class AttributeSetMetaData_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public Int64 ObjectId { get; set; }

        [PetaPoco.Column("AttributeSetName", Length = 50, IsNullable = false)]
		public String Name { get; set; }

        [PetaPoco.Column(Length = 150, IsNullable = false)]
		public String DisplayText { get; set; }

        [PetaPoco.Column(Length = 255)]
		public string Description { get; set; }

        [PetaPoco.Column(Length = 50)]
        public string Alias { get; set; }

        [PetaPoco.Column("TypeCode", IsNullable = false)]
		public AttributeSetType Type { get; set; }

        [PetaPoco.Column("CategoryCode", IsNullable = false)]
		public AttributeSetCategory Category { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean EditableFromProgram { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean EditableFromCampaign { get; set; }

        [PetaPoco.Column("ParentAttributeSetCode", IsNullable = false)]
        public Int64 ParentID { get; set; }

        [PetaPoco.Column]
		public DateTime? TableCreationDate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

        [PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }      
    }
}