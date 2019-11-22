using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Messaging.Config
{
    public class ConsumerCfg
    {
        public string Name { get; set; }
        public string FactoryTypeName { get; set; }
        public string FactoryAssemblyName { get; set; }
        public Type Type { get; set; }
        public string MessageName { get; set; }
        public ConsumerLifecyclePolicy LifecyclePolicy { get; set; }
        public int ConsumerPoolSize { get; set; } // only applicable if life cycle is consumer pool
    }
}
