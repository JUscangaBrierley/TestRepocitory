using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Brierley.FrameWork.Dmc
{
	/// <summary>
	/// Represents an attribute in the DMC system.
	/// </summary>
	[Serializable]
	public class Attribute
	{
		/// <summary>
		/// Gets or sets the name of the attribute.
		/// </summary>
		[DataMember(Name = "name")]
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the value of the attribute. 
		/// </summary>
		[DataMember(Name = "value")]
		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; }

		public Attribute()
		{
		}

		public Attribute(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}
