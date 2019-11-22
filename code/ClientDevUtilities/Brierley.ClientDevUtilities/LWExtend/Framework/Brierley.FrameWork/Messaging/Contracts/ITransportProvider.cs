using System;
using System.Threading.Tasks;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Messaging.Config;

namespace Brierley.FrameWork.Messaging.Contracts
{
	public interface ITransportProvider
	{
        IMessageSerializer Serializer { get; }

		EndPointCfg InitializeQueue(EndPointCfg cfg);
		void MoveToSubQueue(object queue, string subQueueName, IMessage message, EndPointCfg cfg);
		void SendMessage(EndPointCfg endpointCfg, object messageContent, long msgId);
		IMessage ReceiveAndConsume(EndPointCfg epCfg);
        MessageListener GetListener(LWConfigurationContext ctx, EndPointCfg endpoint, BusConfiguration config);
    }
}
