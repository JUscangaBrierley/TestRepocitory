using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Messaging.Config;

namespace Brierley.FrameWork.Messaging.Msmq
{
	internal class MessageState
	{
		public EndPointCfg Endpoint { get; set; }
		public object Message { get; set; }
		public long Id { get; set; }

		public MessageState()
		{
		}
		
		public MessageState(object message, long id, EndPointCfg endpoint)
		{
			Message = message;
			Id = id;
			Endpoint = endpoint;
		}
	}
}
