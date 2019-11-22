using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Brierley.FrameWork.Dmc.Exceptions
{
	/// <summary>
	/// Base class for DMC object exceptions (currently ObjectAlreadyExistsException and NoSuchObjectException)
	/// </summary>
	public class ObjectException : DmcException
	{
		/// <summary>
		/// Gets or sets the object type.
		/// </summary>
		[DataMember(Name = "objectType")]
		[JsonProperty(PropertyName = "objectType")]
		public string ObjectType { get; set; }

		/// <summary>
		/// Gets or sets the name of the property that was used to look up the object.
		/// </summary>
		[DataMember(Name = "propertyName")]
		[JsonProperty(PropertyName = "propertyName")]
		public string PropertyName { get; set; }

		/// <summary>
		/// Gets or sets the value of the property that was used to look up the object.
		/// </summary>
		[DataMember(Name = "propertyValue")]
		[JsonProperty(PropertyName = "propertyValue")]
		public string PropertyValue { get; set; }
		
		public ObjectException(string message, JObject json)
			: base(message, json)
		{
			if (json["objectType"] != null)
			{
				ObjectType = json["objectType"].ToString();
			}

			if (json["propertyName"] != null)
			{
				PropertyName = json["propertyName"].ToString();
			}

			if (json["propertyValue"] != null)
			{
				PropertyValue = json["propertyValue"].ToString();
			}
		}
	}
}
