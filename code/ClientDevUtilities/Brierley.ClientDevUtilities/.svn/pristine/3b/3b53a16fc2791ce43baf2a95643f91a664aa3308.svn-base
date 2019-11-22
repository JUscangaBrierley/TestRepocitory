using System;
using System.Messaging;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Msmq;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Messaging
{
    public class JsonMessageSerializer : IMessageSerializer
    {
        private const string _className = "JSONMessageSerializer";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        public IMessage Serialize(string msgId, object msg)
        {
            const string methodName = "Serialize";
            Message msmqMessage;
            try
            {
                string msgJSON = JsonConvert.SerializeObject(msg);
                msmqMessage = new Message(msgJSON) { Label = "Message_" + msgId };
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, string.Format("Error serializing message."), ex);
                throw;
            }
            return msmqMessage != null ? new MsmqMessage(msmqMessage) : null;
        }

        public object Deserialize(IMessage message, Type type)
        {
            const string methodName = "Deserialize";
            object msg = null;
            try
            {
                Message msmqMessage = message.TransportMessage as Message;
				if (msmqMessage == null)
				{
					throw new Exception("Failed to deserialize message. msmqMessage is null.");
				}
                msg = JsonConvert.DeserializeObject(msmqMessage.Body.ToString(), type);                
            }
            catch (Exception ex)
            {
                _logger.Error(_className, methodName, "Error deserializing message.", ex);
                throw;
            }
            return msg;
        }
    }
}
