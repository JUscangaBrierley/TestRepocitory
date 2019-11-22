using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Messaging.Messages
{
	public class SmsMessage
	{
		public long SmsId { get; set; }
		public string MobileNumber { get; set; }
		public Dictionary<string, string> Personalizations { get; set; }
	
		
		public SmsMessage()
		{
		}

		public SmsMessage(long smsId)
		{
			SmsId = smsId;
		}

		public SmsMessage(long smsId, string mobileNumber)
			: this(smsId)
		{
			MobileNumber = mobileNumber;
		}

		public SmsMessage(long smsId, string mobileNumber, Dictionary<string, string> personalizations)
			: this(smsId, mobileNumber)
		{
			Personalizations = personalizations;
		}
	}
}
