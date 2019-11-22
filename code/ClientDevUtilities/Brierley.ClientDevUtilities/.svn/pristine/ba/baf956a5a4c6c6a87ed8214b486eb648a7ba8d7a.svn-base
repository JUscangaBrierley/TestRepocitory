using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Messaging.Config;

namespace Brierley.ClientDevUtilities.LWGateway.Messaging
{
    public interface IMessagingBus
    {
        bool CanSend(Type messageType);
        bool HasInstance();
        bool IsMessagingEnabled();
        void Send<TMessage>(TMessage msg);
    }
}
