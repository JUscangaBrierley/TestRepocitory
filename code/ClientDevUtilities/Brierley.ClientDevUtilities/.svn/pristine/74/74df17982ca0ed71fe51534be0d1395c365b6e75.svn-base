using System;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [Serializable]
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Ar_ID", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_AL_MemberRewards")]
    public class MemberReward_AL : LWObjectAuditLogBase
    {
        /// <summary>
		/// Gets or sets the Id for the current MemberReward
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long ObjectId { get; set; }
		
		/// <summary>
		/// Gets or sets the RewardDefId for the current MemberReward
		/// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public long RewardDefId { get; set; }
		
		/// <summary>
		/// Gets or sets the CertificateNmbr for the current MemberReward
		/// </summary>
        [PetaPoco.Column(Length = 50)]
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

        [PetaPoco.Column]
        public decimal? PointsConsumed { get; set; }

        [PetaPoco.Column(Length = 3)]
        public string FromCurrency { get; set; }

        [PetaPoco.Column(Length = 3)]
        public string ToCurrency { get; set; }

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
		
        /// <summary>
        /// Gets or sets the CreateDate for the current LIBJob
        /// </summary>
		[PetaPoco.Column(IsNullable = false)]
		public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the UpdateDate for the current LIBJob
        /// </summary>
		[PetaPoco.Column]
		public DateTime? UpdateDate { get; set; }
		
		/// <summary>
		/// Initializes a new instance of the PointEvent class
		/// </summary>
		public MemberReward_AL()
		{
			FulfillmentOption = RewardFulfillmentOption.Electronic;
			ProductId = -1;
			ProductVariantId = -1;
			DateIssued = DateTime.Now;
		}
    }
}