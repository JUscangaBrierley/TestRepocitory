using System;
using System.Messaging;

namespace Brierley.FrameWork.Messaging.Contracts
{
	public interface IMessageSerializer
	{
		IMessage Serialize(string msgId, object message);
		object Deserialize(IMessage message, Type type);
	}
}
