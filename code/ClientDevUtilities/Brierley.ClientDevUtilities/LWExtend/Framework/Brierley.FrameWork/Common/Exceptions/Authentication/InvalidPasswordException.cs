using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions.Authentication
{
	public class InvalidPasswordException : AuthenticationException
	{
		public InvalidPasswordException()
			: base("Provided password is incorrect.")
		{
		}
	}
}
