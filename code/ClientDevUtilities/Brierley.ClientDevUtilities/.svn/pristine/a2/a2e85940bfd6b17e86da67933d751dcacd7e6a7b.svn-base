//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Reflection;
//using System.Xml;
//using System.Xml.Linq;

//using Brierley.FrameWork.Messaging.Exceptions;

//namespace Brierley.FrameWork.Messaging.Serializers
//{
//    public class XmlMessageSerializer : IMessageSerializers
//    {
//        #region Private Helpers
//        private IDictionary<string, XNamespace> GetNamespaces(object message)
//        {
//            var namespaces = new Dictionary<string, XNamespace>
//            {
//                {"msb", "http://brierley.loyaltyware.com/msb"},
//            };

//            var type = message.GetType();
//            var prefix = GetNamespacePrefixForXml(type);
//            Assembly assembly = type.Assembly;
//            string fullName = assembly.FullName ?? assembly.GetName().Name;
//            var ns = type.FullName + ", " + fullName.Split(',')[0];
//            namespaces[prefix] = ns;

//            return namespaces;
//        }

//        private string GetNamespacePrefixForXml(Type type)
//        {
//            return type.Namespace.Split('.')
//                          .Last().ToLowerInvariant() + "." + type.Name.ToLowerInvariant();
//        }

//        private string GetNamespaceForXml(Type type)
//        {
//            string value;
//            //if (typeToWellKnownTypeName.TryGetValue(type, out value))
//            //    return value;

//            Assembly assembly = type.Assembly;
//            string fullName = assembly.FullName ?? assembly.GetName().Name;
//            if (type.IsGenericType)
//            {
//                var builder = new StringBuilder();
//                int startOfGenericName = type.FullName.IndexOf('[');
//                builder.Append(type.FullName.Substring(0, startOfGenericName))
//                    .Append("[")
//                    .Append(String.Join(",",
//                                    type.GetGenericArguments()
//                                        .Select(t => "[" + GetNamespaceForXml(t) + "]")
//                                        .ToArray()))
//                    .Append("], ");
//                if (assembly.GlobalAssemblyCache)
//                {
//                    builder.Append(fullName);
//                }
//                else
//                {
//                    builder.Append(fullName.Split(',')[0]);
//                }
//                return builder.ToString();
//            }

//            if (assembly.GlobalAssemblyCache == false)
//            {
//                return type.FullName + ", " + fullName.Split(',')[0];
//            }
//            return type.AssemblyQualifiedName;
//        }

//        private string GetNameForXml(Type type)
//        {
//            var typeName = type.Name;
//            typeName = typeName.Replace('[', '_').Replace(']', '_');
//            var indexOf = typeName.IndexOf('`');
//            if (indexOf == -1)
//                return typeName;
//            typeName = typeName.Substring(0, indexOf) + "_of_";
//            foreach (var argument in type.GetGenericArguments())
//            {
//                typeName += GetNamespacePrefixForXml(argument) + "_";
//            }
//            return typeName.Substring(0, typeName.Length - 1);
//        }

//        //private void WriteObject(string name, object value, XContainer parent, IDictionary<string, XNamespace> namespaces)
//        //{
            
            
//        //    else if (ShouldPutAsString(value))
//        //    {
//        //        var elementName = GetXmlNamespace(namespaces, value.GetType()) + name;
//        //        parent.Add(new XElement(elementName, FormatAsString(value)));
//        //    }
//        //    else if (value is byte[])
//        //    {
//        //        var elementName = GetXmlNamespace(namespaces, typeof(byte[])) + name;
//        //        parent.Add(new XElement(elementName, Convert.ToBase64String((byte[])value)));
//        //    }
//        //    else if (ShouldTreatAsDictionary(value.GetType()))
//        //    {
//        //        XElement list = GetContentWithNamespace(value, namespaces, name);
//        //        parent.Add(list);
//        //        var itemCount = 0;
//        //        foreach (var item in ((IEnumerable)value))
//        //        {
//        //            if (item == null)
//        //                continue;
//        //            itemCount += 1;
//        //            if (itemCount > MaxNumberOfAllowedItemsInCollection)
//        //                throw new UnboundedResultSetException("You cannot send collections with more than 256 items (" + value + " " + name + ")");

//        //            var entry = new XElement("entry");
//        //            var keyProp = reflection.Get(item, "Key");
//        //            if (keyProp == null)
//        //                continue;
//        //            WriteObject("Key", keyProp, entry, namespaces);
//        //            var propVal = reflection.Get(item, "Value");
//        //            if (propVal != null)
//        //            {
//        //                WriteObject("Value", propVal, entry, namespaces);
//        //            }

//        //            list.Add(entry);
//        //        }
//        //    }
//        //    else if (value is IEnumerable)
//        //    {
//        //        XElement list = GetContentWithNamespace(value, namespaces, name);
//        //        parent.Add(list);
//        //        var itemCount = 0;
//        //        foreach (var item in ((IEnumerable)value))
//        //        {
//        //            if (item == null)
//        //                continue;
//        //            itemCount += 1;
//        //            if (itemCount > MaxNumberOfAllowedItemsInCollection)
//        //                throw new UnboundedResultSetException("You cannot send collections with more than 256 items (" + value + " " + name + ")");

//        //            WriteObject("value", item, list, namespaces);
//        //        }
//        //    }
//        //    else
//        //    {
//        //        XElement content = GetContentWithNamespace(value, namespaces, name);
//        //        foreach (var property in reflection.GetProperties(value))
//        //        {
//        //            var propVal = reflection.Get(value, property);
//        //            if (propVal == null)
//        //                continue;
//        //            WriteObject(property, propVal, content, namespaces);
//        //        }
//        //        content = ApplyMessageSerializationBehaviorIfNecessary(value.GetType(), content);
//        //        parent.Add(content);
//        //    }
//        //}
//        #endregion

//        #region Interface Methods
//        public void Serialize(object message, Stream messageStream)
//        {
//            var namespaces = GetNamespaces(message);
//            var messagesElement = new XElement(namespaces["msb"] + "messages");
//            var xml = new XDocument(messagesElement);

//            try
//            {
//                //WriteObject(GetNameForXml(message.GetType()), message, messagesElement, namespaces);
//            }
//            catch (Exception e)
//            {
//                throw new LWSerializationException("Could not serialize " + message.GetType() + ".", e);
//            }

//            messagesElement.Add(
//                namespaces.Select(x => new XAttribute(XNamespace.Xmlns + x.Key, x.Value))
//                );

//            var streamWriter = new StreamWriter(messageStream);
//            var writer = XmlWriter.Create(streamWriter, new XmlWriterSettings
//            {
//                Indent = true,
//                Encoding = Encoding.UTF8
//            });
//            if (writer == null)
//                throw new InvalidOperationException("Could not create xml writer from stream");

//            xml.WriteTo(writer);
//            writer.Flush();
//            streamWriter.Flush();
//        }
//        #endregion
//    }
//}
