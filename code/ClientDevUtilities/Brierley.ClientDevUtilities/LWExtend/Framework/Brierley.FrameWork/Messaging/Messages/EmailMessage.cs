using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Messaging.Messages
{
	public class EmailMessage
	{
		public long EmailId { get; set; }
		public string EmailName { get; set; }
		public string RecipientEmail { get; set; }
		public Dictionary<string, string> Personalizations { get; set; }

		public EmailMessage()
		{
		}

		public EmailMessage(long emailId)
		{
			EmailId = emailId;
		}

		public EmailMessage(long emailId, string recipientEmail)
			: this(emailId)
		{
			RecipientEmail = recipientEmail;
		}

		public EmailMessage(long emailId, string recipientEmail, Dictionary<string, string> personalizations)
			: this(emailId, recipientEmail)
		{
			Personalizations = personalizations;
		}
	}
}
