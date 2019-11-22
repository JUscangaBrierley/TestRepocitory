using System;
using System.Threading;
using Brierley.FrameWork.Messaging.Config;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Exceptions;

namespace Brierley.FrameWork.Messaging.Sqs
{
    public class SqsMessageListener : MessageListener
    {
        private const string _className = "SqsMessageListener";

        public SqsMessageListener(EndPointCfg endpointCfg, string orgName, string envName, ITransportProvider messagingProvider, IMessagingMonitor monitor = null) : base(endpointCfg, orgName, envName, messagingProvider, monitor)
        {
        }

        protected override void ConsumeMessages(object state)
        {
            const string methodName = "ConsumeMessages";
            while (_keepRunning)
            {
                try
                {
                    ConsumeNextMessage();
                }
                catch (ThreadAbortException ex)
                {
                    //nothing to do here; process is being killed
                    _logger.Warning(_className, methodName, "Thread aborted during message processing. This is not critical.", ex);
                }
                catch (Exception ex)
                {
                    _logger.Critical(_className, methodName, "An error occured during message dispatch", ex);
                }
            }
            _logger.Trace(_className, methodName, string.Format("Background thread {0} for {1} is shutting down.", Thread.CurrentThread.Name, _endpoinCfg.Name));
        }

        private void ConsumeNextMessage()
        {
            const string methodName = "ConsumeNextMessage";

            IMessage message = null;
            try
            {
                message = _messagingProvider.ReceiveAndConsume(_endpoinCfg);

                if (_monitor != null && message != null && message.Exception == null)
                {
                    _monitor.Success(_endpoinCfg.MessageName, message);
                }
            }
            catch (LWTransportTimeoutException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.Critical(_className, methodName, string.Format("Failed to process message."), ex);

                if (_monitor != null)
                {
                    _monitor.Failure(_endpoinCfg.MessageName, message, ex, false);
                }

                throw;
            }
        }
    }
}
