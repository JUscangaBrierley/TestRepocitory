using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for RuleTrigger.
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("RuleTriggerId", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_RuleTriggers")]
    [AuditLog(true)]
	public class RuleTrigger : LWCoreObjectBase
	{
		private Brierley.FrameWork.Rules.RuleBase rule;

		/// <summary>
		/// Initializes a new instance of the RuleTrigger class
		/// </summary>
		public RuleTrigger()
		{
			StartDate = DateTime.Now;
			EndDate = DateTimeUtil.MaxValue;
			Targeted = true;
			AlwaysRun = true;
		}

		/// <summary>
		/// Gets or sets the Id for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column("RuleTriggerId", IsNullable = false)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the AttributeSetCode for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long AttributeSetCode { get; set; }

		/// <summary>
		/// Gets or sets the Sequence for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public int Sequence { get; set; }

		/// <summary>
		/// Gets or sets the InvocationType for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(Length = 30, IsNullable = false)]
		public string InvocationType { get; set; }



		//todo: LW_RuleTriggers stores invocation type as a string. RuleTrigger also exposes it as a string (above), which it should not 
		//do - it should be the enumeration. The property needs to be converted, which may be a breaking change. For now, a second property
		//to make rule processing easier (it's currently being converted manually all over the place):
		private RuleInvocationType? _invocationType = null;
		public RuleInvocationType ProperInvocationType
		{
			get
			{
				if (_invocationType == null)
				{
					_invocationType = (RuleInvocationType)Enum.Parse(typeof(RuleInvocationType), InvocationType);
				}
				return _invocationType.GetValueOrDefault();
			}
		}



		/// <summary>
		/// Gets or sets the ConditionalExpression for the current RuleTrigger
		/// </summary>
		[PetaPoco.Column]
		public string ConditionalExpression { get; set; }

		/// <summary>
		/// Gets or sets the RuleInstance for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column]
		public string RuleInstance { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public bool CanQueue { get; set; }

		public bool IsActive
		{
			get
			{
				return DateTimeUtil.IsDateInBetween(DateTime.Now, (DateTime)this.StartDate, (DateTime)this.EndDate, true);
			}
		}

		/// <summary>
		/// Gets or sets the IsConfigured for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool IsConfigured { get; set; }

		/// <summary>
		/// Gets or sets the RuleName for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string RuleName { get; set; }

		/// <summary>
		/// Gets or sets the PromotionCode for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(Length = 150)]
		public string PromotionCode { get; set; }

		/// <summary>
		/// Gets or sets the StartDate for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DateTime StartDate { get; set; }

		/// <summary>
		/// Gets or sets the EndDate for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DateTime EndDate { get; set; }

		/// <summary>
		/// Gets or sets the Targeted for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool Targeted { get; set; }

		/// <summary>
		/// Gets or sets the ContinueOnError for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public bool ContinueOnError { get; set; }

		/// <summary>
		/// Gets or sets the AlwaysRun for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public bool AlwaysRun { get; set; }

		/// <summary>
		/// Gets or sets the LogExecution for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public bool LogExecution { get; set; }

		/// <summary>
		/// Gets or sets the OwningObject for the current RuleTrigger
		/// </summary>
        [PetaPoco.Column(Length = 30)]
		public string OwningObject { get; set; }

		[System.Xml.Serialization.XmlIgnore]
		public Brierley.FrameWork.Rules.RuleBase Rule { get; set; }

		public RuleTrigger Clone()
		{
			return Clone(new RuleTrigger());
		}

		public RuleTrigger Clone(RuleTrigger dest)
		{
			dest.AlwaysRun = AlwaysRun;
			dest.AttributeSetCode = AttributeSetCode;
            dest.CanQueue = CanQueue;
			dest.ConditionalExpression = ConditionalExpression;
			dest.ContinueOnError = ContinueOnError;
			dest.EndDate = EndDate;
			dest.InvocationType = InvocationType;
			dest.IsConfigured = IsConfigured;
			dest.Targeted = Targeted;
			dest.LogExecution = LogExecution;
			dest.OwningObject = OwningObject;
			dest.RuleInstance = RuleInstance;
			dest.RuleName = RuleName;
			dest.PromotionCode = PromotionCode;
			dest.Sequence = Sequence;
			dest.StartDate = StartDate;
			return (RuleTrigger)base.Clone(dest);
		}

		public override LWObjectAuditLogBase GetArchiveObject()
		{
			RuleTrigger_AL ar = new RuleTrigger_AL()
			{
				ObjectId = this.Id,
				AttributeSetCode = this.AttributeSetCode,
				Sequence = this.Sequence,
				InvocationType = this.InvocationType,
				ConditionalExpression = this.ConditionalExpression,
				RuleInstance = this.RuleInstance,
				IsConfigured = this.IsConfigured,
				RuleName = this.RuleName,
				PromotionCode = this.PromotionCode,
				StartDate = this.StartDate,
				EndDate = this.EndDate,
				Targeted = this.Targeted,
				ContinueOnError = this.ContinueOnError,
				AlwaysRun = this.AlwaysRun,
				LogExecution = this.LogExecution,
				OwningObject = this.OwningObject,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}