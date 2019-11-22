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
	public class InvalidParameterException : DmcException
	{
		/// <summary>
		/// Gets or sets the name of the invalid parameter
		/// </summary>
		/// <remarks>
		/// From DMC documentation: For list parameters, the offset of the invalid element within the list is specified 
		/// as a suffix. The suffix is the list offset (starting with zero for the first element) in square brackets.
		/// </remarks>
		[DataMember(Name = "parameterName")]
		[JsonProperty(PropertyName = "parameterName")]
		public string ParameterName { get; set; }

		/// <summary>
		/// Gets or sets the name of the invalid property.
		/// </summary>
		/// <remarks>
		/// Per the documentation, this is only applies to complex types.
		/// </remarks>
		[DataMember(Name = "propertyName")]
		[JsonProperty(PropertyName = "propertyName")]
		public string PropertyName { get; set; }

		/// <summary>
		/// Gets or sets the value of the property that is invalid.
		/// </summary>
		[DataMember(Name = "value")]
		[JsonProperty(PropertyName = "value")]
		public string Value { get; set; }

		public InvalidParameterException()
		{
			ErrorCode = Dmc.ErrorCode.INVALID_PARAMETER;
		}

		public InvalidParameterException(string message, JObject json)
			: base(message, json)
		{
			ErrorCode = Dmc.ErrorCode.INVALID_PARAMETER;
			if (json != null)
			{
				if (json["parameterName"] != null)
				{
					ParameterName = json["parameterName"].ToString();
				}
				if (json["propertyName"] != null)
				{
					PropertyName = json["propertyName"].ToString();
				}
				if (json["value"] != null)
				{
					Value = json["value"].ToString();
				}
			}
		}
	}
}
