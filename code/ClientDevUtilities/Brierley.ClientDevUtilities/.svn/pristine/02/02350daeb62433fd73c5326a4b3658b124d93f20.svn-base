using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Messaging.Config
{
    [ConfigurationCollection(typeof(ConsumerElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ConsumerElementCollection : ConfigurationElementCollection
    {
		public void Add(ConsumerElement consumerElement)
		{
			BaseAdd(consumerElement);
		}
		
		protected override ConfigurationElement CreateNewElement()
        {
            return new ConsumerElement();
        }

        protected override object GetElementKey(ConfigurationElement configElement)
        {
			return ((ConsumerElement)configElement).Name;
        }
    }
}
