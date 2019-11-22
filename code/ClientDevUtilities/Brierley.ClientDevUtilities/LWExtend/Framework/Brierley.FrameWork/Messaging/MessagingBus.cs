using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Messaging.Config;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Exceptions;

namespace Brierley.FrameWork.Messaging
{
    public sealed class MessagingBus : IDisposable
    {
        private const string _className = "MessagingBus";

        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
        private static object instanceLock = new object();
        private static MessagingBus _instance;
        private static BusConfiguration _config;
        private static long _msgID = 0;
        private static Dictionary<string, ITransportProvider> _transportProviders = new Dictionary<string, ITransportProvider>(); // = new Brierley.FrameWork.Messaging.Msmq.MsmqTransportFactory();
        private static bool _messagingEnabled = true;

        private MessagingBus()
        {
        }

        public static MessagingBus Instance()
        {
            if (_instance == null)
            {
                Initialize();
            }
            return _instance;
        }

        public static MessagingBus Initialize(BusConfiguration config = null)
        {
            string methodName = "Initialize";
            if (_instance == null)
            {
                lock (instanceLock)
                {
                    if (_instance == null)
                    {
                        string strValue = LWConfigurationUtil.GetConfigurationValue("MessagingEnabled");
                        if (!string.IsNullOrEmpty(strValue))
                        {
                            _messagingEnabled = bool.Parse(strValue);
                        }

                        _logger.Trace(_className, methodName, "Initializing message bus...");

                        if (_messagingEnabled)
                        {
                            _config = config ?? new BusConfiguration();
                            _config.InitializeConfig();
                            _config.Validate();

                            foreach (EndPointCfg ep in _config.Endpoints)
                            {
                                string key = GetProviderGey(ep);
                                if (!_transportProviders.ContainsKey(key))
                                {
                                    var factory = ClassLoaderUtil.CreateInstance(ep.FactoryAssemblyName, ep.FactoryTypeName) as ITransportProviderFactory;
                                    if (factory == null)
                                    {
                                        throw new Exception(string.Format("Failed to create instance of ITransportProviderFactory from type {0}, assembly {1}", ep.FactoryTypeName, ep.FactoryAssemblyName));
                                    }
                                    _transportProviders.Add(key, factory.CreateMessageProvider());
                                }
                                var provider = _transportProviders[key];
                                provider.InitializeQueue(ep);
                            }
                        }
                        else
                        {
                            _logger.Warning(_className, methodName, "Messaging support is disabled.");
                        }
                        _instance = new MessagingBus();
                    }
                }
            }
            return _instance;
        }

        public static bool HasInstance()
        {
            lock (instanceLock)
            {
                return _instance != null;
            }
        }

        public static bool IsMessagingEnabled()
        {
            return _messagingEnabled;
        }

        public static void BeginListening()
        {
            const string methodName = "BeginListening";

            Instance();

            if (_messagingEnabled)
            {
                // start internal listenting thread(s) to listen on message queues and dispatch messages to consumers when received.
                LWConfigurationContext ctx = LWConfigurationUtil.GetCurrentEnvironmentContext();
                foreach (EndPointCfg endpoint in _config.Endpoints)
                {
                    if (endpoint.Direction == EndPointCfg.EndPointDirection.Incoming)
                    {
                        _logger.Trace(
                            _className,
                            methodName,
                            string.Format("Setting up endpoint {0} for listening activity.", endpoint.Name));

                        if (endpoint.Listener == null)
                        {
                            endpoint.Listener = _transportProviders[GetProviderGey(endpoint)].GetListener(ctx, endpoint, _config); //  new MessageListener(endpoint, ctx.Organization, ctx.Environment, _transportProviders[GetProviderGey(endpoint)], _config.Monitor);
                        }

                        endpoint.Listener.Start();
                    }
                }
            }
            else
            {
                _logger.Warning(_className, methodName, "Messaging support is disabled.");
            }
        }

        public static void Shutdown()
        {
            string methodName = "Shutdown";

            _logger.Trace(_className, methodName, "Shutting down service bus.");

            if (HasInstance())
            {
                Instance().Dispose();
            }
        }

        /// <summary>
        /// Returns a boolean indicating whether or not the bus is configured to accept and send messages of the specified type.
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public static bool CanSend(Type messageType)
        {
            const string methodName = "CanSend";
            try
            {
                Instance(); // <-- this is incredibly stupid for a static method. What's wrong with this class? Are you static, singleton or none of the above?

                if (!_messagingEnabled)
                {
                    return false;
                }

                MessageCfg config = _config.LookupMessageCfgByType(messageType);
                if (config == null || config.EndPointCfgs == null || config.EndPointCfgs.Count == 0)
                {
                    return false;
                }

                foreach (EndPointCfg e in config.EndPointCfgs)
                {
                    if (e.Direction == EndPointCfg.EndPointDirection.Outgoing && e.Queue != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Critical(_className, methodName, "Failed to determine whether message queueing is enabled due to an unexpected exception. As a result, work intended to be queued may not be.", ex);
            }
            return false;
        }

        public void Send<TMessage>(TMessage msg)
        {
            string methodName = "Send";

            if (_messagingEnabled)
            {
                _logger.Debug(_className, methodName, "Sending message of type " + typeof(TMessage).Name);

                MessageCfg msgCfg = _config.LookupMessageCfgByType(typeof(TMessage));
                if (msgCfg != null)
                {
                    if (msgCfg.EndPointCfgs == null || msgCfg.EndPointCfgs.Count == 0)
                    {
                        string errMsg = string.Format(
                            "No endpoints have been configured for message {0}.",
                            msgCfg.Name);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWMessagingException(errMsg);
                    }

                    if (msgCfg.EndPointCfgs.Count > 1)
                    {
                        string errMsg = string.Format(
                            "{0} endpoints have been configured for message {1}.  Using first one.",
                            msgCfg.EndPointCfgs.Count,
                            msgCfg.Name);
                        _logger.Warning(_className, methodName, errMsg);
                    }

                    if (msgCfg.EndPointCfgs[0].Direction != EndPointCfg.EndPointDirection.Outgoing)
                    {
                        string errMsg = string.Format(
                            "Endpoint {0} is not designated as Outgoing.  Cannot send message over it.",
                            msgCfg.EndPointCfgs[0].Name);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWMessagingException(errMsg);
                    }

                    if (msgCfg.EndPointCfgs[0].Queue == null)
                    {
                        string errMsg = string.Format(
                            "No queue available for message {0}.",
                            typeof(TMessage).Name);
                        _logger.Error(_className, methodName, errMsg);
                        throw new LWMessagingException(errMsg);
                    }

                    try
                    {
                        _logger.Debug(_className, methodName, string.Format("Sending message {0} over queue {1}", msgCfg.Name, msgCfg.EndPointCfgs[0].Name));
                        _transportProviders[GetProviderGey(msgCfg.EndPointCfgs[0])].SendMessage(msgCfg.EndPointCfgs[0], msg, _msgID);
                        _msgID++;
                    }
                    catch (Exception ex)
                    {
                        string errMsg = string.Format(
                            "Unable to send message {0} over q with Uri {1}.",
                            msgCfg.Name,
                            msgCfg.EndPointCfgs[0].Name);
                        _logger.Error(_className, methodName, errMsg, ex);
                        throw;
                    }
                }
                else
                {
                    string errMsg = string.Format("Unable to find configuration for message of type {0}.", typeof(TMessage).Name);
                    _logger.Error(_className, methodName, errMsg);
                    throw new LWMessagingException(errMsg);
                }
            }
        }

        public void Dispose()
        {
            if (_config != null)
            {
                foreach (EndPointCfg endpoint in _config.Endpoints)
                {
                    if (endpoint.Listener != null)
                    {
                        endpoint.Listener.ShutDown();
                    }
                }
            }
        }

        private static string GetProviderGey(EndPointCfg endpoint)
        {
            return string.Format("{0};{1}", endpoint.FactoryTypeName, endpoint.FactoryAssemblyName).ToLower();
        }
    }
}
