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
	/// An exception thrown by DMC when an object is not found.
	/// </summary>
	public class NoSuchObjectException : ObjectException
	{
		public NoSuchObjectException(string message, JObject json)
			: base(message, json)
		{
			ErrorCode = Dmc.ErrorCode.NO_SUCH_OBJECT;
		}
	}
}
