//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Xml.Linq;

namespace Brierley.FrameWork.Data.DomainModel
{
	public class FieldCollection
	{
		private IList<Field> _fields;

		public FieldCollection(string xml)
		{
			_fields = Deserialize(xml);
		}

		public string Serialize()
		{
			XElement e = new XElement("fields");
			foreach (Field field in _fields)
			{
				e.Add(field.Serialize());
			}

			return e.ToString();
		}

		private IList<Field> Deserialize(string xml)
		{
			IList<Field> fields = new List<Field>();

			if (!string.IsNullOrEmpty(xml))
			{
				var e = XElement.Parse(xml);
				foreach (var child in e.Elements())
				{
					if (child.NodeType == XmlNodeType.Element)
					{
						Field field = Field.Deserialize(child);
						if (field != null)
						{
							fields.Add(field);
						}
					}
				}
			}
			return fields;
		}

		public Field GetField(string id)
		{
			Field field = null;
			foreach (Field f in _fields)
			{
				if (f.Id == id)
				{
					field = f;
					break;
				}
			}
			return field;
		}

		public Field GetField(int idx)
		{
			return _fields[idx];
		}

		public Field GetFieldByName(string fieldname)
		{
			Field field = null;
			foreach (Field f in _fields)
			{
				if (f.Name == fieldname)
				{
					field = f;
					break;
				}
			}
			return field;
		}

		public int CountFields()
		{
			return _fields.Count();
		}

		public void AddField(Field field)
		{
			_fields.Add(field);
		}

		public void RemoveField(string id)
		{
			Field field = GetField(id);
			if (field != null)
			{
				_fields.Remove(field);
			}
		}

		public List<string> FieldNames
		{
			get
			{
				List<string> names = new List<string>();
				foreach (Field f in _fields)
				{
					names.Add(f.Name);
				}
				return names;
			}
		}

		public IList<Field> Fields
		{
			get
			{
				return _fields;
			}
		}

		/*
		 * logic:
		 * 1. If all fields have values then generate list with those values.
		 * 2. If some fields have values and there are some fields that do not have values 
		 *    and do not allow blanks, then throw an exception.
		 * 3. If none of the fields have values then generate an empty row.		  
		 * */
		public List<Dictionary<string, string>> GenerateList()
		{
			//make sure values exist for all fields that require it
			if (_fields.Count(o => !o.AllowBlank && o.CountValues() == 0) > 0)
			{
				throw new Brierley.FrameWork.Common.Exceptions.LWException("Please provide values for fields that do not allow blanks.");
			}
			var permutations = new Dictionary<string, List<string>>();
			foreach (Field field in _fields)
			{
				if (field.CountValues() > 0)
				{
					var values = new List<string>();
					foreach (FieldValue v in field.GetValues())
					{
						values.Add(v.Value.ToString());
					}
					permutations.Add(field.Name, values);
				}
				else
				{
					permutations.Add(field.Name, new List<string>());
				}
			}

			return Combine(permutations);
		}

		private List<Dictionary<string, string>> Combine(Dictionary<string, List<string>> permutations)
		{
			long rows = 1;
			foreach (var list in permutations)
			{
				rows *= Math.Max(1, list.Value.Count());
			}

			if (rows > int.MaxValue)
			{
				throw new Exception(string.Format("Cannot generate test list - too many rows ({0})", rows));
			}

			int rowCount = (int)rows;

			var ret = new List<Dictionary<string, string>>(rowCount);
			for (int i = 0; i < rows; i++)
			{
				ret.Add(new Dictionary<string, string>());
			}

			int repeats = rowCount;
			foreach (var p in permutations)
			{
				if (p.Value.Count == 0)
				{
					p.Value.Add(string.Empty);
				}
				repeats /= p.Value.Count;
				for (int i = 0; i < rowCount; i++)
				{
					ret[i][p.Key] = p.Value[i / repeats % p.Value.Count];
				}
			}
			return ret;
		}
	}
}
