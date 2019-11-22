using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Common.Exceptions.Authentication
{
	public class AuthenticationException : LWException
	{
		public AuthenticationException(string message) 
			: base(message) 
		{ 
		}

		public AuthenticationException(string message, Exception exception)
            : base(message, exception)
        {
        }
	}
}
