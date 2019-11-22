using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
    /// <summary>
    /// POCO for TierDef. 
    /// </summary>
    [Serializable]
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("TierId", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_Tiers")]
    [AuditLog(true)]
    public class TierDef : ContentDefBase
    {
        /// <summary>
        /// Gets or sets the Id for the current TierDef
        /// </summary>
        [PetaPoco.Column("TierId", IsNullable = false)]
        public new long Id
        {
            get
            {
                return base.Id;
            }
            set
            {
                base.Id = value;
            }
        }

        /// <summary>
        /// Gets or sets the Name for the current TierDef
        /// </summary>
        [PetaPoco.Column("TierName", Length = 50, IsNullable = false)]
        [UniqueIndex(RequiresLowerFunction = false)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the DisplayText for the current TierDef
        /// </summary>
        [PetaPoco.Column(Length = 100)]
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the Description for the current TierDef
        /// </summary>
        [PetaPoco.Column(Length = 500)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the EntryPoints for the current TierDef
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public decimal EntryPoints { get; set; }

        /// <summary>
        /// Gets or sets the ExitPoints for the current TierDef
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public decimal ExitPoints { get; set; }

        /// <summary>
        /// Gets or sets the PointTypeNames for the current TierDef
        /// </summary>
        [PetaPoco.Column(Length = 2000)]
        public string PointTypeNames { get; set; }

        /// <summary>
        /// Gets or sets the PointEventNames for the current TierDef
        /// </summary>
        [PetaPoco.Column(Length = 2000)]
        public string PointEventNames { get; set; }

        /// <summary>
        /// Gets or sets the ExpirationDateExpression for the current TierDef
        /// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public bool AddToEnrollmentDate { get; set; }

        /// <summary>
        /// Gets or sets the ExpirationDateExpression for the current TierDef
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string ExpirationDateExpression { get; set; }

        /// <summary>
        /// Gets or sets the ActivityPeriodStartExpression for the current TierDef
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string ActivityPeriodStartExpression { get; set; }

        /// <summary>
        /// Gets or sets the ActivityPeriodEndExpression for the current TierDef
        /// </summary>
        [PetaPoco.Column(Length = 255)]
        public string ActivityPeriodEndExpression { get; set; }

        /// <summary>
        /// Gets or sets the Id of the tier's default reward choice.
        /// </summary>
        [PetaPoco.Column]
        [ForeignKey(typeof(RewardDef), "Id", ForeignKeyName = "FK_Tier_RewardId")]
        public long? DefaultRewardId { get; set; }

        /// <summary>
        /// Initializes a new instance of the TierDef class
        /// </summary>
        public TierDef() : base(ContentObjType.Tier)
        {
            Id = -1;
            Name = string.Empty;
            Description = string.Empty;
            PointTypeNames = string.Empty;
        }

        /// <summary>
        /// Minimal constructor for class TierDef
        /// </summary>
        /// <param name="name">Initial <see cref="TierDef.Name" /> value</param>
        /// <param name="UpdateDate">Initial <see cref="TierDef.Last_DML_Date" /> value</param>
        public TierDef(String name, DateTime updateDate) : base(ContentObjType.Tier)
        {
            Name = name;
            UpdateDate = updateDate;
        }

        public TierDef Clone()
        {
            return Clone(new TierDef());
        }

        public TierDef Clone(TierDef other)
        {
            other.Name = Name;
            other.DisplayText = DisplayText;
            other.Description = Description;
            other.EntryPoints = EntryPoints;
            other.ExitPoints = ExitPoints;
            other.PointTypeNames = PointTypeNames;
            other.PointEventNames = PointEventNames;
            other.AddToEnrollmentDate = AddToEnrollmentDate;
            other.ExpirationDateExpression = ExpirationDateExpression;
            other.ActivityPeriodStartExpression = ActivityPeriodStartExpression;
            other.ActivityPeriodEndExpression = ActivityPeriodEndExpression;
            other.DefaultRewardId = DefaultRewardId;
            return (TierDef)base.Clone(other);
        }

        public override LWObjectAuditLogBase GetArchiveObject()
        {
            TierDef_AL ar = new TierDef_AL()
            {
                ObjectId = this.Id,
                Name = this.Name,
                DisplayText = this.DisplayText,
                Description = this.Description,
                EntryPoints = this.EntryPoints,
                ExitPoints = this.ExitPoints,
                PointTypeNames = this.PointTypeNames,
                PointEventNames = this.PointEventNames,
                AddToEnrollmentDate = this.AddToEnrollmentDate,
                ExpirationDateExpression = this.ExpirationDateExpression,
                ActivityPeriodStartExpression = this.ActivityPeriodStartExpression,
                ActivityPeriodEndExpression = this.ActivityPeriodEndExpression,
                DefaultRewardId = this.DefaultRewardId,
                CreateDate = this.CreateDate,
                UpdateDate = this.UpdateDate
            };
            return ar;
        }

        public string[] GetPointTypes()
        {
            return !string.IsNullOrEmpty(PointTypeNames) ? PointTypeNames.Split(';') : null;
        }

        public string[] GetPointEvents()
        {
            return !string.IsNullOrEmpty(PointEventNames) ? PointEventNames.Split(';') : null;
        }

        public bool Qualifies(decimal points)
        {
            return EntryPoints <= points && points <= ExitPoints;
        }
    }
}