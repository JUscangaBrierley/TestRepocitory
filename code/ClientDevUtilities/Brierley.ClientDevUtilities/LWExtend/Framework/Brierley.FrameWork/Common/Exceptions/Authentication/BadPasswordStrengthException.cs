using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions.Authentication
{
	public class BadPasswordStrengthException : AuthenticationException
	{
		public BadPasswordStrengthException() : base("Password must contain 3 or more categories: number, uppercase, lowercase, special character.") { }
	}
}
