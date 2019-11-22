using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Messaging.Config;

namespace Brierley.ClientDevUtilities.LWGateway.Messaging
{
    public class MessagingBus : IMessagingBus
    {
        private Brierley.FrameWork.Messaging.MessagingBus _bus;

        public MessagingBus()
        {
            _bus = Brierley.FrameWork.Messaging.MessagingBus.Instance();
        }

        public static MessagingBus Instance { get; }

        static MessagingBus()
        {
            Instance = new MessagingBus();
        }

        public bool CanSend(Type messageType)
        {
            return Brierley.FrameWork.Messaging.MessagingBus.CanSend(messageType);
        }

        public bool HasInstance()
        {
            return Brierley.FrameWork.Messaging.MessagingBus.HasInstance();
        }

        public bool IsMessagingEnabled()
        {
            return Brierley.FrameWork.Messaging.MessagingBus.IsMessagingEnabled();
        }

        public void Send<TMessage>(TMessage msg)
        {
            _bus.Send<TMessage>(msg);
        }
    }
}
