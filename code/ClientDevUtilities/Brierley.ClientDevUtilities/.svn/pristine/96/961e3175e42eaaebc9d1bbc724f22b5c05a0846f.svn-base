//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Brierley.FrameWork.Common.Extensions;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class FieldValue
	{
		public string Id { get; set; }
		public string Value { get; set; }

		public XElement Serialize()
		{
			XElement e = new XElement("fieldvalue",
				new XAttribute("id", Id),
				new XAttribute("value", Value ?? string.Empty)
				);

			return e;
		}

		public static FieldValue Deserialize(string field)
		{
			XElement e = XElement.Parse(field);
			return Deserialize(e);
		}

		public static FieldValue Deserialize(XElement e)
		{
			FieldValue val = null;
			if (e.Name.LocalName.Equals("fieldvalue", StringComparison.OrdinalIgnoreCase))
			{
				val = new FieldValue();

				if (e.Attribute("id") != null)
				{
					//new way
					val.Id = e.AttributeValue("id");
					val.Value = e.AttributeValue("value");
				}
				else
				{
					//old way, before LW 4.5
					foreach (var child in e.Nodes())
					{
						if (child.NodeType == XmlNodeType.Element)
						{
							if (((XElement)child).Name.LocalName == "ValueId")
							{
								val.Id = ((XElement)child).Value;
							}
							else if (((XElement)child).Name.LocalName == "Value")
							{
								val.Value = ((XElement)child).Value;
							}
						}
					}
				}
			}
			return val;
		}
	}
}
