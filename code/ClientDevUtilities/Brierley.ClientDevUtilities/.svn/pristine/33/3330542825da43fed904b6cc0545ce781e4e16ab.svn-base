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
	/// According to DMC, the "base type for all exceptions representing unexpected error conditions."
	/// </summary>
	public class UnexpectedErrorException : DmcException
	{
		/// <summary>
		/// Gets or sets the Id of the unexpected error.
		/// </summary>
		[DataMember(Name = "errorId")]
		[JsonProperty(PropertyName = "errorId")]
		public string ErrorId { get; set; }

		public UnexpectedErrorException()
		{
			ErrorCode = Dmc.ErrorCode.UNEXPECTED_ERROR;
		}

		public UnexpectedErrorException(string message, JObject json)
			: base(message, json)
		{
			ErrorCode = Dmc.ErrorCode.UNEXPECTED_ERROR;
			if (json != null && json["errorId"] != null)
			{
				ErrorId = json["errorId"].ToString();
			}
		}
	}
}
