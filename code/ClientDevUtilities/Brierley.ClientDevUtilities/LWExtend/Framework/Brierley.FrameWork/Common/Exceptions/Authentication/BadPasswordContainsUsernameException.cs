using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions.Authentication
{
	public class BadPasswordContainsUsernameException : AuthenticationException
	{
		public BadPasswordContainsUsernameException() : base("Password may not contain the username.") { }
	}
}
