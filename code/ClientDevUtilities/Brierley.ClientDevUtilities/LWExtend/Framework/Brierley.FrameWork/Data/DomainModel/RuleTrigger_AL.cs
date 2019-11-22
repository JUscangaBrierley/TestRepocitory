using System;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_RuleTriggers")]
    public class RuleTrigger_AL : LWObjectAuditLogBase
    {
        /// <summary>
        /// Initializes a new instance of the PointEvent class
        /// </summary>
        public RuleTrigger_AL()
        {
        }

        [PetaPoco.Column(IsNullable = false)]
		public Int64 ObjectId { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Int64 AttributeSetCode { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Int32 Sequence { get; set; }

        [PetaPoco.Column(Length = 30, IsNullable = false)]
		public String InvocationType { get; set; }

        [PetaPoco.Column]
		public String ConditionalExpression { get; set; }

        [PetaPoco.Column]
		public String RuleInstance { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean IsConfigured { get; set; }

        [PetaPoco.Column(Length = 50, IsNullable = false)]
		public String RuleName { get; set; }

        [PetaPoco.Column(Length = 150)]
		public string PromotionCode { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime EndDate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public bool Targeted { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean ContinueOnError { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean AlwaysRun { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public Boolean LogExecution { get; set; }

        [PetaPoco.Column(Length = 30)]
		public String OwningObject { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

        [PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
    }
}
