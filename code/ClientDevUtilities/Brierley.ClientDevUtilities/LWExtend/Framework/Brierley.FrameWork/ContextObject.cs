using System;
using System.Collections.Generic;
using System.Text;

using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork
{
	/// <summary>
	/// The Context object is used as a general container to pass data out of the framework to a rule object.
	/// </summary>
	public class ContextObject
	{
		private Dictionary<string, PromotionContext> _promotionContexts = null;

		internal Dictionary<string, object> ResultCache { get; set; }

		public class RuleResult
		{
			public enum RuleSkippedReason
			{
				RuleTargetedAndNoMember,
				PromotionNotValid,
				MemberNotInPromotion,
				MemberNotEnrolledInPromotion, 
				RuleNotInDateRange,
				RuleExpressionNotMet,
				RuleExpressionException,
				RuleInstanceNull,
				RuleExecutionError
			}

			// name of the rule executed.
			public string Name;
			public RuleExecutionMode Mode;
			public Type RuleType;
			public long MemberId { get; set; }
			public long ResultCode = 0;
			public RuleSkippedReason? SkipReason { get; set; }
			public PointTransactionOwnerType? OwnerType { get; set; }
			public long? OwnerId { get; set; }
			public long? RowKey { get; set; }
			public string Detail { get; set; }
			public long ExecutionLogId { get; set; }

			public RuleResult()
			{
			}

			public RuleResult(long memberId, string name, Type ruleType, RuleSkippedReason? skipReason = null)
			{
				MemberId = memberId;
				Name = name;
				RuleType = ruleType;
				SkipReason = skipReason;
			}
		}

		public class PromotionContext
		{
			public bool Targeted { get; set; }
			public PromotionEnrollmentSupportType EnrollmentType { get; set; }
			public bool Enrolled { get; set; }

			public PromotionContext(bool targeted = false, PromotionEnrollmentSupportType enrollmentType = PromotionEnrollmentSupportType.None, bool enrolled = false)
			{
				Targeted = targeted;
				EnrollmentType = enrollmentType;
				Enrolled = enrolled;
			}
		}

		public List<RuleResult> Results = new List<RuleResult>();

		/// <summary>
		/// Name of the rule being executed.
		/// </summary>
		public string Name;

		/// <summary>
		/// This is the mode of execution for the rule.  The default value is "Real".
		/// </summary>
		public RuleExecutionMode Mode;

		/// <summary>
		/// The owner of the attribute set that is invoking the rule. Will either be an instance of <see cref="Member"/>
		/// or and instance of <see cref="VirtualCard"/>
		/// </summary>
		public IAttributeSetContainer Owner = null;

		/// <summary>
		/// The <see cref="IClientDataObject"/> of data that has invoked the rule.
		/// </summary>
		public IClientDataObject InvokingRow = null;

		/// <summary>
		/// This is a general purpose environment object that can be used for extensions.
		/// </summary>
		public Dictionary<string, object> Environment = new Dictionary<string, object>();

		/// <summary>
		/// This attribute can contain the document template that me be needed by some functions.
		/// </summary>
		public object Template = null;

		/// <summary>
		/// Thsi is starting index for a looping function.
		/// </summary>
		public int StartIndex = 0;

		/// <summary>
		/// This is the ending index for a looping function.
		/// </summary>
		public int EndIndex = 0;

		/// <summary>
		/// This is the current index of a loop.
		/// </summary>
		public int CurrentIndex = 0;

		/// <summary>
		/// Contains a list of promotion codes that have been determined to be valid under the current context.
		/// </summary>
		public Dictionary<string, PromotionContext> PromotionContexts
		{
			get
			{
				if (_promotionContexts == null)
				{
					_promotionContexts = new Dictionary<string, PromotionContext>();
				}
				return _promotionContexts;
			}
		}

		/// <summary>
		/// default constructor
		/// </summary>
		public ContextObject()
		{
			Mode = RuleExecutionMode.Real;
		}

		public Member ResolveMember()
		{
			Member ret = null;
			IAttributeSetContainer owner = Owner;

			while (owner != null && owner.Parent != null)
			{
				owner = owner.Parent;
			}

			if (owner != null)
			{
				if (owner is Member)
				{
					ret = (Member)owner;
				}
				else if (owner is VirtualCard)
				{
					ret = ((VirtualCard)owner).Member;
				}
			}
			return ret;
		}
	}
}
