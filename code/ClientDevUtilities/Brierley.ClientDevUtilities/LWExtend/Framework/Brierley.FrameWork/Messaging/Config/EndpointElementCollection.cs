using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Messaging.Config
{
    [ConfigurationCollection(typeof(EndpointElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class EndpointElementCollection : ConfigurationElementCollection
    {
		public void Add(EndpointElement endpointElement)
		{
			BaseAdd(endpointElement);
		}
		
		protected override ConfigurationElement CreateNewElement()
        {
            return new EndpointElement();
        }

        protected override object GetElementKey(ConfigurationElement configElement)
        {
			var endpointElement = (EndpointElement)configElement;
			return endpointElement.Name;
        }
    }
}
