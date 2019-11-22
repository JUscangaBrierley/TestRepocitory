using System;
using System.Messaging;

using Brierley.FrameWork.Messaging.Contracts;

namespace Brierley.FrameWork.Messaging.Msmq
{
	public class MsmqMessage : IMessage
	{
		public MsmqMessage(Message message)
		{
			TransportMessage = message;
		}

		public string Id
		{
			get
			{
				return TransportMessage != null ? TransportMessage.Id : string.Empty;
			}
		}

		public Message TransportMessage { get; private set; }

		public Exception Exception { get; set; }
	}
}
