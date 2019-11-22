using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

using Brierley.FrameWork.LWIntegration;

namespace Brierley.FrameWork.LWIntegration.Util
{
	public class EmailUtil
	{
		public static void SendEmail(LWIntegrationConfig.EmailInformation emailInfo, string subject, string emailMessage)
		{
			SmtpClient client = new SmtpClient(emailInfo.SmtpServer);
			MailAddress addrTo = new MailAddress(emailInfo.RecepientEmail);
			MailAddress addrFom = new MailAddress(emailInfo.SenderEmail, emailInfo.SenderDisplayName);
			MailMessage message = new MailMessage(addrFom, addrTo);
			//StringBuilder sb = new StringBuilder();
			//sb.Append("DAP processing finished for job " + job.JobNumber + "\n");
			//sb.Append("\tJob id                     : " + job.JobId + "\n");
			//sb.Append("\tFile name                  : " + job.FileName + "\n");
			//sb.Append("\tProcessing start time      : " + job.StartTime + "\n");
			//sb.Append("\tProcessing end time        : " + job.EndTime + "\n");
			//sb.Append("\tNumber of messages received: " + job.MessagesReceived + "\n");
			//sb.Append("\tNumber of messages failed  : " + job.MessagesFailed + "\n");
			//message.Body = sb.ToString();
			message.Body = emailMessage;
			message.Subject = subject;
			client.Send(message);
		}

	}
}
