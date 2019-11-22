using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Messaging.Contracts;

namespace Brierley.FrameWork.Messaging.Msmq
{
    public class MsmqTransportFactory : ITransportProviderFactory
    {
        public ITransportProvider CreateMessageProvider()
        {
            return new MsmqTransportProvider();
        }
    }
}
