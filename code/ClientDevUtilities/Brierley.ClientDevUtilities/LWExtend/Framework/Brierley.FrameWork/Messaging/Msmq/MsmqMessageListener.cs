using System;
using System.Collections.Generic;
using System.Threading;
using System.Transactions;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Messaging.Config;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Exceptions;

namespace Brierley.FrameWork.Messaging.Msmq
{
    public class MsmqMessageListener : MessageListener
    {
        private const string _className = "MsmqMessageListener";

        public MsmqMessageListener(EndPointCfg endpointCfg, string orgName, string envName, ITransportProvider messagingProvider, IMessagingMonitor monitor = null) : base (endpointCfg, orgName, envName, messagingProvider, monitor)
        {
        }

        protected override void ConsumeMessages(object state)
        {
            string methodName = "ConsumeMessages";
            while (_keepRunning)
            {
                try
                {
                    ConsumeNextMessage();
                }
                catch (TransactionAbortedException ex)
                {
                    //transaction aborted - log warning, but the exception that caused the abort should have been logged already
                    _logger.Warning(_className, methodName, "Transaction aborted during message processing. This is not critical.", ex);
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
            TransactionScope scope = null;
            try
            {
                if (_endpoinCfg.Transactional)
                {
                    scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromMilliseconds(_endpoinCfg.TransactionTimeout) /*, TransactionScopeAsyncFlowOption.Enabled*/);
                }
                message = _messagingProvider.ReceiveAndConsume(_endpoinCfg);

                if (message.Exception != null)
                {
                    lock (GetMutex(_endpoinCfg))
                    {
                        //kill/rollback the transaction
                        if (scope != null)
                        {
                            scope.Dispose();
                            scope = null;
                        }

                        //todo: a better policy on how many retries a message can take before moving off to the errors subqueue.

                        bool toSubqueue = message != null && message.TransportMessage != null && _endpoinCfg.Transactional;

                        if (_monitor != null)
                        {
                            _monitor.Failure(_endpoinCfg.MessageName, message, message.Exception, toSubqueue);
                        }

                        if (toSubqueue)
                        {
                            _logger.Error(
                                _className,
                                methodName,
                                string.Format("Failed to process message {0} properly. Transaction aborted. Moving message to errors subqueue.", message.Id),
                                message.Exception);
                            _messagingProvider.MoveToSubQueue(_endpoinCfg.Queue, "errors", message, _endpoinCfg);
                        }
                        else
                        {
                            _logger.Error(
                                _className,
                                methodName,
                                string.Format("Failed to process message properly. Transaction aborted."),
                                message.Exception);
                        }
                    }
                }
                else
                {
                    if (_monitor != null)
                    {
                        _monitor.Success(_endpoinCfg.MessageName, message);
                    }
                }
            }
            catch (LWTransportTimeoutException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.Critical(
                    _className,
                    methodName,
                    string.Format("Failed to process message. Transaction aborted."),
                    ex);
                throw;
            }
            finally
            {
                if (scope != null)
                {
                    scope.Complete();
                    scope.Dispose();
                }
            }
        }
    }
}
