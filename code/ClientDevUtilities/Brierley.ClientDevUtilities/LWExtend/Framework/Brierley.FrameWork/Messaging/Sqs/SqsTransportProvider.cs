using System;
using System.Threading;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Messaging.Config;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Exceptions;
using Brierley.FrameWork.Messaging.Messages;

namespace Brierley.FrameWork.Messaging.Sqs
{
    public class SqsTransportProvider : ITransportProvider
    {
        private const string _className = "SqsTransportProvider";
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
                ret = new SqsMessageListener(endpoint, ctx.Organization, ctx.Environment, this, config.Monitor);
            }
            else
            {
                endpoint.Listener = new SqsMessageListener(endpoint, string.Empty, string.Empty, this, config.Monitor);
            }
            return ret;
        }

        public EndPointCfg InitializeQueue(EndPointCfg cfg)
        {
            const string methodName = "InitializeQueue";

            if (cfg == null)
            {
                throw new ArgumentNullException("cfg");
            }

            BasicAWSCredentials credentials = new BasicAWSCredentials(cfg.AwsAccessKey, cfg.AwsSecretKey);
            Amazon.RegionEndpoint region = Amazon.RegionEndpoint.USWest2;
            if (!string.IsNullOrEmpty(cfg.AwsRegion))
            {
                region = Amazon.RegionEndpoint.GetBySystemName(cfg.AwsRegion);
            }
            AmazonSQSClient client = new AmazonSQSClient(credentials, region);

            GetQueueAttributesRequest request = new GetQueueAttributesRequest();
            request.QueueUrl = cfg.Uri;

            GetQueueAttributesResponse response = client.GetQueueAttributes(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                cfg.Queue = client;
                _logger.Trace(_className, methodName, "SQS queue opened with path: " + cfg.Uri);
            }
            else
            {
                throw new LWMessagingException(string.Format("SQS queue does not exist at URI {1}", cfg.Uri));
            }
            return cfg;
        }

        public void MoveToSubQueue(object q, string subQueueName, IMessage m, EndPointCfg cfg)
        {
            //AWS, if configured correctly, will automatically move to dead-letter queue. No need for implementation here
        }

        public void SendMessage(EndPointCfg endpointCfg, object messageContent, long msgId)
        {
            throw new NotImplementedException();
        }

        public virtual IMessage ReceiveAndConsume(EndPointCfg cfg)
        {
            const string methodName = "ReceiveAndConsume";

            //SqsMessage is useless, other than to indicate to the listener that a message was processed, so that it may increment the counter
            //if no message is processed, we return null.
            SqsMessage ret = null;

            try
            {
                //todo: ideally, we should be using SQS's batching feature. Billing is done by number of calls to the queue;
                //batching messages and then dispatching them to the threads would reduce cost and latency in getting messages.
                //The present architecture does not easily allow us to do this.

                var client = (AmazonSQSClient)cfg.Queue;

                ReceiveMessageRequest request = new ReceiveMessageRequest();
                request.MaxNumberOfMessages = 1;
                request.QueueUrl = cfg.Uri;

                ReceiveMessageResponse response = client.ReceiveMessage(request);

                if (response.Messages.Count > 0)
                {
                    Message message = response.Messages[0];

                    ConsumeMessage(cfg, message);

                    //remove from queue
                    var deleteRequest = new DeleteMessageRequest();
                    deleteRequest.QueueUrl = cfg.Uri;
                    deleteRequest.ReceiptHandle = message.ReceiptHandle;

                    var deleteResponse = client.DeleteMessage(deleteRequest);
                    if (deleteResponse.HttpStatusCode >= System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new Exception(string.Format("Failed to delete message {0} from SQS. Delete request returned status code {1}.", message.MessageId, deleteResponse.HttpStatusCode.ToString()));
                    }
                    ret = new SqsMessage(null);
                }
                else
                {
                    //no messages in the queue. Sleep for the configured polling timeout. This mimics MSMQ and allows 
                    //us to pause, rather than immediately ask SQS for a message
                    System.Threading.Thread.Sleep(cfg.PollingTimeout);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(_className, methodName, ex.Message);
                throw new LWTransportTimeoutException("Error receiving and consuming SQS message.", ex);
            }

            return ret;
        }

        protected void ConsumeMessage(EndPointCfg epCfg, Message message)
        {
            const string methodName = "ConsumeMessage";

            IConsumer consumer = null;

            try
            {
                EmailFeedbackMessage msg = Newtonsoft.Json.JsonConvert.DeserializeObject<EmailFeedbackMessage>(message.Body);
                msg.Notification = Newtonsoft.Json.JsonConvert.DeserializeObject<AmazonSesBounceNotification>(msg.Message);

                _logger.Debug(_className, methodName, "Consuming message " + message.MessageId);
                consumer = epCfg.ConsumerFactory.ReserveInstance();

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
                        {
                            throw;
                        }
                        _logger.Warning(string.Format("Exception thrown in Consume. Sleeping for {0}ms before retrying {1} more times.", epCfg.RetryTimeout, tries));
                        tries--;
                        Thread.Sleep(epCfg.RetryTimeout);
                    }
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
            //SQS messages are in JSON format and there's no need for custom serialization; Newtonsoft works just fine
            return null;
        }
    }
}
