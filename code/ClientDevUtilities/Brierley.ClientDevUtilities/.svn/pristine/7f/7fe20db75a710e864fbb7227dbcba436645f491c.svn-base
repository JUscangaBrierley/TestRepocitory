using System;
using System.Configuration;

namespace Brierley.FrameWork.Messaging.Config
{
    [ConfigurationCollection(typeof(MessageElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class MessageElementCollection : ConfigurationElementCollection
    {
		public void Add(MessageElement messageElement)
		{
			BaseAdd(messageElement);
		}
		
		protected override ConfigurationElement CreateNewElement()
        {
            return new MessageElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var messageElement = (MessageElement)element;
			return messageElement.MessageTypeName + messageElement.MessageAssemblyName + messageElement.Name;
        }
    }
}
