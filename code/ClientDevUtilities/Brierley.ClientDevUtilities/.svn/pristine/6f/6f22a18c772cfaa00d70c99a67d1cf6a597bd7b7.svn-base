using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Brierley.FrameWork.Common.Threading;
using Brierley.FrameWork.Messaging.Contracts;

namespace Brierley.FrameWork.Messaging.Config
{
    public class EndPointCfg
    {
        public enum EndPointDirection { Incoming, Outgoing }
        public string FactoryTypeName { get; set; }
        public string FactoryAssemblyName { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public bool Public { get; set; }
        public EndPointDirection Direction { get; set; }
        public bool Transactional { get; set; }
        public int NumberOfThreads { get; set; }
        public int PollingTimeout { get; set; }
        public int TransactionTimeout { get; set; }
        public string MessageName { get; set; }
        public int RetryCount { get; set; }
        public int RetryTimeout { get; set; }
        public MessageCfg MessageCfg { get; set; }
        public object Queue { get; set; }
        public IConsumerFactory ConsumerFactory { get; set; }
        public MessageListener Listener { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
        public string AwsRegion { get; set; }

        public EndPointCfg()
        {
            FactoryTypeName = "Brierley.FrameWork.Messaging.Msmq.MsmqTransportFactory";
            FactoryAssemblyName = "brierley.framework.dll";
        }

    }
}
