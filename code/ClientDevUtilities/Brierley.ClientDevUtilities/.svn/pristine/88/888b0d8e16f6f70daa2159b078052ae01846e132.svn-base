using System;
using System.ComponentModel;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Messaging.Config;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Exceptions;

namespace Brierley.FrameWork.Messaging.Msmq
{
	public class MsmqTransportProvider : ITransportProvider
	{
		private const string _className = "MsmqTransportProvider";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private static IMessageSerializer _serializer = new JsonMessageSerializer();
		private static object _lock = new object();

        public IMessageSerializer Serializer
        {
            get
            {
                return _serializer;
            }
        }

        public MessageListener GetListener(LWConfigurationContext ctx, EndPointCfg endpoint, BusConfiguration config)
        {
            MessageListener ret = null;
            if (ctx != null)
            {
                ret = new MsmqMessageListener(endpoint, ctx.Organization, ctx.Environment, this, config.Monitor);
            }
            else
            {
                endpoint.Listener = new MsmqMessageListener(endpoint, string.Empty, string.Empty, this, config.Monitor);
            }
            return ret;
        }

		public EndPointCfg InitializeQueue(EndPointCfg cfg)
		{
			const string methodName = "InitializeQueue";
			string queuePath = GetQueuePath(cfg);
			if (MessageQueue.Exists(queuePath))
			{
				_logger.Trace(_className, methodName, "Opening existing queue with path: " + queuePath);
				MessageQueue q = new MessageQueue(queuePath);
				cfg.Queue = q;
			}
			else
			{
				throw new LWMessagingException(string.Format("Queue with path {0} does not exist.", queuePath));
			}
			return cfg;
		}

		public void MoveToSubQueue(object q, string subQueueName, IMessage m, EndPointCfg cfg)
		{
			if (q == null)
			{
				throw new ArgumentNullException("q");
			}
			if (!(q is MessageQueue))
			{
				throw new ArgumentException("q must be of type System.Messaging.MessageQueue");
			}
			if (subQueueName == null)
			{
				throw new ArgumentNullException("subQueueName");
			}
			if (subQueueName == string.Empty)
			{
				throw new ArgumentOutOfRangeException("subQueueName cannot be an empty string");
			}
			if (m == null)
			{
				throw new ArgumentNullException("m");
			}
			if (cfg == null)
			{
				throw new ArgumentNullException("cfg");
			}

			MessageQueue queue = q as MessageQueue;
			Message message = m.TransportMessage as Message;

			string methodName = "MoveToSubQueue";
			var fullSubQueueName = cfg.Public ?
				@"DIRECT=OS:" + GetHostName(cfg) + @"\" + queue.QueueName + ";" + subQueueName :
				@"DIRECT=OS:.\" + queue.QueueName + ";" + subQueueName;
			try
			{
				IntPtr queueHandle = IntPtr.Zero;
				var error = NativeMethods.MQOpenQueue(fullSubQueueName, NativeMethods.MQ_MOVE_ACCESS, NativeMethods.MQ_DENY_NONE, ref queueHandle);
				if (error != 0)
				{
					MessageQueueErrorCode ecode = (MessageQueueErrorCode)error;
					string errMsg = string.Format("Failed to open queue: {0}.  Error code: {1}.", fullSubQueueName, ecode.ToString());
					_logger.Error(_className, methodName, errMsg);
					throw new LWMsmqException(errMsg, new Win32Exception(error));
				}
				try
				{
					error = NativeMethods.MQMoveMessage(queue.ReadHandle, queueHandle, message.LookupId, null);
					if (error != 0)
					{
						MessageQueueErrorCode ecode = (MessageQueueErrorCode)error;
						string errMsg = string.Format("Failed to move message with lookup id {2} to queue: {0}.  Error code {1}.", fullSubQueueName, ecode.ToString(), message.LookupId);
						_logger.Error(_className, methodName, errMsg);
						throw new LWMsmqException(errMsg, new Win32Exception(error));
					}
				}
				finally
				{
					error = NativeMethods.MQCloseQueue(queueHandle);
					if (error != 0)
					{
						MessageQueueErrorCode ecode = (MessageQueueErrorCode)error;
						string errMsg = string.Format("Failed to close queue: {0}.  Error code: {1}.", fullSubQueueName, ecode.ToString());
						_logger.Error(_className, methodName, errMsg);
						throw new LWMsmqException(errMsg, new Win32Exception(error));
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(
					_className,
					methodName,
					string.Format(
						"Error moving message with Id {0} to subqueue {1}",
						message.Id,
						fullSubQueueName),
					ex);
				throw;
			}
		}

		public void SendMessage(EndPointCfg endpointCfg, object messageContent, long msgId)
		{
			MessageState msgState = new MessageState(messageContent, msgId, endpointCfg);
			SendMessage(msgState);
		}

		public virtual IMessage ReceiveAndConsume(EndPointCfg epCfg)
		{
			const string methodName = "ReceiveAndConsume";
			IMessage message = null;
			MessageQueueTransactionType transactionType = GetTransactionType(epCfg);
			try
			{
				Message msmqMessage = null;
				MessageQueue queue = epCfg.Queue as MessageQueue;
				if (queue != null)
				{
					//hack: when moving messages to the errors subqueue, we must do a PInvoke, which does not seem to play well 
					//with transactions. Without a usable transaction in play, we have to lock in order to prevent other threads 
					//consuming this endpoint from pulling the message back out of the queue before we're able to send it to the
					//errors subqueue.
					lock (MessageListener.GetMutex(epCfg))
					{
						msmqMessage = queue.Receive(new TimeSpan(0, 0, 0, 0, epCfg.PollingTimeout), transactionType);
					}

					if (msmqMessage != null)
					{
						message = new MsmqMessage(msmqMessage);
						msmqMessage.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });
						ConsumeMessage(epCfg, message);
					}
				}
			}
			catch (Exception ex)
			{
				if (ex is MessageQueueException && ex.Message == "Timeout for the requested operation has expired.")
				{
					_logger.Debug(_className, methodName, ex.Message);
					throw new LWTransportTimeoutException("Error receiving and consuming message.", ex);
				}
				if (message == null)
				{
					message = new MsmqMessage(null);
				}
				message.Exception = ex;
			}
			return message;
		}

		protected void ConsumeMessage(EndPointCfg epCfg, IMessage message)
		{
			const string methodName = "ConsumeMessage";

			object msg = _serializer.Deserialize(message, epCfg.MessageCfg.Type);
			_logger.Debug(_className, methodName, "Consuming message " + message.Id);
			IConsumer consumer = null;
			try
			{
				consumer = epCfg.ConsumerFactory.ReserveInstance();
				using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
				{
                    int tries = epCfg.RetryCount;
                    while (true)
                    {
                        try
                        {
                            consumer.Consume(msg);
                            break;
                        }
                        catch
                        {
                            if (tries <= 0)
                                throw;
                            _logger.Warning(string.Format("Exception thrown in Consume. Sleeping for {0}ms before retrying {1} more times.", epCfg.RetryTimeout, tries));
                            tries--;
                            Thread.Sleep(epCfg.RetryTimeout);
                        }
                    }
                    scope.Complete();
				}
			}
			finally
			{
				if (consumer != null)
				{
					epCfg.ConsumerFactory.ReturnInstance(consumer);
				}
			}
		}

		private static IMessageSerializer GetSerializer()
		{
			if (_serializer == null)
			{
				lock (_lock)
				{
					if (_serializer == null)
					{
						_serializer = new JsonMessageSerializer();
					}
				}
			}
			return _serializer;
		}

		private string GetHostName(EndPointCfg cfg)
		{
			if (cfg == null)
			{
				throw new ArgumentNullException("cfg");
			}
			return (new Uri(cfg.Uri)).Host;
		}

		private string GetQueuePath(EndPointCfg cfg)
		{
			if (cfg == null)
			{
				throw new ArgumentNullException("cfg");
			}

			Uri uri = new Uri(cfg.Uri);
			string hostName = uri.Host;
			string queuePathWithFlatSubQueue = uri.AbsolutePath.Substring(1);
			if (!string.IsNullOrEmpty(uri.Fragment) && uri.Fragment.Length > 1)
			{
				queuePathWithFlatSubQueue += uri.Fragment;
			}

			if (cfg.Public)
			{
				if (hostName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
				{
					hostName = System.Net.Dns.GetHostName();
				}
				return hostName + @"\" + queuePathWithFlatSubQueue;
			}
			return hostName + @"\private$\" + queuePathWithFlatSubQueue;
		}

		private MessageQueueTransactionType GetTransactionType(EndPointCfg epCfg)
		{
			const string methodName = "GetTransactionType";
			try
			{
				if (epCfg.Transactional)
				{
					return Transaction.Current == null ? MessageQueueTransactionType.Single : MessageQueueTransactionType.Automatic;
				}
				return MessageQueueTransactionType.None;
			}
			catch (Exception e)
			{
				_logger.Error(_className, methodName, "Could not access the ambient transaction", e);
				throw;
			}
		}

		private void SendMessage(object state)
		{
			const string methodName = "SendMessage";

			if (state == null)
			{
				throw new ArgumentNullException("state");
			}
			if (!(state is MessageState))
			{
				throw new ArgumentException("state must be of type Brierley.FrameWork.Messaging.MsmqMsmqTransportProvider.MessageState");
			}

			MessageState msgState = (MessageState)state;
			if (msgState.Endpoint == null)
			{
				throw new Exception("Cannot send message. MessageState endpoint is null");
			}

			MessageQueue queue = msgState.Endpoint.Queue as MessageQueue;
			if (msgState.Endpoint.Queue == null)
			{
				throw new Exception("Failed to cast queue as MessageQueue");
			}

			try
			{
				IMessage message = GetSerializer().Serialize(msgState.Id.ToString(), msgState.Message);
				Message msmqMessage = message.TransportMessage as Message;

				if (msgState.Endpoint.Transactional)
				{
					queue.Send(msmqMessage, MessageQueueTransactionType.Single);
				}
				else
				{
					queue.Send(msmqMessage);
				}
				_logger.Debug(_className, methodName, string.Format("Sent message {0} to {1}.", msmqMessage.Label, msgState.Endpoint.Name));
			}
			catch (Exception ex)
			{
				_logger.Error(_className, methodName, string.Format("Error sending message to {0}.", msgState.Endpoint.Name), ex);
				throw;
			}
		}
	}
}
