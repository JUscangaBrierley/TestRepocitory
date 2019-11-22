using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Messaging.Config
{
    public class MessageElement : ConfigurationElement
    {
        [ConfigurationProperty("Name")]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

		[ConfigurationProperty("MessageTypeName")]
		public string MessageTypeName
        {
			get { return (string)this["MessageTypeName"]; }
			set { this["MessageTypeName"] = value; }
        }

		[ConfigurationProperty("MessageAssemblyName")]
		public string MessageAssemblyName
        {
			get { return (string)this["MessageAssemblyName"]; }
			set { this["MessageAssemblyName"] = value; }
        }
    }
}
