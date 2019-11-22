using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.bScript;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_BScript")]
    [AuditLog(true)]
	public class Bscript : LWCoreObjectBase
	{
		[PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		[PetaPoco.Column(Length = 100, IsNullable = false)]
		public string Name { get; set; }

        [PetaPoco.Column(Length = 512)]
		public string Description { get; set; }

        [PetaPoco.Column]
        [ColumnIndex]
		public long? FolderId { get; set; }

        [PetaPoco.Column]
		public string Expression { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public ExpressionContexts ExpressionContext { get; set; }

        [PetaPoco.Column(Length=50)]
		public string CurrentConditionAttributeSet { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public bool IsLocked { get; set; }

		public Bscript Clone()
		{
			return Clone(new Bscript());
		}

		public Bscript Clone(Bscript dest)
		{
			dest.Name = Name;
			dest.Description = Description;
			dest.FolderId = FolderId;
			dest.Expression = Expression;
			dest.ExpressionContext = ExpressionContext;
			dest.CurrentConditionAttributeSet = CurrentConditionAttributeSet;
			dest.IsLocked = IsLocked;
			return (Bscript)base.Clone(dest);
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			Bscript_AL ar = new Bscript_AL()
			{
				ObjectId = this.Id,
				Name = this.Name,
				Description = this.Description,
				FolderId = this.FolderId,
				Expression = this.Expression,
				ExpressionContext = this.ExpressionContext,
				CurrentConditionAttributeSet = this.CurrentConditionAttributeSet,
				IsLocked = this.IsLocked,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate,
			};
			return ar;
		}
	}
}
