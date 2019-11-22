//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.Xml.Linq;

namespace Brierley.FrameWork.Common.XML
{
    // TODO: Consider switching to System.Xml.Linq  - It might be more efficient.
    public static class XmlUtils
    {
        #region Private Helper Methods
        private static XmlNode SelectSingleNode(XmlNode node, int index, List<string> paths)
        {
            XmlNode respNode = null;
            string nodeToSearch = paths[index];
            if (node.LocalName == nodeToSearch)
            {
                respNode = node;
            }
            else
            {
                foreach (XmlNode cnode in node.ChildNodes)
                {
                    if (cnode.NodeType != XmlNodeType.Element)
                        continue;
                    if (cnode.LocalName == nodeToSearch)
                    {
                        respNode = cnode;
                        break;
                    }
                }                
            }
            if (index != paths.Count - 1 && respNode != null)
            {
                respNode = SelectSingleNode(respNode, index+1, paths);
            }
            return respNode;
        }

        private static XmlNode LocateNode(XmlNode axis, ref int index, List<string> pathList, ref bool isAttribute)
        {
            XmlNode respNode = null;
            isAttribute = false;
            string nodeToSearch = pathList[index++];
            if (axis.LocalName == nodeToSearch)
            {
                respNode = axis;
            }
            else
            {
                foreach (XmlNode cnode in axis.ChildNodes)
                {
                    if (cnode.NodeType != XmlNodeType.Element)
                        continue;
                    if (cnode.LocalName == nodeToSearch)
                    {
                        respNode = cnode;
                        break;
                    }
                }
            }
            if (respNode == null && index == pathList.Count)
            {
                // This might be an attribute.
                if (axis.Attributes[nodeToSearch] != null)
                {
                    respNode = axis;
                    isAttribute = true;
                }
            }
            else if (respNode != null && index != pathList.Count)
            {
                respNode = LocateNode(respNode, ref index, pathList, ref isAttribute);
            }
            return respNode;
        }
        #endregion

        #region Public Methods

        #region Creation

        public static XmlNode CreateNewNode(XmlDocument doc, string elementName, string nsprefix, string ns)
        {
            XmlNode node = doc.CreateElement(nsprefix, elementName, ns);
            return node;
        }

        public static XmlNode CreateNewNode(XmlDocument doc, string elementName, string ns)
        {           
            XmlNode node = doc.CreateElement(doc.Prefix, elementName, ns);
            return node;
        }

        public static XmlNode CreateNewNode(XmlDocument doc, string elementName)
        {
            XmlNode node = null;
            if (!string.IsNullOrEmpty(doc.Prefix) && !string.IsNullOrEmpty(doc.NamespaceURI))
            {
                node = doc.CreateElement(doc.Prefix, elementName, doc.NamespaceURI);
            }
            else
            {
                node = doc.CreateElement(elementName);
            }
            return node;
        }
        public static void SetAttribute(string name, string val, XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node", "Specify non-null node.");
            }
            ((XmlElement)node).SetAttribute(name, node.NamespaceURI, val);
        }
        #endregion

        #region Selection

        //public static XmlNodeList SelectNodes(XElement node, string xpath)
        //{
        //    XElement e = XElement.Parse(node.OuterXml);
        //    var test = e.Elements(xpath);
        //    return null;

        //}

        public static XmlNode SelectSingleNode(XmlNode node,string xpath)
        {
            // first try the xml way.            
            XmlNode respNode = node.SelectSingleNode(xpath);
            if (respNode == null)
            {
                string[] paths = xpath.Split('/');
                if (paths != null && paths.Length > 0)
                {
                    List<string> pathList = new List<string>(paths);
                    int index = 0;
                    respNode = SelectSingleNode(node, index, pathList);
                }
            }
            return respNode;
        }

        public static XmlNode SelectSingleNode(XmlDocument doc,string xpath)
        {            
            // first try the xml way.
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("xxx", doc.NamespaceURI);
            string expr = string.Format("//xxx:{0}", xpath);
            XmlNode respNode = null;
            try
            {
                respNode = doc.DocumentElement.SelectSingleNode(expr, nsmgr);
            }
            catch
            {
            }
            if (respNode == null)
            {
                respNode = doc.DocumentElement.SelectSingleNode(xpath);
                if (respNode == null)
                {
                    // now try the hard way
                    respNode = SelectSingleNode(doc.DocumentElement, xpath);
                }
            }
            return respNode;            
        }
        
        public static string SelectSingleNodeValue(XmlNode searchRoot, string xpath)
        {
            if ( string.IsNullOrEmpty(xpath) )
                return string.Empty;

            string value = string.Empty;            
            if (xpath.EndsWith("/"))
            {
                xpath = xpath.Substring(0, xpath.Length - 1);
            }
            string[] paths = xpath.Split('/');
            List<string> pathList = new List<string>(paths);
            int index = 0;
            bool isAttribute = false;
            XmlNode respNode = LocateNode(searchRoot, ref index, pathList, ref isAttribute);
            if (respNode != null )
            {
                if (isAttribute)
                {
                    XmlAttribute att = respNode.Attributes[pathList[index-1]];
                    if (att != null)
                    {
                        value = att.Value;
                    }
                }
                else
                {
                    value = respNode.OuterXml;
                }                
            }
            return value;
        }

        public static string SelectSingleNodeValue(XmlDocument doc, string xpath)
        {
            if (doc == null)
            {
                throw new ArgumentNullException("doc", "Specify non-null parameter.");
            }
            if (xpath == null)
            {
                throw new ArgumentNullException("xpath", "Specify non-null parameter.");
            }
            return SelectSingleNodeValue(doc.DocumentElement, xpath);
        }
        #endregion

        #endregion
    }
}
