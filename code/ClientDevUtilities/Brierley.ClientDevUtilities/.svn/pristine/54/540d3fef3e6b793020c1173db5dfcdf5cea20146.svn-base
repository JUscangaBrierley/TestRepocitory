using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Messaging.Config
{
    public class EndpointElement : ConfigurationElement
    {
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

        [ConfigurationProperty("Name")]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("Uri")]
        public string Uri
        {
            get { return (string)this["Uri"]; }
            set { this["Uri"] = value; }
        }

        [ConfigurationProperty("Public")]
        public bool Public
        {
            get { return StringUtils.FriendlyBool(this["Public"], true); }
            set { this["Public"] = value; }
        }

        [ConfigurationProperty("Direction")]
        public string Direction
        {
            get { return (string)this["Direction"]; }
            set { this["Direction"] = value; }
        }

		[ConfigurationProperty("Transactional")]
        public bool Transactional
        {
            get { return StringUtils.FriendlyBool(this["Transactional"], true); }
            set { this["Transactional"] = value; }
        }

        [ConfigurationProperty("PollingTimeout")]
        public int PollingTimeout
        {
            get { return StringUtils.FriendlyInt32(this["PollingTimeout"], 1000); }
            set { this["PollingTimeout"] = value; }
        }

        [ConfigurationProperty("TransactionTimeout")]
        public int TransactionTimeout
        {
            get { return StringUtils.FriendlyInt32(this["TransactionTimeout"], 300000); }
            set { this["TransactionTimeout"] = value; }
        }

        [ConfigurationProperty("NumberOfThreads")]
        public int NumberOfThreads
        {
            get { return StringUtils.FriendlyInt32(this["NumberOfThreads"], 3); }
            set { this["NumberOfThreads"] = value; }
        }

        [ConfigurationProperty("MessageName")]
        public string MessageName
        {
            get { return (string)this["MessageName"]; }
            set { this["MessageName"] = value; }
        }

        [ConfigurationProperty("RetryCount")]
        public int RetryCount
        {
            get { return StringUtils.FriendlyInt32(this["RetryCount"], 1); }
            set { this["RetryCount"] = value; }
        }

        [ConfigurationProperty("RetryTimeout")]
        public int RetryTimeout
        {
            get { return StringUtils.FriendlyInt32(this["RetryTimeout"], 1000); }
            set { this["RetryTimeout"] = value; }
        }

        [ConfigurationProperty("AwsAccessKey")]
        public string AwsAccessKey
        {
            get { return (string)this["AwsAccessKey"]; }
            set { this["AwsAccessKey"] = value; }
        }

        [ConfigurationProperty("AwsSecretKey")]
        public string AwsSecretKey
        {
            get { return (string)this["AwsSecretKey"]; }
            set { this["AwsSecretKey"] = value; }
        }

        [ConfigurationProperty("AwsRegion")]
        public string AwsRegion
        {
            get { return (string)this["AwsRegion"]; }
            set { this["AwsRegion"] = value; }
        }
    }
}
