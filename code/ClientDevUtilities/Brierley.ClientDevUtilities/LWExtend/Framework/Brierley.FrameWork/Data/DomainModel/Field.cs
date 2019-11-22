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
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Extensions;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class Field
	{
		private IList<FieldValue> _values = new List<FieldValue>();

		public string Id { get; set; }
		public string Name { get; set; }
		public string Expression { get; set; }
		public string Description { get; set; }
		public string DefaultValue { get; set; }
		public bool AllowBlank { get; set; }
		public bool UseCDATA { get; set; }
		public bool Reportable { get; set; }

		public static Field Deserialize(string s)
		{
			XElement e = XElement.Parse(s);
			return Deserialize(e);
		}

		public static Field Deserialize(XElement e)
		{
			Field field = null;
			if (e.Name.LocalName == "field")
			{
				//new way
				field = new Field();
				field.Id = e.AttributeValue("id");
				field.Name = e.AttributeValue("name");
				field.Expression = e.AttributeValue("expression", null);
				field.Description = e.AttributeValue("description", null);
				field.DefaultValue = e.AttributeValue("default", null);
				field.AllowBlank = bool.Parse(e.AttributeValue("allowblank"));
				field.UseCDATA = bool.Parse(e.AttributeValue("usecdata"));
				field.Reportable = bool.Parse(e.AttributeValue("reportable", "false"));

				var values = e.Element("values");
				foreach (var node in values.Elements())
				{
					FieldValue val = FieldValue.Deserialize(node);
					if (val != null)
					{
						field._values.Add(val);
					}
				}
			}
			else if (e.Name.LocalName == "Field")
			{
				//old way
				field = new Field();
				foreach (var c in e.Nodes())
				{
					if (c.NodeType == XmlNodeType.Element)
					{
						XElement child = (XElement)c;
						if (child.Name.LocalName == "FieldId")
						{
							field.Id = child.Value;
						}
						else if (child.Name.LocalName == "FieldName")
						{
							field.Name = child.Value;
						}
						else if (child.Name.LocalName == "FieldExpression")
						{
							field.Expression = child.Value;
						}
						else if (child.Name.LocalName == "FieldDescription")
						{
							field.Description = child.Value;
						}
						else if (child.Name.LocalName == "FieldDefault")
						{
							field.DefaultValue = child.Value;
						}
						else if (child.Name.LocalName == "FieldAllowBlank")
						{
							field.AllowBlank = StringUtils.FriendlyBool(child.Value, true);
						}
						else if (child.Name.LocalName == "FieldUseCDATA")
						{
							field.UseCDATA = StringUtils.FriendlyBool(child.Value, false);
						}
						else if (child.Name.LocalName == "FieldValues")
						{
							foreach (var vNode in child.Nodes())
							{
								FieldValue val = FieldValue.Deserialize((XElement)vNode);
								if (val != null)
								{
									field._values.Add(val);
								}
							}
						}
					}
				}
			}

			return field;
		}

		public XElement Serialize()
		{
			var e = new XElement(
				"field",
				new XAttribute("id", Id),
				new XAttribute("name", Name),
				new XAttribute("allowblank", AllowBlank),
				new XAttribute("usecdata", UseCDATA),
				new XAttribute("reportable", Reportable));

			if (!string.IsNullOrEmpty(Expression))
			{
				e.Add(new XAttribute("expression", Expression));
			}

			if (!string.IsNullOrEmpty(Description))
			{
				e.Add(new XAttribute("description", Description));
			}

			if (!string.IsNullOrEmpty(DefaultValue))
			{
				e.Add(new XAttribute("default", DefaultValue));
			}

			XElement values = new XElement("values");
			foreach (FieldValue value in _values)
			{
				values.Add(value.Serialize());
			}
			e.Add(values);

			return e;
		}

		public FieldValue GetValue(string id)
		{
			FieldValue value = null;
			foreach (FieldValue v in _values)
			{
				if (v.Id == id)
				{
					value = v;
					break;
				}
			}
			return value;
		}

		public FieldValue GetValue(int idx)
		{
			return _values[idx];
		}

		public IList<FieldValue> GetValues()
		{
			return _values;
		}

		public void AddFieldValue(string value)
		{
			FieldValue v = new FieldValue();
			v.Id = Guid.NewGuid().ToString();
			v.Value = value;
			_values.Add(v);
		}

		public void UpdateFieldValue(string id, string value)
		{
			FieldValue v = GetValue(id);
			if (v != null)
			{
				v.Value = value;
			}
		}

		public void RemoveFieldValue(string id)
		{
			FieldValue v = GetValue(id);
			if (v != null)
			{
				_values.Remove(v);
			}
		}

		public void ClearValues()
		{
			_values.Clear();
		}

		public int CountValues()
		{
			return _values.Count;
		}
	}
}
