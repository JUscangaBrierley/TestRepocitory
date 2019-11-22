using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Messaging.Config;
using Brierley.FrameWork.Messaging.Msmq;

namespace Brierley.ClientDevUtilities.MessageQueue
{
    public static class MessageQueueUtility
    {
        private const string CLASS_NAME = "MessageQueueUtility";

        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        public static void RetryFailedMessages(string endpointUri, bool isPublic)
        {
            const string METHOD_NAME = "RetryFailedMessages";
            const int MAX_ERROR_COUNT = 5;
            int errorCount = 0;
            int moveCount = 0;

            var initResult = InitializeErrorAndRootQueue(endpointUri, isPublic);
            var errorQueue = initResult.ErrorQueue;
            var rootQueue = initResult.RootQueue;

            string rootQueueName = !isPublic ?
                @"DIRECT=OS:.\" + rootQueue.QueueName :
                @"DIRECT=OS:" + (new Uri(endpointUri)).Host + @"\" + rootQueue.QueueName;

            var lookupIdList = new List<long>();
            var cursor = errorQueue.CreateCursor();
            var peekAction = PeekAction.Current;

            while (errorCount < MAX_ERROR_COUNT)
            {
                try
                {
                    Message message;

                    try
                    {
                        message = errorQueue.Peek(new TimeSpan(0, 0, 0, 0, 10000), cursor, peekAction);
                    }
                    catch (MessageQueueException ex)
                    {
                        if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                        {
                            break;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    peekAction = PeekAction.Next;
                    lookupIdList.Add(message.LookupId);
                }
                catch (Exception ex)
                {
                    errorCount++;

                    string errorMessage = string.Format("Failed to read message. Number of errors encountered: {0}; Error Message: {1}", errorCount, ex.Message);
                    _logger.Error(CLASS_NAME, METHOD_NAME, errorMessage, ex);
                }
            }

            if (errorCount >= MAX_ERROR_COUNT)
            {
                _logger.Error(CLASS_NAME, METHOD_NAME, "The maximum number of errors has been reached. Aborting the operation.");
                return;
            }

            IntPtr queueHandle = IntPtr.Zero;
            var openError = (MessageQueueErrorCode)NativeMethods.MQOpenQueue(rootQueueName, NativeMethods.MQ_MOVE_ACCESS, NativeMethods.MQ_DENY_NONE, ref queueHandle);

            if (openError != 0)
            {
                string errorMessage = string.Format("Failed to open queue: {0}. Error code: {1}.", rootQueueName, openError);
                _logger.Error(CLASS_NAME, METHOD_NAME, errorMessage);
                throw new Exception(errorMessage);
            }

            try
            {
                foreach (long lookupId in lookupIdList)
                {
                    try
                    {
                        if (errorCount >= MAX_ERROR_COUNT)
                        {
                            break;
                        }

                        var moveError = (MessageQueueErrorCode)NativeMethods.MQMoveMessage(errorQueue.ReadHandle, queueHandle, lookupId, null);

                        if (moveError != 0)
                        {
                            string errorMessage = string.Format("Failed to move message with lookup id {2} to queue: {0}. Error code {1}.", rootQueueName, moveError, lookupId);
                            _logger.Error(CLASS_NAME, METHOD_NAME, errorMessage);
                            throw new Exception(errorMessage);
                        }

                        moveCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;

                        string errorMessage = string.Format("Failed to move message. Number of errors encountered: {0}; Error Message: {1}", errorCount, ex.Message);
                        _logger.Error(CLASS_NAME, METHOD_NAME, errorMessage, ex);
                    }
                }
            }
            finally
            {
                var closeError = (MessageQueueErrorCode)NativeMethods.MQCloseQueue(queueHandle);

                if (closeError != 0)
                {
                    string errorMessage = string.Format("Failed to close queue: {0}. Error code: {1}.", rootQueueName, closeError);
                    _logger.Error(CLASS_NAME, METHOD_NAME, errorMessage);
                    throw new Exception(errorMessage);
                }
            }

            _logger.Trace(string.Format("{0} messages have been moved to queue: {1}", moveCount, rootQueueName));

            if (errorCount >= MAX_ERROR_COUNT)
            {
                _logger.Error(CLASS_NAME, METHOD_NAME, "The maximum number of errors has been reached. Aborting the operation.");
                return;
            }
        }

        private class InitializeErrorAndRootQueueResult
        {
            public System.Messaging.MessageQueue ErrorQueue { get; set; }
            public System.Messaging.MessageQueue RootQueue { get; set; }
        }

        private static InitializeErrorAndRootQueueResult InitializeErrorAndRootQueue(string endpointUri, bool isPublic)
        {
            var factory = new MsmqTransportFactory();

            var errorConfig = new EndPointCfg();
            errorConfig.Uri = endpointUri + ";errors";
            errorConfig.Public = isPublic;

            var errorProvider = factory.CreateMessageProvider();
            errorProvider.InitializeQueue(errorConfig);

            var rootConfig = new EndPointCfg();
            rootConfig.Uri = endpointUri;
            rootConfig.Public = isPublic;

            var rootProvider = factory.CreateMessageProvider();
            rootProvider.InitializeQueue(rootConfig);

            return new InitializeErrorAndRootQueueResult { ErrorQueue = (System.Messaging.MessageQueue)errorConfig.Queue, RootQueue = (System.Messaging.MessageQueue)rootConfig.Queue };
        }

        public static void MoveMessagesToDestinationQueue(string sourceEndpointUri, bool sourcIsPublic, string destinationEndpointUri, bool destinationIsPublic)
        {
            const string METHOD_NAME = "MoveMessagesToDestinationQueue";
            const int MAX_ERROR_COUNT = 5;
            int errorCount = 0;
            uint moveCount = 0;

            using (System.Messaging.MessageQueue sourceQueue = InitializeQueue(sourceEndpointUri, sourcIsPublic),
                destinationQueue = InitializeQueue(destinationEndpointUri, destinationIsPublic))
            {
                uint sourceMessageCount = MessageQueueNativeUtility.GetCount(sourceQueue);

                while (moveCount < sourceMessageCount)
                {
                    try
                    {
                        using (var scope = new TransactionScope())
                        {
                            if (errorCount >= MAX_ERROR_COUNT)
                            {
                                break;
                            }

                            var message = sourceQueue.Receive(new TimeSpan(0, 0, 0, 0, 10000), MessageQueueTransactionType.Automatic);
                            message.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });

                            if (message == null)
                            {
                                break;
                            }

                            var newMessage = new Message(message.Body) { Label = message.Label };

                            destinationQueue.Send(newMessage, MessageQueueTransactionType.Automatic);
                            scope.Complete();
                        }

                        moveCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;

                        string errorMessage = string.Format("Failed to move message. Number of errors encountered: {0}; Error Message: {1}", errorCount, ex.Message);
                        _logger.Error(CLASS_NAME, METHOD_NAME, errorMessage, ex);
                    }
                }

                _logger.Trace(string.Format("{0} messages have been moved from source queue '{1}' to destination queue '{2}'.", moveCount, sourceQueue.Path, destinationQueue.Path));
            }

            if (errorCount >= MAX_ERROR_COUNT)
            {
                _logger.Error(CLASS_NAME, METHOD_NAME, "The maximum number of errors has been reached. Aborting the operation.");
                return;
            }
        }

        private static System.Messaging.MessageQueue InitializeQueue(string endpointUri, bool isPublic)
        {
            var factory = new MsmqTransportFactory();

            var config = new EndPointCfg();
            config.Uri = endpointUri;
            config.Public = isPublic;

            var provider = factory.CreateMessageProvider();
            provider.InitializeQueue(config);

            return (System.Messaging.MessageQueue)config.Queue;
        }
    }
}
