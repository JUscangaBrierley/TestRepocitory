using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.FrameWork.Dmc
{
	/// <summary>
	/// User represents a user in the Teradata DMC system. 
	/// It is used to deserialize a user retrieved by the /user/getByEmail API method.
	/// </summary>
	[DataContract]
	public class User
	{
		/// <summary>
		/// Gets or sets DMC's unique identifier. 
		/// </summary>
		/// <remarks>
		/// This property is used to identify the user in DMC, and its value is passed as 
		/// the recipientId parameter in the /message/sendSingle API call.
		/// </remarks>
		[DataMember(Name = "id")]
		public long Id { get; set; }

		/// <summary>
		/// Gets or sets the user's name.
		/// </summary>
		[DataMember(Name = "email")]
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the user's mobile number.
		/// </summary>
		[DataMember(Name = "mobileNumber")]
		public string MobileNumber { get; set; }

		/// <summary>
		/// Gets or sets the user's identifier.
		/// </summary>
		/// <remarks>
		/// This is an optional identifier that is set by B+P and is external to DMC.
		/// For example, this property could be used to store the member's IPCode.
		/// </remarks>
		[DataMember(Name = "identifier")]
		public string Identifier { get; set; }
	}
}
