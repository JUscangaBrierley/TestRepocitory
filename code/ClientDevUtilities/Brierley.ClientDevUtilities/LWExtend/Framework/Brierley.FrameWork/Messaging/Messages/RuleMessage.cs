using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Messaging.Messages
{
	public class RuleMessage
	{
		public long MemberId { get; set; }
		public long VCKey { get; set; }
		public RuleInvocationType InvocationType { get; set; }

		//rules can be invoked by either an event...
		public string EventName { get; set; }

		//...or an attribute set row
		public string AttributeSetName { get; set; }
		public long AttributeSetId { get; set; }
		public long InvokingRow { get; set; }

		public Dictionary<string, object> ContextEnvironment { get; set; }

		public RuleMessage()
		{
		}

		public RuleMessage(long memberId, RuleInvocationType invocationType)
		{
			MemberId = memberId;
			InvocationType = invocationType;
		}

		public RuleMessage(long memberId, RuleInvocationType invocationType, string eventName) : this(memberId, invocationType)
		{
			if (string.IsNullOrEmpty(eventName))
			{
				throw new ArgumentNullException("eventName");
			}
			if (invocationType != RuleInvocationType.AfterInsert && invocationType != RuleInvocationType.AfterUpdate && invocationType != RuleInvocationType.Manual)
			{
				throw new ArgumentException("Invalid invocationType. Valid values are RuleInvocationType.AfterInsert, RuleInvocationType.AfterUpdate or RuleInvocationType.Manual");
			}
			EventName = eventName;
		}

		public RuleMessage(long memberId, RuleInvocationType invocationType, string attributeSetName, long invokingRow, Dictionary<string, object> contextEnvironment = null) : this(memberId, invocationType)
		{
			AttributeSetName = attributeSetName;
			InvokingRow = invokingRow;
			ContextEnvironment = contextEnvironment;
		}

		public RuleMessage(long memberId, RuleInvocationType invocationType, long attributeSetId, long invokingRow, Dictionary<string, object> contextEnvironment = null) : this(memberId, invocationType)
		{
			AttributeSetId = attributeSetId;
			InvokingRow = invokingRow;
			ContextEnvironment = contextEnvironment;
		}
	}
}
