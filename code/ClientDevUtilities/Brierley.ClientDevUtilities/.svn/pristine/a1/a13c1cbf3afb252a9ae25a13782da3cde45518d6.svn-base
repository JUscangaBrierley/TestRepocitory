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
	/// Represents the message content structure in the Teradata DMC system.
	/// Used to serialize message content when sending triggered emails.
	/// </summary>
	[Serializable]
	public class MessageContent
	{
		/// <summary>
		/// Gets or sets the list of personalizations for the message. 
		/// </summary>
		[DataMember(Name = "parameters")]
		[JsonProperty(PropertyName = "parameters")]
		public List<Attribute> Personalizations { get; set; }

		/// <summary>
		/// Gets or sets the list of attachments for the message.
		/// </summary>
		/// <remarks>
		/// Attachments are not supported in this version. Therefore, the property is defined as an
		/// object and will be serialized to JSON with a null value. The actual structure for this 
		/// class is string name, string contentType, string content
		/// </remarks>
		[DataMember(Name = "attachments")]
		[JsonProperty(PropertyName = "attachments")]
		public object Attachments { get; set; }
	}
}