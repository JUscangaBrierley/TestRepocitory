using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Messaging.Config
{
    public class MessageCfg
    {
        public string Name { get; set; }
        public string MessageTypeName { get; set; }
        public string MessageAssemblyName { get; set; }
        public Type Type { get; set; }
        public IList<EndPointCfg> EndPointCfgs { get; set; }
        public ConsumerCfg ConsumerCfg { get; set; }
    }
}
