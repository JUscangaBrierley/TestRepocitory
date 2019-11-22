using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Messaging.Contracts;

namespace Brierley.FrameWork.Messaging.Sqs
{
    public class SqsTransportFactory : ITransportProviderFactory
    {
        public ITransportProvider CreateMessageProvider()
        {
            return new SqsTransportProvider();
        }
    }
}
