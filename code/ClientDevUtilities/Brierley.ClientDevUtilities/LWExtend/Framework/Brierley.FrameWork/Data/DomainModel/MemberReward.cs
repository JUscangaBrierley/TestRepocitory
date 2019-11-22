using System;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// POCO for MemberReward. This class is autogenerated
	/// </summary>
	[Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberRewards")]
    [AuditLog(false)]
	public class MemberReward : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the Id for the current MemberReward
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the RewardDefId for the current MemberReward
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(RewardDef), "Id")]
		public long RewardDefId { get; set; }

		/// <summary>
		/// Gets or sets the CertificateNmbr for the current MemberReward
		/// </summary>
        [PetaPoco.Column(Length = 50)]
        [UniqueIndex]
		public string CertificateNmbr { get; set; }

		/// <summary>
		/// Gets or sets the OfferCode for the current MemberReward
		/// </summary>
        [PetaPoco.Column(Length = 50)]
		public string OfferCode { get; set; }

		/// <summary>
		/// Gets or sets the AvailableBalance for the current MemberReward
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public decimal AvailableBalance { get; set; }

		/// <summary>
		/// Gets or sets the FulfillmentOption for the current MemberReward
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public RewardFulfillmentOption FulfillmentOption { get; set; }

		/// <summary>
		/// Gets or sets the MemberId for the current MemberReward
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        [ForeignKey(typeof(Member), "IpCode")]
        [ColumnIndex]
		public long MemberId { get; set; }

		/// <summary>
		/// Gets or sets the ProductId for the current MemberReward
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long ProductId { get; set; }

		/// <summary>
		/// Gets or sets the ProductVariantId for the current MemberReward
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
        public long ProductVariantId { get; set; }

		/// <summary>
		/// Gets or sets the DateIssued for the current MemberReward
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public DateTime DateIssued { get; set; }

		/// <summary>
		/// Gets or sets the Expiration for the current MemberReward
		/// </summary>
        [PetaPoco.Column]
		public DateTime? Expiration { get; set; }

		/// <summary>
		/// Gets or sets the FulfillmentDate for the current MemberReward
		/// </summary>
        [PetaPoco.Column]
		public DateTime? FulfillmentDate { get; set; }

        [PetaPoco.Column]
		public long? FulfillmentProviderId { get; set; }

		/// <summary>
		/// Gets or sets the RedemptionDate for the current MemberReward
		/// </summary>
        [PetaPoco.Column]
		public DateTime? RedemptionDate { get; set; }

        [PetaPoco.Column(Length = 50)]
		public string ChangedBy { get; set; }

        [PetaPoco.Column(Length = 100)]
		public string LWOrderNumber { get; set; } // Order assigned by LoyaltyWare

        [PetaPoco.Column(Length = 100)]
		public string FPOrderNumber { get; set; } // Order assigned by Fulfillment Provider

        [PetaPoco.Column(Length = 100)]
		public string LWCancellationNumber { get; set; } // Cancellation number assigned by LoyaltyWare

        [PetaPoco.Column(Length = 100)]
		public string FPCancellationNumber { get; set; } // Cancellation number assigned by Fulfillment Provider

        [PetaPoco.Column(Length = 25, PersistEnumAsString = true)]
		public RewardOrderStatus? OrderStatus { get; set; }

        // LCAP properties
        [PetaPoco.Column]
        public decimal? PointsConsumed { get; set; }

        [PetaPoco.Column(Length = 3)]
        public string FromCurrency { get; set; }

        [PetaPoco.Column(Length = 3)]
        public string ToCurrency { get; set;}

        [PetaPoco.Column]
        public decimal? PointConversionRate { get; set; }

        [PetaPoco.Column]
        public decimal? ExchangeRate { get; set; }

        [PetaPoco.Column]
        public decimal? MonetaryValue { get; set; }

        [PetaPoco.Column]
        public decimal? CartTotalMonetaryValue { get; set; }

        public string TrackingNumber { get; set; }

		public string TrackingUrl { get; set; }

        public RewardDef RewardDef { get; set; }

		/// <summary>
		/// Initializes a new instance of the MemberReward class
		/// </summary>
		public MemberReward()
		{
			FulfillmentOption = RewardFulfillmentOption.Electronic;
			ProductId = -1;
			ProductVariantId = -1;
			DateIssued = DateTime.Now;
		}


		public override LWObjectAuditLogBase GetArchiveObject()
		{
			MemberReward_AL ar = new MemberReward_AL()
			{
				AvailableBalance = this.AvailableBalance,
				CertificateNmbr = this.CertificateNmbr,
				ChangedBy = this.ChangedBy,
				DateIssued = this.DateIssued,
				Expiration = this.Expiration,
				FPCancellationNumber = this.FPCancellationNumber,
				FPOrderNumber = this.FPOrderNumber,
				FulfillmentDate = this.FulfillmentDate,
				FulfillmentOption = this.FulfillmentOption,
				FulfillmentProviderId = this.FulfillmentProviderId,
				LWCancellationNumber = this.LWCancellationNumber,
				LWOrderNumber = this.LWOrderNumber,
				MemberId = this.MemberId,
				ObjectId = this.Id,
				OfferCode = this.OfferCode,
				OrderStatus = this.OrderStatus,
				ProductId = this.ProductId,
				ProductVariantId = this.ProductVariantId,
				RedemptionDate = this.RedemptionDate,
				RewardDefId = this.RewardDefId,
				TrackingNumber = this.TrackingNumber,
				TrackingUrl = this.TrackingUrl,
                PointsConsumed = this.PointsConsumed,
                FromCurrency = this.FromCurrency,
                ToCurrency = this.ToCurrency,
                PointConversionRate = this.PointConversionRate,
                ExchangeRate = this.ExchangeRate,
                MonetaryValue = this.MonetaryValue,
                CartTotalMonetaryValue = this.CartTotalMonetaryValue,
				CreateDate = this.CreateDate,
				UpdateDate = this.UpdateDate
			};
			return ar;
		}
	}
}