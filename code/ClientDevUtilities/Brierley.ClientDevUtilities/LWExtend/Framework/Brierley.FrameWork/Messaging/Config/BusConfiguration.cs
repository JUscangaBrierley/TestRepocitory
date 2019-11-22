using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Messaging.Contracts;
using Brierley.FrameWork.Messaging.Exceptions;

namespace Brierley.FrameWork.Messaging.Config
{
	public class BusConfiguration
	{
		private const string _className = "BusConfiguration";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private static Dictionary<string, EndPointCfg> _endpoints = new Dictionary<string, EndPointCfg>();
		private static Dictionary<string, MessageCfg> _messages = new Dictionary<string, MessageCfg>();
		private static Dictionary<string, ConsumerCfg> _consumers = new Dictionary<string, ConsumerCfg>();

		public IEnumerable<EndPointCfg> Endpoints
		{
			get
			{
				return _endpoints.Values;
			}
		}

		public IMessagingMonitor Monitor { get; set; }

		public BusConfiguration()
		{
		}

		public void InitializeConfig()
		{
			const string methodName = "InitializeConfig";

			//Read Configuration Data
			_logger.Debug(_className, methodName, "Initializing lw.msg configuration.");
			MSBConfigurationSection cfg = ConfigurationManager.GetSection("lw.msg") as MSBConfigurationSection;

			if (cfg == null)
			{
				string msg = "No messaging configuration defined under lw.msg section.";
				_logger.Critical(_className, methodName, msg);
				throw new LWMessagingCfgException(msg);
			}

			foreach (EndpointElement endpoint in cfg.Endpoints)
			{
				EndPointCfg ep = new EndPointCfg()
				{
					Name = endpoint.Name,
					Uri = endpoint.Uri,
					Public = endpoint.Public,
					Transactional = endpoint.Transactional,
					NumberOfThreads = endpoint.NumberOfThreads,
					PollingTimeout = endpoint.PollingTimeout,
                    TransactionTimeout = endpoint.TransactionTimeout, 
					Direction = EndPointCfg.EndPointDirection.Outgoing,
					MessageName = endpoint.MessageName,
                    RetryCount = endpoint.RetryCount,
                    RetryTimeout = endpoint.RetryTimeout, 
                    AwsAccessKey = endpoint.AwsAccessKey, 
                    AwsSecretKey = endpoint.AwsSecretKey, 
                    AwsRegion = endpoint.AwsRegion, 
                    FactoryAssemblyName = endpoint.FactoryAssemblyName, 
                    FactoryTypeName = endpoint.FactoryTypeName
				};
				if (!string.IsNullOrEmpty(endpoint.Direction))
				{
					ep.Direction = (EndPointCfg.EndPointDirection)Enum.Parse(typeof(EndPointCfg.EndPointDirection), endpoint.Direction);
				}
				AddEndpoint(ep);
			}

			foreach (MessageElement msg in cfg.Messages)
			{
				MessageCfg m = new MessageCfg()
				{
					Name = msg.Name,
					MessageTypeName = msg.MessageTypeName,
					MessageAssemblyName = msg.MessageAssemblyName,
				};
				AddMessage(m);
			}

			foreach (ConsumerElement consumer in cfg.Consumers)
			{
				ConsumerCfg c = new ConsumerCfg()
				{
					Name = consumer.Name,
					FactoryTypeName = consumer.FactoryTypeName,
					FactoryAssemblyName = consumer.FactoryAssemblyName,
					MessageName = consumer.MessageName
				};
                
				c.LifecyclePolicy = ConsumerLifecyclePolicy.PerInstanceConsumption;
				if (!string.IsNullOrEmpty(consumer.LifeCyclePolicy))
				{
					c.LifecyclePolicy = (ConsumerLifecyclePolicy)Enum.Parse(typeof(ConsumerLifecyclePolicy), consumer.LifeCyclePolicy);
				}
				if (c.LifecyclePolicy == ConsumerLifecyclePolicy.ConsumerPool)
				{
					c.ConsumerPoolSize = consumer.ConsumerPoolSize > 0 ? consumer.ConsumerPoolSize : 10;
				}
				AddConsumer(c);
			}

			if (!string.IsNullOrEmpty(cfg.MonitorAssembly) && !string.IsNullOrEmpty(cfg.MonitorTypeName))
			{
				var instance = ClassLoaderUtil.CreateInstance(cfg.MonitorAssembly, cfg.MonitorTypeName);
				if (instance == null)
				{
					throw new Exception(string.Format("Failed to create instance of {0} from assembly {1}", cfg.MonitorTypeName, cfg.MonitorAssembly));
				}
				if (!(instance is IMessagingMonitor))
				{
					throw new Exception(string.Format("The instance {0} from assembly {1} is not of type IMessagingMonitor", cfg.MonitorTypeName, cfg.MonitorAssembly));
				}
				Monitor = (IMessagingMonitor)instance;
			}
		}

		public void Validate()
		{
			const string methodName = "Validate";

			_logger.Debug(_className, methodName, "Validating bus configuration.");

			foreach (EndPointCfg ep in _endpoints.Values)
			{
				if (_messages.ContainsKey(ep.MessageName))
				{
					MessageCfg msg = _messages[ep.MessageName];
					ep.MessageCfg = msg;
					if (msg.EndPointCfgs == null)
					{
						msg.EndPointCfgs = new List<EndPointCfg>();
					}
					msg.EndPointCfgs.Add(ep);
				}
				else
				{
					string errMsg = string.Format("Message {0} not defined but required by endpoint {1}", ep.MessageName, ep.Name);
					_logger.Error(_className, methodName, errMsg);
					throw new LWMessagingCfgException(errMsg);
				}
			}

			foreach (MessageCfg msg in _messages.Values)
			{
				object inst = ClassLoaderUtil.CreateInstance(msg.MessageAssemblyName, msg.MessageTypeName);
				if (inst == null)
				{
					string errMsg = string.Format("Unable to instantiate message of type {0} from assembly {1}.", msg.MessageTypeName, msg.MessageAssemblyName);
					_logger.Error(_className, methodName, errMsg);
					throw new LWMessagingCfgException(errMsg);
				}
				msg.Type = inst.GetType();
			}

			IConsumerFactory consumerFactory = null;
			foreach (ConsumerCfg cfg in _consumers.Values)
			{
				consumerFactory = ClassLoaderUtil.CreateInstance(cfg.FactoryAssemblyName, cfg.FactoryTypeName) as IConsumerFactory;
				if (consumerFactory == null)
				{
					string errMsg = string.Format("Unable to instantiate consumer factory of type {0} from assembly {1}.", cfg.FactoryTypeName, cfg.FactoryAssemblyName);
					_logger.Error(_className, methodName, errMsg);
					throw new LWMessagingCfgException(errMsg);
				}
				consumerFactory.Initialize(cfg);
				cfg.Type = consumerFactory.GetType();
				MessageCfg mcfg = LookupMessageCfgByName(cfg.MessageName);
				if (mcfg != null && mcfg.EndPointCfgs != null && mcfg.EndPointCfgs.Count > 0)
				{
					mcfg.ConsumerCfg = cfg;
					foreach (EndPointCfg epcfg in mcfg.EndPointCfgs)
					{
						if (epcfg.ConsumerFactory == null)
						{
							epcfg.ConsumerFactory = consumerFactory;
						}
					}
				}
				else
				{
					string errMsg = string.Format("Unable to find configuration of message {0} from consumer {1}.", cfg.MessageName, cfg.Name);
					_logger.Error(_className, methodName, errMsg);
					throw new LWMessagingCfgException(errMsg);
				}
			}
		}

		public void AddEndpoint(EndPointCfg endpoint)
		{
			string methodName = "AddEndpoint";

			//hack: check for multiple endpoints that use the same URI. This is allowed (for now), but will log a warning.
			if (_endpoints.Values.FirstOrDefault(o => o.Uri.Equals(endpoint.Uri)) != null)
			{
				_logger.Warning(_className, methodName, "An endpoint with the URI {0} has already been configured. Endpoint URIs should be unique.");
			}

			if (!_endpoints.ContainsKey(endpoint.Name))
			{
				_endpoints.Add(endpoint.Name, endpoint);
			}
			else
			{
				string errMsg = string.Format("Endpoint {0} has already been specified in configuration.", endpoint.Name);
				_logger.Error(_className, methodName, errMsg);
				throw new LWMessagingCfgException(errMsg);
			}
		}

		public void AddMessage(MessageCfg msg)
		{
			string methodName = "AddMessage";

			if (!_messages.ContainsKey(msg.Name))
			{
				_messages.Add(msg.Name, msg);
			}
			else
			{
				string errMsg = string.Format("Message {0} has already been specified in configuration.", msg.Name);
				_logger.Error(_className, methodName, errMsg);
				throw new LWMessagingCfgException(errMsg);
			}
		}

		public void AddConsumer(ConsumerCfg consumer)
		{
			string methodName = "AddConsumer";

			if (!_consumers.ContainsKey(consumer.Name))
			{
				_consumers.Add(consumer.Name, consumer);
			}
			else
			{
				string errMsg = string.Format("Consumer {0} has already been specified in configuration.", consumer.Name);
				_logger.Error(_className, methodName, errMsg);
				throw new LWMessagingCfgException(errMsg);
			}
		}

		public MessageCfg LookupMessageCfgByName(string name)
		{
			MessageCfg cfg = null;
			if (_messages != null && _messages.Count > 0)
			{
				foreach (MessageCfg c in _messages.Values)
				{
					if (c.Name == name)
					{
						cfg = c;
						break;
					}
				}
			}
			return cfg;
		}

		public MessageCfg LookupMessageCfgByType(Type type)
		{
			MessageCfg cfg = null;
			if (_messages != null && _messages.Count > 0)
			{
				foreach (MessageCfg c in _messages.Values)
				{
					if (c.Type == type)
					{
						cfg = c;
						break;
					}
				}
			}
			return cfg;
		}

		public ConsumerCfg LookupConsumerCfgByType(Type type)
		{
			ConsumerCfg cfg = null;
			if (_consumers != null && _consumers.Count > 0)
			{
				foreach (ConsumerCfg c in _consumers.Values)
				{
					if (c.Type == type)
					{
						cfg = c;
						break;
					}
				}
			}
			return cfg;
		}
	}
}
