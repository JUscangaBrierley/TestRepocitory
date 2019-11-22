using System;

namespace Brierley.FrameWork.Messaging.Contracts
{
    public interface ITransportProviderFactory
    {
        ITransportProvider CreateMessageProvider();
    }
}
