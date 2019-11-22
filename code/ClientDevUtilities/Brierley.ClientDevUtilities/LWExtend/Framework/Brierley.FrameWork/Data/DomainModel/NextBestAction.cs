using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
	/// <summary>
	/// Next best action represents a list of actions for a member in priority order. 
	/// </summary>
	/// <remarks>
	/// The action may be one of a number of LoyaltyWare objects: coupon, bonus, message, 
	/// product, etc.. A real-time campaign may pull one or more next best actions and 
	/// assign them to a member (<see cref="Brierley.FrameWork.Data.DomainModel.MemberNextBestAction"/>), 
	/// which may, depending on the type of action, write a record to the corresponding member
	/// table (e.g., MemberCoupon, MemberMessage, etc.).
	/// </remarks>
	[PetaPoco.ExplicitColumns]
	[PetaPoco.PrimaryKey("MemberId, Priority", autoIncrement = false)]
	[PetaPoco.TableName("LW_NextBestAction")]
	public class NextBestAction : LWCoreObjectBase
	{
		/// <summary>
		/// Gets or sets the Id (IPCode) of the member the next best action has been chosen for.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public long MemberId { get; set; }

		/// <summary>
		/// Gets or sets the priority order of the next best action, relative to the MemberId.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public int Priority { get; set; }

		/// <summary>
		/// Gets or sets the action type of the next best action.
		/// </summary>
        [PetaPoco.Column(IsNullable = false)]
		public NextBestActionType ActionType { get; set; }

		/// <summary>
		/// Gets or sets the Id of the next best action.
		/// </summary>
		/// <remarks>
		/// This is the Id of the object definition that may be assigned when the member receives 
		/// the action (e.g., the coupon definition id for coupon action types, message definition
		/// id for message action types, etc.)
		/// </remarks>
        [PetaPoco.Column(IsNullable = false)]
		public long ActionId { get; set; }

		public NextBestAction()
		{
			CreateDate = DateTime.Now;
		}

		public NextBestAction(long memberId, int priority, NextBestActionType actionType, long actionId)
			: this()
		{
			MemberId = memberId;
			Priority = priority;
			ActionType = actionType;
			ActionId = actionId;
		}

		public override bool Equals(object obj)
		{
			NextBestAction otherInstance = obj as NextBestAction;
			if (otherInstance != null)
			{
				return
					otherInstance.MemberId == MemberId &&
					otherInstance.Priority == Priority;
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
