using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Data.ModelAttributes;

namespace Brierley.FrameWork.Data.DomainModel
{
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberCouponRedemption")]
	public class MemberCouponRedemption : LWCoreObjectBase
	{
		[PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		[ColumnIndex]
		public long MemberCouponId { get; set; }

		[PetaPoco.Column(IsNullable = false)]
		public DateTime DateRedeemed { get; set; }

		[PetaPoco.Column]
		public DateTime? DateUnredeemed { get; set; }

		/// <summary>
		/// Gets or sets the violation that occurred during redemption. 
		/// </summary>
		/// <remarks>
		/// Typically, a coupon redemption is not allowed if any redemption rules are violated (e.g., too many redemptions in one day). A
		/// redemption that is denied for any reason would result in the history record not being written to LW_MemberCouponRedemption. 
		/// However, a parameter in the framework redemption method allows the redemption record to be written even if a redemption would 
		/// normally not be allowed. This helps in situations where the redemption does not happen in real time and instead comes later 
		/// (member redeems coupon at POS, but we don't know until the transaction file comes in the next day). The redemption cannot be 
		/// denied at this point because it has already happened. We'll write the redemption to the history table and populate the violation 
		/// column to indicate why this redemption would not have been allowed otherwise.
		/// </remarks>
		[PetaPoco.Column]
		public CouponRedemptionViolation? Violation { get; set; }
	}
}
