using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_ContentAttribute")]
	public class ContentAttribute_AL : LWObjectAuditLogBase
	{
		/// <summary>
		/// Gets or sets the ID for the current ContentAttribute
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public virtual long ObjectId { get; set; }

		/// <summary>
		/// Gets or sets the ContentType for the current ContentAttribute
		/// </summary>
        [PetaPoco.Column(Length = 25, PersistEnumAsString = true, IsNullable = false)]
		public virtual ContentObjType ContentType { get; set; }

		/// <summary>
		/// Gets or sets the RefId for the current ContentAttribute
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public virtual long RefId { get; set; }

		/// <summary>
		/// Gets or sets the ContentAttributeDefId for the current ContentAttribute
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public virtual long ContentAttributeDefId { get; set; }

		/// <summary>
		/// Gets or sets the Value for the current ContentAttribute
		/// </summary>
        [PetaPoco.Column("AttributeValue", Length = 2000)]
		public virtual string Value { get; set; }
	}
}