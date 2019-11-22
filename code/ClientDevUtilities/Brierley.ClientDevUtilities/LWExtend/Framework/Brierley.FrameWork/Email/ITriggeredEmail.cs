using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Email
{
	public interface ITriggeredEmail : IDisposable
	{
		void Init(long id, LWConfiguration config = null);

		void Init(string name, LWConfiguration config = null);

		Task SendAsync(Member member);

		/// <summary>
		/// Sends a triggered email to a loyalty member.
		/// </summary>
		/// <param name="member">The loyalty member the email will be sent to.</param>
		/// <param name="additionalFields">Name/value pair of fields, which can be used to override the value of any field in the email.</param>
		Task SendAsync(Member member, Dictionary<string, string> additionalFields);

		/// <summary>
		/// Sends a triggered email.
		/// </summary>
		/// <remarks>
		///	This method can be used to send an email in the event that the email should not go to a loyalty 
		/// member (e.g., an alert which needs to be send to a customer service rep or administrator).
		/// </remarks>
		/// <param name="recipientEmail">The recipient's email address, if different from Member.PrimaryEmailAddress.</param>
		/// <param name="manualFields">Name/value pair of fields.</param>
		/// <param name="emailQueueID">The ID of the send record in the EmailQueue table.</param>
		/// <returns>Serial number of the mailing.</returns>
		Task SendAsync(string recipientEmail, Dictionary<string, string> manualFields);
	}
}
