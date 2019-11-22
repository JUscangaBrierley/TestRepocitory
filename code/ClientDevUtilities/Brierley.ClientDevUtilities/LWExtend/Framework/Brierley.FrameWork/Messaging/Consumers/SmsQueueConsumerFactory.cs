using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Messaging;
using Brierley.FrameWork.Messaging.Contracts;

namespace Brierley.FrameWork.Messaging.Consumers
{
	public class SmsQueueConsumerFactory : ConsumerFactoryBase
	{
		protected override IConsumer CreateConsumerInstance()
		{
			return new SmsQueueConsumer();
		}
	}
}
