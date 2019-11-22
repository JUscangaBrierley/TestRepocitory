using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;

namespace Brierley.FrameWork.Messaging.Contracts
{
    public interface IMessage
    {
        string Id { get; }
		Message TransportMessage { get; }
		Exception Exception { get; set; }
    }
}
