using System;
using System.Collections.Generic;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Messaging.Config;
using Brierley.FrameWork.Messaging.Contracts;

namespace Brierley.FrameWork.Messaging
{
	public abstract class ConsumerFactoryBase : IConsumerFactory
	{
		private bool _disposed = false;
		private ConsumerCfg _config;
		private IConsumer _singleton = null;
		private Stack<IConsumer> _availableConsumerPool = new Stack<IConsumer>();
		private IList<IConsumer> _busyConsumerPool = new List<IConsumer>();
		private object aLock = new object();
		private object bLock = new object();

		public virtual void Initialize(ConsumerCfg cfg)
		{
			_config = cfg;
			if (_config.LifecyclePolicy == ConsumerLifecyclePolicy.Singleton)
			{
				_singleton = CreateConsumerInstance();
			}
			else if (_config.LifecyclePolicy == ConsumerLifecyclePolicy.ConsumerPool)
			{
				for (int i = 0; i < _config.ConsumerPoolSize; i++)
				{
					IConsumer consumer = CreateConsumerInstance();
					_availableConsumerPool.Push(consumer);
				}
			}
		}

		/// <summary>
		/// This operation is invoked to get an instance of IConsumer to consume a message.
		/// </summary>
		/// <returns></returns>
		public IConsumer ReserveInstance()
		{
			IConsumer instance = null;
			switch (_config.LifecyclePolicy)
			{
				case ConsumerLifecyclePolicy.Singleton:
					instance = _singleton;
					break;
				case ConsumerLifecyclePolicy.PerInstanceConsumption:
					instance = CreateConsumerInstance();
					break;
				case ConsumerLifecyclePolicy.ConsumerPool:
					while (true)
					{
						instance = ReserveConsumerFromPool();
						if (instance == null)
						{
							System.Threading.Thread.Sleep(2000);
						}
						else
						{
							break;
						}
					}
					break;
			}
			return instance;
		}

		/// <summary>
		/// This is called after the message has been consumed.
		/// </summary>
		/// <param name="consumer"></param>
		/// <returns></returns>
		public void ReturnInstance(IConsumer consumer)
		{
			switch (_config.LifecyclePolicy)
			{
				case ConsumerLifecyclePolicy.PerInstanceConsumption:
					consumer.Dispose();
					consumer = null;
					break;
				case ConsumerLifecyclePolicy.ConsumerPool:
					ReturnConsumerToPool(consumer);
					break;
				default:
					break;
			}
		}

		public virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					switch (_config.LifecyclePolicy)
					{
						case ConsumerLifecyclePolicy.Singleton:
							_singleton.Dispose();
							_singleton = null;
							break;
						case ConsumerLifecyclePolicy.ConsumerPool:
							lock (aLock)
							{
								foreach (IConsumer c in _availableConsumerPool)
								{
									c.Dispose();
								}
								_availableConsumerPool.Clear();
							}
							lock (bLock)
							{
								foreach (IConsumer c in _busyConsumerPool)
								{
									c.Dispose();
								}
								_busyConsumerPool.Clear();
							}
							break;
						default:
							break;
					}
				}
				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected abstract IConsumer CreateConsumerInstance();

		private IConsumer ReserveConsumerFromPool()
		{
			IConsumer consumer = null;
			lock (aLock)
			{
				while (true)
				{
					if (_availableConsumerPool.Count > 0)
					{
						consumer = _availableConsumerPool.Pop();
						break;
					}
				}
			}

			if (consumer != null)
			{
				lock (bLock)
				{
					_busyConsumerPool.Add(consumer);
				}
			}

			return consumer;
		}

		private void ReturnConsumerToPool(IConsumer consumer)
		{
			bool removed = false;
			lock (bLock)
			{
				removed = _busyConsumerPool.Remove(consumer);
			}

			if (removed)
			{
				lock (aLock)
				{
					_availableConsumerPool.Push(consumer);
				}
			}
		}
	}
}
