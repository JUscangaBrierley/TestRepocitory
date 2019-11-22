using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions.Authentication
{
	public class AuthenticationExpiredException : AuthenticationException
	{
		public AuthenticationExpiredException(string message) 
			: base(message) 
		{ 
		}
	}
}
