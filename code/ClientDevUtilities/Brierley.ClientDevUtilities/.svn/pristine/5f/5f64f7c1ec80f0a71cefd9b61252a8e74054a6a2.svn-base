using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Brierley.FrameWork.Dmc.Exceptions
{
	/// <summary>
	/// An exception thrown when attempting to create an object that already exists.
	/// </summary>
	public class ObjectAlreadyExistsException : ObjectException
	{
		public ObjectAlreadyExistsException(string message, JObject json)
			: base(message, json)
		{
			ErrorCode = Dmc.ErrorCode.OBJECT_ALREADY_EXISTS;
		}
	}
}
