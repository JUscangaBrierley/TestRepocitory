using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;

namespace Brierley.FrameWork.Common.Extensions
{
	public static class XmlExtensions
	{
		public static string AttributeValue(this XElement element, string attribute)
		{
			return AttributeValue(element, attribute, string.Empty);
		}

		public static string AttributeValue(this XElement element, string attribute, string defaultValue)
		{
			if (element != null && element.Attribute(attribute) != null)
			{
				return element.Attribute(attribute).Value;
			}
			return defaultValue;
		}


		public static string InnerXml(this XElement element)
		{
			StringBuilder innerXml = new StringBuilder();
			foreach (XNode node in element.Nodes())
			{
				innerXml.Append(node.ToString());
			}
			return innerXml.ToString();
		}

        public static XElement FindElementWithAttribute(XElement parent, string nodeName, string attributeName, string attributeValue)
        {
            XElement e = null;
            IEnumerable<XElement> elements = from el in parent.Elements(nodeName) where (string)el.Attribute(attributeName) == attributeValue select el;
            foreach (XElement el in elements)
            {
                e = el;
                break;
            }
            return e;
        }

        public static XElement NSElement(this XElement element, string name)
        {
            if (string.IsNullOrEmpty(element.Name.NamespaceName))
            {
                return element.Element(name);
            }
            else
            {
                return element.Element(element.Name.Namespace+name);
            }
        }

        public static IEnumerable<XElement> NSElements(this XElement element, string name)
        {
            if (string.IsNullOrEmpty(element.Name.NamespaceName))
            {
                return element.Elements(name);
            }
            else
            {
                return element.Elements(element.Name.Namespace + name);
            }
        }

        /// <summary>
        /// The node path is of the form Member/IpCode or Member/VirtualCard/LoyaltyIdNumber.
        /// The last component of the path can either be an attribuute or an element.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetNodeValueByPath(XElement node, string path)
        {
            string value = string.Empty;
            string[] tokens = path.Split('/');
            if (tokens.Length == 1)
            {
                // on the current node.
                XElement target = node.Element(path);
                if (target == null)
                {
                    XAttribute att = node.Attribute(path);
                    if (att != null)
                    {
                        value = att.Value;
                    }
                }
                else
                {
                    value = target.Value;
                }
            }
            else if (tokens.Length == 2)
            {
                if (node.Name.LocalName == tokens[0])
                {
                    // on the current node.
                    XElement target = node.Element(tokens[1]);
                    if (target == null)
                    {
                        XAttribute att = node.Attribute(tokens[1]);
                        if (att != null)
                        {
                            value = att.Value;
                        }
                    }
                    else
                    {
                        value = target.Value;
                    }
                }
            }
            else
            {
                XElement nextNode = node.Element(tokens[1]);
                if (nextNode != null)
                {
                    int idx = path.IndexOf('/');
                    string newPath = path.Substring(idx + 1);
                    value = GetNodeValueByPath(nextNode, newPath);
                }
                else
                {
                    throw new Brierley.FrameWork.Common.Exceptions.LWException(
                        string.Format("Unable to find node {0} in {1}.  Cannot use path {2} to search.", tokens[1], node.ToString(), path)) { ErrorCode = 9993 };
                }
            }
            return value;
        }

        /// <summary>
        /// This method returns the list of nodes that match that path.  The first element in the path is expected
        /// to also be the root node of the element.  The last element
        /// in the path represents the XElement node that will be returned.
        /// e.g. For an xml node that starts with Member, the path will look like 
        /// Member/VirtualCard/TxnHeader/TxnDetailLineItem. 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IList<XElement> GetNodesByPath(this XElement element, string path)
        {
            IList<XElement> list = new List<XElement>();
            string[] tokens = path.Split('/');
            string xpath = path;            
            if (tokens[0] == element.Name.LocalName)
            {
                //xpath = path.Replace(tokens[0], ".");
                xpath = StringUtils.Replace(path, ".", 0, tokens[0].Length-1);
            }           
            if (!xpath.StartsWith("."))
            {
                xpath = "./" + xpath;
            }
            IEnumerable<XElement> temp = element.XPathSelectElements(xpath);
            if (temp != null)
            {
                foreach (XElement e in temp)
                {
                    list.Add(e);
                }
            }            
            return list;
        }
	}
}
