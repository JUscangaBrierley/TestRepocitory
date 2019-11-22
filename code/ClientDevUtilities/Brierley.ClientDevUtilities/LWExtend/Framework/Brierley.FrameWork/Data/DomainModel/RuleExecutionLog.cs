using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_RuleExecutionLog")]
    public class RuleExecutionLog : LWCoreObjectBase
    {
        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public string RuleName { get; set; }

        [PetaPoco.Column(IsNullable = false)]
        [ColumnIndex]
		public long MemberId { get; set; }

        [PetaPoco.Column(Length = 25, PersistEnumAsString = true, IsNullable = false)]
		public RuleExecutionStatus ExecutionStatus { get; set; }

        [PetaPoco.Column(Length = 25, PersistEnumAsString = true, IsNullable = false)]
		public RuleExecutionMode ExecutionMode { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public PointTransactionOwnerType OwnerType { get; set; }

        [PetaPoco.Column]
		public Int64? OwnerId { get; set; }

        [PetaPoco.Column]
		public Int64? RowKey { get; set; }

        [PetaPoco.Column(Length = 50)]
		public string SkipReason { get; set; }

        [PetaPoco.Column(Length = 150)]
		public string Detail { get; set; }
    }
}
