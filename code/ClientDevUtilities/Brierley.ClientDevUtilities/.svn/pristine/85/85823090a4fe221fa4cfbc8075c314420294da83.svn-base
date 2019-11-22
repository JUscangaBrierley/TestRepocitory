using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Messaging.Config
{
    public class ConsumerElement : ConfigurationElement
    {
        [ConfigurationProperty("Name")]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("FactoryTypeName")]
        public string FactoryTypeName
        {
            get { return (string)this["FactoryTypeName"]; }
            set { this["FactoryTypeName"] = value; }
        }

		[ConfigurationProperty("FactoryAssemblyName")]
        public string FactoryAssemblyName
        {
            get { return (string)this["FactoryAssemblyName"]; }
            set { this["FactoryAssemblyName"] = value; }
        }

        [ConfigurationProperty("MessageName")]
        public string MessageName
        {
            get { return (string)this["MessageName"]; }
            set { this["MessageName"] = value; }
        }

        [ConfigurationProperty("LifeCyclePolicy")]
        public string LifeCyclePolicy
        {
            get { return (string)this["LifeCyclePolicy"]; }
            set { this["LifeCyclePolicy"] = value; }
        }

        [ConfigurationProperty("ConsumerPoolSize")]
        public int ConsumerPoolSize
        {
            get { return (int)this["ConsumerPoolSize"]; }
            set { this["ConsumerPoolSize"] = value; }
        }
    }
}
