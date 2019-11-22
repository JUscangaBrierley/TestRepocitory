using System;
using System.Collections.Generic;
using System.Linq;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_RewardsDef")]
    public class RewardDef_AL : LWObjectAuditLogBase
	{
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }

        [PetaPoco.Column(Length = 50)]
		public string CertificateTypeCode { get; set; }

        [PetaPoco.Column(Length = 300, IsNullable = false)]
		public string Name { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public decimal HowManyPointsToEarn { get; set; }

        [PetaPoco.Column(Length = 2000)]
		public string PointType { get; set; }

        [PetaPoco.Column(Length = 2000)]
		public string PointEvent { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public long ProductId { get; set; }

		[PetaPoco.Column]
		public long? TierId { get; set; }

		[PetaPoco.Column("Threshhold", IsNullable = false)]
		public long Threshold { get; set; }

        [PetaPoco.Column(Length = 250)]
		public string SmallImageFile { get; set; }

        [PetaPoco.Column(Length = 250)]
		public string LargeImageFile { get; set; }

		[PetaPoco.Column]
		public DateTime? CatalogStartDate { get; set; }

		[PetaPoco.Column]
		public DateTime? CatalogEndDate { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public bool Active { get; set; }

		[PetaPoco.Column]
		public long? FulfillmentProviderId { get; set; }

        [PetaPoco.Column(Length = 256)]
		public string MediumImageFile { get; set; }

        [PetaPoco.Column]
        public long? RedeemTimeLimit { get; set; }

        [PetaPoco.Column]
        public long? PushNotificationId { get; set; }

        [PetaPoco.Column(PersistEnumAsString = false, IsNullable = false)]
        public RewardType RewardType { get; set; }

        [PetaPoco.Column]
        public decimal? ConversionRate { get; set; }

        [PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
    }
}