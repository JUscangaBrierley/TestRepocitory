using System;

namespace Brierley.FrameWork.Data.DomainModel
{	
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_ClientConfiguration")]
    public class ClientConfiguration_AL : LWObjectAuditLogBase
	{
		[PetaPoco.Column("ObjectId", Length = 100, IsNullable = false)]
		public virtual String Key { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public virtual String Value { get; set; }

        [PetaPoco.Column]
        public virtual long? FolderId { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public virtual Boolean ExternalValue { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        public virtual DateTime CreateDate { get; set; }

        [PetaPoco.Column]
        public virtual DateTime? UpdateDate { get; set; }
	}
}