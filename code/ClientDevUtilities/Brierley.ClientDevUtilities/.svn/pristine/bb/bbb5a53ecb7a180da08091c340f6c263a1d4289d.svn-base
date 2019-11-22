using System;
using Amazon.SQS.Model;
using Brierley.FrameWork.Messaging.Contracts;

namespace Brierley.FrameWork.Messaging.Sqs
{
    public class SqsMessage : IMessage
    {
        private Message _message;

        public SqsMessage(Message message)
        {
            _message = message;
        }

        public string Id
        {
            get
            {
                return _message != null ? _message.MessageId : string.Empty;

            }
        }

        public System.Messaging.Message TransportMessage { get; private set; }

        public Exception Exception { get; set; }
    }
}
