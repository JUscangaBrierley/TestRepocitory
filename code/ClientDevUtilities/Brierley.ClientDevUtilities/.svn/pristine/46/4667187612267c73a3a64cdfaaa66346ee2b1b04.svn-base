using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.bScript;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_BScript")]
	public class Bscript_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }

        [PetaPoco.Column(Length = 100, IsNullable = false)]
		public string Name { get; set; }

        [PetaPoco.Column(Length = 512)]
		public string Description { get; set; }

        [PetaPoco.Column]
		public long? FolderId { get; set; }

        [PetaPoco.Column]
		public string Expression { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public ExpressionContexts ExpressionContext { get; set; }

        [PetaPoco.Column(Length = 50)]
		public string CurrentConditionAttributeSet { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public bool IsLocked { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

        [PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
	}
}
