using System;
using System.Collections.Generic;
using System.Net.Http;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Dmc;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Common.Logging
{
	public class CommunicationLogData
	{
		[JsonIgnore]
		public CommunicationType MessageType { get; set; }

		[JsonIgnore]
		public HttpResponseMessage Response { get; set; }

		public string Path { get; set; }

		[JsonIgnore]
		public string Body { get; set; }

		[JsonIgnore]
		public long? MessageId { get; set; }

		[JsonIgnore]
		public long? QueueId { get; set; }

		public User User { get; set; }

		[JsonIgnore]
		public Exception Exception { get; set; }

		public Dictionary<string, string> Personalizations { get; set; }

		public List<Dmc.Attribute> Attributes { get; set; }

		[JsonConstructor]
		public CommunicationLogData()
		{
		}

		public CommunicationLogData(CommunicationType messageType)
		{
			MessageType = messageType;
		}

		public CommunicationLogData(CommunicationType messageType, HttpResponseMessage response, string path)
			: this(messageType)
		{
			Response = response;
			Path = path;
		}

		public CommunicationLogData(CommunicationType messageType, HttpResponseMessage response, string path, string body)
			: this(messageType, response, path)
		{
			Body = body;
		}

		public CommunicationLogData(CommunicationType messageType, HttpResponseMessage response, string path, Exception ex)
			: this(messageType, response, path)
		{
			Exception = ex;
		}

		public CommunicationLogData(CommunicationType messageType, HttpResponseMessage response, string path, string body, Exception ex)
			: this(messageType, response, path, ex)
		{
			Body = body;
		}
	}
}
