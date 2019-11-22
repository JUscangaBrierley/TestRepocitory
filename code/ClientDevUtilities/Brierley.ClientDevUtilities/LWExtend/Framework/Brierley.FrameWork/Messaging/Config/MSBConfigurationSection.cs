using System;
using System.Configuration;

namespace Brierley.FrameWork.Messaging.Config
{
	public class MSBConfigurationSection : ConfigurationSection
	{
		public MSBConfigurationSection()
		{
			SetupDefaults();
		}

		[ConfigurationProperty(name: "MonitorAssembly", IsRequired = false)]
		public string MonitorAssembly
		{
			get { return this["MonitorAssembly"] as string; }
		}

		[ConfigurationProperty(name: "MonitorTypeName", IsRequired = false)]
		public string MonitorTypeName
		{
			get { return this["MonitorTypeName"] as string; }
		}

		public MessageElementCollection Messages
		{
			get { return this["Messages"] as MessageElementCollection; }
		}

		public EndpointElementCollection Endpoints
		{
			get { return this["EndPoints"] as EndpointElementCollection; }
		}

		public ConsumerElementCollection Consumers
		{
			get { return this["Consumers"] as ConsumerElementCollection; }
		}

		private void SetupDefaults()
		{
			Properties.Add(new ConfigurationProperty("Messages", typeof(MessageElementCollection), null));
			Properties.Add(new ConfigurationProperty("EndPoints", typeof(EndpointElementCollection), null));
			Properties.Add(new ConfigurationProperty("Consumers", typeof(ConsumerElementCollection), null));
		}
	}
}
