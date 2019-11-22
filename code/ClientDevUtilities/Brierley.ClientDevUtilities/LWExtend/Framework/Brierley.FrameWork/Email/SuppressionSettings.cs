using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Email
{
	public class SuppressionSettings
	{
		public class SuppressionRule
		{
			public EmailBounceRuleType Type { get; set; }
			public int Limit { get; set; }
			public int Interval { get; set; }

			public SuppressionRule(EmailBounceRuleType type, int limit, int interval)
			{
				Type = type;
				Limit = limit;
				Interval = interval;
			}

			public SuppressionRule(EmailBounceRuleType type)
			{
				Type = type;
			}

			public SuppressionRule()
			{
				Type = EmailBounceRuleType.None;
			}
		}

		public SuppressionRule PermanentBounceRule { get; set; }

		public SuppressionRule TransientBounceRule { get; set; }

		public SuppressionRule ComplaintBounceRule { get; set; }

		public SuppressionRule UndeterminedBounceRule { get; set; }
	}
}
