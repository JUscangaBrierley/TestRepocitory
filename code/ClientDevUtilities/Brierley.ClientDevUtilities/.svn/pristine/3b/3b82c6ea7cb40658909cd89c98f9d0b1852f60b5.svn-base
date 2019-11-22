using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Represents a Next Best Action that was assigned to a member.
	/// </summary>
	/// <remarks>
	/// Where NextBestAction represents a list of potential actions for a member, 
	/// that list contains actions that have not necessarily been assigned. Once
	/// assigned to a member, the record is contained here.
	/// 
	/// NextBestAction's underlying table may be truncated as new data replaces
	/// stale offers. This serves as a historical account of what was taken from
	/// that table and given to a member, along with the Id of the LoyaltyWare
	/// object (MemberCoupon, MemberMessage, etc.).
	/// </remarks>
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
	[PetaPoco.TableName("LW_MemberNextBestAction")]
	public class MemberNextBestAction : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the Id of the member next best action.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the member Id (IPCode) of the member next best action.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long MemberId { get; set; }

		/// <summary>
		/// Gets or sets the priority order of the member next best action.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public int Priority { get; set; }

		/// <summary>
		/// Gets or sets the action type of the member next best action.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public NextBestActionType ActionType { get; set; }

		/// <summary>
		/// Gets or sets the action Id of the member next best action.
		/// </summary>
		/// <remarks>
		/// This is the Id of the object definition that may be assigned when the member receives 
		/// the action (e.g., the coupon definition id for coupon action types, message definition
		/// id for message action types)
		/// </remarks>
        [PetaPoco.Column(IsNullable = false)]
		public long ActionId { get; set; }

		/// <summary>
		/// Gets or sets the member action Id of the member next best action.
		/// </summary>
		/// <remarks>
		/// This is the Id of the member object that is assigned when the member receives the 
		/// action (e.g., the Id of the MemberCoupon for coupon action types, MemberMessage Id 
		/// for message action types, etc.).
		/// </remarks>
        [PetaPoco.Column]
		public long? MemberActionId { get; set; }

		/// <summary>
		/// Gets or sets the number of times the action has been viewed.
		/// </summary>
		/// <remarks>
		/// This value is only used for next best actions that are not "assigned" to a member. 
		/// One example is a recommendation engine (ActionType.Sku) showing a list of products 
		/// the member may be interested in. These products are not assigned to the member
		/// the same way coupons are and there is no ability to track the usage. Each time the 
		/// member views the list of recommendations, the Views property will be incremented 
		/// to show the total number of views for the recommendation.
		/// </remarks>
        [PetaPoco.Column]
		public int? Views { get; set; }

		public MemberNextBestAction()
		{
			CreateDate = DateTime.Now;
		}

		public MemberNextBestAction(long memberId, int priority, NextBestActionType actionType, long actionId, long memberActionId) : this()
		{
			MemberId = memberId;
			ActionType = actionType;
			ActionId = actionId;
			Priority = priority;
			MemberActionId = memberActionId;
		}
	}
}
